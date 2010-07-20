using System;
using System.Collections.Generic;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;
using OpenMinecraft.MapGenerators;

namespace OpenMinecraft
{
    public class InfdevHandler:IMapHandler
    {
        public event CorruptChunkHandler CorruptChunk;

        NbtFile root = new NbtFile();
        NbtFile chunk = new NbtFile();

        private const int ChunkX = 16;
        private const int ChunkY = 16;
        private const int ChunkZ = 128;

        public string Filename {get;set;}
        public int InventoryCapacity { get { return 9*4; } }
        private string FilesName;
        private string Folder;

        Dictionary<string, Chunk> Chunks = new Dictionary<string, Chunk>();

        List<string> ChangedChunks = new List<string>();

        // Using UUIDs since they're globally unique and native to C#.
        Dictionary<Guid, Entity> _Entities = new Dictionary<Guid, Entity>();
        Dictionary<Guid, TileEntity> _TileEntities = new Dictionary<Guid, TileEntity>();

        Vector3i CurrentBlock = new Vector3i(0, 0, 0);
        byte[] CurrentBlocks;


        public Vector3i ChunkScale { get { return new Vector3i(16, 16, 128); } }
        public bool HasMultipleChunks { get { return true; } }
        //public Vector3i MapMin { get { return new Vector3i(int.MinValue, int.MinValue, 0); } }
        //public Vector3i MapMax { get { return new Vector3i(int.MaxValue, int.MaxValue, 128); } }
        public Vector3i MapMin { get { return new Vector3i(-1000, -1000, 0); } }
        public Vector3i MapMax { get { return new Vector3i(1000, 1000, 127); } }

        /// <summary>
        /// BROKEN:  infdev currently resets this to 0,64,0
        /// </summary>
        public Vector3i Spawn
        {
            get
            {
                NbtCompound Data = (NbtCompound)root.GetTag("/Data");
                return new Vector3i(
                    (Data["SpawnX"] as NbtInt).Value,
                    (Data["SpawnY"] as NbtInt).Value,
                    (Data["SpawnZ"] as NbtInt).Value);
            }

            set
            {
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                Player.Tags.Remove(Player["SpawnX"]);
                Player.Tags.Remove(Player["SpawnY"]);
                Player.Tags.Remove(Player["SpawnZ"]);
                Player.Tags.Add(new NbtInt("SpawnX", (int)value.X));
                Player.Tags.Add(new NbtInt("SpawnY", (int)Utils.Clamp(value.Y, 0, 128)));
                Player.Tags.Add(new NbtInt("SpawnZ", (int)value.Z));
                // BROKEN root.SetTag("/Data/Player", h);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;
            }
        }



        public int Height
        {
            get
            {
                return 10000;
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
                return 10000;
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

        /// <summary>
        /// -1 = dead?
        /// 0 = dead
        /// 20 = full health
        /// </summary>
        public int Health
        {
            get
            {
                // /Player/Health;
                NbtShort h = (NbtShort)root.GetTag("/Data/Player/Health");
                return (int)h.Value;
            }
            set
            {
                // Clamp value to between -1 and 21
                NbtShort h = new NbtShort("Health", (short)Utils.Clamp(value, -1, 21));
                //root.SetTag("/Data/Player/Health",h);
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                NbtShort oh = (NbtShort)Player["Health"];
                Console.WriteLine("Health: {0}->{1}", oh.Value, h.Value);
                Player.Tags.Remove(oh);
                Player.Tags.Add(h);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;
            }
        }

        // Set to 0 to avoid the Laying Down bug.
        public int HurtTime
        {
            get
            {
                NbtShort h = (NbtShort)root.GetTag("/Data/Player/HurtTime");
                return (int)h.Value;
            }
            set
            {
                // TODO: Find value range.
                NbtShort h = new NbtShort("HurtTime", (short)/*Utils.Clamp(*/value/*, 0, 21)*/);
                //root.SetTag("/Data/Player/HurtTime",h);
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                NbtShort oh = (NbtShort)Player["HurtTime"];
                Player.Tags.Remove(oh);
                Player.Tags.Add(h);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;
            }
        }
        // Don't clamp, all sorts of weird shit can be done here.
        public int Air
        {
            get
            {
                // /Player/Air;
                NbtShort h = (NbtShort)root.GetTag("/Data/Player/Air");
                return (int)h.Value;
            }
            set
            {
                NbtShort h = new NbtShort("Air", (short)value);
                //root.SetTag("/Data/Player/Air", h); ;

                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                Player.Tags.Remove(Player["Air"]);
                Player.Tags.Add(h);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;
            }
        }

        // Dunno the range
        public int Fire
        {
            get
            {
                NbtShort h = (NbtShort)root.GetTag("/Data/Player/Fire");
                return (int)h.Value;
            }
            set
            {
                NbtShort f = new NbtShort("Fire", (short)value);
                // BROKEN root.SetTag("/Data/Player/Fire", h);
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                Player.Tags.Remove(Player["Fire"]);
                Player.Tags.Add(f);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;
            }
        }

        // Dunno the range
        public int Time
        {
            get
            {
                NbtLong h = (NbtLong)root.GetTag("/Data/Time");
                return (int)h.Value;
            }
            set
            {
                NbtLong f = new NbtLong("Time", (short)value);
                // BROKEN root.SetTag("/Data/Time", h);
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                Data.Tags.Remove(Data["Time"]);
                Data.Tags.Add(f);
                root.RootTag["Data"] = Data;
            }
        }

        public Vector3d PlayerPos
        {
            get
            {
                NbtList h = (NbtList)root.GetTag("/Data/Player/Pos");
                Vector3d pos = new Vector3d();
                pos.X = (h[0] as NbtDouble).Value;
                pos.Y = (h[1] as NbtDouble).Value;
                pos.Z = (h[2] as NbtDouble).Value;
                //Console.WriteLine("Player is on chunk {0}.",GetChunkFilename((int)pos.X >> 4,(int)pos.Z >> 4));
                return pos;
            }
            set
            {
                NbtList h = new NbtList("Pos");
                h.Tags.Add(new NbtDouble(value.X));
                h.Tags.Add(new NbtDouble(Utils.Clamp(value.Y, 0, 128)));
                h.Tags.Add(new NbtDouble(value.Z));
                // BROKEN root.SetTag("/Data/Player/Pos", h);
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                NbtCompound Player = (NbtCompound)Data["Player"];
                Player.Tags.Remove(Player["Pos"]);
                Player.Tags.Add(h);
                Data["Player"] = Player;
                root.RootTag["Data"] = Data;

            }
        }

        public Dictionary<Guid, Entity> Entities
        {
            get
            {
                return _Entities;
            }
        }

        public Dictionary<Guid, TileEntity> TileEntities
        {
            get
            {
                return _TileEntities;
            }
        }

        public bool Snow
        {
            get
            {
                NbtByte SnowCovered = (NbtByte)root.GetTag("/Data/SnowCovered");
                if (SnowCovered == null) return false;
                return SnowCovered.Value == (byte)1;
            }
            set
            {
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                try
                {
                    Data.Tags.Remove(Data["SnowCovered"]);
                }
                catch (Exception) { }
                byte v = 0;
                if (value == true) v = 1;
                Console.WriteLine("Map is now {0}.", (value) ? "covered in snow" : "normal");
                Data.Tags.Add(new NbtByte("SnowCovered", v));
                root.RootTag["Data"] = Data;
            }
        }

        public long RandomSeed
        {
            get
            {
                return root.GetTag("/Data/RandomSeed").asLong();
            }
            set
            {
                NbtCompound Data = (NbtCompound)root.RootTag["Data"];
                try
                {
                    Data.Tags.Remove(Data["RandomSeed"]);
                }
                catch (Exception) { }
                Data.Tags.Add(new NbtLong("RandomSeed", value));
                root.RootTag["Data"] = Data;
            }
        }

        public void Load()
        {
            Load(Filename);
        }
        public void Load(string filename)
        {
            Chunks.Clear();
            CurrentBlock = new Vector3i(0, 0, 0);
            CurrentBlocks = null;
            Filename = filename;
            Folder=Path.GetDirectoryName(filename);
            root = new NbtFile(filename);
            root.LoadFile();
            LoadChunk(0,0);
        }


        public Chunk GetChunk(Vector3i chunkpos)
        {
            return GetChunk(chunkpos.X, chunkpos.Y);
        }
        public void GetOverview(int CX,int CY,Vector3i pos, out int h, out byte block, out int waterdepth)
        {
            h = 0;
            block = 0;
            waterdepth = 0;

            int x = (int)pos.X;
            int y = (int)pos.Y;
            //int z = (int)pos.Z;// % ChunkZ;
            string ci = string.Format("{0},{1}", CX, CY);

            for (int z = (int)pos.Z; z > 0; --z)
            {
                byte b = GetBlockIn(CX,CY,new Vector3i(x, y, z));
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
                case ViewAngle.FrontSlice:
                    p.X = mp.X / scale;
                    p.Y = ChunkY-(mp.Y / scale);
                    p.Z = mp.Z;
                    break;
                case ViewAngle.TopDown:
                    p.X = mp.X/scale;
                    p.Y = mp.Y/scale;
                    p.Z = mp.Z; // wut 
                    break;
                case ViewAngle.SideSlice:
                    p.X=mp.Z;
                    p.Y=mp.Y/scale;
                    p.Z=mp.X/scale;
                    break;
            }
            return p;
        }

        private byte[,,] DecompressLighting(byte[] databuffer)
        {
            byte[, ,] blocks = new byte[ChunkX, ChunkY, ChunkZ];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 128; z++)
                    {

                        int half = x * 64 + y * 64 * 16 + z / 2;
                        int l;
                        if (z % 2 == 0)
                            l = (int)databuffer[half] % 16;
                        else
                            l = (int)databuffer[half] / 16;
                        blocks[x, y, z]=(byte)l;
                    }
                }
            }
            return blocks;
        }

        private Chunk _LoadChunk(int x, int z)
        {
            byte[] CurrentBlocks;
            /*
            if (CurrentBlock.X == x && CurrentBlock.Z == z)
            {
                Console.WriteLine("Already loaded...!");
                return null;
            }
            */
            //x = (x < 0) ? x - 2 : x;
            //z = (z < 0) ? z - 2 : z;
            CurrentBlock.X = x;
            CurrentBlock.Z = z;
            string derp;
            Chunk c = new Chunk(this);
            c.Loading = true;
            c.Filename = GetChunkFilename(x, z);
            c.CreationDate = File.GetCreationTime(c.Filename);
            c.Creator = "?";
            c.Size = ChunkScale;
            if (!File.Exists(c.Filename))
            {
                Console.WriteLine("! {0}", c.Filename);
                return null;
            }
            try
            {
                chunk = new NbtFile(c.Filename);
                chunk.LoadFile();
            }
            catch (Exception e)
            {
                string err = string.Format(" *** ERROR: Chunk {0},{1} ({2}) failed to load:\n\n{3}", x, z, c.Filename, e);
                Console.WriteLine(err);
                if(CorruptChunk!=null)
                    CorruptChunk(err, c.Filename);
                return null;
            }

            NbtCompound level = (NbtCompound)chunk.RootTag["Level"];

            c.Position = new Vector3i(
                level["xPos"].asInt(),
                level["zPos"].asInt(), 0);

            if ((int)c.Position.X != x || (int)c.Position.Y != z)
            {
                throw new Exception(string.Format("Chunk pos is wrong.  {0}!={1}", c.Filename, c.Position));
            }
            NbtList TileEntities = (NbtList)level["TileEntities"];
            if (TileEntities.Tags.Count > 0)
            {
                //Console.WriteLine("*** Found TileEntities.");
                LoadTileEnts((int)x, (int)z, TileEntities);
            }

            NbtList Entities = (NbtList)level["Entities"];
            if (Entities.Tags.Count > 0)
            {
                //Console.WriteLine("*** Found Entities.");
                LoadEnts((int)x, (int)z, Entities);
            }

            // Blocks
            c.Blocks = DecompressBlocks(level["Blocks"].asBytes());

            // Block Lighting
            c.BlockLight = DecompressLighting(level["BlockLight"].asBytes());

            // Sky Lighting
            c.SkyLight = DecompressLighting(level["SkyLight"].asBytes());

            c.Loading = false;
            c.UpdateOverview();

            string ci = string.Format("{0},{1}", x, z);
            if (Chunks.ContainsKey(ci))
                return Chunks[ci];
            Chunks.Add(ci, c);
            /*
                @TODO: Make Pig spawner converter.
            for (int Z = 0; Z < ChunkScale.X; Z++)
            {
                for (int Y = 0; Y < ChunkScale.Y; Y++)
                {
                    for (int X = 0; X < ChunkScale.X; X++)
                    {
                        long index = X + (Z * ChunkY + Y) * ChunkZ;
                        byte b = CurrentChunks[index];
                        if (b == Blocks.Find("Mob spawner").ID)
                        {
                            MobSpawner ms = new MobSpawner(X + (int)(x * ChunkScale.X), Y + (int)(z * ChunkScale.Y), Z, "Pig", 20);
                            ms.id = "MobSpawner";
                            ms.UUID = Guid.NewGuid();
                            _TileEntities.Add(ms.UUID, ms);
                            c++;
                        }
                    }
                }
            }
                */
            //if (c>0)  Console.WriteLine("*** {0} spawners found.", c);
            //Console.WriteLine("Loaded {0} bytes from chunk {1}.", CurrentChunks.Length, c.Filename);
            return c;
        }

        private byte[, ,] DecompressBlocks(byte[] p)
        {
            byte[, ,] NewBlocks = new byte[ChunkX, ChunkY, ChunkZ];
            for (int x = 0; x < ChunkScale.X; x++)
            {
                for (int y = 0; y < ChunkScale.Y; y++)
                {
                    for (int z = 0; z < ChunkScale.Z; z++)
                    {
                        NewBlocks[x, y, z] = p[GetBlockIndex(x, y, z)];
                    }
                }
            }
            return NewBlocks;
        }

        private void LoadEnts(int CX, int CY, NbtList ents)
        {
            foreach (NbtCompound c in ents.Tags)
            {
                Entity hurp = Entity.GetEntity(c);
                //hurp.Pos.X += ((double)CX*16d);
                //hurp.Pos.Y += ((double)CY*16d);
                hurp.UUID = Guid.NewGuid();
                _Entities.Add(hurp.UUID,hurp);
            }
        }

        private void LoadTileEnts(int CX,int CY,NbtList ents)
        {
            foreach (NbtCompound c in ents.Tags)
            {
                TileEntity hurp = TileEntity.GetEntity(CX,CY,(int)ChunkScale.X,c);
                int _CX = (int)(hurp.Pos.X / 16);
                int _CY = (int)(hurp.Pos.Y / 16);
                if (_CX != CX || _CY != CY)
                {
                    Console.WriteLine("TileEntity at {4} is not in chunk {0},{1};  It's in chunk {2},{3}!", CX, CY, _CX, _CY, hurp.Pos);
                    //Environment.Exit(0);
                }
                hurp.UUID = Guid.NewGuid();
                _TileEntities.Add(hurp.UUID, hurp);
            }
        }
        
        public void SetTileEntity(TileEntity e)
        {
            long CX=e.Pos.X/16;
            long CY=e.Pos.Y/16;
            string f = GetChunkFilename((int)CX, (int)CY);

            try
            {
                chunk = new NbtFile(f);
                chunk.LoadFile();
                NbtCompound level = (NbtCompound)chunk.RootTag["Level"];

                NbtList tents = (NbtList)level["TileEntities"];
                int found = -1;
                for(int i = 0;i<tents.Tags.Count;i++)
                {
                    TileEntity te = new TileEntity((NbtCompound)tents[i]);
                    if (te.Pos == e.Pos)
                    {
                        found = i;
                    }
                }
                if (found > -1)
                    tents[found] = e.ToNBT();
                else
                    tents.Tags.Add(e.ToNBT());

                level["TileEntities"] = tents;
                chunk.RootTag["Level"] = level;
                chunk.SaveFile(f);
            }
            catch (Exception) { }
        }

        public void RemoveTileEntity(TileEntity e)
        {
            long CX = e.Pos.X / 16;
            long CY = e.Pos.Y / 16;
            string f = GetChunkFilename((int)CX, (int)CY);

            try
            {
                chunk = new NbtFile(f);
                chunk.LoadFile();
                NbtCompound level = (NbtCompound)chunk.RootTag["Level"];

                NbtList TileEntities = (NbtList)level["TileEntities"];
                int found = -1;
                for (int i = 0; i < TileEntities.Tags.Count; i++)
                {
                    TileEntity te = new TileEntity((NbtCompound)TileEntities[i]);
                    if (te.Pos == e.Pos)
                    {
                        found = i;
                    }
                }
                if (found > -1)
                    TileEntities.Tags.RemoveAt(found);

                level["TileEntities"] = TileEntities;
                chunk.RootTag["Level"] = level;
                chunk.SaveFile(f);
            }
            catch (Exception) { }
        }

        private string GetChunkFilename(int x, int z)
        {
            string file = "c." + Utils.IntToBase(x, 36) + "." + Utils.IntToBase(z, 36) + ".dat";
            string dirX = Utils.IntToBase(x & 0x3F, 36);
            string dirY = Utils.IntToBase(z & 0x3F, 36);
            string f= 
                Path.Combine(Folder,
                    Path.Combine(dirX,
                        Path.Combine(dirY,
                            file
                        )
                    )
                );
            //Console.WriteLine("{0},{1} = {2}",x,z,f);
            return f;
        }
        private void SaveChunk(int x, int z)
        {
            Chunk c = GetChunk(x,z,false);
            if(c!=null)
                return;
            SaveChunk(c);
        }

        public Chunk GetChunk(int x, int z, bool GenerateNewChunkIfNeeded=true)
        {
            string id = x.ToString() + "," + z.ToString();
            Chunk c;
            if (!Chunks.TryGetValue(id, out c))
            {
                if (GenerateNewChunkIfNeeded)
                {
                    Generate(GeneratorAlgorithm, x, z);
                    return GetChunk(x, z);
                }
                return null;
            }
            return c;
        }
        public void SaveChunk(Chunk cnk)
        {
            NbtFile c = new NbtFile(cnk.Filename);
            c.LoadFile();
            //Console.WriteLine("Saving "+f);

            NbtCompound Level = (NbtCompound)c.RootTag["Level"];
            //string ci = string.Format("{0},{1}", x, z);
            Level.Tags.Remove(Level["Blocks"]);

            // BLOCKS /////////////////////////////////////////////////////
            byte[] blocks = new byte[ChunkX*ChunkY*ChunkZ];
            for(int X = 0;X < ChunkX;X++)
            {
                for(int Y = 0;Y < ChunkY;Y++)
                {
                    for(int Z = 0;Z < ChunkZ;Z++)
                        blocks[GetBlockIndex(X,Y,Z)] = cnk.Blocks[X,Y,Z];
                }
            }
            Level.Tags.Add(new NbtByteArray("Blocks", blocks));
            blocks = null;

            // LIGHTING ///////////////////////////////////////////////////
            byte[] lighting = new byte[ChunkX * ChunkY * ChunkZ];
            for (int X = 0; X < ChunkX; X++)
            {
                for (int Y = 0; Y < ChunkY; Y++)
                {
                    for (int Z = 0; Z < ChunkZ; Z++)
                        lighting[GetBlockIndex(X, Y, Z)] = cnk.SkyLight[X, Y, Z];
                }
            }
            Level.Tags.Add(new NbtByteArray("SkyLight", lighting));

            lighting = new byte[ChunkX * ChunkY * ChunkZ];
            for (int X = 0; X < ChunkX; X++)
            {
                for (int Y = 0; Y < ChunkY; Y++)
                {
                    for (int Z = 0; Z < ChunkZ; Z++)
                        lighting[GetBlockIndex(X, Y, Z)] = cnk.BlockLight[X, Y, Z];
                }
            }
            Level.Tags.Add(new NbtByteArray("BlockLight", lighting));

            c.RootTag["Level"] = Level;
            c.SaveFile(cnk.Filename);
        }

        public bool IsMyFiletype(string f)
        {
            return Path.GetFileName(f).Equals("level.dat");
        }

        public bool Save()
        {
            /*
            foreach (KeyValuePair<string, byte[,,]> vp in Chunks)
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
        public void SetEntity(Entity e)
        {
            Guid ID = e.UUID;

            if (_Entities.ContainsKey(ID))
                _Entities.Remove(ID);
            _Entities.Add(ID, e);

            int CX = (int)e.Pos.X / 16;
            int CY = (int)e.Pos.Y / 16;
            e.Pos.X = (int)e.Pos.X - CX;
            e.Pos.Y = (int)e.Pos.Y - CY;

            string f = GetChunkFilename(CX, CY);
            if (!File.Exists(f))
            {
                Console.WriteLine("! {0}", f);
                return;
            }
            try
            {
                chunk = new NbtFile(f);
                chunk.LoadFile();
                NbtCompound level = (NbtCompound)chunk.RootTag["Level"];

                NbtList ents = (NbtList)level["Entities"];
                if (e.OrigPos != null)
                {
                    NbtCompound dc = null;
                    foreach (NbtCompound c in ents.Tags)
                    {
                        Entity ent = new Entity(c);
                        if (e.Pos == ent.Pos)
                        {
                            dc = c;
                            break;
                        }
                    }
                    if (dc != null)
                        ents.Tags.Remove(dc);
                }
                ents.Tags.Add(e.ToNBT());
                level["Entities"] = ents;
                chunk.RootTag["Level"] = level;
                chunk.SaveFile(f);
            }
            catch (Exception) { }
        }

        public void RemoveEntity(Entity e)
        {
            Guid ID = e.UUID;
            if (_Entities.ContainsKey(ID))
                _Entities.Remove(ID);

            int CX = (int)e.Pos.X / 16;
            int CY = (int)e.Pos.Y / 16;
            e.Pos.X = (int)e.Pos.X % 16;
            e.Pos.Y = (int)e.Pos.Y % 16;

            string f = GetChunkFilename(CX, CY);
            if (!File.Exists(f))
            {
                Console.WriteLine("! {0}", f);
                return;
            }
            try
            {
                chunk = new NbtFile(f);
                chunk.LoadFile();
                NbtCompound level = (NbtCompound)chunk.RootTag["Level"];

                NbtList ents = (NbtList)level["Entities"];
                if (e.OrigPos != null)
                {
                    NbtCompound dc = null;
                    foreach (NbtCompound c in ents.Tags)
                    {
                        Entity ent = new Entity(c);
                        if (e.Pos == ent.Pos)
                        {
                            dc = c;
                            break;
                        }
                    }
                    if (dc != null)
                        ents.Tags.Remove(dc);
                }
                level["Entities"] = ents;
                chunk.RootTag["Level"] = level;
                chunk.SaveFile(f);
            }
            catch (Exception) { }
        }
        int BlockCount = 0;
        private bool InTransaction;

        private int GetBlockIndex(int x, int y, int z)
        {
            //return x * ChunkZ + y * ChunkZ * ChunkX + z;
            return y * ChunkZ + x * ChunkZ * ChunkX + z;
            //return (x << 11 | z << 7 | y);
        }

        public void Generate(string gen, long X, long Y)
        {
            NbtCompound c = NewChunk(X,Y);
            NbtCompound Level = c["Level"].asCompound();
            byte[,,] newblocks = MapGenerator.Get(gen,RandomSeed).Generate(X,Y,ChunkScale);
            byte[] blocks = new byte[ChunkX*ChunkY*ChunkZ];
            for(int x = 0;x<ChunkX;x++)
                for(int y = 0;y<ChunkY;y++)
                    for(int z = 0;z<ChunkZ;z++)
                        blocks[GetBlockIndex(x,y,z)]=newblocks[x,y,z];
            Level.Add("Blocks", blocks);
        }

        public NbtCompound NewChunk(long X, long Y)
        {
            NbtCompound Level = new NbtCompound("Level");
            Level.Add("TerrainPopulated", false);
            Level.Add("xPos", X);
            Level.Add("zPos", Y);
            Level.Add("LastUpdate", 0); // idk what the format is, not going to decompile.
            Level.Add("BlockLight",  new byte[16384]);
            Level.Add("Blocks",      new byte[ChunkX*ChunkY*ChunkZ]);
            Level.Add("Data", new byte[16384]);
            Level.Add("HeightMap", new byte[256]);
            Level.Add("SkyLight", new byte[16384]);
            Level.Add("Entities", new NbtCompound());
            Level.Add("TileEntities", new NbtCompound());

            NbtCompound Chunk = new NbtCompound("_ROOT_");
            Chunk.Tags.Add(Level);
            return Chunk;
        }

        public void ReplaceBlocksIn(long X, long Y, Dictionary<byte, byte> Replacements)
        {
            if (Replacements == null) return;
            Chunk c = _LoadChunk((int)X, (int)Y);
            if (c == null) return;

            bool bu=false;
            for(int x = 0;x<ChunkX;x++)
            {
                for (int y = 0; y < ChunkY; y++)
                {
                    for (int z = 0; z < ChunkZ; z++)
                    {
                        if (Replacements.ContainsKey(c.Blocks[x,y,z]))
                        {
                            c.Blocks[x,y,z] = Replacements[c.Blocks[x,y,z]];
                            bu = true;
                        }
                    }
                }
            }
            if(!bu) return;
            string ci = string.Format("{0},{1}", X, Y);
            if (Chunks.ContainsKey(ci))
            {
                Chunks.Remove(ci);
            }
            Chunks.Add(ci,c);
            if(!ChangedChunks.Contains(ci))
                ChangedChunks.Add(ci);
        }
        public byte GetBlockIn(long CX, long CY, Vector3i pos)
        {
            pos = new Vector3i(pos.Y, pos.X, pos.Z);
            /*
            if (
                !Check(pos.X, -1, ChunkX) ||
                !Check(pos.Y, -1, ChunkY) ||
                !Check(pos.Z, -1, ChunkZ))
            {
                //Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
                return 0x00;
            }
             */

            string ci = string.Format("{0},{1}", CX, CY);
            int i = GetBlockIndex((int)pos.X, (int)pos.Y, (int)pos.Z);
            //try
            //{
            if (Chunks.ContainsKey(ci))
            {
                if (Chunks[ci] == null) return 0x00;
                return Chunks[ci].Blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
            }
            //}
            //catch (Exception)
            //{
            //    return 0x00;
            //}
            Chunk c = _LoadChunk((int)CX, (int)CY);

            if (!Chunks.ContainsKey(ci))
                Chunks.Add(ci, c);

            try
            {
                return c.Blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
            }
            catch (Exception)
            {
                return 0x00;
            }
        }

        public void LoadChunk(long X, long Y)
        {
            _LoadChunk((int)X, (int)Y);
        }

        public void SetBlockIn(long CX, long CY, Vector3i pos, byte type)
        {
            pos = new Vector3i(pos.Y, pos.X, pos.Z);
            // block saving to any negative chunk due to being unreadable.
            if (CX < 0 || CY < 0) return;

            string ci = string.Format("{0},{1}", CX, CY);
            //try
            //{
            if (Chunks.ContainsKey(ci))
            {
                Chunks[ci].Blocks[(int)pos.X,(int)pos.Y,(int)pos.Z] = type;
                return;
            }
            // Don't mess with unloaded blocks.
        }

        private bool Check(long x, long min, long max)
        {
            return (x > min && x < max);
        }

        public void BeginTransaction()
        {
            InTransaction = true;
            ChangedChunks.Clear();
            Console.WriteLine("BEGIN TRANSACTION");
        }
        public void CommitTransaction()
        {
            Console.WriteLine("{0} chunks changed.", ChangedChunks.Count);
            foreach (string v in ChangedChunks)
            {
                string[] chunks = v.Split(',');
                SaveChunk(int.Parse(chunks[0]), int.Parse(chunks[1]));
            }
            Console.WriteLine("COMMIT TRANSACTION");
            InTransaction = false;
        }
        public void SetBlockAt(Vector3i p, byte id)
        {
            //Console.WriteLine("{0}", p);
            int CX = (int)p.X >> 4;// / (long)ChunkX);
            int CZ = (int)p.Y >> 4;// / (long)ChunkY);

            int x = ((int)p.Y % ChunkX) & 0xf;
            int y = ((int)p.X % ChunkY) & 0xf;
            int z = (int)p.Z;// % ChunkZ;

            if (
                !Check(x, -1, ChunkX) ||
                !Check(y, -1, ChunkY) ||
                !Check(z, -1, ChunkZ))
            {
                //Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
                return;
            }
            string ci = string.Format("{0},{1}", CX, CZ);
            if (!Chunks.ContainsKey(ci))
                return;
            Chunk c = Chunks[ci];
            c.Blocks[x,y,z]=id;
            Chunks[ci] = c;
            if (!ChangedChunks.Contains(ci))
            {
                ChangedChunks.Add(ci);
                Console.WriteLine(ci+" has changed");
            }
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

        public bool GetInventory(int slot, out short itemID, out short Damage, out byte Count, out string failreason)
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
            //NbtCompound Data = (NbtCompound)root.RootTag["Data"];
            //NbtCompound Player = (NbtCompound)Data["Player"];
            //NbtList pi = (NbtList)Player["Inventory"];
            NbtList pi = (NbtList)root.GetTag("/Data/Player/Inventory");
            foreach (NbtTag t in pi.Tags)
            {
                NbtCompound s = (NbtCompound)t;
                if ((s["Slot"] as NbtByte).Value == (byte)slot)
                {
                    Count = (s["Count"] as NbtByte).Value;
                    itemID = (s["id"] as NbtShort).Value;
                    Damage = (s["Damage"] as NbtShort).Value;
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

        public bool SetArmor(ArmorType Armor, short itemID, int Damage, int Count)
        {
            int slot = 103;
            switch (Armor)
            {
                case ArmorType.Helm:
                    slot = 103;
                    break;
                case ArmorType.Torso:
                    slot = 102;
                    break;
                case ArmorType.Legs:
                    slot = 101;
                    break;
                case ArmorType.Boots:
                    slot = 100;
                    break;
            }
            return SetInventory(slot, itemID, Damage, Count);
        }

        public bool GetArmor(ArmorType Armor, out short itemID, out short Damage, out byte Count, out string failreason)
        {
            int slot = 103;
            itemID = 0;
            Damage = 0;
            Count = 0;
            switch (Armor)
            {
                case ArmorType.Helm:
                    slot = 103;
                    break;
                case ArmorType.Torso:
                    slot = 102;
                    break;
                case ArmorType.Legs:
                    slot = 101;
                    break;
                case ArmorType.Boots:
                    slot = 100;
                    break;
            }
            return GetInventory(slot, out itemID, out Damage, out Count, out failreason);
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
        public void ForEachChunk(Chunk.ChunkModifierDelegate cmd)
        {
            string[] dirsX = Directory.GetDirectories(Folder);
            foreach (string dirX in dirsX)
            {
                //Console.WriteLine(dirX);
                string[] dirsY = Directory.GetDirectories(dirX);
                foreach(string dirY in dirsY)
                {
                    //Console.WriteLine(dirY);
                    string[] files = Directory.GetFiles(dirY);
                    foreach (string file in files)
                    {
                        //Console.WriteLine(file);
                        //Console.WriteLine(Path.GetExtension(file));
                        if (Path.GetExtension(file) == "dat") continue;
                        NbtFile f = new NbtFile(file);
                        f.LoadFile();
                        NbtCompound Level = (NbtCompound)f.RootTag["Level"];
                        long x = (Level["xPos"] as NbtInt).Value;
                        long y = (Level["zPos"] as NbtInt).Value;
                        f.Dispose();
                        cmd(x, y);
                    }
                }
            }
        }

        public string GeneratorAlgorithm { get; set; }

        public void ChunkModified(long x, long y)
        {

        }

        public Chunk GetChunk(long x, long y)
        {
            return GetChunk((int)x, (int)y);
        }
    }
}
