﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
