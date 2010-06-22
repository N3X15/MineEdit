namespace MineEdit
{
    partial class MapControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapControl));
            this.lblError = new System.Windows.Forms.Label();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLyrDown = new System.Windows.Forms.Button();
            this.btnLyrUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Location = new System.Drawing.Point(0, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(413, 15);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "The map in this version of MineEdit is screwy, so editing the terrain has been di" +
                "sabled.";
            this.lblError.Visible = false;
            // 
            // btnLeft
            // 
            this.btnLeft.FlatAppearance.BorderSize = 0;
            this.btnLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLeft.Image = ((System.Drawing.Image)(resources.GetObject("btnLeft.Image")));
            this.btnLeft.Location = new System.Drawing.Point(64, 163);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 1;
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnDown
            // 
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(110, 192);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 2;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.BackgroundImage = global::MineEdit.Properties.Resources.go_up;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Location = new System.Drawing.Point(110, 134);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 3;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRight
            // 
            this.btnRight.FlatAppearance.BorderSize = 0;
            this.btnRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRight.Image = ((System.Drawing.Image)(resources.GetObject("btnRight.Image")));
            this.btnRight.Location = new System.Drawing.Point(158, 163);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 4;
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLyrDown
            // 
            this.btnLyrDown.FlatAppearance.BorderSize = 0;
            this.btnLyrDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLyrDown.Image = ((System.Drawing.Image)(resources.GetObject("btnLyrDown.Image")));
            this.btnLyrDown.Location = new System.Drawing.Point(239, 192);
            this.btnLyrDown.Name = "btnLyrDown";
            this.btnLyrDown.Size = new System.Drawing.Size(75, 23);
            this.btnLyrDown.TabIndex = 2;
            this.btnLyrDown.UseVisualStyleBackColor = true;
            this.btnLyrDown.Click += new System.EventHandler(this.btnLyrDown_Click);
            // 
            // btnLyrUp
            // 
            this.btnLyrUp.FlatAppearance.BorderSize = 0;
            this.btnLyrUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLyrUp.Image = ((System.Drawing.Image)(resources.GetObject("btnLyrUp.Image")));
            this.btnLyrUp.Location = new System.Drawing.Point(239, 134);
            this.btnLyrUp.Name = "btnLyrUp";
            this.btnLyrUp.Size = new System.Drawing.Size(75, 23);
            this.btnLyrUp.TabIndex = 3;
            this.btnLyrUp.UseVisualStyleBackColor = true;
            this.btnLyrUp.Click += new System.EventHandler(this.btnLyrUp_Click);
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLyrUp);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnLyrDown);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.lblError);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(433, 356);
            this.Load += new System.EventHandler(this.MapControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLyrDown;
        private System.Windows.Forms.Button btnLyrUp;
    }
}
