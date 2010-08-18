using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaunchServer
{
    public class Packet023:Packet
    {
        int A; // ?
        byte B; // ?
        int C;
        int D;
        int E;

        public override string ToString()
        {
            return "{\n\tA:"+A.ToString()+"\n\tB:"+B.ToString()+"\n\tC:"+C.ToString()+"\n\tD:"+D.ToString()+"\n\tE:"+E.ToString()+"\n}";
        }

        public override int Length
        {
            get { return 17; }
        }

        public override int PacketID
        {
            get { return 23; }
        }

        public override string PacketName
        {
            get { return "PACKET023"; }
        }

        public override void Read(ref System.IO.BinaryReader stream)
        {
            A = stream.ReadInt32();
            B = stream.ReadByte();
            C = stream.ReadInt32();
            D = stream.ReadInt32();
            E = stream.ReadInt32();
        }

        public override void Write(ref System.IO.BinaryWriter stream)
        {
            stream.Write(A);
            stream.Write(B);
            stream.Write(C);
            stream.Write(D);
            stream.Write(E);
        }
    }
}
