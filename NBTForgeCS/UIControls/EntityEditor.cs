using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class EntityEditor : UserControl
    {
        public event EventHandler EntityModified;
        public Vector3i EntityChunk { get; set; }
        public Entity CurrentEntity { get; set; }
        public Vector3d PlayerPos { get; set; }
        public Vector3d SpawnPos { get; set; }
        public EntityEditor()
        {
            InitializeComponent();
            cmbEntities.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEntities.DrawItem += new DrawItemEventHandler(cmbEntities_DrawItem);
            groupBox1.Enabled = false;

            numEntPosX.Minimum = decimal.MinValue;
            numEntPosX.Maximum = decimal.MaxValue;
            numEntPosY.Minimum = decimal.MinValue;
            numEntPosY.Maximum = decimal.MaxValue;
            numEntPosZ.Minimum = decimal.MinValue;
            numEntPosZ.Maximum = decimal.MaxValue;
        }

        void cmbEntities_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width=16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Entity ent = (Entity)cmbEntities.Items[e.Index];
                int dist = (int)Vector3i.Distance(ent.Pos,PlayerPos);

                // Draw entity icon
                g.DrawImage(ent.Image,iconArea);

                // Entity name
                SizeF entName = g.MeasureString(ent.ToString(),this.Font);
                Rectangle ctxt = area;
                ctxt.X = iconArea.Right + 3;
                ctxt.Width=(int)entName.Width+1;
                g.DrawString(ent.ToString(),this.Font,new SolidBrush(e.ForeColor),ctxt);

                // Distance.
                string derp = string.Format("({0}m away)", dist); 
                SizeF entDist = g.MeasureString(derp, this.Font);
                Rectangle distArea = area;
                distArea.X = ctxt.Right + 3;
                distArea.Width = (int)entDist.Width;
                g.DrawString(derp, this.Font, new SolidBrush(Color.FromArgb(128,e.ForeColor)), distArea);
            }
        }

        public void Load(ref IMapHandler map)
        {
            PlayerPos = map.PlayerPos;
            List<Entity> LOLSORT = new List<Entity>();
            foreach (KeyValuePair<Vector3i, Entity> ent in map.Entities)
            {
                LOLSORT.Add(ent.Value);
            }
            EntityDistanceSorter comp = new EntityDistanceSorter();
            comp.PlayerPos = PlayerPos;
            LOLSORT.Sort(comp);
            cmbEntities.Items.Clear();
            foreach (Entity ent in LOLSORT)
            {
                cmbEntities.Items.Add(ent);
            }
        }

        public void Save(ref IMapHandler map)
        {
        }

        private void cmbEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            groupBox1.Enabled = (cmbEntities.SelectedItem != null);
            CurrentEntity = (Entity)cmbEntities.SelectedItem;
            if (CurrentEntity != null)
            {
                numEntPosX.Value = (decimal)CurrentEntity.Pos.X;
                numEntPosY.Value = (decimal)CurrentEntity.Pos.Y;
                numEntPosZ.Value = (decimal)CurrentEntity.Pos.Z;
            }
        }

        private void numEntPosX_ValueChanged(object sender, EventArgs e)
        {
            EntityHasChanged();
        }

        private void EntityHasChanged()
        {
            if (CurrentEntity == null) return;
            CurrentEntity.Pos.X = (double)numEntPosX.Value;
            CurrentEntity.Pos.Y = (double)numEntPosY.Value;
            CurrentEntity.Pos.Z = (double)numEntPosZ.Value;

            if(EntityModified!=null)
                EntityModified(this, EventArgs.Empty);
        }

        private void cmdEntToMe_Click(object sender, EventArgs e)
        {
            numEntPosX.Value = (decimal)PlayerPos.X;
            numEntPosY.Value = (decimal)PlayerPos.Y;
            numEntPosZ.Value = (decimal)PlayerPos.Z;
        }

        private void cmdEntToSpawn_Click(object sender, EventArgs e)
        {
            numEntPosX.Value = (decimal)SpawnPos.X;
            numEntPosY.Value = (decimal)SpawnPos.Y;
            numEntPosZ.Value = (decimal)SpawnPos.Z;
        }
    }

    public class EntityDistanceSorter : Comparer<Entity>
    {
        public Vector3d PlayerPos;

        public override int Compare(Entity x, Entity y)
        {
            double _x = Vector3i.Distance(x.Pos, PlayerPos);
            double _y = Vector3i.Distance(y.Pos, PlayerPos);
            return _x.CompareTo(_y);
        }
    }

}
