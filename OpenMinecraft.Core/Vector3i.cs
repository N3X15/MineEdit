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

        public long this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: return 0;
                }
            }
            set
            {
                switch (idx)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: break;
                }
            }
        }

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

        public Vector3i(Vector3i a)
        {
            this.X = a.X;
            this.Y = a.Y;
            this.Z = a.Z;
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

        public static int Distance(Vector3i v1, Vector3i v2)
        {
            if (v1 == null || v2 == null) return int.MaxValue;
            int x = (int)(v1.X - v2.X);
            int y = (int)(v1.Y - v2.Y);
            int z = (int)(v1.Z - v2.Z);
            return Isqrt(x * x + y * y + z * z);
        }
        private static int Isqrt(int num)
        {
            if (0 == num) { return 0; }  // Avoid zero divide
            int n = (num / 2) + 1;       // Initial estimate, never low
            int n1 = (n + (num / n)) / 2;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (num / n)) / 2;
            } // end while
            return n;
        }
        public static double Distance(Vector3d v1, Vector3d v2)
        {
            if (v1 == null || v2 == null) return double.MaxValue;
            double x = v1.X - v2.X;
            double y = v1.Y - v2.Y;
            double z = v1.Z - v2.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        // For doing shit like flipping around axes.
        public long[] ToArray()
        {
            return new long[]{X,Y,Z};
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
        }

        public static Vector3i operator *(Vector3i a, Vector3i b)
        {
            return new Vector3i(
                a.X * b.X,
                a.Y * b.Y,
                a.Z * b.Z);
        }

        public static Vector3i operator *(Vector3i a, double b)
        {
            return new Vector3i(
                (long)(a.X * b),
                (long)(a.Y * b),
                (long)(a.Z * b));
        }

        public static Vector3i operator /(Vector3i a, double b)
        {
            return new Vector3i(
                (long)(a.X / b),
                (long)(a.Y / b),
                (long)(a.Z / b));
        }

        public static Vector3i operator +(Vector3i a, double b)
        {
            return new Vector3i(
                (long)(a.X + b),
                (long)(a.Y + b),
                (long)(a.Z + b));
        }

        public static Vector3i operator -(Vector3i a, double b)
        {
            return new Vector3i(
                (long)(a.X - b),
                (long)(a.Y - b),
                (long)(a.Z - b));
        }

    }
}
