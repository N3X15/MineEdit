using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public enum ViewAngle
    {
        XY,
        TopDown,
        YZ
    }
    public partial class frmMap : Form
    {
        private int _ZoomLevel=4;
        private IMapHandler _Map = null;
        private string _Filename = "";
        //protected MapControl mapCtrl;
        //protected MapControlGL mapCtrl;
        protected Label lblMapDisabled = new Label();
        public frmMap()
        {
            InitializeComponent();
            
            lblMapDisabled.Text = "Map is completely broken, so it has been disabled for the time being.\n\nIf you'd like to help, please contact me, 'cuz I'm stumped.";
            lblMapDisabled.Dock = DockStyle.Fill;
            lblMapDisabled.TextAlign = ContentAlignment.TopCenter;
            tabMap.Controls.Add(lblMapDisabled);
            tclMap.SelectedTab = tabInventory;
            
            //mapCtrl = new MapControl();
            //mapCtrl = new MapControlGL();
            //tabMap.Controls.Add(mapCtrl);
            //mapCtrl.Dock = DockStyle.Fill;
            //mapCtrl.MouseDown += new MouseEventHandler(mapCtrl_MouseDown);

            SetStyle(ControlStyles.ResizeRedraw, true);

            this.ViewingAngle = MineEdit.ViewAngle.TopDown;
        }
        /*
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
        }*/

        void mnuChunkDetails_Click(object sender, EventArgs e)
        {
            //dlgChunk chunk = new dlgChunk(_Map, mapCtrl.SelectedVoxel);
        }

        void mnuPlaceEntity_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        /*Vector3i GetVoxelFromMouse(int x, int y)
        {
            //return _Map.GetMousePos(new Vector3i(x, y, mapCtrl.CurrentPosition.Z), mapCtrl.ZoomLevel, ViewingAngle);
        }*/

        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                this.invMain.Map = value;
                if (_Map != null && !string.IsNullOrEmpty(_Map.Filename))
                //mapCtrl.Map = _Map;
                Reload();
                Refresh();
            }
        }

        private void Reload()
        {
            if (_Map != null)
            {
                this.Text = this._Map.Filename;
                numHealth.Value = (decimal)_Map.Health;
                numFire.Value = (decimal)_Map.Fire;
                PlayerPos = _Map.PlayerPos;
                numAir.Value = _Map.Air;
                LockApplyCancel();

                numSpawnX.Value = _Map.Spawn.X;
                numSpawnY.Value = _Map.Spawn.Y;
                numSpawnZ.Value = _Map.Spawn.Z;

                txtTime.Text = _Map.Time.ToString();
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
            this.cbViewingStyle.SelectedIndex = 0;

            tscbMaterial.Items.Clear();
            foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
            {
                if(k.Key<100)
                    tscbMaterial.Items.Add(k.Value);
            }
        }
        /*
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            mapCtrl.CurrentPosition.Z += e.Delta;
            ClampCZ();
            //Console.WriteLine("Changing Level to "+CZ.ToString());
        }*/

        /*
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //this.SelectedVoxel = GetVoxelFromMouse(e.X, e.Y);
            //(this.MdiParent as frmMain).SetStatus("Mouse at <" + SelectedVoxel.X.ToString() + "," + SelectedVoxel.Y.ToString() + "," + SelectedVoxel.Z.ToString() + ">");
        }*/

        private void cbViewingStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cbViewingStyle.SelectedIndex)
            {
                case 0:
                    ViewingAngle = ViewAngle.TopDown;
                    break;
                case 1:
                    ViewingAngle = ViewAngle.XY;
                    break;
                case 2:
                    ViewingAngle = ViewAngle.YZ;
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
        /*
        private void ClampCZ()
        {
            if (mapCtrl.CurrentPosition.Z == -1) mapCtrl.CurrentPosition.Z = 127;
            mapCtrl.CurrentPosition.Z = Math.Abs(mapCtrl.CurrentPosition.Z % _Map.ChunkScale.Z);
        }
        */
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
            _Map.Health = 100; // idk what each version uses
            _Map.Fire = -200;
            _Map.Air = 300;
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
    }
}
