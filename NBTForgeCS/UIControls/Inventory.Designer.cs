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
            this.cmdDeleteInv = new System.Windows.Forms.Button();
            this.numDamage = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.cmdRepair = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdSuperRepair = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSaveTemplate = new System.Windows.Forms.ToolStripButton();
            this.tsbOpenTemplate = new System.Windows.Forms.ToolStripButton();
            this.splitInv.Panel2.SuspendLayout();
            this.splitInv.SuspendLayout();
            this.gbEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitInv
            // 
            this.splitInv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInv.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitInv.Location = new System.Drawing.Point(0, 0);
            this.splitInv.Name = "splitInv";
            // 
            // splitInv.Panel1
            // 
            this.splitInv.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitInv_Panel1_Paint);
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
            this.gbEdit.Controls.Add(this.cmdSuperRepair);
            this.gbEdit.Controls.Add(this.cmdRepair);
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
            // cmdDeleteInv
            // 
            this.cmdDeleteInv.Location = new System.Drawing.Point(80, 194);
            this.cmdDeleteInv.Name = "cmdDeleteInv";
            this.cmdDeleteInv.Size = new System.Drawing.Size(120, 23);
            this.cmdDeleteInv.TabIndex = 11;
            this.cmdDeleteInv.Text = "Delete Selected";
            this.cmdDeleteInv.UseVisualStyleBackColor = true;
            this.cmdDeleteInv.Click += new System.EventHandler(this.cmdDeleteInv_Click);
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
            // cmdRepair
            // 
            this.cmdRepair.Location = new System.Drawing.Point(80, 136);
            this.cmdRepair.Name = "cmdRepair";
            this.cmdRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdRepair.TabIndex = 8;
            this.cmdRepair.Text = "Repair Selected";
            this.cmdRepair.UseVisualStyleBackColor = true;
            this.cmdRepair.Click += new System.EventHandler(this.button1_Click);
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
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownWidth = 200;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(80, 28);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbType_DrawItem);
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
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
            // cmdSuperRepair
            // 
            this.cmdSuperRepair.Location = new System.Drawing.Point(80, 165);
            this.cmdSuperRepair.Name = "cmdSuperRepair";
            this.cmdSuperRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdSuperRepair.TabIndex = 8;
            this.cmdSuperRepair.Text = "Super Repair";
            this.cmdSuperRepair.UseVisualStyleBackColor = true;
            this.cmdSuperRepair.Click += new System.EventHandler(this.cmdSuperRepair_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSaveTemplate,
            this.tsbOpenTemplate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(552, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbSaveTemplate
            // 
            this.tsbSaveTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveTemplate.Image = global::MineEdit.Properties.Resources.document_save;
            this.tsbSaveTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveTemplate.Name = "tsbSaveTemplate";
            this.tsbSaveTemplate.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveTemplate.Text = "Save Template";
            this.tsbSaveTemplate.Click += new System.EventHandler(this.tsbSaveTemplate_Click);
            // 
            // tsbOpenTemplate
            // 
            this.tsbOpenTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenTemplate.Image = global::MineEdit.Properties.Resources.document_open;
            this.tsbOpenTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenTemplate.Name = "tsbOpenTemplate";
            this.tsbOpenTemplate.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenTemplate.Text = "Open Template";
            this.tsbOpenTemplate.Click += new System.EventHandler(this.tsbOpenTemplate_Click);
            // 
            // Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitInv);
            this.Name = "Inventory";
            this.Size = new System.Drawing.Size(552, 229);
            this.splitInv.Panel2.ResumeLayout(false);
            this.splitInv.ResumeLayout(false);
            this.gbEdit.ResumeLayout(false);
            this.gbEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitInv;
        private System.Windows.Forms.GroupBox gbEdit;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button cmdRepair;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numDamage;
        private System.Windows.Forms.Button cmdDeleteInv;
        private System.Windows.Forms.Button cmdSuperRepair;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbSaveTemplate;
        private System.Windows.Forms.ToolStripButton tsbOpenTemplate;

    }
}
