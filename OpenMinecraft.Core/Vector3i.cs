using System;
using System.Collections.Generic;

using System.Text;

namespace OpenMinecraft
{
    public class Vector3i
    {
        public Int64 X = 0;
        public Int64 Y = 0;
        public Int64 Z = 0;

        public Vector3i(Int64 x, Int64 y, Int64 z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3i(Vector3d f)
        {
            this.X = (long)f.X;
            this.Y = (long)f.Y;
            this.Z = (long)f.Z;
        }

        public Vector3i(int x, int y, int z)
        {
            this.X = (long)x;
            this.Y = (long)y; 
            this.Z = (long)z;
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
            return a;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>",X,Y,Z);
        }

        public static double Distance(Vector3d v1, Vector3d v2)
        {
            double x = v1.X - v2.X;
            double y = v1.Y - v2.Y;
            double z = v1.Z - v2.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }
    }
}
