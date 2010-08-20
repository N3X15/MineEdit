using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    /*
*** BUG: Unknown entity (ID: Sheep)
TAG_Compound: 13 entries
{
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 0
		TAG_Double: 0
		TAG_Double: 0
	}
	TAG_Byte("OnGround"): 0
	TAG_Short("HurtTime"): 0
	TAG_Short("Health"): 10
	TAG_Short("Air"): 300
	TAG_String("id"): Sheep
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 129.5
		TAG_Double: 82.6499999761581
		TAG_Double: 125.5
	}
	TAG_Short("AttackTime"): 0
	TAG_Short("Fire"): 0
	TAG_Byte("Sheared"): 0
	TAG_Float("FallDistance"): 0
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 43.19674
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}
     */
    public class Sheep:LivingEntity
    {
		private static Image icon = new Bitmap("mobs/sheep.png");
		
        public bool Sheared = false;

        public Sheep()
        {
        }
        public Sheep(NbtCompound c)
            :base(c)
        {
            Sheared = (c["Sheared"] as NbtByte).Value==1;
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
            c.Tags.Add(new NbtByte("Sheared", Sheared ? (byte)1:(byte)0));
            return c;
        }
        public override string ToString()
        {
            return "Sheep";
        }

        public override string GetID()
        {
            return "Sheep";
        }

        public override Image Image
        {
            get
            {
                return icon;
            }
        }
    }
}
