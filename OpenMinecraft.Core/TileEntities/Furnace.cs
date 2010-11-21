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

using System;
using System.Drawing;
using LibNbt.Tags;

namespace OpenMinecraft.TileEntities
{
	/*
	*** Unknown TileEntity: Furnace
	TAG_Compound: 7 entries
	{
		TAG_List("Items"): 1 entries
		{
			TAG_Compound: 4 entries
			{
				TAG_Short("id"): 20
				TAG_Short("Damage"): 0
				TAG_Byte("Count"): 12
				TAG_Byte("Slot"): 2
			}
		}
		TAG_String("id"): Furnace
		TAG_Short("BurnTime"): 0
		TAG_Int("z"): -267
		TAG_Short("CookTime"): 0
		TAG_Int("y"): 73
		TAG_Int("x"): 42
	}
	 */
	public class Furnace:TileEntity
	{
		private static Image icon;
		public InventoryItem[] Slots = new InventoryItem[3];
		public short BurnTime = 0;
		public short CookTime = 0;

		public Furnace() {}
		
		public Furnace(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
			: base(CX, CY, CS, c)
		{
			BurnTime = (c["BurnTime"] as NbtShort).Value;
			CookTime = (c["CookTime"] as NbtShort).Value;

			for (int i = 0; i < Slots.Length; i++)
			{
					try
					{
						if ((c["Items"] as NbtList).Tags[i]!=null)
						{
							NbtCompound cc = (NbtCompound)(c["Items"] as NbtList).Tags[i];
							Slots[i] = new InventoryItem();
							Slots[i].ID = cc.Get<NbtShort>("id").Value;
							Slots[i].Damage = cc.Get<NbtShort>("Damage").Value;
							Slots[i].Slot = 0;
							Slots[i].Count = cc.Get<NbtByte>("Count").Value;

						}
					}
					catch (Exception)
					{
					}
			}
		}
		public Furnace(NbtCompound c)
			:base(c)
		{
			BurnTime = (c["BurnTime"] as NbtShort).Value;
			CookTime = (c["CookTime"] as NbtShort).Value;

			for (byte i = 0; i < 3; i++)
			{
				if ((c["Items"] as NbtList).Tags[i] != null)
				{
					NbtCompound cc = (NbtCompound)(c["Items"] as NbtList).Tags[i];
					Slots[i]=new InventoryItem();
					Slots[i].ID = cc.Get<NbtShort>("id").Value;
					Slots[i].Damage = cc.Get<NbtShort>("Damage").Value;
					Slots[i].Count = cc.Get<NbtByte>("Count").Value;
					Slots[i].Slot = i;
				}
			}
		}

		public override Image Image 
		{ 
			get 
			{
				if(icon==null)
					icon = Blocks.Get(62).Image;
				return icon; 
			} 
		}

		public override NbtCompound ToNBT()
		{
			NbtCompound c = new NbtCompound();
			Base2NBT(ref c);
			c.Add(new NbtShort("BurnTime",BurnTime));
			c.Add(new NbtShort("CookTime",CookTime));
			NbtList Items = new NbtList("Items");
			for (int i = 0; i < 3; i++)
			{
				if (Slots[i].ID != 0x00)
				{
					NbtCompound cc = new NbtCompound();

					cc.Add(new NbtShort("id",Slots[i].ID));
					cc.Add(new NbtShort("Damage",(short)Slots[i].Damage));
					cc.Add(new NbtByte("Count",(byte)Slots[i].Count));
					cc.Add(new NbtByte("Slot", (byte)i));

					Items.Add(cc);
				}
			}
			c.Add(Items);
			return c;
		}

		public override string ToString()
		{
			return "Furnace";
		}

        public override string GetID()
        {
            return "Furnace";
        }
	}
}
