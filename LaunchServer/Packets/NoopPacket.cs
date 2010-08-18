using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaunchServer
{
    public class NOOPPacket:Packet
    {
        public NOOPPacket()
        {
        }

        public override int Length
        {
            get { return 0; }
        }

        public override int PacketID
        {
            get { return 0; }
        }

        public override string PacketName
        {
            get { return "NULL"; }
        }

        public override void Read(ref System.IO.BinaryReader stream)
        {
            
        }

        public override void Write(ref System.IO.BinaryWriter stream)
        {
            
        }
    }
}
