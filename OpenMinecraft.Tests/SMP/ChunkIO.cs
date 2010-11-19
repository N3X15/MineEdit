using NUnit.Framework;
using System;
using System.IO;
using OpenMinecraft;

namespace OpenMinecraft.Tests
{
    [TestFixture]
    public class IOTest
    {
        [SetUp]
        public void DoSetup()
        {
            Environment.CurrentDirectory = Setup.mNewCwd;
        }

        [TearDown]
        public void Teardown()
        {
            Environment.CurrentDirectory = Setup.mOldCwd;
        }

        // Test loading a known-good level.dat
        [Test]
        public void TEST001_ReadMap()
        {

            if (Directory.Exists("001"))
                Directory.Delete("001", true);
            Directory.CreateDirectory("001"); // Copies map here.
            Utils.CopyRecursive("001_PRISTINE", "001");

            InfdevHandler mh = new InfdevHandler();
            mh.Load("001/level.dat");
            mh.SetDimension(0);
            
            //Loading 3 entities in chunk 14,8 (C:\Users\Rob\AppData\Roaming\.minecraft\saves\World5\e\8\c.e.8.dat):
            Chunk a = mh.GetChunk(14, 8);
            Assert.AreEqual(3, a.Entities.Count);
        }

        // Test saving a chunk
        [Test]
        public void TEST002_SaveMap()
        {

            if (Directory.Exists("002"))
                Directory.Delete("002", true);
            Directory.CreateDirectory("002"); // Saves map here.

            InfdevHandler mh = new InfdevHandler();
            mh.Save("002/level.dat");
            mh.SetDimension(0);
            
            Chunk cnkA = mh.NewChunk(0, 0);
            cnkA.Blocks[0, 0, 0] = 0x01;
            cnkA.Blocks[0, 0, 1] = 0x02;
            cnkA.Blocks[0, 0, 2] = 0x03;
            cnkA.Blocks[0, 0, 3] = 0x04;
            cnkA.Save();

            FileInfo fA = new FileInfo(cnkA.Filename);

            Assert.Greater(0, fA.Length, "System writing zero-length chunks.");

            Chunk cnkB = mh.GetChunk(0, 0);
            Assert.AreEqual(cnkB.Blocks[0, 0, 0], 0x01);
            Assert.AreEqual(cnkB.Blocks[0, 0, 1], 0x02);
            Assert.AreEqual(cnkB.Blocks[0, 0, 2], 0x03);
            Assert.AreEqual(cnkB.Blocks[0, 0, 3], 0x04);

        }
    }
}