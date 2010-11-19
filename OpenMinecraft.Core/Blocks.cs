//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Net;
using System.Xml;
namespace OpenMinecraft
{

	public enum DrawMode {
		Normal,
		Leaves,
		Glass,
		Water
	}
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
        
        public DrawMode DrawMode {get;set;}

        public BlockSide Top;
        public BlockSide Sides;
        public BlockSide Bottom;
        public string FromImage;
		public BlockSide All {
			set {
				Top = value;
				Sides = value;
				Bottom = value;
			}
		}
		
		public bool SideVisible(Block adjacent)
		{
			return (adjacent == null || adjacent.DrawMode != DrawMode.Normal && !(adjacent.DrawMode > DrawMode.Leaves && adjacent==this));
		}
        
        public Block(byte id,string name,Bitmap image,Color color)
        {
            this.ID=id;
            this.Name=name;
            this.Image=image;
            this.Color=color;
            DrawMode = DrawMode.Normal;
            Console.Write(ToString());
            // TODO:  We need a better way of doing this.  A much better way of doing this.
            switch(ID)
            {
            	case 1: All = 1; break;							// ROCK
            	case 2: Top = 0; Sides = 3; Bottom = 2; break; 	// GRASS
            	case 3: All = 2; break;							// DIRT
            	case 4: All = 16; break;						
				case 5: All = 4; break;							// WOOD
				case 7: All = 17; break;						// ADMINIUM
				case 8: 
				//Water
				case 9: 
					DrawMode = DrawMode.Water; 
					All = 13*16 + 15;
					break;
				// Lava
				case 10:
				case 11:
					DrawMode = DrawMode.Glass; 
					All = 15*16 + 15;
					break;
				case 12: All = 1*16 + 2; break; // Sand
				case 13: All = 1*16 + 3; break; // Gravel
				case 14: All = 2*16 + 0; break; // Gold Ore
				case 15: All = 2*16 + 1; break; // Iron Ore
				case 16: All = 2*16 + 2; break; // Coal Ore
				case 17: All = 1*16 + 5; Sides = 1*16 + 4; break; // TREE
				// Leaves
				case 18: 
					DrawMode = DrawMode.Leaves; 
					All = 3*16 + 4; 
					break;
				case 19: All = 3*16 + 0; break; // Sponge
				// Glass
				case 20: 
					DrawMode = DrawMode.Glass;
					All = 3*16 + 1;
					break;
				case 35: All = 4*16 + 0; break; // Cloth
				// Gold
				case 41:
					Top = 1*16 + 7;
					Sides = 2*16 + 7;
					Bottom = 3*16 + 7;
					break;
				// Iron
				case 42:
					Top = 1*16 + 6;
					Sides = 2*16 + 6; 
					Bottom = 3*16 + 6;
					break;
				case 43: All = 6; Sides = 5; break; 									// Doublestair
				case 44: All = 6; Sides = 5; break; 									// Stair
				case 45: All = 7; break; 												//Brick
				case 46: Top = 9; Sides = 8; Bottom = 10; break; 						// TNT
				case 47: Top = 4; Sides = 2*16 + 3; Bottom = 4; break; 					// Bookshelf
				case 48: All = 2*16 + 4; break; 										// Mossy cobbo
				case 49: All = 2*16 + 5; break; 										// Obsidian
				case 52: DrawMode = DrawMode.Glass; All = 4*16 + 1; break; 				// Mob Spawner
				case 54: All = 1*16 + 9; Sides = 1*16 + 11; break; 						// Chest
				case 56: All = 3*16 + 2; break; 										// Diamond Ore
				case 57: Top = 1*16 + 8; Sides = 2*16 + 8; Bottom = 3*16 + 8; break; 	// Diamond
				case 58: Top = 2*16 + 11; Sides = 3*16 + 11; Bottom = 4; break; 		// Workbench
				case 59: All = 2; Top = 5*16 + 7; break; 								// Soil
				case 60: All = 1; Sides = 2*16 + 12; break;	 							// Furnace
				case 61: All = 1; Sides = 3*16 + 13; break; 							// Lit Furnace
				case 73: All = 3*16 + 3;break; 											// Redstone Ore
				case 79: DrawMode = DrawMode.Glass; All = 4*16 + 3; break; 				// Ice
				case 80: All = 4*16 + 2; break; 										// Snow block
				case 81: Top = 4*16 + 5; Sides = 4*16 + 6; Bottom = 4*16 + 7; break; 	// Cactus
				case 82: All = 4*16 + 8; break; 										// Clay
            }
        }
        public Block()
        {
        }
        public override string ToString()
        {
            return Name;
        }
    }
	public struct BlockSide
	{
		public byte ID { get; private set; }
		
		public double X1 { get { return (double)(ID % 16)/16; } }
		public double Y1 { get { return (double)(ID / 16)/16; } }
		public double X2 { get { return (double)(ID % 16 + 1)/16; } }
		public double Y2 { get { return (double)(ID / 16 + 1)/16; } }
		
		public BlockSide(byte id) : this()
		{
			ID = id;
		}
		
		public static implicit operator BlockSide(byte id)
		{
			return new BlockSide(id);
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
        public static string Version = "11182010";

        /// <summary>
        /// Don't allow saving if past this date.
        /// </summary>
        public static string BrokenBefore = "11182010";

        /// <summary>
        /// Total images for download.
        /// </summary>
        public static int TotalImages = 0;
        public static byte Water=9;

        public static bool Broken=false;

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

        private static long VersionToTicks(string v)
        {
            int month = int.Parse(v.Substring(0, 2));
            int day = int.Parse(v.Substring(2, 2));
            int year = int.Parse(v.Substring(4, 4));
            DateTime dt = new DateTime(year, month, day);
            return dt.Ticks;
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
                if (line.StartsWith("public static string BrokenBefore = "))
                {
                    string v = line.Substring("public static string BrokenBefore = ".Length + 1, 8);
                    Blocks.Broken = (VersionToTicks(Blocks.BrokenBefore) < VersionToTicks(v));
                    return true;
                }
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

        // Since copyboy's getting burnt out...
        // TODO: Make a mirror in case someone bitches.
        public static void UpdateIDs()
        {
            //WebClient wc = new WebClient();
            //wc.Headers.Add("User-Agent: MineEdit/" + Version);
            //wc.Headers.Add("Referer: http://www.minecraftwiki.net/");
            //string derp = wc.DownloadString("http://www.minecraftwiki.net/wiki/Data_values");

            //<tr>
            //   <td>
            //       <a href="/wiki/File:Stone2.png" class="image">
            //           <img alt="Stone2.png" src="/images/thumb/a/a0/Stone2.png/25px-Stone2.png" height="25" width="25">
            //       </a>
            //   </td>
            //   <td>1</td>
            //   <td>01</td>
            //   <td>
            //       <a href="/wiki/Stone" title="Stone">Stone</a>
            //   </td>
            //</tr>
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ValidationType = ValidationType.None;
            xrs.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
            xrs.ConformanceLevel = ConformanceLevel.Auto;
            xrs.ProhibitDtd = false;
            xrs.XmlResolver = null;

            XmlDocument dom = new XmlDocument();
            dom.XmlResolver = null;
            dom.Load(XmlReader.Create("http://www.minecraftwiki.net/wiki/Data_values",xrs));

            foreach (XmlNode node in dom.GetElementsByTagName("tr"))
            {
                if (node.ChildNodes.Count == 4)
                {
                    string imgurl,name;
                    
                    // Image, Decimal, Hex, Name
                    // Yay xpath
                    XmlElement img = (XmlElement)node.SelectSingleNode("/td[1]/a/img");
                    if(img==null) continue;

                    XmlElement xNameParent = (XmlElement)node.SelectSingleNode("/td[4]");
                    if(xNameParent==null) continue;
                    if (xNameParent.ChildNodes[0].Name == "a")
                        name = xNameParent.ChildNodes[0].Value;
                    else if (xNameParent.ChildNodes[0].Name == "i")
                        name = xNameParent.ChildNodes[0].ChildNodes[0].Value;
                    else continue;
                    
                    XmlElement xID = (XmlElement)node.SelectSingleNode("/td[2]");
                    if(xID==null) continue;

                    imgurl = img.Attributes["src"].Value;

                    // Load it up
                    Block nb = new Block();
                    nb.ID = short.Parse(xID.Value);
                    nb.Name = xID.InnerText;
                    nb.FromImage = imgurl;

                    short i = 0;
                    if (Find(nb.Name) != null)
                    {
                        string type = (nb.ID < 255) ? " block" : " (item)";
                        nb.Name += type;
                        if (Find(nb.Name) != null)
                        {
                            while (true)
                            {
                                string nm = nb.Name + " " + (++i).ToString();
                                if (Find(nm) == null)
                                {
                                    nb.Name = nm;
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
        /// <summary>
        /// Update blocks from web.
        /// </summary>
        public static void UpdateBlocks()
        {
            WebClient wc = new WebClient();
            
            string[] derp = wc.DownloadString("http://copy.mcft.net/mc/blocks/").Split(new string[]{"\n","\r\n"},StringSplitOptions.RemoveEmptyEntries);
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
            string[] derp = wc.DownloadString("http://copy.mcft.net/mc/items/").Split(new string[]{"\n","\r\n"},StringSplitOptions.RemoveEmptyEntries);
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
            icondl.Headers.Add("User-Agent: MineEdit/" + Version);
            //icondl.Headers.Add("Referer: http://www.minecraftwiki.net/");
            string f = string.Format("http://copy.mcft.net/mc/icons/{0}.png", nb.ID);
            //string f = nb.FromImage;
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