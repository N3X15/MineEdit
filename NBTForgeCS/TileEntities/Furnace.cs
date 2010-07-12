using System;
using System.Collections.Generic;

using System.Text;
using LibNbt.Tags;

namespace MineEdit
{
    /*
    *** Unknown TileEntity: Furnace
    TAG_Compound: 7 entries
    {
	    TAG_List("Items"): 1 entries
	    {
		    TAG_Compound: 4 entries
		    {
			    TAG_Short("id"): 20
			    TAG_Short("Damage"): 0
			    TAG_Byte("Count"): 12
			    TAG_Byte("Slot"): 2
		    }
	    }
	    TAG_String("id"): Furnace
	    TAG_Short("BurnTime"): 0
	    TAG_Int("z"): -267
	    TAG_Short("CookTime"): 0
	    TAG_Int("y"): 73
	    TAG_Int("x"): 42
    }
     */
    public class Furnace:TileEntity
    {
        public InventoryItem[] Slots = new InventoryItem[3];
        public short BurnTime = 0;
        public short CookTime = 0;

        public Furnace() {}
        
        public Furnace(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            BurnTime = (c["BurnTime"] as NbtShort).Value;
            CookTime = (c["CookTime"] as NbtShort).Value;

            for (int i = 0; i < Slots.Length; i++)
            {
                    try
                    {
                        if ((c["Items"] as NbtList).Tags[i]!=null)
                        {
                                NbtCompound cc = (NbtCompound)(c["Items"] as NbtList).Tags[i];
                                Slots[i] = new InventoryItem(
                                    (cc["id"] as NbtShort).Value, (cc["Damage"] as NbtShort).Value, (cc["id"] as NbtByte).Value);

                        }
                    }
                    catch (Exception)
                    {
                    }
            }
        }
        public Furnace(NbtCompound c)
            :base(c)
        {
            BurnTime = (c["BurnTime"] as NbtShort).Value;
            CookTime = (c["CookTime"] as NbtShort).Value;

            for (int i = 0; i < 3; i++)
            {
                if ((c["Items"] as NbtList).Tags[i] != null)
                {
                    NbtCompound cc = (NbtCompound)(c["Items"] as NbtList).Tags[i];
                    Slots[i]=new InventoryItem(
                        (cc["id"] as NbtShort).Value,(cc["Damage"] as NbtShort).Value,(cc["id"] as NbtByte).Value);

                }
            }
        }

        public override System.Drawing.Image Image
        {
            get
            {
                return Blocks.Find("Furnace").Image;
            }
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c);
            c.Tags.Add(new NbtShort("BurnTime",BurnTime));
            c.Tags.Add(new NbtShort("CookTime",CookTime));
            NbtList Items = new NbtList("Items");
            for (int i = 0; i < 3; i++)
            {
                if (Slots[i]!=null)
                {
                    NbtCompound cc = new NbtCompound();

                    cc.Tags.Add(new NbtShort("id",Slots[i].MyType));
                    cc.Tags.Add(new NbtShort("Damage",(short)Slots[i].Damage));
                    cc.Tags.Add(new NbtByte("Count",(byte)Slots[i].Count));
                    cc.Tags.Add(new NbtByte("Slot", (byte)i));

                    Items.Tags.Add(cc);
                }
            }
            c.Tags.Add(Items);
            return c;
        }
    }
}
