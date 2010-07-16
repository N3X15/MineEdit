using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class Zombie:LivingEntity
    {
        public Zombie()
        {
        }
        public Zombie(NbtCompound c)
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
            return "Zombie";
        }

        public override string GetID()
        {
            return "Zombie";
        }

        public override Image Image
        {
            get
            {
                return MobIcons.mobzombie;
            }
        }
    }
}
