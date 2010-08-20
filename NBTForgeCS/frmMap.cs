using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using OpenMinecraft;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;
using System.Threading;
namespace MineEdit
{
    public partial class frmMap : Form
    {
        private IMapHandler _Map = null;
        // Use this TEMPORARILY.
        protected MapControl mapCtrl;
        // Work towards using THIS.
        //protected MapControlGL mapCtrl;
        protected Label lblMapDisabled = new Label();
        //Dictionary<byte, byte> Replacements = new Dictionary<byte, byte>();
        public frmMap()
        {
            InitializeComponent();
            /*
            lblMapDisabled.Text = "Map is completely broken, so it has been disabled for the time being.\n\nIf you'd like to help, please contact me, 'cuz I'm stumped.";
            lblMapDisabled.Dock = DockStyle.Fill;
            lblMapDisabled.TextAlign = ContentAlignment.TopCenter;
            tabMap.Controls.Add(lblMapDisabled);
            tclMap.SelectedTab = tabInventory;
            */

            mapCtrl = new MapControl();
            //mapCtrl = new MapControlGL();
            tabMap.Controls.Add(mapCtrl);
            mapCtrl.Dock = DockStyle.Fill;
            mapCtrl.MouseDown += new MouseEventHandler(mapCtrl_MouseDown);
            mapCtrl.EntityClicked += new MapControl.EntityClickHandler(mapCtrl_EntityClicked);
            mapCtrl.TileEntityClicked += new MapControl.TileEntityClickHandler(mapCtrl_TileEntityClicked);

            Replacements.DrawItem += new DrawItemEventHandler(Replacements_DrawItem);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.ViewingAngle = ViewAngle.TopDown;

            entityEditor1.EntityModified += new EventHandler(entityEditor1_EntityModified);
            entityEditor1.EntityDeleted += new EntityEditor.EntityHandler(entityEditor1_EntityDeleted);

            tileEntityEditor1.EntityModified += new TileEntityEditor.TileEntityHandler(tileEntityEditor1_EntityModified);
            tileEntityEditor1.EntityDeleted += new TileEntityEditor.TileEntityHandler(tileEntityEditor1_EntityDeleted);

        }
        void Replacements_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block enta = Blocks.Get((short)((KeyValuePair<byte, byte>)Replacements.Items[e.Index]).Key);
                Block entb = Blocks.Get((short)((KeyValuePair<byte, byte>)Replacements.Items[e.Index]).Value);

                // Draw block icon A
                g.DrawImage(enta.Image, iconArea);

                // Block Name A
                SizeF idaSz = g.MeasureString(enta.ToString(), this.Font);
                Rectangle idAreaA = area;
                idAreaA.X = iconArea.Right + 3;
                idAreaA.Width = (int)idaSz.Width + 1;
                g.DrawString(enta.ToString(), this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idAreaA);

                // Arrow
                SizeF arrowsz = g.MeasureString("->", this.Font);
                Rectangle ctxt = area;
                ctxt.X = idAreaA.Right + 3;
                ctxt.Width = (int)arrowsz.Width + 1;
                g.DrawString("->", this.Font, new SolidBrush(e.ForeColor), ctxt);


                // Draw block icon B
                iconArea.X = ctxt.Right + 3;
                g.DrawImage(entb.Image, iconArea);

                // Block Name B
                SizeF idbSz = g.MeasureString(entb.ToString(), this.Font);
                Rectangle idAreaB = area;
                idAreaB.X = iconArea.Right + 3;
                idAreaB.Width = (int)idbSz.Width + 1;
                g.DrawString(entb.ToString(), this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idAreaB);
            }
        }

        void mapCtrl_TileEntityClicked(TileEntity e)
        {
            tileEntityEditor1.SetSelectedTEnt(e);
            tclMap.SelectedTab=tabTEnts;
        }

        void mapCtrl_EntityClicked(Entity e)
        {
            entityEditor1.SetSelectedEnt(e);
            tclMap.SelectedTab=tabEnts;
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
            dlgChunk chunk = new dlgChunk(_Map, mapCtrl.SelectedVoxel);
        }

        void mnuPlaceEntity_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        Vector3i GetVoxelFromMouse(int x, int y)
        {
            return _Map.GetMousePos(new Vector3i(x, y, mapCtrl.CurrentPosition.Z), mapCtrl.ZoomLevel, ViewingAngle);
        }

        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                this.invMain.Map = value;
                if (_Map != null && !string.IsNullOrEmpty(_Map.Filename))
                    mapCtrl.Map = _Map;
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
                chkSnow.Checked = _Map.Snow;

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
                p.X = (double)numPosX.Value;
                p.Y = (double)numPosY.Value;
                p.Z = (double)numPosZ.Value;
                return p;
            }
            set
            {
                numPosX.Value = (decimal)Math.Max(Math.Min(value.X, (double)decimal.MaxValue), (double)decimal.MinValue);
                numPosY.Value = (decimal)Math.Max(Math.Min(value.Y, (double)decimal.MaxValue), (double)decimal.MinValue);
                numPosZ.Value = (decimal)Math.Max(Math.Min(value.Z, (double)decimal.MaxValue), (double)decimal.MinValue);
            }
        }

        public ViewAngle ViewingAngle { get; set; }

        private void frmMap_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Just a warning:  Placed pig spawners aren't in the TileEntity list yet (they're a block).  I'll add a converter soon.\n\n  -- Nexypoo", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.cbViewingStyle.SelectedIndex = 0;

            tscbMaterial.Items.Clear();
            foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
            {
                if(k.Key<100)
                    tscbMaterial.Items.Add(k.Value);
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //mapCtrl.CurrentPosition.Z += e.Delta;
            //ClampCZ();
            //Console.WriteLine("Changing Level to "+CZ.ToString());
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //this.SelectedVoxel = GetVoxelFromMouse(e.X, e.Y);
            //(this.MdiParent as frmMain).SetStatus("Mouse at <" + SelectedVoxel.X.ToString() + "," + SelectedVoxel.Y.ToString() + "," + SelectedVoxel.Z.ToString() + ">");
        }

        private void cbViewingStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cbViewingStyle.SelectedIndex)
            {
                case 0:
                    ViewingAngle = ViewAngle.TopDown;
                    break;
                case 1:
                    ViewingAngle = ViewAngle.FrontSlice;
                    break;
                case 2:
                    ViewingAngle = ViewAngle.SideSlice;
                    break;
            }
            Console.WriteLine("Viewing angle updated to " + ViewingAngle.ToString());
        }

        private void frmMap_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = this.CreateGraphics();
            //g.DrawImage(bmp,0,0,Width,Height);
            //g.Dispose();
        }

        private void pbox_MouseDown(object sender, MouseEventArgs e)
        {
        }
        
        private void ClampCZ()
        {
            if (mapCtrl.CurrentPosition.Z == -1) mapCtrl.CurrentPosition.Z = 127;
            mapCtrl.CurrentPosition.Z = Math.Abs(mapCtrl.CurrentPosition.Z % _Map.ChunkScale.Z);
        }
        
        // Up
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            /*mapCtrl.CurrentPosition.Z++;
            ClampCZ();
            (MdiParent as frmMain).SetStatus("Moved to level " + mapCtrl.CurrentPosition.Z.ToString());
            Console.WriteLine("Moved to level " + mapCtrl.CurrentPosition.Z.ToString());

            mapCtrl.Render();
            Refresh();*/
        }

        private void tsbDown_Click(object sender, EventArgs e)
        {
            /*mapCtrl.CurrentPosition.Z--;
            ClampCZ();
            (MdiParent as frmMain).SetStatus("Moved to level " + mapCtrl.CurrentPosition.Z.ToString());
            Console.WriteLine("Moved to level " + mapCtrl.CurrentPosition.Z.ToString());
            mapCtrl.Render();
            Refresh();*/
        }

        private void pbox_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void tscbMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(tscbMaterial.SelectedItem!=null)
                //mapCtrl.CurrentMaterial=(tscbMaterial.SelectedItem as Block).ID;
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuBrush3D.Checked = !mnuBrush3D.Checked;
        }

        private void mnuBrushSphere_Click(object sender, EventArgs e)
        {
            mnuBrushSphere.Checked = !mnuBrushSphere.Checked;
        }

        private void mnuBrushHollow_Click(object sender, EventArgs e)
        {
            mnuBrushHollow.Checked = !mnuBrushHollow.Checked;
        }

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
            cmdCancel.Enabled = false;
        }
        private void UnlockApplyCancel(object sender, EventArgs e)
        {
            UnlockApplyCancel();
        }

        private void UnlockApplyCancel()
        {
            cmdApply.Enabled = true;
            cmdCancel.Enabled = true;
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
            SetTime();
        }

        private void cmdDay_Click(object sender, EventArgs e)
        {
            txtTime.Text = "0";
        }

        private void cmdNight_Click(object sender, EventArgs e)
        {
            txtTime.Text = "12125"; // Beginning of night
        }

        private void txtSeconds_TextChanged(object sender, EventArgs e)
        {
            SetTime();
        }

        private void SetTime()
        {
            try
            {
                _Map.Time = int.Parse(txtTime.Text);
            }
            catch (Exception)
            {
            }
        }
        public int NumChunks = 0;
        public int ProcessedChunks = 0;

        int lastpct = 0;
        private void CountChunks(long X, long Y)
        {
            (MdiParent as frmMain).tsbStatus.Text = string.Format("Counting chunks ({0})...",NumChunks++);
            Application.DoEvents();
        }
        private void ReplaceStuff(long X,long Y)
        {
            
        }

        private void ClearSnow()
        {
            Replacements.Items.Clear();
            byte lolsnow = 78;
            byte lolice = 79;
            byte lolwater = 9;
            Replacements.Items.Add(new KeyValuePair<byte, byte>(lolsnow, 0));
            Replacements.Items.Add(new KeyValuePair<byte, byte>(lolice, lolwater));
            DoReplace();
        }

        private void DoReplace()
        {
            
            this.Enabled = false;
            string q = "Are you sure you want to do the following replacements:\n\n\t{0}\n\nTHIS WILL TAKE A VERY LONG TIME!";
            List<string> reps = new List<string>();
            foreach (KeyValuePair<byte, byte> rep in Replacements.Items)
            {
                reps.Add(string.Format("{0} to {1}",Blocks.Get((short)rep.Key).Name,Blocks.Get((short)rep.Value).Name));
            }
            DialogResult dr = MessageBox.Show(string.Format(q,string.Join("\n\t",reps.ToArray())), "Are you sure?", MessageBoxButtons.YesNo);


            if (dr == DialogResult.Yes)
            {
                dlgLongTask dlt = new dlgLongTask();
                dlt.Title = "Replacing blocks";
                dlt.Subtitle = "This will take a long time, take a break.";
                dlt.VocabSubtask = "subtask";
                dlt.VocabSubtasks = "subtasks";
                dlt.VocabTask = "chunk";
                dlt.VocabTasks = "chunks";
                dlt.Start(delegate()
                {
                    Map.ForEachProgress += new ForEachProgressHandler(delegate(int Total, int Progress)
                    {
                        dlt.TasksTotal = Total;
                        dlt.TasksComplete = Progress;
                    });
                    dlt.CurrentTask = "Replacing blocks...";
                    _Map.ForEachChunk(delegate(long X, long Y)
                    {
                        dlt.CurrentTask = string.Format("Replacing stuff in chunk {0} of {1}...", dlt.TasksComplete, dlt.TasksTotal);
                        Dictionary<byte, byte> durr = new Dictionary<byte, byte>();
                        foreach (KeyValuePair<byte, byte> derp in Replacements.Items)
                        {
                            durr.Add(derp.Key, derp.Value);
                        }
                        _Map.ReplaceBlocksIn(X, Y, durr);
                        ++ProcessedChunks;
                    });
                });
                dlt.ShowDialog();
                (MdiParent as frmMain).ResetStatus();
                dlt.Done();
                MessageBox.Show("Done.", "Operation complete!");
            }
            this.Enabled = true;
        }
        private void chkSnow_CheckedChanged(object sender, EventArgs e)
        {
            _Map.Snow = chkSnow.Checked;
        }

        private void cmdDefrost_Click(object sender, EventArgs e)
        {
            ClearSnow();
        }

        private void invMain_Load(object sender, EventArgs e)
        {

        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            byte a = (byte)(blkReplace.SelectedItem as Block).ID;
            byte b = (byte)(blkWith.SelectedItem as Block).ID;
            Replacements.Items.Add(new KeyValuePair<byte, byte>(a, b));
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            Replacements.Items.Clear();
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            if (Replacements.SelectedItem != null)
            {
                Replacements.Items.Remove(Replacements.SelectedItem);
            }
        }

        private void cmdReplaceWaterWithLava_Click(object sender, EventArgs e)
        {
            Replacements.Items.Clear();
            byte lollava = 11;
            byte lolobsidian = 49;
            byte lolwater = 9;
            byte lolsand = 12;
            Replacements.Items.Add(new KeyValuePair<byte, byte>(lolwater,lollava));
            Replacements.Items.Add(new KeyValuePair<byte, byte>(lolsand, lolobsidian));

            DoReplace();
        }

        private void cmdProcess_Click(object sender, EventArgs e)
        {
            DoReplace();
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
                mapCtrl.Render();
        }

        private void Replacements_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void blkReplace_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
