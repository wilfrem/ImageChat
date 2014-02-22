using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageChat;

// ReSharper disable once CheckNamespace
namespace Owin
{
    static class RoomExtensions
    {
        public static void UseRoom(this IAppBuilder builder, string path)
        {
            builder.Use(typeof(RoomMiddleware), path);
        }
    }
}