using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
    internal interface INbtTagList
    {
        List<NbtTag> Tags { get; }

        T Get<T>(int tagIdx) where T : NbtTag;
    }
}
