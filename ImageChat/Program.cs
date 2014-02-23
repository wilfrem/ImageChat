using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daemonizer;
using Microsoft.Owin.Hosting;

namespace ImageChat
{
    class Program
    {
        public const string Host = "localhost:12121";
        
        static void Main(string[] args)
        {
            int port;
            if (args.Length == 0 || int.TryParse(args[0], out port))
            {
                port = 12121;
            }
#if DEBUG
            var url = "http://localhost:" + port;
#else
            var url = "http://+:" + port;
#endif
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server started at " + port);
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
