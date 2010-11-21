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
using OpenMinecraft.TileEntities;

namespace MineEdit
{
    public partial class MapChunkControl : UserControl
    {
        public IMapHandler Map;
        MapControl parent;
        public Dictionary<Guid, PictureBox> EntityButtons = new Dictionary<Guid, PictureBox>();
        public Dictionary<Guid, PictureBox> TileEntityButtons = new Dictionary<Guid, PictureBox>();
        bool Drawing = false;

        /// <summary>
        /// Chunk (x,y,z) index
        /// </summary>
        public Vector3i AssignedChunk = new Vector3i(0, 0, 0);

        /// <summary>
        /// Size of chunk
        /// </summary>
        public Vector3i ChunkSize = new Vector3i(0, 0, 0);
        Bitmap bmp;
        Chunk MyChunk;
        //int lh = 1, lw = 1, ly=0;

        public MapChunkControl()
        {

        }

        /// <summary>
        /// Initialize and set up events for this MapChunk.
        /// </summary>
        /// <param name="mc">Parent mapcontrol</param>
        /// <param name="pos">Position of chunk</param>
        /// <param name="sz">Chunk size</param>
        public MapChunkControl(MapControl mc,Vector3i pos,Vector3i sz)
        {
            parent=mc;
            Map = parent.Map;
            AssignedChunk = pos;
            ChunkSize = sz;
            MyChunk = Map.GetChunk(AssignedChunk);
            //Console.WriteLine("{0}: Chunk ({1},{2}), origin ({3},{4}), size {5}", this, pos.X, pos.Y, pos.X * sz.X, pos.Y * sz.Y, sz);
            InitializeComponent();
            Paint += new PaintEventHandler(MapChunkControl_Paint);
        }

        Vector3i LastPos = new Vector3i(0, 0, 0);
        void MapChunkControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (parent == null)
            {
                g.FillRectangle(Brushes.Blue, 0, 0, Width - 1, Height - 1);
                g.DrawString("[ NO PARENT! ]", new Font(FontFamily.GenericSansSerif, 7), Brushes.White, 2, 2);
                return;
            }
            if (MyChunk == null)
            {
                g.FillRectangle(new SolidBrush(Blocks.GetColor(9)), 0, 0, Width - 1, Height - 1);
                g.DrawString("[ Missing Chunk ]", new Font(FontFamily.GenericSansSerif, 7), Brushes.White, 2, 2);
                return;
            }
            if (bmp != null/* && LastPos==parent.CurrentPosition*/)
                g.DrawImage(bmp, 0, 0, Width, Height);
            else
            {
                g.FillRectangle(Brushes.Maroon, 0, 0, Width-1, Height-1);
                g.DrawString("[ Loading ]", new Font(FontFamily.GenericSansSerif, 7), Brushes.White, 2, 2);
                Render();
            }
            if(parent!=null)
                LastPos = parent.CurrentPosition;
        }

        public void Render()
        {
            //Console.WriteLine("[MapChunkControl::Render()] Rendering chunk ({0},{1})...", AssignedChunk.X, AssignedChunk.Y);
            if (Drawing)
            {
                Console.WriteLine("Drawing, aborting render.");
                return;
            }
            if (Map == null)
            {
                Console.WriteLine("parent.Map = null");
                return;
            }
            if (MyChunk == null)
            {
                Console.WriteLine("MyChunk = null");
                return;
            }
            Drawing = true;
            int w = Width;
            int h = Height;
            //if (lh == h && lw == w && ly==parent.CurrentPosition.Z) return;
            if (w == 0 || h == 0) 
                return;
            bmp = new Bitmap(w, h);
            bool DrawSquares=true;
            if(parent!=null)
                DrawSquares=(parent.ViewingAngle == ViewAngle.TopDown);
            Graphics g = Graphics.FromImage((Image)bmp);
            // No AA.  We WANT pixels :V
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            // Chunk        1,1
            // ChunkSZ      16,16
            // ChunkCoords  16,16
            int z = 128;
            int zoom = 8;
            Color wc = Blocks.GetColor(9);
            if (parent != null)
            {
                z = (int)parent.CurrentPosition.Z;
                if (z > MyChunk.Size.Z - 1)
                    z = (int)MyChunk.Size.Z - 1;
                if (z < 0)
                    z = 0;
                zoom = parent.ZoomLevel;
            }
            for (int x = 0; x < Map.ChunkScale.X; x++)
            {
                for (int y = 0; y < Map.ChunkScale.Y; y++)
                {
                    Vector3i blockpos = new Vector3i(x, y, z);

                    byte block = MyChunk.Blocks[x,y,z];
                    byte ub;
                    int waterdepth = 0;
                    int bh=0,ubh = 0;
                    Color c = Blocks.GetColor(block);
                    Color shadow = Color.Transparent;
                    if (block == 0)
                    {
                        Vector3i bp = blockpos;
                        //bp.X += Map.CurrentPosition.X + (x >> 4);
                        //bp.Y += Map.CurrentPosition.Y + (y >> 4);
                        // Console.WriteLine("hurr air");

                        // BROKEN for some reason (?!)
                        MyChunk.GetOverview(blockpos, out bh,out ubh, out block,out ub, out waterdepth);

                        c = Blocks.GetColor(waterdepth>0 ? ub:block);
                        // Water translucency
                        if (waterdepth > 0)
                        {
                            if (waterdepth > 15)
                                waterdepth = 15;
                            float pct = ((float)waterdepth / 15f) * 100f;
                            c = Color.FromArgb(
                                Utils.Lerp(c.R, wc.R, (int)pct),
                                Utils.Lerp(c.G, wc.G, (int)pct),
                                Utils.Lerp(c.B, wc.B, (int)pct));
                        }

                        // Height (TODO: Replace with lighting?)
                        // 128-50 = 
                        bh = (int)parent.CurrentPosition.Z - bh;
                        if (bh > 0)
                        {
                            //if (bh > 5)
                            //    bh = 5;
                            float pct = ((float)bh / 128f) * 100f;
                            //shadow = Color.FromArgb((int)pct, Color.Black);
                            c = Color.FromArgb(
                                Utils.Lerp(c.R, 0, (int)pct),
                                Utils.Lerp(c.G, 0, (int)pct),
                                Utils.Lerp(c.B, 0, (int)pct));
                            //Console.WriteLine("{0}h = {1}% opacity shadow",bh,pct);
                        }
                    }
                    Vector3i bgpos = new Vector3i(
                        x + (AssignedChunk.X * Map.ChunkScale.X),    
                        y + (AssignedChunk.Y * Map.ChunkScale.Y),    
                        z
                    );
                    if (parent != null)
                    {
                        Color selcol = Color.Orange;
                        int points = 0;
                        if (bgpos.X == parent.SelectedVoxel.X) points++;
                        if (bgpos.Y == parent.SelectedVoxel.Y) points++;
                        if(points>0)
                        {
                            float pct = ((float)points / 3f) * 100f;
                            c = Color.FromArgb(
                                Utils.Lerp(c.R, selcol.R, (int)pct),
                                Utils.Lerp(c.G, selcol.G, (int)pct),
                                Utils.Lerp(c.B, selcol.B, (int)pct));
                        }
                    }
                    g.FillRectangle(new SolidBrush(c), x * zoom, y * zoom, zoom, zoom);
                    //g.FillRectangle(new SolidBrush(shadow), x * zoom, y * zoom, zoom, zoom);
                    //g.DrawString(bh.ToString(), new Font(FontFamily.GenericSansSerif, 5), Brushes.Blue, new PointF((float)(x * zoom), (float)(y * zoom)));
                    if (Settings.ShowWaterDepth && waterdepth > 0)
                        g.DrawString(waterdepth.ToString(), new Font(FontFamily.GenericSansSerif, 5), Brushes.Blue, new PointF((float)(x * zoom), (float)(y * zoom)));
                    if (Settings.ShowGridLines)
                    {
                        if (parent != null && bgpos == parent.SelectedVoxel)
                            g.DrawRectangle(new Pen(Color.Orange), x * zoom, y * zoom, zoom, zoom);
                        else
                            g.DrawRectangle(new Pen(Color.Black), x * zoom, y * zoom, zoom, zoom);
                    }
                }
            }
            Pen fp = new Pen(Color.Black);
            if (Settings.ShowChunks)
            {
                long ox = AssignedChunk.X * (int)zoom;
                long oz = AssignedChunk.Y * (int)zoom;
                int chunksz = (16 * zoom);
                Font f = new Font(FontFamily.GenericSansSerif, 7,FontStyle.Bold);
                for (int x = 0; x < w / 2; x++)
                {
                    if (DrawSquares)
                    {
                        DrawCross(ref g, Pens.Black, 1, 1);
                        DrawCross(ref g, Pens.White, 0, 0);
                        g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Y), f, Brushes.Black, 2, 2);
                        g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Y), f, Brushes.White, 1, 1);
                    }
                    else
                    {
                        g.DrawRectangle(fp, 0, 0, Map.ChunkScale.X * zoom, Map.ChunkScale.Y * zoom);
                        g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Y), f, Brushes.Black, 2, 2);
                        g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Y), f, Brushes.White, 1, 1);
                    }
                }

                /// FUCKING ENTITIES HURRRRRRRRRRRRR ////////////////////////////////
                foreach (KeyValuePair<Guid, PictureBox> p in EntityButtons)
                {
                    p.Value.MouseDown += new MouseEventHandler(EntityMouseDown);
                    Controls.Remove(p.Value);
                    p.Value.Dispose();
                }
                EntityButtons.Clear();
                foreach (KeyValuePair<Guid, Entity> k in MyChunk.Entities)
                {
                    if (
                        k.Value.Pos.X > (AssignedChunk.X * 16) &&
                        k.Value.Pos.X < (AssignedChunk.X * 16) + 16 &&
                        k.Value.Pos.Y > (AssignedChunk.Y * 16) &&
                        k.Value.Pos.Y < (AssignedChunk.Y * 16) + 16)
                    {
                        float x = (float)(k.Value.Pos.X - ((double)MyChunk.Position.X * (double)MyChunk.Size.X));
                        float y = (float)(k.Value.Pos.Y - ((double)MyChunk.Position.Y * (double)MyChunk.Size.Y));
                        x *= parent.ZoomLevel;
                        y *= parent.ZoomLevel;
                        if ((int)k.Value.Pos.Z != parent.CurrentPosition.Z)
                        {
                            g.DrawImage(Fade(k.Value.Image), new RectangleF(x, y, 16, 16));
                            if ((int)k.Value.Pos.Z > parent.CurrentPosition.Z)
                            {
                                g.DrawString("^", f, Brushes.Black, x + 1f, y + 1f);
                                g.DrawString("^", f, Brushes.White, x, y);
                            }
                            else
                            {
                                g.DrawString("v", f, Brushes.Black, x + 1f, y + 1f);
                                g.DrawString("v", f, Brushes.White, x, y);
                            }
                        }
                        else
                        {
                            Guid lol = k.Key;
                            EntityButtons.Add(lol, new PictureBox());
                            EntityButtons[lol].Tag = lol;
                            EntityButtons[lol].Image = k.Value.Image;
                            EntityButtons[lol].BackColor = Color.Transparent;
                            EntityButtons[lol].MouseDown += new MouseEventHandler(EntityMouseDown);
                            EntityButtons[lol].ContextMenu = new ContextMenu(new MenuItem[]{
                                new MenuItem("Entity Editor...",delegate(object s,EventArgs e){
                                    if (parent != null)
                                        parent.SelectEntity(lol);
                                }),
                                new MenuItem("Delete",delegate(object s,EventArgs e){
                                    if (parent != null)
                                        parent.Map.RemoveEntity(parent.Map.Entities[lol]);
                                })
                            });
                            EntityButtons[lol].MouseHover += new EventHandler(EntityHover);
                            EntityButtons[lol].SetBounds((int)x, (int)y, 16, 16);
                            Controls.Add(EntityButtons[lol]);
                        }
                    }
                }

                /// FUCKING TILEENTITIES HURRRRRRRRRRRRR ////////////////////////////////
                foreach (KeyValuePair<Guid, PictureBox> p in TileEntityButtons)
                {
                    p.Value.MouseDown += new MouseEventHandler(TileEntityMouseDown);
                    Controls.Remove(p.Value);
                    p.Value.Dispose();
                }
                TileEntityButtons.Clear();
                foreach (KeyValuePair<Guid, TileEntity> k in MyChunk.TileEntities)
                {
                    if (
                        k.Value.Pos.X > (AssignedChunk.X * 16) &&
                        k.Value.Pos.X < (AssignedChunk.X * 16) + 16 &&
                        k.Value.Pos.Y > (AssignedChunk.Y * 16) &&
                        k.Value.Pos.Y < (AssignedChunk.Y * 16) + 16)
                    {
                        float x = (float)(k.Value.Pos.X - ((double)MyChunk.Position.X * (double)MyChunk.Size.X));
                        float y = (float)(k.Value.Pos.Y - ((double)MyChunk.Position.Y * (double)MyChunk.Size.Y));
                        x *= parent.ZoomLevel;
                        y *= parent.ZoomLevel;
                        if ((int)k.Value.Pos.Z != parent.CurrentPosition.Z)
                        {
                            g.DrawImage(Fade(k.Value.Image), new RectangleF(x, y, 16, 16));
                            if ((int)k.Value.Pos.Z > parent.CurrentPosition.Z)
                            {
                                g.DrawString("^", f, Brushes.Black, x + 1f, y + 1f);
                                g.DrawString("^", f, Brushes.White, x, y);
                            }
                            else
                            {
                                g.DrawString("v", f, Brushes.Black, x + 1f, y + 1f);
                                g.DrawString("v", f, Brushes.White, x, y);
                            }
                        }
                        else
                        {
                            Guid lol = k.Key;
                            TileEntityButtons.Add(lol, new PictureBox());
                            TileEntityButtons[lol].Tag = lol;
                            TileEntityButtons[lol].Image = k.Value.Image;
                            TileEntityButtons[lol].BackColor = Color.Transparent;
                            TileEntityButtons[lol].MouseDown += new MouseEventHandler(TileEntityMouseDown);
                            TileEntityButtons[lol].ContextMenu = new ContextMenu(new MenuItem[]{
                                new MenuItem("TileEntity Editor...",delegate(object s,EventArgs e){
                                    if (parent != null)
                                        parent.SelectTileEntity(lol);
                                }),
                                new MenuItem("Delete",delegate(object s,EventArgs e){
                                    if (parent != null)
                                        parent.Map.RemoveEntity(parent.Map.Entities[lol]);
                                })
                            });
                            TileEntityButtons[lol].MouseHover += new EventHandler(TileEntityHover);
                            TileEntityButtons[lol].SetBounds((int)x, (int)y, 16, 16);
                            Controls.Add(TileEntityButtons[lol]);
                        }
                    }
                }
            }
            Drawing = false;
        }

        void TileEntityMouseDown(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Image Fade(Image image)
        {
            Bitmap bmp = (Bitmap)image;
            for (int x = 0; x < image.Size.Width; x++)
            {
                for (int y = 0; y < image.Size.Width; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    c = Color.FromArgb(128, c);
                    try
                    {
                        bmp.SetPixel(x, y, c);
                    }
                    catch (Exception)
                    {
                        return bmp;
                    }
                }
            }
            return bmp;
        }

        void EntityHover(object sender, EventArgs e)
        {
            Guid lol = (Guid)(sender as PictureBox).Tag;
            ToolTip t = new ToolTip();
            t.Show(parent.Map.Entities[lol].ToString(), ParentForm);
        }

        void TileEntityHover(object sender, EventArgs e)
        {
            Guid lol = (Guid)(sender as PictureBox).Tag;
            ToolTip t = new ToolTip();
            t.Show(parent.Map.TileEntities[lol].ToString(), ParentForm);
        }
            
        void EntityMouseDown(object sender, MouseEventArgs e)
        {
            Guid lol = (Guid)(sender as PictureBox).Tag;
            Entity ent = parent.Map.Entities[lol];
            if (e.Button == MouseButtons.Left)
            {
                foreach (KeyValuePair<Guid, PictureBox> k in new Dictionary<Guid,PictureBox>(EntityButtons))
                {
                    Guid clol = (Guid)k.Value.Tag;
                    EntityButtons[k.Key].BorderStyle = (clol == lol) ? BorderStyle.FixedSingle : BorderStyle.None;
                    EntityButtons[k.Key].BackColor = (clol == lol) ? Color.Orange : Color.Transparent;
                }
            }
        } 

        public void Draw()
        {
            this.Refresh();
        }

        private void DrawCross(ref Graphics g, Pen p, int x, int y)
        {
            g.DrawLine(p, x + 4, y, x - 4, y);
            g.DrawLine(p, x, y + 4, x, y - 4);
        }
        private void DrawBlock(ref Bitmap bmp, int x, int y, Color c)
        {
            bmp.SetPixel(x, y, c);
        }
    }
}
