using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    public class Dimension
    {
        public int ID;
        public string Name;
        public string Folder;
        public Vector2i MaximumChunk = new Vector2i(-1,-1);
        public Vector2i MinimumChunk = new Vector2i(1,1);
        public Vector2i Size
        {
            get
            {
                return new Vector2i(
                    MaximumChunk.X - MinimumChunk.X,
                    MaximumChunk.Y - MinimumChunk.Y
                );
            }
        }

        public Dimension(int id, string name, string folder)
        {
            ID = id;
            Name = name;
            Folder = folder;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
