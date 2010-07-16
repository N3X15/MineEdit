namespace MineEdit
{
    partial class dlgChunk
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMaxMin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCreation = new System.Windows.Forms.TextBox();
            this.txtChunkFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtChunkSz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChunkCoords = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdReload = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdRedoLighting = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMaxMin);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCreation);
            this.groupBox1.Controls.Add(this.txtChunkFile);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtChunkSz);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtChunkCoords);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 160);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chunk Information";
            // 
            // txtMaxMin
            // 
            this.txtMaxMin.Enabled = false;
            this.txtMaxMin.Location = new System.Drawing.Point(148, 122);
            this.txtMaxMin.Name = "txtMaxMin";
            this.txtMaxMin.Size = new System.Drawing.Size(121, 20);
            this.txtMaxMin.TabIndex = 10;
            this.txtMaxMin.Text = "0/0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Maximum/Minimum Height:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Creation Date:";
            // 
            // txtCreation
            // 
            this.txtCreation.Enabled = false;
            this.txtCreation.Location = new System.Drawing.Point(148, 96);
            this.txtCreation.Name = "txtCreation";
            this.txtCreation.Size = new System.Drawing.Size(121, 20);
            this.txtCreation.TabIndex = 7;
            // 
            // txtChunkFile
            // 
            this.txtChunkFile.Enabled = false;
            this.txtChunkFile.Location = new System.Drawing.Point(148, 70);
            this.txtChunkFile.Name = "txtChunkFile";
            this.txtChunkFile.Size = new System.Drawing.Size(202, 20);
            this.txtChunkFile.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Chunk File:";
            // 
            // txtChunkSz
            // 
            this.txtChunkSz.Enabled = false;
            this.txtChunkSz.Location = new System.Drawing.Point(148, 44);
            this.txtChunkSz.Name = "txtChunkSz";
            this.txtChunkSz.Size = new System.Drawing.Size(121, 20);
            this.txtChunkSz.TabIndex = 4;
            this.txtChunkSz.Text = "<x,y,z>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Chunk Scale:";
            // 
            // txtChunkCoords
            // 
            this.txtChunkCoords.Enabled = false;
            this.txtChunkCoords.Location = new System.Drawing.Point(148, 16);
            this.txtChunkCoords.Name = "txtChunkCoords";
            this.txtChunkCoords.Size = new System.Drawing.Size(96, 20);
            this.txtChunkCoords.TabIndex = 2;
            this.txtChunkCoords.Text = "(x,y)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chunk Coordinates:";
            // 
            // cmdReload
            // 
            this.cmdReload.Location = new System.Drawing.Point(387, 25);
            this.cmdReload.Name = "cmdReload";
            this.cmdReload.Size = new System.Drawing.Size(127, 23);
            this.cmdReload.TabIndex = 1;
            this.cmdReload.Text = "Reload";
            this.cmdReload.UseVisualStyleBackColor = true;
            this.cmdReload.Click += new System.EventHandler(this.cmdReload_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(387, 54);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(127, 23);
            this.cmdDelete.TabIndex = 2;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // cmdRedoLighting
            // 
            this.cmdRedoLighting.Enabled = false;
            this.cmdRedoLighting.Location = new System.Drawing.Point(387, 83);
            this.cmdRedoLighting.Name = "cmdRedoLighting";
            this.cmdRedoLighting.Size = new System.Drawing.Size(127, 23);
            this.cmdRedoLighting.TabIndex = 3;
            this.cmdRedoLighting.Text = "Recalc Lighting";
            this.cmdRedoLighting.UseVisualStyleBackColor = true;
            this.cmdRedoLighting.Click += new System.EventHandler(this.cmdRedoLighting_Click);
            // 
            // dlgChunk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 202);
            this.Controls.Add(this.cmdRedoLighting);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdReload);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgChunk";
            this.Text = "Details of Chunk (x,y)";
            this.Load += new System.EventHandler(this.dlgChunk_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMaxMin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCreation;
        private System.Windows.Forms.TextBox txtChunkFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtChunkSz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChunkCoords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdReload;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Button cmdRedoLighting;
    }
}