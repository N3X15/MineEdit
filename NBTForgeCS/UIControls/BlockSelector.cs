using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using OpenMinecraft;

namespace MineEdit
{
    public class BlockSelector:ComboBox
    {
        protected bool _blocksonly=false;
        public bool BlocksOnly { 
            get
            {
                return _blocksonly;
            }
            set
            {
                _blocksonly = value;
                
                Items.Clear();
                foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
                {
                    if (_blocksonly || k.Value.ID <= 255)
                        Items.Add(k.Value);
                }
            }
        }
        public BlockSelector()
        {
            Items.Clear();
            ValueMember = "ID";
            DisplayMember = "Name";

            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
            {
                Items.Add(k.Value);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (Items.Count > 90 && _blocksonly)
            {
                Items.Clear();
                foreach (KeyValuePair<short, Block> k in Blocks.BlockList)
                {
                    if (!_blocksonly || k.Value.ID <= 255)
                        Items.Add(k.Value);
                }
            }
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block ent = (Block)Items[e.Index];

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

    }
}
