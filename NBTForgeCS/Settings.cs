using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MineEdit
{
    class Settings
    {
        // Rendering info.
        public static bool ShowGridLines = false;
        public static bool ShowChunks = true;
        public static bool ShowSpawnZone = true;
        public static float MiniMapScale = 0.75f;
        public static Stack<string> LastUsedFiles = new Stack<string>();
        public static void Init()
        {
            if (File.Exists(".luf"))
            {
                foreach (string f in File.ReadAllLines(".luf"))
                {
                    LastUsedFiles.Push(f);
                }
            }
        }

        public static void Save()
        {
            File.WriteAllLines(".luf", LastUsedFiles.ToArray());
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
