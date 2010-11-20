using NUnit.Framework;
using System;
using System.IO;
using OpenMinecraft;
using OpenMinecraft.Entities;

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

            Directory.Delete("001", true);
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

            Assert.Greater(fA.Length, 0, "System writing zero-length chunks.");

            Chunk cnkB = mh.GetChunk(0, 0);
            Assert.AreEqual(cnkB.Blocks[0, 0, 0], 0x01);
            Assert.AreEqual(cnkB.Blocks[0, 0, 1], 0x02);
            Assert.AreEqual(cnkB.Blocks[0, 0, 2], 0x03);
            Assert.AreEqual(cnkB.Blocks[0, 0, 3], 0x04);

            Directory.Delete("002", true);
        }

        // Test changing map settings.
        [Test]
        public void TEST003_EditLevel()
        {

            if (Directory.Exists("003"))
                Directory.Delete("003", true);
            Directory.CreateDirectory("003"); // Copies map here.
            Utils.CopyRecursive("001_PRISTINE", "003");

            {
                InfdevHandler a = new InfdevHandler();
                a.Load("003/level.dat");
                a.SetDimension(0);

                a.Health = 10;

                a.Save();
            }
            InfdevHandler b = new InfdevHandler();
            b.Load("003/level.dat");
            b.SetDimension(0);

            Assert.True(b.Health == 10);

            Directory.Delete("003", true);
        }

        // Test adding entities.
        [Test]
        public void TEST004_AddEntities()
        {

            if (Directory.Exists("004"))
                Directory.Delete("004", true);
            Directory.CreateDirectory("004"); // Copies map here.

            {
                InfdevHandler a = new InfdevHandler();
                a.Save("004/level.dat");
                a.SetDimension(0);

                Sheep b = new Sheep();
                b.Air = 300;
                b.AttackTime = 0;
                b.DeathTime = 0;
                b.FallDistance = 0;
                b.Fire = -1;
                b.Health = 20;
                b.Sheared = false;
                b.UUID = Guid.NewGuid();
                b.Pos = new Vector3d(8, 100, 8);
                b.Rotation = new Rotation(0, 0);

                a.AddEntity(b);

                a.Save();
            }

            {
                InfdevHandler a = new InfdevHandler();
                a.Load("004/level.dat");
                a.SetDimension(0);

                Chunk chunk = a.GetChunk(0, 0,false);
                Assert.IsNotNull(chunk, "Sheep didn't get saved; Chunk didn't even get created.");
                Assert.AreEqual(1,chunk.Entities.Count,"The sheep didn't get saved.");
            }

            Directory.Delete("004", true);
        }
    }
}