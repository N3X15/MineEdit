using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using OpenMinecraft;
using System.Diagnostics;

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
