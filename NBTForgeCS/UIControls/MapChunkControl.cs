using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class MapChunkControl : UserControl
    {
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
        int lh = 1, lw = 1, ly=0;

        /// <summary>
        /// Initialize and set up events for this MapChunk.
        /// </summary>
        /// <param name="mc">Parent mapcontrol</param>
        /// <param name="pos">Position of chunk</param>
        /// <param name="sz">Chunk size</param>
        public MapChunkControl(MapControl mc,Vector3i pos,Vector3i sz)
        {
            parent=mc;
            AssignedChunk = pos;
            ChunkSize = sz;
            Console.WriteLine("{0}: Chunk ({1},{2}), origin ({3},{4})", this, pos.X, pos.Y, pos.X * sz.X, pos.Y * sz.Y);
            InitializeComponent();
            Paint += new PaintEventHandler(MapChunkControl_Paint);
        }

        Vector3i LastPos = new Vector3i(0, 0, 0);
        void MapChunkControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (bmp != null && LastPos==parent.CurrentPosition)
                g.DrawImage(bmp, 0, 0, Width, Height);
            else
            {
                g.FillRectangle(Brushes.Maroon, 0, 0, Width-1, Height-1);
                g.DrawString("[ Loading ]", new Font(FontFamily.GenericSansSerif, 7), Brushes.White, 2, 2);
                Render();
            }
            LastPos = parent.CurrentPosition;
        }

        /// <summary>
        /// I've rewritten this 8x and it's still not working.  Feel free to point out that I'm stupid.
        /// </summary>
        public void Render()
        {
            if (Drawing) return;
            if (parent.Map == null) return;
            Drawing = true;
            int w = Width;
            int h = Height;
            //if (lh == h && lw == w && ly==parent.CurrentPosition.Y) return;
            if (w == 0 || h == 0) return;
            bmp = new Bitmap(w,h);
            bool DrawSquares = (parent.ViewingAngle == ViewAngle.XZ);
            Graphics g = Graphics.FromImage((Image)bmp);
            // Chunk        1,1
            // ChunkSZ      16,16
            // ChunkCoords  16,16
            for (int x = 0; x < Width / parent.ZoomLevel; x++)
            {
                for (int y = 0; y < Height / parent.ZoomLevel; y++)
                {
                    Vector3i blockpos = new Vector3i(x * parent.ZoomLevel, y * parent.ZoomLevel, parent.CurrentPosition.Y);
                    blockpos.X += AssignedChunk.X * ChunkSize.X;
                    blockpos.Y += AssignedChunk.Y * ChunkSize.Y;
                    
                    byte block = parent.Map.GetBlockAt(blockpos);
                    int waterdepth=0;
                    if (block == 0)
                    {
                       // Console.WriteLine("hurr air");
                        int hurr;

                        // Slow for some reason
                        //parent.Map.GetOverview(blockpos, out hurr, out block, out waterdepth);
                    }
                    g.FillRectangle(new SolidBrush(Blocks.GetColor(block)),x*parent.ZoomLevel,y*parent.ZoomLevel,parent.ZoomLevel,parent.ZoomLevel);
                }
            }
            Pen fp = new Pen(Color.Black);
            if (Settings.ShowChunks)
            {
                long ox = AssignedChunk.X * (int)parent.ZoomLevel;
                long oz = AssignedChunk.Y * (int)parent.ZoomLevel;
                int chunksz = (16 * parent.ZoomLevel);
                Font f = new Font(FontFamily.GenericSansSerif, 5);
                for (int x = 0; x < w / 2; x++)
                {
                    if (DrawSquares)
                    {
                         DrawCross(ref g, fp, 0, 0);
                         g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Z), f, Brushes.Black,1,1);
                    }
                    else
                    {
                        g.DrawRectangle(fp, 0, 0, parent.Map.ChunkScale.X*parent.ZoomLevel, parent.Map.ChunkScale.Y * parent.ZoomLevel);
                        g.DrawString(string.Format("Chunk {0},{1}", AssignedChunk.X, AssignedChunk.Z), f, Brushes.Black, 1, 1);
                    }
                }
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

        private void MapChunkControl_Load(object sender, EventArgs e)
        {

        }
    }
}
