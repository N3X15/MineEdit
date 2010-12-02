using System;
using System.ComponentModel;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using LibNoise;

namespace OpenMinecraft
{
    public class QuickHillGenerator : IMapGenerator
    {

        protected long Seed;

        protected Random rand;
        // Main terrain noise (two combined Perlin noises)
        protected FastRidgedMultifractal TerrainNoise;
        protected Perlin ContinentNoise;
        protected Voronoi v;
        protected Perlin CaveNoise;
        protected Perlin GravelNoise;
        protected Perlin TreeNoise;

        public double mCaveThreshold = 0.85d;
        public const int WaterHeight = 63;
        public int MAX_DERT_DEPTH = 6;
        double mTerrainDivisor = 0.33;
        double mCaveDivisor = 2.0;
        private int mContinentNoiseOctaves=16;
        private MapGenMaterials mMats;

        public double Frequency { get; set; }
        public NoiseQuality NoiseQuality { get; set; }
        public int OctaveCount { get; set; }
        public double Lacunarity { get; set; }
        public double Persistance { get; set; }
        public double ContinentNoiseFrequency { get; set; }

        public override MapGenMaterials Materials
        {
            get
            {
                return mMats;
            }
            set
            {
                mMats = value;
            }
        }

        public double CaveThreshold
        {
            get { return mCaveThreshold; }
            set { mCaveThreshold = value; }
        }

        [Description("Z-axis stretching of the terrain.  (z*TerrainDivisor)")]
        public double TerrainDivisor
        {
            get { return mTerrainDivisor; }
            set { mTerrainDivisor = value; }
        }

        [Description("Z-axis stretching of cave systems.  (z*CaveDivisor)")]
        public double CaveDivisor
        {
            get { return mCaveDivisor; }
            set { mCaveDivisor = value; }
        }

        public override string Name
        {
            get
            {
                return "Quick Hills";
            }
        }

        public QuickHillGenerator(long seed)
        {
            Seed = seed;
            Setup();
        }

        public QuickHillGenerator() { Setup(); }

        private void Setup()
        {
            Frequency = 0.1;
            Lacunarity = 0.05;
            Persistance = 0.25;
            OctaveCount = mContinentNoiseOctaves = 4;

            TerrainNoise = new FastRidgedMultifractal();
            ContinentNoise = new Perlin();
            CaveNoise = new Perlin();
            GravelNoise = new Perlin();
            TreeNoise = new Perlin();
            Voronoi v = new Voronoi();
            TerrainNoise.Seed = (int)Seed;
            ContinentNoise.Seed = (int)Seed + 2;
            CaveNoise.Seed = (int)Seed + 3;
            GravelNoise.Seed = (int)Seed + 4;
            TreeNoise.Seed = (int)Seed + 4;
            rand = new Random((int)Seed);


            TerrainNoise.Frequency = Frequency;
            TerrainNoise.NoiseQuality = NoiseQuality;
            TerrainNoise.OctaveCount = OctaveCount;
            TerrainNoise.Lacunarity = Lacunarity;

            ContinentNoise.Frequency = ContinentNoiseFrequency;
            ContinentNoise.NoiseQuality = NoiseQuality;
            ContinentNoise.OctaveCount = mContinentNoiseOctaves;
            ContinentNoise.Lacunarity = Lacunarity;
            ContinentNoise.Persistence = Persistance;

            CaveNoise.Frequency = Frequency;
            CaveNoise.NoiseQuality = NoiseQuality;
            CaveNoise.OctaveCount = OctaveCount + 2;
            CaveNoise.Lacunarity = Lacunarity;
            CaveNoise.Persistence = Persistance;

            GravelNoise.Frequency = Frequency;
            GravelNoise.NoiseQuality = NoiseQuality;
            GravelNoise.OctaveCount = OctaveCount;
            GravelNoise.Lacunarity = Lacunarity;
            GravelNoise.Persistence = Persistance;

            TreeNoise.Frequency = Frequency + 2;
            TreeNoise.NoiseQuality = NoiseQuality;
            TreeNoise.OctaveCount = OctaveCount;
            TreeNoise.Lacunarity = Lacunarity;
            TreeNoise.Persistence = Persistance;

            v.Frequency = 1d;
            v.DistanceEnabled = true;
            v.Displacement = 1;
            v.Seed = (int)Seed;
        }

        /// <summary>
        /// From the VoxelSim project
        /// http://github.com/N3X15/VoxelSim
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Z"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public override byte[, ,] Generate(ref IMapHandler mh, long X, long Z)
        {
            Vector3i chunksize = mh.ChunkScale;

            int YH = (int)chunksize.Y-2;
            byte[, ,] b = new byte[chunksize.X, chunksize.Y, chunksize.Z];
            int minHeight = (int)chunksize.Y;
            /*
				Voronoi voronoi = new Voronoi(256, 4, 4, 1, 1f, seed);
				Channel cliffs = voronoi.getDistance(-1f, 1f, 0f).brightness(1.5f).multiply(0.33f);
				terrain.multiply(0.67f).channelAdd(cliffs);
				terrain.channelSubtract(voronoi.getDistance(1f, 0f, 0f).gamma(.5f).flipV().rotate(90))
            */
            for (int x = 0; x < chunksize.X; x++)
            {
                for (int z = 0; z < chunksize.Z; z++)
                {
                    double heightoffset = (ContinentNoise.GetValue((double)(x + (X * chunksize.X)) / 10d, (double)(z + (Z * chunksize.Z)) / 10d, 0) + 1d)/3d;// *5d; // 2.0
                    double height = 30 + heightoffset;
                    
                    //for(int o = 0;o<5;o++)
                        height+=(TerrainNoise.GetValue(x + (X * chunksize.X), z + (Z * chunksize.Z), 0) + heightoffset)/3.0;

                    
                    if (height < minHeight) minHeight = (int)height;
                    for (int y = 0; y < chunksize.Y; y++)
                    {
                        // If below height, set rock.  Otherwise, set air.
                        byte block = (y <= height) ? (byte)1 : (byte)0; //Fill
                        block=(y <= 63 && block==0) ? (byte)9 : block; // Water

                        // Only try to calc caves if we're in rock.  Otherwise we'll be slow and have holes in our water.
                        if (block == 1)
                        {
                            double _do = ((CaveNoise.GetValue(x + (X * chunksize.X), z + (Z * chunksize.Z), y * CaveDivisor) + 1) / 2.0);
                            bool d3 = _do > CaveThreshold;

                            if (d3)
                                block = 0;
                        }

                        b[x, y, z] = block;
                    }
                }
            }
            //Console.WriteLine("Generate (Quick): {0}", minHeight);
            return b;
        }


        public override bool GenerateCaves
        {
            get;
            set;
        }

        public override bool GenerateDungeons
        {
            get;
            set;
        }

        public override bool GenerateOres
        {
            get;
            set;
        }

        public override bool GenerateWater
        {
            get;
            set;
        }

        public override bool HellMode
        {
            get;
            set;
        }

        public override bool NoPreservation
        {
            get;
            set;
        }

        public override bool GenerateTrees
        {
            get;
            set;
        }

        public override void Save(string Folder)
        {
            string f = Path.Combine(Folder, "DefaultMapGenerator.dat");
            NbtFile nf = new NbtFile(f);
            nf.RootTag = new NbtCompound("__ROOT__");
            NbtCompound c = new NbtCompound("DefaultMapGenerator");
            c.Add(new NbtByte("GenerateCaves", (byte)(GenerateCaves ? 1 : 0)));
            c.Add(new NbtByte("GenerateDungeons", (byte)(GenerateDungeons ? 1 : 0)));
            c.Add(new NbtByte("GenerateOres", (byte)(GenerateOres ? 1 : 0)));
            c.Add(new NbtByte("GenerateWater", (byte)(GenerateWater ? 1 : 0)));
            c.Add(new NbtByte("HellMode", (byte)(HellMode ? 1 : 0)));
            c.Add(new NbtByte("GenerateTrees", (byte)(GenerateTrees ? 1 : 0)));
            c.Add(new NbtDouble("Frequency", Frequency));
            c.Add(new NbtByte("NoiseQuality", (byte)NoiseQuality));
            c.Add(new NbtInt("OctaveCount", OctaveCount));
            c.Add(new NbtDouble("Lacunarity", Lacunarity));
            c.Add(new NbtDouble("Persistance", Persistance));
            c.Add(new NbtDouble("ContinentNoiseFrequency", ContinentNoiseFrequency));
            c.Add(new NbtDouble("CaveThreshold", CaveThreshold));
            nf.RootTag.Add(c);
            nf.SaveFile(f);
        }

        public override void Load(string Folder)
        {
            string f = Path.Combine(Folder, "DefaultMapGenerator.dat");
            if (!File.Exists(f)) return;

            NbtFile nf = new NbtFile(f);
            nf.LoadFile(f);
            GenerateCaves = nf.Query<NbtByte>("/DefaultMapGenerator/GenerateCaves").Value == 0x01 ? true : false;
            GenerateDungeons = nf.Query<NbtByte>("/DefaultMapGenerator/GenerateDungeons").Value == 0x01 ? true : false;
            GenerateOres = nf.Query<NbtByte>("/DefaultMapGenerator/GenerateOres").Value == 0x01 ? true : false;
            GenerateWater = nf.Query<NbtByte>("/DefaultMapGenerator/GenerateWater").Value == 0x01 ? true : false;
            HellMode = nf.Query<NbtByte>("/DefaultMapGenerator/HellMode").Value == 0x01 ? true : false;
            GenerateTrees = nf.Query<NbtByte>("/DefaultMapGenerator/GenerateTrees").Value == 0x01 ? true : false;
            Frequency = nf.Query<NbtDouble>("/DefaultMapGenerator/Frequency").Value;
            NoiseQuality = (NoiseQuality)nf.Query<NbtByte>("/DefaultMapGenerator/NoiseQuality").Value;
            OctaveCount = nf.Query<NbtInt>("/DefaultMapGenerator/OctaveCount").Value;
            Lacunarity = nf.Query<NbtDouble>("/DefaultMapGenerator/Lacunarity").Value;
            Persistance = nf.Query<NbtDouble>("/DefaultMapGenerator/Persistance").Value;
            ContinentNoiseFrequency = nf.Query<NbtDouble>("/DefaultMapGenerator/ContinentNoiseFrequency").Value;
            CaveThreshold = nf.Query<NbtDouble>("/DefaultMapGenerator/CaveThreshold").Value;
        }
        public override string Author
        {
            get { return "Rob \"N3X15\" Nelson"; }
        }

        public override string Version
        {
            get { return "07292010"; }
        }
    }
}
