using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineEdit
{
    public enum ArmorType
    {
        Helm,
        Torso,
        Legs,
        Boots
    }
    public interface IMapHandler
    {
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
        
        byte GetBlockAt(Vector3i p);
        void SetBlockAt(Vector3i p, byte id);

        void ClearInventory();
        bool GetInventory(int slot, out short itemID, out int Damage, out int Count, out string failreason);
        bool SetInventory(int slot, short itemID, int Damage, int Count);

        bool GetArmor(ArmorType slot, out short itemID, out int Damage, out int Count, out string failreason);
        bool SetArmor(ArmorType slot, short itemID, int Damage, int Count);

        void Repair();

        bool IsMyFiletype(string FileName);

        Vector3i GetMousePos(Vector3i p, int scale, ViewAngle viewAngle);

        int InventoryCapacity { get; }
        int InventoryOnHandCapacity { get; }
        int InventoryColumns { get; }

        void GetOverview(Vector3i pos, out int h, out byte block, out int waterdepth);

        void Load();

        byte GetBlockIn(long x, long y, Vector3i blockpos);
        void SetBlockIn(long x, long y, Vector3i blockpos,byte type);

        int Time { get; set; }
    }
}
