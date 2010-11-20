/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using OpenMinecraft;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;
namespace MineEdit
{
    public partial class frmMap : Form
    {
        private IMapHandler _Map = null;
        // Use this TEMPORARILY.
        //protected MapControl mapCtrl;
        // Work towards using THIS.
        //protected Viewport mapCtrl;
        private Inventory invMain;
        private TabPage tabCharacter;
        private TabPage tabEntities;
        private EntityEditor entityEditor1;
        private TabPage tabTileEntities;
        private TileEntityEditor tileEntityEditor1;
        private GroupBox grpHealth;
        private Button cmdKill;
        private Label lblHurtTime;
        private NumericUpDown numHurtTime;
        private Button cmdHeal;
        private Label lblAir;
        private NumericUpDown numAir;
        private Label lblFire;
        private NumericUpDown numFire;
        private Label lblHealth;
        private NumericUpDown numHealth;
        private ToolTip mTooltip;
        private IContainer components;
        private Button cmdReset;
        private Button cmdApply;
        private GroupBox grpPosition;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblX;
        private Label lblY;
        private Label lblZ;
        private Label lblSpawn;
        private Label lblPlayerPos;
        private Label lblPlayerRot;
        private NumericUpDown numSpawnX;
        private NumericUpDown numSpawnY;
        private NumericUpDown numSpawnZ;
        private NumericUpDown numPlayerPosX;
        private NumericUpDown numPlayerPosY;
        private NumericUpDown numPlayerPosZ;
        private NumericUpDown numPlayerRotX;
        private NumericUpDown numPlayerRotY;
        private GroupBox grpTime;
        private Label lblTime;
        private TextBox txtTime;
        private UIControls.Dial dlTime;
        private Label lblTimeOfDay;
        protected Label lblMapDisabled = new Label();
        //Dictionary<byte, byte> Replacements = new Dictionary<byte, byte>();
        public frmMap()
        {
            InitializeComponent();

            //mapCtrl = new MapControl();
            tabControl.TabPages.Remove(tabMap);

            SetStyle(ControlStyles.ResizeRedraw, true);
            this.ViewingAngle = ViewAngle.TopDown;

            entityEditor1.EntityModified += new EventHandler(entityEditor1_EntityModified);
            entityEditor1.EntityDeleted += new EntityEditor.EntityHandler(entityEditor1_EntityDeleted);

            tileEntityEditor1.EntityModified += new TileEntityEditor.TileEntityHandler(tileEntityEditor1_EntityModified);
            tileEntityEditor1.EntityDeleted += new TileEntityEditor.TileEntityHandler(tileEntityEditor1_EntityDeleted);

            // Character tab
            dlTime.MouseDown += UnlockApplyCancel;
            txtTime.TextChanged += UnlockApplyCancel;
            numAir.ValueChanged += UnlockApplyCancel;
            numFire.ValueChanged += UnlockApplyCancel;
            numHealth.ValueChanged += UnlockApplyCancel;
            numHurtTime.ValueChanged += UnlockApplyCancel;
            numPlayerPosX.ValueChanged += UnlockApplyCancel;
            numPlayerPosY.ValueChanged += UnlockApplyCancel;
            numPlayerPosZ.ValueChanged += UnlockApplyCancel;
            numSpawnX.ValueChanged += UnlockApplyCancel;
            numSpawnY.ValueChanged += UnlockApplyCancel;
            numSpawnZ.ValueChanged += UnlockApplyCancel;

            UpdateToD();
        }

        /// <summary>
        /// Make everything less confusing.
        /// </summary>
        private void SetupTooltips()
        {
            mTooltip.SetToolTip(numHealth,      "Your current health.\n20 = full health.");
            mTooltip.SetToolTip(numFire,        "How on fire you are?\nA value of -200 appears to indicate you're not on fire.");
            mTooltip.SetToolTip(numAir,         "How much oxygen you have left (if underwater).\n300 = full air.");
            mTooltip.SetToolTip(numHurtTime,    "I have no idea what this is, but it will hurt you when you load the game if not set to 0.");
        }

        void mapCtrl_TileEntityClicked(TileEntity e)
        {
            tileEntityEditor1.SetSelectedTEnt(e);
            tabControl.SelectedTab=tabTileEntities;
        }

        void mapCtrl_EntityClicked(Entity e)
        {
            entityEditor1.SetSelectedEnt(e);
            tabControl.SelectedTab=tabEntities;
        }

        void tileEntityEditor1_EntityDeleted(TileEntity e)
        {
            _Map.RemoveTileEntity(e);
        }

        void tileEntityEditor1_EntityModified(TileEntity e)
        {
            _Map.SetTileEntity(e);
        }

        void entityEditor1_EntityDeleted(Entity entity)
        {
            _Map.RemoveEntity(entity);
        }

        void entityEditor1_EntityModified(object sender, EventArgs e)
        {
            Entity ent = entityEditor1.CurrentEntity;
            _Map.SetEntity(ent);
        }
        
        void mapCtrl_MouseDown(object sender, MouseEventArgs e)
        {

            Vector3i hurr = GetVoxelFromMouse(e.X, e.Y);
            (MdiParent as frmMain).SetStatus("Mouse at " + hurr.ToString());
            Console.WriteLine("MouseDown");
            if (e.Button == MouseButtons.Middle)
            {
                //mapCtrl.CurrentPosition=hurr;
            }
            if (e.Button == MouseButtons.Left)
            {
               // mapCtrl.PerformBrushAction(hurr);
            }
            if (e.Button == MouseButtons.Right)
            {
                //mapCtrl.SelectedVoxel = hurr;
                MenuItem mnuMap = new MenuItem();
                
                MenuItem mnuPlaceEntity = new MenuItem("Place Entity");
                mnuPlaceEntity.Enabled = false;
                mnuPlaceEntity.Click += new EventHandler(mnuPlaceEntity_Click);
                mnuMap.MenuItems.Add(mnuPlaceEntity);

                MenuItem mnuChunkDetails = new MenuItem("Chunk details...");
                mnuChunkDetails.Click += new EventHandler(mnuChunkDetails_Click);
            }
        }

        void mnuChunkDetails_Click(object sender, EventArgs e)
        {
            //dlgChunk chunk = new dlgChunk(_Map, mapCtrl.SelectedVoxel);
        }

        void mnuPlaceEntity_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        Vector3i GetVoxelFromMouse(int x, int y)
        {
        	return new Vector3i(0,0,0);
            //return _Map.GetMousePos(new Vector3i(x, y, mapCtrl.CurrentPosition.Z), mapCtrl.ZoomLevel, ViewingAngle);
        }

        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                this.invMain.Map = value;
                //if (_Map != null && !string.IsNullOrEmpty(_Map.Filename))
                //    mapCtrl.World = _Map;
                Reload();
                Refresh();
            }
        }

        private void Reload()
        {
            if (_Map != null)
            {
                this.Text = this._Map.Filename;
                numHealth.Value = (decimal)Utils.Clamp(_Map.Health,0,9999);
                numFire.Value = (decimal)Utils.Clamp(_Map.Fire,-200,900);
                PlayerPos = _Map.PlayerPos;
                numAir.Value = Utils.Clamp(_Map.Air,0,9999);
                numHurtTime.Value = Utils.Clamp(_Map.HurtTime,0,9999);
                LockApplyCancel();

                numSpawnX.Value = _Map.Spawn.X;
                numSpawnY.Value = _Map.Spawn.Y;
                numSpawnZ.Value = _Map.Spawn.Z;

                txtTime.Text = _Map.Time.ToString();

                entityEditor1.Load(ref _Map);
                tileEntityEditor1.Load(ref _Map);

            }
        }

        private void AdjustSpinner(ref NumericUpDown numControl, decimal p)
        {
        }
        public Vector3d PlayerPos
        {
            get
            {
                Vector3d p = new Vector3d();
                p.X = (double)numPlayerPosX.Value;
                p.Y = (double)numPlayerPosY.Value;
                p.Z = (double)numPlayerPosZ.Value;
                return p;
            }
            set
            {
                numPlayerPosX.Value = (decimal)Math.Max(Math.Min(value.X, (double)decimal.MaxValue), (double)decimal.MinValue);
                numPlayerPosY.Value = (decimal)Math.Max(Math.Min(value.Y, (double)decimal.MaxValue), (double)decimal.MinValue);
                numPlayerPosZ.Value = (decimal)Math.Max(Math.Min(value.Z, (double)decimal.MaxValue), (double)decimal.MinValue);
            }
        }

        public ViewAngle ViewingAngle { get; set; }

        private void cmdHeal_Click(object sender, EventArgs e)
        {
            _Map.Health = 20; // idk what each version uses
            _Map.Fire = -200;
            _Map.Air = 300;
            _Map.HurtTime = 0;
            Reload();
        }

        private void cmdSpawn_Click(object sender, EventArgs e)
        {
            PlayerPos.X = _Map.Spawn.X;
            PlayerPos.Y = _Map.Spawn.Y;
            PlayerPos.Z = _Map.Spawn.Z;
            Reload();
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            _Map.Health = (int)numHealth.Value;
            _Map.Fire = (int)numFire.Value;
            _Map.PlayerPos = PlayerPos;
            _Map.Air = (int)numAir.Value;
            _Map.HurtTime = (int)numHurtTime.Value;
            Reload();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Reload();
        }


        private void LockApplyCancel()
        {
            cmdApply.Enabled = false;
            cmdReset.Enabled = false;
        }
        private void UnlockApplyCancel(object sender, EventArgs e)
        {
            UnlockApplyCancel();
        }

        private void UnlockApplyCancel()
        {
            cmdApply.Enabled = true;
            cmdReset.Enabled = true;
        }

        internal void ReloadAll()
        {
            Enabled=false;
            IMapHandler m = Map;
            Map = null;

            invMain.Reload();

            m.Load();
            Map = m;
            
            Reload();
            Enabled=true;
        }

        private void cmdSetSpawnToPos_Click(object sender, EventArgs e)
        {
            numSpawnX.Value = (decimal)this.PlayerPos.X;
            numSpawnY.Value = (decimal)this.PlayerPos.Y;
            numSpawnZ.Value = (decimal)this.PlayerPos.Z;
        }

        private void numSpawnX_ValueChanged(object sender, EventArgs e)
        {
            SetSpawn();
        }

        private void SetSpawn()
        {
            _Map.Spawn.X = (int)numSpawnX.Value;
            _Map.Spawn.Y = (int)numSpawnY.Value;
            _Map.Spawn.Z = (int)numSpawnZ.Value;
        }

        private void numSpawnY_ValueChanged(object sender, EventArgs e)
        {
            SetSpawn();
        }

        private void numSpawnZ_ValueChanged(object sender, EventArgs e)
        {
            SetSpawn();
        }

        private void txtMinutes_TextChanged(object sender, EventArgs e)
        {
        }

        private void cmdDay_Click(object sender, EventArgs e)
        {
            txtTime.Text = "0";
        }

        private void cmdNight_Click(object sender, EventArgs e)
        {
            txtTime.Text = "12125"; // Beginning of night?
        }

        private void txtSeconds_TextChanged(object sender, EventArgs e)
        {
            SetTime();
        }

        private void SetTime()
        {
            try
            {
                _Map.Time = (int)dlTime.Value;
                Console.WriteLine("Time: " + _Map.Time);
            }
            catch (Exception)
            {
            }
        }
        public int NumChunks = 0;
        public int ProcessedChunks = 0;

        private TabControl tabControl;
        private TabPage tabMap;
        private TabPage tabInventory;
        private void CountChunks(long X, long Y)
        {
            (MdiParent as frmMain).tsbStatus.Text = string.Format("Counting chunks ({0})...",NumChunks++);
            Application.DoEvents();
        }
        private void ReplaceStuff(long X,long Y)
        {
            
        }

        internal void FixLava()
        {
            dlgLongTask dlt = new dlgLongTask();
            ThreadStart ts = new ThreadStart(delegate()
            {
                dlt.Title = "Fixing Lava";
                dlt.Subtitle = "This will take a very long time, take a break.";
                dlt.VocabSubtask = "subtask";
                dlt.VocabSubtasks = "subtasks";
                dlt.SubtasksTotal = (int)(Map.ChunkScale.X * Map.ChunkScale.Y * Map.ChunkScale.Z);
                dlt.TasksTotal = NumChunks;
                dlt.TasksComplete = 0;
                (ActiveMdiChild as frmMap).Map.ForEachProgress += new ForEachProgressHandler(delegate(int Total, int Progress)
                {
                    dlt.TasksTotal = Total;
                    dlt.TasksComplete = Progress;
                });
                Map.ForEachChunk(delegate(long X, long Y)
                {
                    if (dlt.STOP) return;
                    Chunk c = Map.GetChunk(X, Y);
                    if (c == null) return;
                    byte[, ,] b = c.Blocks;
                    for (int x = 0; x < Map.ChunkScale.X; x++)
                    {
                        for (int y = 0; y < Map.ChunkScale.Y; y++)
                        {
                            for (int z = 0; z < Map.ChunkScale.Z; z++)
                            {
                                if (z == 0)
                                    b[x, y, z] = 7;
                                else if (z == 1)
                                    b[x, y, z] = 11;
                            }
                        }
                    }
                    c.Blocks = b;
                    _Map.SaveChunk(c);
                });
                if (dlt.TasksTotal - dlt.TasksComplete > 0)
                    MessageBox.Show(string.Format("{0} chunks were skipped?!", dlt.TasksTotal - dlt.TasksComplete));
                dlt.Done();
            });
            dlt.Start(ts);
            dlt.ShowDialog();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.invMain = new MineEdit.Inventory();
            this.tabCharacter = new System.Windows.Forms.TabPage();
            this.grpTime = new System.Windows.Forms.GroupBox();
            this.lblTimeOfDay = new System.Windows.Forms.Label();
            this.dlTime = new MineEdit.UIControls.Dial();
            this.lblTime = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.cmdReset = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.grpPosition = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblSpawn = new System.Windows.Forms.Label();
            this.lblPlayerPos = new System.Windows.Forms.Label();
            this.lblPlayerRot = new System.Windows.Forms.Label();
            this.numSpawnX = new System.Windows.Forms.NumericUpDown();
            this.numSpawnY = new System.Windows.Forms.NumericUpDown();
            this.numSpawnZ = new System.Windows.Forms.NumericUpDown();
            this.numPlayerPosX = new System.Windows.Forms.NumericUpDown();
            this.numPlayerPosY = new System.Windows.Forms.NumericUpDown();
            this.numPlayerPosZ = new System.Windows.Forms.NumericUpDown();
            this.numPlayerRotX = new System.Windows.Forms.NumericUpDown();
            this.numPlayerRotY = new System.Windows.Forms.NumericUpDown();
            this.grpHealth = new System.Windows.Forms.GroupBox();
            this.cmdKill = new System.Windows.Forms.Button();
            this.lblHurtTime = new System.Windows.Forms.Label();
            this.numHurtTime = new System.Windows.Forms.NumericUpDown();
            this.cmdHeal = new System.Windows.Forms.Button();
            this.lblAir = new System.Windows.Forms.Label();
            this.numAir = new System.Windows.Forms.NumericUpDown();
            this.lblFire = new System.Windows.Forms.Label();
            this.numFire = new System.Windows.Forms.NumericUpDown();
            this.lblHealth = new System.Windows.Forms.Label();
            this.numHealth = new System.Windows.Forms.NumericUpDown();
            this.tabEntities = new System.Windows.Forms.TabPage();
            this.entityEditor1 = new MineEdit.EntityEditor();
            this.tabTileEntities = new System.Windows.Forms.TabPage();
            this.tileEntityEditor1 = new MineEdit.TileEntityEditor();
            this.mTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.tabCharacter.SuspendLayout();
            this.grpTime.SuspendLayout();
            this.grpPosition.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerRotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerRotY)).BeginInit();
            this.grpHealth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHurtTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).BeginInit();
            this.tabEntities.SuspendLayout();
            this.tabTileEntities.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabMap);
            this.tabControl.Controls.Add(this.tabInventory);
            this.tabControl.Controls.Add(this.tabCharacter);
            this.tabControl.Controls.Add(this.tabEntities);
            this.tabControl.Controls.Add(this.tabTileEntities);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(624, 313);
            this.tabControl.TabIndex = 0;
            // 
            // tabMap
            // 
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(616, 287);
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
            this.tabInventory.Size = new System.Drawing.Size(616, 287);
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
            this.invMain.Size = new System.Drawing.Size(610, 281);
            this.invMain.TabIndex = 0;
            // 
            // tabCharacter
            // 
            this.tabCharacter.Controls.Add(this.grpTime);
            this.tabCharacter.Controls.Add(this.cmdReset);
            this.tabCharacter.Controls.Add(this.cmdApply);
            this.tabCharacter.Controls.Add(this.grpPosition);
            this.tabCharacter.Controls.Add(this.grpHealth);
            this.tabCharacter.Location = new System.Drawing.Point(4, 22);
            this.tabCharacter.Name = "tabCharacter";
            this.tabCharacter.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharacter.Size = new System.Drawing.Size(616, 287);
            this.tabCharacter.TabIndex = 3;
            this.tabCharacter.Text = "Character";
            this.tabCharacter.UseVisualStyleBackColor = true;
            // 
            // grpTime
            // 
            this.grpTime.Controls.Add(this.lblTimeOfDay);
            this.grpTime.Controls.Add(this.dlTime);
            this.grpTime.Controls.Add(this.lblTime);
            this.grpTime.Controls.Add(this.txtTime);
            this.grpTime.Location = new System.Drawing.Point(8, 171);
            this.grpTime.Name = "grpTime";
            this.grpTime.Size = new System.Drawing.Size(602, 79);
            this.grpTime.TabIndex = 4;
            this.grpTime.TabStop = false;
            this.grpTime.Text = "Time of Day";
            // 
            // lblTimeOfDay
            // 
            this.lblTimeOfDay.AutoSize = true;
            this.lblTimeOfDay.Location = new System.Drawing.Point(107, 52);
            this.lblTimeOfDay.Name = "lblTimeOfDay";
            this.lblTimeOfDay.Size = new System.Drawing.Size(10, 13);
            this.lblTimeOfDay.TabIndex = 4;
            this.lblTimeOfDay.Text = "-";
            // 
            // dlTime
            // 
            this.dlTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dlTime.Label = null;
            this.dlTime.Location = new System.Drawing.Point(6, 13);
            this.dlTime.Maximum = 24000D;
            this.dlTime.Name = "dlTime";
            this.dlTime.Size = new System.Drawing.Size(62, 66);
            this.dlTime.TabIndex = 3;
            this.mTooltip.SetToolTip(this.dlTime, "Upside-down graphical representation of sun angle, which is what this actually af" +
                    "fects.");
            this.dlTime.Value = 0D;
            this.dlTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dlTime_MouseDown);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(71, 21);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(33, 13);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "Time:";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(110, 18);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(100, 20);
            this.txtTime.TabIndex = 1;
            this.mTooltip.SetToolTip(this.txtTime, "0=Just after dawn, full light.");
            this.txtTime.TextChanged += new System.EventHandler(this.txtTime_TextChanged);
            // 
            // cmdReset
            // 
            this.cmdReset.Location = new System.Drawing.Point(535, 256);
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(75, 23);
            this.cmdReset.TabIndex = 3;
            this.cmdReset.Text = "Reset";
            this.cmdReset.UseVisualStyleBackColor = true;
            // 
            // cmdApply
            // 
            this.cmdApply.Location = new System.Drawing.Point(454, 256);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(75, 23);
            this.cmdApply.TabIndex = 2;
            this.cmdApply.Text = "Apply";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // grpPosition
            // 
            this.grpPosition.Controls.Add(this.tableLayoutPanel1);
            this.grpPosition.Location = new System.Drawing.Point(231, 6);
            this.grpPosition.Name = "grpPosition";
            this.grpPosition.Size = new System.Drawing.Size(379, 159);
            this.grpPosition.TabIndex = 1;
            this.grpPosition.TabStop = false;
            this.grpPosition.Text = "Position/Speed/Spawn Stuff";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel1.Controls.Add(this.lblX, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblY, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblZ, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblSpawn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPlayerPos, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPlayerRot, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numSpawnX, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.numSpawnY, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.numSpawnZ, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.numPlayerPosX, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.numPlayerPosY, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.numPlayerPosZ, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.numPlayerRotX, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.numPlayerRotY, 2, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(373, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblX.Location = new System.Drawing.Point(184, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(58, 20);
            this.lblX.TabIndex = 0;
            this.lblX.Text = "X";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblY.Location = new System.Drawing.Point(248, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(58, 20);
            this.lblY.TabIndex = 1;
            this.lblY.Text = "Y";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblZ
            // 
            this.lblZ.AutoSize = true;
            this.lblZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblZ.Location = new System.Drawing.Point(312, 0);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(58, 20);
            this.lblZ.TabIndex = 2;
            this.lblZ.Text = "Z";
            this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpawn
            // 
            this.lblSpawn.AutoSize = true;
            this.lblSpawn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpawn.Location = new System.Drawing.Point(3, 20);
            this.lblSpawn.Name = "lblSpawn";
            this.lblSpawn.Size = new System.Drawing.Size(175, 26);
            this.lblSpawn.TabIndex = 3;
            this.lblSpawn.Text = "Spawn Position:";
            this.lblSpawn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mTooltip.SetToolTip(this.lblSpawn, "Where you spawn when you die.");
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.AutoSize = true;
            this.lblPlayerPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlayerPos.Location = new System.Drawing.Point(3, 46);
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.Size = new System.Drawing.Size(175, 26);
            this.lblPlayerPos.TabIndex = 7;
            this.lblPlayerPos.Text = "Player Position:";
            this.lblPlayerPos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mTooltip.SetToolTip(this.lblPlayerPos, "Where you currently are.");
            // 
            // lblPlayerRot
            // 
            this.lblPlayerRot.AutoSize = true;
            this.lblPlayerRot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlayerRot.Location = new System.Drawing.Point(3, 72);
            this.lblPlayerRot.Name = "lblPlayerRot";
            this.lblPlayerRot.Size = new System.Drawing.Size(175, 28);
            this.lblPlayerRot.TabIndex = 11;
            this.lblPlayerRot.Text = "Player Rotation:";
            this.lblPlayerRot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mTooltip.SetToolTip(this.lblPlayerRot, "Player rotation using Notch\'s strange rotation system.");
            // 
            // numSpawnX
            // 
            this.numSpawnX.Location = new System.Drawing.Point(184, 23);
            this.numSpawnX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numSpawnX.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numSpawnX.Name = "numSpawnX";
            this.numSpawnX.Size = new System.Drawing.Size(58, 20);
            this.numSpawnX.TabIndex = 4;
            // 
            // numSpawnY
            // 
            this.numSpawnY.Location = new System.Drawing.Point(248, 23);
            this.numSpawnY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numSpawnY.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numSpawnY.Name = "numSpawnY";
            this.numSpawnY.Size = new System.Drawing.Size(58, 20);
            this.numSpawnY.TabIndex = 5;
            // 
            // numSpawnZ
            // 
            this.numSpawnZ.Location = new System.Drawing.Point(312, 23);
            this.numSpawnZ.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numSpawnZ.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numSpawnZ.Name = "numSpawnZ";
            this.numSpawnZ.Size = new System.Drawing.Size(58, 20);
            this.numSpawnZ.TabIndex = 6;
            // 
            // numPlayerPosX
            // 
            this.numPlayerPosX.Location = new System.Drawing.Point(184, 49);
            this.numPlayerPosX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPlayerPosX.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numPlayerPosX.Name = "numPlayerPosX";
            this.numPlayerPosX.Size = new System.Drawing.Size(58, 20);
            this.numPlayerPosX.TabIndex = 8;
            // 
            // numPlayerPosY
            // 
            this.numPlayerPosY.Location = new System.Drawing.Point(248, 49);
            this.numPlayerPosY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPlayerPosY.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numPlayerPosY.Name = "numPlayerPosY";
            this.numPlayerPosY.Size = new System.Drawing.Size(58, 20);
            this.numPlayerPosY.TabIndex = 9;
            // 
            // numPlayerPosZ
            // 
            this.numPlayerPosZ.Location = new System.Drawing.Point(312, 49);
            this.numPlayerPosZ.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPlayerPosZ.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numPlayerPosZ.Name = "numPlayerPosZ";
            this.numPlayerPosZ.Size = new System.Drawing.Size(58, 20);
            this.numPlayerPosZ.TabIndex = 10;
            // 
            // numPlayerRotX
            // 
            this.numPlayerRotX.Enabled = false;
            this.numPlayerRotX.Location = new System.Drawing.Point(184, 75);
            this.numPlayerRotX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPlayerRotX.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numPlayerRotX.Name = "numPlayerRotX";
            this.numPlayerRotX.Size = new System.Drawing.Size(58, 20);
            this.numPlayerRotX.TabIndex = 12;
            // 
            // numPlayerRotY
            // 
            this.numPlayerRotY.Enabled = false;
            this.numPlayerRotY.Location = new System.Drawing.Point(248, 75);
            this.numPlayerRotY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numPlayerRotY.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.numPlayerRotY.Name = "numPlayerRotY";
            this.numPlayerRotY.Size = new System.Drawing.Size(58, 20);
            this.numPlayerRotY.TabIndex = 13;
            // 
            // grpHealth
            // 
            this.grpHealth.Controls.Add(this.cmdKill);
            this.grpHealth.Controls.Add(this.lblHurtTime);
            this.grpHealth.Controls.Add(this.numHurtTime);
            this.grpHealth.Controls.Add(this.cmdHeal);
            this.grpHealth.Controls.Add(this.lblAir);
            this.grpHealth.Controls.Add(this.numAir);
            this.grpHealth.Controls.Add(this.lblFire);
            this.grpHealth.Controls.Add(this.numFire);
            this.grpHealth.Controls.Add(this.lblHealth);
            this.grpHealth.Controls.Add(this.numHealth);
            this.grpHealth.Location = new System.Drawing.Point(8, 6);
            this.grpHealth.Name = "grpHealth";
            this.grpHealth.Size = new System.Drawing.Size(217, 159);
            this.grpHealth.TabIndex = 0;
            this.grpHealth.TabStop = false;
            this.grpHealth.Text = "Health-Related Stuff";
            // 
            // cmdKill
            // 
            this.cmdKill.Location = new System.Drawing.Point(119, 123);
            this.cmdKill.Name = "cmdKill";
            this.cmdKill.Size = new System.Drawing.Size(75, 23);
            this.cmdKill.TabIndex = 9;
            this.cmdKill.Text = "Kill";
            this.mTooltip.SetToolTip(this.cmdKill, "Kill yourself 8D");
            this.cmdKill.UseVisualStyleBackColor = true;
            // 
            // lblHurtTime
            // 
            this.lblHurtTime.AutoSize = true;
            this.lblHurtTime.Location = new System.Drawing.Point(15, 99);
            this.lblHurtTime.Name = "lblHurtTime";
            this.lblHurtTime.Size = new System.Drawing.Size(53, 13);
            this.lblHurtTime.TabIndex = 6;
            this.lblHurtTime.Text = "HurtTime:";
            this.lblHurtTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.mTooltip.SetToolTip(this.lblHurtTime, "I have no idea, but it hurts you when you load the save.");
            // 
            // numHurtTime
            // 
            this.numHurtTime.Location = new System.Drawing.Point(74, 97);
            this.numHurtTime.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numHurtTime.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.numHurtTime.Name = "numHurtTime";
            this.numHurtTime.Size = new System.Drawing.Size(120, 20);
            this.numHurtTime.TabIndex = 7;
            this.numHurtTime.ThousandsSeparator = true;
            // 
            // cmdHeal
            // 
            this.cmdHeal.Location = new System.Drawing.Point(38, 123);
            this.cmdHeal.Name = "cmdHeal";
            this.cmdHeal.Size = new System.Drawing.Size(75, 23);
            this.cmdHeal.TabIndex = 8;
            this.cmdHeal.Text = "Heal";
            this.mTooltip.SetToolTip(this.cmdHeal, "Heal yourself, cure all effects, and restore full air.");
            this.cmdHeal.UseVisualStyleBackColor = true;
            // 
            // lblAir
            // 
            this.lblAir.AutoSize = true;
            this.lblAir.Location = new System.Drawing.Point(46, 73);
            this.lblAir.Name = "lblAir";
            this.lblAir.Size = new System.Drawing.Size(22, 13);
            this.lblAir.TabIndex = 4;
            this.lblAir.Text = "Air:";
            this.lblAir.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.mTooltip.SetToolTip(this.lblAir, "-300 = full air");
            // 
            // numAir
            // 
            this.numAir.Location = new System.Drawing.Point(74, 71);
            this.numAir.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numAir.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.numAir.Name = "numAir";
            this.numAir.Size = new System.Drawing.Size(120, 20);
            this.numAir.TabIndex = 5;
            this.numAir.ThousandsSeparator = true;
            // 
            // lblFire
            // 
            this.lblFire.AutoSize = true;
            this.lblFire.Location = new System.Drawing.Point(41, 47);
            this.lblFire.Name = "lblFire";
            this.lblFire.Size = new System.Drawing.Size(27, 13);
            this.lblFire.TabIndex = 2;
            this.lblFire.Text = "Fire:";
            this.lblFire.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.mTooltip.SetToolTip(this.lblFire, "How hot you are. negative values = not on fire.");
            // 
            // numFire
            // 
            this.numFire.Location = new System.Drawing.Point(74, 45);
            this.numFire.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numFire.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.numFire.Name = "numFire";
            this.numFire.Size = new System.Drawing.Size(120, 20);
            this.numFire.TabIndex = 3;
            this.numFire.ThousandsSeparator = true;
            // 
            // lblHealth
            // 
            this.lblHealth.AutoSize = true;
            this.lblHealth.Location = new System.Drawing.Point(27, 21);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(41, 13);
            this.lblHealth.TabIndex = 0;
            this.lblHealth.Text = "Health:";
            this.lblHealth.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.mTooltip.SetToolTip(this.lblHealth, "Player health.");
            // 
            // numHealth
            // 
            this.numHealth.Location = new System.Drawing.Point(74, 19);
            this.numHealth.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numHealth.Minimum = new decimal(new int[] {
            3000,
            0,
            0,
            -2147483648});
            this.numHealth.Name = "numHealth";
            this.numHealth.Size = new System.Drawing.Size(120, 20);
            this.numHealth.TabIndex = 1;
            this.numHealth.ThousandsSeparator = true;
            // 
            // tabEntities
            // 
            this.tabEntities.Controls.Add(this.entityEditor1);
            this.tabEntities.Location = new System.Drawing.Point(4, 22);
            this.tabEntities.Name = "tabEntities";
            this.tabEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabEntities.Size = new System.Drawing.Size(616, 287);
            this.tabEntities.TabIndex = 4;
            this.tabEntities.Text = "Entities";
            this.tabEntities.UseVisualStyleBackColor = true;
            // 
            // entityEditor1
            // 
            this.entityEditor1.CurrentEntity = null;
            this.entityEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityEditor1.EntityChunk = null;
            this.entityEditor1.Location = new System.Drawing.Point(3, 3);
            this.entityEditor1.Name = "entityEditor1";
            this.entityEditor1.PlayerPos = null;
            this.entityEditor1.Size = new System.Drawing.Size(610, 281);
            this.entityEditor1.SpawnPos = null;
            this.entityEditor1.TabIndex = 0;
            this.entityEditor1.EntityAdded += new MineEdit.EntityEditor.EntityHandler(this.entityEditor1_EntityAdded);
            // 
            // tabTileEntities
            // 
            this.tabTileEntities.Controls.Add(this.tileEntityEditor1);
            this.tabTileEntities.Location = new System.Drawing.Point(4, 22);
            this.tabTileEntities.Name = "tabTileEntities";
            this.tabTileEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabTileEntities.Size = new System.Drawing.Size(616, 287);
            this.tabTileEntities.TabIndex = 5;
            this.tabTileEntities.Text = "Tile Entities";
            this.tabTileEntities.UseVisualStyleBackColor = true;
            // 
            // tileEntityEditor1
            // 
            this.tileEntityEditor1.CurrentEntity = null;
            this.tileEntityEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileEntityEditor1.EntityChunk = null;
            this.tileEntityEditor1.Location = new System.Drawing.Point(3, 3);
            this.tileEntityEditor1.Name = "tileEntityEditor1";
            this.tileEntityEditor1.PlayerPos = null;
            this.tileEntityEditor1.Size = new System.Drawing.Size(610, 281);
            this.tileEntityEditor1.SpawnPos = null;
            this.tileEntityEditor1.TabIndex = 0;
            // 
            // frmMap
            // 
            this.ClientSize = new System.Drawing.Size(624, 313);
            this.Controls.Add(this.tabControl);
            this.Name = "frmMap";
            this.Text = "Map";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tabCharacter.ResumeLayout(false);
            this.grpTime.ResumeLayout(false);
            this.grpTime.PerformLayout();
            this.grpPosition.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawnZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerRotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerRotY)).EndInit();
            this.grpHealth.ResumeLayout(false);
            this.grpHealth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHurtTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHealth)).EndInit();
            this.tabEntities.ResumeLayout(false);
            this.tabTileEntities.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void dlTime_MouseDown(object sender, MouseEventArgs e)
        {
            txtTime.Text = ((int)Math.Round(dlTime.Value)).ToString();
            _Map.Time = (int)Math.Round(dlTime.Value);
            UpdateToD();
        }

        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            double val;
            if (!double.TryParse(txtTime.Text, out val))
            {
                MessageBox.Show("Please enter an integer.");
                return;
            }
            int nv = (int)val % (int)dlTime.Maximum; ;
            dlTime.Value = nv;
            _Map.Time = nv;
            UpdateToD();
        }

        private void entityEditor1_EntityAdded(Entity e)
        {
            this.Map.AddEntity(e);
        }

        private void UpdateToD()
        {
            // 24000 = 1 day
            // Day 1, 24:00:00
            int data = int.Parse(txtTime.Text);
            int day = data / 24000;
            data = data % 24000;
            int hour = data / 1000;
            data = data % 1000;
            data = (int)(((float)data / (float)999) * (float)3600);
            int minutes = data / 60;
            int seconds = data % 60;

            lblTimeOfDay.Text = string.Format("Day {0}, {1}:{2}:{3}", day, hour, minutes, seconds);
        }
    }
}
