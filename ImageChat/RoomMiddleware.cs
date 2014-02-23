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

    struct WebSocketData 
    {
        public string Text;
        public WebSocketReceiveResult RecievedResult;
    }
    class ChatText
    {
        public string Color;
        public string Text;
    }
    class RoomMiddleware
    {
        readonly PathString _pathString;
        readonly AppFunc _next;
        readonly Room _room;
        readonly Queue<ChatText> _texts = new Queue<ChatText>();
        readonly TemplateService _templateService = new TemplateService();
        readonly Subject<WebSocketData> notifyAll = new Subject<WebSocketData>();
        public RoomMiddleware(AppFunc next, string path)
        {
            _next = next;
            _pathString = new PathString(path);
            _templateService.AddNamespace("ImageChat");
            _room = (new Room("ニコニコ", "http://deliver.commons.nicovideo.jp/thumbnail/nc19665?size=l"));
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path;

            PathString remainingPath;
            if (!path.StartsWithSegments(_pathString, out remainingPath))
            {
                await _next(environment);
                return;
            }
            var accept = context.Get<WebSocketAccept>("websocket.Accept");
            if (accept == null)
            {
                //ページを表示
                context.Response.Headers.Add("Content-type", new[] { "text/html" });
                await context.Response.WriteAsync(_templateService.Display("Room.cshtml", new { _room.ImageUrl, Texts = _texts.Reverse(), Program.Host }, null, null));
                return;
            }
            accept(null, WebSocketChat);
        }
        private async Task WebSocketChat(IDictionary<string, object> websocketContext)
        {
            var sendAsync = (WebSocketSendAsync)websocketContext["websocket.SendAsync"];
            var receiveAsync = (WebSocketReceiveAsync)websocketContext["websocket.ReceiveAsync"];
            var closeAsync = (WebSocketCloseAsync)websocketContext["websocket.CloseAsync"];
            var callCancelled = (CancellationToken)websocketContext["websocket.CallCancelled"];

            var buffer = new byte[4096];

            var disposable = notifyAll.Subscribe(async ( message) =>
            {
                var data = Encoding.UTF8.GetBytes(message.Text);
                await sendAsync(new ArraySegment<byte>(data, 0, data.Length), message.RecievedResult.Item1, message.RecievedResult.Item2, callCancelled);
            });
            object status;
            while (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out status) || (int)status == 0)
            {
                var received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
                var text = Encoding.UTF8.GetString(buffer, 0, received.Item3);
                var chatText = JsonConvert.DeserializeObject<ChatText>(text);
                _texts.Enqueue(chatText);
                while (_texts.Count > 100)
                    _texts.Dequeue();
                notifyAll.OnNext(new WebSocketData { Text = text, RecievedResult = received });
            }
            disposable.Dispose();
            await closeAsync((int)websocketContext["websocket.ClientCloseStatus"], (string)websocketContext["websocket.ClientCloseDescription"], callCancelled);
        }
    }
}
