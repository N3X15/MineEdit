using System;
using System.Collections.Generic;

using System.Text;
using LibNbt.Tags;

namespace MineEdit
{
    public class MobSpawner:TileEntity
    {
        public string EntityId = "Pig";
        public short Delay = 20;
        public MobSpawner(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            EntityId=(c["EntityId"] as NbtString).Value;
            Delay = (c["Delay"] as NbtShort).Value;
        }

        public MobSpawner(int X, int Y, int Z, string EntID, int delay)
        {
            // TODO: Complete member initialization
            this.Pos = new Vector3i(X, Y, Z);
            this.EntityId = EntID;
            this.Delay = (short)delay;
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c);
            c.Tags.Add(new NbtString("EntityId", EntityId));
            c.Tags.Add(new NbtShort("Delay", Delay));
            return c;
        }

        public override System.Drawing.Image Image
        {
            get
            {
                return Blocks.Find("Mob spawner").Image;
            }
        }

        public override string ToString()
        {
            return EntityId+" Spawner";
        }
    }
}
