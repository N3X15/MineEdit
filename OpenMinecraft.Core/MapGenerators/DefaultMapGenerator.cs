using System;
using System.Collections.Generic;

using System.Text;
using LibNoise;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using System.ComponentModel;

namespace OpenMinecraft
{
    public class DefaultMapGenerator:IMapGenerator
    {
        
        protected long Seed;

        protected Random rand;
        protected Perlin CaveNoise;

        double _CaveDivisor = 2.0;
        public double _CaveThreshold = 0.85d;//0.70d;
        public int WaterHeight = 65;
        public int DERT_DEPTH = 6;
        private MapGenMaterials mMats;
        [Description("Frequency of the terrain noise.")]
        public double Frequency { get; set; }
        [Description("Quality of the noise generated (lower = faster)")]
        public NoiseQuality NoiseQuality { get; set; }
        [Browsable(false)]
        public int OctaveCount { get; set; }
        [Description("\"Gappiness\" of the fractal noise.")]
        public double Lacunarity { get; set; }
        [Description("TODO Oh god what is this")]
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
        [Description("Bigger caves = slightly smaller number (0.70 = hueg), Smaller caves = slightly smaller (0.85=decent size)")]
        public double CaveThreshold
        {
            get
            {
                return _CaveThreshold;
            }
            set { _CaveThreshold = value; }
        }
        [Description("Z-axis stretching of cave systems.  (z*CaveDivisor)")]
        public double CaveDivisor
        {
            get { return _CaveDivisor; }
            set { _CaveDivisor = value; }
        }
        [Browsable(false)]
        public override string Name
        {
            get
            {
                return "Flat";
            }
        }
        public DefaultMapGenerator()
        {
            Frequency = 0.03;
            Lacunarity = 0.01;
            Persistance = 0.01;
            OctaveCount = 1;

            CaveNoise = new Perlin();
            CaveNoise.Seed = (int)Seed + 3;
            rand = new Random((int)Seed);

            CaveNoise.Frequency = Frequency;
            CaveNoise.NoiseQuality = NoiseQuality;
            CaveNoise.OctaveCount = OctaveCount + 2;
            CaveNoise.Lacunarity = Lacunarity;
            CaveNoise.Persistence = Persistance;
        }

        public DefaultMapGenerator(long seed):
            this()
        {
            Seed = seed;
        }

        /// <summary>
        /// From the VoxelSim project
        /// http://github.com/N3X15/VoxelSim
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Z"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public override byte[, ,] Generate(ref IMapHandler mh, long X, long Z, out int minHeight, out int maxHeight)
        {
            minHeight = (int)mh.ChunkScale.Y;
            maxHeight = 0;

            Vector3i chunksize = mh.ChunkScale;

            int YH = (int)chunksize.Y;
            byte[, ,] b = new byte[chunksize.X, chunksize.Y, chunksize.Z];
            bool[, ,] cavemap = new bool[chunksize.X, chunksize.Y, chunksize.Z];
            for (int y = 0; y < YH; y++)
            {
                for (int x = 0; x < chunksize.X; x++)
                {
                    for (int z = 0; z < chunksize.Z; y++)
                    {
                        //Console.WriteLine("HeightOffset {0}",heightoffset);
                        cavemap[x, y, z] = false;
                        if (y == 0)
                            b[x, y, z] = 7; // Adminite layer
                        else if (y == 1)
                            b[x, y, z] = 11;
                        else
                        {

                            double _do = ((CaveNoise.GetValue(x + (X * chunksize.X), y + (Z * chunksize.Y), z * CaveDivisor) + 1) / 2.0);
                            bool d3 = _do > CaveThreshold;
                            if(z<=WaterHeight+7)//if (!(!d1 || !d2))
                            {
                                // If in water...
                                if (d3 && (b[x, y, z] == 8 || b[x, y, z] == 9))
                                {
                                    b[x, y, z] = 1;
                                }
                                b[x, y, z] = (d3) ? b[x,y,z] : (byte)1;
                                cavemap[x, y, z] = d3;
                            }
                            // Sand setup
                        }
                    }
                }
            }

            return b;
        }
        [Browsable(false)]
        public override bool GenerateCaves { get; set; }
        [Browsable(false)]
        public override bool GenerateDungeons { get; set; }
        [Browsable(false)]
        public override bool GenerateOres { get; set; }
        [Browsable(false)]
        public override bool GenerateWater { get; set; }
        [Browsable(false)]
        public override bool HellMode { get; set; }
        [Browsable(false)]
        public override bool NoPreservation { get; set; }
        [Browsable(false)]
        public override bool GenerateTrees { get; set; }

        public override void Save(string Folder)
        {
            string f = Path.Combine(Folder, "DefaultMapGenerator.dat");
            NbtFile nf = new NbtFile(f);
            nf.RootTag = new NbtCompound("__ROOT__");
            NbtCompound c = new NbtCompound("DefaultMapGenerator");
            c.Add(new NbtByte("GenerateCaves", (byte) (GenerateCaves ? 1 : 0)));
            c.Add(new NbtByte("GenerateDungeons", (byte) (GenerateDungeons ? 1 : 0)));
            c.Add(new NbtByte("GenerateOres", (byte) (GenerateOres ? 1 : 0)));
            c.Add(new NbtByte("GenerateWater", (byte) (GenerateWater ? 1 : 0)));
            c.Add(new NbtByte("HellMode", (byte) (HellMode ? 1 : 0)));
            c.Add(new NbtByte("GenerateTrees", (byte) (GenerateTrees ? 1 : 0)));
            c.Add(new NbtDouble("Frequency", Frequency));
            c.Add(new NbtByte("NoiseQuality", (byte) NoiseQuality));
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
            NoiseQuality = (NoiseQuality) nf.Query<NbtByte>("/DefaultMapGenerator/NoiseQuality").Value;
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
            get { return "08192010"; }
        }
    }
}
