using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.Entities
{
    public class Spider:LivingEntity
    {
        public Spider()
        {
        }
        public Spider(NbtCompound c)
            :base(c)
        {
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
            return c;
        }
        public override string ToString()
        {
            return "Spider";
        }

        public override string GetID()
        {
            return "Spider";
        }

        public override Image Image
        {
            get
            {
                return new Bitmap("mobs/spider.png"); 
            }
        }
    }
}
