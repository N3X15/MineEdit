/*

TAG_Compound: 12 entries
{
	TAG_String("id"): Cow
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 100.264384874142
		TAG_Double: 71
		TAG_Double: 96.9944802007176
	}
	TAG_Short("AttackTime"): 0
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: -1.92082406241897E-10
		TAG_Double: -0.0784000015258789
		TAG_Double: 1.69633677193254E-10
	}
	TAG_Short("HurtTime"): 0
	TAG_Byte("OnGround"): 1
	TAG_Short("Fire"): -1
	TAG_Short("Health"): 10
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 417.2553
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}

*/

using System.Drawing;
using LibNbt.Tags;

namespace OpenMinecraft.Entities
{
    public class Cow : LivingEntity
    {

        private static Image icon = new Bitmap("mobs/notch.png");
        public Cow()
        {
        }
        public Cow(NbtCompound c)
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
            return "Cow";
        }

        public override string GetID()
        {
            return "Cow";
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
