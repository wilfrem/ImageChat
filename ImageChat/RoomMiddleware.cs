using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using RazorEngine.Templating;

namespace ImageChat
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using WebSocketAccept = Action<IDictionary<string, object>, // options
        Func<IDictionary<string, object>, Task>>; // callback
    using WebSocketCloseAsync =
        Func<int /* closeStatus */,
            string /* closeDescription */,
            CancellationToken /* cancel */,
            Task>;
    using WebSocketReceiveAsync =
        Func<ArraySegment<byte> /* data */,
            CancellationToken /* cancel */,
            Task<Tuple<int /* messageType */,
                bool /* endOfMessage */,
                int /* count */>>>;
    using WebSocketSendAsync =
        Func<ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            CancellationToken /* cancel */,
            Task>;
    using WebSocketReceiveResult = Tuple<int, // type
        bool, // end of message?
        int>; // count

    internal class RoomMiddleware
    {
        private readonly PathString _pathString;
        private readonly AppFunc _next;
        private readonly Room _room;
        private readonly TemplateService _templateService = new TemplateService();
        private readonly Subject<ChatEchoData> _notifyAll = new Subject<ChatEchoData>();

        public RoomMiddleware(AppFunc next, string path)
        {
            _next = next;
            _pathString = new PathString(path);
            _templateService.AddNamespace("ImageChat");
            _room = (new Room(Program.Host, "http://deliver.commons.nicovideo.jp/thumbnail/nc19665?size=l"));
            _notifyAll.Subscribe(data =>
            {
                var chatText = JsonConvert.DeserializeObject<ChatText>(data.Text);
                _room.Texts.Enqueue(chatText);
                while (_room.Texts.Count > 100)
                    _room.Texts.Dequeue();
            });
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path;

            var accept = context.Get<WebSocketAccept>("websocket.Accept");
            if (accept != null)
            {
                accept(null, WebSocketChat);
                return;
            }
            
            PathString remainingPath;
            if (!path.StartsWithSegments(_pathString, out remainingPath))
            {
                await _next(environment);
                return;
            }
            if (remainingPath.StartsWithSegments(new PathString("/god")))
            {
                context.Response.Headers.Add("Content-type", new[] {"text/plain; charset=utf-8"});
                var text = context.Request.Query.Get("text");
                if (text != null)
                {
                    var jsonData =
                        JsonConvert.SerializeObject(new ChatText
                        {
                            IsGod = true,
                            Color = "#FF0000",
                            Text = "神は言っている、" + text + " と"
                        });
                    _notifyAll.OnNext(new ChatEchoData(jsonData, 1, true));
                    await context.Response.WriteAsync("神は言っている、 送信が完了したと");
                    return;
                }
                await context.Response.WriteAsync("そんなクエリで大丈夫か\ntext=[メッセージ]で頼む");
                return;
            }
            //ページを表示
            context.Response.Headers.Add("Content-type", new[] {"text/html; charset=utf-8"});
            await context.Response.WriteAsync(_templateService.Display("Room.cshtml", _room, null, null));
        }



        private async Task WebSocketChat(IDictionary<string, object> websocketContext)
        {
            var sendAsync = (WebSocketSendAsync) websocketContext["websocket.SendAsync"];
            var receiveAsync = (WebSocketReceiveAsync) websocketContext["websocket.ReceiveAsync"];
            var closeAsync = (WebSocketCloseAsync) websocketContext["websocket.CloseAsync"];
            var callCancelled = (CancellationToken) websocketContext["websocket.CallCancelled"];

            var buffer = new byte[4096];

            var disposable = _notifyAll.Subscribe(async message =>
            {
                var data = Encoding.UTF8.GetBytes(message.Text);
                await
                    sendAsync(new ArraySegment<byte>(data, 0, data.Length), message.MessaageType, message.EndOfMessage,
                        callCancelled);
            });
            object status;
            while (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out status) || (int) status == 0)
            {
                var received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
                var text = Encoding.UTF8.GetString(buffer, 0, received.Item3);
                _notifyAll.OnNext(new ChatEchoData(text, received.Item1, received.Item2));
            }
            disposable.Dispose();
            await
                closeAsync((int) websocketContext["websocket.ClientCloseStatus"],
                    (string) websocketContext["websocket.ClientCloseDescription"], callCancelled);
        }
    }
}