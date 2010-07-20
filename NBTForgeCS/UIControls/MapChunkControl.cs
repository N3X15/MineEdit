using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class MapChunkControl : UserControl
    {
        public IMapHandler Map;
        MapControl parent;
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
        int lh = 1, lw = 1, ly=0;

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

        /// <summary>
        /// I've rewritten this 8x and it's still not working.  Feel free to point out that I'm stupid.
        /// </summary>
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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
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
                zoom = parent.ZoomLevel;
            }
            for (int x = 0; x < Map.ChunkScale.X; x++)
            {
                for (int y = 0; y < Map.ChunkScale.Y; y++)
                {
                    Vector3i blockpos = new Vector3i(x, y, z);

                    byte block = MyChunk.Blocks[x,y,z];

                    int waterdepth = 0;
                    int bh = 0;
                    Color c = Blocks.GetColor(block);
                    Color shadow = Color.Transparent;
                    if (block == 0)
                    {
                        Vector3i bp = blockpos;
                        //bp.X += Map.CurrentPosition.X + (x >> 4);
                        //bp.Y += Map.CurrentPosition.Y + (y >> 4);
                        // Console.WriteLine("hurr air");

                        // BROKEN for some reason (?!)
                        MyChunk.GetOverview(blockpos, out bh, out block, out waterdepth);

                        c = Blocks.GetColor(block);
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
                /*
                foreach (KeyValuePair<Guid, Entity> k in parent.Map.Entities)
                {
                    if (
                        k.Value.Pos.X > (AssignedChunk.X * 16) &&
                        k.Value.Pos.X < (AssignedChunk.X * 16) + 16 &&
                        k.Value.Pos.Y > (AssignedChunk.Y * 16) &&
                        k.Value.Pos.Y < (AssignedChunk.Y * 16) + 16)
                    {
                        int x = (int)k.Value.Pos.X % (int)parent.Map.ChunkScale.X;
                        int y = (int)k.Value.Pos.Y % (int)parent.Map.ChunkScale.Y;
                        x *= parent.ZoomLevel;
                        y *= parent.ZoomLevel;
                        DrawCross(ref g, new Pen(Color.Yellow), x, y);
                        g.DrawString(k.Value.ToString(), f, Brushes.Black, x + 1, y + 1);
                    }
                }*/
            }
            Drawing = false;
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
