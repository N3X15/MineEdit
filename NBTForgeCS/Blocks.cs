//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Net;
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
            return Name;
        }
    }
    public static class Blocks
    {
        public static Dictionary<short,Block> BlockList = new Dictionary<short,Block>();
        private static Queue<Block> BlocksToDL = new Queue<Block>();
        public static string Version = "07042010";
        public static int TotalImages = 0;
        public static void Init()
        {
            if (!File.Exists("blocks.txt"))
            {
                frmUpdate up = new frmUpdate();
                up.Start();
                up.ShowDialog();
                return;
            }
            foreach(string line in File.ReadAllLines("blocks.txt"))
            {
                if (string.IsNullOrEmpty(line)) 
                    continue;
                if (line.StartsWith("#")) continue;
                //Console.WriteLine(line);
                // dec file color name
                string[] chunks = line.Split(new string[]{"\t"},StringSplitOptions.RemoveEmptyEntries);

                Block b = new Block();
                short id = short.Parse(chunks[0]);
                b.ID = id;
                b.Name = chunks[2];
                string bf = Path.Combine("blocks", string.Format("{0}.png", (short)id));
                string if_ = Path.Combine("items", chunks[1]);
                string af = Path.Combine("blocks","0.png");

                if (id<255 && File.Exists(bf))
                    b.Image = (Bitmap)Bitmap.FromFile(bf);
                else if (File.Exists(if_))
                    b.Image = (Bitmap)Bitmap.FromFile(if_);
                else
                    b.Image = new Bitmap(16,16);
                b.Color = GetColorFor(b);
                BlockList.Add(id, b);
                Console.WriteLine(b);
            }
        }

        public static void Clear()
        {
            foreach (KeyValuePair<short,Block> br in BlockList)
            {
                br.Value.Image.Dispose(); // Reload images
            }
            BlockList.Clear();
            TotalImages = 0;
        }

        public static void Save()
        {
            List<string> lolfile = new List<string>();
            foreach (KeyValuePair<short, Block> p in BlockList)
            {
                lolfile.Add(string.Join("\t",new string[] { p.Key.ToString(), p.Key.ToString() + ".png", p.Value.Name }));
            }
            File.WriteAllLines("blocks.txt",lolfile.ToArray());
        }
        public static void UpdateBlocks()
        {
            WebClient wc = new WebClient();
            string[] derp = wc.DownloadString("http://copy.bplaced.net/mc/blocks/").Split(new string[]{"\n","\r\n"},StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in derp)
            {
                if (line.StartsWith("<img src=\"/mc/icons/"))
                {

                    string[] chunks = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //Console.WriteLine(string.Join(", ", chunks));
                    // icon                         HEX  DEC  NAME
                    //<img src="/mc/icons/0.png"/>    0    0  Air
                    Block nb = new Block();
                    nb.ID = short.Parse(chunks[3]);
                    nb.Name = string.Join(" ", chunks, 4, chunks.Length - 4);
                    WebClient icondl = new WebClient();

                    BlocksToDL.Enqueue(nb);
                    TotalImages++;
                }
            }
        }

        public static void UpdateItems()
        {
            WebClient wc = new WebClient();
            string[] derp = wc.DownloadString("http://copy.bplaced.net/mc/items/").Split(new string[]{"\n","\r\n"},StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in derp)
            {
                if (line.StartsWith("<img src=\"/mc/icons/"))
                {
                    string[] chunks = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    // icon                         HEX  DEC  NAME
                    //<img src="/mc/icons/0.png"/>    0    0  Air
                    Block nb = new Block();
                    nb.ID = short.Parse(chunks[3]);
                    nb.Name = string.Join(" ", chunks, 4, chunks.Length - 4);
                    BlocksToDL.Enqueue(nb);
                    TotalImages++;
                }
            }
        }

        public static int GetImagesLeft(out string image)
        {
            Block nb = BlocksToDL.Dequeue();
            image = Path.Combine("blocks", nb.ID.ToString() + ".png");
            if(nb.ID>255)
                image = Path.Combine("items", nb.ID.ToString() + ".png");

            WebClient icondl = new WebClient();
            string f = string.Format("http://copy.bplaced.net/mc/icons/{0}.png", nb.ID);
            Console.WriteLine(" * Downloading "+f+"...");
            icondl.DownloadFile(f, image);
            Console.WriteLine("Done.");
            nb.Image = (Bitmap)Image.FromFile(image);
            nb.Color = GetColorFor(nb);


            BlockList.Add(nb.ID, nb);
            return BlocksToDL.Count;
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
                return Color.Red;
            }
            return BlockList[b].Color;
        }

        internal static Block Get(short type)
        {
            if (!BlockList.ContainsKey(type))
            {
                Console.WriteLine("BlockList does not contain a definition for {0} (0x{0:X2}).", type);
                Block b = new Block();
                b.Color = Color.Black;
                b.ID = type;
                b.Image = GetQuestionMark();
                b.Name = "[UNKNOWN]";
                return b;
            }
            return BlockList[type];
        }

        private static Bitmap GetQuestionMark()
        {
            Bitmap bmp = new Bitmap(16,16);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString("?", new Font(FontFamily.GenericSansSerif, 10), Brushes.Red, 0, 0);
            g.Dispose();
            return bmp;
        }

        internal static Block Find(string p)
        {
            foreach (KeyValuePair<short, Block> b in BlockList)
            {
                if (b.Value.Name.StartsWith(p))
                    return b.Value;
            }
            return null;
        }
    }
}