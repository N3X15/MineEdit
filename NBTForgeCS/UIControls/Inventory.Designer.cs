namespace MineEdit
{
    partial class Inventory
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitInv = new System.Windows.Forms.SplitContainer();
            this.gbEdit = new System.Windows.Forms.GroupBox();
            this.numDamage = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdDeleteInv = new System.Windows.Forms.Button();
            this.splitInv.Panel2.SuspendLayout();
            this.splitInv.SuspendLayout();
            this.gbEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            this.SuspendLayout();
            // 
            // splitInv
            // 
            this.splitInv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInv.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitInv.Location = new System.Drawing.Point(0, 0);
            this.splitInv.Name = "splitInv";
            this.splitInv.Panel1MinSize = 300;
            // 
            // splitInv.Panel2
            // 
            this.splitInv.Panel2.Controls.Add(this.gbEdit);
            this.splitInv.Size = new System.Drawing.Size(552, 229);
            this.splitInv.SplitterDistance = 300;
            this.splitInv.TabIndex = 0;
            // 
            // gbEdit
            // 
            this.gbEdit.Controls.Add(this.cmdDeleteInv);
            this.gbEdit.Controls.Add(this.numDamage);
            this.gbEdit.Controls.Add(this.button2);
            this.gbEdit.Controls.Add(this.button1);
            this.gbEdit.Controls.Add(this.label3);
            this.gbEdit.Controls.Add(this.numCount);
            this.gbEdit.Controls.Add(this.label2);
            this.gbEdit.Controls.Add(this.cmbType);
            this.gbEdit.Controls.Add(this.label1);
            this.gbEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEdit.Location = new System.Drawing.Point(0, 0);
            this.gbEdit.Name = "gbEdit";
            this.gbEdit.Size = new System.Drawing.Size(248, 229);
            this.gbEdit.TabIndex = 0;
            this.gbEdit.TabStop = false;
            this.gbEdit.Text = "Inventory Editor";
            // 
            // numDamage
            // 
            this.numDamage.Location = new System.Drawing.Point(80, 81);
            this.numDamage.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numDamage.Minimum = new decimal(new int[] {
            600,
            0,
            0,
            -2147483648});
            this.numDamage.Name = "numDamage";
            this.numDamage.Size = new System.Drawing.Size(120, 20);
            this.numDamage.TabIndex = 10;
            this.numDamage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDamage.ValueChanged += new System.EventHandler(this.numDamage_ValueChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(80, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Apply to Selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 136);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Repair Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Damage:";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(80, 55);
            this.numCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(120, 20);
            this.numCount.TabIndex = 4;
            this.numCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Count:";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(80, 28);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Object Type:";
            // 
            // cmdDeleteInv
            // 
            this.cmdDeleteInv.Location = new System.Drawing.Point(80, 165);
            this.cmdDeleteInv.Name = "cmdDeleteInv";
            this.cmdDeleteInv.Size = new System.Drawing.Size(120, 23);
            this.cmdDeleteInv.TabIndex = 11;
            this.cmdDeleteInv.Text = "Delete Selected";
            this.cmdDeleteInv.UseVisualStyleBackColor = true;
            this.cmdDeleteInv.Click += new System.EventHandler(this.cmdDeleteInv_Click);
            // 
            // Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitInv);
            this.Name = "Inventory";
            this.Size = new System.Drawing.Size(552, 229);
            this.splitInv.Panel2.ResumeLayout(false);
            this.splitInv.ResumeLayout(false);
            this.gbEdit.ResumeLayout(false);
            this.gbEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitInv;
        private System.Windows.Forms.GroupBox gbEdit;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numDamage;
        private System.Windows.Forms.Button cmdDeleteInv;

    }
}
