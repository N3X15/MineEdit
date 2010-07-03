using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
namespace MineEdit
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
        private void start()
        {
            Blocks.Clear();
            lblStatus.Text = "Retrieving blocks...";
            pb.Style = ProgressBarStyle.Marquee;
            Blocks.UpdateBlocks();

            lblStatus.Text = "Retrieving items...";
            Blocks.UpdateItems();

            lblStatus.Text = "Downloading images...";

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
            bool first = true;
            string derp;
            try
            {
                int lol = Blocks.GetImagesLeft(out derp);
                if (first)
                {
                    SetMax(lol + 1);
                    first = false;
                }
                SetVal(pb.Maximum-lol);
                SetText(string.Format("({0}%) {1}", (int)(((float)lol / (float)pb.Maximum) * 100), derp));
                timer.Start();
            }
            catch(Exception)
            {
                SetVal(pb.Maximum);
                SetText("Done.");
                timer.Stop();
                Blocks.Save();
            }
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            cmdStart.Enabled = false;
            start();
        }
    }
}
