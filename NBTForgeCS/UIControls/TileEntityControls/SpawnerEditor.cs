using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class SpawnerEditor
        : UserControl,ITileEntityEditor
    {
        public TileEntity TileEntity { 
            get { return ent; }
            set
            {
                ent = (MobSpawner)value;

                ReadEnt();
            }
        }
        private MobSpawner ent;

        public event EventHandler EntityModified;

        public SpawnerEditor(TileEntity e)
        {
            InitializeComponent();
            TileEntity = e;

            
            cmbMob.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMob.DrawItem +=new DrawItemEventHandler(cmbMob_DrawItem);
            cmbMob.Items.Clear();
            cmbMob.ValueMember = "ID";
            cmbMob.DisplayMember = "Name";
            cmbMob.Items.Add(new Pig());
            cmbMob.Items.Add(new Skeleton());
            cmbMob.Items.Add(new Creeper());
            cmbMob.Items.Add(new FallingSand());
            cmbMob.Items.Add(new Sheep());
            cmbMob.Items.Add(new Spider());
        }


        private void ReadEnt()
        {
            numDelay.Value = ent.Delay;
            cmbMob.SelectedText = ent.EntityId;
        }

        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            ent.Delay = (short)numDelay.Value;
            if (this.EntityModified != null)
                this.EntityModified(this, EventArgs.Empty);
        }

        private void cmbMob_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMob.SelectedItem != null)
            {
                ent.EntityId = cmbMob.Text= (cmbMob.SelectedItem as Entity).GetID();
                if (this.EntityModified != null)
                    this.EntityModified(this, EventArgs.Empty);
            }
        }



        private void cmbMob_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Entity ent = (Entity)cmbMob.Items[e.Index];

                // Draw block icon
                g.DrawImage(ent.Image, iconArea);

                // Block name
                SizeF entName = g.MeasureString(ent.ToString(), this.Font);
                Rectangle ctxt = area;
                ctxt.X = iconArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), this.Font, new SolidBrush(e.ForeColor), ctxt);
            }
        }
    }
}
