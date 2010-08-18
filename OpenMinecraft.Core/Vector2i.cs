
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
    }
}
