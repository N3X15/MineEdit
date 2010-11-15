using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;
using System.ComponentModel;

namespace OpenMinecraft.Entities
{
    public class Slime:LivingEntity
    {
        /*
	TAG_Int("Size"): 0
}*/

		private static Image icon = new Bitmap("mobs/slime.png");

        [Category("Slime"), Description("Size of slime - 1.  A 4x4 slime would be size 3.")]
        public int Size {get;set;}

        public Slime()
        {
        }
        public Slime(NbtCompound c)
            :base(c)
        {
            Size = c.Get<NbtInt>("Size").Value;
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
            c.Tags.Add(new NbtInt("Size", Size));
            return c;
        }
        public override string ToString()
        {
            return string.Format("Slime ({0}x{0})",Size+1);
        }

        public override string GetID()
        {
            return "Slime";
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
