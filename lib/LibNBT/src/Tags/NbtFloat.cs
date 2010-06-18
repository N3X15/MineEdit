using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public class NbtFloat : NbtTag
    {
        public float Value { get; protected set; }

        public NbtFloat()
        {
            Name = "";
            Value = 0.00f;
        }
		public NbtFloat(string tagName)
		{
			Name = tagName;
			Value = 0.00f;
		}
		public NbtFloat(float value)
		{
			Name = "";
			Value = value;
		}
		public NbtFloat(string tagName, float value)
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


            byte[] buffer = new byte[4];
            int totalRead = 0;
            while ((totalRead += readStream.Read(buffer, totalRead, 4)) < 4)
            { }
            if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
            Value = BitConverter.ToSingle(buffer, 0);
        }

        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte)NbtTagType.TAG_Float);
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
            return NbtTagType.TAG_Float;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TAG_Float");
            if (Name.Length > 0)
            {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": {0}", Value);
            return sb.ToString();
        }
    }
}
