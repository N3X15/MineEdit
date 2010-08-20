using System;
using System.Collections.Generic;

using System.Text;

namespace LaunchServer
{
    public abstract class IPlugin
    {
        public IPlugin(ServerWrapper w) 
        { 
            Console.WriteLine("Why is this being called oh god"); 
        }
        public IPlugin() { }
    }
}
