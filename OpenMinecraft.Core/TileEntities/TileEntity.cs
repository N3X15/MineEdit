using System;
using System.IO;
using LibNbt.Tags;
using System.Drawing;
namespace OpenMinecraft.TileEntities
{
    public class TileEntity
    {
        public Vector3i Pos = new Vector3i(0, 0, 0);
        public string id = "NULL";

        private NbtCompound orig;
        private static Image icon;
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
                c["x"].asInt(),
                c["z"].asInt(),
                c["y"].asInt());
            id = (c["id"] as NbtString).Value;
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
                c["x"].asInt(),
                c["z"].asInt(),
                c["y"].asInt());
            id = (c["id"] as NbtString).Value;
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
            TileEntity e;
            switch ((c["id"] as NbtString).Value)
            {
                case "Chest":
                    e = new Chest(CX,CY,CS,c);
                    break;
                case "MobSpawner":
                    e = new MobSpawner(CX, CY, CS, c);
                    break;
                case "Furnace":
                    e = new Furnace(CX, CY, CS, c);
                    break;
                case "Sign":
                    e = new Sign(CX, CY, CS, c);
                    break;
                case "NULL":
                    // Ignore it :|
                    return new TileEntity(CX, CY, CS, c);
                default:
#if DEBUG
                    Console.WriteLine("*** Unknown TileEntity: {0}", (c["id"] as NbtString).Value);
                    Console.WriteLine(c);
#endif
                    File.WriteAllText(string.Format("UnknownTileEntity.{0}.txt", (c["id"] as NbtString).Value),c.ToString().Replace("\n","\r\n"));
                    return new TileEntity(CX, CY, CS, c);
            }
#if DEBUG
            Console.WriteLine("Loaded {1} @ {0}", e,e.Pos);
#endif
            return e;
        }

        public void Base2NBT(ref NbtCompound c)
        {
            c.Tags.Add(new NbtString("id", id));
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
            return id + "?";
        }
    }
}
