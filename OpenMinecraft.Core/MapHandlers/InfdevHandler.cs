#define CATCH_CHUNK_ERRORS

using System;
using System.Collections.Generic;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;

namespace OpenMinecraft
{
	public class InfdevHandler:IMapHandler
	{
		public event CorruptChunkHandler CorruptChunk;
		public event ForEachProgressHandler ForEachProgress;

#if DEBUG
        public bool _DEBUG = true; // Improve performance.  Maybe.
#else
        public bool _DEBUG = false; // Improve performance.  Maybe.
#endif

        private string mFolder;
		private const int ChunkX = 16;
		private const int ChunkY = 16;
        private const int ChunkZ = 128;

		NbtFile mRoot = new NbtFile();
		NbtFile mChunk = new NbtFile();
		Dictionary<string, Chunk> mChunks = new Dictionary<string, Chunk>();
		List<string> mChangedChunks = new List<string>();
		Dictionary<Guid, Entity> mEntities = new Dictionary<Guid, Entity>();
		Dictionary<Guid, TileEntity> mTileEntities = new Dictionary<Guid, TileEntity>();

		public string Filename {get;set;}
		public int InventoryCapacity { get { return 9*4; } }
		public Vector3i ChunkScale { get { return new Vector3i(16, 16, 128); } }
		public bool HasMultipleChunks { get { return true; } }
		public Vector3i MapMin { get { return new Vector3i(-1000, -1000, 0); } }
		public Vector3i MapMax { get { return new Vector3i(1000, 1000, 127); } }

		public Vector3i Spawn
		{
			get
			{
				NbtCompound Data = mRoot.Query<NbtCompound>("//Data");
				return new Vector3i(
					(Data["SpawnX"] as NbtInt).Value,
					(Data["SpawnY"] as NbtInt).Value,
					(Data["SpawnZ"] as NbtInt).Value);
			}

			set
			{
				NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
				NbtCompound Player = (NbtCompound)Data["Player"];
				Player.Remove("SpawnX");
				Player.Remove("SpawnY");
				Player.Remove("SpawnZ");
				Player.Add("SpawnX", new NbtInt("SpawnX",(int)value.X));
				Player.Add("SpawnY", new NbtInt("SpawnY",(int)Utils.Clamp(value.Y, 0, 128)));
				Player.Add("SpawnZ", new NbtInt("SpawnZ",(int)value.Z));
				Data["Player"] = Player;
				mRoot.RootTag["Data"] = Data;
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
				NbtShort h = mRoot.Query<NbtShort>("//Data/Player/Health");
				return (int)h.Value;
			}
			set
			{
				// Clamp value to between -1 and 21
                mRoot.SetQuery<short>("//Data/Player/Health", (short)Utils.Clamp(value, -1, 21));
                /*
				NbtShort h = new NbtShort("Health", (short)Utils.Clamp(value, -1, 21));
				//root.SetTag("/Data/Player/Health",h);
				NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
				NbtCompound Player = (NbtCompound)Data["Player"];
				NbtShort oh = (NbtShort)Player["Health"];
				if(_DEBUG) Console.WriteLine("Health: {0}->{1}", oh.Value, h.Value);
				Player.Tags.Remove(oh);
				Player.Add(h);
				Data["Player"] = Player;
				mRoot.RootTag["Data"] = Data;
                */
			}
		}

		// Set to 0 to avoid the Laying Down bug.
		public int HurtTime
		{
			get
			{
				NbtShort h = mRoot.Query<NbtShort>("//Data/Player/HurtTime");
				return (int)h.Value;
			}
			set
			{
                // TODO: Find value range.
                mRoot.SetQuery<short>("//Data/Player/HurtTime", (short)value);

			}
		}
		// Don't clamp, all sorts of weird shit can be done here.
		public int Air
		{
			get
			{
				// /Player/Air;
				NbtShort h = mRoot.Query<NbtShort>("//Data/Player/Air");
				return (int)h.Value;
			}
			set
			{
                mRoot.SetQuery<short>("//Data/Player/Air",(short)value);
                /*
				NbtShort h = new NbtShort("Air", (short)value);
				//root.SetTag("/Data/Player/Air", h); ;

				NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
				NbtCompound Player = (NbtCompound)Data["Player"];
				Player.Tags.Remove(Player["Air"]);
				Player.Add(h);
				Data["Player"] = Player;
				mRoot.RootTag["Data"] = Data;
                */
			}
		}

		// Dunno the range
		public int Fire
		{
			get
			{
				NbtShort h = mRoot.Query<NbtShort>("//Data/Player/Fire");
				return (int)h.Value;
			}
			set
			{
                mRoot.SetQuery<short>("//Data/Player/Fire",(short)value);
			}
		}

		// Dunno the range
		public int Time
		{
			get
			{
				return (int)mRoot.Query<NbtLong>("//Data/Time").Value;
			}
			set
			{
                mRoot.SetQuery("//Data/Time",(object)value);
			}
		}

		public Vector3d PlayerPos
		{
			get
			{
				NbtList h = mRoot.Query<NbtList>("//Data/Player/Pos");
				Vector3d pos = new Vector3d();
				pos.X = (h[0] as NbtDouble).Value;
				pos.Y = (h[1] as NbtDouble).Value;
				pos.Z = (h[2] as NbtDouble).Value;
				//if(_DEBUG) Console.WriteLine("Player is on chunk {0}.",GetChunkFilename((int)pos.X >> 4,(int)pos.Z >> 4));
				return pos;
			}
			set
			{
				NbtList h = new NbtList("Pos");
				h.Add(new NbtDouble("", value.X));
				h.Add(new NbtDouble("", Utils.Clamp(value.Y, 0, 128)));
				h.Add(new NbtDouble("", value.Z));
				// BROKEN root.SetTag("/Data/Player/Pos", h);
				NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
				NbtCompound Player = (NbtCompound)Data["Player"];
				Player.Remove("Pos");
				Player.Add(h);
				Data["Player"] = Player;
				mRoot.RootTag["Data"] = Data;

			}
		}

		public Dictionary<Guid, Entity> Entities
		{
			get
			{
				return mEntities;
			}
		}

		public Dictionary<Guid, TileEntity> TileEntities
		{
			get
			{
				return mTileEntities;
			}
		}

		public long RandomSeed
		{
			get
			{
				return mRoot.Query<NbtLong>("//Data/RandomSeed").Value;
			}
			set
			{
                mRoot.SetQuery<long>("//Data/RandomSeed",value);
			}
		}

		public void Load()
		{
			Load(Filename);
		}
		public void Load(string filename)
		{
			try
			{
				mChunks.Clear();
				//CurrentBlock = new Vector3i(0, 0, 0);
				Filename = filename;
				mFolder = Path.GetDirectoryName(filename);
				mRoot = new NbtFile(filename);
				mRoot.LoadFile();
			}
			catch (Exception e)
			{
                if(_DEBUG) Console.WriteLine(e);
				System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("Your level.dat is broken. Copying over level.dat_old...","ERROR",System.Windows.Forms.MessageBoxButtons.OKCancel);
				if (dr == System.Windows.Forms.DialogResult.OK)
				{
					File.Copy(Path.ChangeExtension(filename, "dat_old"), filename);
					Load(filename);
				}
				else return;
			}
			LoadChunk(0, 0);
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

		private byte[, ,] DecompressLightmap(byte[] databuffer)
		{
			byte[, ,] blocks = new byte[ChunkX, ChunkY, ChunkZ];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 128; z++)
					{
						blocks[x,y,z]=(byte)DecompressLight(databuffer, x, y, z);
					}
				}
			}
			return blocks;
		}

		private byte[, ,] DecompressDatamap(byte[] databuffer)
		{
			byte[, ,] blocks = new byte[ChunkX, ChunkY, ChunkZ];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 128; z++)
					{
						blocks[x,y,z]=(byte)DecompressData(databuffer, x, y, z);
					}
				}
			}
			return blocks;
		}
		private byte[] CompressLightmap(byte[,,] blocks,bool sky)
		{
			byte[] databuffer = new byte[16384];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 128; z++)
					{
						CompressLight(ref databuffer,sky, x, y, z, blocks[x, y, z]);
					}
				}
			}
			return databuffer;
		}
		private byte[] CompressDatamap(byte[,,] blocks)
		{
			byte[] databuffer = new byte[16384];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					for (int z = 0; z < 128; z++)
					{
						CompressData(ref databuffer, x, y, z, blocks[x, y, z]);
					}
				}
			}
			return databuffer;
		}

		private Chunk _LoadChunk(int x, int z)
		{
			//CurrentBlock.X = x;
			//CurrentBlock.Z = z;

			Chunk c = new Chunk(this);
			c.Loading = true;
			c.Filename = GetChunkFilename(x, z);
			c.CreationDate = File.GetCreationTime(c.Filename);
			c.Creator = "?";
			c.Size = ChunkScale;

			if (!File.Exists(c.Filename))
			{
				if(_DEBUG) Console.WriteLine("! {0}", c.Filename);
				return null;
			}
#if !DEBUG || CATCH_CHUNK_ERRORS
			try
			{
#endif
				mChunk = new NbtFile(c.Filename);
				mChunk.LoadFile();

				NbtCompound level = mChunk.RootTag.Get<NbtCompound>("Level");

				c.Position = new Vector3i(
					level.Get<NbtInt>("xPos").Value,
					level.Get<NbtInt>("zPos").Value, 0);

				if ((int)c.Position.X != x || (int)c.Position.Y != z)
				{
					throw new Exception(string.Format("Chunk pos is wrong.  {0}!={1}", c.Filename, c.Position));
				}
				NbtList TileEntities = (NbtList)level["TileEntities"];
				if (TileEntities.Tags.Count > 0)
				{
					//if(_DEBUG) Console.WriteLine("*** Found TileEntities.");
					LoadTileEnts(ref c, (int)x, (int)z, TileEntities);
				}

                NbtList Entities = (NbtList)level["Entities"];
                if(_DEBUG) Console.WriteLine("*** Found {0} Entities.",Entities.Tags.Count);
				if (Entities.Tags.Count > 0)
				{
					LoadEnts(ref c, (int)x, (int)z, Entities);
				}

				// Blocks
				c.Blocks = DecompressBlocks(level.Get<NbtByteArray>("Blocks").Value);
				c.BlockLight = DecompressLightmap(level.Get<NbtByteArray>("BlockLight").Value);
				c.SkyLight = DecompressLightmap(level.Get<NbtByteArray>("SkyLight").Value);
				c.Data = DecompressDatamap(level.Get<NbtByteArray>("Data").Value);

				c.Loading = false;
				c.UpdateOverview();

				string ci = string.Format("{0},{1}", x, z);
				if (mChunks.ContainsKey(ci))
					return mChunks[ci];
				mChunks.Add(ci, c);
				//TODO: Make Pig spawner converter.
				for (int Z = 0; Z < ChunkScale.X; Z++)
				{
					for (int Y = 0; Y < ChunkScale.Y; Y++)
					{
						for (int X = 0; X < ChunkScale.X; X++)
						{
                            byte b = c.Blocks[X, Y, Z];
							if (b == 52)
							{
								MobSpawner ms = new MobSpawner();
                                ms.Pos=new Vector3i(X, Y, Z);
                                ms.EntityId = "Pig";
								ms.UUID = Guid.NewGuid();
								mTileEntities.Add(ms.UUID, ms);
								//c++;
							}
						}
					}
				}
				//if (c>0)  if(_DEBUG) Console.WriteLine("*** {0} spawners found.", c);
				//if(_DEBUG) Console.WriteLine("Loaded {0} bytes from chunk {1}.", CurrentChunks.Length, c.Filename);
				return c;

#if !DEBUG || CATCH_CHUNK_ERRORS
            }
			catch (Exception e)
			{
				string err = string.Format(" *** ERROR: Chunk {0},{1} ({2}) failed to load:\n\n{3}", x, z, c.Filename, e);
				if(_DEBUG) Console.WriteLine(err);
				if (CorruptChunk != null)
					CorruptChunk(x,z,err, c.Filename);
				return null;
			}
#endif
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

		private void LoadEnts(ref Chunk cnk, int CX, int CZ, NbtList ents)
		{
            if(_DEBUG) Console.WriteLine("Loading {0} entities in chunk {1},{2} ({3}):", ents.Tags.Count, CX, CZ, cnk.Filename);
			foreach (NbtCompound c in ents.Tags)
			{
                Entity e = Entity.GetEntity(c);
                
                // TODO: Verify entity positioning.
                e.Pos.X = (e.Pos.X * 16d) + CX;
                e.Pos.Z = (e.Pos.Z * 16d) + CZ;

				e.UUID = Guid.NewGuid();

				cnk.Entities.Add(e.UUID, e);
				mEntities.Add(e.UUID, e);
			}
		}

		private void LoadTileEnts(ref Chunk cnk, int CX,int CZ,NbtList ents)
        {
            if(_DEBUG) Console.WriteLine("Loading {0} tile entities in chunk {1},{2} ({3}):", ents.Tags.Count, CX, CZ, cnk.Filename);
			foreach (NbtCompound c in ents.Tags)
			{
                TileEntity te = TileEntity.GetEntity(CX, CZ, (int)ChunkScale.X, c);
                
                // TODO: Verify TileEntity positioning.
                te.Pos.X = (te.Pos.X * 16) + CX;
                te.Pos.Z = (te.Pos.Z * 16) + CZ;

				te.UUID = Guid.NewGuid();
				mTileEntities.Add(te.UUID, te);
			}
		}
		
		public void SetTileEntity(TileEntity e)
		{
			long CX=e.Pos.X/16;
			long CY=e.Pos.Y/16;
			string f = GetChunkFilename((int)CX, (int)CY);

			try
			{
				mChunk = new NbtFile(f);
				mChunk.LoadFile();
				NbtCompound level = (NbtCompound)mChunk.RootTag["Level"];

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
					tents.Add(e.ToNBT());

				level["TileEntities"] = tents;
				mChunk.RootTag["Level"] = level;
				mChunk.SaveFile(f);
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
				mChunk = new NbtFile(f);
				mChunk.LoadFile();
				NbtCompound level = (NbtCompound)mChunk.RootTag["Level"];

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
				mChunk.RootTag["Level"] = level;
				mChunk.SaveFile(f);
			}
			catch (Exception) { }
		}

		private string GetChunkFilename(int x, int z)
		{
			string file = "c." + Utils.IntToBase(x, 36) + "." + Utils.IntToBase(z, 36) + ".dat";
			string dirX = Utils.IntToBase(x & 0x3F, 36);
			string dirY = Utils.IntToBase(z & 0x3F, 36);
            string dirDimension = string.Format("DIM-{0}", mDimension);
            if (mDimension != 0)
            {
                return Path.Combine(mFolder,
                    Path.Combine(dirDimension,
                        Path.Combine(dirX,
                            Path.Combine(dirY,
                                file
                            )
                        )
                    )
                );
            }
            else
            {
                return Path.Combine(mFolder,
                    Path.Combine(dirX,
                        Path.Combine(dirY,
                            file
                        )
                    )
                );
            }
		}
		private void SaveChunk(int x, int z)
		{
			Chunk c = GetChunk(x,z,false);
			if(c!=null)
				return;
			SaveChunk(c);
		}

		public Chunk GetChunk(int x, int z, bool GenerateNewChunkIfNeeded)
		{
			string id = x.ToString() + "," + z.ToString();
			Chunk c;
			if (!mChunks.TryGetValue(id, out c))
			{
				if(File.Exists(GetChunkFilename(x,z)))
					return _LoadChunk(x, z);
				if (GenerateNewChunkIfNeeded)
				{
					Generate(this, x, z);
					return GetChunk(x, z);
				}
				return null;
			}
			return c;
		}

		public void SetChunk(Chunk cnk)
		{
			string id = cnk.Position.ToString() + "," + cnk.Position.Y.ToString();
			if(mChunks.ContainsKey(id))
				mChunks[id]=cnk;
			else
				mChunks.Add(id,cnk);
		}
		public void SaveChunk(Chunk cnk)
		{
			NbtFile c = new NbtFile(cnk.Filename);

            // TODO: PosY -> PosZ
            c.RootTag = NewNBTChunk(cnk.Position.X, cnk.Position.Y);
            NbtCompound Level = (NbtCompound)c.RootTag["Level"];

			// BLOCKS /////////////////////////////////////////////////////
			byte[] blocks = new byte[ChunkX * ChunkY * ChunkZ];
			for (int X = 0; X < ChunkX; X++)
			{
				for (int Y = 0; Y < ChunkY; Y++)
				{
					for (int Z = 0; Z < ChunkZ; Z++)
					{
						blocks[GetBlockIndex(X, Y, Z)] = cnk.Blocks[X, Y, Z];
					}
				}
			}
			Level.Set("Blocks", new NbtByteArray("Blocks",blocks));
			blocks = null;

			// LIGHTING ///////////////////////////////////////////////////
			// TODO:  Whatever is going on in here is crashing Minecraft now.
			byte[] lighting = CompressLightmap(cnk.SkyLight,true);
			Level.Set("SkyLight",new NbtByteArray("SkyLight", lighting));

			lighting = CompressLightmap(cnk.BlockLight,false);
            Level.Set("BlockLight",new NbtByteArray("BlockLight", lighting));
			
			lighting = CompressDatamap(cnk.Data);
            Level.Set("Data", new NbtByteArray("Data", lighting));

			// HEIGHTMAP (Needed for lighting).
			byte[] hm = new byte[256];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					hm[y + (x * 16)] = (byte)cnk.HeightMap[x, y];
				}
			}

			NbtList ents = new NbtList("Entities");
			// ENTITIES ///////////////////////////////////////////////////
			foreach (KeyValuePair<Guid, Entity> ent in cnk.Entities)
			{
				ents.Add(ent.Value.ToNBT());
			}
			Level.Set("Entities",ents);

			NbtList tents = new NbtList("TileEntities");
			// TILE ENTITIES //////////////////////////////////////////////
			foreach (KeyValuePair<Guid, TileEntity> tent in cnk.TileEntities)
			{
				ents.Add(tent.Value.ToNBT());
			}
			Level.Set("TileEntities",tents);

			c.RootTag.Set("Level",Level);
			
            c.SaveFile(cnk.Filename);
			
            // For debuggan
            File.WriteAllText(cnk.Filename + ".txt", c.RootTag.ToString());
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
			mRoot.SaveFile(Filename);
			return true;
		}

		public bool Save(string filename)
        {
            mFolder = Path.GetDirectoryName(filename);
            Filename = filename;
            if (File.Exists(filename))
            {
                File.Copy(filename, Path.ChangeExtension(filename, "bak"), true);
                mRoot.SaveFile(filename);
            }
            else
            {
                mRoot.SaveFile(filename);
            }
            return true;
		}
        public void AddEntity(Entity e)
        {
            int CX = (int)(e.Pos.X / 16d);
            int CZ = (int)(e.Pos.Z / 16d);
            e.Pos.X = (int)e.Pos.X % 16;
            e.Pos.Z = (int)e.Pos.Z % 16;

            e.UUID = Guid.NewGuid();

            mEntities.Add(e.UUID, e);

            string f = GetChunkFilename((int)CX, (int)CZ);
            Chunk c;
            if (!File.Exists(f))
            {
                c = NewChunk(CX, CZ);
            }
            else
            {
                c = GetChunk(CX, CZ);
            }
            try
            {
                if(_DEBUG) Console.WriteLine("Saving {0} to chunk {1},{2}...", e.ToString(), CX, CZ);
                if (c.Entities.ContainsKey(e.UUID))
                    c.Entities.Remove(e.UUID);
                c.Entities.Add(e.UUID, e);
                c.Save();
            }
            catch (Exception ex) 
            {
                if(_DEBUG) Console.WriteLine(ex.ToString());
            }
        }
		public void SetEntity(Entity e)
		{
			Guid ID = e.UUID;

			if (mEntities.ContainsKey(ID))
				mEntities.Remove(ID);
			mEntities.Add(ID, e);

            int CX = (int)(e.Pos.X / 16d);
            int CZ = (int)(e.Pos.Z / 16d);
            e.Pos.X = (int)e.Pos.X % 16;
            e.Pos.Z = (int)e.Pos.Z % 16;

			string f = GetChunkFilename(CX, CZ);
			if (!File.Exists(f))
			{
				if(_DEBUG) Console.WriteLine("! {0}", f);
				return;
			}
			try
			{
				Chunk c = GetChunk(CX, CZ);
				if (c.Entities.ContainsKey(e.UUID))
					c.Entities.Remove(e.UUID);
				c.Entities.Add(e.UUID, e);
				c.Save();
			}
			catch (Exception) { }
		}

		public void RemoveEntity(Entity e)
		{
            // Get our UUID (why UUIDs, Rob? WTF)
			Guid ID = e.UUID;

            // Is this entity loaded?
			if (mEntities.ContainsKey(ID))
				mEntities.Remove(ID);

            // Try and translate global coords to chunk local coords
            // TODO: What the fuck are you doing
			int CX = (int)e.Pos.X / 16;
			int CZ = (int)e.Pos.Z / 16;
			e.Pos.X = (int)e.Pos.X % 16;
			e.Pos.Z = (int)e.Pos.Z % 16;

            // Find parent chunk, remove
			string f = GetChunkFilename(CX, CZ);
			if (!File.Exists(f))
			{
                // Can't find it, so don't bother.
				if(_DEBUG) Console.WriteLine("! {0}", f);
				return;
			}
			try
			{
                Chunk c = GetChunk(CX,CZ);
                c.Entities.Remove(e.UUID);
                c.Save();
			}
			catch (Exception) { }
		}

		private int GetBlockIndex(int x, int y, int z)
		{
			return y * ChunkZ + x * ChunkZ * ChunkX + z;
		}


		public byte GetBlockAt(int px, int py, int z)
		{
			int X = px / 16;
			int Y = py / 16;

			int x = (px >> 4) & 0xf;// - (X * (int)ChunkScale.X);
			int y = (py >> 4) & 0xf;// - (Y * (int)ChunkScale.Y);

			Chunk c = GetChunk(X, Y);
			if (c == null) return 0x00;
			return c.Blocks[x, y, z];
		}

		public void SetBlockAt(int px, int py, int z, byte val)
		{
			int X = px / 16;
			int Y = py / 16;

			int x = (px >> 4) & 0xf;// - (X * (int)ChunkScale.X);
			int y = (py >> 4) & 0xf;// - (Y * (int)ChunkScale.Y);

			Chunk c = GetChunk(X, Y);
			if (c == null) return;
			c.Blocks[x, y, z] = val;
			SetChunk(X, Y, c);
		}

		private void SetChunk(int X, int Y, Chunk c)
		{
			string id = X.ToString() + "," + Y.ToString();
			if (!mChunks.ContainsKey(id))
			{
				mChunks.Add(id, c);
			}
			else
			{
				mChunks[id] = c;
			}
			if (!mChangedChunks.Contains(id))
				mChangedChunks.Add(id);
		}

		private void UnloadChunks()
		{
			mChunks.Clear();
			mChangedChunks.Clear();
            mEntities.Clear();
            mTileEntities.Clear();
		}

        public int ExpandFluids(byte fluidID, bool CompleteRegen, ForEachProgressHandler ph)
        {
            int total = 0;
            int chunksProcessed=0;
            ForEachChunk(delegate(long X, long Z)
            {
                total += ExpandFluids(X, Z, fluidID, CompleteRegen);
                ph(-1, ++chunksProcessed);
            });
            return total;
        }

		private int ExpandFluids(long X, long Y, byte fluidID, bool CompleteRegen)
        {
            Chunk chunk = GetChunk(X, Y);
            byte[, ,] blocks = new byte[ChunkX + 2, ChunkY + 2, ChunkZ + 1]; // Allow a border of 1 voxel to the sides.
            #region Border setup
            {

                Chunk XYp1 = GetChunk(X, Y + 1);
                Chunk XYm1 = GetChunk(X, Y - 1);
                Chunk Xp1Y = GetChunk(X + 1, Y);
                Chunk Xm1Y = GetChunk(X - 1, Y);

                //////////////////////////////////////
                //          //          //          //
                //          //          //          //
                //          //   XYp1   //          //
                //          //          //          //
                //          //          //          //
                //////////////////////////////////////
                //          //          //          //
                //          //          //          //
                //   Xm1Y   //  chunk   //   Xp1Y   //
                //          //          //          //
                //          //          //          //
                //////////////////////////////////////
                //          //          //          //
                //          //          //          //
                //          //   XYm1   //          //
                //          //          //          //
                //          //          //          //
                //////////////////////////////////////

                for (int x = 0; x < ChunkX + 1; x++)
                {
                    for (int y = 0; y < ChunkY + 1; y++)
                    {
                        for (int z = 0; z < ChunkZ + 1; z++)
                        {
                            if (x > 0 && x < ChunkX + 1 && y > 0 && y < ChunkY + 1 && z > 0 && z < ChunkZ)
                            {
                                blocks[x + 1, y + 1, z] = chunk.Blocks[x, y, z];
                            }
                            else if (Xm1Y != null && x == 0 && y > 0 && y < ChunkY && z < ChunkZ)
                            {
                                blocks[x, y, z] = Xm1Y.Blocks[ChunkX - 1, y, z];
                            }
                            else if (Xp1Y != null && x == ChunkX && y > 0 && y < ChunkY && z < ChunkZ)
                            {
                                blocks[x, y, z] = Xp1Y.Blocks[0, y, z];
                            }
                            else if (XYm1 != null && y == 0 && x > 0 && x < ChunkX && z < ChunkZ)
                            {
                                blocks[x, y, z] = XYm1.Blocks[x, ChunkY - 1, z];
                            }
                            else if (XYp1 != null && y == ChunkY && x > 0 && x < ChunkX && z < ChunkZ)
                            {
                                blocks[x, y, z] = XYp1.Blocks[x, 0, z];
                            }
                        }
                    }
                }
            }
            #endregion
            // Add border to bytemap
            int BlocksChanged = 0;
            for (int x = 1; x < ChunkX; x++)
            {
                for (int y = 1; y < ChunkY; y++)
                {
                    for (int z = 0; z < ChunkZ; z++)
                    {
                        // If block is air, check above or to the sides to see if water should be expanded into it.
                        if (chunk.Blocks[x, y, z] == 0)
                        {
                            if (
                                blocks[x, y, z + 1] == fluidID // Above
                                || blocks[x + 1, y, z] == fluidID  // Sides
                                || blocks[x - 1, y, z] == fluidID
                                || blocks[x, y + 1, z] == fluidID
                                || blocks[x, y - 1, z] == fluidID)
                            {
                                blocks[x, y, z] = fluidID;
                                chunk.Blocks[x+1, y+1, z] = fluidID;
                            }
                        }
                    }
                }
            }
            chunk.Save();
            return BlocksChanged;
        }


		public void Generate(IMapHandler mh, long X, long Y)
		{
			if (_Generator == null) return;
			string lockfile = Path.ChangeExtension(GetChunkFilename((int)X,(int)Y), "genlock");
			if (!_Generator.NoPreservation)
			{
				if (File.Exists(lockfile))
					return;
			}
			else
			{
				if (File.Exists(lockfile))
					File.Delete(lockfile);
			}
			Chunk _c = mh.NewChunk(X, Y);
			byte[,,] b = _Generator.Generate(ref mh, X, Y);
			if (b == null) return;
			/*
			try
			{
				//if(_DEBUG) Console.WriteLine("{0} blocks of stone post-generation.", GetBlockNumbers(b)[1]);
			}
			catch (Exception) { }
			*/
			_Generator.AddTrees(ref b, (int)X, (int)Y, (int)ChunkScale.Z);
			_c.Blocks = b;
			_c.UpdateOverview();
			//_c.SkyLight=Utils.RecalcLighting(_c,true);
			//_c.BlockLight=Utils.RecalcLighting(_c,false);
			_c.Save();
			File.WriteAllText(lockfile, "");
		}

		private Dictionary<byte,int> GetBlockNumbers(byte[, ,] b)
		{
			Dictionary<byte, int> BlockCount = new Dictionary<byte, int>();
			for (int x = 0; x < ChunkScale.X; x++)
			{
				for (int y = 0; y < ChunkScale.Y; y++)
				{
					for (int z = 0; z < ChunkScale.Z; z++)
					{
						byte blk = b[x, y, z];
						if (!BlockCount.ContainsKey(blk))
							BlockCount.Add(blk, 0);
						++BlockCount[blk];
					}
				}
			}
			return BlockCount;
		}

		public Chunk NewChunk(long X, long Y)
        {
            NbtCompound c = NewNBTChunk(X, Y);
            string f = GetChunkFilename((int)X, (int)Y);

            // Create folder first.
            string d = Path.GetDirectoryName(f);
            
            if(!Directory.Exists(d))
                Directory.CreateDirectory(d);

            // Get around crash where Windows doesn't like creating new files in Write mode for some fucking stupid reason
            {
                FileInfo fi = new FileInfo(f);
                FileStream fs = fi.Create();
                fs.Close();
            }

            // Create the damn file.
            NbtFile cf = new NbtFile(f);
            cf.RootTag = c;
            cf.SaveFile(f);
            cf.Dispose(); // Pray.

            return GetChunk(X, Y);
        }

		protected NbtCompound NewNBTChunk(long X, long Y)
		{
			NbtCompound Level = new NbtCompound("Level");
			Level.Add(new NbtByte("TerrainPopulated", 0x00)); // Don't add ores, y/n? Usually get better performance with true on first load.
            Level.Add(new NbtInt("xPos", (int)X));
            Level.Add(new NbtInt("zPos", (int)Y));
            Level.Add(new NbtInt("LastUpdate", 0)); // idk what the format is, not going to decompile.
            Level.Add(new NbtByteArray("BlockLight", new byte[16384]));
            Level.Add(new NbtByteArray("Blocks", new byte[32768]));
            Level.Add(new NbtByteArray("Data", new byte[16384]));
            Level.Add(new NbtByteArray("HeightMap", new byte[256]));
            Level.Add(new NbtByteArray("SkyLight", new byte[16384]));
            Level.Add(new NbtList("Entities"));
            Level.Add(new NbtList("TileEntities"));

			NbtCompound Chunk = new NbtCompound("_ROOT_");
			Chunk.Add(Level);
			return Chunk;
		}

		public void ReplaceBlocksIn(long X, long Y, Dictionary<byte, byte> Replacements)
		{
			if (Replacements == null) return;
			Chunk c = GetChunk((int)X, (int)Y);
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
			if (mChunks.ContainsKey(ci))
			{
				mChunks.Remove(ci);
			}
			mChunks.Add(ci,c);
			//if(!ChangedChunks.Contains(ci))
			//    ChangedChunks.Add(ci);

			c.Save();
		}

		/// <summary>
		/// Decompress lighting data from the tiny array that is used for lighting.
		/// </summary>
		/// <param name="lightdata">Compressed lighting bytearray (from /Level/{BlockLight,SkyLight})</param>
		/// <param name="x">XCoord</param>
		/// <param name="y">YCoord</param>
		/// <param name="z">ZCoord</param>
		/// <returns>Lighting value at this position</returns>
		public int DecompressLight(byte[] lightdata, int x, int y, int z)
		{
	        int index = z + (y * 128) + (x * 128 * 16);
	        return lightdata[index >> 1];
		}

		public void CompressLight(ref byte[] lightdata, bool sky, int x, int y, int z, byte val)
        {
            int index = z + (y * 128) + (x * 128 * 16);
            int lightval = lightdata[index >> 1];
            if ((y % 2) !=0)
            {
                if (sky)
                {
                    lightval &= 0x0f;
                    lightval |= val << 4;
                } else {
                    lightval &= 0xf;
                    lightval |= val << 4;
                }
            }
            else
            {
                if (sky)
                {
                    lightval &= 0xf0;
                    lightval |= val;
                } else {
                    lightval &= 0xf0;
                    lightval |= val;
                }
            }
            lightdata[index >> 1] = val;
		}

		
		
		public byte DecompressData(byte[] data,int x, int y, int z)
		{
			int index = GetBlockIndex(x, y, z);
			if (index % 2 == 0) return (byte)(data[index/2] >> 4);
			else return (byte)(data[index/2] & 0xF);
		}
		public void CompressData(ref byte[] data,int x, int y, int z, byte b)
		{
			int index = GetBlockIndex(x, y, z);
			if (index % 2 == 0) data[index/2] = (byte)((data[index/2] & 0x0F) | (b << 4));
			else data[index/2] = (byte)((data[index/2] & 0xF) | (b & 0x0F));
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
				//if(_DEBUG) Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
				return 0x00;
			}
			 */

			string ci = string.Format("{0},{1}", CX, CY);
			int i = GetBlockIndex((int)pos.X, (int)pos.Y, (int)pos.Z);
			//try
			//{
			if (mChunks.ContainsKey(ci))
			{
				if (mChunks[ci] == null) return 0x00;
				return mChunks[ci].Blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
			}
			//}
			//catch (Exception)
			//{
			//    return 0x00;
			//}
			Chunk c = _LoadChunk((int)CX, (int)CY);

			if (!mChunks.ContainsKey(ci))
				mChunks.Add(ci, c);

			try
			{
				return c.Blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
			}
			catch (Exception)
			{
				return 0x00;
			}
		}

		public void CullChunk(long X, long Y)
		{
			string ci = string.Format("{0},{1}", X, Y);
			if(mChunks.ContainsKey(ci))
				mChunks.Remove(ci);
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
			if (mChunks.ContainsKey(ci))
			{
				mChunks[ci].Blocks[(int)pos.X,(int)pos.Y,(int)pos.Z] = type;
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
			mChangedChunks.Clear();
			if(_DEBUG) Console.WriteLine("BEGIN TRANSACTION");
		}
		public void CommitTransaction()
		{
			if(_DEBUG) Console.WriteLine("{0} chunks changed.", mChangedChunks.Count);
			foreach (string v in mChangedChunks)
			{
				string[] chunks = v.Split(',');
				SaveChunk(int.Parse(chunks[0]), int.Parse(chunks[1]));
			}
			if(_DEBUG) Console.WriteLine("COMMIT TRANSACTION");
		}
		public void SetBlockAt(Vector3i p, byte id)
		{
			//if(_DEBUG) Console.WriteLine("{0}", p);
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
				//if(_DEBUG) Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
				return;
			}
			string ci = string.Format("{0},{1}", CX, CZ);
			if (!mChunks.ContainsKey(ci))
				return;
			Chunk c = mChunks[ci];
			c.Blocks[x,y,z]=id;
			mChunks[ci] = c;
			if (!mChangedChunks.Contains(ci))
			{
				mChangedChunks.Add(ci);
				if(_DEBUG) Console.WriteLine(ci+" has changed");
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
			if (mRoot == null)
			{
				failreason = "root==null";
				return false;
			}
			if (mRoot.RootTag == null)
			{
				failreason = "root.RootTag==null";
				return false;
			}
			//NbtCompound Data = (NbtCompound)root.RootTag["Data"];
			//NbtCompound Player = (NbtCompound)Data["Player"];
			//NbtList pi = (NbtList)Player["Inventory"];
			NbtList pi = mRoot.Query<NbtList>("//Data/Player/Inventory");
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
			Slot.Add(new NbtByte("Count",(byte)Count));
			Slot.Add(new NbtShort("id",(short)itemID));
			Slot.Add(new NbtShort("Damage",(short)Damage));
			Slot.Add(new NbtByte("Slot", (byte)slot));

			NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
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
				pi.Add(Slot);
			Player["Inventory"] = pi;
			Data["Player"] = Player;
			mRoot.RootTag["Data"] = Data;
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
			NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
			NbtCompound Player = (NbtCompound)Data["Player"];
			NbtList pi = (NbtList)Player["Inventory"];
			Player["Inventory"] = pi;
			Data["Player"] = Player;
			mRoot.RootTag["Data"] = Data;
		}
		public void Repair()
		{
			throw new NotImplementedException();
		}
		public void ForEachCachedChunk(CachedChunkDelegate cmd)
		{
			Dictionary<string, Chunk> C = new Dictionary<string, Chunk>(mChunks);
			foreach (KeyValuePair<string, Chunk> k in C)
			{
				cmd(k.Value.Position.X, k.Value.Position.Y, k.Value);
			}
		}
		public void ForEachChunk(Chunk.ChunkModifierDelegate cmd)
		{
			string[] f = Directory.GetFiles(mFolder,"c*.*.dat",SearchOption.AllDirectories);
			int Complete=0;
			foreach (string file in f)
			{

				if (ForEachProgress != null)
					ForEachProgress(f.Length, Complete++);
				//if(_DEBUG) Console.WriteLine(Path.GetExtension(file));
				if (Path.GetExtension(file) == "dat") continue;
				if (file.EndsWith(".genlock")) continue;
				NbtFile nf = new NbtFile(file);
                try
                {
                    nf.LoadFile();
                    NbtCompound Level = (NbtCompound)nf.RootTag["Level"];
                    long x = (Level["xPos"] as NbtInt).Value;
                    long y = (Level["zPos"] as NbtInt).Value;
                    nf.Dispose();
                    cmd(x, y);
                }
                catch (Exception e)
                {
                    if (CorruptChunk != null)
                    {
                        Vector2i pos = GetChunkCoordsFromFile(file);
                        CorruptChunk(pos.X,pos.Y,"[" + Complete.ToString() + "]" + e.ToString(), file);
                    }
                //    continue;
                }
			}
			// This MUST be done.
			ForEachProgress = null;
		}

		public Vector2i GetChunkCoordsFromFile(string file)
        {
            return GetChunkCoordsFromFile(file,false);
        }
        public Vector2i GetChunkCoordsFromFile(string file,bool test)
        {
            Vector2i r = new Vector2i(0, 0);
            // cX.Y.dat
            // X.Y.dat
            string[] peices =Path.GetFileName(file.Substring(1)).Split('.');
            long X;
            long Y;
            Radix.Decode(peices[0],36,out X);
            Radix.Decode(peices[1],36,out Y);
            r.X = (int)X;
            r.Y = (int)Y;
            if (test)
            {
                NbtFile f = new NbtFile(file);
                try
                {
                    f.LoadFile();
                }
                catch (Exception e)
                {
                    if (CorruptChunk != null)
                        CorruptChunk(X,Y,e.ToString(), file);
                    return null;
                }
                NbtCompound Level = (NbtCompound)f.RootTag["Level"];
                Vector2i t = new Vector2i(0, 0);
                t.X = (Level["xPos"] as NbtInt).Value;
                t.Y = (Level["zPos"] as NbtInt).Value;
                f.Dispose();
                if (t != r)
                    throw new Exception("Test failed.");
            }
			return r;
		}

		IMapGenerator _Generator = new DefaultMapGenerator(0);
        private int mDimension;
		public IMapGenerator Generator
		{
			get
			{
				return _Generator;
			}
			set
			{
				_Generator = value;
				SaveMapGenerator();
			}
		}

		private void SaveMapGenerator()
		{
			File.WriteAllText(_Generator.GetType().Name, Path.Combine(mFolder, "mapgen.id"));
			_Generator.Save(mFolder);
		}

		private void LoadMapGenerator()
		{
			string f = Path.Combine(mFolder, "mapgen.id");
			if (File.Exists(f))
			{
				string mg = File.ReadAllText(f);
				_Generator = MapGenerators.Get(mg, RandomSeed);
				_Generator.Load(mFolder);
			}
		}

		public void ChunkModified(long x, long y)
		{

		}

		public Chunk GetChunk(long x, long y)
		{
			return GetChunk((int)x, (int)y, false);
		}

        public IEnumerable<Dimension> GetDimensions()
        {
            return new Dimension[]
            {
                new Dimension(0,"Default",""),
                new Dimension(1,"Nether","")
            };
        }

        public void SetDimension(int ID)
        {
            mDimension = ID;
            UnloadChunks();
        }
	}
}
