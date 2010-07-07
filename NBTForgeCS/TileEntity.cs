using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt.Tags;

namespace MineEdit
{
    public class TileEntity
    {
        public Vector3i Pos = new Vector3i(0, 0, 0);
        public string id = "NULL";

        private NbtCompound orig;
        public TileEntity()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CX">Chunk X</param>
        /// <param name="CY">Chunk Y</param>
        /// <param name="CS">Chunk horizontal scale</param>
        /// <param name="c"></param>
        public TileEntity(int CX,int CY,int CS,NbtCompound c)
        {
            Pos = new Vector3i(
                (c["x"] as NbtInt).Value + (CX * CS),
                (c["y"] as NbtInt).Value + (CY * CS),
                Math.Abs((c["z"] as NbtInt).Value));
            id = (c["id"] as NbtString).Value;
            orig = c;
        }

        public TileEntity(NbtCompound c)
        {
            // TODO: Complete member initialization
            orig = c;
            Pos = new Vector3i(
                (c["x"] as NbtInt).Value,
                (c["y"] as NbtInt).Value,
                Math.Abs((c["z"] as NbtInt).Value));
            id = (c["id"] as NbtString).Value;
        }

        internal static TileEntity GetEntity(int CX, int CY, int CS, NbtCompound c)
        {
            switch ((c["id"] as NbtString).Value)
            {
                case "Chest":
                    return new Chest(CX,CY,CS,c);
                case "MobSpawner":
                    return new MobSpawner(CX, CY, CS, c);
                default:
                    Console.WriteLine("*** Unknown TileEntity: {0}", (c["id"] as NbtString).Value);
                    Console.WriteLine(c);
                    return new TileEntity(CX, CY, CS, c);
            }
        }

        public void Base2NBT(ref NbtCompound c)
        {
            c.Tags.Add(new NbtString("id", id));
            c.Tags.Add(new NbtInt("x", (int)Pos.X / 16));
            c.Tags.Add(new NbtInt("y", (int)Pos.X / 16));
            c.Tags.Add(new NbtInt("z", (int)Pos.X / 16));
        }

        public virtual NbtCompound ToNBT()
        {
            return orig;
        }

        public Guid UUID { get; set; }

        public virtual System.Drawing.Image Image { get { return Properties.Resources.mobnotch;} }
    }
}
