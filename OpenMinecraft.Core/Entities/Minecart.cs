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
/*
TAG_Compound: 12 entries
{
	TAG_String("id"): Minecart
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 227.230091474848
		TAG_Double: 64.5
		TAG_Double: 124.5
	}
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: 0
		TAG_Double: 0
		TAG_Double: 0
	}
	TAG_Byte("OnGround"): 0
	TAG_Int("Type"): 2
	TAG_Short("Fire"): -1
	TAG_Double("PushX"): 0
	TAG_Double("PushZ"): 0
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 158.4776
		TAG_Float: 0
	}
	TAG_Short("Fuel"): 0
}
*/
    public enum MinecartType :int
    {
        EMPTY=0,
        CHEST=1,
        FURNACE=2
    }
    public class Minecart:Entity
    {
		
		[Category("Minecart"), Description("(WIP)")]
		public MinecartType Type {get;set;}

		[Category("Minecart"), Description("(WIP)")]
		public double PushX {get;set;}

		[Category("Minecart"), Description("(WIP)")]
		public double PushZ {get;set;}

		[Category("Minecart"), Description("(WIP)")]
		public short Fuel {get;set;}

        public Minecart()
        {
            Type = MinecartType.EMPTY;
            PushX = 0;
            PushZ = 0;
            Fuel = 0;
        }

        public Minecart(NbtCompound c)
        {
            SetBaseStuff(c);
			
			Type = (MinecartType)c.Get<NbtInt>("Type").Value;
            if (c.Has("PushX"))
            {
                PushX = c.Get<NbtDouble>("PushX").Value;
                PushZ = c.Get<NbtDouble>("PushZ").Value;
                Fuel = c.Get<NbtShort>("Fuel").Value;
            }
            else
            {
                PushX = 0;
                PushZ = 0;
                Fuel = 0;
            }
        }
        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c, GetID());
			
			c.Tags.Add(new NbtInt("Type", (int)Type));
			c.Tags.Add(new NbtDouble("PushX", PushX));
			c.Tags.Add(new NbtDouble("PushZ", PushZ));
			c.Tags.Add(new NbtShort("Fuel", Fuel));
            return c;
        }
        public override string ToString()
        {
            return "Minecart";
        }

        public override string GetID()
        {
            return "Minecart";
        }

        public override Image Image
        {
            get
            {
                switch (Type)
                {
                    case MinecartType.EMPTY:
                        return Blocks.Get(328).Image;
                    case MinecartType.CHEST:
                        return Blocks.Get(342).Image;
                    case MinecartType.FURNACE:
                        return Blocks.Get(343).Image;
                    default:
                        return Bitmap.FromFile("mob/notch.png");
                }
            }
        }
    }
}
