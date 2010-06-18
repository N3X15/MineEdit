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
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Location = new System.Drawing.Point(0, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(355, 15);
            this.lblError.TabIndex = 0;
            this.lblError.Text = "(Map is broken in this version, please check the thread or git for updates.)";
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblError);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(433, 356);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblError;
    }
}
