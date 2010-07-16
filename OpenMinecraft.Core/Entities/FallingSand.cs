using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class FallingSand:Entity
    {
        public byte Tile = 13;
        public byte OnGround = 0;

        public FallingSand()
        {
        }
        public FallingSand(NbtCompound c)
        {
            SetBaseStuff(c);
            Tile = (c["Tile"] as NbtByte).Value;
            OnGround = (c["OnGround"] as NbtByte).Value;
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c,GetID());
            c.Tags.Add(new NbtByte("Tile", Tile));
            c.Tags.Add(new NbtByte("OnGround", OnGround));
            return c;
        }
        public override string ToString()
        {
            return "Falling Sand";
        }

        public override string GetID()
        {
            return "FallingSand";
        }

        public override Image Image
        {
            get
            {
                return Blocks.Find("Sand").Image;
            }
        }
    }
}
