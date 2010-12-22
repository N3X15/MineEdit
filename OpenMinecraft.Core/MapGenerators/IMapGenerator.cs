using System;
using System.Collections.Generic;

using System.ComponentModel;
using LibNoise;

namespace OpenMinecraft
{
    public abstract class IMapGenerator
    {
        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }

        public abstract MapGenMaterials Materials { get; set; }

        [Category("Caves"),
        Description("The perlin noise generator spits out a value from 0 to 1.  Blocks are removed for caves above this threshold.")]
        public double CaveThreshold 
        { 
            get {return mCaveThreshold;} 
            set {mCaveThreshold=value; }
        }

        [Category("Caverns"), Description("At ground level, the value threshold for caverns is this value.")]
        public double CavernThresholdMin 
        { 
            get {return mCavernThresholdMin;} 
            set {mCavernThresholdMin=Utils.Clamp(value,0,1); }
        }

        [Category("Caverns"), Description("At bedrock level, the value threshold for caverns is this value.")]
        public double CavernThresholdMax 
        { 
            get {return mCavernThresholdMax;} 
            set {mCavernThresholdMax=Utils.Clamp(value,0,1); }
        }
        
        public abstract double[,] Generate(IMapHandler map, long X, long Z, out double min, out double max);

        [Browsable(false)]
        public abstract bool GenerateCaves { get; set; }
        [Browsable(false)]
        public abstract bool GenerateDungeons { get; set; }
        [Browsable(false)]
        public abstract bool GenerateOres { get; set; }
        [Browsable(false)]
        public abstract bool GenerateWater { get; set; }
        [Browsable(false)]
        public abstract bool HellMode { get; set; }
        [Browsable(false)]
        public abstract bool NoPreservation { get; set; }
        [Browsable(false)]
        public abstract bool GenerateTrees { get; set; }

        [Category("Biomes"), Description("Temperature offset.  Valid temperatures are between -1 (cold) and 1 (hot).")]
        public double TemperatureOffset { get; set; }

        [Category("Biomes"), Description("Humidity offset.  Valid humidities are between -1 (dry) and 1 (wet).")]
        public double HumidityOffset { get; set; }

        Simplex HumidityNoise;
        Simplex TemperatureNoise;
        private static double BIOME_SCALE=200;

        double mCaveThreshold = 0.85d;
        double mCavernThresholdMin = 0.25d;
        double mCavernThresholdMax = 0.95d;

        public abstract void Save(string Folder);
        public abstract void Load(string Folder);

        public void SetupBiomeNoise(int RandomSeed)
        {
            TemperatureNoise = new Simplex(RandomSeed+1);
            HumidityNoise = new Simplex(RandomSeed);
        }
        public virtual BiomeType[,] DetermineBiomes(Vector3i chunkScale, long X, long Z)
        {
            BiomeType[,] bt = new BiomeType[chunkScale.X, chunkScale.Z];
            int xo = (int)(X*chunkScale.X);
            int zo = (int)(Z*chunkScale.Z);
            double maxT = 0;
            for (int x = 0; x < chunkScale.X; x++)
            {
                for (int z = 0; z < chunkScale.Z; z++)
                {
                    double h = HumidityNoise.Noise((double)(x + xo) / BIOME_SCALE, 0, (double)(z + zo) / BIOME_SCALE) + HumidityOffset;
                    double t = TemperatureNoise.Noise((double)(x + xo) / BIOME_SCALE, 0, (double)(z + zo) / BIOME_SCALE) + TemperatureOffset; 
                    if (t > maxT) maxT = t;
                    bt[x, z] = Biome.GetBiomeType(h, t);
                }
            }
            //Console.WriteLine("t=" + maxT.ToString());
            return bt;
        }
        public virtual void AddTrees(ref IMapHandler mh, BiomeType[,] biomes, ref Random rand, int X, int Z, int H)
        {
            int xo = (int)(X * mh.ChunkScale.X);
            int zo = (int)(Z * mh.ChunkScale.Z);
            List<Vector2i> PlantedTrees = new List<Vector2i>();
            int DistanceReqd = 3;
            for (int t = 0; t < (int)((HumidityNoise.Noise((double)(xo) / BIOME_SCALE, (double)(zo) / BIOME_SCALE, 0) + HumidityOffset) * 5.0); t++)
            {
                Vector2i me = new Vector2i(rand.Next(0, 15),rand.Next(0, 15));
                if (!Biome.NeedsTrees(biomes[me.X, me.Y]))
                    continue;
                bool tooclose=false;
                foreach (Vector2i tree in PlantedTrees)
                {
                    if (Vector2i.Distance(tree, me) < DistanceReqd)
                    {
                        tooclose = true;
                        break;
                    }
                }

                if (tooclose) continue;
                bool founddert = false;
                for (int y = (int)H - 10; y > 0; y--)
                {
                    switch (mh.GetBlockAt(me.X+xo, y, me.Y+zo))
                    {
                        case 0: // Air
                        case 78: // Snow cover
                            continue;
                        // case 1: // ROCK
                        case 2: // GRASS
                        case 3: // DIRT
                            //Utils.GrowTree(ref blocks, rand, (int)me.X, (int)y + 1, (int)me.Y);
                            mh.SetBlockAt(me.X + xo, y + 1, me.Y + zo, 6); // Sapling
                            mh.SetDataAt(me.X + xo, y + 1, me.Y + zo, 15); // Growth stage 15.
                            /*
                            Tree tree = new NormalTree(me.X + xo, y + 1, me.Y + zo, rand.Next(5, 8));
                            tree.MakeTrunk(ref mh);
                            tree.MakeFoliage(ref mh);
                            */
                            mh.SaveAll();
                            founddert = true;
                            break;
                        case 11: // SAND
                            //Utils.GrowCactus(ref b, rand, me.X, y + 1, me.Y);
                            break;
                        default:
                            founddert = true; 
                            break;
                    }
                    if (founddert) break;
                }
                PlantedTrees.Add(me);
            }
        }
        public virtual void AddDungeons(ref byte[, ,] b, ref IMapHandler mh, Random rand, long X, long Z)
        {
            if (!this.GenerateDungeons) return;
            if (rand.Next(0, 100) == 0)
            {
                int DungeonTries = 128;
                while (!Utils.MakeDungeon((int)X, (int)Z, ref b, ref mh, rand))
                {
                    //Console.WriteLine("Making dungeon...");
                    if (DungeonTries-- == 0)
                        break;
                }
            }
        }
        public virtual void AddSoil(long X, long Z, RidgedMultifractal CavernNoise, Perlin CaveNoise, double[,] hm,ref byte[,,] b, BiomeType[,] biomes, int WaterHeight, int depth, MapGenMaterials mats)
        {
            int YH = b.GetLength(1) - 2;
            double xo = (double)(X * b.GetLength(0));
            double zo = (double)(Z * b.GetLength(2));
            for (int x = 0; x < b.GetLength(0); x++)
            {
                //Console.WriteLine();
                for (int z = 0; z < b.GetLength(2); z++)
                {
                    double hmY=(double)(System.Math.Min(hm[x, z],1d) * (b.GetLength(1) - 3));
                    bool HavePloppedGrass = false;
                    bool HaveTouchedSoil = false;
                    // Caves first
                    if (GenerateCaves)
                    {
                        for (int y = 0; y < b.GetLength(1); y++)
                        {
                            // If we're in rock, and CavernNoise value is under a threshold value calculated by height 
                            //  or CaveNoise value is over threshold value, and the block we're removing won't fall on us...
                            if (
                                b[x, y, z] == 1
                                && (
                                    ((CavernNoise.GetValue(x + xo, y, z + zo) / 2) + 1) < Utils.Lerp(CavernThresholdMax, CavernThresholdMin, (((double)y / (hmY + 1))))
                                    || (Utils.FixLibnoiseOutput(CaveNoise.GetValue(x + xo, y, z + zo)) > CaveThreshold)
                                )
                                && !(b[x, y, z] == 9 || b[x, y, z] == 8 || b[x, y, z] == 12 || b[x, y, z] == 13))
                            {
                                // Remove it
                                b[x, y, z] = 0;
                            }
                        }
                    }
                    for (int y = (int)b.GetLength(1) - 1; y > 0; y--)
                    {
                        byte supportBlock = b[x, y-1, z];
                        // Ensure there's going to be stuff holding us up.
                        if (b[x, y, z] == mats.Rock && supportBlock==mats.Rock)
                        {
                            HaveTouchedSoil = true;
                            if (y + depth >= YH)
                                continue;
                            byte ddt = b[x, y+depth, z];
                            switch (ddt)
                            {
                                case 0: // Air
                                case 8: // Water
                                case 9: // Water
                                    BiomeType bt = biomes[x,z];
                                    if (bt == BiomeType.Tundra)
                                    {
                                        b[x, y, z] = mats.Sand;
                                    }
                                    else
                                    {
                                        if (y - depth <= WaterHeight && GenerateWater)
                                        {
                                            if ((bt == BiomeType.Taiga || bt == BiomeType.TemperateForest || bt == BiomeType.Tundra) && y > WaterHeight)
                                            {
                                                b[x, y, z] = (HavePloppedGrass) ? mats.Soil : mats.Grass;
                                            }
                                            else
                                            {
                                                b[x, y, z] = mats.Sand;
                                            }
                                        }
                                        else
                                            b[x, y, z] = (HavePloppedGrass) ? mats.Soil : mats.Grass;
                                    }
                                    if (!HavePloppedGrass)
                                        HavePloppedGrass = true;
                                    break;
                                default:
                                    y = 0;
                                    break;
                            }
                        }
                        else if (b[x, y, z] == 0 && y <= WaterHeight && !HaveTouchedSoil && GenerateWater)
                        {
                            b[x, y, z] = mats.Water;
                        }
                    }
                }
            }
        }

        internal virtual void Precipitate(ref byte[, ,] b, BiomeType[,] bt, MapGenMaterials mats, long X, long Z)
        {
            int xs = b.GetLength(0);
            int zs = b.GetLength(2);

            for (int x = 0; x < xs; x++)
            {
                for (int z = 0; z < zs; z++)
                {
                    if(Biome.NeedsSnowAndIce(bt[x,z]))
                        continue;
                    // Fall down
                    for (int y = b.GetLength(1) - 1; y > 0; y--)
                    {
                        byte block = b[x, y, z];
                        if (block == 0) continue;
                        if (block == mats.Water)
                            b[x, y, z] = mats.Ice;
                        else
                            b[x, y + 1, z] = mats.Snow;
                        break;
                    }
                }
            }
        }
    }
}
