using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibNbt;
using LibNbt.Tags;

namespace MineEdit
{
    class Settings
    {
        // Rendering info.
        private static bool _ShowGridLines = false;
        private static bool _ShowChunks = true;
        private static bool _ShowMapIcons = true;
        private static bool _ShowWaterDepth = false;
        // Just for ease of loading.
        public static Dictionary<short, float> Worlds = new Dictionary<short, float>();

        public static bool ShowGridLines
        {
            get
            {
                return _ShowGridLines;
            }
            set
            {
                _ShowGridLines = value;
                Save();
            }
        }
        public static bool ShowChunks
        {
            get
            {
                return _ShowChunks;
            }
            set
            {
                _ShowChunks = value;
                Save();
            }
        }
        public static bool ShowMapIcons
        {
            get
            {
                return _ShowMapIcons;
            }
            set
            {
                _ShowMapIcons = value;
                Save();
            }
        }
        public static bool ShowWaterDepth
        {
            get
            {
                return _ShowWaterDepth;
            }
            set
            {
                _ShowWaterDepth = value;
                Save();
            }
        }
        public static float MiniMapScale = 0.75f;
        public static Stack<string> LastUsedFiles = new Stack<string>();
        public static void Init()
        {
            if (File.Exists(".luf"))
            {
                foreach (string f in File.ReadAllLines(".luf"))
                {
                    if(File.Exists(f))
                        LastUsedFiles.Push(f);
                }
            }
            if (File.Exists(".settings"))
            {
                NbtFile f = new NbtFile(".settings");
                f.LoadFile();
                ShowGridLines = f.RootTag["GridLines"].asBool();
                ShowChunks = f.RootTag["ShowChunks"].asBool();
                ShowMapIcons = f.RootTag["ShowMapIcons"].asBool();
                ShowWaterDepth = f.RootTag["ShowWaterDepth"].asBool();
                f.Dispose();
            }
        }

        public static void Save()
        {
            File.WriteAllLines(".luf", LastUsedFiles.ToArray());


            NbtFile f = new NbtFile();
            f.RootTag.Add("GridLines", ShowGridLines);
            f.RootTag.Add("ShowChunks", ShowChunks);
            f.RootTag.Add("ShowMapIcons", ShowMapIcons); 
            f.RootTag.Add("ShowWaterDepth", ShowWaterDepth);
            f.SaveFile(".settings");
            f.Dispose();
        }

        internal static void SetLUF(string FileName)
        {
            Stack<string> luf = new Stack<string>();
            foreach (string f in LastUsedFiles)
            {
                if (f != FileName && luf.Count<10)
                    luf.Push(f);
            }
            luf.Push(FileName);
            LastUsedFiles = luf;

            Save();
        }
    }
}
