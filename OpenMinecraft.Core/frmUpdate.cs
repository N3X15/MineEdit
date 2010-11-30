using System;
using System.Collections.Generic;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Net;
namespace OpenMinecraft
{
    public class frmUpdate : Form
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdCancel = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmdStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(407, 51);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 22);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(12, 50);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(380, 23);
            this.pb.Step = 1;
            this.pb.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pb.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Performing Updates...";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(29, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(108, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Click \"Begin\" to start.";
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(407, 26);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(90, 22);
            this.cmdStart.TabIndex = 1;
            this.cmdStart.Text = "Begin";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // frmUpdate
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(509, 85);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdate";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updating...";
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button cmdStart;
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
            if (!Blocks.CheckForUpdates())
            {
                DialogResult dr = MessageBox.Show("New version of MineEdit is available.\n\nWould you like to visit the download site to get the newest version?", "Update available",MessageBoxButtons.YesNo);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("http://github.com/N3X15/MineEdit/downloads/");
                    return;
                }
                else if (Blocks.Broken)
                {
                    MessageBox.Show("Update required due to a danger of damaging your saves.  Exiting...");
                    Environment.Exit(0);
                }
            }

            lblStatus.Text = "Retrieving blocks...";
            pb.Style = ProgressBarStyle.Marquee;
            Blocks.UpdateBlocks();

            lblStatus.Text = "Retrieving items...";
            Blocks.UpdateItems();

            //lblStatus.Text = "Parsing wiki...";
            //Blocks.UpdateIDs();

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
