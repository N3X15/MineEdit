using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class LivingEditor : UserControl,IEntityEditor
    {
        public Entity Entity { 
            get { return ent; }
            set
            {
                ent = (LivingEntity)value;
                if (!(ent is Sheep))
                    chkSheared.Enabled = false;
                if (!(ent is Pig))
                    chkSaddled.Enabled = false;

                ReadEnt();
            }
        }
        private LivingEntity ent;

        public event EventHandler EntityModified;

        public LivingEditor(Entity e)
        {
            InitializeComponent();
            Entity = e;
        }

        private void ReadEnt()
        {
            numHealth.Value = ent.Health;
            if (ent is Pig)
                chkSaddled.Checked = (ent as Pig).Saddle;
            if (ent is Sheep)
                chkSheared.Checked = (ent as Sheep).Sheared;

        }

        private void numHealth_ValueChanged(object sender, EventArgs e)
        {
            ent.Health = (short)numHealth.Value;
            if (EntityModified != null)
                EntityModified(this,EventArgs.Empty);
        }

        private void chkSaddled_CheckedChanged(object sender, EventArgs e)
        {
            (ent as Pig).Saddle = chkSaddled.Checked;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void chkSheared_CheckedChanged(object sender, EventArgs e)
        {
            (ent as Sheep).Sheared = chkSheared.Checked;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void cmdKill_Click(object sender, EventArgs e)
        {
            numHealth.Value = 0;
        }

    }
}
