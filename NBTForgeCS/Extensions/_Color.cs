using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
namespace MineEdit
{
    public static class _Color
    {
        public static float[] ToArray(this Color butts)
        {
            float[] fv = new float[4];
            fv[0] = butts.R / 255;
            fv[1] = butts.G / 255;
            fv[2] = butts.B / 255;
            fv[3] = butts.A / 255;

            return fv;
        }
    }
}
