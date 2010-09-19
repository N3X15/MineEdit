using System;
using System.IO;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtDouble : NbtTag, INbtTagValue<double>
	{
		public double Value { get; set; }

		public NbtDouble() : this("") { }
		public NbtDouble(string tagName) : this(tagName, 0.00f) { }
		[Obsolete("This constructor will be removed in favor of using NbtDouble(string tagName, double value)")]
		public NbtDouble(double value) : this("", value) { }
		public NbtDouble(string tagName, double value)
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


			var buffer = new byte[8];
			int totalRead = 0;
			while ((totalRead += readStream.Read(buffer, totalRead, 8)) < 8)
			{ }
			if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
			Value = BitConverter.ToDouble(buffer, 0);
		}

		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte)NbtTagType.TAG_Double);
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
			return NbtTagType.TAG_Double;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("TAG_Double");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": {0}", Value);
			return sb.ToString();
		}
	}
}
