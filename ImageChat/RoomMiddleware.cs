using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin;
using RazorEngine;
using RazorEngine.Templating;

namespace ImageChat
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class RoomMiddleware
    {
        readonly PathString _pathString;
        readonly AppFunc _next;
        readonly Regex _regex;
        readonly List<Room> _roomList = new List<Room>();
        readonly TemplateService _templateService = new TemplateService();
        public RoomMiddleware(AppFunc next, string path)
        {
            _next = next;
            _pathString = new PathString(path);
            _regex = new Regex(@"\/\d+", RegexOptions.Compiled);
            _templateService.AddNamespace("ImageChat");
            _roomList.Add(new Room("ニコニコ", "http://deliver.commons.nicovideo.jp/thumbnail/nc19665?size=l"));
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path;

            PathString remainingPath;
            if (path.StartsWithSegments(_pathString, out remainingPath))
            {
                var match = _regex.Match(remainingPath.ToString());
                if (match.Success)
                {
                    var roomId = int.Parse(match.Value.Substring(1));
                    Room room = null;
                    if (roomId >= 0 && roomId < _roomList.Count)
                    {
                        room = _roomList[roomId];
                    }
                    var template = TemplateLoader.Load("Room.cshtml");
                    var parsed = _templateService.Parse(template, room, null, null);
                    await context.Response.WriteAsync(parsed);
                }
                else
                {
                    var template = TemplateLoader.Load("RoomList.cshtml");
                    var parsed = _templateService.Parse(template, _roomList, null, null);
                    await context.Response.WriteAsync(parsed);
                }
            }
            else
            {
                await _next(environment);
            }
        }
    }
}
