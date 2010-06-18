using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtList : NbtTag
    {
        public List<NbtTag> Tags { get; protected set; }
        public NbtTagType Type { get; protected set; }
		
		public NbtTag this[int index]
		{
			get { return Tags[index]; }
			set { Tags[index] = value; }
		}
		
		public NbtList()
        {
            Name = "";
            Tags = new List<NbtTag>();
        }
		public NbtList(string tagName)
		{
			Name = tagName;
			Tags = new List<NbtTag>();
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

            NbtByte tagId = new NbtByte();
            tagId.ReadTag(readStream, false);
            Type = (NbtTagType)tagId.Value;

            NbtInt length = new NbtInt();
            length.ReadTag(readStream, false);

            Tags.Clear();
            for (int idx = 0; idx < length.Value; idx++)
            {
                switch ((NbtTagType)tagId.Value)
                {
                    case NbtTagType.TAG_Byte:
                        NbtByte nextByte = new NbtByte();
                        nextByte.ReadTag(readStream, false);
                        Tags.Add(nextByte);
                        break;
                    case NbtTagType.TAG_Short:
                        NbtShort nextShort = new NbtShort();
                        nextShort.ReadTag(readStream, false);
                        Tags.Add(nextShort);
                        break;
                    case NbtTagType.TAG_Int:
                        NbtInt nextInt = new NbtInt();
                        nextInt.ReadTag(readStream, false);
                        Tags.Add(nextInt);
                        break;
                    case NbtTagType.TAG_Long:
                        NbtLong nextLong = new NbtLong();
                        nextLong.ReadTag(readStream, false);
                        Tags.Add(nextLong);
                        break;
                    case NbtTagType.TAG_Float:
                        NbtFloat nextFloat = new NbtFloat();
                        nextFloat.ReadTag(readStream, false);
                        Tags.Add(nextFloat);
                        break;
                    case NbtTagType.TAG_Double:
                        NbtDouble nextDouble = new NbtDouble();
                        nextDouble.ReadTag(readStream, false);
                        Tags.Add(nextDouble);
                        break;
                    case NbtTagType.TAG_Byte_Array:
                        NbtByteArray nextByteArray = new NbtByteArray();
                        nextByteArray.ReadTag(readStream, false);
                        Tags.Add(nextByteArray);
                        break;
                    case NbtTagType.TAG_String:
                        NbtString nextString = new NbtString();
                        nextString.ReadTag(readStream, false);
                        Tags.Add(nextString);
                        break;
                    case NbtTagType.TAG_List:
                        NbtList nextList = new NbtList();
                        nextList.ReadTag(readStream, false);
                        Tags.Add(nextList);
                        break;
                    case NbtTagType.TAG_Compound:
                        NbtCompound nextCompound = new NbtCompound();
                        nextCompound.ReadTag(readStream, false);
                        Tags.Add(nextCompound);
                        break;
                }
            }
        }

        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte)NbtTagType.TAG_List);
            if (writeName)
            {
                NbtString name = new NbtString("", Name);
                name.WriteData(writeStream);
            }

            WriteData(writeStream);
        }

        internal override void WriteData(Stream writeStream)
        {
            // Figure out the type of this list, then check
            // to make sure all elements are that type.
            if (Tags.Count > 0)
            {
                NbtTagType listType = Tags[0].GetTagType();
                foreach(NbtTag tag in Tags)
                {
                    if (tag.GetTagType() != listType)
                    {
                        throw new Exception("All list items must be the same tag type.");
                    }
                }
                Type = listType;
            }

            NbtByte tagType = new NbtByte((byte)Type);
            tagType.WriteData(writeStream);

            NbtInt length = new NbtInt(Tags.Count);
            length.WriteData(writeStream);

            foreach (NbtTag tag in Tags)
            {
                tag.WriteData(writeStream);
            }
        }

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_List;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TAG_List");
            if (Name.Length > 0)
            {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": {0} entries\n", Tags.Count);

            sb.Append("{\n");
            foreach (NbtTag tag in Tags)
            {
                sb.AppendFormat("\t{0}\n", tag.ToString().Replace("\n", "\n\t"));
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
