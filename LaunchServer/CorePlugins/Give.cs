using LaunchServer;
namespace LaunchServer.CorePlugins
{
    public class Give:IPlugin
    {
        ServerWrapper wrap;

        public Give(ServerWrapper w)
        {
            wrap = w;
            wrap.Join += new JoinPartHandler(wrap_Join);
            wrap.ChatCommand += new ChatCommandHandler(wrap_ChatCommand);
        }

        void wrap_ChatCommand(Player player, string cmd, string[] args)
        {
            if (cmd == "give" || cmd=="#")
            {
                if ((player.Permissions & PermissionsMask.Give) != PermissionsMask.Give)
                {
                    wrap.Say("You do not have permission to execute this command, peasant.");
                    return;
                }
                if(args.Length<1)
                {
                    wrap.Say("Syntax: !{0} ID [Number]");
                    return;
                }
                byte num = 1;
                short item = 0;
                if(!short.TryParse(args[0],out item))
                {
                    wrap.Say("Failed to parse argument #1, which should be a number between 1 and "+short.MaxValue.ToString()+".");
                    return;
                }
                if(args.Length==2)
                {
                    if(!byte.TryParse(args[1], out num))
                    {
                        wrap.Say("Failed to parse argument #2, which should be a number between 1 and 64.");
                        return;
                    }
                }
                wrap.Give(player.Name, item, num);
            }
        }

        void wrap_Join(Player player, string IP, int port)
        {
            wrap.Give(player.Name, 276, 1); // Sword
            wrap.Give(player.Name, 277, 1); // Shovel
            wrap.Give(player.Name, 278, 1); // Pickaxe
            wrap.Give(player.Name, 279, 1); // Axe
        }
    }
}
