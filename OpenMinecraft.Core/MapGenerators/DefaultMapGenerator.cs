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
        protected Perlin TreeNoise;

        public double _CaveThreshold = 0.85d;//0.70d;
        public int WaterHeight = 65;
        public int DERT_DEPTH = 6;
        private int ContinentNoiseOctaves;
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
        [Description("Bigger caves = slightly smaller number (0.70 = hueg), Smaller caves = slightly smaller (0.85=decent size)")]
        public double CaveThreshold
        {
            get
            {
                return _CaveThreshold;
            }
            set { _CaveThreshold = value; }
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
            ContinentNoiseFrequency = Frequency / 2.0;
            Lacunarity = 0.01;
            Persistance = 0.01;
            OctaveCount = 1;
        }

        public DefaultMapGenerator(long seed)
        {
            Frequency = 0.03;
            Lacunarity = 0.01;
            Persistance = 0.01;
            OctaveCount = 1;

            Seed = seed;
            CaveNoise = new Perlin();
            TreeNoise = new Perlin();
            CaveNoise.Seed = (int)Seed + 3;
            TreeNoise.Seed = (int)Seed + 4;
            rand = new Random((int)Seed);

            CaveNoise.Frequency = Frequency;
            CaveNoise.NoiseQuality = NoiseQuality;
            CaveNoise.OctaveCount = OctaveCount+2;
            CaveNoise.Lacunarity = Lacunarity;
            CaveNoise.Persistence = Persistance;

            TreeNoise.Frequency = Frequency+2;
            TreeNoise.NoiseQuality = NoiseQuality;
            TreeNoise.OctaveCount = OctaveCount;
            TreeNoise.Lacunarity = Lacunarity;
            TreeNoise.Persistence = Persistance;
        }

        /// <summary>
        /// From the VoxelSim project
        /// http://github.com/N3X15/VoxelSim
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public override byte[, ,] Generate(ref IMapHandler mh, long X, long Y)
        {
            Vector3i chunksize = mh.ChunkScale;

            int ZH = (int)chunksize.Z;
            byte[, ,] b = new byte[chunksize.X, chunksize.Y, chunksize.Z];
            bool[, ,] cavemap = new bool[chunksize.X, chunksize.Y, chunksize.Z];
            byte Sand=12;
            byte Grass=2;
            byte Soil=3;
            byte Water=9;
            if (HellMode)
            {
                Sand = 49; // Obsidian
                Grass = Soil;
                Water = 11; // Lava
            }
            for (int z = 0; z < ZH; z++)
            {
                for (int x = 0; x < chunksize.X; x++)
                {
                    for (int y = 0; y < chunksize.Y; y++)
                    {
                        //Console.WriteLine("HeightOffset {0}",heightoffset);
                        cavemap[x, y, z] = false;
                        if (z == 0)
                            b[x, y, z] = 7; // Adminite layer
                        else
                        {

                            double _do = ((CaveNoise.GetValue(x + (X * chunksize.X), y + (Y * chunksize.Y), z * 2.0) + 1) / 2.0);
                            bool d3 = _do > CaveThreshold;
                            // XOR?
                            if(z<=WaterHeight+7)//if (!(!d1 || !d2))
                            {
                                //Console.Write("#");
                                b[x, y, z] = (d3) ? b[x,y,z] : (byte)1;
                                cavemap[x, y, z] = d3;
                                //if (x == 0|| y == 0)
                                //    b[x, y, z] = 41;
                            }
                            else if (z == 1)
                                b[x, y, z] = 11;
                        }
                    }
                }
            }
            //Console.WriteLine("Done generating chunk.  [{0},{1}]",min,max);
            // Add soil, sand

            for (int x = 0; x < chunksize.X; x++)
            {
                //Console.WriteLine();
                for (int y = 0; y < chunksize.Y; y++)
                {
                    bool HavePloppedGrass = false;
                    bool HaveTouchedSoil = false;
                    byte BlockForThisColumn = 0;
                    for (int z = (int)chunksize.Z - 1; z > 0; z--)
                    {
                        if (b[x, y, z] == 1)
                        {
                            HaveTouchedSoil=true;
                            if (z + DERT_DEPTH >= ZH)
                                continue;
                            byte ddt = b[x, y, z + DERT_DEPTH];
                            switch (ddt)
                            {
                                case 0: // Air
                                case 8: // Water
                                case 9: // Water
                                    if (BlockForThisColumn == 0)
                                    {
                                        if (z - DERT_DEPTH <= WaterHeight)
                                        {
                                            b[x, y, z] = Sand; // Sand
                                            BlockForThisColumn = Sand;
                                        }
                                        else
                                        {
                                            b[x, y, z] = (HavePloppedGrass) ? Soil : Grass; // Dirt or grass
                                            if (!HavePloppedGrass)
                                            {
                                                BlockForThisColumn = Soil;
                                                HavePloppedGrass = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        b[x, y, z] = BlockForThisColumn;
                                    }
                                    break;
                                default:
                                    z = 0;
                                    break;
                            }
                        }
                        // Place water
                        else if (b[x, y, z] == 0 && z <= WaterHeight && !HaveTouchedSoil)
                        {
                            b[x, y, z] = (cavemap[x,y,z]) ? Soil:Water;
                        }
                    }
                }
            }
            for (int x = 0; x < chunksize.X; x++)
            {
                for (int y = 0; y < chunksize.Y; y++)
                {
                    int z = 1;
                    // TODO Yell at Notch for not making Lava occlude. :|
                    if (b[x, y, z] == 0)
                        b[x, y, z] = 11; // Lava for air.
                    else if (b[x, y, z] == 9)
                        b[x, y, z] = 49; // Obsidian for underwater shit.
                }
            }
            if (rand.Next(0, 10) == 0)
            {
                int DungeonTries = 128;
                while (!Utils.MakeDungeon((int)X, (int)Y, ref b, ref mh, rand))
                {
                    //Console.WriteLine("Making dungeon...");
                    if (DungeonTries-- == 0)
                        break;
                }
            }
            /*
            int StillWater = 0;
            int RunningWater = 0;
            for (int z = 0; z < ZH; z++)
            {
                for (int x = 0; x < chunksize.X; x++)
                {
                    for (int y = 0; y < chunksize.Y; y++)
                    {
                        switch (b[x, y, z])
                        {
                            case 8:
                                RunningWater++;
                                break;
                            case 9:
                                StillWater++;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            Console.WriteLine("{0} still water, {1} running water", StillWater, RunningWater);*/
            AddTrees(ref b, (int)X, (int)Y, (int)chunksize.Z);
            return b;
        }
        public override void AddTrees(ref byte[, ,] b, int X, int Y, int H)
        {
            for (int t = 0; t < (int)(TreeNoise.GetValue(X, Y, 0) * 10.0); t++)
            {
                int _x = rand.Next(2, 13);
                int _y = rand.Next(2, 13);
                for (int z = (int)H - 2; z > 0; z--)
                {
                    switch (b[_x, _y, z])
                    {
                        case 0: // Air
                            continue;
                        case 2:
                            Utils.GrowTree(ref b, rand, _x, _y, z + 1);
                            break;
                        case 51:
                            Utils.GrowCactus(ref b, rand, _x, _y, z + 1);
                            break;
                        default: break;
                    }
                }
            }
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
            c.Add("GenerateCaves",GenerateCaves);
            c.Add("GenerateDungeons",GenerateDungeons);
            c.Add("GenerateOres",GenerateOres);
            c.Add("GenerateWater",GenerateWater);
            c.Add("HellMode",HellMode);
            c.Add("GenerateTrees",GenerateTrees);
            c.Add("Frequency",Frequency);
            c.Add("NoiseQuality",(byte)NoiseQuality);
            c.Add("OctaveCount",OctaveCount);
            c.Add("Lacunarity",Lacunarity);
            c.Add("Persistance",Persistance);
            c.Add("ContinentNoiseFrequency",ContinentNoiseFrequency);
            c.Add("CaveThreshold",CaveThreshold);
            nf.RootTag.Tags.Add(c);
            nf.SaveFile(f);
        }

        public override void Load(string Folder)
        {
            string f = Path.Combine(Folder, "DefaultMapGenerator.dat");
            if (!File.Exists(f)) return;

            NbtFile nf = new NbtFile(f);
            nf.LoadFile(f);
            GenerateCaves = nf.GetTag("/DefaultMapGenerator/GenerateCaves").asBool();
            GenerateDungeons=nf.GetTag("/DefaultMapGenerator/GenerateDungeons").asBool();
            GenerateOres=nf.GetTag("/DefaultMapGenerator/GenerateOres").asBool();
            GenerateWater=nf.GetTag("/DefaultMapGenerator/GenerateWater").asBool();
            HellMode=nf.GetTag("/DefaultMapGenerator/HellMode").asBool();
            GenerateTrees=nf.GetTag("/DefaultMapGenerator/GenerateTrees").asBool();
            Frequency=nf.GetTag("/DefaultMapGenerator/Frequency").asDouble();
            NoiseQuality=(NoiseQuality)nf.GetTag("/DefaultMapGenerator/NoiseQuality").asByte();
            OctaveCount=nf.GetTag("/DefaultMapGenerator/OctaveCount").asInt();
            Lacunarity=nf.GetTag("/DefaultMapGenerator/Lacunarity").asDouble();
            Persistance=nf.GetTag("/DefaultMapGenerator/Persistance").asDouble();
            ContinentNoiseFrequency=nf.GetTag("/DefaultMapGenerator/ContinentNoiseFrequency").asDouble();
            CaveThreshold=nf.GetTag("/DefaultMapGenerator/CaveThreshold").asDouble();
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
