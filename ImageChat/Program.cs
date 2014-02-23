using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Daemonizer;
using Microsoft.Owin.Hosting;

namespace ImageChat
{
    internal class Program
    {
        public static string Host { get; set; }

        private static void Main(string[] args)
        {
            int port;
            if (args.Length == 0 || int.TryParse(args[0], out port))
            {
                port = 12121;
            }
            var platform = Environment.OSVersion.Platform;
            string url;
            if (platform != PlatformID.Unix && platform != PlatformID.MacOSX)
            {
                url = "http://localhost:" + port;
                Host = "localhost:" + port;
            }
            else
            {
                url = "http://"+Dns.GetHostName()+":" + port;
                Host = Dns.GetHostName() + ":" + port;
            }
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server started at " + url);
                try
                {
                    Daemon.Instance.WaitForUnixSignal();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("not in *nix platform. wait using Console.ReadLine");
                    Console.ReadLine();
                }
            }
        }
    }
}