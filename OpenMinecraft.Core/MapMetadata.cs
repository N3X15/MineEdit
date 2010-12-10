using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace OpenMinecraft.Core
{
    public class MapMetadata
    {

        public string Filename { get; set; }
        public List<string> TablesPresent { get; set; }
        private static readonly int VERSION=1;
        SQLiteConnection database;
        IMapHandler map;
       
        // CHUNKS(PK(cnkX, cnkZ), cnkTemperatureMap, cnkHumidityMap, cnkOriginalVoxels)
        // TREES(PK(treeX,treeZ), treeHeight, treeType)
        // DUNGEONS(PK(dgnPos), dgnSpawns)

        public MapMetadata(ref IMapHandler _map, string folder)
        {
            map=_map;
            Filename=Path.Combine(folder, "mineedit.cache");
            string dsn = string.Format("Data Source={0}", Filename);
            database = new SQLiteConnection(dsn);
            database.Open();
            database.ChangeDatabase("MineEdit");

            BuildDatabaseIfNeeded();

            UpdateCache();
        }

        public void UpdateCache()
        {
            foreach (Dimension dim in map.GetDimensions())
            {
                map.ForEachChunkFile(dim.ID, delegate(IMapHandler _map, string file)
                {
                        bool NeedsUpdate = false;
                        using (SQLiteCommand cmd = database.CreateCommand())
                        {
                            cmd.CommandText = "SELECT cnkMD5 FROM Chunks WHERE cnkFile='" + file + "'";
                            SQLiteDataReader rdr = cmd.ExecuteReader();
                            if (!rdr.HasRows) NeedsUpdate = true;
                            if (!NeedsUpdate)
                            {
                                if (rdr.GetString(0) != GetMD5HashFromFile(file))
                                    NeedsUpdate = true;
                            }
                        }
                        if (NeedsUpdate)
                        {
                            Vector2i pos = map.GetChunkCoordsFromFile(file);

                            using (SQLiteCommand cullcmd = database.CreateCommand())
                            {
                            }
                            Chunk c = map.GetChunk(pos.X, pos.Y);

                        }
                });
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
            
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText="SELECT name FROM sqlite_master WHERE type='table';";
                SQLiteDataReader rdr = cmd.ExecuteReader();
                if(rdr.HasRows)
                {
                    while (rdr.NextResult())
                    {
                        string name = rdr.GetString(0);
                        if(!name.StartsWith("sqlite_"))
                            TablesPresent.Add(name);
                    }
                }
            }
            if (!TablesPresent.Contains("Cache") 
                || !TablesPresent.Contains("Dimensions")
                || !TablesPresent.Contains("Chunks")
                || !TablesPresent.Contains("Trees")
                || !TablesPresent.Contains("Entities")
                || !TablesPresent.Contains("Dungeons"))
                RebuildCache();

            int reportedVersion=-1;
            using(SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText="SELECT cacheVersion FROM Cache;";
                reportedVersion=(int)cmd.ExecuteScalar();
            }
            if(VERSION!=reportedVersion)
                RebuildCache();
        }

        private void RebuildCache()
        {
            string cmd = "DROP TABLE "+string.Join(",",TablesPresent.ToArray());
 	        CreateCacheTable();
            CreateDimensionsTable();
            CreateChunksTable();
            CreateDungeonsTable();
            CreateTreesTable();
        }

        private void CreateDimensionsTable()
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE Dimensions 
(
    dimID       INTEGER,
    dimFolder   INTEGER,
    dimSizeX    INTEGER,
    dimSizeZ    INTEGER,
    dimMinX     INTEGER,
    dimMinZ     INTEGER,
    dimMaxX     INTEGER,
    dimMaxZ     INTEGER
);";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Simple table for detecting version number of cache.
        /// </summary>
        private void CreateCacheTable()
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE Cache 
(
    cacheVersion INTEGER
);
INSERT INTO Cache ("+VERSION+");";
                cmd.ExecuteNonQuery();
            }
        }

        // DUNGEONS(PK(dgnPos), dgnSpawns)
        private void CreateDungeonsTable()
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE Dungeons 
(
    dgnX    INTEGER,
    dgnY    INTEGER,
    dgnZ    INTEGER,
    dimID   INTEGER
    dgnSpawns TEXT,
    PRIMARY KEY(dgnX,dgnY,dgnZ,dimID)
);";
                cmd.ExecuteNonQuery();
            }
        }

        // TREES(PK(treeX,treeZ), treeHeight, treeType)
        private void CreateTreesTable()
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE Trees (
                    treeX INTEGER,
                    treeZ INTEGER,
                    dimID INTEGER,
                    treeHeight INTEGER,
                    treeType TEXT,
                    PRIMARY KEY(treeX,treeY,dimID)
                    );";
                cmd.ExecuteNonQuery();
            }
        }

        // CHUNKS(PK(cnkX, cnkZ,dimID), cnkTemperatureMap, cnkHumidityMap, cnkFlags)
        private void CreateChunksTable()
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE Chunks (
                    cnkX INTEGER,
                    cnkZ INTEGER,
                    dimID INTEGER,
                    cnkFile TEXT,
                    cnkTemperatures BLOB,
                    cnkHumidity BLOB,
                    cnkFlags INTEGER,
                    PRIMARY KEY(cnkX,cnkZ,dimID)
                    );";
                cmd.ExecuteNonQuery();
            }
        }


        internal void SaveChunkMetadata(Chunk c)
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText="INSERT INTO Chunks (cnkX, cnkZ, dimID, cnkFile, cnkTemperatures, cnkHumidity, cnkFlags) VALUES (@cnkX,@cnkZ,@dimID,@cnkFile,@cnkTemperatures,@cnkHumidity,@cnkFlags)";
                cmd.Parameters.Add(new SQLiteParameter("@cnkX", c.Position.X));
                cmd.Parameters.Add(new SQLiteParameter("@cnkZ", c.Position.Z));
                cmd.Parameters.Add(new SQLiteParameter("@dimID", c.Dimension));

                byte[] tmap,hmap;
                int idx = 0;
                tmap=hmap= new byte[c.Size.X * c.Size.Z];
                for (int x = 0; x < c.Size.X; x++)
                {
                    for (int z = 0; z < c.Size.Z; z++)
                    {
                        tmap[idx] = (byte)(c.Temperatures[x, z] * 255);
                        hmap[idx++] = (byte)(c.Humidity[x, z] * 255);
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
            }
        }

        internal bool LoadChunkMetadata(ref Chunk c)
        {
            using (SQLiteCommand cmd = database.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT cnkFile, cnkTemperatures, cnkHumidity, cnkFlags FROM Chunks WHERE cnkX={0} AND cnkZ={1} AND dimID={2}", c.Position.X, c.Position.Z, map.Dimension);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                if (!rdr.HasRows) return false;

                int i = 0;
                c.Filename = rdr.GetString(i++);
                
                byte[] tmap,hmap;
                tmap=hmap= new byte[c.Size.X * c.Size.Y * c.Size.Z];

                rdr.GetBytes(i++, 0, tmap, 0, (int)(c.Size.X * c.Size.Y * c.Size.Z));
                rdr.GetBytes(i++, 0, hmap, 0, (int)(c.Size.X * c.Size.Y * c.Size.Z));

                int idx = 0;
                tmap = hmap = new byte[c.Size.X * c.Size.Z];
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
            return true;
        }
    }
}
