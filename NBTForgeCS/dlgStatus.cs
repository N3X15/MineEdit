using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class dlgStatus : Form
    {
        IMapHandler mMap;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(49, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(235, 49);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "Please wait...";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMessage.UseMnemonic = false;
            // 
            // pbIcon
            // 
            this.pbIcon.BackgroundImage = global::MineEdit.Properties.Resources.emblem_important;
            this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbIcon.Location = new System.Drawing.Point(0, 0);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(49, 49);
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            // 
            // dlgStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(284, 49);
            this.ControlBox = false;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbIcon);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgStatus";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "dlgStatus";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblMessage;
        public dlgStatus(IMapHandler mh,string message)
        {
            mMap = mh;
            InitializeComponent();
            lblMessage.Text = message;
            mMap.StatusUpdate += new StatusUpdateHandler(mMap_StatusUpdate);
        }

        void mMap_StatusUpdate(IMapHandler map, short status, string message)
        {
            if (status == 0)
            {
                Close();
                return;
            }
            lblMessage.Text = message;
        }
    }
}
