using System;
using System.Collections.Generic;

using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;

namespace OpenMinecraft
{
    public enum ArmorType
    {
        Helm,
        Torso,
        Legs,
        Boots
    }
    public delegate void ChunkIteratorDelegate(IMapHandler me, long X, long Y);
    public delegate void CorruptChunkHandler(long X, long Y, string error, string file);
    public delegate void ForEachProgressHandler(int Total, int Complete);
	public delegate void CachedChunkDelegate(long x, long y,Chunk c);
    public abstract class IMapHandler
    {
        public abstract event CorruptChunkHandler CorruptChunk;
        public abstract event ForEachProgressHandler ForEachProgress;

        public abstract Dictionary<Guid, Entity> Entities { get; }
        public abstract Dictionary<Guid, TileEntity> TileEntities { get; }

        public abstract void Load(string filename);
        public abstract bool Save();
        public abstract bool Save(string filename);

        public abstract int Height { get; }
        public abstract int Width { get; }
        public abstract int Depth { get; }

        public abstract Vector3i ChunkScale { get; }
        public abstract Vector3i MapMin { get; }
        public abstract Vector3i MapMax { get; }

        public abstract int Health { get; set; }
        public abstract int Air { get; set; }
        public abstract int Fire { get; set; }
        public abstract string Filename { get; set; }
        public abstract Vector3d PlayerPos { get; set; }
        public abstract Vector3i Spawn { get; set; }

        public abstract void ClearInventory();
        public abstract bool GetInventory(int slot, out short itemID, out short Damage, out byte Count, out string failreason);
        public abstract bool SetInventory(int slot, short itemID, int Damage, int Count);

        public abstract bool GetArmor(ArmorType slot, out short itemID, out short Damage, out byte Count, out string failreason);
        public abstract bool SetArmor(ArmorType slot, short itemID, int Damage, int Count);

        public abstract void Repair();

        public abstract bool IsMyFiletype(string FileName);

        public abstract Vector3i GetMousePos(Vector3i p, int scale, ViewAngle viewAngle);

        public abstract int InventoryCapacity { get; }
        public abstract int InventoryOnHandCapacity { get; }
        public abstract int InventoryColumns { get; }

        public abstract void GetOverview(int CX, int CY, Vector3i pos, out int h, out byte block, out int waterdepth);

        public abstract void Load();

        public abstract int Time { get; set; }

        public abstract bool HasMultipleChunks { get; }

        public abstract void LoadChunk(long X, long Y);
        public abstract void CullChunk(long X, long Y);

        public abstract void AddEntity(Entity e);
        public abstract void SetEntity(Entity e);
        public abstract void RemoveEntity(Entity e);

        public abstract void SetTileEntity(TileEntity e);
        public abstract void RemoveTileEntity(TileEntity e);

        public abstract int HurtTime { get; set; }

        public abstract void ForEachChunk(ChunkIteratorDelegate cmd);
        public abstract void ForEachCachedChunk(CachedChunkDelegate cmd);

        public abstract void BeginTransaction();
        public abstract void CommitTransaction();

        public abstract void ReplaceBlocksIn(long X, long Y, Dictionary<byte, byte> Replacements);

        public abstract Chunk GetChunk(Vector3i ChunkPos);
        public abstract Chunk GetChunk(long x, long y);
        public abstract void SetChunk(Chunk c);

        public abstract long RandomSeed { get; set; }

        public abstract void SaveChunk(Chunk chunk);
        public abstract void ChunkModified(long x, long y);

        public abstract Chunk NewChunk(long X, long Y);
        public abstract IMapGenerator Generator { get; set; }
        public abstract void Generate(IMapHandler mh, long X, long Y, out double min, out double max);
        public abstract void FinalizeGeneration(IMapHandler mh, long X, long Z);

        public abstract Vector2i GetChunkCoordsFromFile(string filename);

        public abstract byte GetBlockAt(int x, int y, int z);
        public abstract void SetBlockAt(int x, int y, int z, byte val);

        public abstract int ExpandFluids(byte fluidID, bool CompleteRegen, ForEachProgressHandler ph);

        // Dimensions (Nether, etc)
        public abstract void SetDimension(int p);
        public abstract IEnumerable<Dimension> GetDimensions();

        public abstract bool RegenerateLighting(long X, long Z);
        public abstract void GetLightAt(int _X, int _Y, int _Z, out byte skyLight, out byte blockLight);
        public abstract void SetSkyLightAt(int px, int py, int z, byte val);
        public abstract void SetBlockLightAt(int px, int py, int z, byte val);
        public abstract byte GetSkyLightAt(int px, int py, int z);
        public abstract byte GetBlockLightAt(int px, int py, int z);

        public abstract int GetHeightAt(int x, int z);
        public abstract void SetHeightAt(int x, int z, int h, byte mat);

        internal abstract double GetPrelimHeightAt(int x, int z);
        internal abstract void SetPrelimHeightAt(int x, int z, double h);

        public abstract void SaveAll();

        public abstract bool Autorepair { get; set; }

        public abstract int ChunksLoaded { get; }

        public abstract void SetChunk(long X, long Y, Chunk c);

        public abstract void CullUnchanged();

        public abstract Vector3i Local2Global(int CX, int CZ, Vector3i vector3i);
        public abstract Vector3i Global2Local(Vector3i global, out int CX, out int CZ);
        public abstract Vector3d Local2Global(int CX, int CZ, Vector3d local);
        public abstract Vector3d Global2Local(Vector3d global, out int CX, out int CZ);


        /// <summary>
        /// Convolution matrices!
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="divisor"></param>
        /// <param name="offset"></param>
        /// <param name="X"></param>
        /// <param name="Z"></param>
        public void Convolution(double[][] filter, double divisor, double offset, int X, int Z)
        {
            int x_o = X * (int)ChunkScale.X;
            int z_o = Z * (int)ChunkScale.Z;
            int radius = (filter.GetLength(0) - 1) / 2;
            for (int z = 0; z < ChunkScale.Z; z++)
            {
                for (int x = 0; x < ChunkScale.X; x++)
                {
                    double value = 0;
                    for (int i = -radius; i <= radius; i++)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            value += filter[i + radius][j + radius] * GetPrelimHeightAt(x + x_o + i, z + z_o + j);
                        }
                    }
                    value = value / divisor + offset;
                    SetPrelimHeightAt(x + x_o, z + z_o, (int)value);
                }
            }
        }

        /// <summary>
        /// Smooths below-water surfaces and, optionally, adds beaches.
        /// </summary>
        /// <param name="waterlevel">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="beaches">
        /// A <see cref="System.Boolean"/>
        /// </param>
        /// <returns>
        /// A <see cref="Channel"/>
        /// </returns>
        public void Silt(double waterlevel, bool beaches, int X, int Z)
        {
            // 1. Copy Image
            // 2. Apply gauss blur to lower layer
            // 3. Bring back unblurred terrain from above the water level.

            int x_o = X * (int)ChunkScale.X;
            int z_o = Z * (int)ChunkScale.Z;

            double wl = (beaches) ? waterlevel + 6d : waterlevel;
            wl = wl / 256f;

            // Gaussian blur for silt and beaches.	
            double[][] gaussian_matrix = new double[3][]{
				new double[3]{1,2,1},
				new double[3]{2,4,2},
				new double[3]{1,2,1}
			};
            IMapHandler Blurred = this;
            Blurred.Convolution(gaussian_matrix, 32f, waterlevel / 2d, X, Z);

            for (int x = 0; x < ChunkScale.X; x++)
            {
                for (int z = 0; z < ChunkScale.Z; z++)
                {
                    // If > WL: use unblurred image.
                    // If <=WL: Use blurred image.
                    if (GetPrelimHeightAt(x+x_o, z+z_o) <= wl)
                        SetPrelimHeightAt(x + x_o, z + z_o, Blurred.GetPrelimHeightAt(x+x_o, z+z_o));
                }
            }
        }

        public void Erode(double talus, int iterations, int X, int Z)
        {
            double h, h1, h2, h3, h4, d1, d2, d3, d4, max_d, max_h=0d;
            int i, j;
            int x_o = X * (int)ChunkScale.X;
            int z_o = Z * (int)ChunkScale.Z;
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int z = 1; z < ChunkScale.Z - 1; z++)
                {
                    for (int x = 1; x < ChunkScale.X - 1; x++)
                    {
                        int abs_x = x + x_o;
                        int abs_z = z + z_o;

                        h = (double)GetPrelimHeightAt(abs_x, abs_z);
                        if (max_h < h) max_h = h;
                        h1 = (double)GetPrelimHeightAt(abs_x, abs_z + 1);
                        h2 = (double)GetPrelimHeightAt(abs_x - 1, abs_z);
                        h3 = (double)GetPrelimHeightAt(abs_x + 1, abs_z);
                        h4 = (double)GetPrelimHeightAt(abs_x, abs_z - 1);
                        d1 = h - h1;
                        d2 = h - h2;
                        d3 = h - h3;
                        d4 = h - h4;
                        i = 0;
                        j = 0;
                        max_d = 0f;
                        if (d1 > max_d)
                        {
                            max_d = d1;
                            j = 1;
                        }
                        if (d2 > max_d)
                        {
                            max_d = d2;
                            i = -1;
                            j = 0;
                        }
                        if (d3 > max_d)
                        {
                            max_d = d3;
                            i = 1;
                            j = 0;
                        }
                        if (d4 > max_d)
                        {
                            max_d = d4;
                            i = 0;
                            j = -1;
                        }
                        if (max_d > talus)
                        {
                            continue;
                        }
                        max_d *= 0.5f;
                        SetPrelimHeightAt(abs_x, abs_z, GetPrelimHeightAt(x, z) - (int)max_d);
                        SetPrelimHeightAt(abs_x + i, abs_z + j, GetPrelimHeightAt(abs_x + i, abs_z + j) + (int)max_d);
                    }
                }
            }
            //Console.WriteLine("Erode(): max_h={0}m", max_h);
        }


        public void ErodeThermal(double talus, int iterations, int X, int Z)
        {
            double h, h1, h2, h3, h4, d1, d2, d3, d4, max_d, max_h=0;
            int i, j;
            int x_o = X * (int)ChunkScale.X;
            int z_o = Z * (int)ChunkScale.Z;
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int z = 1; z < ChunkScale.Z - 1; z++)
                {
                    for (int x = 1; x < ChunkScale.X - 1; x++)
                    {
                        int abs_x = x + x_o;
                        int abs_z = z + z_o;

                        h = (double)GetPrelimHeightAt(abs_x, abs_z);
                        if (max_h < h) max_h = h;
                        h1 = (double)GetPrelimHeightAt(abs_x, abs_z + 1);
                        h2 = (double)GetPrelimHeightAt(abs_x - 1, abs_z);
                        h3 = (double)GetPrelimHeightAt(abs_x + 1, abs_z);
                        h4 = (double)GetPrelimHeightAt(abs_x, abs_z - 1);
                        d1 = h - h1;
                        d2 = h - h2;
                        d3 = h - h3;
                        d4 = h - h4;
                        i = 0;
                        j = 0;
                        max_d = 0f;
                        if (d1 > max_d)
                        {
                            max_d = d1;
                            j = 1;
                        }
                        if (d2 > max_d)
                        {
                            max_d = d2;
                            i = -1;
                            j = 0;
                        }
                        if (d3 > max_d)
                        {
                            max_d = d3;
                            i = 1;
                            j = 0;
                        }
                        if (d4 > max_d)
                        {
                            max_d = d4;
                            i = 0;
                            j = -1;
                        }
                        if (max_d < talus)
                        {
                            continue;
                        }
                        max_d *= 0.5f;
                        SetPrelimHeightAt(abs_x, abs_z, GetPrelimHeightAt(abs_x, abs_z) - (int)max_d);
                        SetPrelimHeightAt(abs_x + i, abs_z + j, GetPrelimHeightAt(abs_x + i, abs_z + j) + (int)max_d);
                    }
                }
            }
            //Console.WriteLine("ErodeThermal(): max_h={0}m", max_h);
        }


        public void GrowTree(Random rand, int x, int y, int z)
        {
            /*
            SetBlockAt(x, y, z, 6);
            SetDataAt(x, y, z, 15);
            */
            int height = rand.Next(6, 8);
            #region Foliage
            
            // Note, foliage will disintegrate if there is no foliage below, or
	        // if there is no "log" block within range 2 (square) at the same level or
	        // one level below
            
            int astart = y - 2;
            int aend = y + 2;
            int rad;
            for (int block_y = astart; block_y < aend; block_y++)
            {
                if (block_y > astart + 1)
                    rad = 1;
                else
                    rad = 2;
                for (int xoff = -rad; xoff < rad + 1; xoff++)
                {
                    for (int yoff = -rad; yoff < rad + 1; yoff++)
                    {
                        for (int zoff = -rad; zoff < rad + 1; zoff++)
                        {
                            if (Math.Abs(xoff) == Math.Abs(zoff) ||
                               Math.Abs(xoff) <= rad ||
                               Math.Abs(zoff) <= rad
                            )
                            {
                                SetBlockAt(x + xoff, block_y, z + zoff, 18);
                            }
                        }
                    }
                }
            }
            #endregion


            for (int i = 0; i < height; i++)
                SetBlockAt(x, y + i, z, 17);
        }



        internal void HeightmapToVoxelspace(double[,] hm, ref byte[, ,] b)
        {
            for (int x = 0; x < b.GetLength(0); ++x)
            {
                for (int y = 0; y < b.GetLength(1); ++y)
                {
                    for (int z = 0; z < b.GetLength(2); ++z)
                    {
                        byte block = (hm[x, z] * b.GetLength(1)-3 >= y) ? (byte)1 : (byte)0;
                        if (y == 0)
                            block = 7; // Adminium
                        else if (y == 1)
                        {
                            // TODO Yell at Notch for not making Lava occlude. :|
                            if (block == 0)
                                block = 11; // Lava for air.
                            else if (b[x, y, z] == 9 || b[x, y, z] == 8 )
                                block = 49; // Obsidian for underwater shit.
                            break;
                        }
                        else if (y >= b.GetLength(1) - 3)
                        {
                            block = 0;
                        }
                        b[x, y, z] = block;
                    }
                }
            }
        }

        public abstract void SetDataAt(int x, int y, int z, byte p);
        public abstract byte GetDataAt(int x, int y, int z);

        public abstract void SetChunkHeightmap(int X, int Z, double[,] cnk);
        public abstract double[,] GetChunkHeightmap(int X, int Z);

        public void SetBlockAt(long x, long y, long z, byte b)
        {
            SetBlockAt((int)x, (int)y, (int)z, b);
        }

        /// <summary>
        /// travel from cord along vec and return how far it was to a point of matidx
        ///
        ///the distance is returned in number of iterations.  If the edge of the map
        ///is reached, then return the number of iterations as well.
        ///if invert == True, search for anything other than matidx
        /// </summary>
        /// <param name="cord"></param>
        /// <param name="vec"></param>
        /// <param name="matidx"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public double DistanceToMaterial(Vector3i cord, Vector3i vec, byte matidx, bool invert=false)
        {
            Vector3i curcord = cord + .5;
            int iterations = 0;
            while(
                curcord.X>0 && curcord.Y>0 && curcord.Z > 0 &&
                curcord.X<ChunkScale.X && curcord.Y<ChunkScale.Y && curcord.Z < ChunkScale.Z)
            {
                byte mat = GetBlockAt(curcord.X,curcord.Y,curcord.Z);
                if(mat == matidx && invert == false) break;
                else if (mat != matidx && invert) break;
                else
                {
                    curcord = curcord + vec;
                    iterations ++;
                }
            }
            return iterations;
        }

        public byte GetBlockAt(long x,long y,long z)
        {
 	        return GetBlockAt((int)x,(int)y,(int)z);
        }
    }
}
