using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaunchServer
{
    [Flags]
    public enum PermissionsMask :int
    {
        Give,
        Ban,
        Unban,
        Op,
        Deop,
        Kick,
        ADMIN
    }
}
