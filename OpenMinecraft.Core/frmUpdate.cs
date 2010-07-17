using System;
using System.Collections.Generic;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Net;
namespace OpenMinecraft
{
    public partial class frmUpdate : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer(100);
        public frmUpdate()
        {
            InitializeComponent();
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
        }
        public void Start()
        {
            cmdStart.Enabled = false;
            Blocks.Clear();

            lblStatus.Text = "Checking version...";

            WebClient wc = new WebClient();
            string hurp = wc.DownloadString("http://github.com/N3X15/MineEdit/raw/master/OpenMinecraft.Core/Blocks.cs");
            foreach (string l in hurp.Split('\n'))
            {
                string line=l.Trim();
                if (line.StartsWith("public static string Version = "))
                {
                    string v = line.Substring("public static string Version = ".Length+1, 8);
                    if (Blocks.Version != v)
                    {
                        MessageBox.Show("New version of MineEdit is available.  Please visit the thread to get the newest version.","Update available");
                        Console.WriteLine("{0} != {1}", v, Blocks.Version);
                    }
                }

            }
            lblStatus.Text = "Retrieving blocks...";
            pb.Style = ProgressBarStyle.Marquee;
            Blocks.UpdateBlocks();

            lblStatus.Text = "Retrieving items...";
            Blocks.UpdateItems();
            pb.Maximum = Blocks.TotalImages;
            lblStatus.Text = "Downloading images...";

            pb.Style = ProgressBarStyle.Continuous;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = false;
            timer.Start();
        }

        delegate void hurf(int val);
        delegate void setstring(string val);
        public void SetMax(int max)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new hurf(SetMax), max);
            }
            else
            {
                pb.Maximum = max;
            }
        }
        public void SetVal(int val)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new hurf(SetVal), val);
            }
            else
            {
                pb.Value = val;
            }
        }
        public void SetText(string c)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new setstring(SetText), c);
            }
            else
            {
                lblStatus.Text = c;
            }
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string derp;
            try
            {
                int lol = Blocks.GetImagesLeft(out derp);
                SetVal(pb.Maximum-lol);
                SetText(string.Format("({0}%) {1}", (int)(((float)(pb.Maximum-lol) / (float)pb.Maximum) * 100), derp));
                timer.Start();
            }
            catch(Exception)
            {
                SetVal(pb.Maximum);
                SetText("Done.");
                timer.Stop();
                Blocks.Save();
                MessageBox.Show("You now need to restart MineEdit.");
                Environment.Exit(0);
            }
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            Start();
        }
    }
}
