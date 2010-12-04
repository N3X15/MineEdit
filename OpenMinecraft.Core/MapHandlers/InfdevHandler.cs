#define CATCH_CHUNK_ERRORS

using System;
using System.Collections.Generic;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using LibNoise;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;

namespace OpenMinecraft
{
	public class InfdevHandler:IMapHandler
	{

#if DEBUG
        public bool _DEBUG = true; // Improve performance.  Maybe.
#else
        public bool _DEBUG = false; // Improve performance.  Maybe.
#endif

        public override event CorruptChunkHandler CorruptChunk;
        public override event ForEachProgressHandler ForEachProgress;

        private string mFolder;
		private const int ChunkX = 16;
        private const int ChunkY = 128;
		private const int ChunkZ = 16;

        private bool mAutoRepair = false;

		NbtFile mRoot = new NbtFile();
        NbtFile mChunk = new NbtFile();
        Dictionary<Vector2i, Chunk> mChunks = new Dictionary<Vector2i, Chunk>();
        Dictionary<Vector2i, double[,]> mChunkHeightmaps = new Dictionary<Vector2i, double[,]>();
        List<Vector2i> mChangedChunks = new List<Vector2i>();
		Dictionary<Guid, Entity> mEntities = new Dictionary<Guid, Entity>();
		Dictionary<Guid, TileEntity> mTileEntities = new Dictionary<Guid, TileEntity>();

        public override string Filename { get; set; }
        public override int InventoryCapacity { get { return 9 * 4; } }
        public override int ChunksLoaded { get { return mChunks.Count; } }
        public override Vector3i ChunkScale { get { return new Vector3i(16, 128, 16); } }
        public override bool HasMultipleChunks { get { return true; } }
        public override Vector3i MapMin { get { return new Vector3i(-1000, 0, -1000); } }
        public override Vector3i MapMax { get { return new Vector3i(1000, 127, 1000); } }
        public override bool Autorepair
        {
            get { return mAutoRepair; }
            set { mAutoRepair = value; }
        }

        /// <summary>
        /// Spawn position (Y/Z positions flipped)
        /// </summary>
        public override Vector3i Spawn
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
				Player.Add("SpawnZ", new NbtInt("SpawnY",(int)Utils.Clamp(value.Y, 0, 128)));
				Player.Add("SpawnY", new NbtInt("SpawnZ",(int)value.Z));
				Data["Player"] = Player;
				mRoot.RootTag["Data"] = Data;
			}
		}

        public override int Height
		{
			get
			{
				return 10000;
			}
		}

        public override int Width
		{
			get
			{
				return 10000;
			}
		}

        public override int Depth
		{
			get
			{
				return 128;
			}
		}

		/// <summary>
		/// -1 = dead?
		/// 0 = dead
		/// 20 = full health
		/// </summary>
        public override int Health
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
			}
		}

		// Set to 0 to avoid the Laying Down bug.
        public override int HurtTime
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
        public override int Air
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
			}
		}

		// Dunno the range
        public override int Fire
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
        public override int Time
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

        /// <summary>
        /// Player position (Y/Z flipped)
        /// </summary>
        public override Vector3d PlayerPos
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

        public override Dictionary<Guid, Entity> Entities
		{
			get
			{
				return mEntities;
			}
		}

        public override Dictionary<Guid, TileEntity> TileEntities
		{
			get
			{
				return mTileEntities;
			}
		}

        public override long RandomSeed
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

        public override void Load()
		{
			Load(Filename);
		}
        public override void Load(string filename)
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


		public override Chunk GetChunk(Vector3i chunkpos)
		{
			return GetChunk(chunkpos.X, chunkpos.Y);
		}
		public override void GetOverview(int CX,int CY,Vector3i pos, out int h, out byte block, out int waterdepth)
		{
			h = 0;
			block = 0;
			waterdepth = 0;

			int x = (int)pos.X;
			int y = (int)pos.Y;
			//int z = (int)pos.Z;// % ChunkY;
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

		public override Vector3i GetMousePos(Vector3i mp, int scale, ViewAngle angle)
		{
			Vector3i p = new Vector3i(0,0,0);
			switch(angle)
			{
				case ViewAngle.FrontSlice:
					p.X = mp.X / scale;
					p.Y = ChunkZ-(mp.Y / scale);
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
            for (int x = 0; x < ChunkX; x++)
            {
                for (int y = 0; y < ChunkY; y++)
                {
                    for (int z = 0; z < ChunkZ; z++)
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
			for (int x = 0; x < ChunkX; x++)
			{
				for (int y = 0; y < ChunkY; y++)
				{
					for (int z = 0; z < ChunkZ; z++)
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
            for (int x = 0; x < ChunkX; x++)
            {
                for (int y = 0; y < ChunkY; y++)
                {
                    for (int z = 0; z < ChunkZ; z++)
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
            for (int x = 0; x < ChunkX; x++)
            {
                for (int y = 0; y < ChunkY; y++)
                {
                    for (int z = 0; z < ChunkZ; z++)
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
                    0,
					level.Get<NbtInt>("zPos").Value);

				if ((int)c.Position.X != x || (int)c.Position.Z != z)
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
                //if(_DEBUG) Console.WriteLine("*** Found {0} Entities.",Entities.Tags.Count);
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

				Vector2i ci = GetChunkHandle(x, z);
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
                                c.TileEntities.Add(ms.UUID, ms);
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
                if (mAutoRepair)
                {
                    File.Delete(c.Filename);
                    return null;
                }
				if(_DEBUG) Console.WriteLine(err);
				if (CorruptChunk != null)
					CorruptChunk(x,z,err, c.Filename);
				return null;
			}
#endif
        }

        private Vector2i GetChunkHandle(int x, int z)
        {
            return new Vector2i(x, z);
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
                e.Pos.X = e.Pos.X + (CX * (double)ChunkX);
                e.Pos.Z = e.Pos.Z + (CZ * (double)ChunkZ);

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
                te.Pos.X = te.Pos.X + (CX * ChunkX);
                te.Pos.Z = te.Pos.Z + (CZ * ChunkZ);

				te.UUID = Guid.NewGuid();
				mTileEntities.Add(te.UUID, te);
                cnk.TileEntities.Add(te.UUID, te); // DURP
			}
		}
		
		public override void SetTileEntity(TileEntity e)
		{
			long CX = e.Pos.X / ChunkX;
            long CZ = e.Pos.Z / ChunkZ;

            int x = (int)e.Pos.X - (((int)e.Pos.X >> 4) * ChunkX); //(px >> 4) & 0xf;
            int y = (int)e.Pos.Y;
            int z = (int)e.Pos.Z - (((int)e.Pos.Z >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(CX, CZ);
            if (c == null) return;
            
            if (c.TileEntities.ContainsKey(e.UUID))
                c.TileEntities[e.UUID] = e;
            else
                c.TileEntities.Add(e.UUID, e);

            SetChunk(c);

		}

		public override void RemoveTileEntity(TileEntity e)
		{
			long CX = e.Pos.X / ChunkX;
            long CZ = e.Pos.Z / ChunkZ;

            int x = (int)e.Pos.X - (((int)e.Pos.X >> 4) * ChunkX); //(px >> 4) & 0xf;
            int y = (int)e.Pos.Y;
            int z = (int)e.Pos.Z - (((int)e.Pos.Z >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(CX, CZ);
            if (c == null) return;

            if (c.TileEntities.ContainsKey(e.UUID))
                c.TileEntities.Remove(e.UUID);

            if (mTileEntities.ContainsKey(e.UUID))
                mTileEntities.Remove(e.UUID);

            SetChunk(c);
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
            Vector2i id = GetChunkHandle(x, z);
			Chunk c;
            double min, max;
			if (!mChunks.TryGetValue(id, out c))
			{
				if(File.Exists(GetChunkFilename(x,z)))
					return _LoadChunk(x, z);
				if (GenerateNewChunkIfNeeded)
				{
					Generate(x, z, out min, out max);
					return GetChunk(x, z);
				}
				return null;
			}
			return c;
		}

        public override void SetChunk(Chunk cnk)
        {
            Vector2i id = GetChunkHandle((int)cnk.Position.X, (int)cnk.Position.Z);
            if (mChunks.ContainsKey(id))
                mChunks[id] = cnk;
            else
                mChunks.Add(id, cnk);
        }

        public override void SetChunkHeightmap(int X, int Z, double[,] cnk)
        {
            Vector2i id = GetChunkHandle(X, Z);
            if (mChunkHeightmaps.ContainsKey(id))
                mChunkHeightmaps[id] = cnk;
            else
                mChunkHeightmaps.Add(id, cnk);
        }

        public override double[,] GetChunkHeightmap(int X, int Z)
        {
            Vector2i i = GetChunkHandle(X, Z);
            if(mChunkHeightmaps.ContainsKey(i))
                return mChunkHeightmaps[i];
            return null;
        }

		public override void SaveChunk(Chunk cnk)
		{
			NbtFile c = new NbtFile(cnk.Filename);

            // TODO: PosY -> PosZ
            c.RootTag = NewNBTChunk(cnk.Position.X, cnk.Position.Z);
            NbtCompound Level = (NbtCompound)c.RootTag["Level"];

			// BLOCKS /////////////////////////////////////////////////////
			byte[] blocks = new byte[ChunkX * ChunkZ * ChunkY];
			for (int x = 0; x < ChunkX; x++)
			{
				for (int y = 0; y < ChunkY; y++)
				{
					for (int z = 0; z < ChunkZ; z++)
					{
						blocks[GetBlockIndex(x, y, z)] = cnk.Blocks[x, y, z];
					}
				}
            }
            Level.Set("xPos", new NbtInt("xPos", (int)cnk.Position.X));
            Level.Set("zPos", new NbtInt("zPos", (int)cnk.Position.Z));
            Level.Set("Blocks", new NbtByteArray("Blocks", blocks));
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
			for (int x = 0; x < ChunkX; x++)
			{
				for (int z = 0; z < ChunkZ; z++)
				{
                    hm[z + (x << 4)] = (byte)cnk.HeightMap[x, z];
				}
			}
            Level.Set("HeightMap", new NbtByteArray("HeightMap", hm));


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

            // Tick when last saved.  But ticks are process associated...
            Level.Set("LastUpdate", new NbtLong("LastUpdate", (long)Utils.UnixTimestamp()));

			c.RootTag.Set("Level",Level);
			
            c.SaveFile(GetChunkFilename((int)cnk.Position.X,(int)cnk.Position.Z));
			
            // For debuggan
            File.WriteAllText(cnk.Filename + ".txt", c.RootTag.ToString());
        }

        public override Vector3i Local2Global(int CX, int CZ, Vector3i local)
        {
            Vector3i r = new Vector3i(local);
            r.X += CX * ChunkX;
            r.Z += CZ * ChunkZ;
            return r;
        }
        public override Vector3i Global2Local(Vector3i global, out int CX, out int CZ)
        {
            Vector3i r = new Vector3i(global);
            CX = (int)r.X / ChunkX;
            CZ = (int)r.Z / ChunkZ;

            r.X = r.X - ((r.X >> 4) * ChunkX); //(px >> 4) & 0xf;
            r.Z = r.Z - ((r.Z >> 4) * ChunkZ); //(py >> 4) & 0xf;
            return r;
        }

        public override Vector3d Local2Global(int CX, int CZ, Vector3d local)
        {
            Vector3d r = local;
            r.X += CX * ChunkX;
            r.Z += CZ * ChunkZ;
            return r;
        }
        public override Vector3d Global2Local(Vector3d global, out int CX, out int CZ)
        {
            Vector3d r = global;
            CX = (int)r.X / ChunkX;
            CZ = (int)r.Z / ChunkZ;

            r.X = r.X % ChunkX; //(px >> 4) & 0xf;
            r.Z = r.Z % ChunkZ; //(py >> 4) & 0xf;
            return r;
        }

		public override bool IsMyFiletype(string f)
		{
			return Path.GetFileName(f).Equals("level.dat");
		}

		public override bool Save()
		{
			File.Copy(Filename, Path.ChangeExtension(Filename, "bak"),true);
			mRoot.SaveFile(Filename);
			return true;
		}

		public override bool Save(string filename)
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
        public override void AddEntity(Entity e)
        {
            int CX = (int)(e.Pos.X / ChunkX);
            int CZ = (int)(e.Pos.Z / ChunkZ);
            e.Pos.X = (int)e.Pos.X % ChunkX;
            e.Pos.Z = (int)e.Pos.Z % ChunkZ;

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
		public override void SetEntity(Entity e)
		{
			Guid ID = e.UUID;

			if (mEntities.ContainsKey(ID))
				mEntities.Remove(ID);
			mEntities.Add(ID, e);

            int CX, CZ;

            e.Pos = Global2Local(e.Pos, out CX, out CZ);

			Chunk c = GetChunk(CX, CZ);
            if (c == null) return;
			if (c.Entities.ContainsKey(e.UUID))
				c.Entities.Remove(e.UUID);
			c.Entities.Add(e.UUID, e);
			SetChunk(c);
		}

		public override void RemoveEntity(Entity e)
		{
            // Get our UUID (why UUIDs, Rob? WTF)
			Guid ID = e.UUID;

            // Is this entity loaded?
			if (mEntities.ContainsKey(ID))
				mEntities.Remove(ID);

            // Try and translate global coords to chunk local coords
            // TODO: What the fuck are you doing
            int CX, CZ;
            e.Pos = Global2Local(e.Pos, out CX, out CZ);

            // Find parent chunk, remove
			Chunk c = GetChunk(CX, CZ);
            if (c == null) return;
            c.Entities.Remove(e.UUID);
            SetChunk(c);
		}

		private int GetBlockIndex(int x, int y, int z)
		{
			//return y * ChunkY + x * ChunkY * ChunkX + z;
            //return y + (z * ChunkY + (x * ChunkY * ChunkZ)); // WIKI LIES
            //return z * ChunkY + x * ChunkY * ChunkX + y;  
            //return y + (z * 128) + (x * 128 * 16);
            return x << 11 | z << 7 | y;
		}
        

		public override byte GetBlockAt(int px, int y, int pz)
		{
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return 0;
			return c.Blocks[x, y, z];
		}

		public override void GetLightAt(int px, int y, int pz, out byte skyLight, out byte blockLight)
        {
            skyLight = 0;
            blockLight = 0;
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return;
			skyLight=c.SkyLight[x, y, z];
			blockLight=c.BlockLight[x, y, z];
		}

		public override void SetBlockAt(int px, int y, int pz, byte val)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return;
			c.Blocks[x, y, z] = val;
			SetChunk(X, Z, c);
        }

        public override void SetSkyLightAt(int px, int y, int pz, byte val)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return ;
            c.SkyLight[x, y, z] = val;
            SetChunk(X, Z, c);
        }

        public override void SetBlockLightAt(int px, int y, int pz, byte val)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return;
            c.BlockLight[x, y, z] = val;
            SetChunk(X, Z, c);
        }

        public override byte GetSkyLightAt(int px, int y, int pz)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return 0;
            return c.SkyLight[x, y, z];
        }

        public override byte GetBlockLightAt(int px, int y, int pz)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return 0;
            return c.BlockLight[x, y, z];
        }

		private void SetChunk(int X, int Z, Chunk c)
        {
            Vector2i id = GetChunkHandle(X, Z);
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

        public override int ExpandFluids(byte fluidID, bool CompleteRegen, ForEachProgressHandler ph)
        {
            int total = 0;
            int chunksProcessed=0;
            ForEachChunk(delegate(IMapHandler mh, long X, long Z)
            {
                total += ExpandFluids(X, Z, fluidID, CompleteRegen);
                ph(-1, ++chunksProcessed);
            });
            return total;
        }

		private int ExpandFluids(long X, long Z, byte fluidID, bool CompleteRegen)
        {
            Chunk chunk = GetChunk(X, Z);
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
                                GetBlockAt(x, y+1, z) == fluidID // Above
                                || GetBlockAt(x + 1, y, z) == fluidID  // Sides
                                || GetBlockAt(x - 1, y, z) == fluidID
                                || GetBlockAt(x, y, z+1) == fluidID
                                || GetBlockAt(x, y, z - 1) == fluidID)
                            {
                                SetBlockAt(x, y, z, fluidID);
                            }
                        }
                    }
                }
            }
            SaveAll();
            return BlocksChanged;
        }


        Perlin treeNoise;
        Random dungeonNoise;

        Random rand = new Random();

		public override bool Generate(long X, long Z, out double min, out double max)
        {
            min = 0;
            max = (double)ChunkY;
            if (treeNoise == null)
            {
                treeNoise = new Perlin();
                treeNoise.Seed = (int)this.RandomSeed + 4;
                treeNoise.Frequency = 1;
                treeNoise.Persistence = 0.5;
                treeNoise.OctaveCount = 1;
                dungeonNoise = new Random((int)RandomSeed);
            }
            if (_Generator == null)
            {
                return false;
            }
			string lockfile = Path.ChangeExtension(GetChunkFilename((int)X,(int)Z), "genlock");
			if (!_Generator.NoPreservation)
			{
				if (File.Exists(lockfile))
					return true;
			}
			else
			{
				if (File.Exists(lockfile))
					File.Delete(lockfile);
			}

            //Console.WriteLine("GEN");
			double[,] hm = _Generator.Generate(this, X, Z,out min, out max);

            if (hm == null)
            {
                Console.WriteLine("ERROR: hm==null");
                return false;
            }

            // Erosion shit here.

            Chunk _c = NewChunk(X, Z);
            byte[, ,] blocks = _c.Blocks;

            //Console.WriteLine("BIOME");
            BiomeType[,] biomes = _Generator.DetermineBiomes(hm, X, Z);

            IMapHandler mh = this;
            // These use the block array.

            //Console.WriteLine("VOXELIZE");
            HeightmapToVoxelspace(hm, ref blocks);                                       AssertBottomBarrierIntegrity(blocks, "HeightmapToVoxelspace");

            //Console.WriteLine("ADDSOIL");
            _Generator.AddSoil(ref blocks, biomes, 63, 6, _Generator.Materials);         AssertBottomBarrierIntegrity(blocks, "AddSoil");

            //Console.WriteLine("DUNGEONS");
            _Generator.AddDungeons(ref blocks, ref mh, dungeonNoise, X, Z);              AssertBottomBarrierIntegrity(blocks, "AddDungeons");

            //Console.WriteLine("PRECIP");
            _Generator.Precipitate(ref blocks, biomes, _Generator.Materials, X, Z);      AssertBottomBarrierIntegrity(blocks, "Precipitate");
            mh.SaveAll();

            _c.Blocks = blocks;
            _c.UpdateOverview();
            _c.Save();
            SetChunk(_c);
            File.WriteAllText(lockfile, _Generator.ToString());


            //Console.WriteLine("TREES");
            // These use SetBlockAt() and company.
            //_Generator.AddTrees(ref mh, biomes, ref rand, (int)X, (int)Z, (int)ChunkY);

            //Console.WriteLine("SAVE");
            SaveAll();
            return true;
		}

        private void AssertBottomBarrierIntegrity(byte [,,] b,string message)
        {
            return; // I know they're OK in this version.
            //Console.WriteLine("Assert "+message+"...");
            for (int x = 0; x < b.GetLength(0); x++)
            {
                for (int z = 0; z < b.GetLength(2); z++)
                {
                    if (b[x, 0, z] != 7)
                    {
                        Console.WriteLine(message+" failed to retain blocks, bottom barrier compromised.");
                        Environment.Exit(0);
                    }
                }
            }
            //Console.WriteLine(message+"OK");
        }

        private Dictionary<byte, int> GetBlockNumbers(byte[, ,] b)
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

		public override Chunk NewChunk(long X, long Y)
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

		protected NbtCompound NewNBTChunk(long X, long Z)
		{
			NbtCompound Level = new NbtCompound("Level");
			Level.Add(new NbtByte("TerrainPopulated", 0x00)); // Don't add ores, y/n? Usually get better performance with true on first load.
            Level.Add(new NbtInt("xPos", (int)X));
            Level.Add(new NbtInt("zPos", (int)Z));
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

		public override void ReplaceBlocksIn(long X, long Z, Dictionary<byte, byte> Replacements)
		{
			if (Replacements == null) return;
			Chunk c = GetChunk((int)X, (int)Z);
			if (c == null) return;

			bool bu=false;
			for(int x = 0;x<ChunkX;x++)
			{
				for (int y = 0; y < ChunkZ; y++)
				{
					for (int z = 0; z < ChunkY; z++)
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
            Vector2i ci = GetChunkHandle((int)X, (int)Z);
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
            int index = GetBlockIndex(x, y, z);
	        return lightdata[index >> 1];
		}

		public void CompressLight(ref byte[] lightdata, bool sky, int x, int y, int z, byte val)
        {
            int index = GetBlockIndex(x, y, z);
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
		
		public byte GetBlockIn(long CX, long CZ, Vector3i pos)
		{
			pos = new Vector3i(pos.Y, pos.X, pos.Z);
			/*
			if (
				!Check(pos.X, -1, ChunkX) ||
				!Check(pos.Y, -1, ChunkZ) ||
				!Check(pos.Z, -1, ChunkY))
			{
				//if(_DEBUG) Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
				return 0x00;
			}
			 */

            Vector2i ci = GetChunkHandle((int)CX, (int)CZ);
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
			Chunk c = _LoadChunk((int)CX, (int)CZ);

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

		public override void CullChunk(long X, long Z)
        {
            Vector2i ci = GetChunkHandle((int)X, (int)Z);
			if(mChunks.ContainsKey(ci))
				mChunks.Remove(ci);
            GC.Collect();
		}
		public override void LoadChunk(long X, long Y)
		{
			_LoadChunk((int)X, (int)Y);
		}

		public void SetBlockIn(long CX, long CZ, Vector3i pos, byte type)
		{
			pos = new Vector3i(pos.Y, pos.X, pos.Z);
			// block saving to any negative chunk due to being unreadable.
			if (CX < 0 || CZ < 0) return;

            Vector2i ci = GetChunkHandle((int)CX, (int)CZ);
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

		public override void BeginTransaction()
		{
			mChangedChunks.Clear();
			if(_DEBUG) Console.WriteLine("BEGIN TRANSACTION");
		}
		public override void CommitTransaction()
		{
			if(_DEBUG) Console.WriteLine("{0} chunks changed.", mChangedChunks.Count);
            foreach (Vector2i v in mChangedChunks)
			{
                Chunk c = mChunks[v];
				SaveChunk(c);
			}
			if(_DEBUG) Console.WriteLine("COMMIT TRANSACTION");
		}
		public void SetBlockAt(Vector3i p, byte id)
		{
			//if(_DEBUG) Console.WriteLine("{0}", p);
			int CX = (int)p.X >> 4;// / (long)ChunkX);
			int CZ = (int)p.Y >> 4;// / (long)ChunkZ);

			int x = ((int)p.Y % ChunkX) & 0xf;
			int y = ((int)p.X % ChunkZ) & 0xf;
			int z = (int)p.Z;// % ChunkY;

			if (
				!Check(x, -1, ChunkX) ||
				!Check(y, -1, ChunkZ) ||
				!Check(z, -1, ChunkY))
			{
				//if(_DEBUG) Console.WriteLine("<{0},{1},{2}> out of bounds", x, y, z);
				return;
			}
            Vector2i ci = GetChunkHandle(CX, CZ);
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

		public override int InventoryColumns
		{
			get
			{
				return 9;
			}
		}
		public override int InventoryOnHandCapacity
		{
			get
			{
				return 9;
			}
		}

		public override bool GetInventory(int slot, out short itemID, out short Damage, out byte Count, out string failreason)
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

		public override bool SetInventory(int slot, short itemID, int Damage, int Count)
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

		public override bool SetArmor(ArmorType Armor, short itemID, int Damage, int Count)
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

		public override bool GetArmor(ArmorType Armor, out short itemID, out short Damage, out byte Count, out string failreason)
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
		public override void ClearInventory()
		{
			NbtCompound Data = (NbtCompound)mRoot.RootTag["Data"];
			NbtCompound Player = (NbtCompound)Data["Player"];
			NbtList pi = (NbtList)Player["Inventory"];
			Player["Inventory"] = pi;
			Data["Player"] = Player;
			mRoot.RootTag["Data"] = Data;
		}
		public override void Repair()
		{
			throw new NotImplementedException();
		}
		public override void ForEachCachedChunk(CachedChunkDelegate cmd)
		{
			List<Chunk> cl = new List<Chunk>(mChunks.Values);
			foreach (Chunk c in cl)
			{
				cmd(c.Position.X, c.Position.Z, c);
			}
		}
		public override void ForEachChunk(ChunkIteratorDelegate cmd)
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
                    cmd(this, x, y);
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

		public override Vector2i GetChunkCoordsFromFile(string file)
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

		IMapGenerator _Generator = new QuickHillGenerator(0);
        private int mDimension;
		public override IMapGenerator Generator
		{
			get
			{
				return _Generator;
			}
			set
			{
				_Generator = value;
				SaveMapGenerator();
                _Generator.SetupBiomeNoise((int)this.RandomSeed);
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
				_Generator = MapGenerators.Get(mg, RandomSeed,new MapGenMaterials());
				_Generator.Load(mFolder);
			}
		}

        private void AddLightSourceAt(int _x, int _y, int _z, int strength)
        {
            Console.WriteLine("AddLightSourceAt(x={0},y={1},z={2},strength={3});", _x, _y, _z, strength);
            int written = 0;
            // 15 light
            // Loss of 1 light per block + STOP value
            // 15.5 block radius = 31 block diameter = 31 block bounding box
            for (int x = 0; x < (strength * 2) + 1; ++x)
            {
                for (int y = 0; y < (strength * 2) + 1; ++y)
                {
                    for (int z = 0; z < (strength * 2) + 1; ++z)
                    {
                        int absolute_x=x+strength+_x-1;
                        int absolute_y=y+strength+_y-1;
                        int absolute_z=z+strength+_z-1;

                        if (absolute_z < 0 || absolute_z > 128)
                            continue;

                        int d = Vector3i.Distance(new Vector3i(_x, _y, _z),new Vector3i(absolute_x, absolute_y, absolute_z));

                        byte block = GetBlockAt(absolute_x, absolute_y, absolute_z);
                        Block b = Blocks.Get(block);

                        int light = strength - d - b.Stop;
                        if (light < 0) light = 0;

                        // If we're not adding anything, go see if there's any blocks that will.
                        if (light == 0) continue;
                        
                        // If the current light value in this block is brighter than what we're putting in, don't bother.
                        byte blight = GetBlockLightAt(absolute_x, absolute_y, absolute_z);
                        if (blight >= light) continue;

                        // Yay, add shit.
                        SetBlockLightAt(absolute_x, absolute_y, absolute_z, (byte)light);
                        written++;
                    }
                }
            }
            //SaveAll();
            Console.WriteLine("{0} blocks lighted.", written);
        }

        /// <summary>
        /// Adapted from MineServer map.cpp
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public override bool RegenerateLighting(long x, long z)
        {
#if DEBUG
//            printf("generateLight(x=%d, z=%d, chunk=%p)\n", x, z, chunk);
#endif
            Chunk chunk = GetChunk(x, z);
            byte[,,] blocks = chunk.Blocks;
            byte[, ,] skylight = new byte[ChunkX, ChunkY, ChunkZ];//chunk.SkyLight;
            byte[, ,] blocklight = new byte[ChunkX, ChunkY, ChunkZ];//chunk.BlockLight;
            byte[,] heightmap = new byte[ChunkX, ChunkZ]; //chunk.HeightMap;

            int highest_y = 0;

            // Sky light
            int light = 0;
            bool foundheight = false;
            for(int block_x = 0; block_x < ChunkX; block_x++)
            {
                for(int block_z = 0; block_z < ChunkZ; block_z++)
                {
                    light = 15;
                    foundheight = false;

                    for(int block_y = 127; block_y > 0; block_y--)
                    {
                        int absolute_x = (int)x*ChunkX+block_x;
                        int absolute_z = (int)z*ChunkZ+block_z;
                        byte block = blocks[block_x,block_z,block_y];

                        int stopLight = Blocks.Get(block).Stop;
                        int emitLight = Blocks.Get(block).Emit;

                        light -= stopLight;
                        if (light < 0) { light = 0; }

                        // Calculate heightmap while looping this
                        if ((stopLight > 0) && (foundheight == false)) 
                        {
                            heightmap[block_x,block_z] = (byte)((block_y == 127) ? block_y : block_y + 1);
                            foundheight = true;
                        }

                        if (light < 1) 
                        {
                            if (block_y > highest_y)
                            highest_y = block_y;

                            break;
                        }

                        SetSkyLightAt(absolute_x, absolute_z, block_y, (byte)light);
                    }
                }
            }
  
            // Block light
            for (int block_x = 0; block_x < ChunkX; block_x++)
            {
                for (int block_z = 0; block_z < ChunkZ; block_z++)
                {
                    for (int block_y = highest_y; block_y >= 0; block_y--)
                    {
                        int absolute_x = (int)x*ChunkX+block_x;
                        int absolute_z = (int)z*ChunkZ+block_z;
                        byte block = blocks[block_x,block_z,block_y];

                        int stopLight = Blocks.Get(block).Stop;
                        int emitLight = Blocks.Get(block).Emit;

                        // If light emitting block
                        if (emitLight > 0)
                        {
                            //SetBlockLightAt(absolute_x, absolute_z, block_y, (byte)emitLight);
                            AddLightSourceAt(absolute_x, absolute_z, block_y, emitLight);
                        }
                    }
                }
            }
  
            /*
            // Spread light
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = heightmap[block_x,block_z]; block_y >= 0; block_y--)
                    {
                        int absolute_x = (int)x * 16 + block_x;
                        int absolute_z = (int)z * 16 + block_z;

                        byte skylight_s, blocklight_s;

                        GetLightAt(absolute_x, absolute_z, block_y, out skylight_s, out blocklight_s);

                        if (skylight_s!=0 || blocklight_s!=0)
                            SpreadLight(absolute_x, block_y, absolute_z, skylight_s, blocklight_s);
                    }
                }
            }
            */
            SaveAll();
            return true;
        }

        bool SpreadLight(int x, int y, int z, int skylight, int blocklight, int recurselevel=0)
        {

            byte block,stopLight,emitLight;
            Block meta;

            // If no light, stop!
            if((skylight < 1) && (blocklight < 1))
                return false;

            for(int i = 0; i < 6; i++)
            {
                // Going too high
                if((y == 127) && (i == 2))
                i++;
                // going negative
                if((y == 0) && (i == 3))
                i++;

                int x_toset = x;
                int y_toset = y;
                int z_toset = z;

                switch(i)
                {
                    case 0: x_toset++; break;
                    case 1: x_toset--; break;
                    case 2: y_toset++; break;
                    case 3: y_toset--; break;
                    case 4: z_toset++; break;
                    case 5: z_toset--; break;
                }

                block = GetBlockAt(x_toset, z_toset, y_toset);
                meta = Blocks.Get(block);
                stopLight = meta.Stop;
                emitLight = meta.Emit;

                if (true)
                {
                    byte skylightCurrent, blocklightCurrent;
                    int skylightNew, blocklightNew;
                    bool spread = false;

                    skylightNew = skylight-stopLight-1;
                    if (skylightNew < 0)
                    skylightNew = 0;

                    blocklightNew = blocklight-stopLight-1;
                    if (blocklightNew < 0)
                    blocklightNew = 0;

                    GetLightAt(x_toset, z_toset, y_toset, out skylightCurrent, out blocklightCurrent);

                    if (skylightNew > skylightCurrent)
                    {
                        skylightCurrent = (byte)skylightNew;
                        spread = true;
                    }

                    if (blocklightNew > blocklightCurrent)
                    {
                        blocklightCurrent = (byte)blocklightNew;
                        spread = true;
                    }

                    if (spread)
                    {
                        SetSkyLightAt(x_toset, z_toset, y_toset, skylightCurrent);
                        SetBlockLightAt(x_toset, z_toset, y_toset, blocklightCurrent);

                        SpreadLight(x_toset, y_toset, z_toset, skylightCurrent, blocklightCurrent, recurselevel+1);
                    }
                }
            }

            return true;
        }

		public override void ChunkModified(long x, long y)
		{

		}

		public override Chunk GetChunk(long x, long y)
		{
			return GetChunk((int)x, (int)y, false);
		}

        public override IEnumerable<Dimension> GetDimensions()
        {
            return new Dimension[]
            {
                new Dimension(0,"Default",""),
                new Dimension(1,"Nether","")
            };
        }

        public override void SetDimension(int ID)
        {
            mDimension = ID;
            UnloadChunks();
        }

        public override int GetHeightAt(int px, int py)
        {
            int X = px / ChunkX;
            int Y = py / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int y = py - ((py >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Y);
            return c.HeightMap[x, y];
        }

        internal override double GetPrelimHeightAt(int px, int py)
        {
            int X = px / ChunkX;
            int Z = py / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int y = py - ((py >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Vector2i i = GetChunkHandle(X, Z);
            if (mChunkHeightmaps.ContainsKey(i))
                return mChunkHeightmaps[i][x, y];
            return 0;
        }

        internal override void SetPrelimHeightAt(int px, int py, double val)
        {
            int X = px / ChunkX;
            int Z = py / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int y = py - ((py >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Vector2i i = GetChunkHandle(X, Z);
            if(mChunkHeightmaps.ContainsKey(i))
                mChunkHeightmaps[i][x, y] = val;
        }


        public override void SaveAll(bool cullheightmaps=true)
        {
            lock(mChunks)
            {
                List<Vector2i> Keys = new List<Vector2i>(mChunks.Keys);
                foreach (Vector2i k in Keys)
                {
                    mChunks[k].Save();
                }

                // CULL, CULL LIKE NO TOMORROW
                mChunks.Clear();
                mChangedChunks.Clear();
                mEntities.Clear();
                mTileEntities.Clear();
                //if(cullheightmaps)
                //    mChunkHeightmaps.Clear();
                //GC.Collect();
            }
        }


        public override void SetChunk(long X, long Y, Chunk c)
        {
            SetChunk((int)X, (int)Y, c);
        }


        public override void CullUnchanged()
        {
            List<Vector2i> chunkKeys = new List<Vector2i>(mChunks.Keys);
            foreach (Vector2i k in chunkKeys)
            {
                if (!mChangedChunks.Contains(k))
                    mChunks.Remove(k);
            }
            mChangedChunks.Clear();
        }

        public override void SetHeightAt(int x, int z, int h, byte mat)
        {
            int oh=GetHeightAt(x,z);
            // Ignore air/water/lava
            List<byte> excused_blocks=new List<byte>(new byte[]{0,8,9,10,11});
            for (int y = 0; y < ChunkY; y++)
            {
                byte block = GetBlockAt(x,y,z);
                bool excused = excused_blocks.Contains(block);
                if (!excused && h < y) block = 0;   // REMOVE SHIT
                if (excused && h >= y) block = mat; // ADD DERT
                if (block==3 && h == y) block = 2;  // ADD GRASS
                SetBlockAt(x, y, z, block);
            }
            int X = x / ChunkX;
            int Z = z / ChunkZ;

            int _x = x - ((z >> 4) * ChunkX); //(px >> 4) & 0xf;
            int _z = z - ((z >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            c.HeightMap[_x, _z]=h;
            SetChunk(c);
        }

        public override void SetDataAt(int px, int y, int pz, byte p)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return;
            c.Data[x, y, z] = p;
        }

        public override byte GetDataAt(int px, int y, int pz)
        {
            int X = px / ChunkX;
            int Z = pz / ChunkZ;

            int x = px - ((px >> 4) * ChunkX); //(px >> 4) & 0xf;
            int z = pz - ((pz >> 4) * ChunkZ); //(py >> 4) & 0xf;

            Chunk c = GetChunk(X, Z);
            if (c == null) return 0;
            return c.Data[x, y, z];
        }
    }
}
