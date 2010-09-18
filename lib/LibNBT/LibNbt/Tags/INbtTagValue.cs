using System;
using System.Collections.Generic;
using System.Text;

namespace LibNbt.Tags
{
    internal interface INbtTagValue<T>
    {
        T Value { get; set; }
    }
}
