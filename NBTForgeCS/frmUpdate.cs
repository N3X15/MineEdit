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
            Blocks.Clear();
            lblStatus.Text = "Retrieving blocks...";
            pb.Style = ProgressBarStyle.Marquee;
            Blocks.UpdateBlocks();

            lblStatus.Text = "Retrieving items...";
            Blocks.UpdateItems();

            lblStatus.Text = "Downloading images...";

            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool first = true;
            string derp;
            int lol = Blocks.GetImagesLeft(out derp);
            if (first)
            {
                pb.Maximum = lol + 1;
                first = false;
            }
            pb.Value = lol;
            lblStatus.Text = string.Format("({0}%) {1}", (int)(((float)lol / (float)pb.Maximum) * 100));
            if (lol == 1)
            {
                timer.Stop();
                Blocks.Save();
            }
        }
    }
}
