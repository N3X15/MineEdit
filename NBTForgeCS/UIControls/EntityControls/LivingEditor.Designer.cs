namespace MineEdit
{
    partial class LivingEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSheared = new System.Windows.Forms.CheckBox();
            this.chkSaddled = new System.Windows.Forms.CheckBox();
            this.cmdKill = new System.Windows.Forms.Button();
            this.numHealth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSheared);
            this.groupBox1.Controls.Add(this.chkSaddled);
            this.groupBox1.Controls.Add(this.cmdKill);
            this.groupBox1.Controls.Add(this.numHealth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 332);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Living Entity Editor";
            // 
            // chkSheared
            // 
            this.chkSheared.AutoSize = true;
            this.chkSheared.Location = new System.Drawing.Point(23, 130);
            this.chkSheared.Name = "chkSheared";
            this.chkSheared.Size = new System.Drawing.Size(66, 17);
            this.chkSheared.TabIndex = 4;
            this.chkSheared.Text = "Sheared";
            this.chkSheared.UseVisualStyleBackColor = true;
            this.chkSheared.CheckedChanged += new System.EventHandler(this.chkSheared_CheckedChanged);
            // 
            // chkSaddled
            // 
            this.chkSaddled.AutoSize = true;
            this.chkSaddled.Location = new System.Drawing.Point(23, 107);
            this.chkSaddled.Name = "chkSaddled";
            this.chkSaddled.Size = new System.Drawing.Size(59, 17);
            this.chkSaddled.TabIndex = 3;
            this.chkSaddled.Text = "Saddle";
            this.chkSaddled.UseVisualStyleBackColor = true;
            this.chkSaddled.CheckedChanged += new System.EventHandler(this.chkSaddled_CheckedChanged);
            // 
            // cmdKill
            // 
            this.cmdKill.Location = new System.Drawing.Point(23, 56);
            this.cmdKill.Name = "cmdKill";
            this.cmdKill.Size = new System.Drawing.Size(75, 23);
            this.cmdKill.TabIndex = 2;
            this.cmdKill.Text = "Kill";
            this.cmdKill.UseVisualStyleBackColor = true;
            this.cmdKill.Click += new System.EventHandler(this.cmdKill_Click);
            // 
            // numHealth
            // 
            this.numHealth.Location = new System.Drawing.Point(67, 30);
            this.numHealth.Name = "numHealth";
            this.numHealth.Size = new System.Drawing.Size(120, 20);
            this.numHealth.TabIndex = 1;
            this.numHealth.ValueChanged += new System.EventHandler(this.numHealth_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Health:";
            // 
            // LivingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "LivingEditor";
            this.Size = new System.Drawing.Size(314, 332);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdKill;
        private System.Windows.Forms.NumericUpDown numHealth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSheared;
        private System.Windows.Forms.CheckBox chkSaddled;
    }
}
