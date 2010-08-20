using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Net;
using System.ComponentModel;

namespace LaunchServer
{
    public class Player
    {
        public string Name { get; set; }
        public PermissionsMask Permissions { get; set; }
        public short[] Items { get; set; }

        public Player(string name)
        {
            Items = new short[256];
            Name = name;
            Load();
        }

        public void Load()
        {
            if (File.Exists("Players/" + Name + ".dat"))
            {
                using (BinaryReader rdr = new BinaryReader(File.OpenRead("Players/" + Name + ".dat")))
                {
                    Permissions=(PermissionsMask)rdr.ReadInt32();
                    for (int i = 0; i < 256; i++)
                    {
                        Items[i]=rdr.ReadInt16();
                    }
                }
            }
        }

        public void Save()
        {
            using (BinaryWriter w = new BinaryWriter(File.OpenWrite("Players/" + Name + ".dat")))
            {
                w.Write((int)Permissions);
                for (int i = 0; i < 256; i++)
                {
                    w.Write(Items[i]);
                }
            }
        }
    }
    public delegate void ServerOutputHandler(string dat);
    public delegate void ChatHandler(Player player, string msg);
    public delegate void ChatCommandHandler(Player player, string cmd, string[] args);
    public delegate void JoinPartHandler(Player player, string IP, int port);
    public class ServerWrapper
    {
        public Dictionary<string, Player> KnownPlayers = new Dictionary<string, Player>();
        public List<string> Whitelist = new List<string>();
        public List<string> Bans = new List<string>();
        public Dictionary<string,string> CurrentPlayers = new Dictionary<string,string>();

        public int MaxPlayers = 1;
        Process ServerProcess;
        Proxy ServerProxy;

        List<IPlugin> Plugins = new List<IPlugin>();

        public event ServerOutputHandler STDOUT;
        public event ServerOutputHandler STDERR;
        public event ChatHandler Chat;
        public event ChatHandler Emote;
        public event ChatCommandHandler ChatCommand;
        public event JoinPartHandler Join;
        public event JoinPartHandler Leave;
        public ServerWrapper()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            SetupServer();
            ServerProxy = new Proxy(25564, 25565, IPAddress.Any);
            ServerProxy.Connect();
            LoadPlugins(Assembly.GetExecutingAssembly());
            Start();
            Say("Ready.");
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "exit")
                    Stop(true);
                SendCommand(str);
            }
        }

        void SetupServer()
        {
            if (ServerProcess != null)
                ServerProcess.Close();
            ProcessStartInfo psi = new ProcessStartInfo("java", "-Xmx1024M -Xms1024M -jar minecraft_server.jar nogui");
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            ServerProcess = new Process();
            ServerProcess.EnableRaisingEvents = true;
            ServerProcess.StartInfo = psi;
            ServerProcess.OutputDataReceived += new DataReceivedEventHandler(ServerProcess_OutputDataReceived);
            ServerProcess.ErrorDataReceived += new DataReceivedEventHandler(ServerProcess_ErrorDataReceived);
            ServerProcess.Exited += ServerProcess_Exited;
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Stop(true);
        }

        private void LoadPlugins(Assembly assembly)
        {
            Console.WriteLine("Loading plugins from " + assembly.GetName()+"...");
            foreach (Type t in assembly.GetTypes())
            {
                if(t.IsSubclassOf(typeof(IPlugin)))
                {
                    IPlugin p = (IPlugin)t.GetConstructor(new Type[]{typeof(ServerWrapper)}).Invoke(new object[]{this});
                    Console.WriteLine(" * Loaded plugin "+p.ToString()+"...");
                    Plugins.Add(p);
                }
            }
        }

        void ServerProcess_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("!!! SERVER CRASHED. RESTARTING. !!!");
            SetupServer();
            Start();
        }

        public void SendCommand(string val)
        {
            if(ServerProcess.StartInfo.RedirectStandardInput)
                ServerProcess.StandardInput.WriteLine(val);
        }

        public void Say(string val)
        {
            ServerProcess.StandardInput.WriteLine("say [HurpScript] " + val);
        }

        public void Give(string name, short what, byte num)
        {
            for (int i = 0; i < Math.Min((int)num, 64); i++)
            {
                SendCommand(string.Format("give {0} {1}", name, what));
            }
        }

        bool ShuttingDown = false;
        public void Stop(bool permanently)
        {
            if (ShuttingDown) return;
            ShuttingDown = true;
            Console.WriteLine("Shutting down server...");
            if (permanently)
                ServerProcess.Exited -= ServerProcess_Exited;
            SendCommand("stop");
            if (!ServerProcess.WaitForExit(10000))
                    ServerProcess.Kill();
            if (permanently)
            {
                Close();
                Environment.Exit(0);
            }
            ShuttingDown = false;
        }

        public void Start()
        {
            ServerProcess.Start();
            ServerProcess.BeginOutputReadLine();
            ServerProcess.BeginErrorReadLine();
        }

        public void Close()
        {
            ServerProcess.Close();
        }

        public void Ban(string name)
        {
            Kick(name);
            if (!Bans.Contains(name))
            {
                Bans.Add(name);
                Save();
            }
        }

        public void Unban(string name)
        {
            if (Bans.Contains(name))
            {
                Bans.Remove(name);
                Save();
            }
        }


        public bool IsBanned(string name)
        {
            return Bans.Contains(name);
        }

        private void Save()
        {
            XmlWriter xml = new XmlTextWriter("Hurrscript.xml",Encoding.UTF8);
            xml.WriteStartDocument();
            xml.WriteStartElement("config");

            xml.WriteAttributeString("max_players", MaxPlayers.ToString());

            xml.WriteStartElement("whitelist");
            foreach (string wle in Whitelist)
                xml.WriteElementString("player", wle);
            xml.WriteEndElement();

            xml.WriteStartElement("bans");
            foreach (string wle in Bans)
                xml.WriteElementString("player", wle);
            xml.WriteEndElement();

            xml.WriteEndElement();
            xml.WriteEndDocument();
            xml.Close();
        }

        private void Load()
        {
            LoadConfig();
            XmlDocument doc = new XmlDocument();
            doc.Load("Hurrscript.xml");
            XmlNodeList xnl = doc.GetElementsByTagName("config");
            MaxPlayers=int.Parse(xnl[0].Attributes["max_players"].Value);
            Say("Max players set to: " + MaxPlayers.ToString());
            xnl = doc.GetElementsByTagName("whitelist");
            foreach (XmlNode node in xnl[0].ChildNodes)
            {
                Whitelist.Add(node.Value);
            }
            xnl = doc.GetElementsByTagName("bans");
            foreach (XmlNode node in xnl[0].ChildNodes)
            {
                Whitelist.Add(node.Value);
            }
        }

        public void Kick(string name)
        {
            SendCommand("kick " + name);
        }
        void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            Console.WriteLine("[STDERR] " + e.Data);
            string line = e.Data;
            //Strip timestamp: 2010-08-07 01:58:53
            line=line.Remove(0, 20).Trim();
            // [INFO] N3X15 [/127.0.0.1:54913] logged in
            if (line.EndsWith("logged in"))
                ProcessLogin(line);
            // [INFO] N3X15 lost connection: Quitting
            if (line.Contains("lost connection:"))
                ProcessQuit(line);
            // [INFO] <N3X15> hurr
            if (line.StartsWith("[INFO] <"))
                ProcessChat(line);
            // [INFO] * N3X15 durr
            if (line.StartsWith("[INFO] * "))
                ProcessEmote(line);
            if (STDERR != null)
                STDERR(e.Data);
        }

        private void ProcessEmote(string line)
        {
            string[] c = line.Remove(0, 8).Split(' ');
            if (Emote != null)
                Emote(KnownPlayers[c[0]], line.Remove(0,9+c[0].Length));
        }

        private void ProcessChat(string line)
        {
            // Line: [INFO] <N3X15> !#
            string name = line.Substring(8, line.IndexOf('>')-8);
            string msg = line.Substring(line.IndexOf('>')+2);
            Console.WriteLine("[Chat]: {0} -> {1} sez: \"{2}\"", line, name, msg);
            if (msg.StartsWith("!") && msg.Length > 1)
            {
                string[] c = msg.Split(' ');
                string[] args = new string[c.Length-1];
                string cmd = c[0].Remove(0,1);
                Array.Copy(c, 1, args, 0, c.Length - 1);
                Console.WriteLine("Command " + cmd + " received.");
                if(ChatCommand!=null)
                    ChatCommand(KnownPlayers[name], cmd, args);
            }
            if (Chat != null)
                Chat(KnownPlayers[name], msg);
        }

        private void ProcessQuit(string line)
        {
            string[] c = line.Split(' ');
            if(CurrentPlayers.ContainsKey(c[1]))
            {
                CurrentPlayers.Remove(c[1]);
                if(Leave!=null)
                    Leave(KnownPlayers[c[1]],"",0);
            }
        }

        private void ProcessLogin(string line)
        {
            string[] c = line.Split(' ');
            string name = c[1];
            string[] addrc = c[2].Substring(2,c[2].Length-3).Split(':');
            string IP = addrc[0];
            int Port = int.Parse(addrc[1]);
            if(IsBanned(name))
            {
                Kick(name);
                return;
            }
            if(KnownPlayers.ContainsKey(name))
                KnownPlayers.Remove(name);
            Console.WriteLine("Added player: " + name + " " + IP + ".");
            KnownPlayers.Add(name,new Player(name));
            CurrentPlayers.Add(name, IP);
            if (Join != null)
                Join(KnownPlayers[name], IP, Port);
        }

        void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (ServerProcess.HasExited)
                {
                    //ServerProcess.Start();
                    return;
                }
                string line = e.Data;
                if (line == null) return;
                Console.WriteLine("[STDOUT] " + e.Data);
                if (STDOUT != null)
                    STDOUT(e.Data);
            }
            catch (Exception) { }
        }

        public string ManagementPassword { get; set; }

        Dictionary<string, string> ServConfig = new Dictionary<string, string>();
        internal void SetConfig(string k, string v)
        {
            if (!ServConfig.ContainsKey(k))
                ServConfig.Add(k, v);
            else
                ServConfig[k] = v;
            SaveConfig();
        }

        internal string GetConfig(string k)
        {
            if (!ServConfig.ContainsKey(k))
                return "";
            else
                return ServConfig[k];
        }

        public void LoadConfig()
        {
            foreach (string l in File.ReadAllLines("server.properties"))
            {
                if (l.StartsWith("#")) continue;
                string[] c = l.Split('=');
                ServConfig.Add(c[0], c[1]);
            }
        }

        public void SaveConfig()
        {
            string[] props = new string[ServConfig.Count + 2];
            int i =0;
            props[i++]="#Minecraft Server Configuration";
            props[i++]="# Do Not Manually Edit, maintained by MineManager ServerWrapper.";
            foreach (KeyValuePair<string,string> kvp in ServConfig)
            {
                props[i++] = string.Format("{0}={1}", kvp.Key, kvp.Value);
            }
            File.WriteAllLines("server.properties", props);
        }
        internal string GetServerMemory()
        {
            ServerProcess.Refresh();
            return ServerProcess.WorkingSet64.ToString();
        }
    }
}
