/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.ComponentModel;
using System.Drawing;
using LibNbt.Tags;
using System;

namespace OpenMinecraft.Entities
{
    public class LivingEntity:Entity
    {

        [Category("LivingEntity"), Description("Amount of air this creature has left"), DefaultValue(200)]
        public short Air { get; set; }

        [Category("LivingEntity"), Description("OH GOD I'M ON FIRE"), DefaultValue(-20)]
        public short Fire { get; set; }
		
        [Category("LivingEntity"),Description("Health of the living entity.")]
        public short Health = 20;

        [Category("LivingEntity"),Description("Affects what damage is currently being dealt.  (If left nonzero while healing, the entity will be sideways.)")]
        public short HurtTime = 0;

        [Category("LivingEntity")]
        public short AttackTime = 0;

        [Category("LivingEntity")]
        public short DeathTime = 0;

        private string lolID = ""; 

        public LivingEntity()
        {
        }
        public LivingEntity(NbtCompound c)
        {
            SetBaseStuff(c);
            if (!c.Has("HurtTime"))
            {
                Console.WriteLine(c);
                return;
            }
            Air = (c["Air"] as NbtShort).Value;
            Fire = (c["Fire"] as NbtShort).Value;
            lolID = (c["id"] as NbtString).Value;
            Health = (c["Health"] as NbtShort).Value;
            HurtTime = (c["HurtTime"] as NbtShort).Value;
            AttackTime = (c["AttackTime"] as NbtShort).Value;
            DeathTime = (c["DeathTime"] as NbtShort).Value;
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c, GetID());
            c.Add(new NbtShort("Air", Air));
            c.Add(new NbtShort("Fire", Fire));
            c.Add(new NbtShort("Health",       Health));
            c.Add(new NbtShort("HurtTime",     HurtTime));
            c.Add(new NbtShort("AttackTime",   AttackTime));
            c.Add(new NbtShort("DeathTime",    DeathTime));
            return c;
        }

        public override string ToString()
        {
            return lolID + "?";
        }

        public override string GetID()
        {
            return lolID;
        }

        public override Image Image
        {
            get
            {
                return new Bitmap("mobs/pig.png");
            }
        }
    }
}
