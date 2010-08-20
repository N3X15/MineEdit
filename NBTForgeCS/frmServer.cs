using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Timer = System.Timers.Timer;
namespace ServerWrap
{
    public partial class frmServer : Form
    {
        TcpClient client;
        NetworkStream server;
        Thread t;
        Timer tick = new Timer(1000);
        Queue<long> MemoryHistory = new Queue<long>(256);
        string Pass;
        string Hostname;
        int Port;
        public frmServer(string IP, int Port, string pass)
        {
            Enabled = false;
            Hostname = IP;
            this.Port = Port;

            tick.Elapsed += new System.Timers.ElapsedEventHandler(tick_Elapsed);
            try
            {
                client = new TcpClient(IP, Port);
                tick.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Can't connect to the designated server.  Check your settings.");
                return;
            }
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Pass = pass;
            Enabled = true;
        }

        void tick_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendData("09");
        }

        public void Info(string text)
        {
            Log(text, Color.White);
        }

        public void Warning(string text)
        {
            Log(text, Color.Orange);
        }
        public void Error(string text)
        {
            Log(text, Color.Red);
        }

        public void Log(string EventText,Color TextColor)
        {
            string nDateTime = DateTime.Now.ToString("hh:mm:ss tt") + " - ";

            // color text.
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectionColor = TextColor;

            // newline if first line, append if else.
            if (txtLog.Lines.Length == 0)
            {
                txtLog.AppendText(nDateTime + EventText);
                txtLog.ScrollToCaret();
                txtLog.AppendText(System.Environment.NewLine);
            }
            else
            {
                txtLog.AppendText(nDateTime + EventText + System.Environment.NewLine);
                txtLog.ScrollToCaret();
            }
        }


        private void MainThread()
        {
            byte[] bytes = new byte[256];
            int i=0;
            Info("Logging in...");
            SendData("00" + Pass);
            try
            {
                while ((i = server.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string ln = Encoding.ASCII.GetString(bytes);
                    ln = ln.Replace("\0", "");
                    string pcmd = ln.Substring(0, 2);
                    byte cmd = byte.Parse(pcmd, System.Globalization.NumberStyles.AllowHexSpecifier);
                    string[] args = ln.Substring(2).Split('\t');
                    try
                    {
                        switch (cmd)
                        {
                            case 0:
                                Console.WriteLine(args[0].Trim());
                                if (args[0] == "BAD")
                                    Error("Login failure.  Most commands will be disabled.");
                                else
                                    Info("Logged in successfully.");
                                break;
                            case 9:
                                Text = string.Format("Minecraft Server {0}:{1} ({2} players)", Hostname, Port, args[0]);
                                MemoryHistory.Enqueue(long.Parse(args[1]));
                                MemoryHistory.Dequeue();
                                memGraph.Title = string.Format("Working Set: {0}", FormatBytes(long.Parse(args[1])));
                                memGraph.DrawHistogram(MemoryHistory.ToArray());
                                break;
                            case 0xfe:
                                Info(args[0]);
                                break;
                            case 0xff:
                                Error(args[0]);
                                break;
                            default:
                                Info(string.Format("Received {0}.", ln));
                                break;

                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception e)
            {
                Error(e.ToString());
            }
        }
        public string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }
        public void SendData(string dat)
        {
            byte[] b = Encoding.UTF8.GetBytes(dat);
            server.Write(b, 0, b.Length);
        }
        private void frmServer_Load(object sender, EventArgs e)
        {
            Info("MineManager v1.0 Starting up...");
            for(int i =0;i<256;i++)
                MemoryHistory.Enqueue(0L);
            server = client.GetStream();

            t = new Thread(MainThread);
            t.Start();
        }

        private void txtSendCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendData(txtSendCommand.Text);
                txtSendCommand.Clear();
            }
        }
    }
}
