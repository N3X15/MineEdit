using System;
using System.Collections.Generic;
using System.Text;

namespace LibNbt.Queries
{
    public class TagQueryToken
    {
        public TagQuery Query { get; internal set; }
        public string Name { get; internal set; }
    }
}
