namespace MineEdit.MapMonitor
{
    partial class frmMain
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblIsGenerating = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmdRegenerate = new System.Windows.Forms.Button();
            this.cmdStartStop = new System.Windows.Forms.Button();
            this.picMapStatus = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMapStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.picMapStatus, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(555, 389);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.lblIsGenerating);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(549, 94);
            this.panel1.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(19, 38);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(126, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Generating Chunk (0,0)...";
            // 
            // lblIsGenerating
            // 
            this.lblIsGenerating.AutoSize = true;
            this.lblIsGenerating.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIsGenerating.Location = new System.Drawing.Point(3, 16);
            this.lblIsGenerating.Name = "lblIsGenerating";
            this.lblIsGenerating.Size = new System.Drawing.Size(179, 13);
            this.lblIsGenerating.TabIndex = 2;
            this.lblIsGenerating.Text = "Generating Content in World 1";
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(549, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmdRegenerate);
            this.panel2.Controls.Add(this.cmdStartStop);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 103);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(549, 26);
            this.panel2.TabIndex = 2;
            // 
            // cmdRegenerate
            // 
            this.cmdRegenerate.Location = new System.Drawing.Point(107, 0);
            this.cmdRegenerate.Name = "cmdRegenerate";
            this.cmdRegenerate.Size = new System.Drawing.Size(75, 23);
            this.cmdRegenerate.TabIndex = 1;
            this.cmdRegenerate.Text = "Regenerate";
            this.cmdRegenerate.UseVisualStyleBackColor = true;
            // 
            // cmdStartStop
            // 
            this.cmdStartStop.Location = new System.Drawing.Point(22, 0);
            this.cmdStartStop.Name = "cmdStartStop";
            this.cmdStartStop.Size = new System.Drawing.Size(75, 23);
            this.cmdStartStop.TabIndex = 0;
            this.cmdStartStop.Text = "Start";
            this.cmdStartStop.UseVisualStyleBackColor = true;
            this.cmdStartStop.Click += new System.EventHandler(this.cmdStartStop_Click);
            // 
            // picMapStatus
            // 
            this.picMapStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMapStatus.Location = new System.Drawing.Point(3, 135);
            this.picMapStatus.Name = "picMapStatus";
            this.picMapStatus.Size = new System.Drawing.Size(549, 251);
            this.picMapStatus.TabIndex = 3;
            this.picMapStatus.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 389);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmMain";
            this.Text = "MineEdit Map Monitor (World 0)";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMapStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblIsGenerating;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cmdRegenerate;
        private System.Windows.Forms.Button cmdStartStop;
        private System.Windows.Forms.PictureBox picMapStatus;

    }
}