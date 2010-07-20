using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OpenMinecraft.MapGenerators
{
    public class MapGenerator
    {
        protected static byte Air = 0x00;
        protected static byte Rock = 0x01;

        protected static byte Lava = 0x11;

        protected static Hashtable Generators = new Hashtable();

        public MapGenerator()
        {
            // Do RNG init here
        }

        public MapGenerator(long Seed)
        {
            // Ditto
        }

        public virtual byte[,,] Generate(long X, long Y, Vector3i chunksize)
        {
            byte[, ,] b = new byte[chunksize.X, chunksize.Y, chunksize.Z];
            for(int x=0;x<chunksize.X;x++)
            {
                for (int y = 0; y < chunksize.X; y++)
                {
                    for (int z = 0; z < chunksize.X; z++)
                    {
                        if (z == 0)
                            b[x, y, z] = Rock;
                        else
                            b[x, y, z] = Air;
                    }
                }
            }
            return b;
        }
        public override string ToString()
        {
            return "Test";
        }

        public static MapGenerator Get(string gen,long Seed)
        {
            if (Generators.Count == 0)
            {
                Generators.Add("Test", typeof(MapGenerator));
            }
            string id = "Test";
            if (Generators.Contains(gen))
                id = gen;
            return (MapGenerator)(Generators[id] as Type).GetConstructor(new Type[] { typeof(long) }).Invoke(new object[]{Seed});
        }
    }
}
