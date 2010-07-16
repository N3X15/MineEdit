using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMinecraft;
using OpenMinecraft.Entities;
namespace MineEdit
{
    public partial class EntityEditor : UserControl
    {
        public event EventHandler EntityModified;

        public delegate void EntityHandler(Entity e);
        public event EntityHandler EntityDeleted;
        public event EntityHandler EntityAdded;
        public Vector3i EntityChunk { get; set; }
        public Entity CurrentEntity { get; set; }
        public Vector3d PlayerPos { get; set; }
        public Vector3d SpawnPos { get; set; }

        public IEntityEditor editor;
        private bool PauseEvents;
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
            foreach (KeyValuePair<Guid, Entity> ent in map.Entities)
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
            PauseEvents = true;
            editor = null;
            spltEnt.Panel2.Controls.Clear();
            groupBox1.Enabled = (cmbEntities.SelectedItem != null);
            CurrentEntity = (Entity)cmbEntities.SelectedItem;
            if (CurrentEntity != null)
            {
                Console.WriteLine("Entity {0} selected @ {1} - <{2}, {3}, {4}>", 
                    CurrentEntity.GetID(), 
                    CurrentEntity.Pos,
                    (decimal)CurrentEntity.Pos.X,
                    (decimal)CurrentEntity.Pos.Y,
                    (decimal)CurrentEntity.Pos.Z);
                numEntPosX.Value = Convert.ToDecimal(CurrentEntity.Pos.X);
                numEntPosY.Value = Convert.ToDecimal(CurrentEntity.Pos.Y);
                numEntPosZ.Value = Convert.ToDecimal(CurrentEntity.Pos.Z);

                if (CurrentEntity is LivingEntity)
                    editor = new LivingEditor(CurrentEntity);
                else if (CurrentEntity is Item)
                    editor = new ItemEditor(CurrentEntity);
                else
                {
                    PauseEvents = false;
                    return;
                }
                (editor as Control).Dock = DockStyle.Fill;
                editor.EntityModified += new EventHandler(editor_EntityModified);
                spltEnt.Panel2.Controls.Add((editor as Control));
            }
            PauseEvents = false;
        }

        void editor_EntityModified(object sender, EventArgs e)
        {
            CurrentEntity = editor.Entity;
            EntityHasChanged(true);
        }

        private void numEntPosX_ValueChanged(object sender, EventArgs e)
        {
            EntityHasChanged(false);
        }

        private void EntityHasChanged(bool FromEditor)
        {
            if (PauseEvents == true) return;
            if (CurrentEntity == null) return;
            if (!FromEditor && editor != null)
                editor.Entity = CurrentEntity;
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

        private void cmdRemoveEntity_Click(object sender, EventArgs e)
        {
            Entity ent = (Entity)cmbEntities.SelectedItem;
            if (ent != null)
            {
                cmbEntities.Items.Remove(ent);
                PauseEvents = true;
                editor = null;
                spltEnt.Panel2.Controls.Clear();
                groupBox1.Enabled = false;
                CurrentEntity = null;

                numEntPosX.Value = 0;
                numEntPosY.Value = 0;
                numEntPosZ.Value = 0;

                PauseEvents = false;
                if(EntityDeleted != null)
                    EntityDeleted((Entity)ent);
            }
        }

        internal void SetSelectedEnt(Entity e)
        {
            cmbEntities.SelectedItem = e;
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
