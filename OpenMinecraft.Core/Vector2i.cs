
using System;
namespace OpenMinecraft
{
    public class Vector2i
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        /// <summary>
        /// Pythagorean distance.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="me"></param>
        /// <returns></returns>
        public static double Distance(Vector2i a, Vector2i b)
        {
            return Math.Sqrt(((a.X - b.X) ^ 2) + ((a.Y - b.Y) ^ 2));
        }
    }
}
