using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Threading;
using OpenMinecraft.Entities;
using OpenMinecraft.TileEntities;
using LibNbt.Tags;
using LibNbt;

namespace OpenMinecraft
{
    public class MapMetadata
    {
        bool mEnabled = true;
        public bool Enabled {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
                if (mEnabled)
                    UpdateCache();
            }
        }
        bool FirstCache = true;
        ReaderWriterLock rwLock = new ReaderWriterLock();
        public string Filename { get; set; }
        public List<string> TablesPresent { get; set; }
        private static readonly int VERSION=3;
        SQLiteConnection database;
        IMapHandler map;
        Dictionary<string, Vector3i> mFileCoords = new Dictionary<string, Vector3i>();
        private SQLiteTransaction mTransaction;

        public Dictionary<Guid, Entity> Entities = new Dictionary<Guid, Entity>();
        public Dictionary<Guid, TileEntity> TileEntities = new Dictionary<Guid, TileEntity>();
       
        // CHUNKS(PK(cnkX, cnkZ), cnkTemperatureMap, cnkHumidityMap, cnkOriginalVoxels)
        // TREES(PK(treeX,treeZ), treeHeight, treeType)
        // DUNGEONS(PK(dgnPos), dgnSpawns)

        public MapMetadata(IMapHandler _map, string folder)
        {
            map=_map;
            Filename=Path.Combine(folder, "mineedit.cache");
            FirstCache = !File.Exists(Filename);

            string dsn = string.Format(@"Data Source={0};Version=3", Filename);
            database = new SQLiteConnection(dsn);
            database.Open();
            //database.ChangeDatabase("MineEdit");
            BuildDatabaseIfNeeded();
        }

        private object ExecuteScalarSQL(string sql)
        {
            //Console.WriteLine(sql);
            object r = null;
            try
            {
                rwLock.AcquireReaderLock(300);
                using (SQLiteCommand cmd = database.CreateCommand())//new SQLiteCommand(sql, database))
                {
                    cmd.CommandText = sql;
                    r = cmd.ExecuteScalar();
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            return r;
        }

        private void ExecuteNonquerySQL(string sql)
        {
            //Console.WriteLine(sql);
            try
            {
                rwLock.AcquireWriterLock(300);
                using (SQLiteCommand cmd = new SQLiteCommand(sql, database))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }

        public void UpdateCache()
        {
            TileEntities.Clear();
            Entities.Clear();
            SQLiteTransaction trans = database.BeginTransaction();
            int numChunksChanged = 0;
            int numNewChunks = 0;
            int numChunksTotal = 0;
            //mThread = new Thread(delegate()
            //{
                map.SetBusy("Updating cache...");
                map.SetBusy(string.Format("Please wait, {0}...\r\n{1} new, {2} changed", (FirstCache) ? "creating cache (may take a while)" : "updating cache", numNewChunks, numChunksChanged));
                Dimension[] dims = (Dimension[])map.GetDimensions();
                Dimension dim = dims[map.Dimension];
                map.ForEachChunkFile(dim.ID, delegate(IMapHandler _map, string file)
                {
                    bool NeedsUpdate = false;
                    bool NewChunk = false;
                    using (SQLiteCommand cmd = database.CreateCommand())
                    {
                        Vector2i pos;
                        cmd.CommandText="SELECT cnkMD5,cnkX,cnkZ,dimID FROM Chunks WHERE cnkFile='" + file + "';";
                        SQLiteDataReader rdr = cmd.ExecuteReader();
                        if (!rdr.HasRows)
                        {
                            NewChunk = true;
                            NeedsUpdate = true;
                            pos = map.GetChunkCoordsFromFile(file,true);
                        } else {
                            pos=new Vector2i(
                                (int)((long)rdr["cnkX"]),
                                (int)((long)rdr["cnkZ"])
                                );
                            if(!mFileCoords.ContainsKey(file))
                                mFileCoords.Add(file,new Vector3i(pos.X,pos.Y,
                                    (int)((long)rdr["dimID"])));
                        }
                            
                        if (dim.MinimumChunk.X > pos.X) dim.MinimumChunk.X = pos.X;
                        if (dim.MaximumChunk.X < pos.X) dim.MaximumChunk.X = pos.X;
                        if (dim.MinimumChunk.Y > pos.Y) dim.MinimumChunk.Y = pos.Y;
                        if (dim.MaximumChunk.Y < pos.Y) dim.MaximumChunk.Y = pos.Y;

                        if (!NeedsUpdate)
                        {
                            if (rdr["cnkMD5"].ToString() != GetMD5HashFromFile(file))
                                NeedsUpdate = true;
                        }
                        if (NeedsUpdate)
                        {
                            if (NewChunk)
                                numNewChunks++;
                            else
                                numChunksChanged++;
                            if (pos == null) return;
                            //Console.WriteLine(string.Format("Updating chunk {0} in {1} ({2})...", pos, dim.Name, file));
                            map.SetBusy(string.Format("Please wait, {0}...\r\n{1} new, {2} changed", (FirstCache) ? "creating cache (may take a while)" : "updating cache", numNewChunks, numChunksChanged));
                            Chunk c = map.GetChunk(pos.X, pos.Y);
                            map.UnloadChunks();
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }

                    // Dump it from RAM onto disk to prevent bloat.
                    if (numChunksChanged + numNewChunks % 500 == 499)
                    {
                        Console.WriteLine("Saving cache...");
                        map.SetBusy(string.Format("Please wait, {0}...\r\n[Saving]", (FirstCache) ? "creating cache (may take a while)" : "updating cache"));
                        trans.Commit();
                        trans = database.BeginTransaction();
                        map.UnloadChunks();
                    }
                });
                UpdateDimension(dim);
                map.SetIdle();
                trans.Commit(); // NOW save to disk.
            //});
            //mThread.Start();
        }

        private void UpdateDimension(Dimension dim)
        {
            try
            {
                rwLock.AcquireWriterLock(300);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "REPLACE INTO Dimensions (dimID,dimFolder,dimMinX,dimMinZ,dimMaxX,dimMaxZ) VALUES (@dimID,@dimFolder,@dimMinX,@dimMinZ,@dimMaxX,@dimMaxZ);";
                    cmd.Parameters.AddRange(new SQLiteParameter[]{
                        new SQLiteParameter("@dimID",dim.ID),
                        new SQLiteParameter("@dimFolder",dim.Folder),
                        new SQLiteParameter("@dimMinX",dim.MinimumChunk.X),
                        new SQLiteParameter("@dimMinZ",dim.MinimumChunk.Y),
                        new SQLiteParameter("@dimMaxX",dim.MaximumChunk.X),
                        new SQLiteParameter("@dimMaxZ",dim.MaximumChunk.Y)
                    });
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }
        protected string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
 
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        private void BuildDatabaseIfNeeded()
        {
            TablesPresent = new List<string>();

            try
            {
                rwLock.AcquireReaderLock(300);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string name = rdr["name"].ToString();
                            if (!name.StartsWith("sqlite_"))
                            {
                                Console.WriteLine("[CACHE] Table {0} exists!", name);
                                TablesPresent.Add(name);
                            }
                        }
                    }
                }
            }
            finally
            {
                if(rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            /*
            [CACHE] Table Cache exists!
            [CACHE] Table Chunks exists!
            [CACHE] Table Dimensions exists!
            [CACHE] Table Dungeons exists!
            [CACHE] Table Entities exists!
            [CACHE] Table TileEntities exists!
            [CACHE] Table Trees exists!
            */
            if (!TablesPresent.Contains("Cache")
                || !TablesPresent.Contains("Chunks")
                || !TablesPresent.Contains("Dimensions")
                || !TablesPresent.Contains("Dungeons")
                || !TablesPresent.Contains("Entities")
                || !TablesPresent.Contains("Trees")
                || !TablesPresent.Contains("TileEntities"))
                RebuildCache();

            object reportedVersion=ExecuteScalarSQL("SELECT cacheVersion FROM Cache;");
            if(reportedVersion==null || VERSION!=(int)((Int64)reportedVersion))
                RebuildCache();
        }

        private void RebuildCache()
        {
            Console.WriteLine("*** REBUILDING CACHE! ***");
            lock (TablesPresent)
            {
                if (TablesPresent.Count > 0)
                {
                    foreach (string name in TablesPresent)
                    {
                        ExecuteNonquerySQL("DROP TABLE IF EXISTS " + name);
                    }
                }
            }
 	        CreateCacheTable();
            CreateDimensionsTable();
            CreateChunksTable();
            CreateDungeonsTable();
            CreateTreesTable();
            CreateEntitiesTable();
            CreateTileEntitiesTable();
        }

        private void CreateDimensionsTable()
        {
            string sql = @"
CREATE TABLE IF NOT EXISTS Dimensions
(
    dimID       INTEGER,
    dimFolder   TEXT,
    dimMinX     INTEGER,
    dimMinZ     INTEGER,
    dimMaxX     INTEGER,
    dimMaxZ     INTEGER,
    PRIMARY KEY (dimID)
);";
            ExecuteNonquerySQL(sql);
        }

        /// <summary>
        /// Simple table for detecting version number of cache.
        /// </summary>
        private void CreateCacheTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Cache
(
    cacheVersion INTEGER
);

INSERT INTO Cache (cacheVersion) VALUES (" + VERSION + ");";
            ExecuteNonquerySQL(sql);
        }

        // DUNGEONS(PK(dgnPos), dgnSpawns)
        private void CreateDungeonsTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Dungeons
(
    dgnX    INTEGER,
    dgnY    INTEGER,
    dgnZ    INTEGER,
    dimID   INTEGER
    dgnSpawns TEXT,
    PRIMARY KEY(dgnX,dgnY,dgnZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }
        private void CreateEntitiesTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Entities
(
    entX    REAL,
    entY    REAL,
    entZ    REAL,
    dimID   INTEGER,
    entType TEXT,
    entNBT  BLOB,
    PRIMARY KEY(entX,entY,entZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }
        private void CreateTileEntitiesTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS TileEntities
(
    tentX    INTEGER,
    tentY    INTEGER,
    tentZ    INTEGER,
    dimID   INTEGER,
    tentType TEXT,
    tentNBT  BLOB,
    PRIMARY KEY(tentX,tentY,tentZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }

        // TREES(PK(treeX,treeZ), treeHeight, treeType)
        private void CreateTreesTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Trees 
(
    treeX INTEGER,
    treeZ INTEGER,
    dimID INTEGER,
    treeHeight INTEGER,
    treeType TEXT,
    PRIMARY KEY(treeX,treeZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }

        // CHUNKS(PK(cnkX, cnkZ,dimID), cnkTemperatureMap, cnkHumidityMap, cnkFlags)
        private void CreateChunksTable()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Chunks 
(
    cnkX INTEGER,
    cnkZ INTEGER,
    dimID INTEGER,
    cnkMD5 TEXT,
    cnkFile TEXT,
    cnkTemperatures BLOB,
    cnkHumidity BLOB,
    cnkOverview BLOB,
    cnkFlags INTEGER,
    PRIMARY KEY(cnkX,cnkZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }

        public void BeginTransaction()
        {
            try
            {
                rwLock.AcquireWriterLock(300);
                mTransaction = database.BeginTransaction();
            }
            finally
            {
                if(rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                rwLock.AcquireWriterLock(300);
                mTransaction.Rollback();
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }

        public void CommitTransaction()
        {
            try
            {
                rwLock.AcquireWriterLock(300);
                mTransaction.Commit();
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }


        internal void SaveChunkMetadata(Chunk c)
        {
            if (!mEnabled) return;
            c.UpdateOverview();
            try
            {
                rwLock.AcquireWriterLock(1000);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "REPLACE INTO Chunks (cnkX, cnkZ, dimID, cnkMD5, cnkFile, cnkTemperatures, cnkHumidity, cnkOverview, cnkFlags) VALUES (@cnkX,@cnkZ,@dimID,@cnkMD5,@cnkFile,@cnkTemperatures,@cnkHumidity,@cnkOverview,@cnkFlags);";
                    cmd.Parameters.Add(new SQLiteParameter("@cnkX", c.Position.X));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkZ", c.Position.Z));
                    cmd.Parameters.Add(new SQLiteParameter("@dimID", c.Dimension));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkMD5", GetMD5HashFromFile(c.Filename)));

                    byte[] tmap, hmap, omap;
                    int idx = 0;
                    tmap = new byte[c.Size.X * c.Size.Z];
                    hmap = new byte[c.Size.X * c.Size.Z];
                    omap = new byte[c.Size.X * c.Size.Z];
                    if (c.Temperatures != null && c.Humidity != null)
                    {
                        for (int x = 0; x < c.Size.X; x++)
                        {
                            for (int z = 0; z < c.Size.Z; z++)
                            {
                                tmap[idx] = (byte)(c.Temperatures[x, z] * 255);
                                hmap[idx] = (byte)(c.Humidity[x, z] * 255);
                                omap[idx] = c.Overview[x, z];
                                idx++;
                            }
                        }
                    }
                    idx = 0;
                    for (int x = 0; x < c.Size.X; x++)
                    {
                        for (int z = 0; z < c.Size.Z; z++)
                        {
                            omap[idx] = c.Overview[x, z];
                            idx++;
                        }
                    }

                    cmd.Parameters.Add(new SQLiteParameter("@cnkTemperatures", DbType.Binary, tmap.Length,
    ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, tmap));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkHumidity", DbType.Binary, hmap.Length,
    ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, hmap));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkOverview", DbType.Binary, omap.Length,
    ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, omap));


                    cmd.Parameters.Add(new SQLiteParameter("@cnkFile", c.Filename));

                    int flags = 0;
                    if (c.TerrainPopulated)
                        flags |= 1;
                    if (c.GeneratedByMineEdit)
                        flags |= 2;
                    cmd.Parameters.Add(new SQLiteParameter("@cnkFlags", flags));
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }

            ExecuteNonquerySQL(
                string.Format(
@"DELETE FROM Entities 
WHERE 
    entX < {0}+15 AND 
    entY < {1}+127 AND 
    entZ < {2}+15 AND
    entX > {0} AND
    entY > {1} AND
    entZ > {2} AND
    dimID = {3};",
                 c.Position.X,
                 c.Position.Y,
                 c.Position.Z,
                 c.Dimension));

            ExecuteNonquerySQL(
                string.Format(
@"DELETE FROM TileEntities 
WHERE 
    tentX < {0}+15 AND 
    tentY < {1}+127 AND 
    tentZ < {2}+15 AND
    tentX > {0} AND
    tentY > {1} AND
    tentZ > {2} AND
    dimID = {3};",
                 c.Position.X,
                 c.Position.Y,
                 c.Position.Z,
                 c.Dimension));

            try
            {
                rwLock.AcquireWriterLock(1000);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "REPLACE INTO Entities (entX,entY,entZ,dimID,entType,entNBT) VALUES (@entX,@entY,@entZ,@dimID,@entType,@entNBT)";
                    cmd.Parameters.AddRange(new SQLiteParameter[] 
                {
                    new SQLiteParameter("@entX"),
                    new SQLiteParameter("@entY"),
                    new SQLiteParameter("@entZ"),
                    new SQLiteParameter("@dimID"),
                    new SQLiteParameter("@entType"),
                    new SQLiteParameter("@entNBT")
                });
                    cmd.Prepare();
                    foreach (Entity e in c.Entities.Values)
                    {
                        cmd.Parameters["@entX"].Value = e.Pos.X;
                        cmd.Parameters["@entY"].Value = e.Pos.Y;
                        cmd.Parameters["@entZ"].Value = e.Pos.Z;
                        cmd.Parameters["@dimID"].Value = c.Dimension;
                        cmd.Parameters["@entType"].Value = e.GetID();
                        cmd.Parameters["@entNBT"].Value = NBT2Bytes(e.ToNBT());
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception) { }
                        if (Entities.ContainsKey(e.UUID))
                            Entities[e.UUID] = e;
                        else
                            Entities.Add(e.UUID, e);
                    }
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }

            try
            {
                rwLock.AcquireWriterLock(1000);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "REPLACE INTO TileEntities (tentX,tentY,tentZ,dimID,tentType,tentNBT) VALUES (@tentX,@tentY,@tentZ,@dimID,@tentType,@tentNBT)";
                    cmd.Parameters.AddRange(new SQLiteParameter[] 
                    {
                        new SQLiteParameter("@tentX"),
                        new SQLiteParameter("@tentY"),
                        new SQLiteParameter("@tentZ"),
                        new SQLiteParameter("@dimID"),
                        new SQLiteParameter("@tentType"),
                        new SQLiteParameter("@tentNBT")
                    });
                    cmd.Prepare();
                    foreach (TileEntity e in c.TileEntities.Values)
                    {
                        cmd.Parameters["@tentX"].Value = e.Pos.X;
                        cmd.Parameters["@tentY"].Value = e.Pos.Y;
                        cmd.Parameters["@tentZ"].Value = e.Pos.Z;
                        cmd.Parameters["@dimID"].Value = c.Dimension;
                        cmd.Parameters["@tentType"].Value = e.GetID();
                        cmd.Parameters["@tentNBT"].Value = NBT2Bytes(e.ToNBT());
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception) { }
                        if (TileEntities.ContainsKey(e.UUID))
                            TileEntities[e.UUID] = e;
                        else
                            TileEntities.Add(e.UUID, e);
                    }
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                    rwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Load cached data into the chunk
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal bool LoadChunkMetadata(ref Chunk c)
        {
            if (!mEnabled) return true;
            try
            {
                rwLock.AcquireReaderLock(1000);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT cnkFile, cnkTemperatures, cnkHumidity, cnkFlags FROM Chunks WHERE cnkX={0} AND cnkZ={1} AND dimID={2};", c.Position.X, c.Position.Z, map.Dimension);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows) return false;

                    rdr.Read(); // Load up first row

                    int i = 0;
                    c.Filename = rdr.GetString(i++);

                    byte[] tmap, hmap;
                    tmap = hmap = new byte[c.Size.X * c.Size.Z];

                    rdr.GetBytes(i++, 0, tmap, 0, (int)(c.Size.X * c.Size.Z));
                    rdr.GetBytes(i++, 0, hmap, 0, (int)(c.Size.X * c.Size.Z));

                    int idx = 0;
                    c.Temperatures = c.Humidity = new double[c.Size.X, c.Size.Z];
                    for (int x = 0; x < c.Size.X; x++)
                    {
                        for (int z = 0; z < c.Size.Z; z++)
                        {
                            c.Temperatures[x, z] = ((double)tmap[idx] / 255d);
                            c.Humidity[x, z] = ((double)hmap[idx++] / 255d);
                        }
                    }
                    int flags = rdr.GetInt32(i++);
                    c.TerrainPopulated = (flags & 1) == 1;
                    c.GeneratedByMineEdit = (flags & 2) == 2;
                    c.Cached = true;
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            return true;
        }

        private object NBT2Bytes(NbtCompound nbtCompound)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                NbtFile f = new NbtFile();
                f.RootTag = nbtCompound;
                f.SaveFile(ms,false);
                //ms.Position = 0;
                //byte[] buffer = new byte[ms.Length];
                //ms.Read(buffer, 0, buffer.Length);
                return ms.ToArray();
            }
        }

        internal Vector2i GetChunkCoords(string file)
        {
            Vector2i pos=null;
            if (mFileCoords.ContainsKey(file))
            {
                Vector3i coords = mFileCoords[file];
                return new Vector2i((int)coords.X, (int)coords.Y); // Z is dimension
            }
            Console.WriteLine("[CACHE] Couldn't find file " + file + " in the coordinate cache. Trying database...");
            try
            {
                rwLock.AcquireReaderLock(300);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "SELECT cnkX,cnkZ FROM Chunks WHERE cnkFile='" + file + "';";
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        pos = new Vector2i(
                            (int)((long)rdr["cnkX"]),
                            (int)((long)rdr["cnkZ"])
                        );
                    }
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            return pos;
        }

        public IEnumerable<Vector2i> GetKnownChunks(int dimension)
        {
            List<Vector2i> known = new List<Vector2i>();
            try
            {
                rwLock.AcquireReaderLock(300);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "SELECT cnkX,cnkZ FROM Chunks WHERE dimID=" + dimension.ToString() + " ORDER BY cnkX,cnkZ;";
                    SQLiteDataReader rdr =cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Vector2i cpos = new Vector2i(
                            (int)(long)rdr["cnkX"],
                            (int)(long)rdr["cnkZ"]
                        );
                        if (!known.Contains(cpos))
                            known.Add(cpos);
                    }
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            return known;
        }
    }
}
