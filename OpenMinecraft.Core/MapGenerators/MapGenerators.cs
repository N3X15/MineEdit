using System;
using System.Collections;

using System.Text;
using System.Reflection;
using LibNoise;
using Math=System.Math;
using System.Collections.Generic;
namespace OpenMinecraft
{
    public class MapGenerators
    {

        protected static Dictionary<string, Type> Generators = new Dictionary<string, Type>();
        protected static List<Type> Trees = new List<Type>();

        public static void Init()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if(t.IsSubclassOf(typeof(IMapGenerator)) && !Generators.ContainsKey(t.Name))
                {
                    Generators.Add(t.Name, t);
                    Console.WriteLine(t.Name);
                }
            }
        }

        public static IMapGenerator Get(string gen, long Seed, MapGenMaterials Materials)
        {
            if (gen == null)
                gen = "DefaultMapGenerator";
            if (Generators.Count == 0)
            {
                Generators.Add("DefaultMapGenerator", typeof(HillGenerator));
            }
            string id = "DefaultMapGenerator";
            if (Generators.ContainsKey(gen))
                id = gen;
            Console.WriteLine("Initializing {0}.", id);
            IMapGenerator g = (IMapGenerator)Generators[id].GetConstructor(new Type[] { typeof(long) }).Invoke(new object[] { Seed });
            g.Materials = Materials;
            g.GenerateCaves = true;
            g.GenerateDungeons = true;
            g.GenerateOres = true;
            g.GenerateWater = true;
            g.HellMode = false;
            g.GenerateTrees = true;
            g.NoPreservation = false;
            return g;
        }

        public static Dictionary<string,string> GetList()
        {
            Dictionary<string, string> Gens = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Type> k in Generators)
            {
                if (!Gens.ContainsKey(k.Key))
                {
                    object instance = k.Value.GetConstructor(new Type[] {}).Invoke(new object[] {});
                    string n = (string)k.Value.GetProperty("Name").GetValue(instance, null) ;
                    string a = (string)k.Value.GetProperty("Author").GetValue(instance, null);
                    string v = (string)k.Value.GetProperty("Version").GetValue(instance, null);
                    Gens.Add(k.Key, string.Format("{0} v.{1} by {2}", n, v, a));
                }
            }
            return Gens;
        }

    }
}
