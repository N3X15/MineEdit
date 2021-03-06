﻿using System;
using System.Collections.Generic;

using System.Text;
using LibNbt.Tags;

namespace OpenMinecraft
{
    public class InventoryCollection:Dictionary<byte,InventoryItem>
    {
        public delegate void InventoryChangedDelegate(byte slot);
        public event InventoryChangedDelegate Changed;

        public void Add(NbtCompound item)
        {
            InventoryItem c = new InventoryItem();
            c.Count = item.Get<NbtByte>("Count").Value;
            c.Damage = item.Get<NbtShort>("Damage").Value;
            c.ID = item.Get<NbtShort>("id").Value;
            c.Slot = item.Get<NbtByte>("Slot").Value;
            if (ContainsKey(c.Slot))
            {
                Console.WriteLine("Tried to add to slot {0}, which already has something.", c.Slot);
                Remove(c.Slot);
            }
            base.Add(c.Slot, c);
            SlotChanged(c.Slot);
        }

        public new void Add(byte slot, InventoryItem i)
        {
            base.Add(slot, i);
            SlotChanged(slot);
        }

        public new void Remove(byte ID)
        {
            base.Remove(ID);
            SlotChanged(ID);
        }

        public NbtTag ToNBT()
        {
            NbtList Items = new NbtList("Items");
            foreach (KeyValuePair<byte, InventoryItem> p in this)
            {
                NbtCompound nc = new NbtCompound();
                nc.AddRange(new NbtTag[]{
                    new NbtByte("Count",(byte)p.Value.Count),
                    new NbtByte("Slot",p.Key),
                    new NbtShort("id",p.Value.ID),
                    new NbtShort("Damage",(short)p.Value.Damage)
                });
                Items.Add(nc);
            }
            return Items;
        }

        private void SlotChanged(byte p)
        {
            if (Changed != null)
                Changed(p);
        }
    }
}
