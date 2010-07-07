using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using System.Drawing;
namespace MineEdit
{
    public class Entity
    {
        public Vector3d Pos = new Vector3d();
        public Vector3d Motion = new Vector3d();
        public short Air = 300;
        public short Fire = 0;
        public float FallDistance = 0f;
        public NbtTag Rotation;
        private NbtCompound orig;
        private string id;
        public int ChunkX=0;
        public int ChunkY=0;
        public Vector3d OrigPos;

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
            Console.WriteLine("*** BUG: Unknown entity (ID: {0})",id);
            Console.WriteLine(orig);
        }


        internal void SetBaseStuff(NbtCompound c)
        {
            Air = (c["Air"] as NbtShort).Value;
            Fire = (c["Fire"] as NbtShort).Value;
            FallDistance = (c["FallDistance"] as NbtFloat).Value;
            Motion = new Vector3d(c["Motion"] as NbtList);
            Pos = new Vector3d(c["Pos"] as NbtList);
            Pos = new Vector3d(Pos.X, Pos.Z, Pos.Y);
            Rotation = c["Rotation"];
            Console.WriteLine("Loaded entity {0} @ {1}", (c["id"] as NbtString).Value, Pos);
        }

        internal void Base2NBT(ref NbtCompound c,string _id)
        {
            c.Tags.Add(new NbtShort("Air", Air));
            c.Tags.Add(new NbtShort("Fire", Fire));
            c.Tags.Add(new NbtFloat("FallDistance", FallDistance));
            c.Tags.Add(new NbtString("id", _id));
            NbtList motion = new NbtList("Motion");
            motion.Tags.AddRange(new NbtDouble[]{
                new NbtDouble(Motion.X),
                new NbtDouble(Motion.Y),
                new NbtDouble(Motion.Z)
            });
            c.Tags.Add(motion);
            NbtList pos = new NbtList("Pos");
            pos.Tags.AddRange(new NbtDouble[]{
                new NbtDouble(Pos.X),
                new NbtDouble(Pos.Z),
                new NbtDouble(Pos.Y)
            });
            c.Tags.Add(pos);
            c.Tags.Add(Rotation);
        }
        public override string ToString()
        {
            return "[UNKNOWN ENTITY: " + id + "]";
        }

        public static string[] GetEntityList()
        {
            return new string[]
            {
                "FallingSand"
            };
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
                default: 
                    return new Entity(c);
            }
        }

        public virtual string GetID() { return "_NULL_"; }

        public virtual Image Image {
            get
            {
                return MineEdit.Properties.Resources.mobpig;
            }
        }

        public Guid UUID { get; set; }
    }
}
