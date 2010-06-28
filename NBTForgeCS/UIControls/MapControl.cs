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
    public partial class MapControl: UserControl
    {
        private float mScale = 256; // Size of a region in pixels
        private float mPixelsPerMeter; // world meters to map pixels
        private float mObjectMapTPM; // texels per meter on map
        private float mObjectMapPixels; // Width of object map in pixels
        private int mDotRadius; // Size of avatar markers
        private float mTargetPanX;
        private float mTargetPanY;
        private float mCurPanX;
        private float mCurPanY;
        private bool mUpdateNow;
        private Vector3d mObjectImageCenterGlobal;

        private Vector3i _CurrentPosition = new Vector3i(0, 0, 64);
        private Vector3i _SelectedVoxel = new Vector3i(-1, -1, -1);
        private bool Drawing = false;
        private IMapHandler _Map;
        private ViewAngle _ViewingAngle = ViewAngle.TopDown;
        private Dictionary<Vector3i, MapChunkControl> Chunks = new Dictionary<Vector3i, MapChunkControl>();
        private int _ZoomLevel = 8;
        private bool Dragging = false;
        /// <summary>
        /// Currently active brush material.
        /// </summary>
        public short CurrentMaterial = 0;

        public MapControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Black;

            Resize += new EventHandler(MapControl_Resize);
            MouseDown += new MouseEventHandler(MapControl_MouseDown);
            DoLayout();
        }

        void  MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    // DoBrush(e.Location);
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    this.Cursor = Cursors.Hand;
                    this.Dragging = true;
                    break;
            }
        }

        /// <summary>
        /// Order all child MapChunks to re-render and refresh.
        /// </summary>
        public void Render()
        {
            foreach (KeyValuePair<Vector3i, MapChunkControl> p in Chunks)
            {
                p.Value.Render();
            }
        }
        void MapControl_Resize(object sender, EventArgs e)
        {
            DoLayout();
        }

        /// <summary>
        /// Perform map layout stuff.
        /// </summary>
        void DoLayout()
        {
            if(_Map==null) return;

            Vector3i Sides = new Vector3i(_Map.ChunkScale.X * ZoomLevel, _Map.ChunkScale.Y * ZoomLevel, _Map.ChunkScale.Z * ZoomLevel);
            Vector3i min = new Vector3i(CurrentPosition.X - ((Width / 2) / ZoomLevel), CurrentPosition.Y - ((Height / 2) / ZoomLevel), 0);
            Vector3i max = new Vector3i(CurrentPosition.X + ((Width / 2) / ZoomLevel), CurrentPosition.Y + ((Height / 2) / ZoomLevel), 0);

            min.X = Math.Max(min.X, _Map.MapMin.X);
            max.X = Math.Min(max.X, _Map.MapMax.X);

            min.Y = Math.Max(min.Y, _Map.MapMin.Y);
            max.Y = Math.Min(max.Y, _Map.MapMax.Y);

            min.Z = Math.Max(min.Z, _Map.MapMin.Z);
            max.Z = Math.Min(max.Z, _Map.MapMax.Z);

            Console.WriteLine("DoLayout(): {0} - {1}", min, max);
            Dictionary<Vector3i, MapChunkControl> c = new Dictionary<Vector3i,MapChunkControl>(Chunks);
            foreach (KeyValuePair<Vector3i, MapChunkControl> p in Chunks)
            {
                if (p.Key.X > max.X / _Map.ChunkScale.X ||
                    p.Key.X < min.X / _Map.ChunkScale.X ||
                    p.Key.Y > max.Y / _Map.ChunkScale.Y ||
                    p.Key.Y < min.Y / _Map.ChunkScale.Y ||
                    p.Key.Z > max.Z / _Map.ChunkScale.Z ||
                    p.Key.Z < min.Z / _Map.ChunkScale.Z)
                {
                    c.Remove(p.Key);
                    Controls.Remove(p.Value);
                }
            }
            Chunks = c;

            switch (ViewingAngle)
            {
                case ViewAngle.XY:  // Slice N-S?
                    LayoutXY(Sides, min, max);
                    break;
                case ViewAngle.TopDown: // Top Down
                    LayoutTopdown(Sides, min, max);
                    break;
                case ViewAngle.YZ: // Slice E-W?
                    LayoutYZ(Sides, min, max);
                    break;
            }
            LayoutControls();
            foreach (KeyValuePair<Vector3i, MapChunkControl> chunk in Chunks)
            {
                chunk.Value.Refresh();
            }
            Refresh();
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
                        MapChunkControl mcc = new MapChunkControl(this, cc, _Map.ChunkScale);
                        mcc.SetBounds((int)(cc.Y * _Map.ChunkScale.Y)+6, (int)(cc.Z * _Map.ChunkScale.Z)+6, (int)(_Map.ChunkScale.Y), (int)(_Map.ChunkScale.Z));
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);
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
            for (int x = 0; x < Width/Sides.X; x++)
            {
                for (int y = 0; y <Height/Sides.Y; y++)
                {
                    Vector3i cc = new Vector3i(x + (min.X / _Map.ChunkScale.X),y + (CurrentPosition.Y / _Map.ChunkScale.Y), (min.Y / _Map.ChunkScale.Z));
                    if (!Chunks.ContainsKey(cc))
                    {
                        MapChunkControl mcc = new MapChunkControl(this, cc, _Map.ChunkScale);
                        mcc.SetBounds((int)(x * _Map.ChunkScale.X)*ZoomLevel+6, (int)(y * _Map.ChunkScale.Y)*ZoomLevel+6, (int)(_Map.ChunkScale.X)*ZoomLevel, (int)(_Map.ChunkScale.Y)*ZoomLevel);
                        //Console.WriteLine("Added chunk to {0},{1}", mcc.Top, mcc.Left);
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);
                    }
                    else
                    {
                        MapChunkControl mcc = Chunks[cc];
                        mcc.SetBounds((int)(x * _Map.ChunkScale.X) * ZoomLevel+6, (int)(y * _Map.ChunkScale.Y) * ZoomLevel+6, (int)(_Map.ChunkScale.X) * ZoomLevel, (int)(_Map.ChunkScale.Y) * ZoomLevel);
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);
                    }
                }
            }
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
                        MapChunkControl mcc = new MapChunkControl(this, cc, _Map.ChunkScale);
                        mcc.SetBounds((int)(cc.X * _Map.ChunkScale.X), (int)(cc.Y * _Map.ChunkScale.Y), (int)(_Map.ChunkScale.X), (int)(_Map.ChunkScale.Y));
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);
                    }
                    else
                    {
                        MapChunkControl mcc = Chunks[cc];
                        mcc.SetBounds((int)(cc.X * _Map.ChunkScale.X), (int)(cc.Y * _Map.ChunkScale.Y), (int)(_Map.ChunkScale.X), (int)(_Map.ChunkScale.Y));
                        Controls.Add(mcc);
                        Chunks.Add(cc, mcc);
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
                _CurrentPosition = value;
                //CurrentPosition.Z = CurrentPosition.Z % _Map.ChunkScale.Z;
                DoLayout();
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
                DoLayout();
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
                _ZoomLevel = Math.Min(256,value);
                _ZoomLevel = Math.Max(32, _ZoomLevel);
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

        }

    }
}
