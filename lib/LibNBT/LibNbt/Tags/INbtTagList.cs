using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
    internal interface INbtTagList
    {
        // Standardizing
        //List<NbtTag> Tags { get; }
        void Remove(int i);
        void Add(NbtTag t);
        void Set(int i, NbtTag t);

        T Get<T>(int tagIdx) where T : NbtTag;

        int Count { get; }

        IEnumerator<NbtTag> GetEnumerator();
    }
}
