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

/*

TAG_Compound: 12 entries
{
	TAG_String("id"): Chicken
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 84.6725482556877
		TAG_Double: 68
		TAG_Double: 138.479193280034
	}
	TAG_Short("AttackTime"): 0
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 8.82782662812105E-07
		TAG_Double: -0.0784000015258789
		TAG_Double: -2.03381141496923E-06
	}
	TAG_Short("HurtTime"): 0
	TAG_Byte("OnGround"): 1
	TAG_Short("Fire"): -1
	TAG_Short("Health"): 4
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: -246.3079
		TAG_Float: 0
	}
	TAG_Short("DeathTime"): 0
}

*/

using System.Drawing;
using LibNbt.Tags;

namespace OpenMinecraft.Entities
{
    public class Chicken : LivingEntity
    {

        private static Image icon = new Bitmap("mobs/notch.png");
        public Chicken()
        {
        }
        public Chicken(NbtCompound c)
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
            return "Chicken";
        }

        public override string GetID()
        {
            return "Chicken";
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
