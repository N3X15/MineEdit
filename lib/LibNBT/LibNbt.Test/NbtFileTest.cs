using System;
using System.IO;
using LibNbt.Tags;
using NUnit.Framework;

namespace LibNbt.Test
{
    [TestFixture]
    public class NbtFileTest
    {
        [SetUp]
        public void NbtFileTestSetup()
        {
            Directory.CreateDirectory("TestTemp");
        }

        [TearDown]
        public void NbtFileTestTearDown()
        {
            if (Directory.Exists("TestTemp"))
            {
                foreach (var file in Directory.GetFiles("TestTemp"))
                {
                    File.Delete(file);
                }
                Directory.Delete("TestTemp");
            }
        }

        #region Loading Small Nbt Test File
        [Test]
        public void TestNbtSmallFileLoading()
        {
            var file = new NbtFile();
            file.LoadFile("TestFiles/test.nbt.gz");

            AssertNbtSmallFile(file);
        }
        [Test]
        public void TestNbtSmallFileLoadingUncompressed()
        {
            var file = new NbtFile();
            file.LoadFile("TestFiles/test.nbt", false);

            AssertNbtSmallFile(file);
        }
        private void AssertNbtSmallFile(NbtFile file)
        {
            // See TestFiles/test.nbt.txt to see the expected format
            Assert.IsInstanceOf<NbtCompound>(file.RootTag);

            NbtCompound root = file.RootTag;
            Assert.AreEqual("hello world", root.Name);
            Assert.AreEqual(1, root.Tags.Count);

            Assert.IsInstanceOf<NbtString>(root[0]);
            Assert.IsInstanceOf<NbtString>(root["name"]);

            var node = (NbtString)root[0];
            Assert.AreEqual("name", node.Name);
            Assert.AreEqual("Bananrama", node.Value);
        }
        #endregion

        #region Loading Big Nbt Test File
        [Test]
        public void TestNbtBigFileLoading()
        {
            var file = new NbtFile();
            file.LoadFile("TestFiles/bigtest.nbt.gz");

            AssertNbtBigFile(file);
        }
        [Test]
        public void TestnbtBigFileLoadingUncompressed()
        {
            var file = new NbtFile();
            file.LoadFile("TestFiles/bigtest.nbt", false);

            AssertNbtBigFile(file);
        }
        private void AssertNbtBigFile(NbtFile file)
        {
            // See TestFiles/bigtest.nbt.txt to see the expected format
            Assert.IsInstanceOf<NbtCompound>(file.RootTag);

            NbtCompound root = file.RootTag;
            Assert.AreEqual("Level", root.Name);
            Assert.AreEqual(11, root.Tags.Count);

            NbtTag node;
            Assert.IsInstanceOf<NbtLong>(root[0]);
            Assert.IsInstanceOf<NbtLong>(root["longTest"]);
            node = root[0];
            Assert.AreEqual("longTest", node.Name);
            Assert.AreEqual(9223372036854775807, ((NbtLong)node).Value);

            Assert.IsInstanceOf<NbtShort>(root[1]);
            Assert.IsInstanceOf<NbtShort>(root["shortTest"]);
            node = root[1];
            Assert.AreEqual("shortTest", node.Name);
            Assert.AreEqual(32767, ((NbtShort)node).Value);

            Assert.IsInstanceOf<NbtString>(root[2]);
            Assert.IsInstanceOf<NbtString>(root["stringTest"]);
            node = root[2];
            Assert.AreEqual("stringTest", node.Name);
            Assert.AreEqual("HELLO WORLD THIS IS A TEST STRING ÅÄÖ!", ((NbtString)node).Value);

            Assert.IsInstanceOf<NbtFloat>(root[3]);
            Assert.IsInstanceOf<NbtFloat>(root["floatTest"]);
            node = root[3];
            Assert.AreEqual("floatTest", node.Name);
            Assert.AreEqual(0.49823147f, ((NbtFloat)node).Value);

            Assert.IsInstanceOf<NbtInt>(root[4]);
            Assert.IsInstanceOf<NbtInt>(root["intTest"]);
            node = root[4];
            Assert.AreEqual("intTest", node.Name);
            Assert.AreEqual(2147483647, ((NbtInt)node).Value);

            Assert.IsInstanceOf<NbtCompound>(root[5]);
            Assert.IsInstanceOf<NbtCompound>(root["nested compound test"]);
            node = root[5];
            Assert.AreEqual("nested compound test", node.Name);
            Assert.AreEqual(2, ((NbtCompound)node).Tags.Count);

            // First nested test
            NbtCompound subNode;
            Assert.IsInstanceOf<NbtCompound>(((NbtCompound)node)[0]);
            Assert.IsInstanceOf<NbtCompound>(((NbtCompound)node)["ham"]);
            subNode = (NbtCompound)((NbtCompound)node)[0];
            Assert.AreEqual("ham", subNode.Name);
            Assert.AreEqual(2, subNode.Tags.Count);

            // Checking sub node values
            Assert.IsInstanceOf<NbtString>(subNode[0]);
            Assert.IsInstanceOf<NbtString>(subNode["name"]);
            Assert.AreEqual("name", subNode[0].Name);
            Assert.AreEqual("Hampus", ((NbtString)subNode[0]).Value);

            Assert.IsInstanceOf<NbtFloat>(subNode[1]);
            Assert.IsInstanceOf<NbtFloat>(subNode["value"]);
            Assert.AreEqual("value", subNode[1].Name);
            Assert.AreEqual(0.75, ((NbtFloat)subNode[1]).Value);
            // End sub node

            // Second nested test
            Assert.IsInstanceOf<NbtCompound>(((NbtCompound)node)[1]);
            Assert.IsInstanceOf<NbtCompound>(((NbtCompound)node)["egg"]);
            subNode = (NbtCompound)((NbtCompound)node)[1];
            Assert.AreEqual("egg", subNode.Name);
            Assert.AreEqual(2, subNode.Tags.Count);

            // Checking sub node values
            Assert.IsInstanceOf<NbtString>(subNode[0]);
            Assert.IsInstanceOf<NbtString>(subNode["name"]);
            Assert.AreEqual("name", subNode[0].Name);
            Assert.AreEqual("Eggbert", ((NbtString)subNode[0]).Value);

            Assert.IsInstanceOf<NbtFloat>(subNode[1]);
            Assert.IsInstanceOf<NbtFloat>(subNode["value"]);
            Assert.AreEqual("value", subNode[1].Name);
            Assert.AreEqual(0.5, ((NbtFloat)subNode[1]).Value);
            // End sub node

            Assert.IsInstanceOf<NbtList>(root[6]);
            Assert.IsInstanceOf<NbtList>(root["listTest (long)"]);
            node = root[6];
            Assert.AreEqual("listTest (long)", node.Name);
            Assert.AreEqual(5, ((NbtList)node).Tags.Count);

            // The values should be: 11, 12, 13, 14, 15
            for (int nodeIndex = 0; nodeIndex < ((NbtList)node).Tags.Count; nodeIndex++)
            {
                Assert.IsInstanceOf<NbtLong>(((NbtList)node)[nodeIndex]);
                Assert.AreEqual("", ((NbtList)node)[nodeIndex].Name);
                Assert.AreEqual(nodeIndex + 11, ((NbtLong)((NbtList)node)[nodeIndex]).Value);
            }

            Assert.IsInstanceOf<NbtList>(root[7]);
            Assert.IsInstanceOf<NbtList>(root["listTest (compound)"]);
            node = root[7];
            Assert.AreEqual("listTest (compound)", node.Name);
            Assert.AreEqual(2, ((NbtList)node).Tags.Count);

            // First Sub Node
            Assert.IsInstanceOf<NbtCompound>(((NbtList)node)[0]);
            subNode = (NbtCompound)((NbtList)node)[0];

            // First node in sub node
            Assert.IsInstanceOf<NbtString>(subNode[0]);
            Assert.IsInstanceOf<NbtString>(subNode["name"]);
            Assert.AreEqual("name", subNode[0].Name);
            Assert.AreEqual("Compound tag #0", ((NbtString)subNode[0]).Value);

            // Second node in sub node
            Assert.IsInstanceOf<NbtLong>(subNode[1]);
            Assert.IsInstanceOf<NbtLong>(subNode["created-on"]);
            Assert.AreEqual("created-on", subNode[1].Name);
            Assert.AreEqual(1264099775885, ((NbtLong)subNode[1]).Value);

            // Second Sub Node
            Assert.IsInstanceOf<NbtCompound>(((NbtList)node)[1]);
            subNode = (NbtCompound)((NbtList)node)[1];

            // First node in sub node
            Assert.IsInstanceOf<NbtString>(subNode[0]);
            Assert.IsInstanceOf<NbtString>(subNode["name"]);
            Assert.AreEqual("name", subNode[0].Name);
            Assert.AreEqual("Compound tag #1", ((NbtString)subNode[0]).Value);

            // Second node in sub node
            Assert.IsInstanceOf<NbtLong>(subNode[1]);
            Assert.IsInstanceOf<NbtLong>(subNode["created-on"]);
            Assert.AreEqual("created-on", subNode[1].Name);
            Assert.AreEqual(1264099775885, ((NbtLong)subNode[1]).Value);

            Assert.IsInstanceOf<NbtByte>(root[8]);
            Assert.IsInstanceOf<NbtByte>(root["byteTest"]);
            node = root[8];
            Assert.AreEqual("byteTest", node.Name);
            Assert.AreEqual(127, ((NbtByte)node).Value);

            Assert.IsInstanceOf<NbtByteArray>(root[9]);
            Assert.IsInstanceOf<NbtByteArray>(root["byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))"]);
            node = root[9];
            Assert.AreEqual("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))", node.Name);
            Assert.AreEqual(1000, ((NbtByteArray)node).Value.Length);

            // Values are: the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...)
            for (int n = 0; n < 1000; n++)
            {
                Assert.AreEqual((n * n * 255 + n * 7) % 100, ((NbtByteArray)node)[n]);
            }

            Assert.IsInstanceOf<NbtDouble>(root[10]);
            Assert.IsInstanceOf<NbtDouble>(root["doubleTest"]);
            node = root[10];
            Assert.AreEqual("doubleTest", node.Name);
            Assert.AreEqual(0.4931287132182315, ((NbtDouble)node).Value);
        }
        #endregion

        [Test]
        public void TestNbtSmallFileSavingUncompressed()
        {
            var file = new NbtFile
                           {
                               RootTag = new NbtCompound("hello world", new NbtTag[]
                                                                            {
                                                                                new NbtString("name", "Bananrama")
                                                                            })
                           };

            file.SaveFile("TestTemp/test.nbt", false);

            FileAssert.AreEqual("TestFiles/test.nbt", "TestTemp/test.nbt");
        }
        [Test]
        public void TestNbtSmallFileSavingUncompressedStream()
        {
            var file = new NbtFile
                           {
                               RootTag = new NbtCompound("hello world", new NbtTag[]
                                                                            {
                                                                                new NbtString("name", "Bananrama")
                                                                            })
                           };

            var nbtStream = new MemoryStream();
            file.SaveFile(nbtStream, false);

            FileStream testFileStream = File.OpenRead("TestFiles/test.nbt");

            FileAssert.AreEqual(testFileStream, nbtStream);
        }
    }
}
