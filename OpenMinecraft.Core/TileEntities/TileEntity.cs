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
using System.IO;
using LibNbt.Tags;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
namespace OpenMinecraft.TileEntities
{
    public class TileEntity
    {
        public Vector3i Pos = new Vector3i(0, 0, 0);
        public string ID = "NULL";

        private NbtCompound orig;
        private static Image icon;



        // Common entity things (INCLUDING LivingEntity stuff)
        public static List<string> CommonTileEntityVars = new List<string>(new string[] {
            "id",
            "x",
            "y",
            "z"
        });

        public static Dictionary<string, Type> TileEntityTypes = new Dictionary<string, Type>();

        public static void LoadEntityTypes()
        {
            TileEntityTypes.Clear();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(TileEntity)))
                {
                    TileEntity e = (TileEntity)t.GetConstructor(new Type[0]).Invoke(new Object[0]);
                    TileEntityTypes.Add(e.GetID(), t);
                    Console.WriteLine("[TileEntity] Added {0} handler.", e.GetID());
                }
            }
        }

        public virtual string GetID()
        {
            return "NULL";
        }

        public TileEntity()
        {
        }

        /// <summary>
        /// Load a TileEntity's basic values (call via base() in all inheriting files)
        /// </summary>
        /// <param name="CX">Chunk X Coordinate</param>
        /// <param name="CY">Chunk Y Coordinate</param>
        /// <param name="CS">Chunk horizontal scale</param>
        /// <param name="c">TileEntity's NbtCompound.</param>
        public TileEntity(int CX,int CY,int CS,NbtCompound c)
        {
            Pos = new Vector3i(
                c.Get<NbtInt>("x").Value,
                c.Get<NbtInt>("y").Value,
                c.Get<NbtInt>("z").Value);
            ID = (c["id"] as NbtString).Value;
            orig = c;
        }

        /// <summary>
        /// Load a TileEntity's basic values (call via base() in all inheriting files)
        /// </summary>
        /// <param name="c"></param>
        public TileEntity(NbtCompound c)
        {
            orig = c;
            Pos = new Vector3i(
                c.Get<NbtInt>("x").Value,
                c.Get<NbtInt>("y").Value,
                c.Get<NbtInt>("z").Value);
            ID = c.Get<NbtString>("id").Value;
        }

        /// <summary>
        /// Load a TileEntity from an NbtCompound.
        /// </summary>
        /// <param name="CX">Chunk X Coordinate.</param>
        /// <param name="CY">Chunk Y Coordinate.</param>
        /// <param name="CS">Chunk horizontal scale (16 in /game/)</param>
        /// <param name="c"></param>
        /// <returns>TileEntity.</returns>
        public static TileEntity GetEntity(int CX, int CY, int CS, NbtCompound c)
        {
            string entID = c.Get<NbtString>("id").Value;
            TileEntity e;
            if (TileEntityTypes.ContainsKey(entID))
                return (TileEntity)TileEntityTypes[entID].GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(NbtCompound) }).Invoke(new Object[] { CX,CY,CS,c });


            // Try to figure out what the hell this is.

            if (!Directory.Exists("TileEntities"))
                Directory.CreateDirectory("TileEntities");

           
            GenTemplate(c, "tileentity.template");
            return new TileEntity(c);
        }

        public void Base2NBT(ref NbtCompound c)
        {
            c.Tags.Add(new NbtString("id", ID));
            c.Tags.Add(new NbtInt("x", (int)Pos.X));
            c.Tags.Add(new NbtInt("y", (int)Pos.Z));
            c.Tags.Add(new NbtInt("z", (int)Pos.Y));
        }

        public virtual NbtCompound ToNBT()
        {
            return orig;
        }

        public Guid UUID { get; set; }

        public virtual Image Image 
        { 
        	get 
        	{
        		if(icon==null)
        			icon = Blocks.Get(0).Image;
        		return icon; 
        	} 
        }
        public override string ToString()
        {
            return ID + "?";
        }

        // This tosses together a cheapass entity class for faster updates.
        //  Primarily because I'm a lazy fuck.
        private static void GenTemplate(NbtCompound c, string tpl)
        {
            string ID = c.Get<NbtString>("id").Value;
            string Name = ID.Substring(0, 1).ToUpper() + ID.Substring(1);

            string newthing = File.ReadAllText("TileEntities/"+tpl);

            // Construct variables
            string vardec = "";
            string dectmpl = "\n\t\t[Category(\"{0}\"), Description(\"(WIP)\")]\n\t\tpublic {1} {2} {{get;set;}}\n";

            string varassn = "";
            string assntpl = "\n\t\t\t{0} = c.Get<{1}>(\"{2}\").Value;";

            string nbtassn = "";
            string nbttpl = "\n\t\t\tc.Tags.Add(new {0}(\"{1}\", {2}));";


            // Figure out if there are any new fields that we should be concerned about...
            foreach (NbtTag t in c.Tags)
            {
                if (CommonTileEntityVars.Contains(t.Name))
                    continue;
                string vname = t.Name.Substring(0, 1).ToUpper() + t.Name.Substring(1);
                string tagname = t.Name;
                string type = GetNativeType(t);
                string nbtTag = t.GetType().Name;

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
            newthing = newthing.Replace("{TILEENTITY_NAME}", Name);
            newthing = newthing.Replace("{TILEENTITY_ID}", ID);
            newthing = newthing.Replace("{VAR_DECL}", vardec);
            newthing = newthing.Replace("{VAR_ASSIGNMENT}", varassn);
            newthing = newthing.Replace("{TO_NBT}", nbtassn);

            File.WriteAllText("TileEntities/" + Name + ".cs", newthing);

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
    }
}
