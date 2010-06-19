using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineEdit
{
    static class Utils
    {
        #region Clamp
        /// <summary>
        /// Ensure value is between min and max.
        /// </summary>
        /// <param name="val">value</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Clamped value</returns>
        public static short Clamp(short val, short min, short max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static float Clamp(float val, float min, float max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static long Clamp(long val, long min, long max)
        {
            return Math.Min(Math.Max(val, min), max);
        }
        #endregion
    }
}
