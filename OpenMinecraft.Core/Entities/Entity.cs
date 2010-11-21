/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using LibNbt.Tags;

namespace OpenMinecraft.Entities
{
    public class Entity
    {
		
        public Vector3d Pos = new Vector3d(0,0,0);
        public Vector3d Motion = new Vector3d(0,0,0);

        [Category("Entity"),Description("Amount of air this creature has left"),DefaultValue(200)]
        public short Air { get; set; }
        
        [Category("Entity"), Description("OH GOD I'M ON FIRE"), DefaultValue(-20)]
        public short Fire { get; set; }
        
        [Category("Entity"), Description("OH GOD I'M FALLING")]
        public float FallDistance { get; set; }

        [Category("Entity"), Description("Entity rotation in degrees. (0,0) = Facing west.")]
        public Rotation Rotation {
            get { return mRotation; }
            set { mRotation = value; }
        }

        public Rotation mRotation=new Rotation(0,0);
        public byte OnGround=0x00;
        private NbtCompound orig;
        private string id;
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

        public static Dictionary<string, Type> EntityTypes = new Dictionary<string, Type>();

        public static void LoadEntityTypes()
        {
            EntityTypes.Clear();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(Entity)))
                {
                    Console.WriteLine("[Entity] Adding {0}...", t.Name);
                    Entity e = (Entity)t.GetConstructor(new Type[0]).Invoke(new Object[0]);
                    EntityTypes.Add(e.GetID(), t);
                    Console.WriteLine("[Entity] Added {0} handler.", e.GetID());
                }
            }
        }

        public static Entity InitHandlerFor(string entID,NbtCompound c)
        {
            if(EntityTypes.ContainsKey(entID))
                return (Entity)EntityTypes[entID].GetConstructor(new Type[]{typeof(NbtCompound)}).Invoke(new Object[]{c});
            return null;
        }

        public static Entity Get(string entID)
        {
            if (EntityTypes.ContainsKey(entID))
                return (Entity)EntityTypes[entID].GetConstructor(new Type[0]).Invoke(new Object[0]);
            return null;
        }

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
        }


        internal void SetBaseStuff(NbtCompound c)
        {
            Air = (c["Air"] as NbtShort).Value;
            Fire = (c["Fire"] as NbtShort).Value;
            FallDistance = (c["FallDistance"] as NbtFloat).Value;
            Motion = new Vector3d(c["Motion"] as NbtList);
            Pos = new Vector3d(c["Pos"] as NbtList);
            OnGround = c.Get<NbtByte>("OnGround").Value;
            Rotation = Rotation.FromNbt(c.Get<NbtList>("Rotation"));
            Console.WriteLine("Loaded entity {0} @ {1}", (c["id"] as NbtString).Value, Pos);
        }

        internal void Base2NBT(ref NbtCompound c,string _id)
        {
            c.Add(new NbtShort("Air", Air));
            c.Add(new NbtShort("Fire", Fire));
            c.Add(new NbtFloat("FallDistance", FallDistance));
            c.Add(new NbtByte("OnGround", OnGround));
            c.Add(new NbtString("id", _id));
            NbtList motion = new NbtList("Motion");
            motion.Tags.AddRange(new NbtDouble[]{
                new NbtDouble("", Motion.X),
                new NbtDouble("", Motion.Y),
                new NbtDouble("", Motion.Z)
            });
            c.Add(motion);
            NbtList pos = new NbtList("Pos");
            pos.Tags.AddRange(new NbtDouble[]{
                new NbtDouble("", Math.IEEERemainder(Pos.X,16)),
                new NbtDouble("", Pos.Y),
                new NbtDouble("", Math.IEEERemainder(Pos.Z,16))
            });
            c.Add(pos);
            c.Add(Rotation.ToNBT());
        }
        public override string ToString()
        {
            return "[UNKNOWN ENTITY: " + id + "]";
        }

        public static Entity GetEntity(NbtCompound c)
        {
            if (!c.Has("id"))
            {
                Console.WriteLine(c.ToString());
            }
            string entID = c.Get<NbtString>("id").Value;
            if (EntityTypes.ContainsKey(entID))
            {
                try
                {
                    return (Entity)EntityTypes[entID].GetConstructor(new Type[] { typeof(NbtCompound) }).Invoke(new Object[] { c });
                }
                catch (TargetInvocationException e)
                {
                    Console.WriteLine("Failed to load " + entID + ": \n" + e.InnerException.ToString());
                    throw e.InnerException;
                }
            }
            
            // Try to figure out what the hell this is.

            if (!Directory.Exists("Entities"))
                Directory.CreateDirectory("Entities");

            // Do we have a LivingEntity or just an Entity?
            // Quick and simple test: health.
            if (c.Has("Health") && entID!="Item")
            {
                GenTemplate(c, "livingentity.template");
                // Goodie, just whip up a new LivingEntity and we're relatively home free.
                return new LivingEntity(c);
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

            string newthing = File.ReadAllText("Entities/"+tpl);

            // Construct variables
            string vardec = "";
            string dectmpl = "\n\t\t[Category(\"{0}\"), Description(\"(WIP)\")]\n\t\tpublic {1} {2} {{get;set;}}\n";

            string varassn = "";
            string assntpl="\n\t\t\t{0} = c.Get<{1}>(\"{2}\").Value;";

            string nbtassn = "";
            string nbttpl = "\n\t\t\tc.Add(new {0}(\"{1}\", {2}));";


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

            File.WriteAllText("Entities/"+Name + ".cs", newthing);

            // TODO: Compile?
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
                return new Bitmap("mobs/notch.png");
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

        public static void Cleanup()
        {
            KnownEntityVars.Clear();
        }
    }
}
