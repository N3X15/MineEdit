using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK;
using System.Drawing.Imaging;
using TexLib;
namespace MineEdit
{
    public partial class MapControlGL : UserControl
    {
        private Vector3i _CurrentPosition = new Vector3i(0, 0, 64);
        private Vector3i _TargetPos = new Vector3i(0, 0, 64);
        private Vector3i _SelectedVoxel = new Vector3i(-1, -1, -1);
        private IMapHandler _Map;
        private ViewAngle _ViewingAngle = ViewAngle.TopDown;
        private Dictionary<Vector3i, int> Chunks = new Dictionary<Vector3i, int>();
        private int _ZoomLevel = 8;
        private bool Dragging = false;
        /// <summary>
        /// Currently active brush material.
        /// </summary>
        public short CurrentMaterial = 0;

        public MapControlGL()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Black;

            glControl.Paint += GLControl_Paint;

            glControl.Resize+=new EventHandler(glControl_Resize);
            Application.Idle += new EventHandler(Application_Idle);
            Disposed += new EventHandler(MapControlTry2_Disposed);

            DoLayout();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Render();
        }

        void GLControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        void Render()
        {
            try
            {
                glControl.MakeCurrent();
            }
            catch(Exception) { return; }

            GL.Clear(ClearBufferMask.ColorBufferBit); 
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -1, 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            if (_Map != null)
            {
                // Animate panning
                //if (_TargetPos.X != _CurrentPosition.X || _TargetPos.Y != _CurrentPosition.Y)
                //{
                    _CurrentPosition.X = (long)Utils.Lerp((float)_CurrentPosition.X, (float)_TargetPos.X, CriticalDamp.getInterpolant(0.1f, false));
                    _CurrentPosition.Y = (long)Utils.Lerp((float)_CurrentPosition.Y, (float)_TargetPos.Y, CriticalDamp.getInterpolant(0.1f, false));
                //}


                Vector3i Sides = new Vector3i(_Map.ChunkScale.X * ZoomLevel, _Map.ChunkScale.Y * ZoomLevel, _Map.ChunkScale.Z * ZoomLevel);
                Vector3i min = new Vector3i(_CurrentPosition.X - ((glControl.Width / 2) / ZoomLevel), _CurrentPosition.Y - ((glControl.Height / 2) / ZoomLevel), 0);
                Vector3i max = new Vector3i(_CurrentPosition.X + ((glControl.Width / 2) / ZoomLevel), _CurrentPosition.Y + ((glControl.Height / 2) / ZoomLevel), 0);

                int REGION_SZ = (int)_Map.ChunkScale.X;
                for (int x = 0; x < glControl.Width/Sides.X; x++)
                {
                    for (int y = 0; y < glControl.Height/Sides.Y; y++)
                    {
                        Vector3i cc = new Vector3i(x+(min.X / _Map.ChunkScale.X), y + (min.Y / _Map.ChunkScale.Y), (CurrentPosition.Z / _Map.ChunkScale.X));
                        //Console.WriteLine(cc);
                        OpenTK.Graphics.TextPrinter tp = new OpenTK.Graphics.TextPrinter();
                        tp.Begin();
                        tp.Print(string.Format("({0},{1})", cc.X, cc.Y), Font, Color.White, new RectangleF((glControl.Width / 2) + cc.X, (glControl.Height / 2) + cc.Y, (glControl.Width / 2) + Map.ChunkScale.X, (glControl.Height / 2) + Map.ChunkScale.Y));
                        if (!Chunks.ContainsKey(cc))
                        {
                            Bitmap mcc = new Bitmap((int)_Map.ChunkScale.X, (int)_Map.ChunkScale.Y);
                            mcc = (Bitmap)RenderTopdown(cc.X, cc.Y);
                            Chunks.Add(cc, TexUtil.CreateTextureFromBitmap(mcc));
                            gl_rect_2d((int)cc.X, (int)cc.Y, (int)(cc.X + _Map.ChunkScale.X), (int)(cc.Y + _Map.ChunkScale.Y), Color.Black, true);
                        }
                        else
                        {

                            gl_rect_2d((int)cc.X, (int)cc.Y, (int)(cc.X + _Map.ChunkScale.X), (int)(cc.Y + _Map.ChunkScale.Y), Color.White, true);
                            drawImage((int)(cc.X), (int)(cc.Y), (int)_Map.ChunkScale.X, (int)_Map.ChunkScale.Y, Chunks[cc], Color.White);
                        }
                        tp.End();
                    }
                }
                GL.Flush();
            }
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
            glControl.SwapBuffers();
        }

        private void drawImage(int _x, int _y, int _w, int _h, int pic, Color color)
        {
            GL.PushMatrix();
            {
                GL.Translate((float)_x, (float)_y, 0f);
                GL.BindTexture(TextureTarget.Texture2D, pic);
                GL.Color4(color);
                //float x = _x / Width;
                //float y = _y / Height;
                //float w = _w / Width;
                //float h = _h / Height;
                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(0f, 1f); GL.Vertex2(0, _h);
                    GL.TexCoord2(1f, 1f); GL.Vertex2(_w, 0);
                    GL.TexCoord2(1f, 0f); GL.Vertex2(_w, _h);
                    GL.TexCoord2(0f, 0f); GL.Vertex2(0, 0);
                }
                GL.End();
            }
            GL.PopMatrix();

            if (GL.GetError() != ErrorCode.NoError)
                Console.WriteLine(GL.GetError().ToString());
        }

        //private void gl_draw_image()
        //{
        //    glPushMatrix();
        //    {
        //        glTranslatef((F32)x, (F32)y, 0.f);
        //        if( degrees )
        //        {
        //            F32 offset_x = F32(width/2);
        //            F32 offset_y = F32(height/2);
        //            glTranslatef( offset_x, offset_y, 0.f);
        //            glRotatef( degrees, 0.f, 0.f, 1.f );
        //            glTranslatef( -offset_x, -offset_y, 0.f );
        //        }

        //        image->bind();

        //        glColor4fv(color.mV);

        //        glBegin(GL_QUADS);
        //        {
        //            glTexCoord2f(1.f, 1.f);
        //            glVertex2i(width, height );
        //            glTexCoord2f(0.f, 1.f);
        //            glVertex2i(0, height );
        //            glTexCoord2f(0.f, 0.f);
        //            glVertex2i(0, 0);
        //            glTexCoord2f(1.f, 0.f);
        //            glVertex2i(width, 0);
        //        }
        //        glEnd();
        //    }
        //    glPopMatrix();
        //}
        void MapControlTry2_Disposed(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Order all child MapChunks to re-render and refresh.
        /// </summary>
        public void RegenTiles()
        {
            foreach (KeyValuePair<Vector3i, int> p in new Dictionary<Vector3i,int>(Chunks))
            {
                GL.DeleteTexture(Chunks[p.Key]);
                Chunks[p.Key]=TexUtil.CreateTextureFromBitmap((Bitmap)RenderTopdown(p.Key.X, p.Key.Y));
            }
        }

        /// <summary>
        /// Perform map layout stuff.
        /// </summary>
        void DoLayout()
        {
            if(_Map==null) return;

            if (Width != _Map.Width * ZoomLevel)
            {
                Width = _Map.Width * ZoomLevel;
                Height = _Map.Height * ZoomLevel;
                Top = Left = 0;
            }
            Vector3i Sides = new Vector3i(_Map.ChunkScale.X * ZoomLevel, _Map.ChunkScale.Y * ZoomLevel, _Map.ChunkScale.Z * ZoomLevel);
            Vector3i min = new Vector3i(CurrentPosition.X - ((Width / 2) / ZoomLevel), CurrentPosition.Y - ((Height / 2) / ZoomLevel), 0);
            Vector3i max = new Vector3i(CurrentPosition.X + ((Width / 2) / ZoomLevel), CurrentPosition.Y + ((Height / 2) / ZoomLevel), 0);

            min.X = Math.Max(min.X, _Map.MapMin.X);
            max.X = Math.Min(max.X, _Map.MapMax.X);

            min.Y = Math.Max(min.Y, _Map.MapMin.Y);
            max.Y = Math.Min(max.Y, _Map.MapMax.Y);

            min.Z = Math.Max(min.Z, _Map.MapMin.Z);
            max.Z = Math.Min(max.Z, _Map.MapMax.Z);

            //Console.WriteLine("DoLayout(): {0} - {1}", min, max);
            Dictionary<Vector3i, int> c = new Dictionary<Vector3i,int>(Chunks);
            foreach (KeyValuePair<Vector3i, int> p in Chunks)
            {
                if (p.Key.X > max.X / _Map.ChunkScale.X ||
                    p.Key.X < min.X / _Map.ChunkScale.X ||
                    p.Key.Y > max.Y / _Map.ChunkScale.Y ||
                    p.Key.Y < min.Y / _Map.ChunkScale.Y ||
                    p.Key.Z > max.Z / _Map.ChunkScale.Z ||
                    p.Key.Z < min.Z / _Map.ChunkScale.Z)
                {
                    c.Remove(p.Key);
                    GL.DeleteTexture(p.Value);
                }
            }
            Chunks = c;

            switch (ViewingAngle)
            {
                case ViewAngle.XY:  // Slice N-S?
                    LayoutXY(Sides, min, max);
                    break;
                case ViewAngle.TopDown: // Top Down
                    //LayoutTopdown(Sides, min, max);
                    break;
                case ViewAngle.YZ: // Slice E-W?
                    LayoutYZ(Sides, min, max);
                    break;
            }
            LayoutControls();
            //Refresh();
        }

        private void LayoutControls()
        {
            /*        v-- 6px border
                [U]    [LU]
             [L]   [R]
                [D]    [LD]
            */

            btnLyrDown.SetBounds(Width - 23, Height - 23, 22, 22);
            btnLyrUp.SetBounds(Width - 23, Height - 67, 22, 22);
            btnUp.SetBounds(Width - 73, Height - 67, 22, 22);
            btnDown.SetBounds(Width - 73, Height - 23, 22, 22);
            btnLeft.SetBounds(Width - 95, Height - 45, 22, 22);
            btnRight.SetBounds(Width - 51, Height - 45, 22, 22);
        }

        private void LayoutYZ(Vector3i Sides, Vector3i min, Vector3i max)
        {
            for (int y = 0; y < Width/Sides.Y; y++)
            {
                for (int z = 0; z < Height/Sides.Z; z++)
                {
                    Vector3i cc = new Vector3i((min.X / _Map.ChunkScale.X), y + (min.Y / _Map.ChunkScale.Y), z+(CurrentPosition.Z / _Map.ChunkScale.X));
                    if (!Chunks.ContainsKey(cc))
                    {
                        Bitmap mcc = new Bitmap((int)_Map.ChunkScale.Y,(int)_Map.ChunkScale.Z);
                        Chunks.Add(cc, TexUtil.CreateTextureFromBitmap(mcc));
                    }
                    else
                    {
                        /*
                        MapChunkControl mcc = Chunks[cc];
                        mcc.SetBounds((int)(cc.Y * _Map.ChunkScale.Y)+6, (int)(cc.Z * _Map.ChunkScale.Z)+6, (int)(_Map.ChunkScale.Y), (int)(_Map.ChunkScale.Z));
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);*/
                    }
                }
            }
        }

        /// <summary>
        /// Only version that works correctly atm.
        /// </summary>
        /// <param name="Sides"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void LayoutTopdown(Vector3i Sides, Vector3i min, Vector3i max)
        {
            //Console.WriteLine("Client Rectangle: {0}", pbMap.ClientRectangle);
            //for (int x = pbMap.ClientRectangle.Left;x < pbMap.ClientRectangle.Right; x++)
            //{
            //    for (int y = pbMap.ClientRectangle.Top; y < pbMap.ClientRectangle.Bottom; y++)
            //    {
            //        Vector3i cc = new Vector3i(x/Map.ChunkScale.X,y/Map.ChunkScale.Y, (min.Y / _Map.ChunkScale.Z));
            //        if (!Chunks.ContainsKey(cc))
            //        {
            //            Bitmap mcc = new Bitmap((int)_Map.ChunkScale.X,(int)_Map.ChunkScale.Y);
            //            mcc = (Bitmap)RenderTopdown(cc.X, cc.Y);
            //            Chunks.Add(cc, mcc);
            //        }
            //    }
            //}
        }



        public static Rectangle GetVisibleRectangle(ScrollableControl sc, Control child)
        {
            Rectangle work = child.Bounds;
            work.Intersect(sc.ClientRectangle);
            return work;
        }


        private Image RenderTopdown(long X, long Y)
        {
            double distance = Math.Sqrt(
                Math.Pow((double)(X*_Map.ChunkScale.X) - (double)_CurrentPosition.X,2) + 
                Math.Pow((double)(Y*_Map.ChunkScale.Y) - (double)_CurrentPosition.Y,2)
                );
            //Console.WriteLine("Rendering chunk ({0},{1}) @ dist {2}", X, Y, distance);
            if (distance > 256)
                return new Bitmap((int)_Map.ChunkScale.X,(int)_Map.ChunkScale.Y);

            Bitmap bmp = new Bitmap((int)_Map.ChunkScale.X,(int)_Map.ChunkScale.Y);
            for (int x = 0; x < _Map.ChunkScale.X; x++)
            {
                for (int y = 0; y < _Map.ChunkScale.Y; y++)
                {
                    byte block = Map.GetBlockIn(X, Y, new Vector3i(x, y, CurrentPosition.Z));
                    Color color = Color.White;
                    if (block == 0)
                    {
                        //_Map.GetOverview(new Vector3i(x + (X * _Map.ChunkScale.X), y + (Y * _Map.ChunkScale.Y), CurrentPosition.Z),
                    }
                    bmp.SetPixel(x, y, Blocks.GetColor(block));
                }
            }
            return bmp;
        }

        private void LayoutXY(Vector3i Sides, Vector3i min, Vector3i max)
        {
            for (int x = 0; x < (Width/Sides.X); x++)
            {
                for (int y = 0; y < (Height/Sides.Y); y++)
                {
                    Vector3i cc = new Vector3i(x + (min.X / _Map.ChunkScale.X), y + (min.Y / _Map.ChunkScale.Y), (CurrentPosition.Z / _Map.ChunkScale.X));
                    if (!Chunks.ContainsKey(cc))
                    {
                        Bitmap mcc = new Bitmap((int)(_Map.ChunkScale.X), (int)(_Map.ChunkScale.Y));
                        Chunks.Add(cc, TexUtil.CreateTextureFromBitmap(mcc));
                    }
                }
            }
        }

        /// <summary>
        /// Current position
        /// </summary>
        public Vector3i CurrentPosition
        {
            get { return _CurrentPosition; }
            set
            {
                _TargetPos = value;
                //CurrentPosition.Z = CurrentPosition.Z % _Map.ChunkScale.Z;
                RegenTiles();
            }
        }

        /// <summary>
        /// Currently selected voxel.
        /// </summary>
        public Vector3i SelectedVoxel
        {
            get { return _SelectedVoxel; }
            set
            {
                _SelectedVoxel = value;
                RegenTiles();
            }
        }
        /// <summary>
        /// Zoom Level.
        /// </summary>
        public int ZoomLevel
        {
            get
            {
                return _ZoomLevel;
            }
            set
            {
                _ZoomLevel = Math.Max(1,value);
                DoLayout();
            }
        }

        /// <summary>
        /// Specify which slicing angle to take.
        /// </summary>
        public ViewAngle ViewingAngle
        {
            get { return _ViewingAngle; }
            set
            {
                _ViewingAngle = value;
                Chunks.Clear();
                DoLayout();
            }
        }

        /// <summary>
        /// Gets or sets the map handler.
        /// </summary>
        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                //GL.ClearColor(Color.Black);
                Chunks.Clear();
                _Map = value;
                DoLayout();
                Render();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            CurrentPosition.Y -= _Map.ChunkScale.Y;
            DoLayout();
            Refresh();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            CurrentPosition.X += _Map.ChunkScale.X;
            DoLayout();
            Refresh();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            CurrentPosition.X -= _Map.ChunkScale.X;
            DoLayout();
            Refresh();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            CurrentPosition.Y += _Map.ChunkScale.Y;
            DoLayout();
            Refresh();
        }

        private void btnLyrUp_Click(object sender, EventArgs e)
        {
            CurrentPosition.Z++;
            DoLayout();
            Refresh();
        }

        private void btnLyrDown_Click(object sender, EventArgs e)
        {
            CurrentPosition.Z--;
            DoLayout();
            Refresh();
        }

        private void MapControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f); 
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            SetupViewport();

            TexUtil.InitTexturing();

            if (GL.GetError() != ErrorCode.NoError)
                Console.WriteLine("*** "+GL.GetError());
            else
                Console.WriteLine("*** GLControl successfully enabled.");
        }

        private void SetupViewport()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0f, (float)w, 0.0f, (float)h, -1.0f, 1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

        }
        void gl_rect_2d(int left, int top, int right, int bottom, bool filled )
        {
            if( filled )
            {
                GL.Begin(BeginMode.Quads);
                         GL.Vertex2(left, top);
                         GL.Vertex2(left, bottom);
                         GL.Vertex2(right, bottom);
                         GL.Vertex2(right, top);
                GL.End();
            }
            else
            {
                top--;
                right--;
                GL.Begin(BeginMode.LineStrip);
                    GL.Vertex2(left, top);
                    GL.Vertex2(left, bottom);
                    GL.Vertex2(right, bottom);
                    GL.Vertex2(right, top);
                    GL.Vertex2(left, top);
                GL.End();
            }
        }

        void gl_rect_2d(int left, int top, int right, int bottom, Color color, bool filled )
        {
            GL.Color4(color);
            gl_rect_2d(left, top, right, bottom, filled );
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            OpenTK.GLControl c = sender as OpenTK.GLControl;

            if (c.ClientSize.Height == 0)
                c.ClientSize = new System.Drawing.Size(c.ClientSize.Width, 1);

            SetupViewport();

            LayoutControls();
        }
    }
}
