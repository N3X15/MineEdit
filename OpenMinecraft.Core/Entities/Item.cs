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

using System.Drawing;
using LibNbt.Tags;

namespace OpenMinecraft.Entities
{
    /*
*** BUG: Unknown entity (ID: Item)
TAG_Compound: 11 entries
{
	TAG_String("id"): Item
	TAG_List("Pos"): 3 entries
	{
		TAG_Double: 28.7666982214875
		TAG_Double: 82.125
		TAG_Double: 4.44008329172237
	}
	TAG_Short("Age"): 2085
	TAG_Compound("Item"): 3 entries
	{
		TAG_Short("id"): 39
		TAG_Short("Damage"): 0
		TAG_Byte("Count"): 1
	}
	TAG_List("Motion"): 3 entries
	{
		TAG_Double: -4.94065645841247E-324
		TAG_Double: 0
		TAG_Double: 4.94065645841247E-324
	}
	TAG_Byte("OnGround"): 1
	TAG_Short("Fire"): -1
	TAG_Short("Health"): 5
	TAG_Float("FallDistance"): 0
	TAG_Short("Air"): 300
	TAG_List("Rotation"): 2 entries
	{
		TAG_Float: 119.444
		TAG_Float: 0
	}
}*/
    public class Item:Entity
    {
        public short ItemID = 0;
        public short Damage = 0;
        public byte Count = 1;
        public short Health = 5; // WTF notch
        public short Age = 2085;
        private Block B;
        public Item()
        {
        }

        public Item(NbtCompound c)
        {
            SetBaseStuff(c);
            NbtCompound i = (c["Item"] as NbtCompound);
            ItemID = (i["id"] as NbtShort).Value;
            Damage = (i["Damage"] as NbtShort).Value;
            Count = (i["Count"] as NbtByte).Value;
            Health = (c["Health"] as NbtShort).Value;
            Age = (c["Age"] as NbtShort).Value;

            B = Blocks.Get(ItemID);
        }
        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c, GetID());
            NbtCompound i = new NbtCompound("Item");
            i.Add(new NbtShort("id", ItemID));
            i.Add(new NbtShort("Damage", Damage));
            i.Add(new NbtByte("Count", Count));
            c.Add(i);
            c.Add(new NbtShort("Health", Health));
            c.Add(new NbtShort("Age", Age));
            return c;
        }
        public override string ToString()
        {
            return string.Format("{0} {1}{2}",Count,B.Name,(Count>1) ? "s":"");
        }

        public override string GetID() { return "Item"; }

        public override Image Image
        {
            get
            {
                return B.Image;
            }
        }
    }
}
