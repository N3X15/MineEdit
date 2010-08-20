using System;
using System.Collections.Generic;

using System.Text;

namespace LaunchServer
{
    public class EntityUpdatePacket:Packet
    {
        bool what { get; set; }
        double Oh { get; set; }
        double God { get; set; }
        double What { get; set; }
        double IsThis { get; set; }
        public EntityUpdatePacket()
        {
        }

        public override int Length
        {
            get { return 1; }
        }

        public override int PacketID
        {
            get { return 10; }
        }

        public override string PacketName
        {
            get { return "EntityUpdatePacket"; }
        }

        public override void Read(ref System.IO.BinaryReader stream)
        {
            what = stream.ReadBoolean();
        }

        public override void Write(ref System.IO.BinaryWriter stream)
        {
            stream.Write(what);
        }

        public override string ToString()
        {
            return "{\n\tValue:"+what.ToString()+"\n}";
        }
    }
}
