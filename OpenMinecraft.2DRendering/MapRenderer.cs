using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using OpenMinecraft.Entities;

namespace OpenMinecraft.Rendering2D
{
    public class MapRenderer
    {
        public bool ShowPlayer { get; set; }
        public bool ShowEntities { get; set; }
        public bool ShowSpawnpoint { get; set; }
        public bool ShowChunkBoundaries { get; set; }
        public IMapHandler Map { get; set; }
        public Vector2i Minimum { get; set; }
        public Vector2i Maximum { get; set; }
        public I2DMapRenderer MyMapRenderer { get; set; }
        public Image Img { get; set; }
        public MapRenderer(IMapHandler mh,I2DMapRenderer mr)
        {
            Map = mh;
            MyMapRenderer = mr;
            Render();
        }

        public void Render()
        {
            int HS = (int)Map.ChunkScale.X;
            if (Img == null)
            {
                Img = new Bitmap(HS, HS);
                Minimum = new Vector2i(0, 0);
                Maximum = new Vector2i(1, 1);
                Map.ForEachChunk(delegate(long X, long Y)
                {
                    Chunk c = Map.GetChunk(X, Y);
                    if (c == null) return;
                    Render(c);
                });
            }
        }

        private void Render(Chunk c)
        {
            int x = (int)c.Position.X;
            int y = (int)c.Position.X;
            int HS = (int)Map.ChunkScale.X;
            if (x < Minimum.X || y < Minimum.Y || x < Maximum.X || y < Maximum.Y)
            {
                // Find how much bigger it needs to be
                int XShift;
                int YShift;
                if(x < Minimum.X || y < Minimum.Y)
                {
                    XShift=x-Minimum.X;
                    YShift=y-Minimum.Y;
                } else {
                    XShift=x-Maximum.X;
                    YShift=y-Maximum.Y;
                }
                XShift*=HS;
                YShift*=HS;
                // Resize Image
                Image img = new Bitmap(Img.Size.Width + (XShift&0xf), Img.Size.Height + (YShift&0xf));
                Graphics g = Graphics.FromImage(img);
                g.DrawImage(Img, new Point(Minimum.X + XShift, Minimum.Y + YShift));

                if (XShift < 0)
                    Minimum.X += XShift;
                else
                    Maximum.X += XShift;
                if (YShift < 0)
                    Minimum.Y += YShift;
                else
                    Maximum.Y += YShift;
                g.Dispose();
            }
            int L = Minimum.X;
            int T = Minimum.Y;
            Bitmap cimg;
            if (MyMapRenderer.RenderChunk(c, out cimg))
            {
                Point Origin = new Point(((int)c.Position.X - L)*HS, ((int)c.Position.Y - T)*HS);
                Graphics g = Graphics.FromImage(Img);
                g.DrawImage(cimg, Origin);
                if (ShowEntities)
                {
                    foreach (KeyValuePair<Guid,Entity> k in c.Entities)
                    {
                        Entity ent = k.Value;
                        g.DrawImage(ent.Image, new PointF((float)(ent.Pos.X - (ent.Image.Size.Width / 2d)), (float)(ent.Pos.Y - (ent.Image.Size.Height / 2d))));
                    }
                }
                if (ShowChunkBoundaries)
                {
                    DrawCross(ref g, 0, 0, Color.White);
                    DrawText(ref g, 1, 1, Color.White, "Chunk " + c.Position.ToString());
                }
            }
        }

        private void DrawText(ref Graphics g, int p, int p_2, Color color, string p_3)
        {
            //throw new NotImplementedException();
        }

        private void DrawCross(ref Graphics g, int p, int p_2, Color color)
        {
            //throw new NotImplementedException();
        }
    }
}
