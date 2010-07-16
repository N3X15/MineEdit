using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class Skeleton:LivingEntity
    {
        /*
*** BUG: Unknown entity (ID: Skeleton)
TAG_Compound: 12 entries
{
	TAG_String("id"): Skeleton
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: -84.5
		TAG_Double: 15.8999999761581
		TAG_Double: -166.5
	}
	TAG_Short("AttackTime"): 0
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 0
		TAG_Double: 0
		TAG_Double: 0
	}
	TAG_Short("HurtTime"): 0
	TAG_Byte("OnGround"): 0
	TAG_Short("Fire"): 0
	TAG_Short("Health"): 20
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 196.486
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}
         */

        public Skeleton()
        {
        }
        public Skeleton(NbtCompound c)
            :base(c)
        {
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
            return c;
        }
        public override string ToString()
        {
            return "Skeleton";
        }

        public override string GetID()
        {
            return "Skeleton";
        }

        public override Image Image
        {
            get
            {
                return MobIcons.mobskeleton;
            }
        }
    }
}
