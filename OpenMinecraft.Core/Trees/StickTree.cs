using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    /// <summary>
    /// Set up the trunk for trees with a trunk width of 1 and simple geometry.
    /// </summary>
    public class StickTree:Tree
    {
        public StickTree(long x, long y, long z, int h)
            :base(x,y,z,h)
        {
        }
        public override void  MakeTrunk(ref IMapHandler map)
        {
            int x = (int)Pos.X;
            int y = (int)Pos.Y;
            int z = (int)Pos.Z;
        
            for (int i = 0; i<Height;i++)
            {
                map.SetBlockAt(x, y, z, 17);
                y += 1;
            }
        }
    }
}
