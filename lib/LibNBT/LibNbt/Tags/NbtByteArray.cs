using System;
using System.IO;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtByteArray : NbtTag, INbtTagValue<byte[]>
	{
		public byte[] Value { get; set; }
		
		public byte this[int index]
		{
			get { return Value[index]; }
			set { Value[index] = value; }
		}

		public NbtByteArray() : this("") { }
		public NbtByteArray(string tagName) : this(tagName, new byte[] { }){ }
		[Obsolete("This constructor will be removed in favor of using NbtByteArray(string tagName, byte[] value)")]
		public NbtByteArray(byte[] value) : this("", value) { }
		public NbtByteArray(string tagName, byte[] value)
		{
			Name = tagName;
			Value = new byte[value.Length];
			Buffer.BlockCopy(value, 0, Value, 0, value.Length);
		}

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

			var length = new NbtInt();
			length.ReadTag(readStream, false);

			var buffer = new byte[length.Value];
			int totalRead = 0;
			while ((totalRead += readStream.Read(buffer, totalRead, length.Value - totalRead)) < length.Value)
			{}
			Value = buffer;
		}

		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte)NbtTagType.TAG_Byte_Array);
			if (writeName)
			{
				var name = new NbtString("", Name);
				name.WriteData(writeStream);
			}

			WriteData(writeStream);
		}

		internal override void WriteData(Stream writeStream)
		{
			var length = new NbtInt("", Value.Length);
			length.WriteData(writeStream);

			writeStream.Write(Value, 0, Value.Length);
		}

		internal override NbtTagType GetTagType()
		{
			return NbtTagType.TAG_Byte_Array;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("TAG_Byte_Array");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": [{0} bytes]", Value.Length);
			return sb.ToString();
		}
	}
}
