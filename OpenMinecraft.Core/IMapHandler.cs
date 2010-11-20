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
    public delegate void CorruptChunkHandler(long X, long Y, string error, string file);
    public delegate void ForEachProgressHandler(int Total, int Complete);
	public delegate void CachedChunkDelegate(long x, long y,Chunk c);
    public interface IMapHandler
    {
        event CorruptChunkHandler CorruptChunk;
        event ForEachProgressHandler ForEachProgress;

        Dictionary<Guid, Entity> Entities { get; }
        Dictionary<Guid, TileEntity> TileEntities { get; }

        void Load(string filename);
        bool Save();
        bool Save(string filename);

        int Height { get; }
        int Width { get; }
        int Depth { get; }

        Vector3i ChunkScale { get; }
        Vector3i MapMin { get; }
        Vector3i MapMax { get; }

        int Health {get;set;}
        int Air {get;set;}
        int Fire {get;set;}
        string Filename { get; set; }
        Vector3d PlayerPos {get;set;}
        Vector3i Spawn {get;set;}

        void ClearInventory();
        bool GetInventory(int slot, out short itemID, out short Damage, out byte Count, out string failreason);
        bool SetInventory(int slot, short itemID, int Damage, int Count);

        bool GetArmor(ArmorType slot, out short itemID, out short Damage, out byte Count, out string failreason);
        bool SetArmor(ArmorType slot, short itemID, int Damage, int Count);

        void Repair();

        bool IsMyFiletype(string FileName);

        Vector3i GetMousePos(Vector3i p, int scale, ViewAngle viewAngle);

        int InventoryCapacity { get; }
        int InventoryOnHandCapacity { get; }
        int InventoryColumns { get; }

        void GetOverview(int CX,int CY, Vector3i pos, out int h, out byte block, out int waterdepth);

        void Load();

        int Time { get; set; }

        bool HasMultipleChunks { get; }

        void LoadChunk(long X, long Y);

        void CullChunk(long X, long Y);

        void AddEntity(Entity e);
        void SetEntity(Entity e);
        void RemoveEntity(Entity e);

        void SetTileEntity(TileEntity e);
        void RemoveTileEntity(TileEntity e);

        int HurtTime { get; set; }

        void ForEachChunk(Chunk.ChunkModifierDelegate cmd);
        void ForEachCachedChunk(CachedChunkDelegate cmd);

        void BeginTransaction();
        void CommitTransaction();

        void ReplaceBlocksIn(long X, long Y, Dictionary<byte, byte> Replacements);

        Chunk GetChunk(Vector3i ChunkPos);
        Chunk GetChunk(long x, long y);
        void SetChunk(Chunk c);
        
        long RandomSeed { get; set; }

        void SaveChunk(Chunk chunk);
        void ChunkModified(long x, long y);

        Chunk NewChunk(long X, long Y);
        IMapGenerator Generator { get; set; }
        void Generate(IMapHandler mh, long X, long Y);

        Vector2i GetChunkCoordsFromFile(string filename);

        byte GetBlockAt(int x, int y, int z);
        void SetBlockAt(int x, int y, int z, byte val);

        int ExpandFluids(byte fluidID, bool CompleteRegen, ForEachProgressHandler ph);

        // Dimensions (Nether, etc)
        void SetDimension(int p);
        IEnumerable<Dimension> GetDimensions();

    }
}
