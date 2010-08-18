using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.IO;

namespace LaunchServer
{
    public delegate Packet PacketDelegate(Proxy.ClientSession session, Packet p, IPEndPoint iPEndPoint);
    public class Proxy
    {
        public string Hostname;
        public int SrcPort;
        public int DestPort;
        TcpListener TCPReceiver;
        Thread tcpThread;
        public static Dictionary<byte, List<PacketDelegate>> InboundDelegates = new Dictionary<byte, List<PacketDelegate>>();
        public static Dictionary<byte, List<PacketDelegate>> OutboundDelegates = new Dictionary<byte, List<PacketDelegate>>();
        public Dictionary<string, ClientSession> RemoteSessions = new Dictionary<string, ClientSession>();
        public Dictionary<string, ClientSession> LocalSessions = new Dictionary<string, ClientSession>();
        public Proxy(int srcPort,int destPort, IPAddress listenaddr)
        {
            SrcPort = srcPort;
            DestPort = destPort;
            tcpThread = new Thread(TCPThread);
            tcpThread.Name = "TCP Proxy";
            Hostname = "localhost";
            TCPReceiver = new TcpListener(IPAddress.Any, SrcPort);
        }

        public void Connect()
        {
            tcpThread.Start();
        }
        private void TCPThread()
        {
            TCPReceiver.Start();
            int i = 0;
            while (true)
            {
                i++;
                TcpClient Source = TCPReceiver.AcceptTcpClient();
                NetworkStream SrcStream = Source.GetStream();

                TcpClient _RemoteSocket = new TcpClient(Hostname, DestPort);
                NetworkStream DestStream = _RemoteSocket.GetStream();
                string IP = (Source.Client.LocalEndPoint as IPEndPoint).ToString();
                Console.WriteLine("*** {0} Connecting...",IP);
                ClientSession _RemoteClient = new ClientSession("remote" + IP)
                {
                    ClientStream = SrcStream,
                    ServerStream = DestStream,
                    Listener = _RemoteSocket,
                    MyDelegates=InboundDelegates
                };
                ClientSession _LocalClient = new ClientSession("local" + IP)
                {
                    ClientStream = DestStream,
                    ServerStream = SrcStream,
                    Listener = Source,
                    MyDelegates = OutboundDelegates
                };
                RemoteSessions.Add(IP, _RemoteClient);
                LocalSessions.Add(IP, _LocalClient);
            }
        }

        public void SendPacket(string IP, bool Outbound, Packet p)
        {
            if (Outbound)
            {
                RemoteSessions[IP].InjectPacket(p);
            }
            else
            {
                LocalSessions[IP].InjectPacket(p);
            }
        }
        internal static string GetString(byte[] b)
        {
            string[] o = new string[b.Length];
            int i = 0;
            foreach(byte _b in b)
                o[i++]=_b.ToString("X2");
            return string.Join(" ", o);
        }

        public static void AddDelegate(bool Outbound, byte type, PacketDelegate del)
        {
            if (Outbound)
            {
                if (!OutboundDelegates.ContainsKey(type))
                    OutboundDelegates.Add(type, new List<PacketDelegate>());
                OutboundDelegates[type].Add(del);
            }
            else
            {
                if (!InboundDelegates.ContainsKey(type))
                    InboundDelegates.Add(type, new List<PacketDelegate>());
                InboundDelegates[type].Add(del);
            }
        }
        public class ClientSession
        {
            public TcpClient Listener;
            public NetworkStream ClientStream;
            public NetworkStream ServerStream;
            public Dictionary<byte, List<PacketDelegate>> MyDelegates = new Dictionary<byte, List<PacketDelegate>>();
            public bool Outbound;
            Thread _Thread;
            Queue<Packet> PacketsToInject = new Queue<Packet>();
            public ClientSession(string Name)
            {
                _Thread = new Thread(new ThreadStart(ThreadStartHander));
                _Thread.Name = Name;
                _Thread.Start();
            }
            public void InjectPacket(Packet p)
            {
                PacketsToInject.Enqueue(p);
            }
            public void ThreadStartHander()
            {
                try
                {
                    Byte[] data = new byte[99999];
                    BinaryReader cliR = new BinaryReader(ServerStream, Encoding.UTF8);
                    BinaryWriter cliW = new BinaryWriter(ServerStream, Encoding.UTF8);
                    BinaryReader servR = new BinaryReader(ClientStream, Encoding.UTF8);
                    BinaryWriter servW = new BinaryWriter(ClientStream, Encoding.UTF8);
                    while (true)
                    {
                        if (Listener.Available > 0)
                        {
                            Packet p = PacketReader.NextPacket(ref cliR);
                            if (p == null)
                            {
                                int read = cliR.Read(data, 0, Listener.Available);
                                //Console.WriteLine("[TCP] Received: " + Encoding.ASCII.GetString(data).Replace("\0", "").Replace((char)7, '?'));
                                //Console.WriteLine("[TCP] In Hex:   " + Proxy.GetString(data));
                                servW.Write(data, 0, read);
                            }
                            else 
                            {
                                if (p.PacketID != 0)
                                {
                                    Console.WriteLine("---------------------------------------------");
                                    Console.WriteLine("Received " + p.PacketName + " packet in " + _Thread.Name + ".");
                                    Console.WriteLine(p);
                                    Console.WriteLine("---------------------------------------------");
                                }
                                if(MyDelegates.ContainsKey((byte)p.PacketID))
                                    p = CallDelegates(p, (IPEndPoint)Listener.Client.RemoteEndPoint);
                                PacketReader.WritePacket(ref servW, p);
                                continue;
                            }
                        }
                        while (PacketsToInject.Count > 0)
                        {
                            Packet p = PacketsToInject.Dequeue();
                            PacketReader.WritePacket(ref servW, p);
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }

            private Packet CallDelegates(Packet p, IPEndPoint iPEndPoint)
            {
                foreach (PacketDelegate del in MyDelegates[(byte)p.PacketID])
                {
                    try
                    {
                        p = del(this, p, iPEndPoint);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("PacketDelegate error: " + e);
                        continue;
                    }
                }
                return p;
            }

        }
    }
}
