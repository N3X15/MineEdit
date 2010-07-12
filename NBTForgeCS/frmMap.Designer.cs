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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tscbMaterial = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuBrush3D = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBrushSphere = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBrushHollow = new System.Windows.Forms.ToolStripMenuItem();
            this.tscbBrushSize = new System.Windows.Forms.ToolStripComboBox();
            this.cbViewingStyle = new System.Windows.Forms.ToolStripComboBox();
            this.tclMap = new System.Windows.Forms.TabControl();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.invMain = new MineEdit.Inventory();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdHeal = new System.Windows.Forms.Button();
            this.cmdSpawn = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numVelZ = new System.Windows.Forms.NumericUpDown();
            this.numPosZ = new System.Windows.Forms.NumericUpDown();
            this.numVelY = new System.Windows.Forms.NumericUpDown();
            this.numVelX = new System.Windows.Forms.NumericUpDown();
            this.numPosY = new System.Windows.Forms.NumericUpDown();
            this.numPosX = new System.Windows.Forms.NumericUpDown();
            this.numHurtTime = new System.Windows.Forms.NumericUpDown();
            this.numAir = new System.Windows.Forms.NumericUpDown();
            this.lblHurt = new System.Windows.Forms.Label();
            this.numFire = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numHealth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabEnvironment = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdDefrost = new System.Windows.Forms.Button();
            this.chkSnow = new System.Windows.Forms.CheckBox();
            this.grpSpawn = new System.Windows.Forms.GroupBox();
            this.cmdSetSpawnToPos = new System.Windows.Forms.Button();
            this.numSpawnZ = new System.Windows.Forms.NumericUpDown();
            this.numSpawnY = new System.Windows.Forms.NumericUpDown();
            this.numSpawnX = new System.Windows.Forms.NumericUpDown();
            this.grpTOD = new System.Windows.Forms.GroupBox();
            this.cmdNight = new System.Windows.Forms.Button();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.cmdDay = new System.Windows.Forms.Button();
            this.tabEnts = new System.Windows.Forms.TabPage();
            this.entityEditor1 = new MineEdit.EntityEditor();
            this.tabTEnts = new System.Windows.Forms.TabPage();
            this.tileEntityEditor1 = new MineEdit.TileEntityEditor();
            this.mapPic = new System.Windows.Forms.PictureBox();
            this.toolStrip1.SuspendLayout();
            this.tclMap.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVelZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHurtTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).BeginInit();
            this.tabEnvironment.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpSpawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnX)).BeginInit();
            this.grpTOD.SuspendLayout();
            this.tabEnts.SuspendLayout();
            this.tabTEnts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).BeginInit();
            this.SuspendLayout();
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
            this.toolStrip1.Visible = false;
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
            // tclMap
            // 
            this.tclMap.Controls.Add(this.tabMap);
            this.tclMap.Controls.Add(this.tabInventory);
            this.tclMap.Controls.Add(this.tabPage1);
            this.tclMap.Controls.Add(this.tabEnvironment);
            this.tclMap.Controls.Add(this.tabEnts);
            this.tclMap.Controls.Add(this.tabTEnts);
            this.tclMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tclMap.Location = new System.Drawing.Point(0, 0);
            this.tclMap.Name = "tclMap";
            this.tclMap.SelectedIndex = 0;
            this.tclMap.Size = new System.Drawing.Size(597, 343);
            this.tclMap.TabIndex = 2;
            // 
            // tabMap
            // 
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(589, 317);
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
            this.tabInventory.Size = new System.Drawing.Size(589, 317);
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
            this.invMain.Size = new System.Drawing.Size(583, 311);
            this.invMain.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.cmdCancel);
            this.tabPage1.Controls.Add(this.cmdApply);
            this.tabPage1.Controls.Add(this.cmdHeal);
            this.tabPage1.Controls.Add(this.cmdSpawn);
            this.tabPage1.Controls.Add(this.cmdStop);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.numVelZ);
            this.tabPage1.Controls.Add(this.numPosZ);
            this.tabPage1.Controls.Add(this.numVelY);
            this.tabPage1.Controls.Add(this.numVelX);
            this.tabPage1.Controls.Add(this.numPosY);
            this.tabPage1.Controls.Add(this.numPosX);
            this.tabPage1.Controls.Add(this.numHurtTime);
            this.tabPage1.Controls.Add(this.numAir);
            this.tabPage1.Controls.Add(this.lblHurt);
            this.tabPage1.Controls.Add(this.numFire);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.numHealth);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(589, 317);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Player";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(249, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(262, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "(Know the value range for this?  Tell me in the thread!)";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(204, 194);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 10;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdApply
            // 
            this.cmdApply.Location = new System.Drawing.Point(123, 194);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(75, 23);
            this.cmdApply.TabIndex = 10;
            this.cmdApply.Text = "&Apply";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // cmdHeal
            // 
            this.cmdHeal.Location = new System.Drawing.Point(249, 19);
            this.cmdHeal.Name = "cmdHeal";
            this.cmdHeal.Size = new System.Drawing.Size(75, 23);
            this.cmdHeal.TabIndex = 9;
            this.cmdHeal.Text = "Heal";
            this.cmdHeal.UseVisualStyleBackColor = true;
            this.cmdHeal.Click += new System.EventHandler(this.cmdHeal_Click);
            // 
            // cmdSpawn
            // 
            this.cmdSpawn.Location = new System.Drawing.Point(501, 139);
            this.cmdSpawn.Name = "cmdSpawn";
            this.cmdSpawn.Size = new System.Drawing.Size(54, 23);
            this.cmdSpawn.TabIndex = 8;
            this.cmdSpawn.Text = "Spawn";
            this.cmdSpawn.UseVisualStyleBackColor = true;
            this.cmdSpawn.Click += new System.EventHandler(this.cmdSpawn_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Enabled = false;
            this.cmdStop.Location = new System.Drawing.Point(501, 165);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(54, 23);
            this.cmdStop.TabIndex = 7;
            this.cmdStop.Text = "Stop";
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(70, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Velocity:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Position:";
            // 
            // numVelZ
            // 
            this.numVelZ.Enabled = false;
            this.numVelZ.Location = new System.Drawing.Point(375, 168);
            this.numVelZ.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numVelZ.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numVelZ.Name = "numVelZ";
            this.numVelZ.Size = new System.Drawing.Size(120, 20);
            this.numVelZ.TabIndex = 4;
            this.numVelZ.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numPosZ
            // 
            this.numPosZ.Location = new System.Drawing.Point(375, 142);
            this.numPosZ.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPosZ.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numPosZ.Name = "numPosZ";
            this.numPosZ.Size = new System.Drawing.Size(120, 20);
            this.numPosZ.TabIndex = 4;
            this.numPosZ.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numVelY
            // 
            this.numVelY.Enabled = false;
            this.numVelY.Location = new System.Drawing.Point(249, 168);
            this.numVelY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numVelY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numVelY.Name = "numVelY";
            this.numVelY.Size = new System.Drawing.Size(120, 20);
            this.numVelY.TabIndex = 4;
            this.numVelY.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numVelX
            // 
            this.numVelX.Enabled = false;
            this.numVelX.Location = new System.Drawing.Point(123, 168);
            this.numVelX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numVelX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numVelX.Name = "numVelX";
            this.numVelX.Size = new System.Drawing.Size(120, 20);
            this.numVelX.TabIndex = 4;
            this.numVelX.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numPosY
            // 
            this.numPosY.Location = new System.Drawing.Point(249, 142);
            this.numPosY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPosY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numPosY.Name = "numPosY";
            this.numPosY.Size = new System.Drawing.Size(120, 20);
            this.numPosY.TabIndex = 4;
            this.numPosY.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numPosX
            // 
            this.numPosX.Location = new System.Drawing.Point(123, 142);
            this.numPosX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPosX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numPosX.Name = "numPosX";
            this.numPosX.Size = new System.Drawing.Size(120, 20);
            this.numPosX.TabIndex = 4;
            this.numPosX.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numHurtTime
            // 
            this.numHurtTime.Location = new System.Drawing.Point(123, 100);
            this.numHurtTime.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numHurtTime.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.numHurtTime.Name = "numHurtTime";
            this.numHurtTime.Size = new System.Drawing.Size(120, 20);
            this.numHurtTime.TabIndex = 3;
            this.numHurtTime.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // numAir
            // 
            this.numAir.Location = new System.Drawing.Point(123, 74);
            this.numAir.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numAir.Name = "numAir";
            this.numAir.Size = new System.Drawing.Size(120, 20);
            this.numAir.TabIndex = 3;
            this.numAir.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // lblHurt
            // 
            this.lblHurt.AutoSize = true;
            this.lblHurt.Location = new System.Drawing.Point(64, 102);
            this.lblHurt.Name = "lblHurt";
            this.lblHurt.Size = new System.Drawing.Size(53, 13);
            this.lblHurt.TabIndex = 2;
            this.lblHurt.Text = "HurtTime:";
            // 
            // numFire
            // 
            this.numFire.Location = new System.Drawing.Point(123, 48);
            this.numFire.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFire.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numFire.Name = "numFire";
            this.numFire.Size = new System.Drawing.Size(120, 20);
            this.numFire.TabIndex = 3;
            this.numFire.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(95, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Air:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Fire:";
            // 
            // numHealth
            // 
            this.numHealth.Location = new System.Drawing.Point(123, 22);
            this.numHealth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numHealth.Name = "numHealth";
            this.numHealth.Size = new System.Drawing.Size(120, 20);
            this.numHealth.TabIndex = 1;
            this.numHealth.ValueChanged += new System.EventHandler(this.UnlockApplyCancel);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Health:";
            // 
            // tabEnvironment
            // 
            this.tabEnvironment.Controls.Add(this.groupBox1);
            this.tabEnvironment.Controls.Add(this.grpSpawn);
            this.tabEnvironment.Controls.Add(this.grpTOD);
            this.tabEnvironment.Location = new System.Drawing.Point(4, 22);
            this.tabEnvironment.Name = "tabEnvironment";
            this.tabEnvironment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnvironment.Size = new System.Drawing.Size(589, 317);
            this.tabEnvironment.TabIndex = 3;
            this.tabEnvironment.Text = "Environment";
            this.tabEnvironment.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdDefrost);
            this.groupBox1.Controls.Add(this.chkSnow);
            this.groupBox1.Location = new System.Drawing.Point(8, 147);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(545, 100);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Weather Settings";
            // 
            // cmdDefrost
            // 
            this.cmdDefrost.Location = new System.Drawing.Point(160, 15);
            this.cmdDefrost.Name = "cmdDefrost";
            this.cmdDefrost.Size = new System.Drawing.Size(119, 23);
            this.cmdDefrost.TabIndex = 1;
            this.cmdDefrost.Text = "Remove Snow && Ice";
            this.cmdDefrost.UseVisualStyleBackColor = true;
            this.cmdDefrost.Click += new System.EventHandler(this.cmdDefrost_Click);
            // 
            // chkSnow
            // 
            this.chkSnow.AutoSize = true;
            this.chkSnow.Location = new System.Drawing.Point(13, 19);
            this.chkSnow.Name = "chkSnow";
            this.chkSnow.Size = new System.Drawing.Size(141, 17);
            this.chkSnow.TabIndex = 0;
            this.chkSnow.Text = "Map is covered in snow.";
            this.chkSnow.UseVisualStyleBackColor = true;
            this.chkSnow.CheckedChanged += new System.EventHandler(this.chkSnow_CheckedChanged);
            // 
            // grpSpawn
            // 
            this.grpSpawn.Controls.Add(this.cmdSetSpawnToPos);
            this.grpSpawn.Controls.Add(this.numSpawnZ);
            this.grpSpawn.Controls.Add(this.numSpawnY);
            this.grpSpawn.Controls.Add(this.numSpawnX);
            this.grpSpawn.Location = new System.Drawing.Point(8, 62);
            this.grpSpawn.Name = "grpSpawn";
            this.grpSpawn.Size = new System.Drawing.Size(545, 79);
            this.grpSpawn.TabIndex = 7;
            this.grpSpawn.TabStop = false;
            this.grpSpawn.Text = "Spawn";
            // 
            // cmdSetSpawnToPos
            // 
            this.cmdSetSpawnToPos.Location = new System.Drawing.Point(13, 45);
            this.cmdSetSpawnToPos.Name = "cmdSetSpawnToPos";
            this.cmdSetSpawnToPos.Size = new System.Drawing.Size(145, 23);
            this.cmdSetSpawnToPos.TabIndex = 8;
            this.cmdSetSpawnToPos.Text = "Set Spawn to Player Pos";
            this.cmdSetSpawnToPos.UseVisualStyleBackColor = true;
            this.cmdSetSpawnToPos.Click += new System.EventHandler(this.cmdSetSpawnToPos_Click);
            // 
            // numSpawnZ
            // 
            this.numSpawnZ.Location = new System.Drawing.Point(265, 19);
            this.numSpawnZ.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpawnZ.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpawnZ.Name = "numSpawnZ";
            this.numSpawnZ.Size = new System.Drawing.Size(120, 20);
            this.numSpawnZ.TabIndex = 7;
            this.numSpawnZ.ValueChanged += new System.EventHandler(this.numSpawnZ_ValueChanged);
            // 
            // numSpawnY
            // 
            this.numSpawnY.Location = new System.Drawing.Point(139, 19);
            this.numSpawnY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpawnY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpawnY.Name = "numSpawnY";
            this.numSpawnY.Size = new System.Drawing.Size(120, 20);
            this.numSpawnY.TabIndex = 6;
            this.numSpawnY.ValueChanged += new System.EventHandler(this.numSpawnY_ValueChanged);
            // 
            // numSpawnX
            // 
            this.numSpawnX.Location = new System.Drawing.Point(13, 19);
            this.numSpawnX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSpawnX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numSpawnX.Name = "numSpawnX";
            this.numSpawnX.Size = new System.Drawing.Size(120, 20);
            this.numSpawnX.TabIndex = 5;
            this.numSpawnX.ValueChanged += new System.EventHandler(this.numSpawnX_ValueChanged);
            // 
            // grpTOD
            // 
            this.grpTOD.Controls.Add(this.cmdNight);
            this.grpTOD.Controls.Add(this.txtTime);
            this.grpTOD.Controls.Add(this.cmdDay);
            this.grpTOD.Location = new System.Drawing.Point(8, 6);
            this.grpTOD.Name = "grpTOD";
            this.grpTOD.Size = new System.Drawing.Size(545, 50);
            this.grpTOD.TabIndex = 6;
            this.grpTOD.TabStop = false;
            this.grpTOD.Text = "Time of Day";
            // 
            // cmdNight
            // 
            this.cmdNight.Location = new System.Drawing.Point(442, 16);
            this.cmdNight.Name = "cmdNight";
            this.cmdNight.Size = new System.Drawing.Size(97, 23);
            this.cmdNight.TabIndex = 5;
            this.cmdNight.Text = "Set to Midnight";
            this.cmdNight.UseVisualStyleBackColor = true;
            this.cmdNight.Click += new System.EventHandler(this.cmdNight_Click);
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(13, 19);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(120, 20);
            this.txtTime.TabIndex = 2;
            this.txtTime.Text = "0";
            this.txtTime.TextChanged += new System.EventHandler(this.txtMinutes_TextChanged);
            // 
            // cmdDay
            // 
            this.cmdDay.Location = new System.Drawing.Point(339, 16);
            this.cmdDay.Name = "cmdDay";
            this.cmdDay.Size = new System.Drawing.Size(97, 23);
            this.cmdDay.TabIndex = 5;
            this.cmdDay.Text = "Set to Daytime";
            this.cmdDay.UseVisualStyleBackColor = true;
            this.cmdDay.Click += new System.EventHandler(this.cmdDay_Click);
            // 
            // tabEnts
            // 
            this.tabEnts.Controls.Add(this.entityEditor1);
            this.tabEnts.Location = new System.Drawing.Point(4, 22);
            this.tabEnts.Name = "tabEnts";
            this.tabEnts.Padding = new System.Windows.Forms.Padding(3);
            this.tabEnts.Size = new System.Drawing.Size(589, 317);
            this.tabEnts.TabIndex = 4;
            this.tabEnts.Text = "Entities";
            this.tabEnts.UseVisualStyleBackColor = true;
            // 
            // entityEditor1
            // 
            this.entityEditor1.CurrentEntity = null;
            this.entityEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityEditor1.EntityChunk = null;
            this.entityEditor1.Location = new System.Drawing.Point(3, 3);
            this.entityEditor1.Name = "entityEditor1";
            this.entityEditor1.PlayerPos = null;
            this.entityEditor1.Size = new System.Drawing.Size(583, 311);
            this.entityEditor1.SpawnPos = null;
            this.entityEditor1.TabIndex = 0;
            // 
            // tabTEnts
            // 
            this.tabTEnts.Controls.Add(this.tileEntityEditor1);
            this.tabTEnts.Location = new System.Drawing.Point(4, 22);
            this.tabTEnts.Name = "tabTEnts";
            this.tabTEnts.Padding = new System.Windows.Forms.Padding(3);
            this.tabTEnts.Size = new System.Drawing.Size(589, 317);
            this.tabTEnts.TabIndex = 5;
            this.tabTEnts.Text = "Tile Entities";
            this.tabTEnts.UseVisualStyleBackColor = true;
            // 
            // tileEntityEditor1
            // 
            this.tileEntityEditor1.CurrentEntity = null;
            this.tileEntityEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileEntityEditor1.EntityChunk = null;
            this.tileEntityEditor1.Location = new System.Drawing.Point(3, 3);
            this.tileEntityEditor1.Name = "tileEntityEditor1";
            this.tileEntityEditor1.PlayerPos = null;
            this.tileEntityEditor1.Size = new System.Drawing.Size(583, 311);
            this.tileEntityEditor1.SpawnPos = null;
            this.tileEntityEditor1.TabIndex = 0;
            // 
            // mapPic
            // 
            this.mapPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPic.Location = new System.Drawing.Point(0, 0);
            this.mapPic.Name = "mapPic";
            this.mapPic.Size = new System.Drawing.Size(597, 343);
            this.mapPic.TabIndex = 0;
            this.mapPic.TabStop = false;
            // 
            // frmMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 343);
            this.Controls.Add(this.tclMap);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.mapPic);
            this.Name = "frmMap";
            this.Text = "frmMap";
            this.Load += new System.EventHandler(this.frmMap_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMap_Paint);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tclMap.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVelZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHurtTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).EndInit();
            this.tabEnvironment.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpSpawn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnX)).EndInit();
            this.grpTOD.ResumeLayout(false);
            this.grpTOD.PerformLayout();
            this.tabEnts.ResumeLayout(false);
            this.tabTEnts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapPic)).EndInit();
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
        private System.Windows.Forms.TabControl tclMap;
        private System.Windows.Forms.TabPage tabMap;
        private System.Windows.Forms.TabPage tabInventory;
        private Inventory invMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button cmdHeal;
        private System.Windows.Forms.Button cmdSpawn;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numVelZ;
        private System.Windows.Forms.NumericUpDown numPosZ;
        private System.Windows.Forms.NumericUpDown numVelY;
        private System.Windows.Forms.NumericUpDown numVelX;
        private System.Windows.Forms.NumericUpDown numPosY;
        private System.Windows.Forms.NumericUpDown numPosX;
        private System.Windows.Forms.NumericUpDown numFire;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numHealth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.NumericUpDown numAir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabEnvironment;
        private System.Windows.Forms.GroupBox grpSpawn;
        private System.Windows.Forms.Button cmdSetSpawnToPos;
        private System.Windows.Forms.NumericUpDown numSpawnZ;
        private System.Windows.Forms.NumericUpDown numSpawnY;
        private System.Windows.Forms.NumericUpDown numSpawnX;
        private System.Windows.Forms.GroupBox grpTOD;
        private System.Windows.Forms.Button cmdNight;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Button cmdDay;
        private System.Windows.Forms.TabPage tabEnts;
        private System.Windows.Forms.TabPage tabTEnts;
        private EntityEditor entityEditor1;
        private TileEntityEditor tileEntityEditor1;
        private System.Windows.Forms.NumericUpDown numHurtTime;
        private System.Windows.Forms.Label lblHurt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSnow;
        private System.Windows.Forms.Button cmdDefrost;

    }
}