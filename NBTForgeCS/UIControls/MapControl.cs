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
    public partial class MapControl: UserControl
    {
        public delegate void EntityClickHandler(Entity e);
        public delegate void TileEntityClickHandler(TileEntity e);

        public event EntityClickHandler EntityClicked;
        public event TileEntityClickHandler TileEntityClicked;

        private Vector3i _CurrentPosition = new Vector3i(0, 0, 127);
        private Vector3i _SelectedVoxel = new Vector3i(-1, -1, -1);
        //private bool Drawing = false;
        private IMapHandler _Map;
        private ViewAngle _ViewingAngle = ViewAngle.TopDown;
        private Dictionary<Vector3i, MapChunkControl> Chunks = new Dictionary<Vector3i, MapChunkControl>();
        private List<Button> EntityControls = new List<Button>();
        private int _ZoomLevel = 8;

        /// <summary>
        /// Currently active brush material.
        /// </summary>
        public byte CurrentMaterial = 0;

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
        delegate void LayoutDelegate();
        /// <summary>
        /// Perform map layout stuff.
        /// </summary>
        void DoLayout()
        {
            if (InvokeRequired)
            {
                Invoke(new LayoutDelegate(DoLayout));
                return;
            }
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
                case ViewAngle.FrontSlice:  // Slice N-S?
                    LayoutXY(Sides, min, max);
                    break;
                case ViewAngle.TopDown: // Top Down
                    LayoutTopdown(Sides, min, max);
                    break;
                case ViewAngle.SideSlice: // Slice E-W?
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
             [L]   [R] [Z ]
                [D]    [LD]
            */

            btnLyrDown.SetBounds(Width - 23, Height - 23, 22, 22);
            btnLyrDown.BringToFront();
            btnLyrUp.SetBounds(Width - 23, Height - 67, 22, 22);
            btnLyrUp.BringToFront();
            btnUp.SetBounds(Width - 73-22, Height - 67, 22, 22);
            btnUp.BringToFront();
            btnDown.SetBounds(Width - 73-22, Height - 23, 22, 22);
            btnDown.BringToFront();
            btnLeft.SetBounds(Width - 95-22, Height - 45, 22, 22);
            btnLeft.BringToFront();
            btnRight.SetBounds(Width - 51-22, Height - 45, 22, 22);
            btnRight.BringToFront();
            numZ.SetBounds(Width - 45, Height - 45, 44, 22);
            numZ.BringToFront();
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
                        mcc.MouseClick += new MouseEventHandler(ChunkRightClicked);
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
            /*
            foreach (KeyValuePair<Guid, Entity> k in _Map.Entities)
            {
                Entity e = k.Value;
                if (e.Pos.X > min.X && e.Pos.X < max.X && e.Pos.Y > min.Y && e.Pos.Y < max.Y)
                {
                    Button b = new Button();
                    float x = (float)e.Pos.X + (float)min.X;
                    float y = (float)e.Pos.Y + (float)min.Y;
                    //b.SetBounds((int)y * ZoomLevel - 2, (int)y * ZoomLevel - 2, 16, 16);
                    b.SetBounds((int)(e.Pos.X - min.X) * ZoomLevel, (int)(e.Pos.Y - min.Y) * ZoomLevel, 16, 16);
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
            }*/
        }

        void ChunkRightClicked(object sender, MouseEventArgs e)
        {
            MapChunkControl mcc = (MapChunkControl)sender;
            SelectedVoxel = new Vector3i(
                CurrentPosition.X + (e.X / _ZoomLevel) + (mcc.AssignedChunk.X * _Map.ChunkScale.X),
                CurrentPosition.Y + (e.Y / _ZoomLevel) + (mcc.AssignedChunk.Y * _Map.ChunkScale.Y),
                CurrentPosition.Z);
            if (e.Button == MouseButtons.Left)
            {
                Vector3i bp = new Vector3i(e.X/ZoomLevel,e.Y/ZoomLevel,CurrentPosition.Z);
                Chunk c = _Map.GetChunk(mcc.AssignedChunk.X,mcc.AssignedChunk.Y);
                if (c == null) return;
                c.Blocks[bp.X,bp.Y,bp.Z] =CurrentMaterial;
                c.Save();
            }
            if (e.Button == MouseButtons.Right)
            {
                Vector3i bp = new Vector3i(e.X/ZoomLevel,e.Y/ZoomLevel,CurrentPosition.Z);
                Chunk c = _Map.GetChunk(mcc.AssignedChunk);
                Block b = Blocks.Get(0);
                if (c != null)
                {
                    byte bid = c.GetBlock(bp);
                    b = Blocks.Get(bid);
                }
                ContextMenu cmnu = new System.Windows.Forms.ContextMenu();
                cmnu.MenuItems.AddRange(new MenuItem[]{
                    new MenuItem("What's this?",new EventHandler(delegate(object s,EventArgs ea){
                        MessageBox.Show("That is a(n) " + b.ToString() + " block.");
                    })),
                    new MenuItem("Remove this.",new EventHandler(delegate(object s,EventArgs ea){
                        c.SetBlock(bp,0x00);
                        mcc.Render();
                        mcc.Refresh();
                    })),
                    new MenuItem("-"),                    
                    new MenuItem("Replace..."),//,new EventHandler(delegate(object s,EventArgs ea){})),
                    new MenuItem("Paint..."),//,new EventHandler(delegate(object s,EventArgs ea){})),
                    new MenuItem("Generate...",new EventHandler(delegate(object s,EventArgs ea){
                        double min, max;
                        Map.Generate(mcc.AssignedChunk.X,mcc.AssignedChunk.Y, out min, out max);
                        Map.LoadChunk(mcc.AssignedChunk.X, mcc.AssignedChunk.Y);
                        mcc.Render();
                        mcc.Refresh();
                    })),
                    new MenuItem("-"),
                    new MenuItem("Delete Chunk...",new EventHandler(delegate(object s,EventArgs ea){
                        Map.LoadChunk(mcc.AssignedChunk.X, mcc.AssignedChunk.Y);
                        c.Delete();
                        mcc.Render();
                        mcc.Refresh();
                    })),
                    new MenuItem("Refresh",new EventHandler(delegate(object s,EventArgs ea){
                        mcc.Render();
                        mcc.Refresh();
                    })),
                    new MenuItem("Chunk Properties...",new EventHandler(delegate(object s,EventArgs ea){
                        dlgChunk chunkdlg = new dlgChunk(_Map, mcc.AssignedChunk);
                        chunkdlg.ShowDialog();
                    })),
                });
                cmnu.Show(mcc, new Point(0, 0));
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

        public void SelectEntity(Guid e)
        {
            if (EntityClicked != null)
                EntityClicked(_Map.Entities[e]);
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
                _CurrentPosition.Z = Utils.Clamp(_CurrentPosition.Z, 0, _Map.ChunkScale.Z-1);
                numZ.Value = _CurrentPosition.Z;
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
                //Chunks.Clear();
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
                //Chunks.Clear();
                _Map = value;
                CurrentPosition=(Vector3i)_Map.PlayerPos;
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

        private void numZ_ValueChanged(object sender, EventArgs e)
        {
            _CurrentPosition.Z = (int)numZ.Value;
            DoLayout();
            Refresh();
        }

        private void MapControl_Load(object sender, EventArgs e)
        {

        }


        internal void SelectTileEntity(Guid lol)
        {
            if (TileEntityClicked != null)
                TileEntityClicked(_Map.TileEntities[lol]);
        }
    }
}
