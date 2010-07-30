//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Net;
namespace OpenMinecraft
{
    public class Block
    {
        /// <summary>
        /// Item ID #.  
        /// </summary>
        public short    ID;
        /// <summary>
        /// Item's name
        /// </summary>
        public string   Name;
        /// <summary>
        /// Bitmap image of the object's icon.
        /// </summary>
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
        /// <summary>
        /// All known blocks and items, indexed by Minecraft ID number.
        /// </summary>
        public static Dictionary<short,Block> BlockList = new Dictionary<short,Block>();

        /// <summary>
        /// Blocks to download (updater).
        /// </summary>
        private static Queue<Block> BlocksToDL = new Queue<Block>();

        /// <summary>
        /// Version number.  MMDDYYYY format.
        /// </summary>
        public static string Version = "07122010";

        /// <summary>
        /// Total images for download.
        /// </summary>
        public static int TotalImages = 0;
        public static byte Water=9;

        /// <summary>
        /// Load blocks, update if needed.
        /// </summary>
        public static void Init()
        {
            if (!File.Exists("blocks.txt"))
            {
                frmUpdate up = new frmUpdate();
                up.Start();
                up.ShowDialog();
                return;
            }
            MapGenerators.Init();
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
#if DEBUG
                Console.WriteLine(b);
#endif
            }
        }

        public static bool CheckForUpdates()
        {
            WebClient wc = new WebClient();
            string hurp;
            try
            {
                hurp = wc.DownloadString("http://github.com/N3X15/MineEdit/raw/master/OpenMinecraft.Core/Blocks.cs");
            }
            catch(Exception)
            {
                return false;
            }
            foreach (string l in hurp.Split('\n'))
            {
                string line = l.Trim();
                if (line.StartsWith("public static string Version = "))
                {
                    string v = line.Substring("public static string Version = ".Length + 1, 8);
                    if (Blocks.Version != v)
                    {
                        Console.WriteLine("{0} != {1}", v, Blocks.Version);
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clear all blocks
        /// </summary>
        public static void Clear()
        {
            foreach (KeyValuePair<short,Block> br in BlockList)
            {
                br.Value.Image.Dispose(); // Reload images
            }
            BlockList.Clear();
            TotalImages = 0;
        }

        /// <summary>
        /// Save blocks to file.
        /// </summary>
        public static void Save()
        {
            List<string> lolfile = new List<string>();
            foreach (KeyValuePair<short, Block> p in BlockList)
            {
                lolfile.Add(string.Join("\t",new string[] { p.Key.ToString(), p.Key.ToString() + ".png", p.Value.Name }));
            }
            File.WriteAllLines("blocks.txt",lolfile.ToArray());
        }
        /// <summary>
        /// Update blocks from web.
        /// </summary>
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

        /// <summary>
        ///  Update itms from web.
        /// </summary>
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
                    short i=0;
                    if (Find(nb.Name) != null)
                    {
                        string type = (nb.ID<255) ? " block": " (item)";
                        nb.Name+=type;
                        if (Find(nb.Name) != null)
                        {
                            while(true)
                            {
                                string nm = nb.Name+" "+(++i).ToString();
                                if (Find(nm) == null)
                                {
                                    nb.Name=nm;
                                    break;
                                }
                            }
                        }
                    }
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


        public static Color GetColor(byte b)
        {
            if (!BlockList.ContainsKey(b))
            {
                Console.WriteLine("BlockList does not contain a definition for {0} (0x{0:X2}).", b);
                return Color.Red;
            }
            return BlockList[b].Color;
        }

        public static Block Get(short type)
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

        public static Block Find(string p)
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