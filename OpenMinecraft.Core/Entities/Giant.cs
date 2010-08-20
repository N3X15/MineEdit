using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class Giant:LivingEntity
    {

		private static Image icon = new Bitmap("mobs/notch.png");

        public Giant()
        {
        }
        public Giant(NbtCompound c)
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
            return "Giant";
        }

        public override string GetID()
        {
            return "Giant";
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
