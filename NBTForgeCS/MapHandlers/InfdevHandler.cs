using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibNbt;
using LibNbt.Tags;
using System.Diagnostics;
namespace MineEdit
{
    class InfdevHandler:IMapHandler
    {
        NbtFile root = new NbtFile();
        NbtFile chunk = new NbtFile();

        private const int ChunkX = 16;
        private const int ChunkY = 16;
        private const int ChunkZ = 128;

        public string Filename {get;set;}
        public int InventoryCapacity { get { return 9 * 4; } }
        private string FilesName;
        private string Folder;

        Dictionary<string, byte[]> ChunkBlocks = new Dictionary<string, byte[]>();
        Dictionary<string, short[,]> Heightmaps = new Dictionary<string, short[,]>();
        Dictionary<string, byte[,]> CachedOverview = new Dictionary<string, byte[,]>();
        Vector3i CurrentBlock = new Vector3i(0, 0, 0);
        byte[] CurrentBlocks;


        public Vector3i ChunkScale { get { return new Vector3i(16, 16, 128); } }
        public Vector3i MapMin { get { return new Vector3i(int.MinValue,0,int.MinValue); } }
        public Vector3i MapMax { get { return new Vector3i(int.MaxValue,0,int.MaxValue); } }

        public void Load()
        {
            Load(Filename);
        }
        public void Load(string filename)
        {
            ChunkBlocks.Clear();
            Heightmaps.Clear();
            CachedOverview.Clear();
            CurrentBlock = new Vector3i(0, 0, 0);
            CurrentBlocks = null;
            Filename = filename;
            Folder=Path.GetDirectoryName(filename);
            root = new NbtFile(filename);
            root.LoadFile();
            LoadChunk(0,0);
        }

        public Chunk GetChunkData(Vector3i chunkpos)
        {
            Chunk c = new Chunk();
            long x = chunkpos.X/ChunkScale.X;
            long y = chunkpos.Y/ChunkScale.Y;
            c.Filename = GetChunkFilename(x, y);
            if(!File.Exists(c.Filename)) return null;
            c.CreationDate = File.GetCreationTime(c.Filename);
            c.Creator = "?";
            for(int _x=0;_x<ChunkX;_x++)
            {
                for(int _z=0;_z<ChunkZ;_z++)
                {
                    int h;
                    byte block;
                    int wd;
                    GetOverview(new Vector3i(_x+(x*ChunkX),_z+(y*ChunkZ),ChunkZ-1), out h, out block, out wd);
                    if(c.MaxHeight<h) c.MaxHeight=h;
                    if(c.MinHeight<h) c.MinHeight=h;
                }
            }
            c.Position = new Vector3i(x, y, 0);
            return c;
        }

        public void GetOverview(Vector3i pos, out int h, out byte block, out int waterdepth)
        {
            h = 0;
            block = 0;
            waterdepth = 0;

            int CX = (int)pos.X / ChunkX;
            int CY = (int)pos.Y / ChunkY;

            int x = (int)pos.X % ChunkX;
            int y = (int)pos.Y % ChunkY;
            int zs = (int)pos.Z % ChunkZ;
            string ci = string.Format("{0},{1}", CX, CY);

            for (int z = zs; z > 0; z++)
            {
                byte b = GetBlockAt(new Vector3i(x, y, z));
                if (b == 8 || b == 9 || b == 52)
                {
                    waterdepth++;
                    continue;
                }
                if (b != 0)
                {
                    block=b;
                    h = z;
                    return;
                }
            }
        }

        public Vector3i GetMousePos(Vector3i mp, int scale, ViewAngle angle)
        {
            Vector3i p = new Vector3i(0,0,0);
            switch(angle)
            {
                case ViewAngle.XY:
                    p.X = mp.X / scale;
                    p.Y = ChunkY-(mp.Y / scale);
                    p.Z = mp.Z;
                    break;
                case ViewAngle.XZ:
                    p.X = mp.X/scale;
                    p.Y = mp.Y/scale;
                    p.Z = mp.Z; // wut 
                    break;
                case ViewAngle.YZ:
                    p.X=mp.Z;
                    p.Y=mp.Y/scale;
                    p.Z=mp.X/scale;
                    break;
            }
            return p;
        }

        private byte[] LoadChunk(Int64 x, Int64 z)
        {
            byte[] CurrentBlocks;
            /*
            if (CurrentBlock.X == x && CurrentBlock.Z == z)
            {
                Console.WriteLine("Already loaded...!");
                return null;
            }
            */
            CurrentBlock.X = x;
            CurrentBlock.Z = z;
            string derp;
            string f = GetChunkFilename(x, z);
            if (!File.Exists(f))
            {
                Console.WriteLine("! {0}", f);
                return null;
            }
            chunk = new NbtFile(f);
            chunk.LoadFile();
            //Console.WriteLine("({0},{1}) Loading {2} layers of {3}", x, z, ChunkZ, f);
            //Console.WriteLine(chunk.RootTag.ToString());
            NbtCompound level = (NbtCompound)chunk.RootTag["Level"];
            //Console.WriteLine(f);

            NbtByteArray tba = (NbtByteArray)level["Blocks"];
            byte[] CurrentChunkBlocks = tba.Value;
            byte bb=0;
            foreach (byte b in CurrentChunkBlocks)
                if (bb < b) bb = b;
            CurrentBlocks=CurrentChunkBlocks;
            
            string ci = string.Format("{0},{1}", x, z);
            if (!ChunkBlocks.ContainsKey(ci))
                ChunkBlocks.Remove(ci);
            ChunkBlocks.Add(ci, CurrentBlocks);
            Console.WriteLine("Loaded {0} bytes from chunk {1} (biggest byte = 0x{2:X2}).", CurrentBlocks.Length, ci,bb);
            /*StackTrace stack = new StackTrace();
            StackFrame[] stackFrames = stack.GetFrames();  // get method calls (frames)

            // write call stack method names
            foreach (StackFrame stackFrame in stackFrames)
            {
                Console.WriteLine(stackFrame.GetMethod());   // write method name
            }
            */
            return CurrentBlocks;
        }

        private string GetChunkFilename(Int64 x, Int64 z)
        {

            //Int64 X = x / 16;
            //Int64 Z = z / 16;
            Int64 fX = x & 0x3f;
            Int64 fZ = z & 0x3f;

            return 
                Path.Combine(Folder,
                    Path.Combine(Base36.NumberToBase36(fX),
                        Path.Combine(Base36.NumberToBase36(fZ),
                            string.Format("c.{0}.{1}.dat", Base36.NumberToBase36(x), Base36.NumberToBase36(z))
                        )
                    )
                );
        }
        private long Base36Decode(string inputString)
        {
            string clist = "0123456789abcdefghijklmnopqrstuvwxyz";

            inputString = Reverse(inputString.ToLower());
            long result = 0;
            int pos = 0;
            foreach (char c in inputString)
            {
                result += clist.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }

        private string Base36Encode(long inputNumber)
        {
            char[] clist = new char[] { '0', '1', '2', '3', '4',
                                '5', '6', '7', '8', '9',
                                'a', 'b', 'c', 'd', 'e',
                                'f', 'g', 'h', 'i', 'j',
                                'k', 'l', 'm', 'n', 'o',
                                'p', 'q', 'r', 's', 't',
                                'u', 'v', 'w', 'x', 'y',
                                'z' };

            StringBuilder sb = new StringBuilder();
            while (inputNumber != 0)
            {
                sb.Append(clist[inputNumber % 36]);
                inputNumber /= 36;
            }
            return Reverse(sb.ToString());
        }

        private string Reverse(string input)
        {
            Stack<char> resultStack = new Stack<char>();
            foreach (char c in input)
            {
                resultStack.Push(c);
            }

            StringBuilder sb = new StringBuilder();
            while (resultStack.Count > 0)
            {
                sb.Append(resultStack.Pop());
            }
            return sb.ToString();
        }
        private void SaveChunk(int x, int z,byte[,,] data)
        {

            string f = GetChunkFilename(x, z);
            if (!File.Exists(f))
            {
                Console.WriteLine("! {0}", f);
                return;
            }
            chunk= new NbtFile(f);
            Console.WriteLine(f);

            NbtCompound Level = new NbtCompound("Level");
            bool found = false;
            byte[] lolblocks=new byte[ChunkX*ChunkY*ChunkZ];
            for(int _x=0;_x<ChunkX;_x++)
            {
                for(int _y=0;_y<ChunkY;_y++)
                {
                    for(int _z=0;_z<ChunkY;_z++)
                    {
                        long index = _x + (_z * ChunkY + _y) * ChunkZ;
                        lolblocks[index]=data[_x,_y,_z];
                    }
                }
            }
            Level["Blocks"] = new NbtByteArray("Blocks",lolblocks);
            if (found)
            {
                chunk.RootTag.Tags[0] = Level;
                chunk.SaveFile(f);
            }
        }

        public bool IsMyFiletype(string f)
        {
            return Path.GetFileName(f).Equals("level.dat");
        }
            //Convert number in string representation from base:from to base:to. 
        //Return result as a string
        public static String BaseConv(int from, int to, String s)
        {
            //Return error if input is empty
            if (String.IsNullOrEmpty(s))
            {
                return ("Error: Nothing in Input String");
            }
            //only allow uppercase input characters in string
            s = s.ToUpper();
        
            //only do base 2 to base 36 (digit represented by characters 0-Z)"
            if (from <= 2 || from >= 36 || to <= 2 || to >= 36) 
		    { return ("Base requested outside range"); }
        
            //convert string to an array of integer digits representing number in base:from
            int il = s.Length;
            int[] fs = new int[il];
            int k = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] >= '0' && s[i] <= '9') { fs[k++] = (int)(s[i] - '0'); }
                else
                {
                    if (s[i] >= 'A' && s[i] <= 'Z') { fs[k++] = 10 + (int)(s[i] - 'A'); }
                    else
                    { return ("Error: Input string must only contain any of 0-9 or A-Z"); } //only allow 0-9 A-Z characters
                }
            }
        
            //check the input for digits that exceed the allowable for base:from
            foreach(int i in fs)
            {
                if (i >= from) { return ("Error: Not a valid number for this input base"); }
            }
        
            //find how many digits the output needs
            int ol = il * (from / to+1);
            int[] ts = new int[ol+10]; //assign accumulation array
            int[] cums = new int[ol+10]; //assign the result array
            ts[0] = 1; //initialize array with number 1 
        
            //evaluate the output
            for (int i = 0; i < il; i++) //for each input digit
            {
                for (int j = 0; j < ol; j++) //add the input digit 
				    // times (base:to from^i) to the output cumulator
                {
                    cums[j] += ts[j] * fs[i];
                    int temp = cums[j];
                    int rem = 0;
                    int ip = j;
                    do // fix up any remainders in base:to
                    {
                        rem = temp / to;
                        cums[ip] = temp-rem*to; ip++;
                        cums[ip] += rem;
                        temp = cums[ip];
                    }
                    while (temp >=to);
                }
            
                //calculate the next power from^i) in base:to format
                for (int j = 0; j < ol; j++)
                {
                    ts[j] = ts[j] * from;
                } 
                for(int j=0;j<ol;j++) //check for any remainders
                {
                    int temp = ts[j];
                    int rem = 0;
                    int ip = j;
                    do  //fix up any remainders
                    {
                        rem = temp / to;
                        ts[ip] = temp - rem * to; ip++;
                        ts[ip] += rem;
                        temp = ts[ip];
                    }
                    while (temp >= to);
                }
            }
        
            //convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
            String sout = String.Empty; //initialize output string
            bool first = false; //leading zero flag
            for (int i = ol ; i >= 0; i--)
            {
                if (cums[i] != 0) { first = true; }
                if (!first) { continue; }
                if (cums[i] < 10) { sout += (char)(cums[i] + '0'); }
                else { sout += (char)(cums[i] + 'A'-10); }
            }
            if (String.IsNullOrEmpty(sout)) { return "0"; } //input was zero, return 0
            //return the converted string
            return sout;
        }

        public bool Save()
        {
            /*
            foreach (KeyValuePair<string, byte[,,]> vp in ChunkBlocks)
            {
                string[] cc = vp.Key.Split(',');
                int x = int.Parse(cc[0]);
                int z = int.Parse(cc[1]);
                SaveChunk(x, z, vp.Value);
            }
            */
            File.Copy(Filename, Path.ChangeExtension(Filename, "bak"),true);
            root.SaveFile(Filename);
            return true;
        }

        public bool Save(string filename)
        {
            File.Copy(Filename, Path.ChangeExtension(Filename, "bak"),true);
            root.SaveFile(filename);
            return true;
        }

        public int Height
        {
            get
            {
                return 100000000;
            }
            protected set
            {
                return;
            }
        }

        public int Width
        {
            get
            {
                return 100000000;
            }
            protected set
            {
                return;
            }
        }

        public int Depth
        {
            get
            {
                return 128;
            }
            protected set
            {
                return;
            }
        }

        public int Health
        {
            get
            {
                // /Player/Health;
                NbtShort h = (NbtShort)((root.RootTag["Player"] as NbtCompound)["Health"]);
                return (int)h.Value;
            }
            set
            {
                NbtShort h = new NbtShort((short)value);
                NbtCompound p = (NbtCompound)root.RootTag["Player"];
                p["Health"] = h;
                root.RootTag["Player"] = p;
            }
        }

        public int Air
        {
            get
            {
                // /Player/Air;
                NbtShort h = (NbtShort)((root.RootTag["Player"] as NbtCompound)["Air"]);
                return (int)h.Value;
            }
            set
            {
                NbtShort h = new NbtShort((short)value);
                NbtCompound p = (NbtCompound)root.RootTag["Player"];
                p["Air"] = h;
                root.RootTag["Player"] = p;
            }
        }

        public int Fire
        {
            get
            {
                // /Player/Fire;
                NbtShort h = (NbtShort)((root.RootTag["Player"] as NbtCompound)["Fire"]);
                return (int)h.Value;
            }
            set
            {
                NbtShort h = new NbtShort((short)value);
                NbtCompound p = (NbtCompound)root.RootTag["Player"];
                p["Fire"] = h;
                root.RootTag["Player"] = p;
            }
        }

        public Vector3d PlayerPos
        {
            get
            {
                // /Player/Pos/;
                NbtList h = (NbtList)((root.RootTag["Player"] as NbtCompound)["Pos"]);
                Vector3d pos = new Vector3d();
                pos.X = (h[0] as NbtDouble).Value;
                pos.Y = (h[1] as NbtDouble).Value;
                pos.Z = (h[2] as NbtDouble).Value;
                return pos;
            }
            set
            {
                NbtList h = new NbtList("Pos");
                h.Tags.Add(new NbtDouble(value.X));
                h.Tags.Add(new NbtDouble(value.Y));
                h.Tags.Add(new NbtDouble(value.Z));
                NbtCompound p = (NbtCompound)root.RootTag["Player"];
                p["Pos"] = h;
                root.RootTag["Player"] = p;
            }
        }

        public Vector3i Spawn
        {
            get
            {
                return new Vector3i((root.RootTag["SpawnX"] as NbtInt).Value, (root.RootTag["SpawnY"] as NbtInt).Value, (root.RootTag["SpawnZ"] as NbtInt).Value);
            }
            set
            {
                root.RootTag["SpawnX"] = new NbtInt((int)value.X);
                root.RootTag["SpawnY"] = new NbtInt((int)value.Y);
                root.RootTag["SpawnZ"] = new NbtInt((int)value.Z);
            }
        }

        int BlockCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// TODO: Negative chunks produce all kinds of strange artifacts.
        public byte GetBlockAt(Vector3i p)
        {
            //Console.WriteLine("{0}", p);
            int CX = (int)(p.X / (long)ChunkX);
            int CZ = (int)(p.Y / (long)ChunkY);

            int x = ((int)p.X % ChunkX) & 0xf;
            int y = ((int)p.Y % ChunkY) & 0xf;
            int z = (int)p.Z;// % ChunkZ;

            if (
                !Check(x, -1, ChunkX) || 
                !Check(y, -1, ChunkY) || 
                !Check(z, -1, ChunkZ))
            {
                //Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
                return 0x00;
            }


            // X Y Z    = Me
            // X Z Y ?  = Notch
            int index = x << 11 | y << 7 | z;

            //if (CX == -1)
            //    Console.WriteLine("Accessing <{0},{1},{2}> of chunk ({3},{4}) - Index {5}/{6} ", x, y, z, CX, CZ, index, ChunkX * ChunkY * ChunkZ);

            string ci = string.Format("{0},{1}", CX, CZ);
            //try
            //{
                if (ChunkBlocks.ContainsKey(ci))
                    return ChunkBlocks[ci][index];
            //}
            //catch (Exception)
            //{
            //    return 0x00;
            //}
            byte[] Blox = LoadChunk(CX,CZ);

            try
            {
                return Blox[index];
            }
            catch (Exception)
            {
                return 0x00;
            }
        }

        private bool Check(long x, long min, long max)
        {
            return (x > min && x < max);
        }

        public void SetBlockAt(Vector3i p, byte id)
        {
            //Console.WriteLine("{0}", p);
            int CX = (int)(p.X / (long)ChunkX);
            int CZ = (int)(p.Y / (long)ChunkY);

            int x = ((int)p.X % ChunkX) & 0xf;
            int y = ((int)p.Y % ChunkY) & 0xf;
            int z = (int)p.Z;// % ChunkZ;

            if (
                !Check(x, -1, ChunkX) ||
                !Check(y, -1, ChunkY) ||
                !Check(z, -1, ChunkZ))
            {
                //Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
                return;
            }


            // X Y Z    = Me
            // X Z Y ?  = Notch
            int index = x << 11 | y << 7 | z;

            string ci = string.Format("{0},{1}", CX, CZ);

            LoadChunk(CX, CZ);

            if(!ChunkBlocks.ContainsKey(ci))
               ChunkBlocks.Add(ci,CurrentBlocks);
            ChunkBlocks[ci]=CurrentBlocks;
        }

        public int InventoryColumns
        {
            get
            {
                return 9;
            }
        }
        public int InventoryOnHandCapacity
        {
            get
            {
                return 9;
            }
        }

        public bool GetInventory(int slot, out short itemID, out int Damage, out int Count, out string failreason)
        {
            itemID = 0;
            Damage = 0;
            Count = 0;
            failreason = "No error.";
            // /Player/Inventory/
            if (root == null)
            {
                failreason = "root==null";
                return false;
            }
            if (root.RootTag == null)
            {
                failreason = "root.RootTag==null";
                return false;
            }
            NbtCompound Data = (NbtCompound)root.RootTag["Data"];
            NbtCompound Player = (NbtCompound)Data["Player"];
            NbtList pi = (NbtList)Player["Inventory"];
            foreach (NbtTag t in pi.Tags)
            {
                NbtCompound s = (NbtCompound)t;
                if ((s["Slot"] as NbtByte).Value == (byte)slot)
                {
                    Count = (int)(s["Count"] as NbtByte).Value;
                    itemID = (s["id"] as NbtShort).Value;
                    Damage = (int)(s["Damage"] as NbtShort).Value;
                    return true;
                }
            }
            failreason = "Slot is empty.";
            return false;
        }

        public bool SetInventory(int slot, short itemID, int Damage, int Count)
        {
            // /Player/Inventory/
            NbtCompound Slot = new NbtCompound();
            Slot.Tags.Add(new NbtByte("Count",(byte)Count));
            Slot.Tags.Add(new NbtShort("id",(short)itemID));
            Slot.Tags.Add(new NbtShort("Damage",(short)Damage));
            Slot.Tags.Add(new NbtByte("Slot", (byte)slot));

            NbtCompound Data = (NbtCompound)root.RootTag["Data"];
            NbtCompound Player = (NbtCompound)Data["Player"];
            NbtList pi = (NbtList)Player["Inventory"];
            bool Found = false;
            for(int i=0;i<pi.Tags.Count;i++)
            {
                if (((pi[i] as NbtCompound)["Slot"] as NbtByte).Value == (byte)slot)
                {
                    Found = true;
                    pi[i] = Slot;
                }
            }
            if(!Found)
                pi.Tags.Add(Slot);
            Player["Inventory"] = pi;
            Data["Player"] = Player;
            root.RootTag["Data"] = Data;
            return true;
        }

        public void ClearInventory()
        {
            NbtCompound Data = (NbtCompound)root.RootTag["Data"];
            NbtCompound Player = (NbtCompound)Data["Player"];
            NbtList pi = (NbtList)Player["Inventory"];
            Player["Inventory"] = pi;
            Data["Player"] = Player;
            root.RootTag["Data"] = Data;
        }
        public void Repair()
        {
            throw new NotImplementedException();
        }
    }
}
