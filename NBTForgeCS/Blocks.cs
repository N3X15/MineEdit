//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
namespace MineEdit
{
    public class Block
    {
        public short    ID;
        public string   Name;
        public Bitmap   Image;
        public Color    Color;
        public Block(byte id,string name,Bitmap image,Color color)
        {
            this.ID=id;
            this.Name=name;
            this.Image=image;
            this.Color=color;
            Console.Write(ToString());
        }
        public Block()
        {
        }

        public override string ToString()
        {
            return string.Format("0x{0:X2} - {1}",ID,Name);
        }
    }
    public static class Blocks
    {
        public static Dictionary<short,Block> BlockList = new Dictionary<short,Block>();

        public static void Init()
        {
            foreach(string line in File.ReadAllLines("blocks.txt"))
            {
                if (string.IsNullOrEmpty(line)) continue;
                //Console.WriteLine(line);
                // dec file color name
                string[] chunks = line.Split(new string[]{"\t"},StringSplitOptions.RemoveEmptyEntries);
                if (chunks[0].StartsWith("#")) continue;

                Block b = new Block();
                short id = short.Parse(chunks[0]);
                b.ID = id;
                b.Name = chunks[2];
                string bf = Path.Combine("blocks", string.Format("{0}.png", (short)id));
                string if_ = Path.Combine("items", chunks[1]);
                string af = Path.Combine("blocks","0.png");

                if (id<100 && File.Exists(bf))
                    b.Image = (Bitmap)Bitmap.FromFile(bf);
                else if (File.Exists(if_))
                    b.Image = (Bitmap)Bitmap.FromFile(if_);
                else
                    b.Image = (Bitmap)Bitmap.FromFile(af);
                b.Color = GetColorFor(b);
                BlockList.Add(id, b);
                Console.WriteLine(b);
            }
        }

        /// <summary>
        /// Get average color of the image for this block
        /// </summary>
        /// <param name="b">Block</param>
        /// <returns>Average color of block texture</returns>
        private static Color GetColorFor(Block b)
        {
            int R, G, B,C;
            R = G = B = C=0;
            for (int x = 0; x < b.Image.Width; x++)
            {
                for (int y = 0; y < b.Image.Height; y++)
                {
                    Color c = b.Image.GetPixel(x, y);
                    R += (int)c.R;
                    G += (int)c.G;
                    B += (int)c.B;
                    C++;
                }
            }
            return Color.FromArgb(R / C, G / C, B / C);
        }


        internal static Color GetColor(byte b)
        {
            if (!BlockList.ContainsKey(b))
            {
                Console.WriteLine("BlockList does not contain a definition for {0} (0x{0:X2}).", b);
            }
            return BlockList[b].Color;
        }

        internal static Block Get(short type)
        {
            if (!BlockList.ContainsKey(type))
            {
                Console.WriteLine("BlockList does not contain a definition for {0} (0x{0:X2}).", type);
            }
            return BlockList[type];
        }
    }
}