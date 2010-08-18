using System;
using System.IO;
using System.Net;

namespace LaunchServer
{
    static class Program
    {
        static TCPServer serv;
        [STAThread]
        static void Main()
        {
            Settings.Init();
            if (!File.Exists("minecraft_server.jar"))
            {
                Console.WriteLine("Whoops, you forgot to place minecraft_server.jar in this folder. Press something to exit.");
                Console.ReadKey();
            }
            try
            {
                string IP = Settings.Get("Listening Address", "0.0.0.0");
                short Port = short.Parse(Settings.Get("Wrapper Port", "5555"));
                serv = new TCPServer(IP, Port);
                Console.WriteLine(">");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
        }
    }
}
