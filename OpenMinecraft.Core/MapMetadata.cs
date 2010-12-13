using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Threading;

namespace OpenMinecraft
{
    public class MapMetadata
    {
        ReaderWriterLock rwLock = new ReaderWriterLock();
        Thread mThread;
        public string Filename { get; set; }
        public List<string> TablesPresent { get; set; }
        private static readonly int VERSION=1;
        SQLiteConnection database;
        IMapHandler map;
       
        // CHUNKS(PK(cnkX, cnkZ), cnkTemperatureMap, cnkHumidityMap, cnkOriginalVoxels)
        // TREES(PK(treeX,treeZ), treeHeight, treeType)
        // DUNGEONS(PK(dgnPos), dgnSpawns)

        public MapMetadata(IMapHandler _map, string folder)
        {
            map=_map;
            Filename=Path.Combine(folder, "mineedit.db3");
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
            SQLiteTransaction trans = database.BeginTransaction();
            //mThread = new Thread(delegate()
            //{
                BuildDatabaseIfNeeded();
                map.SetBusy("Updating cache...");
                Console.WriteLine("Please wait, caching chunks...");
                foreach (Dimension dim in map.GetDimensions())
                {
                    map.ForEachChunkFile(dim.ID, delegate(IMapHandler _map, string file)
                    {
                        bool NeedsUpdate = false;
                        object ret = ExecuteScalarSQL("SELECT cnkMD5 FROM Chunks WHERE cnkFile='" + file + "';");
                        if(ret==null)
                            NeedsUpdate = true;
                        if (!NeedsUpdate)
                        {
                            if (ret.ToString() != GetMD5HashFromFile(file))
                                NeedsUpdate = true;
                        }
                        if (NeedsUpdate)
                        {
                            Vector2i pos = map.GetChunkCoordsFromFile(file);
                            if (pos == null) return;
                            //Console.WriteLine(string.Format("Updating chunk {0} in {1} ({2})...", pos, dim.Name, file));
                            map.SetBusy(string.Format("Updating chunk {0} in {1}...", pos, dim.Name));
                            Chunk c = map.GetChunk(pos.X, pos.Y);
                            map.SaveAll();
                        }
                    });
                }
                map.SetIdle();
            //});
            //mThread.Start();
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
            if (!TablesPresent.Contains("Cache") 
                || !TablesPresent.Contains("Dimensions")
                || !TablesPresent.Contains("Chunks")
                || !TablesPresent.Contains("Trees")
                || !TablesPresent.Contains("Entities")
                || !TablesPresent.Contains("Dungeons"))
                RebuildCache();

            object reportedVersion=ExecuteScalarSQL("SELECT cacheVersion FROM Cache;");
            if(reportedVersion==null || VERSION!=(int)((Int64)reportedVersion))
                RebuildCache();
        }

        private void RebuildCache()
        {
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
        }

        private void CreateDimensionsTable()
        {
            string sql = @"
CREATE TABLE IF NOT EXISTS Dimensions
(
    dimID       INTEGER,
    dimFolder   INTEGER,
    dimSizeX    INTEGER,
    dimSizeZ    INTEGER,
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
    cnkFlags INTEGER,
    PRIMARY KEY(cnkX,cnkZ,dimID)
);";
            ExecuteNonquerySQL(sql);
        }


        internal void SaveChunkMetadata(Chunk c)
        {
            try
            {
                rwLock.AcquireWriterLock(1000);
                using (SQLiteCommand cmd = database.CreateCommand())
                {
                    cmd.CommandText = "REPLACE INTO Chunks (cnkX, cnkZ, dimID, cnkMD5, cnkFile, cnkTemperatures, cnkHumidity, cnkFlags) VALUES (@cnkX,@cnkZ,@dimID,@cnkMD5,@cnkFile,@cnkTemperatures,@cnkHumidity,@cnkFlags);";
                    cmd.Parameters.Add(new SQLiteParameter("@cnkX", c.Position.X));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkZ", c.Position.Z));
                    cmd.Parameters.Add(new SQLiteParameter("@dimID", c.Dimension));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkMD5", GetMD5HashFromFile(c.Filename)));

                    byte[] tmap, hmap;
                    int idx = 0;
                    tmap = hmap = new byte[c.Size.X * c.Size.Z];
                    if (c.Temperatures != null && c.Humidity != null)
                    {
                        for (int x = 0; x < c.Size.X; x++)
                        {
                            for (int z = 0; z < c.Size.Z; z++)
                            {
                                tmap[idx] = (byte)(c.Temperatures[x, z] * 255);
                                hmap[idx++] = (byte)(c.Humidity[x, z] * 255);
                            }
                        }
                    }

                    cmd.Parameters.Add(new SQLiteParameter("@cnkTemperatures", DbType.Binary, tmap.Length,
    ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, tmap));
                    cmd.Parameters.Add(new SQLiteParameter("@cnkHumidity", DbType.Binary, hmap.Length,
    ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, hmap));


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
        }

        internal bool LoadChunkMetadata(ref Chunk c)
        {
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
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                    rwLock.ReleaseReaderLock();
            }
            return true;
        }
    }
}
