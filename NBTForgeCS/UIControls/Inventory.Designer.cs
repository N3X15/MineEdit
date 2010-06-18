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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tbDamage = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.numSlot = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSlot = new System.Windows.Forms.Label();
            this.splitInv.Panel2.SuspendLayout();
            this.splitInv.SuspendLayout();
            this.gbEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlot)).BeginInit();
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
            this.gbEdit.Controls.Add(this.button2);
            this.gbEdit.Controls.Add(this.button1);
            this.gbEdit.Controls.Add(this.tbDamage);
            this.gbEdit.Controls.Add(this.label3);
            this.gbEdit.Controls.Add(this.numCount);
            this.gbEdit.Controls.Add(this.label2);
            this.gbEdit.Controls.Add(this.cmbType);
            this.gbEdit.Controls.Add(this.numSlot);
            this.gbEdit.Controls.Add(this.label1);
            this.gbEdit.Controls.Add(this.lblSlot);
            this.gbEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEdit.Location = new System.Drawing.Point(0, 0);
            this.gbEdit.Name = "gbEdit";
            this.gbEdit.Size = new System.Drawing.Size(248, 229);
            this.gbEdit.TabIndex = 0;
            this.gbEdit.TabStop = false;
            this.gbEdit.Text = "Inventory Editor";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(80, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Apply to Selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Repair Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbDamage
            // 
            this.tbDamage.LargeChange = 100;
            this.tbDamage.Location = new System.Drawing.Point(80, 106);
            this.tbDamage.Maximum = 500;
            this.tbDamage.Name = "tbDamage";
            this.tbDamage.Size = new System.Drawing.Size(162, 45);
            this.tbDamage.SmallChange = 10;
            this.tbDamage.TabIndex = 7;
            this.tbDamage.TickFrequency = 100;
            this.tbDamage.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Damage:";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(80, 80);
            this.numCount.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
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
            this.label2.Location = new System.Drawing.Point(36, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Count:";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(80, 53);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // numSlot
            // 
            this.numSlot.Location = new System.Drawing.Point(80, 27);
            this.numSlot.Name = "numSlot";
            this.numSlot.Size = new System.Drawing.Size(120, 20);
            this.numSlot.TabIndex = 1;
            this.numSlot.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Object Type:";
            // 
            // lblSlot
            // 
            this.lblSlot.AutoSize = true;
            this.lblSlot.Location = new System.Drawing.Point(46, 29);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Size = new System.Drawing.Size(28, 13);
            this.lblSlot.TabIndex = 0;
            this.lblSlot.Text = "Slot:";
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
            ((System.ComponentModel.ISupportInitialize)(this.tbDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitInv;
        private System.Windows.Forms.GroupBox gbEdit;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TrackBar tbDamage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.NumericUpDown numSlot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSlot;

    }
}
