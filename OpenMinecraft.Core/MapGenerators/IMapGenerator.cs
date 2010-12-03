using System;
using System.Collections.Generic;

using System.Text;
using System.ComponentModel;
using OpenMinecraft;
using LibNoise;

namespace OpenMinecraft
{
    public abstract class IMapGenerator
    {
        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract string Version { get; }

        public abstract MapGenMaterials Materials { get; set; }
        
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public abstract byte[, ,] Generate(ref IMapHandler mh, long X, long Y, out int min, out int max);

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

        public virtual void AddTrees(ref byte[, ,] b, ref Perlin TreeNoise, ref Random rand, int X, int Z, int H)
        {
            List<Vector2i> PlantedTrees = new List<Vector2i>();
            int DistanceReqd = 3;
            for (int t = 0; t < (int)(TreeNoise.GetValue(X, 0, Z) * 10.0); t++)
            {
                Vector2i me = new Vector2i(rand.Next(2, 13),rand.Next(2, 13));

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
                for (int y = (int)H - 10; y > 0; y--)
                {
                    switch (b[me.X, y, me.Y])
                    {
                        case 0: // Air
                            continue;
                        case 1: // ROCK
                        case 2: // GRASS
                        case 3: // DIRT
                            Utils.GrowTree(ref b, rand, me.X, y + 1, me.Y);
                            break;
                        /* Automatic
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
        public virtual void AddPlayerBarriers(ref byte[, ,] b)
        {
            //Console.WriteLine("IMapGenerator.AddPlayerBarriers: x={0};y={1};z={2}", b.GetLength(0), b.GetLength(1), b.GetLength(2));
            for (int x = 0; x < b.GetLength(0); ++x)
            {
                for (int y = 0; y < b.GetLength(1); ++y)
                {
                    for (int z = 0; z < b.GetLength(2); ++z)
                    {
                        if(y==0)
                                b[x,y,z]=7; // Adminium
                        else if(y==1)
                        {
                                // TODO Yell at Notch for not making Lava occlude. :|
                                if (b[x, y, z] == 0)
                                    b[x, y, z] = 11; // Lava for air.
                                else if (b[x, y, z] == 9)
                                    b[x, y, z] = 49; // Obsidian for underwater shit.
                                break;
                        }
                        else if (y >= b.GetLength(1) - 3)
                        {
                            b[x, y, z] = 0;
                        }
                    }
                }
            }
        }
        public virtual void AddSoil(ref byte[,,] b, int WaterHeight, int depth, MapGenMaterials mats)
        {
            if (mats.Rock == 0)
            {
                Console.WriteLine(mats);
            }

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

                                    if (y - depth <= WaterHeight && GenerateWater)
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
                        }
                    }
                }
            }
        }
    }
}
