namespace MineEdit
{
    partial class frmMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMap));
            this.mapPic = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tscbMaterial = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuBrush3D = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBrushSphere = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBrushHollow = new System.Windows.Forms.ToolStripMenuItem();
            this.tscbBrushSize = new System.Windows.Forms.ToolStripComboBox();
            this.cbViewingStyle = new System.Windows.Forms.ToolStripComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.invMain = new MineEdit.Inventory();
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapPic
            // 
            this.mapPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPic.Location = new System.Drawing.Point(0, 0);
            this.mapPic.Name = "mapPic";
            this.mapPic.Size = new System.Drawing.Size(569, 343);
            this.mapPic.TabIndex = 0;
            this.mapPic.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbMaterial,
            this.toolStripDropDownButton1,
            this.tscbBrushSize,
            this.cbViewingStyle});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(569, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tscbMaterial
            // 
            this.tscbMaterial.Name = "tscbMaterial";
            this.tscbMaterial.Size = new System.Drawing.Size(121, 25);
            this.tscbMaterial.Text = "Materials...";
            this.tscbMaterial.SelectedIndexChanged += new System.EventHandler(this.tscbMaterial_SelectedIndexChanged);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBrush3D,
            this.mnuBrushSphere,
            this.mnuBrushHollow});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(80, 22);
            this.toolStripDropDownButton1.Text = "Brush Flags";
            // 
            // mnuBrush3D
            // 
            this.mnuBrush3D.Name = "mnuBrush3D";
            this.mnuBrush3D.Size = new System.Drawing.Size(122, 22);
            this.mnuBrush3D.Text = "3D";
            this.mnuBrush3D.Click += new System.EventHandler(this.dToolStripMenuItem_Click);
            // 
            // mnuBrushSphere
            // 
            this.mnuBrushSphere.Name = "mnuBrushSphere";
            this.mnuBrushSphere.Size = new System.Drawing.Size(122, 22);
            this.mnuBrushSphere.Text = "Spherical";
            this.mnuBrushSphere.Click += new System.EventHandler(this.mnuBrushSphere_Click);
            // 
            // mnuBrushHollow
            // 
            this.mnuBrushHollow.Name = "mnuBrushHollow";
            this.mnuBrushHollow.Size = new System.Drawing.Size(122, 22);
            this.mnuBrushHollow.Text = "Hollow";
            this.mnuBrushHollow.Click += new System.EventHandler(this.mnuBrushHollow_Click);
            // 
            // tscbBrushSize
            // 
            this.tscbBrushSize.DropDownWidth = 64;
            this.tscbBrushSize.Name = "tscbBrushSize";
            this.tscbBrushSize.Size = new System.Drawing.Size(75, 25);
            this.tscbBrushSize.Text = "Brush Size";
            // 
            // cbViewingStyle
            // 
            this.cbViewingStyle.Items.AddRange(new object[] {
            "Top Down",
            "Slice E-W",
            "Slice N-S"});
            this.cbViewingStyle.Name = "cbViewingStyle";
            this.cbViewingStyle.Size = new System.Drawing.Size(121, 25);
            this.cbViewingStyle.SelectedIndexChanged += new System.EventHandler(this.cbViewingStyle_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMap);
            this.tabControl1.Controls.Add(this.tabInventory);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(569, 318);
            this.tabControl1.TabIndex = 2;
            // 
            // tabMap
            // 
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(561, 292);
            this.tabMap.TabIndex = 0;
            this.tabMap.Text = "Map";
            this.tabMap.UseVisualStyleBackColor = true;
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.invMain);
            this.tabInventory.Location = new System.Drawing.Point(4, 22);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new System.Windows.Forms.Padding(3);
            this.tabInventory.Size = new System.Drawing.Size(561, 292);
            this.tabInventory.TabIndex = 1;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            // 
            // invMain
            // 
            this.invMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.invMain.Location = new System.Drawing.Point(3, 3);
            this.invMain.Map = null;
            this.invMain.Name = "invMain";
            this.invMain.Size = new System.Drawing.Size(555, 286);
            this.invMain.TabIndex = 0;
            // 
            // frmMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 343);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.mapPic);
            this.Name = "frmMap";
            this.Text = "frmMap";
            this.Load += new System.EventHandler(this.frmMap_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMap_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mapPic;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox tscbMaterial;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem mnuBrush3D;
        private System.Windows.Forms.ToolStripMenuItem mnuBrushSphere;
        private System.Windows.Forms.ToolStripMenuItem mnuBrushHollow;
        private System.Windows.Forms.ToolStripComboBox tscbBrushSize;
        private System.Windows.Forms.ToolStripComboBox cbViewingStyle;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMap;
        private System.Windows.Forms.TabPage tabInventory;
        private Inventory invMain;

    }
}