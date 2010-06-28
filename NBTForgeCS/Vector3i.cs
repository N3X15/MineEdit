using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineEdit
{
    public class Vector3i
    {
        public Int64 X=0;
        public Int64 Y=0;
        public Int64 Z = 0;

        public Vector3i(Int64 x, Int64 y, Int64 z)
        {
            // TODO: Complete member initialization
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3i(Vector3d f)
        {
            // TODO: Complete member initialization
            this.X = (long)f.X;
            this.Y = (long)f.Y;
            this.Z = (long)f.Z;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>",X,Y,Z);
        }
    }
}
