using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class dlgTerrainGen : Form
    {
        IMapHandler mh = null;
        public dlgTerrainGen(IMapHandler _mh)
        {
            mh = _mh;
            InitializeComponent();
            cmbMapGenSel.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMapGenSel.DrawItem += new DrawItemEventHandler(cmbMapGenSel_DrawItem);
            LockCheckboxes(true);
        }

        void cmbMapGenSel_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                KeyValuePair<string,string> k = (KeyValuePair<string,string>)cmbMapGenSel.Items[e.Index];
                g.DrawString(k.Value, this.Font, new SolidBrush(Color.Black), e.Bounds);
            }
        }

        private void dlgTerrainGen_Load(object sender, EventArgs e)
        {
            MapGenerators.Init();
            cmbMapGenSel.Items.Clear();
            Dictionary<string, string> items = MapGenerators.GetList();
            foreach (KeyValuePair<string, string> k in items)
            {
                cmbMapGenSel.Items.Add(k);
            }
        }

        private void cmbMapGenSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMapGenSel.SelectedItem != null)
            {
                pgMapGen.SelectedObject = MapGenerators.Get(((KeyValuePair<string, string>)cmbMapGenSel.SelectedItem).Key, mh.RandomSeed);
                LockCheckboxes(false);
            }
            else
                LockCheckboxes(true);
            ResetEverything();
        }

        private void LockCheckboxes(bool p)
        {
            chkCaves.Enabled = !p;
            chkDungeons.Enabled = !p;
            chkGenOres.Enabled = !p;
            chkGenWater.Enabled = !p;
            chkHellMode.Enabled = !p;
            chkRegen.Enabled = !p;
            chkTrees.Enabled = !p;
            pgMapGen.Enabled = !p;
        }

        private void ResetEverything()
        {
            if (pgMapGen.SelectedObject == null) 
                return;
            chkCaves.Checked=(pgMapGen.SelectedObject as IMapGenerator).GenerateCaves;
            chkDungeons.Checked = (pgMapGen.SelectedObject as IMapGenerator).GenerateDungeons;
            chkGenOres.Checked = (pgMapGen.SelectedObject as IMapGenerator).GenerateOres;
            chkGenWater.Checked = (pgMapGen.SelectedObject as IMapGenerator).GenerateWater;
            chkHellMode.Checked = (pgMapGen.SelectedObject as IMapGenerator).HellMode;
            chkRegen.Checked = (pgMapGen.SelectedObject as IMapGenerator).NoPreservation;
            chkTrees.Checked = (pgMapGen.SelectedObject as IMapGenerator).GenerateTrees;
        }

        private void chkRegen_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).NoPreservation = chkRegen.Checked;
        }

        private void chkCaves_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).GenerateCaves = chkCaves.Checked;
        }

        private void chkGenWater_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).GenerateWater = chkGenWater.Checked;
        }

        private void chkGenOres_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).GenerateOres = chkGenOres.Checked;
        }

        private void chkDungeons_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).GenerateDungeons = chkDungeons.Checked;
        }

        private void chkTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).GenerateTrees = chkTrees.Checked;
        }

        private void chkHellMode_CheckedChanged(object sender, EventArgs e)
        {
            if (pgMapGen.SelectedObject == null)
                return;
            (pgMapGen.SelectedObject as IMapGenerator).HellMode = chkHellMode.Checked;
        }
        
    }
}
