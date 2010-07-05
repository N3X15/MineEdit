using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MineEdit
{
    public class Chunk
    {
        public Vector3i Position = new Vector3i(0,0,0);
        public Vector3i Size = new Vector3i(0,0,0);
        public string Filename = "";
        public string Creator = "";
        public DateTime CreationDate = DateTime.Now;
        public int MinHeight = 0;
        public int MaxHeight = 1;

        public Chunk() { }

        public void Delete()
        {
            File.Delete(Filename);
        }

    }
}
