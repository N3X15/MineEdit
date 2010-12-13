using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibNbt.Exceptions;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtCompound : NbtTag, INbtTagList
	{
        // Do not expose, so we can maintain referential integrity with TagIndices.
        private Dictionary<string, NbtTag> mTags = new Dictionary<string, NbtTag>();

        // Complete bullshit, just to satisfy get<>(int)
        private List<string> mTagIndices = new List<string>();

        
        public NbtTag this[int tagIdx]
        {
            get { return Get<NbtTag>(tagIdx); }
            set { Set(tagIdx, value); }
        }
        public NbtTag this[string tagName]
        {
            get { return Get<NbtTag>(tagName); }
            set { Set(tagName, value); }
        }

        public void Set(int tagIdx, NbtTag value)
        {
            if (tagIdx >= mTagIndices.Count)
                throw new KeyNotFoundException();

            string k = mTagIndices[tagIdx];
            Set(k, value);
        }

        public T Get<T>(int tagIdx) where T:NbtTag
        {
            if (tagIdx >= mTagIndices.Count)
                throw new KeyNotFoundException();

            string k = mTagIndices[tagIdx];
            return (T)mTags[k];
        }

        public void Remove(int tagIdx)
        {
            if (tagIdx >= mTagIndices.Count)
                throw new KeyNotFoundException();

            string k = mTagIndices[tagIdx];
            mTags.Remove(k);
        }
		public NbtCompound() : this("") { }
		public NbtCompound(string tagName) : this(tagName, new NbtTag[]{}) { }
		public NbtCompound(string tagName, IEnumerable<NbtTag> tags)
		{
			Name = tagName;
            mTags = new Dictionary<string, NbtTag>();

			if (tags != null)
			{
                foreach (NbtTag t in tags)
                {
                    if (!mTags.ContainsKey(t.Name))
                        mTags.Add(t.Name,t);
                }
			}
		}

		#region INbtTagList Methods
        
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
            TagQueryToken token = null;

            if (!bypassCheck)
            {
                token = query.Next();

                if (token != null && !token.Name.Equals(Name))
                {
                    return null;
                }
            }

            TagQueryToken nextToken = query.Peek();
            if (nextToken != null)
            {
                NbtTag nextTag = Get(nextToken.Name);
                if (nextTag == null)
                {
                    return null;
                }

                return nextTag.Query<T>(query);
            }

            return (T)((NbtTag)this);
        }
        internal override void SetQuery<T>(TagQuery query, T val, bool bypassCheck)
        {
            TagQueryToken token = null;

            if (!bypassCheck)
            {
                token = query.Next();

                if (token != null && !token.Name.Equals(Name))
                {
                    return;
                }
            }

            TagQueryToken nextToken = query.Peek();
            if (nextToken != null)
            {
                NbtTag nextTag = Get(nextToken.Name);
                if (nextTag == null)
                {
                    return;
                }

                nextTag.SetQuery<T>(query,val,false);
                Set(nextToken.Name, nextTag);
                return;
            }
        }
		#endregion

		public NbtTag Get(string tagName)
		{
			return Get<NbtTag>(tagName);
		}
        public bool Has(string tagName)
        {
            return mTags.ContainsKey(tagName);
        }
		public T Get<T>(string tagName) where T : NbtTag
		{
			if (mTags.ContainsKey(tagName) &&
				mTags[tagName].Name.Equals(tagName))
			{
				return (T)mTags[tagName];
			}
            Console.WriteLine(this.ToString());
			throw new KeyNotFoundException(tagName); // Make it a LITTLE more helpful.
		}

        public void Add(NbtTag tag)
        {
            Add(tag.Name, tag);
        }

        public void Add(string tagName, NbtTag tag)
        {
            if (mTags.ContainsKey(tagName))
            {
                Set(tagName, tag);
                return;
            }

            tag.Name = tagName; // Just make sure.
            mTags.Add(tagName,tag);
            mTagIndices.Add(tagName);
        }


        public void Remove(NbtTag tag)
        {
            Remove(tag.Name);
        }

        public void Remove(string tagName)
        {
            if (mTags.ContainsKey(tagName))
            {
                mTags.Remove(tagName);
                mTagIndices.Remove(tagName);
            }
        }

		public void Set(string tagName, NbtTag tag)
		{
            if (!mTags.ContainsKey(tagName))
                mTags.Add(tagName,tag);

            tag.Name = tagName; // Just make sure.
            mTags[tagName] = tag;
		}

        public void AddRange(IEnumerable<NbtTag> stuff)
        {
            foreach (NbtTag tag in stuff)
            {
                Set(tag.Name, tag);
            }
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

			mTags.Clear();
			bool foundEnd = false;
            List<NbtTag> buffer = new List<NbtTag>();
			while (!foundEnd)
			{
				int nextTag = readStream.ReadByte();
				switch((NbtTagType) nextTag)
				{
					case NbtTagType.TAG_End:
						foundEnd = true;
						break;
					case NbtTagType.TAG_Byte:
						var nextByte = new NbtByte();
						nextByte.ReadTag(readStream);
						Add(nextByte.Name,nextByte);
						break;
					case NbtTagType.TAG_Short:
						var nextShort = new NbtShort();
						nextShort.ReadTag(readStream);
						Add(nextShort.Name,nextShort);
						break;
					case NbtTagType.TAG_Int:
						var nextInt = new NbtInt();
						nextInt.ReadTag(readStream);
						Add(nextInt.Name,nextInt);
						break;
					case NbtTagType.TAG_Long:
						var nextLong = new NbtLong();
						nextLong.ReadTag(readStream);
						Add(nextLong.Name,nextLong);
						break;
					case NbtTagType.TAG_Float:
						var nextFloat = new NbtFloat();
						nextFloat.ReadTag(readStream);
                        Add(nextFloat.Name, nextFloat);
						break;
					case NbtTagType.TAG_Double:
						var nextDouble = new NbtDouble();
						nextDouble.ReadTag(readStream);
                        Add(nextDouble.Name, nextDouble);
						break;
					case NbtTagType.TAG_Byte_Array:
						var nextByteArray = new NbtByteArray();
						nextByteArray.ReadTag(readStream);
                        Add(nextByteArray.Name, nextByteArray);
						break;
					case NbtTagType.TAG_String:
						var nextString = new NbtString();
						nextString.ReadTag(readStream);
                        Add(nextString.Name, nextString);
						break;
					case NbtTagType.TAG_List:
						var nextList = new NbtList();
						nextList.ReadTag(readStream);
                        Add(nextList.Name, nextList);
						break;
					case NbtTagType.TAG_Compound:
						var nextCompound = new NbtCompound();
						nextCompound.ReadTag(readStream);
                        Add(nextCompound.Name, nextCompound);
						break;
					default:
						Console.WriteLine(string.Format("Unsupported Tag Found: {0}", nextTag));
						break;
				}
			}
		}
		#endregion

		#region Writing Tag
		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte) NbtTagType.TAG_Compound);
			if (writeName)
			{
				var name = new NbtString("", Name);
				name.WriteData(writeStream);
			}

			WriteData(writeStream);
		}
		#endregion

		internal override void WriteData(Stream writeStream)
		{
			foreach (NbtTag tag in mTags.Values)
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
			var sb = new StringBuilder();
			sb.Append("TAG_Compound");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": {0} entries\n", mTags.Count);

			sb.Append("{\n");
			foreach(NbtTag tag in mTags.Values)
			{
				sb.AppendFormat("\t{0}\n", tag.ToString().Replace("\n", "\n\t"));
			}
			sb.Append("}");
			return sb.ToString();
		}


        public int Count
        {
            get { return mTags.Count; }
        }

        public IEnumerator<NbtTag> GetEnumerator()
        {
            return mTags.Values.GetEnumerator();
        }
    }
}
