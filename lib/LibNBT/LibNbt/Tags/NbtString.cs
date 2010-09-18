using System;
using System.IO;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
    public class NbtString : NbtTag, INbtTagValue<string>
    {
        public string Value { get; set; }

        public NbtString() : this("") { }
        public NbtString(string tagName) : this(tagName, "") { }
        public NbtString(string tagName, string value)
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

            // Get the length of the string
            var length = new NbtShort();
            length.ReadTag(readStream, false);

            byte[] buffer = new byte[length.Value];
            int totalRead = 0;
            while ((totalRead += readStream.Read(buffer, totalRead, length.Value)) < length.Value)
            { }
            Value = Encoding.UTF8.GetString(buffer);
        }

        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte)NbtTagType.TAG_String);
            if (writeName)
            {
                var name = new NbtString("", Name);
                name.WriteData(writeStream);
            }

            WriteData(writeStream);
        }

        internal override void WriteData(Stream writeStream)
        {
            byte[] str = Encoding.UTF8.GetBytes(Value);

            var length = new NbtShort("", (short)str.Length);
            length.WriteData(writeStream);

            writeStream.Write(str, 0, str.Length);
        }

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_String;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("TAG_String");
            if (Name.Length > 0)
            {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": {0}", Value);
            return sb.ToString();
        }
    }
}
