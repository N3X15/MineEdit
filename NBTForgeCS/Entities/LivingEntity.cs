using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace MineEdit
{
    public class LivingEntity:Entity
    {
        public short Health = 20;
        public short HurtTime = 0;
        public short Air = 300;
        public short AttackTime = 0;
        public short DeathTime = 0;

        public LivingEntity()
        {
        }
        public LivingEntity(NbtCompound c)
        {
            SetBaseStuff(c);
            Health = (c["Health"] as NbtShort).Value;
            HurtTime = (c["HurtTime"] as NbtShort).Value;
            Air = (c["Air"] as NbtShort).Value;
            AttackTime = (c["AttackTime"] as NbtShort).Value;
            DeathTime = (c["DeathTime"] as NbtShort).Value;
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c,GetID());
            c.Tags.Add(new NbtShort("Health", Health));
            c.Tags.Add(new NbtShort("HurtTime", HurtTime));
            c.Tags.Add(new NbtShort("Air", Air));
            c.Tags.Add(new NbtShort("AttackTime", AttackTime));
            c.Tags.Add(new NbtShort("DeathTime", DeathTime));
            return c;
        }
        public override string ToString()
        {
            return "_LIVINGENTITY_";
        }

        public override string GetID()
        {
            return "LivingEntity";
        }


        public override Image Image
        {
            get
            {
                return Properties.Resources.mobpig;
            }
        }
    }
}
