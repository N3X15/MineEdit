using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using LibNbt.Tags;

namespace LibNbt
{
    public class NbtFile : IDisposable
    {
        public NbtCompound RootTag { get; protected set; }
        protected string LoadedFile { get; set; }

        public NbtFile() 
        {
            LoadedFile = "";
        }
        public NbtFile(string fileName)
        {
            LoadedFile = fileName;
        }

        public virtual void LoadFile() { LoadFile(LoadedFile); }

        public virtual void LoadFile(string fileName)
        {
            if (LoadedFile != fileName) LoadedFile = fileName;

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(string.Format("Could not find NBT file: {0}", fileName), fileName);
            }

            using (FileStream readFileStream = File.OpenRead(fileName))
            {
                using (GZipStream decStream = new GZipStream(readFileStream, CompressionMode.Decompress))
                {
                    using (MemoryStream memStream = new MemoryStream((int)readFileStream.Length))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = decStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memStream.Write(buffer, 0, bytesRead);
                        }

                        // Make sure the stream is at the beginning
                        memStream.Seek(0, SeekOrigin.Begin);

                        // Make sure the first byte in this file is the tag for a TAG_Compound
                        if (memStream.ReadByte() == (int)NbtTagType.TAG_Compound)
                        {
                            NbtCompound rootCompound = new NbtCompound();
                            try
                            {
                                rootCompound.ReadTag(memStream);
                                rootCompound.Path = "";
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            RootTag = rootCompound;
                        }
                        else
                        {
                            throw new InvalidDataException("File format does not start with a TAG_Compound");
                        }
                    }
                }
            }

            //ls();
        }

        public virtual void SaveFile(string fileName)
        {
            using (MemoryStream saveStream = new MemoryStream())
            {
                if (RootTag != null)
                {
                    RootTag.WriteTag(saveStream);

                    saveStream.Seek(0, SeekOrigin.Begin);
                    using (FileStream saveFile = File.OpenWrite(fileName))
                    {
                        using (GZipStream compressStream = new GZipStream(saveFile, CompressionMode.Compress))
                        {
                            byte[] buffer = new byte[4096];
                            int amtSaved;
                            while ((amtSaved = saveStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                compressStream.Write(buffer, 0, amtSaved);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get item by path (/Data/Player/Pos, for example)
        /// </summary>
        /// <param name="path">Path to item</param>
        /// <returns>Item or null</returns>
        public NbtTag GetTag(string path)
        {
            Console.WriteLine("Reading tag " + path);
            NbtTag ctag = RootTag;
            string[] p = path.Split(new char[] { '/','\\' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < p.Length; i++)
            {
                if (ctag is NbtCompound)
                {
                    NbtCompound fff = (NbtCompound)ctag;
                    NbtTag ntag = fff.Tags.Find(delegate(NbtTag t)
                    {
                        return (t.Name == p[i]);
                    });
                    if (ntag == null)
                    {
                        Console.WriteLine("[LibNBT] GetTag(): {0}: can't find tag by the name of {1}", fff.Path, p[i]);
                        return null;
                    }
                    ctag = ntag;
                }
                else if (ctag is NbtList)
                {
                    NbtList l = (NbtList)ctag;
                    int ii = int.Parse(p[i]);
                    try
                    {
                        ctag = l.Tags[ii];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return null;
                    }
                }
            }
            return ctag;
        }

        public void ls(string path="/")
        {
            NbtTag hurr = GetTag(path);
            if (hurr is NbtCompound)
            {
                NbtCompound c = (NbtCompound)hurr;
                foreach (NbtTag t in c.Tags)
                {
                    if (t is NbtCompound || t is NbtList)
                        ls(Path.Combine(path, t.Name));
                    else
                    {
                        Console.WriteLine("{0} - {1}", Path.Combine(path, t.Name), t.GetType().Name);
                    }
                }
            }
            if (hurr is NbtList)
            {
                NbtList c = (NbtList)hurr;
                int i = 0;
                foreach (NbtTag t in c.Tags)
                {
                    if (t is NbtCompound || t is NbtList)
                        ls(Path.Combine(path, t.Name));
                    else
                    {
                        Console.WriteLine("{0} - {1}", Path.Combine(path, i.ToString()), t.GetType().Name);
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// This needs to be more elegant.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void SetTag(string path,object t)
        {
            Console.WriteLine("Writing {0} to tag {1}",path,t);
            RootTag.SaveData(path, t);
        }

        public void Dispose()
        {
            
        }
    }
}
