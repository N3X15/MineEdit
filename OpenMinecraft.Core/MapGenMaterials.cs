using System;
using System.ComponentModel;

namespace OpenMinecraft
{
    /// <summary>
    /// Tell the map generator which materials it should use.
    /// </summary>
    public class MapGenMaterials
    {

        [Browsable(true)]
        public byte Rock=1;

        [Browsable(true)]
        public byte Soil=3;

        [Browsable(true)]
        public byte Grass=2;
        public byte Sand=12;
        public byte Snow=78;
        public byte Gravel=13;
        public byte Water=9;
        public byte Lava=11;
    }
}
