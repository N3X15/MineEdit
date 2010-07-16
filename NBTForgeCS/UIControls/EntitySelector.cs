using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;
using OpenMinecraft;
using OpenMinecraft.Entities;

namespace MineEdit
{
    public class EntitySelector : ComboBox
    {
        public EntitySelector()
        {

        }

        private Vector3d PlayerPos;
        private IMapHandler _Map;
        public IMapHandler Map
        {
            get
            {
                return _Map;
            }
            set
            {
                _Map = value;

                PlayerPos = _Map.PlayerPos;
                List<Entity> LOLSORT = new List<Entity>();
                foreach (KeyValuePair<Guid, Entity> ent in _Map.Entities)
                {
                    LOLSORT.Add(ent.Value);
                }
                EntityDistanceSorter comp = new EntityDistanceSorter();
                comp.PlayerPos = PlayerPos;
                LOLSORT.Sort(comp);
                Items.Clear();
                foreach (Entity ent in LOLSORT)
                {
                    Items.Add(ent);
                }
            }
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
                int dist = (int)Vector3i.Distance(ent.Pos, PlayerPos);

                // Draw entity icon
                g.DrawImage(ent.Image, iconArea);

                // Entity name
                SizeF entName = g.MeasureString(ent.ToString(), this.Font);
                Rectangle ctxt = area;
                ctxt.X = iconArea.Right + 3;
                ctxt.Width = (int)entName.Width + 1;
                g.DrawString(ent.ToString(), this.Font, new SolidBrush(e.ForeColor), ctxt);

                // Distance.
                string derp = string.Format("({0}m away)", dist);
                SizeF entDist = g.MeasureString(derp, this.Font);
                Rectangle distArea = area;
                distArea.X = ctxt.Right + 3;
                distArea.Width = (int)entDist.Width;
                g.DrawString(derp, this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), distArea);
            }
        }

    }
}
