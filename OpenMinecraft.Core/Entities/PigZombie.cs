/**
 * Copyright (c) 2010, MineEdit Contributors
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

using System.Drawing;
using LibNbt.Tags;
using System.ComponentModel;

namespace OpenMinecraft.Entities
{
    public class PigZombie:LivingEntity
    {
/*
TAG_Compound: 13 entries
{
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: -0.0530102556896606
		TAG_Double: -0.0784000015258789
		TAG_Double: -0.00860787307999079
	}
	TAG_Byte("OnGround"): 1
	TAG_Short("HurtTime"): 0
	TAG_Short("Health"): 20
	TAG_Short("Air"): 300
	TAG_Short("Anger"): 0
	TAG_String("id"): PigZombie
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 116.762099332926
		TAG_Double: 54
		TAG_Double: -7.29493684872398
	}
	TAG_Short("AttackTime"): 0
	TAG_Short("Fire"): -1
	TAG_Float("FallDistance"): 0
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 99.22828
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}
*/
		
		[Category("PigZombie"), Description("How much you've pissed it off.")]
		public short Anger {get;set;}
        public PigZombie()
        {
        }
        public PigZombie(NbtCompound c)
            :base(c)
        {
			
			Anger = c.Get<NbtShort>("Anger").Value;
        }

        public NbtCompound ToNBT()
        {
            NbtCompound c = base.ToNBT();
			
			c.Tags.Add(new NbtShort("Anger", Anger));
            return c;
        }
        public override string ToString()
        {
            return "PigZombie";
        }

        public override string GetID()
        {
            return "PigZombie";
        }

        public override Image Image
        {
            get
            {
                return new Bitmap("mobs/pigzombie.png");
            }
        }
    }
}
