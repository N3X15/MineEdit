using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LaunchServer
{
    public class TCPServer
    {
        TcpListener Listener;
        ServerWrapper Wrapper;

        Thread thread;

        string CurrentBuffer="";
        bool ConnectedClient = false;
        bool AuthenticatedClient = false;

        NetworkStream ClientStream;

        public TCPServer(string IP, short myport)
        {
            Listener = new TcpListener(IPAddress.Parse(IP),(int)myport);
            //;Listener.AllowNatTraversal(true); // Fff
            Listener.Start();
            Wrapper = new ServerWrapper();
            Wrapper.STDERR += new ServerOutputHandler(Wrapper_STDERR);
            Wrapper.STDOUT += new ServerOutputHandler(Wrapper_STDOUT);
            thread= new Thread(MainThread);
            thread.Start();
            ClientStream = null;
        }

        void Wrapper_STDOUT(string dat)
        {
            if (dat == null)
                return;
            string rdat = dat.Replace("\n", "").Replace("\r", "");
            int ln = (dat.Length / 256) + 1;
            string[] data = new string[ln];
            for (int i = 0; i < ln; i++)
            {
                int s = 254 * i;
                int l = Math.Min(rdat.Length, 254);
                SendClientData("FE" + rdat.Substring(s, l));
            }
        }

        void Wrapper_STDERR(string dat)
        {
            if (dat == null)
                return;
            string rdat = dat.Replace("\n", "").Replace("\r", "");
            int ln = (dat.Length / 256) + 1;
            string[] data = new string[ln];
            for (int i = 0; i < ln; i++)
            {
                int s = 254 * i;
                int l = Math.Min(rdat.Length, 254);
                SendClientData("FF" + rdat.Substring(s, l));
            }
        }

        private bool ProcessInput(string data)
        {
            // FFArguments
            string[] args = data.Substring(2).Split('\t');
            switch (data.Substring(0, 2))
            {
                case "00": // Authenticate
                    AuthenticatedClient = (data.Substring(2).Equals(Settings.Get("MgmtPassword","89vuhir9gvhur43u9vhiru09c9icv23ri9dfvoui34t8rgfhukq4r8")));
                    SendClientData("00" + ((AuthenticatedClient) ? "OK" : "BAD")); // 00OK/00BAD
                    break;
                case "01": // Shut down gracefully
                    if (!AuthenticatedClient) return true;
                    Wrapper.Stop(true);
                    Thread.Sleep(10000);
                    Environment.Exit(0);
                    break;
                case "02": // Restart gracefully.
                    if (!AuthenticatedClient) return true;
                    Wrapper.Stop();
                    break;
                case "03": // Set server config value X to Y
                    if (!AuthenticatedClient) return true;
                    Wrapper.SetConfig(args[0], args[1]);
                    break;
                case "04": // Get server config value X
                    if (!AuthenticatedClient) return true;
                    string val = Wrapper.GetConfig(args[0]);
                    SendClientData("04" + val);
                    break;
                case "05": // Get/Set wrapper settings.
                    if (!AuthenticatedClient) return true;
                    if (args.Length == 2)
                        Settings.Set(args[0], args[1]);
                    else
                    {
                        string valu = Settings.Get(args[0],"");
                        SendClientData("05"+ valu);
                    }
                    break;
                case "06": // Get Memory Usage
                    if (!AuthenticatedClient) return true;
                    SendClientData("06" + Wrapper.GetServerMemory());
                    break;
                case "07": // Restart wrapper
                    Wrapper.Stop(true);
                    Listener.Stop();
                    // fffffffffff
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        Process.Start("LaunchServer.exe");
                    else
                        Process.Start("mono", "LaunchServer.exe");
                    return false;
                case "08": // Raw server command
                    if (!AuthenticatedClient) return true;
                    Wrapper.SendCommand(data.Substring(2));
                    return true;

                case "09": // Combined stats ( # dweebs, memory usage)
                    if (!AuthenticatedClient) return true;
                    SendClientData("09" + Wrapper.CurrentPlayers.Count.ToString() + "\t" + Wrapper.GetServerMemory().ToString());
                    return true;

                default:
                    if (!AuthenticatedClient) return true;
                    Wrapper.SendCommand(data);
                    break;
            }
            return true;
        }

        public void SendClientData(string line)
        {
            if (ConnectedClient)
            {
                // Anything but ascii fucks up the logging
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(line);
                CurrentBuffer = "";
                // Send back a response.
                ClientStream.Write(msg, 0, msg.Length);
                //Console.WriteLine(String.Format("Sent: {0}", line));
            }
        }

        private void MainThread()
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            string data="";
            while (true)
            {
                try
                {
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    Console.WriteLine("Listening!");
                    TcpClient client = Listener.AcceptTcpClient();
                    Console.WriteLine("[HurpScript] Client connected!");

                    // Get a stream object for reading and writing
                    ClientStream = client.GetStream();

                    int i;
                    data = null;
                    // Loop to receive all the data sent by the client.
                    ConnectedClient = true;
                    while ((i = ClientStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a UTF8 string.
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        //Console.WriteLine(String.Format("Received: {0}", data));

                        // Process the data sent by the client.
                        if (!ProcessInput(data))
                        {
                            ClientStream.Close();
                            client.Close();
                            Environment.Exit(0);
                        }

                        if (CurrentBuffer.Length > 0)
                        {
                            SendClientData(CurrentBuffer);
                        }
                    }
                    ConnectedClient = false;
                    // Shutdown and end connection
                    client.Close();
                }
                catch (Exception) { }
            }
        }
    }
}
