using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace MineEdit
{
    public class MobSelector : ComboBox
    {
        public MobSelector()
        {
            Items.Add(new Pig());
            Items.Add(new Skeleton());
            Items.Add(new Creeper());
            Items.Add(new FallingSand());
            Items.Add(new Sheep());
            Items.Add(new Spider());
            Items.Add(new Zombie());
            Items.Add(new Giant());
        }
        protected override void  OnDrawItem(DrawItemEventArgs e)
        {

            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Entity ent = (Entity)Items[e.Index];

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
