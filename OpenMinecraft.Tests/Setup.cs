using System;
using System.IO;
using NUnit.Framework;

namespace OpenMinecraft.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [SetUp]
        public void DoSetup()
        {
            mOldCwd = Environment.CurrentDirectory;
            
            Environment.CurrentDirectory = mNewCwd=Path.Combine(mOldCwd, "TESTS");
            Console.WriteLine("cd " + Environment.CurrentDirectory);

            File.Copy(Path.Combine(mOldCwd, "blocks.txt"), "blocks.txt", true);

            Directory.CreateDirectory("mobs");
            Directory.CreateDirectory("blocks");
            Directory.CreateDirectory("items");

            Utils.CopyFlat(Path.Combine(mOldCwd, "mobs"), "mobs");
            Utils.CopyFlat(Path.Combine(mOldCwd, "items"), "items");
            Utils.CopyFlat(Path.Combine(mOldCwd, "blocks"), "blocks");

            Blocks.Init();
            OpenMinecraft.Entities.Entity.LoadEntityTypes();
            OpenMinecraft.TileEntities.TileEntity.LoadEntityTypes();
        }

        [TearDown]
        public void Teardown()
        {
            Blocks.Clear();
            OpenMinecraft.Entities.Entity.Cleanup();
            OpenMinecraft.TileEntities.TileEntity.Cleanup();

            Directory.Delete("mobs",true);
            Directory.Delete("items",true);
            Directory.Delete("blocks",true);

            Environment.CurrentDirectory = mOldCwd;
            Console.WriteLine("cd " + Environment.CurrentDirectory);
        }
        public static string mOldCwd { get; set; }
        public static string mNewCwd { get; set; }
    }
}
