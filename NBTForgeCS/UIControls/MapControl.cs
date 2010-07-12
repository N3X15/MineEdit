using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class MapControl: UserControl
    {
        public delegate void EntityClickHandler(Entity e);
        public delegate void TileEntityClickHandler(TileEntity e);

        public event EntityClickHandler EntityClicked;
        public event TileEntityClickHandler TileEntityClicked;

        private Vector3i _CurrentPosition = new Vector3i(0, 0, 64);
        private Vector3i _SelectedVoxel = new Vector3i(-1, -1, -1);
        private bool Drawing = false;
        private IMapHandler _Map;
        private ViewAngle _ViewingAngle = ViewAngle.TopDown;
        private Dictionary<Vector3i, MapChunkControl> Chunks = new Dictionary<Vector3i, MapChunkControl>();
        private List<Button> EntityControls = new List<Button>();
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

            //Console.WriteLine("DoLayout(): {0} - {1}", min, max);
            foreach (KeyValuePair<Vector3i, MapChunkControl> k in Chunks)
            {
                Controls.Remove(k.Value);
            }
            Chunks.Clear();

            foreach (Button b in EntityControls)
            {
                Controls.Remove(b);
            }
            EntityControls.Clear();

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
            Console.WriteLine("long y = {0}; y < {1}; y += {2}", min.Y / Sides.Y, max.Y / Sides.Y, _Map.ChunkScale.Y / Sides.Y);
            for (long y = min.Y / Sides.Y; y < max.Y / Sides.Y; y += _Map.ChunkScale.Y / Sides.Y)
            {
                for (long x = min.X / Sides.X; x < max.X / Sides.X; x += _Map.ChunkScale.X/Sides.X)
                {
                    Vector3i cc = new Vector3i(x+(min.X / _Map.ChunkScale.X), y + (min.Y / _Map.ChunkScale.Y), (CurrentPosition.Z / _Map.ChunkScale.X));
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
                        mcc.SendToBack();
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
            foreach (KeyValuePair<Guid, Entity> k in _Map.Entities)
            {
                Entity e = k.Value;
                if (e.Pos.X > min.X && e.Pos.X < max.X && e.Pos.Y > min.Y && e.Pos.Y < max.Y)
                {
                    Button b = new Button();
                    float x = (float)e.Pos.X + (float)min.X;
                    float y = (float)e.Pos.Y + (float)min.Y;
                    b.SetBounds((int)y * ZoomLevel - 2, (int)y * ZoomLevel - 2, 16, 16);
                    b.Image = e.Image;
                    b.UseVisualStyleBackColor = true;
                    b.BackColor = Color.Transparent;
                    b.FlatStyle = FlatStyle.Flat;
                    b.ImageAlign = ContentAlignment.MiddleCenter;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = Color.Black;
                    b.FlatAppearance.MouseOverBackColor = Color.Transparent;
                    b.Click += new EventHandler(OnEntityClicked);
                    b.Tag = e;
                    Controls.Add(b);
                    EntityControls.Add(b);
                    b.Show();
                    b.BringToFront();
                    Console.WriteLine("{0} {1} added to pos {2},{3}", e, e.UUID, b.Top, b.Left);
                }
            }
            foreach (KeyValuePair<Guid, TileEntity> k in _Map.TileEntities)
            {
                TileEntity e = k.Value;
                if (e.Pos.X > min.X && e.Pos.X < max.X && e.Pos.Y > min.Y && e.Pos.Y < max.Y)
                {
                    Button b = new Button();
                    b.SetBounds((int)(e.Pos.X - min.X) * ZoomLevel + 6, (int)(e.Pos.Y - min.Y) * ZoomLevel + 6, 16, 16);
                    b.Image = e.Image;
                    b.UseVisualStyleBackColor = true;
                    b.BackColor = Color.Transparent;
                    b.FlatStyle = FlatStyle.Flat;
                    b.ImageAlign = ContentAlignment.MiddleCenter;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = Color.Black;
                    b.FlatAppearance.MouseOverBackColor = Color.Transparent;
                    b.Click += new EventHandler(OnTileEntityClicked);
                    b.Tag = e;
                    Controls.Add(b);
                    EntityControls.Add(b);
                    b.Show();
                    b.BringToFront();
                    Console.WriteLine("{0} {1} added to pos {2},{3}", e, e.UUID, b.Top, b.Left);
                }
            }
        }

        void OnTileEntityClicked(object sender, EventArgs e)
        {
            if (TileEntityClicked != null)
                TileEntityClicked((TileEntity)(sender as Button).Tag);
        }

        void OnEntityClicked(object sender, EventArgs e)
        {
            if (EntityClicked != null)
                EntityClicked((Entity)(sender as Button).Tag);
        }

        // Entity button clicked.
        void b_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
