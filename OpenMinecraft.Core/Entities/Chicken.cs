/*

TAG_Compound: 12 entries
{
	TAG_String("id"): Chicken
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 84.6725482556877
		TAG_Double: 68
		TAG_Double: 138.479193280034
	}
	TAG_Short("AttackTime"): 0
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 8.82782662812105E-07
		TAG_Double: -0.0784000015258789
		TAG_Double: -2.03381141496923E-06
	}
	TAG_Short("HurtTime"): 0
	TAG_Byte("OnGround"): 1
	TAG_Short("Fire"): -1
	TAG_Short("Health"): 4
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: -246.3079
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}

*/

using System.Drawing;
using LibNbt.Tags;

namespace OpenMinecraft.Entities
{
    public class Chicken : LivingEntity
    {

        private static Image icon = new Bitmap("mobs/notch.png");
        public Chicken()
        {
        }
        public Chicken(NbtCompound c)
            : base(c)
        {

        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();

            return c;
        }
        public override string ToString()
        {
            return "Chicken";
        }

        public override string GetID()
        {
            return "Chicken";
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
