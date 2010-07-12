using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace MineEdit
{
    class Giant:LivingEntity
    {

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
                return Properties.Resources.mobcreeper;
            }
        }
    }
}
