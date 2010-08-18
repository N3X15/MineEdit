using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;

namespace LaunchServer
{
    class Settings
    {
        static Dictionary<string, string> Stuff = new Dictionary<string, string>();
        internal static void Set(string k, string v)
        {
            if (!Stuff.ContainsKey(k))
                Stuff.Add(k, v);
            else
                Stuff[k] = v;
            Save();
        }

        private static void Save()
        {
            XmlWriter xml = new XmlTextWriter("wrapper.xml",Encoding.UTF8);
            xml.WriteStartDocument();

            xml.WriteStartElement("config");
            foreach (KeyValuePair<string, string> kvp in Stuff)
            {
                xml.WriteStartElement("entry");
                xml.WriteAttributeString("name", kvp.Key);
                xml.WriteValue(kvp.Value);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteEndDocument();
            xml.Close();
        }


        public static void Init()
        {
            if (!File.Exists("wrapper.xml")) return;
            XmlDocument doc = new XmlDocument();
            doc.Load("wrapper.xml");
            Stuff.Clear();
            foreach (XmlNode nod in doc.GetElementsByTagName("entry"))
            {
                Stuff.Add(nod.Attributes["name"].Value, nod.InnerText);
            }
        }
        public static string Get(string k,string def)
        {
            if (!Stuff.ContainsKey(k) && def != "")
            {
                Stuff.Add(k, def);
                Save();
                return def;
            }
            Console.WriteLine("{0}={1}", k, Stuff[k]);
            return Stuff[k];
        }
    }
}
