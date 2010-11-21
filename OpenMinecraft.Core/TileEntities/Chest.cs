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
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;

namespace OpenMinecraft.TileEntities
{
    public class Chest:TileEntity
    {
    	private static Image icon;
        public InventoryCollection Inventory = new InventoryCollection();

        // Needed for autoload
        public Chest() { }

        public Chest(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
            : base(CX, CY, CS, c)
        {
            foreach (NbtCompound item in (c["Items"] as NbtList).Tags)
            {
                //NbtCompound item = (NbtCompound)itm;
                Inventory.Add(item);
            }
        }
        public Chest(LibNbt.Tags.NbtCompound c)
            : base(c)
        {
            for(int i =0;i<54;i++)
            {
                try
                {
                    NbtCompound item = (NbtCompound)(c["Items"] as NbtList)[i];
                    Inventory.Add(item);
                }
                catch (Exception)
                {
                }
            }
        }

        public override NbtCompound ToNBT()
        {
            NbtCompound c = new NbtCompound();
            Base2NBT(ref c);
            c.Add(Inventory.ToNBT());
            return c;
        }

        public override Image Image 
        { 
        	get 
        	{
        		if(icon==null)
        			icon = Blocks.Get(54).Image;
        		return icon; 
        	} 
        }

        public override string ToString()
        {
            return "Chest ("+Inventory.Count.ToString()+" items)";
        }

        public override string GetID()
        {
            return "Chest";
        }
    }
}
