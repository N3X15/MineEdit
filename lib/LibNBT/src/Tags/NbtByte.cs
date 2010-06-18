using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtByte : NbtTag
    {
        public byte Value { get; protected set; }

        public NbtByte()
        {
            Name = "";
            Value = 0x00;
        }
		public NbtByte(string tagName)
		{
			Name = tagName;
			Value = 0x00;
		}
        public NbtByte(byte value)
        {
            Name = "";
            Value = value;
        }
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
                NbtString name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            byte[] buffer = new byte[1];
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
                NbtString name = new NbtString("", Name);
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
            StringBuilder sb = new StringBuilder();
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
