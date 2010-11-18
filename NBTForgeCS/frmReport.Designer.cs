namespace MineEdit
{
    partial class frmReport
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkInfo = new System.Windows.Forms.CheckBox();
            this.chkWarnings = new System.Windows.Forms.CheckBox();
            this.chkErrors = new System.Windows.Forms.CheckBox();
            this.cmdFixAll = new System.Windows.Forms.Button();
            this.cmdDoNothing = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.clmType = new System.Windows.Forms.DataGridViewImageColumn();
            this.clmChunkPosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmChunkPosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlControls.SuspendLayout();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.chkDebug);
            this.pnlControls.Controls.Add(this.chkInfo);
            this.pnlControls.Controls.Add(this.chkWarnings);
            this.pnlControls.Controls.Add(this.chkErrors);
            this.pnlControls.Controls.Add(this.cmdFixAll);
            this.pnlControls.Controls.Add(this.cmdDoNothing);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControls.Location = new System.Drawing.Point(0, 298);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(551, 64);
            this.pnlControls.TabIndex = 1;
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(260, 7);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(78, 17);
            this.chkDebug.TabIndex = 5;
            this.chkDebug.Text = "Debugging";
            this.chkDebug.UseVisualStyleBackColor = true;
            this.chkDebug.CheckedChanged += new System.EventHandler(this.chkErrors_CheckedChanged);
            // 
            // chkInfo
            // 
            this.chkInfo.AutoSize = true;
            this.chkInfo.Location = new System.Drawing.Point(158, 7);
            this.chkInfo.Name = "chkInfo";
            this.chkInfo.Size = new System.Drawing.Size(95, 17);
            this.chkInfo.TabIndex = 4;
            this.chkInfo.Text = "Info Messages";
            this.chkInfo.UseVisualStyleBackColor = true;
            this.chkInfo.CheckedChanged += new System.EventHandler(this.chkErrors_CheckedChanged);
            // 
            // chkWarnings
            // 
            this.chkWarnings.AutoSize = true;
            this.chkWarnings.Checked = true;
            this.chkWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWarnings.Location = new System.Drawing.Point(80, 7);
            this.chkWarnings.Name = "chkWarnings";
            this.chkWarnings.Size = new System.Drawing.Size(71, 17);
            this.chkWarnings.TabIndex = 3;
            this.chkWarnings.Text = "Warnings";
            this.chkWarnings.UseVisualStyleBackColor = true;
            this.chkWarnings.CheckedChanged += new System.EventHandler(this.chkErrors_CheckedChanged);
            // 
            // chkErrors
            // 
            this.chkErrors.AutoSize = true;
            this.chkErrors.Checked = true;
            this.chkErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkErrors.Location = new System.Drawing.Point(21, 7);
            this.chkErrors.Name = "chkErrors";
            this.chkErrors.Size = new System.Drawing.Size(53, 17);
            this.chkErrors.TabIndex = 2;
            this.chkErrors.Text = "Errors";
            this.chkErrors.UseVisualStyleBackColor = true;
            this.chkErrors.CheckedChanged += new System.EventHandler(this.chkErrors_CheckedChanged);
            // 
            // cmdFixAll
            // 
            this.cmdFixAll.Location = new System.Drawing.Point(383, 29);
            this.cmdFixAll.Name = "cmdFixAll";
            this.cmdFixAll.Size = new System.Drawing.Size(75, 23);
            this.cmdFixAll.TabIndex = 1;
            this.cmdFixAll.Text = "Fix All";
            this.cmdFixAll.UseVisualStyleBackColor = true;
            this.cmdFixAll.Click += new System.EventHandler(this.cmdFixAll_Click);
            // 
            // cmdDoNothing
            // 
            this.cmdDoNothing.Location = new System.Drawing.Point(464, 29);
            this.cmdDoNothing.Name = "cmdDoNothing";
            this.cmdDoNothing.Size = new System.Drawing.Size(75, 23);
            this.cmdDoNothing.TabIndex = 0;
            this.cmdDoNothing.Text = "Do Nothing";
            this.cmdDoNothing.UseVisualStyleBackColor = true;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlTop.Controls.Add(this.lblSubtitle);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(551, 64);
            this.pnlTop.TabIndex = 3;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(45, 32);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(220, 13);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Quick and easy run-down of all known errors.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(18, 19);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Damage Report";
            // 
            // dgvMain
            // 
            this.dgvMain.AllowUserToAddRows = false;
            this.dgvMain.AllowUserToDeleteRows = false;
            this.dgvMain.AllowUserToOrderColumns = true;
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmType,
            this.clmChunkPosX,
            this.clmChunkPosY,
            this.clmMessage});
            this.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMain.Location = new System.Drawing.Point(0, 64);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.ReadOnly = true;
            this.dgvMain.Size = new System.Drawing.Size(551, 234);
            this.dgvMain.TabIndex = 4;
            // 
            // clmType
            // 
            this.clmType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.clmType.HeaderText = "Type";
            this.clmType.Name = "clmType";
            this.clmType.ReadOnly = true;
            this.clmType.Width = 37;
            // 
            // clmChunkPosX
            // 
            this.clmChunkPosX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmChunkPosX.HeaderText = "X";
            this.clmChunkPosX.Name = "clmChunkPosX";
            this.clmChunkPosX.ReadOnly = true;
            this.clmChunkPosX.Width = 39;
            // 
            // clmChunkPosY
            // 
            this.clmChunkPosY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmChunkPosY.HeaderText = "Y";
            this.clmChunkPosY.Name = "clmChunkPosY";
            this.clmChunkPosY.ReadOnly = true;
            this.clmChunkPosY.Width = 39;
            // 
            // clmMessage
            // 
            this.clmMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmMessage.HeaderText = "Message";
            this.clmMessage.Name = "clmMessage";
            this.clmMessage.ReadOnly = true;
            // 
            // frmReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 362);
            this.Controls.Add(this.dgvMain);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlControls);
            this.Name = "frmReport";
            this.Text = "Task Completion Report";
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button cmdFixAll;
        private System.Windows.Forms.Button cmdDoNothing;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblSubtitle;
        public System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.CheckBox chkInfo;
        private System.Windows.Forms.CheckBox chkWarnings;
        private System.Windows.Forms.CheckBox chkErrors;
        private System.Windows.Forms.DataGridViewImageColumn clmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmChunkPosX;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmChunkPosY;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMessage;

    }
}