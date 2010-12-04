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

        public abstract void Save(string Folder);
        public abstract void Load(string Folder);

        FastNoise HumidityNoise = new FastNoise();
        FastNoise TemperatureNoise;
        
        [Description("Temperature offset.  Valid temperatures are between -1 (cold) and 1 (hot).")]
        public double TemperatureOffset { get; set; }

        [Description("Humidity offset.  Valid humidities are between -1 (dry) and 1 (wet).")]
        public double HumidityOffset { get; set; }

        public void SetupBiomeNoise(int RandomSeed)
        {
            HumidityNoise.Seed = RandomSeed + 6;
            HumidityNoise.Frequency = 1;
            HumidityNoise.Persistence = 0.5;
            HumidityNoise.OctaveCount = 1;
            TemperatureNoise = HumidityNoise;
            TemperatureNoise.Seed = RandomSeed + 7;
        }
        public virtual BiomeType[,] DetermineBiomes(double[,] hm, long X, long Z)
        {
            BiomeType[,] bt = new BiomeType[hm.GetLength(0), hm.GetLength(1)];
            int xo = (int)(X*hm.GetLength(0));
            int zo = (int)(Z*hm.GetLength(1));
            for (int x = 0; x < hm.GetLength(0); x++)
            {
                for (int z = 0; z < hm.GetLength(1); z++)
                {
                    double h = HumidityNoise.GetValue((double)(x + xo) / 12d, (double)(z + zo) / 12d, 0);
                    double t = TemperatureNoise.GetValue((double)(x + xo) / 12d, (double)(z + zo) / 12d, 0);
                    bt[x, z] = Biome.GetBiomeType(h, t);
                }
            }
            return bt;
        }
        public virtual void AddTrees(ref IMapHandler mh, BiomeType[,] biomes, ref Random rand, int X, int Z, int H)
        {
            List<Vector2i> PlantedTrees = new List<Vector2i>();
            int DistanceReqd = 3;
            for (int t = 0; t < (int)((HumidityNoise.GetValue(X, Z, 0)+1d) * 5.0); t++)
            {
                Vector2i me = new Vector2i(rand.Next(2, 13),rand.Next(2, 13));
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
                int xo = (int)(X * mh.ChunkScale.X);
                int zo = (int)(Z * mh.ChunkScale.Z);
                for (int y = (int)H - 10; y > 0; y--)
                {
                    switch (mh.GetBlockAt(me.X+xo, y, me.Y+zo))
                    {
                        case 0: // Air
                            continue;
                        case 1: // ROCK
                        case 2: // GRASS
                        case 3: // DIRT
                            Tree tree = new NormalTree(me.X + xo, y + 1, me.Y + zo, rand.Next(5, 8));
                            tree.MakeTrunk(ref mh);
                            tree.MakeFoliage(ref mh);
                            break;
                        /* Automatic ?
                        case 11: // SAND
                            Utils.GrowCactus(ref b, rand, me.X, y + 1, me.Y);
                            break;
                        */
                        default: break;
                    }
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
        public virtual void AddSoil(ref byte[,,] b, BiomeType[,] biomes, int WaterHeight, int depth, MapGenMaterials mats)
        {
            int YH = b.GetLength(1)-2;
            for (int x = 0; x < b.GetLength(0); x++)
            {
                //Console.WriteLine();
                for (int z = 0; z < b.GetLength(2); z++)
                {
                    bool HavePloppedGrass = false;
                    bool HaveTouchedSoil = false;
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

                                    if ((y - depth <= WaterHeight && GenerateWater) || biomes[x,z] == BiomeType.Desert)
                                        b[x, y, z] = mats.Sand;
                                    else
                                        b[x, y, z] = (HavePloppedGrass) ? mats.Soil : mats.Grass;
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
                            if (Biome.NeedsSnowAndIce(biomes[x, z]) && y == WaterHeight)
                                b[x,y,z]=mats.Ice;
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
