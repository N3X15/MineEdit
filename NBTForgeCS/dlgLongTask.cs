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
    public partial class dlgLongTask : Form
    {
        private int _SubtasksTotal;
        private int _SubtasksComplete;
        private Thread thread;
        protected Stopwatch totalTime;
        protected System.Timers.Timer Tick = new System.Timers.Timer(250);
        public bool STOP = false;
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
                subProgress.Value = value;
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
    }
}
