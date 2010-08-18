using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaunchServer
{
    public class LoginPacket:Packet
    {
        public int Sesskey { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        
        public LoginPacket() { }

        public override int Length
        {
            get { return 8 + Password.Length + Username.Length; }
        }

        public override int PacketID
        {
            get { return 1; }
        }

        public override string PacketName
        {
            get { return "LoginPacket"; }
        }

        public override void Read(ref System.IO.BinaryReader stream)
        {
            Sesskey = stream.ReadInt32();
            Password = stream.ReadString();
            Username = stream.ReadString();
        }

        public override void Write(ref System.IO.BinaryWriter stream)
        {
            stream.Write(Sesskey);
            stream.Write(Password);
            stream.Write(Username);
        }

        public override string ToString()
        {
            return "{"
                +"\n\tSession Key:"+Sesskey.ToString()
                +"\n\tPassword?:\""+Password+"\""
                +"\n\tUser Name:\""+Username+"\""
            +"\n}";
        }
    }
}
