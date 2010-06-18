using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtByteArray : NbtTag
    {
        public byte[] Value { get; protected set; }
		
		public byte this[int index]
		{
			get { return Value[index]; }
			set { Value[index] = value; }
		}
		
		public NbtByteArray()
        {
            Name = "";
            Value = new byte[0];
        }
		public NbtByteArray(string tagName)
		{
			Name = tagName;
			Value = new byte[0];
		}
		public NbtByteArray(byte[] value)
		{
			Name = "";
			Value = new byte[value.Length];
			Buffer.BlockCopy(value, 0, Value, 0, value.Length);
		}
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
                NbtString name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            NbtInt length = new NbtInt();
            length.ReadTag(readStream, false);

            byte[] buffer = new byte[length.Value];
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
                NbtString name = new NbtString("", Name);
                name.WriteData(writeStream);
            }

            WriteData(writeStream);
        }

        internal override void WriteData(Stream writeStream)
        {
            NbtInt length = new NbtInt(Value.Length);
            length.WriteData(writeStream);

            writeStream.Write(Value, 0, Value.Length);
        }

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_Byte_Array;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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
