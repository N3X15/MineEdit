using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;

namespace OpenMinecraft.TileEntities
{
    public class Chest:TileEntity
    {
        public InventoryCollection Inventory = new InventoryCollection();

        public Chest(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            foreach (NbtCompound item in (c["Items"] as NbtList).Tags)
            {
                //NbtCompound item = (NbtCompound)itm;
                Inventory.Add(item);
            }
        }
        public Chest(LibNbt.Tags.NbtCompound c)
            : base(c)
        {
            for(int i =0;i<54;i++)
            {
                try
                {
                    NbtCompound item = (NbtCompound)(c["Items"] as NbtList)[i];
                    Inventory.Add(item);
                }
                catch (Exception)
                {
                }
            }
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c);
            c.Tags.Add(Inventory.ToNBT());
            return c;
        }

        public override System.Drawing.Image Image { get { return Icons.TileEntities.Chest; } }

        public override string ToString()
        {
            return "Chest ("+Inventory.Count.ToString()+" items)";
        }
    }
}
