/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MineEdit
{
    public class dlgLongTask : Form
    {
        private int _SubtasksTotal;
        private int _SubtasksComplete;
        private Thread thread;
        protected Stopwatch totalTime;
        protected System.Timers.Timer Tick = new System.Timers.Timer(250);
        public bool STOP = false;        
        
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbAnimation = new System.Windows.Forms.PictureBox();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpSubtask = new System.Windows.Forms.GroupBox();
            this.lblSubAverage = new System.Windows.Forms.Label();
            this.lblSubTimeElapsed = new System.Windows.Forms.Label();
            this.lblSubProgress = new System.Windows.Forms.Label();
            this.lblCurrentSubtask = new System.Windows.Forms.Label();
            this.subProgress = new System.Windows.Forms.ProgressBar();
            this.grpTotal = new System.Windows.Forms.GroupBox();
            this.lblAverageTime = new System.Windows.Forms.Label();
            this.lblTotalProgress = new System.Windows.Forms.Label();
            this.lblTotalTimeElapsed = new System.Windows.Forms.Label();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.totalProgress = new System.Windows.Forms.ProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).BeginInit();
            this.grpSubtask.SuspendLayout();
            this.grpTotal.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.pbAnimation);
            this.panel1.Controls.Add(this.lblSubtitle);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(581, 56);
            this.panel1.TabIndex = 0;
            // 
            // pbAnimation
            // 
            this.pbAnimation.Location = new System.Drawing.Point(3, 3);
            this.pbAnimation.Name = "pbAnimation";
            this.pbAnimation.Size = new System.Drawing.Size(51, 50);
            this.pbAnimation.TabIndex = 2;
            this.pbAnimation.TabStop = false;
            this.pbAnimation.Paint += new System.Windows.Forms.PaintEventHandler(this.pbAnimation_Paint);
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(80, 32);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(52, 13);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "lblSubtitle";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(60, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(45, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "lblTitle";
            // 
            // grpSubtask
            // 
            this.grpSubtask.Controls.Add(this.lblSubAverage);
            this.grpSubtask.Controls.Add(this.lblSubTimeElapsed);
            this.grpSubtask.Controls.Add(this.lblSubProgress);
            this.grpSubtask.Controls.Add(this.lblCurrentSubtask);
            this.grpSubtask.Controls.Add(this.subProgress);
            this.grpSubtask.Location = new System.Drawing.Point(12, 62);
            this.grpSubtask.Name = "grpSubtask";
            this.grpSubtask.Size = new System.Drawing.Size(557, 86);
            this.grpSubtask.TabIndex = 1;
            this.grpSubtask.TabStop = false;
            this.grpSubtask.Text = "Current Task";
            // 
            // lblSubAverage
            // 
            this.lblSubAverage.AutoSize = true;
            this.lblSubAverage.Location = new System.Drawing.Point(372, 41);
            this.lblSubAverage.Name = "lblSubAverage";
            this.lblSubAverage.Size = new System.Drawing.Size(76, 13);
            this.lblSubAverage.TabIndex = 5;
            this.lblSubAverage.Text = "Average Time:";
            // 
            // lblSubTimeElapsed
            // 
            this.lblSubTimeElapsed.AutoSize = true;
            this.lblSubTimeElapsed.Location = new System.Drawing.Point(372, 16);
            this.lblSubTimeElapsed.Name = "lblSubTimeElapsed";
            this.lblSubTimeElapsed.Size = new System.Drawing.Size(74, 13);
            this.lblSubTimeElapsed.TabIndex = 4;
            this.lblSubTimeElapsed.Text = "Time Elapsed:";
            // 
            // lblSubProgress
            // 
            this.lblSubProgress.AutoSize = true;
            this.lblSubProgress.Location = new System.Drawing.Point(6, 41);
            this.lblSubProgress.Name = "lblSubProgress";
            this.lblSubProgress.Size = new System.Drawing.Size(162, 13);
            this.lblSubProgress.TabIndex = 3;
            this.lblSubProgress.Text = "Progress: 0/0 subtasks complete";
            // 
            // lblCurrentSubtask
            // 
            this.lblCurrentSubtask.AutoSize = true;
            this.lblCurrentSubtask.Location = new System.Drawing.Point(6, 16);
            this.lblCurrentSubtask.Name = "lblCurrentSubtask";
            this.lblCurrentSubtask.Size = new System.Drawing.Size(35, 13);
            this.lblCurrentSubtask.TabIndex = 2;
            this.lblCurrentSubtask.Text = "label1";
            // 
            // subProgress
            // 
            this.subProgress.Location = new System.Drawing.Point(6, 57);
            this.subProgress.Name = "subProgress";
            this.subProgress.Size = new System.Drawing.Size(545, 23);
            this.subProgress.TabIndex = 0;
            // 
            // grpTotal
            // 
            this.grpTotal.Controls.Add(this.lblAverageTime);
            this.grpTotal.Controls.Add(this.lblTotalProgress);
            this.grpTotal.Controls.Add(this.lblTotalTimeElapsed);
            this.grpTotal.Controls.Add(this.lblCurrentTask);
            this.grpTotal.Controls.Add(this.totalProgress);
            this.grpTotal.Location = new System.Drawing.Point(12, 154);
            this.grpTotal.Name = "grpTotal";
            this.grpTotal.Size = new System.Drawing.Size(557, 83);
            this.grpTotal.TabIndex = 2;
            this.grpTotal.TabStop = false;
            this.grpTotal.Text = "Total Progress";
            // 
            // lblAverageTime
            // 
            this.lblAverageTime.AutoSize = true;
            this.lblAverageTime.Location = new System.Drawing.Point(372, 38);
            this.lblAverageTime.Name = "lblAverageTime";
            this.lblAverageTime.Size = new System.Drawing.Size(76, 13);
            this.lblAverageTime.TabIndex = 4;
            this.lblAverageTime.Text = "Average Time:";
            // 
            // lblTotalProgress
            // 
            this.lblTotalProgress.AutoSize = true;
            this.lblTotalProgress.Location = new System.Drawing.Point(6, 38);
            this.lblTotalProgress.Name = "lblTotalProgress";
            this.lblTotalProgress.Size = new System.Drawing.Size(145, 13);
            this.lblTotalProgress.TabIndex = 3;
            this.lblTotalProgress.Text = "Progress: 0/0 tasks complete";
            // 
            // lblTotalTimeElapsed
            // 
            this.lblTotalTimeElapsed.AutoSize = true;
            this.lblTotalTimeElapsed.Location = new System.Drawing.Point(372, 16);
            this.lblTotalTimeElapsed.Name = "lblTotalTimeElapsed";
            this.lblTotalTimeElapsed.Size = new System.Drawing.Size(74, 13);
            this.lblTotalTimeElapsed.TabIndex = 2;
            this.lblTotalTimeElapsed.Text = "Time Elapsed:";
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.AutoSize = true;
            this.lblCurrentTask.Location = new System.Drawing.Point(6, 16);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(35, 13);
            this.lblCurrentTask.TabIndex = 1;
            this.lblCurrentTask.Text = "label1";
            // 
            // totalProgress
            // 
            this.totalProgress.Location = new System.Drawing.Point(6, 54);
            this.totalProgress.Name = "totalProgress";
            this.totalProgress.Size = new System.Drawing.Size(545, 23);
            this.totalProgress.TabIndex = 0;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(494, 256);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Enabled = false;
            this.cmdOK.Location = new System.Drawing.Point(413, 256);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // dlgLongTask
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(581, 291);
            this.ControlBox = false;
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.grpTotal);
            this.Controls.Add(this.grpSubtask);
            this.Controls.Add(this.panel1);
            this.Name = "dlgLongTask";
            this.Text = "Progress Report";
            this.Load += new System.EventHandler(this.dlgLongTask_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnimation)).EndInit();
            this.grpSubtask.ResumeLayout(false);
            this.grpSubtask.PerformLayout();
            this.grpTotal.ResumeLayout(false);
            this.grpTotal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbAnimation;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpSubtask;
        private System.Windows.Forms.Label lblSubAverage;
        private System.Windows.Forms.Label lblSubTimeElapsed;
        private System.Windows.Forms.Label lblCurrentSubtask;
        private System.Windows.Forms.ProgressBar subProgress;
        private System.Windows.Forms.GroupBox grpTotal;
        private System.Windows.Forms.Label lblAverageTime;
        private System.Windows.Forms.Label lblTotalProgress;
        private System.Windows.Forms.Label lblTotalTimeElapsed;
        private System.Windows.Forms.Label lblCurrentTask;
        private System.Windows.Forms.ProgressBar totalProgress;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        public System.Windows.Forms.Label lblSubProgress;

        public dlgLongTask()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Tick.Elapsed += new System.Timers.ElapsedEventHandler(Tick_Elapsed);
        }
        public void Start(ThreadStart ts)
        {
            thread = new Thread(ts);
        }

        void Tick_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            --ci;
            if(ci==-1)
                ci=11;
            pbAnimation.Invalidate();
            lblTotalTimeElapsed.Text = "Time Elapsed: " + totalTime.Elapsed.ToString();
        }
        public void SetMarquees(bool a, bool b)
        {
            subProgress.Style = (a) ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            totalProgress.Style = (b) ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
        }
        public string Title 
        { 
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text=value;
            }
        }
        public string Subtitle
        {
            get
            {
                return lblSubtitle.Text;
            }
            set
            {
                lblSubtitle.Text = value;
            }
        }
        public string CurrentSubtask
        {
            get
            {
                return lblCurrentSubtask.Text;
            }
            set
            {
                lblCurrentSubtask.Text = value;
            }
        }
        public string VocabSubtask { get; set; }
        public string VocabSubtasks { get; set; }
        public int SubtasksTotal
        {
            get
            {
                return _SubtasksTotal;
            }
            set
            {
                subProgress.Maximum = value;
                _SubtasksTotal = value;
                //(MdiParent as frmMain).tsbProgress.Maximum = value;
            }
        }
        public int SubtasksComplete 
        {
            get
            {
                return _SubtasksComplete;
            }
            set
            {
                _SubtasksComplete = value;
                subProgress.Value = OpenMinecraft.Utils.Clamp(value,subProgress.Minimum,subProgress.Maximum);
                lblSubProgress.Text = string.Format("Progress: {0}/{1} ({2}%) {3} complete.", _SubtasksComplete, _SubtasksTotal, ((float)_SubtasksComplete / (float)_SubtasksTotal) * 100f, (_SubtasksComplete > 1) ? VocabSubtask : VocabSubtasks);
                
            }
        }
        public string CurrentTask
        {
            get
            {
                return lblCurrentTask.Text;
            }
            set
            {
                lblCurrentTask.Text = value;
            }
        }
        public string VocabTask      { get; set; }
        public string VocabTasks { get; set; }
        public int TasksTotal
        {
            get
            {
                return totalProgress.Maximum;
            }
            set
            {
                totalProgress.Maximum = value;
                //(MdiParent as frmMain).tsbProgress.Maximum = value;
            }
        }
        public int TasksComplete
        {
            get
            {
                return totalProgress.Value;
            }
            set
            {
                if (TasksTotal < value)
                    TasksTotal = value;
                totalProgress.Value = value;
                //(ParentForm as frmMain).tsbProgress.Value = value;
                lblTotalProgress.Text = string.Format("Progress: {0}/{1} ({2}%) {3} complete.", TasksComplete, TasksTotal, ((float)TasksComplete / (float)TasksTotal) * 100f, (TasksComplete > 1) ? VocabTasks : VocabTasks);
                //(ParentForm as frmMain).SetStatus(lblTotalProgress.Text);
            }
        }

        public void Done()
        {
            cmdOK.Enabled = true;
            cmdCancel.Enabled = false;
            lblSubtitle.Text = "Job complete!";
            Tick.Stop();
        }

        private void dlgLongTask_Load(object sender, EventArgs e)
        {
            thread.Start();
            totalTime = Stopwatch.StartNew();
            Tick.Start();
        }

        private int ci = 11;
        private void pbAnimation_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int ii=0;
            for (double i = 0; i < Math.PI * 2; i += Math.PI / 6.0)
            {

                float x = (float)Math.Sin(i)*16f;
                float y = (float)Math.Cos(i)*16f;
                x += (pbAnimation.Width / 2f);
                y += (pbAnimation.Height / 2f);
                if (ii == ci)
                    g.FillEllipse(new SolidBrush(Color.Black), new RectangleF(x-4f, y-4f, 8f, 8f));
                else
                    g.FillEllipse(new SolidBrush(Color.LightGray), new RectangleF(x-4f, y-4f, 8f, 8f));
                ii++;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.STOP = true;
            Tick.Stop();
            this.thread.Abort();
        }

        private void lblTotalProgress_Click(object sender, EventArgs e)
        {

        }
    }
}
