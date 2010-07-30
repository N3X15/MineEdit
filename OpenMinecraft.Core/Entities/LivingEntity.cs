using System;
using System.Collections.Generic;

using System.Text;
using LibNbt.Tags;
using System.Drawing;
using System.ComponentModel;

namespace OpenMinecraft.Entities
{
    public class LivingEntity:Entity
    {
        [Category("LivingEntity")]
        [Description("Health of the living entity.")]
        public short Health = 20;

        [Category("LivingEntity")]
        [Description("Affects what damage is currently being dealt.  (If left nonzero while healing, the entity will be sideways.)")]
        public short HurtTime = 0;

        [Category("LivingEntity")]
        public short AttackTime = 0;

        [Category("LivingEntity")]
        public short DeathTime = 0;


        public LivingEntity()
        {
        }
        public LivingEntity(NbtCompound c)
        {
            SetBaseStuff(c);
            Health = (c["Health"] as NbtShort).Value;
            HurtTime = (c["HurtTime"] as NbtShort).Value;
            AttackTime = (c["AttackTime"] as NbtShort).Value;
            DeathTime = (c["DeathTime"] as NbtShort).Value;
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c,GetID());
            c.Tags.Add(new NbtShort("Health", Health));
            c.Tags.Add(new NbtShort("HurtTime", HurtTime));
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
                return MobIcons.mobpig;
            }
        }
    }
}
