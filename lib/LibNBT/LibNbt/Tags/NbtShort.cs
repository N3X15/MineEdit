using System;
using System.IO;
using System.Text;
using LibNbt.Exceptions;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtShort : NbtTag, INbtTagValue<short>
	{
		public short Value { get; set; }

		public NbtShort() : this("") { }
		public NbtShort(string tagName) : this(tagName, 0) { }
		[Obsolete("This constructor will be removed in favor of using NbtShort(string tagName, short value)")]
		public NbtShort(short value) : this("", value) { }
		public NbtShort(string tagName, short value)
		{
			Name = tagName;
			Value = value;
		}

		internal override void ReadTag(Stream readStream) { ReadTag(readStream, true); }
		internal override void ReadTag(Stream readStream, bool readName)
		{
			if (readName)
			{
				var name = new NbtString();
				name.ReadTag(readStream, false);

				Name = name.Value;
			}

			var buffer = new byte[2];
			int numRead = readStream.Read(buffer, 0, buffer.Length);
			if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
			if (numRead == 2)
			{
				Value = BitConverter.ToInt16(buffer, 0);
			}
		}

		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte)NbtTagType.TAG_Short);
			if (writeName)
			{
				var name = new NbtString("", Name);
				name.WriteData(writeStream);
			}

			WriteData(writeStream);
		}

		internal override void WriteData(Stream writeStream)
		{
			byte[] data = BitConverter.GetBytes(Value);
			if (BitConverter.IsLittleEndian) Array.Reverse(data);
			writeStream.Write(data, 0, data.Length);
		}

		internal override NbtTagType GetTagType()
		{
			return NbtTagType.TAG_Short;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("TAG_Short");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": {0}", Value);
			return sb.ToString();
		}
	}
}
