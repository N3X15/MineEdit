namespace MineEdit
{
    partial class EntityEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdRemoveEntity = new System.Windows.Forms.Button();
            this.cmdAddEntity = new System.Windows.Forms.Button();
            this.cmbEntities = new System.Windows.Forms.ComboBox();
            this.spltEnt = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numEntPosZ = new System.Windows.Forms.NumericUpDown();
            this.numEntPosY = new System.Windows.Forms.NumericUpDown();
            this.numEntPosX = new System.Windows.Forms.NumericUpDown();
            this.cmdEntToSpawn = new System.Windows.Forms.Button();
            this.cmdEntToMe = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.spltEnt.Panel1.SuspendLayout();
            this.spltEnt.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosX)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spltEnt);
            this.splitContainer1.Size = new System.Drawing.Size(565, 429);
            this.splitContainer1.SplitterDistance = 32;
            this.splitContainer1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdRemoveEntity);
            this.panel1.Controls.Add(this.cmdAddEntity);
            this.panel1.Controls.Add(this.cmbEntities);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(565, 32);
            this.panel1.TabIndex = 0;
            // 
            // cmdRemoveEntity
            // 
            this.cmdRemoveEntity.Location = new System.Drawing.Point(487, 3);
            this.cmdRemoveEntity.Name = "cmdRemoveEntity";
            this.cmdRemoveEntity.Size = new System.Drawing.Size(75, 23);
            this.cmdRemoveEntity.TabIndex = 2;
            this.cmdRemoveEntity.Text = "Remove";
            this.cmdRemoveEntity.UseVisualStyleBackColor = true;
            this.cmdRemoveEntity.Click += new System.EventHandler(this.cmdRemoveEntity_Click);
            // 
            // cmdAddEntity
            // 
            this.cmdAddEntity.Enabled = false;
            this.cmdAddEntity.Location = new System.Drawing.Point(430, 3);
            this.cmdAddEntity.Name = "cmdAddEntity";
            this.cmdAddEntity.Size = new System.Drawing.Size(51, 23);
            this.cmdAddEntity.TabIndex = 1;
            this.cmdAddEntity.Text = "Add...";
            this.cmdAddEntity.UseVisualStyleBackColor = true;
            // 
            // cmbEntities
            // 
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Location = new System.Drawing.Point(9, 5);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(415, 21);
            this.cmbEntities.TabIndex = 0;
            this.cmbEntities.SelectedIndexChanged += new System.EventHandler(this.cmbEntities_SelectedIndexChanged);
            // 
            // spltEnt
            // 
            this.spltEnt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltEnt.Location = new System.Drawing.Point(0, 0);
            this.spltEnt.Name = "spltEnt";
            // 
            // spltEnt.Panel1
            // 
            this.spltEnt.Panel1.Controls.Add(this.groupBox1);
            // 
            // spltEnt.Panel2
            // 
            this.spltEnt.Panel2.AutoScroll = true;
            this.spltEnt.Size = new System.Drawing.Size(565, 393);
            this.spltEnt.SplitterDistance = 214;
            this.spltEnt.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numEntPosZ);
            this.groupBox1.Controls.Add(this.numEntPosY);
            this.groupBox1.Controls.Add(this.numEntPosX);
            this.groupBox1.Controls.Add(this.cmdEntToSpawn);
            this.groupBox1.Controls.Add(this.cmdEntToMe);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 393);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Common Entity Properties";
            // 
            // numEntPosZ
            // 
            this.numEntPosZ.DecimalPlaces = 2;
            this.numEntPosZ.Location = new System.Drawing.Point(149, 51);
            this.numEntPosZ.Name = "numEntPosZ";
            this.numEntPosZ.Size = new System.Drawing.Size(64, 20);
            this.numEntPosZ.TabIndex = 2;
            this.numEntPosZ.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // numEntPosY
            // 
            this.numEntPosY.DecimalPlaces = 2;
            this.numEntPosY.Location = new System.Drawing.Point(79, 51);
            this.numEntPosY.Name = "numEntPosY";
            this.numEntPosY.Size = new System.Drawing.Size(64, 20);
            this.numEntPosY.TabIndex = 2;
            this.numEntPosY.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // numEntPosX
            // 
            this.numEntPosX.DecimalPlaces = 2;
            this.numEntPosX.Location = new System.Drawing.Point(9, 51);
            this.numEntPosX.Name = "numEntPosX";
            this.numEntPosX.Size = new System.Drawing.Size(64, 20);
            this.numEntPosX.TabIndex = 2;
            this.numEntPosX.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // cmdEntToSpawn
            // 
            this.cmdEntToSpawn.Location = new System.Drawing.Point(119, 22);
            this.cmdEntToSpawn.Name = "cmdEntToSpawn";
            this.cmdEntToSpawn.Size = new System.Drawing.Size(69, 23);
            this.cmdEntToSpawn.TabIndex = 1;
            this.cmdEntToSpawn.Text = "To Spawn";
            this.cmdEntToSpawn.UseVisualStyleBackColor = true;
            this.cmdEntToSpawn.Click += new System.EventHandler(this.cmdEntToSpawn_Click);
            // 
            // cmdEntToMe
            // 
            this.cmdEntToMe.Location = new System.Drawing.Point(59, 22);
            this.cmdEntToMe.Name = "cmdEntToMe";
            this.cmdEntToMe.Size = new System.Drawing.Size(54, 23);
            this.cmdEntToMe.TabIndex = 1;
            this.cmdEntToMe.Text = "To Me";
            this.cmdEntToMe.UseVisualStyleBackColor = true;
            this.cmdEntToMe.Click += new System.EventHandler(this.cmdEntToMe_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Position:";
            // 
            // EntityEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EntityEditor";
            this.Size = new System.Drawing.Size(565, 429);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.spltEnt.Panel1.ResumeLayout(false);
            this.spltEnt.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdRemoveEntity;
        private System.Windows.Forms.Button cmdAddEntity;
        private System.Windows.Forms.ComboBox cmbEntities;
        private System.Windows.Forms.SplitContainer spltEnt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numEntPosZ;
        private System.Windows.Forms.NumericUpDown numEntPosY;
        private System.Windows.Forms.NumericUpDown numEntPosX;
        private System.Windows.Forms.Button cmdEntToSpawn;
        private System.Windows.Forms.Button cmdEntToMe;
        private System.Windows.Forms.Label label6;


    }
}
