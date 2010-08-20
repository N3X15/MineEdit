using System;
using System.Collections.Generic;

using System.Text;

namespace LaunchServer.CorePlugins
{
    public class Who:IPlugin
    {
        ServerWrapper wrap;

        public Who(ServerWrapper w)
        {
            wrap = w;
            wrap.ChatCommand += new ChatCommandHandler(wrap_ChatCommand);
        }

        void wrap_ChatCommand(Player player, string cmd, string[] args)
        {
            if (cmd == "who")
            {
                string[] p = new string[wrap.CurrentPlayers.Count];
                int i = 0;
                foreach(KeyValuePair<string,string> kvp in wrap.CurrentPlayers)
                {
                    p[i++]=kvp.Key;
                }
                wrap.Say(string.Join(", ", p));
            }
        }
    }
}
