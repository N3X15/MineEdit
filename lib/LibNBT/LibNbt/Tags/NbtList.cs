using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibNbt.Exceptions;
using LibNbt.Queries;

namespace LibNbt.Tags
{
    public class NbtList : NbtTag, INbtTagList
    {
        public List<NbtTag> Tags { get; protected set; }
        public NbtTagType Type { get; protected set; }

        public NbtTag this[int tagIdx]
        {
            get { return Get<NbtTag>(tagIdx); }
            set { Tags[tagIdx] = value; }
        }
        
        public NbtList() : this("") { }
        public NbtList(string tagName) : this(tagName, new NbtTag[] { }) { }
        public NbtList(string tagName, IEnumerable<NbtTag> tags)
        {
            Name = tagName;
            Tags = new List<NbtTag>();

            if (tags != null)
            {
                Tags.AddRange(tags);
            }
        }

        public NbtTag Get(int tagIdx)
        {
            return Get<NbtTag>(tagIdx);
        }
        public T Get<T>(int tagIdx) where T : NbtTag
        {
            return (T) Tags[tagIdx];
        }

        public override NbtTag Query(string query)
        {
            return Query<NbtTag>(query);
        }
        public override T Query<T>(string query)
        {
            var tagQuery = new TagQuery(query);

            return Query<T>(tagQuery);
        }
        internal override T Query<T>(TagQuery query, bool bypassCheck)
        {
            TagQueryToken token = query.Next();

            if (!bypassCheck)
            {
                if (token != null && !token.Name.Equals(Name))
                {
                    return null;
                }
            }

            var nextToken = query.Peek();
            if (nextToken != null)
            {
                // Make sure this token is an integer because NbtLists don't have
                // named tag items
                int tagIndex;
                if (!int.TryParse(nextToken.Name, out tagIndex))
                {
                    throw new NbtQueryException(
                        string.Format("Attempt to query by name on a list tag that doesn't support names. ({0})",
                                        Name));
                }

                var indexedTag = Get(tagIndex);
                if (indexedTag == null)
                {
                    return null;
                }

                if (query.TokensLeft() > 1)
                {
                    // Pop the index token so the current token is the next
                    // named token to continue the query
                    query.Next();

                    // Bypass the name check because the tag won't have one
                    return indexedTag.Query<T>(query, true);    
                }

                return (T) indexedTag;
            }

            return (T) ((NbtTag) this);
        }

        #region Reading Tag
        internal override void ReadTag(Stream readStream) { ReadTag(readStream, true); }
        internal override void ReadTag(Stream readStream, bool readName)
        {
            // First read the name of this tag
            Name = "";
            if (readName)
            {
                var name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            var tagId = new NbtByte();
            tagId.ReadTag(readStream, false);
            Type = (NbtTagType)tagId.Value;

            var length = new NbtInt();
            length.ReadTag(readStream, false);

            Tags.Clear();
            for (int idx = 0; idx < length.Value; idx++)
            {
                switch ((NbtTagType)tagId.Value)
                {
                    case NbtTagType.TAG_Byte:
                        var nextByte = new NbtByte();
                        nextByte.ReadTag(readStream, false);
                        Tags.Add(nextByte);
                        break;
                    case NbtTagType.TAG_Short:
                        var nextShort = new NbtShort();
                        nextShort.ReadTag(readStream, false);
                        Tags.Add(nextShort);
                        break;
                    case NbtTagType.TAG_Int:
                        var nextInt = new NbtInt();
                        nextInt.ReadTag(readStream, false);
                        Tags.Add(nextInt);
                        break;
                    case NbtTagType.TAG_Long:
                        var nextLong = new NbtLong();
                        nextLong.ReadTag(readStream, false);
                        Tags.Add(nextLong);
                        break;
                    case NbtTagType.TAG_Float:
                        var nextFloat = new NbtFloat();
                        nextFloat.ReadTag(readStream, false);
                        Tags.Add(nextFloat);
                        break;
                    case NbtTagType.TAG_Double:
                        var nextDouble = new NbtDouble();
                        nextDouble.ReadTag(readStream, false);
                        Tags.Add(nextDouble);
                        break;
                    case NbtTagType.TAG_Byte_Array:
                        var nextByteArray = new NbtByteArray();
                        nextByteArray.ReadTag(readStream, false);
                        Tags.Add(nextByteArray);
                        break;
                    case NbtTagType.TAG_String:
                        var nextString = new NbtString();
                        nextString.ReadTag(readStream, false);
                        Tags.Add(nextString);
                        break;
                    case NbtTagType.TAG_List:
                        var nextList = new NbtList();
                        nextList.ReadTag(readStream, false);
                        Tags.Add(nextList);
                        break;
                    case NbtTagType.TAG_Compound:
                        var nextCompound = new NbtCompound();
                        nextCompound.ReadTag(readStream, false);
                        Tags.Add(nextCompound);
                        break;
                }
            }
        }
        #endregion

        #region Write Tag
        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte)NbtTagType.TAG_List);
            if (writeName)
            {
                var name = new NbtString("", Name);
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

            var tagType = new NbtByte("", (byte)Type);
            tagType.WriteData(writeStream);

            var length = new NbtInt("", Tags.Count);
            length.WriteData(writeStream);

            foreach (NbtTag tag in Tags)
            {
                tag.WriteData(writeStream);
            }
        }
        #endregion

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_List;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
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
