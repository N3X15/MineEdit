namespace MineEdit
{
    partial class ItemEditor
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
            this.grpItem = new System.Windows.Forms.GroupBox();
            this.numDamage = new System.Windows.Forms.NumericUpDown();
            this.cmdSuperRepair = new System.Windows.Forms.Button();
            this.cmdRepair = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            this.SuspendLayout();
            // 
            // grpItem
            // 
            this.grpItem.Controls.Add(this.numDamage);
            this.grpItem.Controls.Add(this.cmdSuperRepair);
            this.grpItem.Controls.Add(this.cmdRepair);
            this.grpItem.Controls.Add(this.label3);
            this.grpItem.Controls.Add(this.numCount);
            this.grpItem.Controls.Add(this.label2);
            this.grpItem.Controls.Add(this.cmbType);
            this.grpItem.Controls.Add(this.label1);
            this.grpItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpItem.Location = new System.Drawing.Point(0, 0);
            this.grpItem.Name = "grpItem";
            this.grpItem.Size = new System.Drawing.Size(304, 312);
            this.grpItem.TabIndex = 0;
            this.grpItem.TabStop = false;
            this.grpItem.Text = "Item Editor";
            // 
            // numDamage
            // 
            this.numDamage.Location = new System.Drawing.Point(91, 72);
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
            this.numDamage.TabIndex = 17;
            this.numDamage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDamage.ValueChanged += new System.EventHandler(this.numDamage_ValueChanged);
            // 
            // cmdSuperRepair
            // 
            this.cmdSuperRepair.Location = new System.Drawing.Point(91, 127);
            this.cmdSuperRepair.Name = "cmdSuperRepair";
            this.cmdSuperRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdSuperRepair.TabIndex = 16;
            this.cmdSuperRepair.Text = "Super Repair";
            this.cmdSuperRepair.UseVisualStyleBackColor = true;
            this.cmdSuperRepair.Click += new System.EventHandler(this.cmdSuperRepair_Click);
            // 
            // cmdRepair
            // 
            this.cmdRepair.Location = new System.Drawing.Point(91, 98);
            this.cmdRepair.Name = "cmdRepair";
            this.cmdRepair.Size = new System.Drawing.Size(120, 23);
            this.cmdRepair.TabIndex = 16;
            this.cmdRepair.Text = "Repair";
            this.cmdRepair.UseVisualStyleBackColor = true;
            this.cmdRepair.Click += new System.EventHandler(this.cmdRepair_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Damage:";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(91, 46);
            this.numCount.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(120, 20);
            this.numCount.TabIndex = 14;
            this.numCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.ValueChanged += new System.EventHandler(this.numCount_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Count:";
            // 
            // cmbType
            // 
            this.cmbType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownWidth = 200;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(91, 19);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 21);
            this.cmbType.TabIndex = 12;
            this.cmbType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbType_DrawItem);
            this.cmbType.SelectionChangeCommitted += new System.EventHandler(this.cmbType_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Object Type:";
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpItem);
            this.Name = "ItemEditor";
            this.Size = new System.Drawing.Size(304, 312);
            this.grpItem.ResumeLayout(false);
            this.grpItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpItem;
        private System.Windows.Forms.NumericUpDown numDamage;
        private System.Windows.Forms.Button cmdRepair;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdSuperRepair;
    }
}
