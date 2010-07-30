namespace MineEdit
{
    partial class dlgLongTask
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

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

    }
}