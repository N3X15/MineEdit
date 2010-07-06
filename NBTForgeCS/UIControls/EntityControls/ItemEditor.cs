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
    public partial class ItemEditor : UserControl
    {
        public Item Item;

        event EventHandler ItemChanged;
        public ItemEditor(Item e)
        {
            InitializeComponent();
            this.Item = e;
            cmbType.SelectedItem = Blocks.Get(e.ItemID);
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
        }

        private void numCount_ValueChanged(object sender, EventArgs e)
        {
            this.Item.Count = (byte)numCount.Value;
        }

        private void numDamage_ValueChanged(object sender, EventArgs e)
        {
            Item.Damage = (short)numDamage.Value;
        }

        private void cmdRepair_Click(object sender, EventArgs e)
        {
            Item.Damage = 0;
        }

        private void cmdSuperRepair_Click(object sender, EventArgs e)
        {
            Item.Damage = -600;
        }
    }
}
