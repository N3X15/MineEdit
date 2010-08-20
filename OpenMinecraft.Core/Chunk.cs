using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;

namespace OpenMinecraft
{
    public class Chunk
    {
        public delegate void ChunkModifierDelegate(long x, long y);
        public event ChunkModifierDelegate ChunkModified;
        public Dictionary<Guid, Entity> Entities = new Dictionary<Guid, Entity>();
        public Dictionary<Guid, TileEntity> TileEntities = new Dictionary<Guid, TileEntity>();
        public IChunkRenderer Renderer;

        /// <summary>
        /// Global position of chunk
        /// </summary>
        public Vector3i Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                Changed();
            }
        }
        protected Vector3i _Position;

        /// <summary>
        /// Scale of chunk (L, W, H)
        /// </summary>
        public Vector3i Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                Changed();
            }
        }
        protected Vector3i _Size;

        /// <summary>
        /// Chunk file location
        /// </summary>
        public string Filename
        {
            get { return _Filename; }
            set
            {
                _Filename = value;
                Changed();
            }
        }
        protected string _Filename;
        /// <summary>
        /// Creator of the chunk
        /// </summary>
        public string Creator
        {
            get { return _Creator; }
            set
            {
                _Creator = value;
                Changed();
            }
        }
        protected string _Creator;
        /// <summary>
        /// Time of creation.
        /// </summary>
        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set
            {
                _CreationDate = value;
                Changed();
            }
        }
        protected DateTime _CreationDate;

        /// <summary>
        /// Maximum height on the map
        /// </summary>
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        /// <summary>
        /// Stores block positions.
        /// </summary>
        public byte[, ,] Blocks
        {
            get { return _Blocks; }
            set
            {
                _Blocks = value;
                UpdateOverview();
                Changed();
            }
        }
        /// <summary>
        /// Stores block data
        /// </summary>
        public byte[, ,] Data {get;set;}
        public void GetOverview(Vector3i pos, out int h, out byte block, out int waterdepth)
        {
            h = 0;
            block = 0;
            waterdepth = 0;

            int x = (int)pos.X;
            int y = (int)pos.Y;
            //int z = (int)pos.Z;// % ChunkZ;
            for (int z = (int)pos.Z; z > 0; --z)
            {
                byte b = Blocks[x, y, z];
                if (b == 8 || b == 9)
                {
                    waterdepth++;
                    continue;
                }
                if (b != 0)
                {
                    block = b;
                    h = z;
                    return;
                }
            }
        }

        internal void UpdateOverview()
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    int h;
                    byte b;
                    int w;
                    GetOverview(new Vector3i(x, y, Size.Z - 1), out h, out b, out w);
                    Overview[x, y] = b;
                    WaterDepth[x, y] = w;
                    HeightMap[x, y] = h; 

                    if (MaxHeight < h) MaxHeight = h;
                    if (MinHeight > h) MinHeight = h;
                }
            }
        }
        protected byte[, ,] _Blocks;

        /// <summary>
        /// Block-based lighting (from torches, lava...)
        /// </summary>
        public byte[, ,] BlockLight { get; set; }
        /// <summary>
        /// Lighting from the environment (Sun)
        /// </summary>
        public byte[, ,] SkyLight { get; set; }
        /// <summary>
        /// 2D heightmap
        /// </summary>
        public int[,] HeightMap { get; protected set; }
        /// <summary>
        /// Overview (without water)
        /// </summary>
        public byte[,] Overview { get; protected set; }
        /// <summary>
        /// Overview of water depth
        /// </summary>
        public int[,] WaterDepth { get; protected set; }

        public IMapHandler Map
        {
            get { return _Map; }
            set
            {
                _Map = value;
                ChunkModified += _Map.ChunkModified;
            }
        }
        protected IMapHandler _Map;
        public bool Loading;

        public Chunk(IMapHandler mh) 
        {
            Map = mh;
            Overview = new byte[mh.ChunkScale.X, mh.ChunkScale.Y];
            HeightMap = new int[mh.ChunkScale.X, mh.ChunkScale.Y];
            WaterDepth = new int[mh.ChunkScale.X, mh.ChunkScale.Y];
        }

        public void Delete()
        {
            File.Delete(Filename);
        }

        public void Save()
        {
            Map.SaveChunk(this);
        }

        public double GetSkyLighting(int x, int y, int z)
        {
            return SkyLight[x,y,z];
        }

        public double GetBlockLighting(int x, int y, int z)
        {
            return BlockLight[x,y,z];
        }
        public double GetLight(int x,int y,int z)
        {
            double b = GetSkyLighting(x, y, z);//*sky;
            double s = GetBlockLighting(x, y, z);// *block;
            //if(!show_water) // What the hell
            //    s = 15*sky;
            if(s > b)
                return s+1;
            else
                return b+1;
        }

        public byte GetBlock(int x, int y, int z)
        {
            return Blocks[x, y, z];
        }
        
        public Block GetBlockType(int x, int y, int z)
        {
        	return OpenMinecraft.Blocks.Get(GetBlock(x,y,z));
        }

        private void Changed()
        {
            if (Loading) return;
            if (ChunkModified != null)
                ChunkModified(Position.X / Size.X, Position.Y / Size.Y);
        }

        public byte GetBlock(Vector3i bp)
        {
            return Blocks[bp.X, bp.Y, bp.Z];
        }

        public void SetBlock(Vector3i bp, byte p)
        {
            Blocks[bp.X, bp.Y, bp.Z] = p;
        }

        public void RecalculateLighting(bool Sky)
        {
            if(!Sky)
                BlockLight = RecalcLighting(this, false);
            else
                SkyLight = RecalcLighting(this, true);
        }

        // Shitty. Shitty. Shitty.
        public static byte[, ,] RecalcLighting(Chunk c, bool Sky)
        {
            byte[, ,] blocklight = c.BlockLight;
            for (int i = 0; i < 15; i++)
            {
                for (int x = 0; x < c.Size.X; x++)
                {
                    for (int y = 0; y < c.Size.Y; y++)
                    {
                        for (int z = 0; z < c.Size.Z; z++)
                        {
                            byte me = blocklight[x, y, z];
                            // SUNLIGHT
                            if (Sky)
                            {
                                if (c.HeightMap[x, y] <= z)
                                    me = (byte)(15 - c.WaterDepth[x,y]);
                            }
                            else
                            {
                                if (IsLightSource(c.Blocks[x, y, z]))
                                    me = GetLightLevel(c.Blocks[x, y, z]);
                            }
                            // Get brightest neighbor
                            if (x < c.Size.X - 1 && me < blocklight[x + 1, y, z])
                                me = blocklight[x + 1, y, z];
                            if (y < c.Size.Y - 1 && me < blocklight[x, y + 1, z])
                                me = blocklight[x, y + 1, z];
                            if (z < c.Size.Z - 1 && me < blocklight[x, y, z + 1])
                                me = blocklight[x, y, z + 1];
                            if (x > 0 && me < blocklight[x - 1, y, z])
                                me = blocklight[x - 1, y, z];
                            if (y > 0 && me < blocklight[x, y - 1, z])
                                me = blocklight[x, y - 1, z];
                            if (z > 0 && me < blocklight[x, y, z - 1])
                                me = blocklight[x, y, z - 1];
                            // Drop 1 level of light ( WHAT ABOUT SQUARE LAW :| )
                            me--;
                            if (me < 0) me = 0;
                            if (me > 15) me = 15;
                            blocklight[x, y, z] = me;
                        }
                    }
                }
            }
            return blocklight;
        }

        private static byte GetLightLevel(byte p)
        {
            if (p == 10 || p == 11)
                return 11;
            if (p == 50 || p == 51)
                return 10;
            if (p == 76)
                return 5;
            return 0;
        }

        private static bool IsLightSource(byte p)
        {
            return (p == 10 || p == 11 || p == 50 || p == 51 || p == 76);
        }
        //
    }
}
