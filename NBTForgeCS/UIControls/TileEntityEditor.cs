/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMinecraft;
using OpenMinecraft.TileEntities;

namespace MineEdit
{
    public partial class TileEntityEditor : UserControl
    {

        public delegate void TileEntityHandler(TileEntity e);
        public event TileEntityHandler EntityDeleted;
        public event TileEntityHandler EntityAdded;
        public event TileEntityHandler EntityModified;
        public Vector3i EntityChunk { get; set; }
        public TileEntity CurrentEntity { get; set; }
        public Vector3d PlayerPos { get; set; }
        public Vector3d SpawnPos { get; set; }

        public ITileEntityEditor editor;
        private bool PauseEvents;
        public TileEntityEditor()
        {
            InitializeComponent();
            cmbTEntities.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbTEntities.DrawItem += new DrawItemEventHandler(cmbEntities_DrawItem);
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
                TileEntity ent = (TileEntity)cmbTEntities.Items[e.Index];
                int dist = (int)Vector3i.Distance((Vector3d)ent.Pos,PlayerPos);

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
            List<TileEntity> LOLSORT = new List<TileEntity>();
            foreach (KeyValuePair<Guid, TileEntity> ent in map.TileEntities)
            {
                LOLSORT.Add(ent.Value);
            }
            TileEntityDistanceSorter comp = new TileEntityDistanceSorter();
            comp.PlayerPos = this.PlayerPos;
            LOLSORT.Sort(comp);
            cmbTEntities.Items.Clear();
            foreach (TileEntity ent in LOLSORT)
            {
                cmbTEntities.Items.Add(ent);
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
            groupBox1.Enabled = (cmbTEntities.SelectedItem != null);
            CurrentEntity = (TileEntity)cmbTEntities.SelectedItem;
            if (CurrentEntity != null)
            {
                Console.WriteLine("Entity {0} selected @ {1} - <{2}, {3}, {4}>", 
                    CurrentEntity.UUID, 
                    CurrentEntity.Pos,
                    (decimal)CurrentEntity.Pos.X,
                    (decimal)CurrentEntity.Pos.Y,
                    (decimal)CurrentEntity.Pos.Z);
                numEntPosX.Value = Convert.ToDecimal(CurrentEntity.Pos.X);
                numEntPosY.Value = Convert.ToDecimal(CurrentEntity.Pos.Y);
                numEntPosZ.Value = Convert.ToDecimal(CurrentEntity.Pos.Z);

                if (CurrentEntity is Chest)
                    editor = new ChestEditor(CurrentEntity);
                else if (CurrentEntity is MobSpawner)
                    editor = new SpawnerEditor(CurrentEntity);
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
            CurrentEntity = editor.TileEntity;
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
                editor.TileEntity = CurrentEntity;
            CurrentEntity.Pos.X = (int)numEntPosX.Value;
            CurrentEntity.Pos.Y = (int)numEntPosY.Value;
            CurrentEntity.Pos.Z = (int)numEntPosZ.Value;

            if(EntityModified!=null)
                EntityModified(CurrentEntity);
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
            TileEntity ent = (TileEntity)cmbTEntities.SelectedItem;
            if (ent != null)
            {
                cmbTEntities.Items.Remove(ent);
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
                    EntityDeleted((TileEntity)ent);
            }
        }

        internal void SetSelectedTEnt(TileEntity e)
        {
            cmbTEntities.SelectedItem = e;
        }
    }

    public class TileEntityDistanceSorter : Comparer<TileEntity>
    {
        public Vector3d PlayerPos;

        public override int Compare(TileEntity x, TileEntity y)
        {
            double _x = Vector3i.Distance((Vector3d)x.Pos, PlayerPos);
            double _y = Vector3i.Distance((Vector3d)y.Pos, PlayerPos);
            return _x.CompareTo(_y);
        }
    }

}
