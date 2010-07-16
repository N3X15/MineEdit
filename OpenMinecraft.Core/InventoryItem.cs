using System;
using System.Collections.Generic;
using System.Text;

namespace OpenMinecraft
{
    public struct InventoryItem
    {
        //public string Name;
        public byte Count;
        public byte Slot;
        public short ID;
        public short Damage;

        /*
        public InventoryItem(short type, short damage, byte count)
        {
            this.ID = type;
            this.Damage = damage;
            this.Count = count;
        }
        */
    }
}
