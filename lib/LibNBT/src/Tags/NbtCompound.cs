using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtCompound : NbtTag
    {
        public List<NbtTag> Tags { get; protected set; }

        public NbtTag this[int idx]
        {
            get { return Tags[idx]; }
        }
        public NbtTag this[string name]
        {
            get
            {
                //Console.WriteLine(name);
                if (TagCache.ContainsKey(name) &&
                    TagCache[name].Name.Equals(name))
                {
                    return TagCache[name];
                }
                foreach (NbtTag tag in Tags)
                {
                    if (tag.Name.Equals(name))
                    {
                        lock(TagCache)
                        {
                            if (!TagCache.ContainsKey(name))
                            {
                                TagCache.Add(name, tag);
                            }
                        }
                        return tag;
                    }
                }
                throw new KeyNotFoundException();
            }
            set
            {
                if (!TagCache.ContainsKey(name))
                    TagCache.Add(name, value);
                else
                    TagCache[name] = value;
            }
        }
        protected Dictionary<string, NbtTag> TagCache { get; set; }

        public NbtCompound()
        {
            Tags = new List<NbtTag>();
            TagCache = new Dictionary<string, NbtTag>();
        }
		public NbtCompound(string tagName)
		{
			Name = tagName;
			Tags = new List<NbtTag>();
			TagCache = new Dictionary<string, NbtTag>();
		}

        internal override void SaveData(string recipient, object data)
        {
            foreach (NbtTag t in Tags)
                t.SaveData(recipient, data);
        }

        internal override void ReadTag(Stream readStream) { ReadTag(readStream, true); }
        internal override void ReadTag(Stream readStream, bool readName)
        {
            // First read the name of this tag
            Name = "";
            if (readName)
            {
                NbtString name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            Tags.Clear();
            bool foundEnd = false;
            while (!foundEnd)
            {
                int nextTag = readStream.ReadByte();
                switch((NbtTagType) nextTag)
                {
                    case NbtTagType.TAG_End:
                        foundEnd = true;
                        break;
                    case NbtTagType.TAG_Byte:
                        NbtByte nextByte = new NbtByte();
                        nextByte.ReadTag(readStream);
                        nextByte.Path = this.Path+"/"+nextByte.Name;
                        Tags.Add(nextByte);
                        break;
                    case NbtTagType.TAG_Short:
                        NbtShort nextShort = new NbtShort();
                        nextShort.ReadTag(readStream);
                        nextShort.Path = this.Path+"/"+nextShort.Name;
                        Tags.Add(nextShort);
                        break;
                    case NbtTagType.TAG_Int:
                        NbtInt nextInt = new NbtInt();
                        nextInt.ReadTag(readStream);
                        nextInt.Path = this.Path+"/"+nextInt.Name;
                        Tags.Add(nextInt);
                        break;
                    case NbtTagType.TAG_Long:
                        NbtLong nextLong = new NbtLong();
                        nextLong.ReadTag(readStream);
                        nextLong.Path = this.Path + "/" + nextLong.Name;
                        Tags.Add(nextLong);
                        break;
                    case NbtTagType.TAG_Float:
                        NbtFloat nextFloat = new NbtFloat();
                        nextFloat.ReadTag(readStream);
                        nextFloat.Path = this.Path+"/"+nextFloat.Name;
                        Tags.Add(nextFloat);
                        break;
                    case NbtTagType.TAG_Double:
                        NbtDouble nextDouble = new NbtDouble();
                        nextDouble.ReadTag(readStream);
                        nextDouble.Path = this.Path+"/"+nextDouble.Name;
                        Tags.Add(nextDouble);
                        break;
                    case NbtTagType.TAG_Byte_Array:
                        NbtByteArray nextByteArray = new NbtByteArray();
                        nextByteArray.ReadTag(readStream);
                        nextByteArray.Path = this.Path+"/"+nextByteArray.Name;
                        Tags.Add(nextByteArray);
                        break;
                    case NbtTagType.TAG_String:
                        NbtString nextString = new NbtString();
                        nextString.ReadTag(readStream);
                        nextString.Path = this.Path+"/"+nextString.Name;
                        Tags.Add(nextString);
                        break;
                    case NbtTagType.TAG_List:
                        NbtList nextList = new NbtList();
                        nextList.ReadTag(readStream);
                        nextList.Path = this.Path+"/"+nextList.Name;
                        Tags.Add(nextList);
                        break;
                    case NbtTagType.TAG_Compound:
                        NbtCompound nextCompound = new NbtCompound();
                        nextCompound.ReadTag(readStream);
                        nextCompound.Path = this.Path+"/"+nextCompound.Name;
                        Tags.Add(nextCompound);
                        break;
                    default:
                        Console.WriteLine(string.Format("Unsupported Tag Found: {0}", nextTag));
                        break;
                }
            }
        }

        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte) NbtTagType.TAG_Compound);
            if (writeName)
            {
                NbtString name = new NbtString("", Name);
                name.WriteData(writeStream);
            }

            WriteData(writeStream);
        }

        internal override void WriteData(Stream writeStream)
        {
            foreach (NbtTag tag in Tags)
            {
                tag.WriteTag(writeStream);
            }

            writeStream.WriteByte((byte) NbtTagType.TAG_End);
        }

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_Compound;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TAG_Compound");
            if (Name.Length > 0)
            {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": {0} entries\n", Tags.Count);

            sb.Append("{\n");
            foreach(NbtTag tag in Tags)
            {
                sb.AppendFormat("\t{0}\n", tag.ToString().Replace("\n", "\n\t"));
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
