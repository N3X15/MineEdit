using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    /*
*** BUG: Unknown entity (ID: Pig)
TAG_Compound: 13 entries
{
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 0
		TAG_Double: -0.08
		TAG_Double: 0
	}
	TAG_Byte("OnGround"): 1
	TAG_Short("HurtTime"): 6
	TAG_Short("Health"): 2
	TAG_Short("Air"): 300
	TAG_String("id"): Pig
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 29.5500000119209
		TAG_Double: 10
		TAG_Double: -178.550000011921
	}
	TAG_Short("AttackTime"): 0
	TAG_Byte("Saddle"): 0
	TAG_Short("Fire"): -1
	TAG_Float("FallDistance"): 0
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: -115.9744
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}
     */
    public class Pig:LivingEntity
    {
		private static Image icon = new Bitmap("mobs/pig.png");
        //.Pig-only stuff
        public bool Saddle = false;

        public Pig()
        {
        }
        public Pig(NbtCompound c)
            :base(c)
        {
            Saddle = (c["Saddle"] as NbtByte).Value==1;
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
            c.Tags.Add(new NbtByte("Saddle", Saddle ? (byte)1:(byte)0));
            return c;
        }
        public override string ToString()
        {
            return "Pig";
        }

        public override string GetID()
        {
            return "Pig";
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
