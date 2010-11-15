using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace OpenMinecraft.Entities
{
    public class Entity
    {
		internal static Image PigIcon = new Bitmap("mobs/pig.png");
		
        public Vector3d Pos;
        public Vector3d Motion;

        [Category("Entity"),Description("Amount of air this creature has left"),DefaultValue(300)]
        public short Air { get; set; }
        [Category("Entity"), Description("OH GOD I'M ON FIRE"), DefaultValue(-20)]
        public short Fire { get; set; }
        [Category("Entity"), Description("OH GOD I'M FALLING")]
        public float FallDistance { get; set; }
        public NbtTag Rotation;
        public byte OnGround;
        private NbtCompound orig;
        private string id;
        public int ChunkX=0;
        public int ChunkY=0;
        public Vector3d OrigPos;
        public Guid UUID;

        // Common entity things (INCLUDING LivingEntity stuff)
        public static List<string> KnownEntityVars = new List<string>(new string[] {
            "id",
            "OnGround",
            "Air",
            "Fire",
            "FallDistance",
            "Motion",
            "Pos",
            "Rotation",

            // LIVINGENTITY
            "Health",
            "HurtTime",
            "AttackTime",
            "DeathTime"
        });
        public virtual NbtCompound ToNBT()
        {
            return orig;
        }

        public Entity()
        {
        }

        public Entity(NbtCompound c)
        {
            orig = c;
            id = (orig["id"] as NbtString).Value;
            SetBaseStuff(c);

#if DEBUG
            Console.WriteLine("*** BUG: Unknown entity (ID: {0})",id);
            Console.WriteLine(orig);
#endif
            File.WriteAllText("UnknownEntity." + id + ".txt", orig.ToString().Replace("\n","\r\n"));
        }


        internal void SetBaseStuff(NbtCompound c)
        {
            Air = (c["Air"] as NbtShort).Value;
            Fire = (c["Fire"] as NbtShort).Value;
            FallDistance = (c["FallDistance"] as NbtFloat).Value;
            Motion = new Vector3d(c["Motion"] as NbtList);
            Pos = new Vector3d(c["Pos"] as NbtList);
            OnGround = c.Get<NbtByte>("OnGround").Value;
            /* TempSMS is a dirty liar. */
            Pos = new Vector3d(Pos.X, Pos.Z, Pos.Y);
            Rotation = c["Rotation"];
            Console.WriteLine("Loaded entity {0} @ {1}", (c["id"] as NbtString).Value, Pos);
        }

        internal void Base2NBT(ref NbtCompound c,string _id)
        {
            c.Tags.Add(new NbtShort("Air", Air));
            c.Tags.Add(new NbtShort("Fire", Fire));
            c.Tags.Add(new NbtFloat("FallDistance", FallDistance));
            c.Tags.Add(new NbtByte("OnGround", OnGround));
            c.Tags.Add(new NbtString("id", _id));
            NbtList motion = new NbtList("Motion");
            motion.Tags.AddRange(new NbtDouble[]{
                new NbtDouble("", Motion.X),
                new NbtDouble("", Motion.Y),
                new NbtDouble("", Motion.Z)
            });
            c.Tags.Add(motion);
            NbtList pos = new NbtList("Pos");
            pos.Tags.AddRange(new NbtDouble[]{
                new NbtDouble("", Pos.X),
                new NbtDouble("", Pos.Z),
                new NbtDouble("", Pos.Y)
            });
            c.Tags.Add(pos);
            c.Tags.Add(Rotation);
        }
        public override string ToString()
        {
            return "[UNKNOWN ENTITY: " + id + "]";
        }

        public static Entity GetEntity(NbtCompound c)
        {
            switch ((c["id"] as NbtString).Value)
            {
                case "FallingSand": 
                    return new FallingSand(c);
                case "Pig":
                    return new Pig(c);
                case "Skeleton":
                    return new Skeleton(c);
                case "Sheep":
                    return new Sheep(c);
                case "Creeper":
                    return new Creeper(c);
                case "Item":
                    return new Item(c);
                case "Spider":
                    return new Spider(c);
                case "Zombie":
                    return new Zombie(c);
                case "Slime":
                    return new Slime(c);
                default: 
                    // Try to figure out what the hell this is.
                    return NewEntity(c);            }
        }

        private static Entity NewEntity(NbtCompound c)
        {
            // Do we have a LivingEntity or just an Entity?
            // Quick and simple test: health.
            if (c.Get("Health") != null)
            {
                LivingEntity e = new LivingEntity(c);
                GenTemplate(c, "livingentity.template");
                // Goodie, just whip up a new LivingEntity and we're relatively home free.
                return e;
            }
            else
            {
                GenTemplate(c, "entity.template");
                return new Entity(c);
            }
        }

        // This tosses together a cheapass entity class for faster updates.
        //  Primarily because I'm a lazy fuck.
        private static void GenTemplate(NbtCompound c, string tpl)
        {
            string ID = c.Get<NbtString>("id").Value;
            string Name = ID.Substring(0, 1).ToUpper() + ID.Substring(1);

            string newthing = File.ReadAllText(tpl);

            // Construct variables
            string vardec = "";
            string dectmpl = "\n\t\t[Category(\"{0}\"), Description(\"(WIP)\")]\n\t\tpublic {1} {2} {{get;set;}}\n";

            string varassn = "";
            string assntpl="\n\t\t\t{0} = c.Get<{1}>(\"{2}\").Value;";

            string nbtassn = "";
            string nbttpl = "\n\t\t\tc.Tags.Add(new {0}(\"{1}\", {2}));";


            // Figure out if there are any new fields that we should be concerned about...
            foreach (NbtTag t in c.Tags)
            {
                if (KnownEntityVars.Contains(t.Name))
                    continue;
                string vname=t.Name.Substring(0, 1).ToUpper() + t.Name.Substring(1);
                string tagname=t.Name;
                string type=GetNativeType(t);
                string nbtTag=t.GetType().Name;

                vardec += string.Format(dectmpl, Name, type, vname);
                varassn += string.Format(assntpl, vname, nbtTag, tagname);
                nbtassn += string.Format(nbttpl, nbtTag, tagname, vname);
            }

            // {DATA_DUMP} - Crap out a dump of the entity.
            // {ENTITY_NAME} - Entity name, but capitalized (CamelCase)
            // {NEW_VARS} - New var declarations.
            // {VAR_ASSIGNMENT} - Set the vars from the Compound.
            // {TO_NBT} - Set the appropriate stuff in the Compound.
            // {ENTITY_ID} - Raw entity ID

            newthing = newthing.Replace("{DATA_DUMP}", c.ToString());
            newthing = newthing.Replace("{ENTITY_NAME}", Name);
            newthing = newthing.Replace("{ENTITY_ID}", ID);
            newthing = newthing.Replace("{NEW_VARS}", vardec);
            newthing = newthing.Replace("{VAR_ASSIGNMENT}", varassn);
            newthing = newthing.Replace("{TO_NBT}", nbtassn);

            File.WriteAllText(Name + ".ENTITY.cs", newthing);
        }

        private static string GetNativeType(NbtTag t)
        {
            switch (t.GetType().Name)
            {
            case "NbtByte": 
                return "byte";
            case "NbtByteArray": 
                return "byte[]";
            case "NbtCompound": 
                return "NbtCompound"; // Note to myself to fix shit
            case "NbtDouble": 
                return "double";
            case "NbtFloat": 
                return "float";
            case "NbtInt": 
                return "int";
            case "NbtList": 
                return "NbtList"; // Note to myself again
            case "NbtLong": 
                return "long";
            case "NbtShort": 
                return "short";
            case "NbtString": 
                return "string";
            default:
                return t.GetType().Name;
            }
        }

        public virtual string GetID() { return "_NULL_"; }

        [Browsable(false)]
        public virtual Image Image {
            get
            {
                return PigIcon;
            }
        }

        internal static string GetRandomMonsterID(Random r)
        {
            string[] mobids = new string[]{
                "Pig", // Just to throw people off :V
                "Sheep",
                "Skeleton",
                "Creeper",
                "Spider",
                "Zombie",
                "Slime"
            };
            int i = r.Next(0, mobids.Length-1);
            return mobids[i];
        }
    }
}
