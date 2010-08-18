using LaunchServer;
using System.Net;
using System.Collections.Generic;
namespace LaunchServer.CorePlugins
{
    public class Ban:IPlugin
    {
        ServerWrapper wrap;
        public List<string> BannedIPs = new List<string>();
        public Ban(ServerWrapper w)
        {
            wrap = w;
            wrap.ChatCommand += new ChatCommandHandler(wrap_ChatCommand);
            Proxy.AddDelegate(false, 1, CheckLogin);
        }

        Packet CheckLogin(Proxy.ClientSession sess, Packet p, IPEndPoint ip)
        {
            string IP = ip.ToString();
            if (BannedIPs.Contains(IP))
                sess.InjectPacket(new DisconnectPacket() {Message="You are banned from this server."});

            return p;
        }
        void wrap_ChatCommand(Player player, string cmd, string[] args)
        {
            if (cmd == "ban" || cmd=="b")
            {
                if ((player.Permissions & PermissionsMask.Ban) != PermissionsMask.Ban)
                {
                    wrap.Say("You do not have permission to execute this command, peasant.");
                    return;
                }
                if (args.Length < 1)
                {
                    wrap.Say("Syntax: !ban UserName");
                    return;
                }
                wrap.Say(string.Format("{0} banned {1} from the server.", player.Name, args[0]));
                wrap.Ban(args[0]);
                wrap.Kick(args[0]);
            }
            if (cmd == "unban")
            {
                if ((player.Permissions & PermissionsMask.Ban) != PermissionsMask.Ban)
                {
                    wrap.Say("You do not have permission to execute this command, peasant.");
                    return;
                }
                if (args.Length < 1)
                {
                    wrap.Say("Syntax: !unban UserName");
                    return;
                }
                wrap.Say(string.Format("{0} unbanned {1} from the server.", player.Name, args[0]));
                wrap.Unban(args[0]);
            }
            if (cmd == "kick")
            {
                if ((player.Permissions & PermissionsMask.Kick) != PermissionsMask.Kick)
                {
                    wrap.Say("You do not have permission to execute this command, peasant.");
                    return;
                }
                if (args.Length < 1)
                {
                    wrap.Say("Syntax: !kick UserName");
                    return;
                }
                wrap.Kick(args[0]);
            }
        }
    }
}
