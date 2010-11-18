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
