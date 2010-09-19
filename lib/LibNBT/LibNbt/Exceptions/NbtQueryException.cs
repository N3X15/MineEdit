using System;
using System.Collections.Generic;
using System.Text;

namespace LibNbt.Exceptions
{
    public class NbtQueryException : Exception
    {
        public NbtQueryException(string message) : base(message) { }
    }
}
