using System;
using System.IO;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtByte : NbtTag, INbtTagValue<byte>
	{
		public byte Value { get; set; }

		public NbtByte() : this("") { }
		public NbtByte(string tagName) : this(tagName, 0x00) { }
		[Obsolete("This constructor will be removed in favor of using NbtByte(string tagName, byte value)")]
		public NbtByte(byte value) :this("", value) { }
		public NbtByte(string name, byte value)
		{
			Name = name;
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

			var buffer = new byte[1];
			int totalRead = 0;
			while ((totalRead += readStream.Read(buffer, totalRead, 1)) < 1)
			{ }
			Value = buffer[0];
		}

		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte)NbtTagType.TAG_Byte);
			if (writeName)
			{
				var name = new NbtString("", Name);
				name.WriteData(writeStream);
			}

			WriteData(writeStream);
		}

		internal override void WriteData(Stream writeStream)
		{
			writeStream.WriteByte(Value);
		}

		internal override NbtTagType GetTagType()
		{
			return NbtTagType.TAG_Byte;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("TAG_Byte");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": {0}", Value);
			return sb.ToString();
		}
	}
}
