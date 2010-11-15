using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    /*
*** BUG: Unknown entity (ID: Item)
TAG_Compound: 11 entries
{
	TAG_String("id"): Item
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 28.7666982214875
		TAG_Double: 82.125
		TAG_Double: 4.44008329172237
	}
	TAG_Short("Age"): 2085
	TAG_Compound("Item"): 3 entries
	{
		TAG_Short("id"): 39
		TAG_Short("Damage"): 0
		TAG_Byte("Count"): 1
	}
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: -4.94065645841247E-324
		TAG_Double: 0
		TAG_Double: 4.94065645841247E-324
	}
	TAG_Byte("OnGround"): 1
	TAG_Short("Fire"): -1
	TAG_Short("Health"): 5
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 119.444
		TAG_Float: 0
	}
}*/
    public class Item:Entity
    {
        public short ItemID = 0;
        public short Damage = 0;
        public byte Count = 1;
        public short Health = 5; // WTF notch
        public short Age = 2085;
        private Block B;
        public Item()
        {
        }

        public Item(NbtCompound c)
        {
            SetBaseStuff(c);
            NbtCompound i = (c["Item"] as NbtCompound);
            ItemID = (i["id"] as NbtShort).Value;
            Damage = (i["Damage"] as NbtShort).Value;
            Count = (i["Count"] as NbtByte).Value;
            Health = (c["Health"] as NbtShort).Value;
            Age = (c["Age"] as NbtShort).Value;

            B = Blocks.Get(ItemID);
        }
        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c, GetID());
            NbtCompound i = new NbtCompound("Item");
            i.Tags.Add(new NbtShort("id", ItemID));
            i.Tags.Add(new NbtShort("Damage", Damage));
            i.Tags.Add(new NbtByte("Count", Count));
            c.Tags.Add(i);
            c.Tags.Add(new NbtShort("Health", Health));
            c.Tags.Add(new NbtShort("Age", Age));
            return c;
        }
        public override string ToString()
        {
            return string.Format("{0} {1}{2}",Count,B.Name,(Count>1) ? "s":"");
        }

        public override string GetID() { return "Item"; }

        public override Image Image
        {
            get
            {
                return B.Image;
            }
        }
    }
}
