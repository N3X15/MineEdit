using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class Creeper:LivingEntity
    {
        /*
*** BUG: Unknown entity (ID: Creeper)
TAG_Compound: 12 entries
{
	TAG_String("id"): Creeper
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 132.5
		TAG_Double: 28.8999999761581
		TAG_Double: 126.5
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
		TAG_Float: 322.8646
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}*/

        public Creeper()
        {
        }
        public Creeper(NbtCompound c)
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
            return "Creeper";
        }

        public override string GetID()
        {
            return "Creeper";
        }

        public override Image Image
        {
            get
            {
                return MobIcons.mobcreeper;
            }
        }
    }
}
