using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LibNbt.Tags;

namespace OpenMinecraft.TileEntities
{
    public class MobSpawner:TileEntity
    {
    	private static System.Drawing.Image icon;
        public string EntityId = "Pig";
        public short Delay = 20;

        // Needed for autoload
        public MobSpawner() { }

        public MobSpawner(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            EntityId=(c["EntityId"] as NbtString).Value;
            Delay = (c["Delay"] as NbtShort).Value;
        }

        public MobSpawner(int X, int Y, int Z, string EntID, int delay)
        {
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

        public override Image Image 
        { 
        	get 
        	{
        		if(icon==null)
        			icon = Blocks.Get(52).Image;
        		return icon; 
        	} 
        }

        public override string ToString()
        {
            return EntityId+" Spawner";
        }

        public override string GetID()
        {
            return "MobSpawner";
        }
    }
}
