using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

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
    }
}
