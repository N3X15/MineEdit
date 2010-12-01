using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using OpenMinecraft;
using System.Threading;
using System.IO;
namespace MineEdit
{
    public partial class frmSplash : Form
    {
        Thread loadthread;
        public frmSplash()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        delegate void STD(string v); 
        public void SetText(string v)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new STD(SetText), v);
            }
            else
            {
                Console.WriteLine(v);
                lblStatus.Text = v;
            }
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            loadthread = new Thread(new ThreadStart(LoadShit));
            loadthread.Start();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(12, 278);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "label1";
            // 
            // frmSplash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.BackgroundImage = global::MineEdit.Properties.Resources.MineEdit;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSplash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading MineEdit";
            this.Load += new System.EventHandler(this.frmSplash_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private void LoadShit()
        {
            SetText("Checking version...");
            if (!Blocks.CheckForUpdates())
            {
                DialogResult dr = MessageBox.Show("New version of MineEdit is available.\n\nWould you like to visit the download site to get the newest version?", "Update available", MessageBoxButtons.YesNo);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                    System.Diagnostics.Process.Start("http://github.com/N3X15/MineEdit/downloads/");
            }

            if (Blocks.Broken)
            {
                MessageBox.Show("Update required due to a danger of damaging your saves.  Exiting...");
                Environment.Exit(0);
            }

            SetText("Loading blocks...");
            Blocks.Init();

            SetText("Loading settings...");
            Settings.Init();


            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            for (short i = 0; i < 5; i++)
            {
                SetText(string.Format("Checking for World{0}...", i + 1));
                string f = Path.Combine(appdata, string.Format(@".minecraft\saves\World{0}\level.dat", i + 1));
                Console.WriteLine(f);
                if (File.Exists(f))
                {
                    Settings.Worlds.Add(i,(Utils.DirSize(new DirectoryInfo(Path.GetDirectoryName(f))) / 1024f) / 1024f);
                }
            }

            Console.WriteLine("Loading /game/ handler.");
            Thread.Sleep(1000);
            Close();
        }
    }
}
