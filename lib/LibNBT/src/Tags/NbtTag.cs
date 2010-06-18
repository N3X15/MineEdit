using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNbt.Tags
{
    public abstract class NbtTag
    {
        protected NbtTag() { Name = ""; }

        public string Name { get; protected set; }

        internal abstract void ReadTag(Stream readStream);
        internal abstract void ReadTag(Stream readStream, bool readName);

        // WriteTag writes the whole tag, including the ID byte
        internal abstract void WriteTag(Stream writeStream);
        internal abstract void WriteTag(Stream writeStream, bool writeName);

        // WriteData does not write the tag's ID byte or the name
        internal abstract void WriteData(Stream writeStream);

        internal virtual NbtTagType GetTagType() { return NbtTagType.TAG_Unknown; }
    }
}
