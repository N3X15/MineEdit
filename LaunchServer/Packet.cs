using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace LaunchServer
{
    public delegate bool PacketHandler(Stream src);
    public abstract class Packet
    {
        public abstract void Read(ref BinaryReader stream);
        public abstract void Write(ref BinaryWriter stream);
        public abstract int Length {get;}
        public abstract int PacketID {get;}
        public abstract string PacketName {get;}
    }
    public class PacketReader
    {
        static Dictionary<int, Packet> PacketHandlers = new Dictionary<int, Packet>();
        static bool Initialized = false;
        public static void RegisterPacketHandler(int PacketID, Packet Handler)
        {
            if(!PacketHandlers.ContainsKey(PacketID))
                PacketHandlers.Add(PacketID,Handler);
        }

        public static Packet NextPacket(ref BinaryReader rdr)
        {
            if (!Initialized)
                Init();
            int pid = (int)rdr.ReadByte();
            if (!PacketHandlers.ContainsKey(pid))
            {
#if DEBUG
                throw new Exception("Packet ID " + pid.ToString() + " is unknown to me!");
#else
                Console.WriteLine("Packet ID " + pid.ToString() + " is unknown to me!");
                return null;
            }
#endif
            Packet p = PacketHandlers[pid];
            p.Read(ref rdr);
            return p;
        }
        public static void WritePacket(ref BinaryWriter w, Packet p)
        {
            w.Write((byte)p.PacketID);
            p.Write(ref w);
        }
        public static void Init()
        {
            RegisterPacketHandler(0, new NOOPPacket());
            RegisterPacketHandler(1, new LoginPacket());
            RegisterPacketHandler(3, new Packet03Handler());

            RegisterPacketHandler(10, new EntityUpdatePacket());

            RegisterPacketHandler(23, new Packet023());

            RegisterPacketHandler(255, new DisconnectPacket());
            Initialized = true;
        }
    }
}
