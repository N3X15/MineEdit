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
        public event CorruptChunkHandler CorruptChunk;
        public event ForEachProgressHandler ForEachProgress;

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
        public abstract void Generate(IMapHandler mh, long X, long Y);

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

        public abstract void SaveAll();

        public abstract bool Autorepair { get; set; }

        public abstract int ChunksLoaded { get; }

        public abstract void SetChunk(long X, long Y, Chunk c);

        public abstract void CullUnchanged();

        public abstract Vector3i Local2Global(int CX, int CZ, Vector3i vector3i);
        public abstract Vector3i Global2Local(Vector3i global, out int CX, out int CZ);
        public abstract Vector3d Local2Global(int CX, int CZ, Vector3d local);
        public abstract Vector3d Global2Local(Vector3d global, out int CX, out int CZ);

        public void Erode(double talus, int iterations, int X, int Z)
        {
            double h, h1, h2, h3, h4, d1, d2, d3, d4, max_d;
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

                        h = (double)GetHeightAt(abs_x, abs_z);
                        h1 = (double)GetHeightAt(abs_x, abs_z + 1);
                        h2 = (double)GetHeightAt(abs_x - 1, abs_z);
                        h3 = (double)GetHeightAt(abs_x + 1, abs_z);
                        h4 = (double)GetHeightAt(abs_x, abs_z - 1);
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
                        SetHeightAt(abs_x, abs_z, GetHeightAt(x, z) - (int)max_d, Generator.Materials.Soil);
                        SetHeightAt(abs_x + i, abs_z + j, GetHeightAt(abs_x + i, abs_z + j) + (int)max_d, Generator.Materials.Soil);
                    }
                }
            }
        }


        public void erodeThermal(double talus, int iterations, int X, int Z)
        {
            double h, h1, h2, h3, h4, d1, d2, d3, d4, max_d;
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

                        h = (double)GetHeightAt(abs_x, abs_z);
                        h1 = (double)GetHeightAt(abs_x, abs_z + 1);
                        h2 = (double)GetHeightAt(abs_x - 1, abs_z);
                        h3 = (double)GetHeightAt(abs_x + 1, abs_z);
                        h4 = (double)GetHeightAt(abs_x, abs_z - 1);
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
                        SetHeightAt(abs_x, abs_z, GetHeightAt(abs_x, abs_z) - (int)max_d, Generator.Materials.Soil);
                        SetHeightAt(abs_x + i, abs_z + j, GetHeightAt(abs_x + i, abs_z + j) + (int)max_d, Generator.Materials.Soil);
                    }
                }
            }
        }

    }
}
