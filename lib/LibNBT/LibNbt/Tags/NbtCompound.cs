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
		public List<NbtTag> Tags { get; protected set; }

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
		protected Dictionary<string, NbtTag> TagCache { get; set; }

		public NbtCompound() : this("") { }
		public NbtCompound(string tagName) : this(tagName, new NbtTag[]{}) { }
		public NbtCompound(string tagName, IEnumerable<NbtTag> tags)
		{
			Name = tagName;
			Tags = new List<NbtTag>();
			TagCache = new Dictionary<string, NbtTag>();

			if (tags != null)
			{
				Tags.AddRange(tags);
			}
		}

		#region INbtTagList Methods
		public T Get<T>(int tagIdx) where T : NbtTag
		{
			return (T) Tags[tagIdx];
		}
		public void Set(int tagIdx, NbtTag tag)
		{
			if (tagIdx > Tags.Count)
			{
				throw new IndexOutOfRangeException();
			}

			Tags[tagIdx] = tag;
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
		#endregion

		public NbtTag Get(string tagName)
		{
			return Get<NbtTag>(tagName);
		}
		public T Get<T>(string tagName) where T : NbtTag
		{
			if (TagCache.ContainsKey(tagName) &&
				TagCache[tagName].Name.Equals(tagName))
			{
				return (T)TagCache[tagName];
			}
			foreach (NbtTag tag in Tags)
			{
				if (tag.Name.Equals(tagName))
				{
					lock (TagCache)
					{
						if (!TagCache.ContainsKey(tagName))
						{
							TagCache.Add(tagName, tag);
						}
					}
					return (T)tag;
				}
			}
			throw new KeyNotFoundException();
		}
		public void Set(string tagName, NbtTag tag)
		{
			foreach (var tg in Tags)
			{
				if (tg.Name.Equals(tagName))
				{
					int idx = Tags.IndexOf(tg);
					Tags[idx] = tag;
					if (TagCache.ContainsKey(tagName))
					{
						TagCache[tagName] = tag;
					}
					return;
				}
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
						var nextByte = new NbtByte();
						nextByte.ReadTag(readStream);
						Tags.Add(nextByte);
						break;
					case NbtTagType.TAG_Short:
						var nextShort = new NbtShort();
						nextShort.ReadTag(readStream);
						Tags.Add(nextShort);
						break;
					case NbtTagType.TAG_Int:
						var nextInt = new NbtInt();
						nextInt.ReadTag(readStream);
						Tags.Add(nextInt);
						break;
					case NbtTagType.TAG_Long:
						var nextLong = new NbtLong();
						nextLong.ReadTag(readStream);
						Tags.Add(nextLong);
						break;
					case NbtTagType.TAG_Float:
						var nextFloat = new NbtFloat();
						nextFloat.ReadTag(readStream);
						Tags.Add(nextFloat);
						break;
					case NbtTagType.TAG_Double:
						var nextDouble = new NbtDouble();
						nextDouble.ReadTag(readStream);
						Tags.Add(nextDouble);
						break;
					case NbtTagType.TAG_Byte_Array:
						var nextByteArray = new NbtByteArray();
						nextByteArray.ReadTag(readStream);
						Tags.Add(nextByteArray);
						break;
					case NbtTagType.TAG_String:
						var nextString = new NbtString();
						nextString.ReadTag(readStream);
						Tags.Add(nextString);
						break;
					case NbtTagType.TAG_List:
						var nextList = new NbtList();
						nextList.ReadTag(readStream);
						Tags.Add(nextList);
						break;
					case NbtTagType.TAG_Compound:
						var nextCompound = new NbtCompound();
						nextCompound.ReadTag(readStream);
						Tags.Add(nextCompound);
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
			var sb = new StringBuilder();
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
