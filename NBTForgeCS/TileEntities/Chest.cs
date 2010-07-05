using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;

namespace MineEdit
{
    public class Chest:TileEntity
    {
        public Dictionary<byte,InventoryItem> Inventory = new Dictionary<byte,InventoryItem>();

        public Chest(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            foreach (NbtTag itm in (c["Items"] as NbtList).Tags)
            {
                NbtCompound item = (NbtCompound)itm;
                InventoryItem inv = new InventoryItem((item["id"] as NbtShort).Value, (item["Damage"] as NbtShort).Value, (item["Count"] as NbtByte).Value);
                Inventory.Add((item["Slot"] as NbtByte).Value, inv);
            }
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c);
            NbtList Items = new NbtList("items");
            foreach (KeyValuePair<byte, InventoryItem> p in Inventory)
            {
                NbtCompound nc = new NbtCompound();
                nc.Tags.AddRange(new NbtTag[]{
                    new NbtByte("Count",(byte)p.Value.Count),
                    new NbtByte("Slot",p.Key),
                    new NbtShort("Type",p.Value.MyType),
                    new NbtShort("Damage",(short)p.Value.Damage)
                });
                Items.Tags.Add(nc);
            }
            c.Tags.Add(Items);
            return c;
        }
    }
}
