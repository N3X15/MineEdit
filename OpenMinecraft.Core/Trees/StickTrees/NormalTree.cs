using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    /// <summary>
    /// Set up the foliage for a 'normal' tree.
    ///
    /// This tree will be a single bulb of foliage above a single width trunk.
    /// This shape is very similar to the default Minecraft tree.
    /// </summary>
    public class NormalTree:StickTree
    {
        public NormalTree(long x, long y, long z, int h)
            : base(x, y, z, h)
        {
        }
        /*
        note, foliage will disintegrate if there is no foliage below, or
        if there is no "log" block within range 2 (square) at the same level or 
        one level below
        */
        public override void MakeFoliage(ref IMapHandler map)
        {
            Console.WriteLine("Adding tree at {0}", Pos);
            int topy = (int)Pos.Y + Height - 1;
            int start = topy - 2;
            int end = topy + 2;
            for(int y = start;y<end;y++)
            {
                int rad=0;
                if(y > start + 1)
                    rad = 1;
                else
                    rad = 2;
                for(int xoff = -rad;xoff<rad+1;xoff++)
                {
                    for(int zoff = -rad;zoff<rad+1;zoff++)
                    {
                        
                        if (Math.Abs(xoff) == Math.Abs(zoff) && Math.Abs(xoff) == rad)
                            continue;
                    
                        int x = (int)Pos.X + xoff;
                        int z = (int)Pos.Z + zoff;
                    
                        map.SetBlockAt(x,y,z,18);
                    }
                }
            }
        }

    }
}
