using System;
using System.Collections.Generic;

using System.Text;

namespace LaunchServer
{
    public class Packet03Handler:Packet
    {
        public Packet03Handler()
        {
        }

        public string Message
        { get; set; }

        public override int Length
        {
            get { return Message.Length; }
        }

        public override int PacketID
        {
            get { return 3; }
        }

        public override string PacketName
        {
            get { return "PACKET03"; }
        }

        public override void Read(ref System.IO.BinaryReader stream)
        {
            Message = stream.ReadString();
        }

        public override void Write(ref System.IO.BinaryWriter stream)
        {
            stream.Write(Message);
        }

        public override string ToString()
        {
            return "{\n\tMessage:\""+Message+"\"\n}";
        }
    }
}
