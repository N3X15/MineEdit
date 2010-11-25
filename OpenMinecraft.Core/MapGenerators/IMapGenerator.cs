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
        public abstract byte[, ,] Generate(ref IMapHandler mh, long X, long Y);

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

        public virtual void AddTrees(ref byte[, ,] b, ref Perlin TreeNoise, ref Random rand, int X, int Y, int H)
        {
            List<Vector2i> PlantedTrees = new List<Vector2i>();
            int DistanceReqd = 3;
            for (int t = 0; t < (int)(TreeNoise.GetValue(X, Y, 0) * 10.0); t++)
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
                for (int z = (int)H - 2; z > 0; z--)
                {
                    switch (b[me.X, me.Y, z])
                    {
                        case 0: // Air
                            continue;
                        case 2:
                            Utils.GrowTree(ref b, rand, me.X, me.Y, z + 1);
                            break;
                        case 51:
                            Utils.GrowCactus(ref b, rand, me.X, me.Y, z + 1);
                            break;
                        default: break;
                    }
                }
            }
        }
        public virtual void AddDungeon(ref byte[, ,] b, ref IMapHandler mh, Random rand, long X, long Y)
        {
            if (!this.GenerateDungeons) return;
            if (rand.Next(0, 100) == 0)
            {
                int DungeonTries = 128;
                while (!Utils.MakeDungeon((int)X, (int)Y, ref b, ref mh, rand))
                {
                    //Console.WriteLine("Making dungeon...");
                    if (DungeonTries-- == 0)
                        break;
                }
            }
        }
        public virtual void AddPlayerBarriers(ref byte[, ,] b)
        {
            for (int x = 0; x < b.GetLength(0); ++x)
            {
                for (int y = 0; y < b.GetLength(1); ++y)
                {
                    for (int z = 0; z < b.GetLength(2); ++z)
                    {
                        if(z==0)
                                b[x,y,z]=7; // Adminium
                        else if(z==1)
                        {
                                // TODO Yell at Notch for not making Lava occlude. :|
                                if (b[x, y, z] == 0)
                                    b[x, y, z] = 11; // Lava for air.
                                else if (b[x, y, z] == 9)
                                    b[x, y, z] = 49; // Obsidian for underwater shit.
                                break;
                        }
                        else if (z >= b.GetLength(2) - 3)
                        {
                            b[x, y, z] = 0;
                        }
                    }
                }
            }
        }
        public virtual void AddSoil(ref byte[,,] b, int WaterHeight, int depth, MapGenMaterials mats)
        {
            int ZH = b.GetLength(2)-2;
            for (int x = 0; x < b.GetLength(0); x++)
            {
                //Console.WriteLine();
                for (int y = 0; y < b.GetLength(1); y++)
                {
                    bool HavePloppedGrass = false;
                    bool HaveTouchedSoil = false;
                    for (int z = (int)b.GetLength(2) - 1; z > 0; z--)
                    {
                        byte supportBlock = b[x, y, z - 1];
                        // Ensure there's going to be stuff holding us up.
                        if (b[x, y, z] == mats.Rock && supportBlock==mats.Rock)
                        {
                            HaveTouchedSoil = true;
                            if (z + depth >= ZH)
                                continue;
                            byte ddt = b[x, y, z + depth];
                            switch (ddt)
                            {
                                case 0: // Air
                                case 8: // Water
                                case 9: // Water

                                    if (z - depth <= WaterHeight && GenerateWater)
                                        b[x, y, z] = mats.Sand;
                                    else
                                        b[x, y, z] = (HavePloppedGrass) ? mats.Soil : mats.Grass;
                                    if (!HavePloppedGrass)
                                        HavePloppedGrass = true;
                                    break;
                                default:
                                    z = 0;
                                    break;
                            }
                        }
                        else if (b[x, y, z] == 0 && z <= WaterHeight && !HaveTouchedSoil && GenerateWater)
                        {
                            b[x, y, z] = mats.Water;
                        }
                    }
                }
            }
        }
    }
}
