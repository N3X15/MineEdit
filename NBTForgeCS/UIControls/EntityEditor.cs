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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdRemoveEntity = new System.Windows.Forms.Button();
            this.cmdAddEntity = new System.Windows.Forms.Button();
            this.cmbEntities = new System.Windows.Forms.ComboBox();
            this.spltEnt = new System.Windows.Forms.SplitContainer();
            this.grpCommon = new System.Windows.Forms.GroupBox();
            this.numEntPosZ = new System.Windows.Forms.NumericUpDown();
            this.numEntPosY = new System.Windows.Forms.NumericUpDown();
            this.numEntPosX = new System.Windows.Forms.NumericUpDown();
            this.cmdEntToSpawn = new System.Windows.Forms.Button();
            this.cmdEntToMe = new System.Windows.Forms.Button();
            this.lblPos = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.spltEnt.Panel1.SuspendLayout();
            this.spltEnt.SuspendLayout();
            this.grpCommon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosX)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spltEnt);
            this.splitContainer1.Size = new System.Drawing.Size(565, 429);
            this.splitContainer1.SplitterDistance = 32;
            this.splitContainer1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdRemoveEntity);
            this.panel1.Controls.Add(this.cmdAddEntity);
            this.panel1.Controls.Add(this.cmbEntities);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(565, 32);
            this.panel1.TabIndex = 0;
            // 
            // cmdRemoveEntity
            // 
            this.cmdRemoveEntity.Location = new System.Drawing.Point(487, 3);
            this.cmdRemoveEntity.Name = "cmdRemoveEntity";
            this.cmdRemoveEntity.Size = new System.Drawing.Size(75, 23);
            this.cmdRemoveEntity.TabIndex = 2;
            this.cmdRemoveEntity.Text = "Remove";
            this.cmdRemoveEntity.UseVisualStyleBackColor = true;
            this.cmdRemoveEntity.Click += new System.EventHandler(this.cmdRemoveEntity_Click);
            // 
            // cmdAddEntity
            // 
            this.cmdAddEntity.Enabled = false;
            this.cmdAddEntity.Location = new System.Drawing.Point(430, 3);
            this.cmdAddEntity.Name = "cmdAddEntity";
            this.cmdAddEntity.Size = new System.Drawing.Size(51, 23);
            this.cmdAddEntity.TabIndex = 1;
            this.cmdAddEntity.Text = "Add...";
            this.cmdAddEntity.UseVisualStyleBackColor = true;
            // 
            // cmbEntities
            // 
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Location = new System.Drawing.Point(9, 5);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(415, 21);
            this.cmbEntities.TabIndex = 0;
            this.cmbEntities.SelectedIndexChanged += new System.EventHandler(this.cmbEntities_SelectedIndexChanged);
            // 
            // spltEnt
            // 
            this.spltEnt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltEnt.Location = new System.Drawing.Point(0, 0);
            this.spltEnt.Name = "spltEnt";
            // 
            // spltEnt.Panel1
            // 
            this.spltEnt.Panel1.Controls.Add(this.grpCommon);
            // 
            // spltEnt.Panel2
            // 
            this.spltEnt.Panel2.AutoScroll = true;
            this.spltEnt.Size = new System.Drawing.Size(565, 393);
            this.spltEnt.SplitterDistance = 214;
            this.spltEnt.TabIndex = 4;
            // 
            // grpCommon
            // 
            this.grpCommon.Controls.Add(this.numEntPosZ);
            this.grpCommon.Controls.Add(this.numEntPosY);
            this.grpCommon.Controls.Add(this.numEntPosX);
            this.grpCommon.Controls.Add(this.cmdEntToSpawn);
            this.grpCommon.Controls.Add(this.cmdEntToMe);
            this.grpCommon.Controls.Add(this.lblPos);
            this.grpCommon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCommon.Location = new System.Drawing.Point(0, 0);
            this.grpCommon.Name = "grpCommon";
            this.grpCommon.Size = new System.Drawing.Size(214, 393);
            this.grpCommon.TabIndex = 0;
            this.grpCommon.TabStop = false;
            this.grpCommon.Text = "Common Entity Properties";
            // 
            // numEntPosZ
            // 
            this.numEntPosZ.DecimalPlaces = 2;
            this.numEntPosZ.Location = new System.Drawing.Point(149, 51);
            this.numEntPosZ.Name = "numEntPosZ";
            this.numEntPosZ.Size = new System.Drawing.Size(64, 20);
            this.numEntPosZ.TabIndex = 2;
            this.numEntPosZ.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // numEntPosY
            // 
            this.numEntPosY.DecimalPlaces = 2;
            this.numEntPosY.Location = new System.Drawing.Point(79, 51);
            this.numEntPosY.Name = "numEntPosY";
            this.numEntPosY.Size = new System.Drawing.Size(64, 20);
            this.numEntPosY.TabIndex = 2;
            this.numEntPosY.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // numEntPosX
            // 
            this.numEntPosX.DecimalPlaces = 2;
            this.numEntPosX.Location = new System.Drawing.Point(9, 51);
            this.numEntPosX.Name = "numEntPosX";
            this.numEntPosX.Size = new System.Drawing.Size(64, 20);
            this.numEntPosX.TabIndex = 2;
            this.numEntPosX.ValueChanged += new System.EventHandler(this.numEntPosX_ValueChanged);
            // 
            // cmdEntToSpawn
            // 
            this.cmdEntToSpawn.Location = new System.Drawing.Point(119, 22);
            this.cmdEntToSpawn.Name = "cmdEntToSpawn";
            this.cmdEntToSpawn.Size = new System.Drawing.Size(69, 23);
            this.cmdEntToSpawn.TabIndex = 1;
            this.cmdEntToSpawn.Text = "To Spawn";
            this.cmdEntToSpawn.UseVisualStyleBackColor = true;
            this.cmdEntToSpawn.Click += new System.EventHandler(this.cmdEntToSpawn_Click);
            // 
            // cmdEntToMe
            // 
            this.cmdEntToMe.Location = new System.Drawing.Point(59, 22);
            this.cmdEntToMe.Name = "cmdEntToMe";
            this.cmdEntToMe.Size = new System.Drawing.Size(54, 23);
            this.cmdEntToMe.TabIndex = 1;
            this.cmdEntToMe.Text = "To Me";
            this.cmdEntToMe.UseVisualStyleBackColor = true;
            this.cmdEntToMe.Click += new System.EventHandler(this.cmdEntToMe_Click);
            // 
            // lblPos
            // 
            this.lblPos.AutoSize = true;
            this.lblPos.Location = new System.Drawing.Point(6, 27);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(47, 13);
            this.lblPos.TabIndex = 0;
            this.lblPos.Text = "Position:";
            // 
            // EntityEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EntityEditor";
            this.Size = new System.Drawing.Size(565, 429);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.spltEnt.Panel1.ResumeLayout(false);
            this.spltEnt.ResumeLayout(false);
            this.grpCommon.ResumeLayout(false);
            this.grpCommon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEntPosX)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdRemoveEntity;
        private System.Windows.Forms.Button cmdAddEntity;
        private System.Windows.Forms.ComboBox cmbEntities;
        private System.Windows.Forms.SplitContainer spltEnt;
        private System.Windows.Forms.GroupBox grpCommon;
        private System.Windows.Forms.NumericUpDown numEntPosZ;
        private System.Windows.Forms.NumericUpDown numEntPosY;
        private System.Windows.Forms.NumericUpDown numEntPosX;
        private System.Windows.Forms.Button cmdEntToSpawn;
        private System.Windows.Forms.Button cmdEntToMe;
        private System.Windows.Forms.Label lblPos;

        #endregion

        public EntityEditor()
        {
            InitializeComponent();
            cmbEntities.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbEntities.DrawItem += new DrawItemEventHandler(cmbEntities_DrawItem);
            grpCommon.Enabled = false;

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
            grpCommon.Enabled = (cmbEntities.SelectedItem != null);
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
                grpCommon.Enabled = false;
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
