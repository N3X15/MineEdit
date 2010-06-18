using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtShort : NbtTag
    {
        public short Value { get; protected set; }

        public NbtShort()
        {
            Name = "";
            Value = 0;
        }
		public NbtShort(string tagName)
		{
			Name = tagName;
			Value = 0;
		}
        public NbtShort(short value)
        {
            Name = "";
            Value = value;
        }
        public NbtShort(string tagName, short value)
        {
            Name = tagName;
            Value = value;
        }

        internal override void ReadTag(System.IO.Stream readStream) { ReadTag(readStream, true); }
        internal override void ReadTag(System.IO.Stream readStream, bool readName)
        {
            if (readName)
            {
                NbtString name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            byte[] buffer = new byte[2];
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
                NbtString name = new NbtString("", Name);
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
            StringBuilder sb = new StringBuilder();
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
