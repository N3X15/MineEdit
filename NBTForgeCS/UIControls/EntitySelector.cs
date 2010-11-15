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
