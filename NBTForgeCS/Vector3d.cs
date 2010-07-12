using System;
using System.Collections.Generic;

using System.Text;
using LibNbt.Tags;

namespace MineEdit
{
    public class Vector3d
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3d()
        {
        }
        public Vector3d(long x, long y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3d(LibNbt.Tags.NbtList nbtList)
        {
            this.X = (nbtList[0] as NbtDouble).Value;
            this.Y = (nbtList[1] as NbtDouble).Value;
            this.Z = (nbtList[2] as NbtDouble).Value;
        }

        public Vector3d(double x, double y, double z)
        {
            // TODO: Complete member initialization
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            Vector3d derp = new Vector3d();
            derp.X = a.X - a.X;
            derp.Y = a.Y - a.Y;
            derp.Z = a.Z - a.Z;
            return derp;
        }

        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            Vector3d derp = new Vector3d();
            derp.X = a.X + a.X;
            derp.Y = a.Y + a.Y;
            derp.Z = a.Z + a.Z;
            return derp;
        }

        public static explicit operator Vector3i(Vector3d a)
        {
            return new Vector3i((long)a.X, (long)a.Y, (long)a.Z);
        }

        public static explicit operator Vector3d(Vector3i a)
        {
            return new Vector3d(a.X, a.Y, a.Z);
        }

        public override string ToString()
        {
            return string.Format("<{0}, {1}, {2}>", X, Y, Z);
        }
    }
}
