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

        public override string ToString()
        {
            return string.Format("[Chunk {0}]",Position);
        }
        /// <summary>
        /// Stores block data
        /// </summary>
        public byte[, ,] Data {get;set;}
        public void GetOverview(Vector3i pos, out int height, out int underwater_height, out byte block, out byte underwater_block, out int waterdepth)
        {
            height = underwater_height = 0;
            block = underwater_block = 0;
            waterdepth = 0;
            bool hf = false;
            int x = (int)pos.X;
            int y = (int)pos.Y;
            //int z = (int)pos.Z;// % ChunkZ;
            for (int z = (int)pos.Z; z > 0; --z)
            {
                byte b = Blocks[x, y, z];
                if (b == 8 || b == 9)
                {
                    if (!hf)
                    {
                        hf = true;
                        height = z;
                        block = b;
                    }
                    waterdepth++;
                    continue;
                }
                if (b != 0)
                {
                    if (!hf)
                    {
                        height = z;
                        hf = true;
                        block = b;
                    }
                    else
                    {
                        underwater_height = z;
                        underwater_block = b;
                    }
                    return;
                }
            }
        }

        public void UpdateOverview()
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    bool hit = false; ;
                    for (int z = (int)Size.Z-1; z > 0; z--)
                    {
                        byte b = Blocks[x,y,z];
                        if (b != 0)
                        {
                            if (!hit)
                            {
                                Overview[x, y] = b;
                                HeightMap[x, y] = z;

                                if (MaxHeight < z) MaxHeight = z;
                                if (MinHeight > z) MinHeight = z;
                            }

                            if (b != 9 && b != 8)
                                break;
                            WaterDepth[x, y]++;
                        }
                    }
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
        private bool mNeedsLighting;

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
            if (x < 0 || y < 0 || z < 0 || x >= Size.X || y >= Size.Y || z >= Size.Z)
            {
                return Map.GetBlockAt(x + (int)(Size.X * Position.X), y + (int)(Size.Y * Position.Y), Utils.Clamp(z,0,127));
                //return 0;
            }
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

        public void StripLighting()
        {
            BlockLight = SkyLight = new byte[_Map.ChunkScale.X, _Map.ChunkScale.Y, _Map.ChunkScale.Z];
        }

        // Shitty. Shitty. Shitty.

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

        #region Contributed by Moose
        //Converted to C#


       public void RecalculateLighting()
        {
	        byte blockType;

	        //if (!mNeedsLighting)
		    //    return;

	        long tickNow = DateTime.Now.Ticks;

            BlockLight=SkyLight = new byte[16, 16, 128];

	        // sky light
	        for (int x = 0; x < 16; x++)
	        {
		        for (int z = 0; z < 16; z++)
		        {
			        for (int y = 127; y >= 0; y--)
			        {

				        blockType = Blocks[x,y,z];

                        BlockLight[x, y, z] = 0;
                        SkyLight[x, y, z] = 15;

				        if (OpenMinecraft.Blocks.Get(blockType).Stop != 0)
					        break;
			        }
		        }
	        }

	        Console.WriteLine("Chunk lighting gen (sky) in {0} ms\n", DateTime.Now.Ticks - tickNow);

            tickNow = DateTime.Now.Ticks;

	        // light spread
	        for (int x = 0; x < 16; x++)
	        {
		        for (int y = 0; y < 16; y++)
		        {
			        for (int z = HeightMap[x,y] + 10; z >= 0; z--)
			        {
				        blockType = Blocks[x,y,z];

				        // stops light going down
				        if (OpenMinecraft.Blocks.Get(blockType).Stop == 16)
				        {
                            BlockLight[x, y, z] = 0;
					        break;
				        } else {
                            BlockLight[x,y,z] = (byte)(15 - OpenMinecraft.Blocks.Get(blockType).Stop);
                            LightMapStep(x, y, z, 15 - OpenMinecraft.Blocks.Get(blockType).Stop);
				        }
			        }
		        }
	        }

	        Console.WriteLine("Chunk lighting gen (spread) in {0} ms\n", DateTime.Now.Ticks - tickNow);

	        tickNow = DateTime.Now.Ticks;

	        // light emitters
	        for (int x = 0; x < 16; x++)
	        {
		        for (int y = 0; y < 16; y++)
		        {
			        for (int z = HeightMap[x,y] + 2; z >= 0; z--)
			        {
				        blockType = Blocks[x,y,z];
                        if (OpenMinecraft.Blocks.Get(blockType).Emit != 0)
				        {
                            BlockLightMapStep(x, y, z, OpenMinecraft.Blocks.Get(blockType).Emit);
				        }
			        }
		        }
	        }

	        mNeedsLighting = false;

	        Console.WriteLine("Chunk lighting gen (emit) in {0} ms\n", DateTime.Now.Ticks - tickNow);
        }


        void BlockLightMapStep(int x, int y, int z, int light)
        {
	        byte blockType, metaData, blockLight, skyLight;
	        int x_, y_, z_;

	        //DBG(L"BLMS: %i,%i,%i,%i,%p\n", x, y, z, light, chunk);

	        if (light < 1)
		        return;

	        for (byte i = 0; i < 6; i++)
	        {
		        if (y == 127 && i == 2)
			        i++;
		        if (y == 0 && i == 3)
			        i++;

		        x_ = x;
		        y_ = y;
		        z_ = z;

		        switch (i)
		        {
		        case 0: x_++; break;
		        case 1: x_--; break;
		        case 2: y_++; break;
		        case 3: y_--; break;
		        case 4: z_++; break;
		        case 5: z_--; break;
		        }

		        if (GetBlockData(x_, y_, z_, out blockType, out metaData, out blockLight, out skyLight))
		        {
                    if (blockLight < (light + OpenMinecraft.Blocks.Get(blockType).Stop - 1))
			        {
                        BlockLight[x_, y_, z_] = (byte)(light - OpenMinecraft.Blocks.Get(blockType).Stop - 1);
                        if (OpenMinecraft.Blocks.Get(blockType).Stop != 16)
				        {
                            BlockLightMapStep(x_, y_, z_, light - OpenMinecraft.Blocks.Get(blockType).Stop - 1);
				        }
			        }
		        }
	        }
        }

        void LightMapStep(int x, int y, int z, int light)
        {
	        byte blockType, metaData, blockLight, skyLight;
	        int x_, y_, z_;

	        if (light < 1)
	        {
		        return;
	        }

	        // loop 1 time for each direction except up (positive y)
	        for (byte i = 0; i < 5; i++)
	        {
		        if (y == 127 && i == 2)
			        i++;
		        if (y == 0 && i == 3)
			        i++;

		        x_ = x;
		        y_ = y;
		        z_ = z;

		        // light spread direction
		        switch (i)
		        {
		        case 0: x_++; break;
		        case 1: x_--; break;
		        case 2: y_--; break;
		        case 3: z_++; break;
		        case 4: z_--; break;
		        }

                
		        if (GetBlockData(x_, y_, z_, out blockType, out metaData, out blockLight, out skyLight))
		        {
                    if (skyLight < (light - OpenMinecraft.Blocks.Get(blockType).Stop - 1))
			        {
				        SkyLight[x_, y_, z_] =  (byte)(light - OpenMinecraft.Blocks.Get(blockType).Stop - 1);
                        if (OpenMinecraft.Blocks.Get(blockType).Stop != 16)	// stop if this block lets no light through
					        LightMapStep(x_, y_, z_, light - OpenMinecraft.Blocks.Get(blockType).Stop - 1);
			        }
		        }
	        }
        }

        private bool GetBlockData(int x, int y, int z, out byte blockType, out byte metaData, out byte blockLight, out byte skyLight)
        {
            blockType = Blocks[x,y,z];
            metaData = 0;
            blockLight = BlockLight[x, y, z];
            skyLight = SkyLight[x, y, z];
            return true;
        }
        #endregion
    }
}
