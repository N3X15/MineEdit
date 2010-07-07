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
    public partial class ItemEditor : UserControl,IEntityEditor
    {
        public Item Item;

        event EventHandler ItemChanged;
        public ItemEditor(Entity e)
        {
            InitializeComponent();

            cmbType.Items.Clear();
            cmbType.ValueMember = "ID";
            cmbType.DisplayMember = "Name";
            foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
            {
                cmbType.Items.Add(k.Value);
            }

            this.Entity = e;
        }

        private void cmbType_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block ent = (Block)cmbType.Items[e.Index];

                // Draw block icon
                g.DrawImage(ent.Image, iconArea);

                // ID.
                string idtxt = string.Format("{0:D3} 0x{0:X3}", ent.ID);
                SizeF idSz = g.MeasureString(idtxt, this.Font);
                Rectangle idArea = area;
                idArea.X = iconArea.Right + 3;
                idArea.Width = (int)idSz.Width + 1;
                g.DrawString(idtxt, this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idArea);

                // Block name
                SizeF entName = g.MeasureString(ent.ToString(), this.Font);
                Rectangle ctxt = area;
                ctxt.X = idArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), this.Font, new SolidBrush(e.ForeColor), ctxt);
            }
        }

        private void cmbType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.Item.ItemID = ((Block)cmbType.SelectedItem).ID;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void numCount_ValueChanged(object sender, EventArgs e)
        {
            this.Item.Count = (byte)numCount.Value;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void numDamage_ValueChanged(object sender, EventArgs e)
        {
            Item.Damage = (short)numDamage.Value;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void cmdRepair_Click(object sender, EventArgs e)
        {
            Item.Damage = 0;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void cmdSuperRepair_Click(object sender, EventArgs e)
        {
            Item.Damage = -600;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        public event EventHandler EntityModified;

        public Entity Entity
        {
            get { return Item; }
            set { 
                Item = (Item)value;
                this.SuspendLayout();
                cmbType.SelectedItem = Blocks.Get(Item.ItemID);
                numCount.Value = Item.Count;
                numDamage.Value = Item.Damage;
                this.ResumeLayout();
            }
        }
    }
}
