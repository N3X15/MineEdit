using LibNbt.Queries;
using LibNbt.Tags;
using NUnit.Framework;

namespace LibNbt.Test.Queries
{
    [TestFixture]
    public class TagQueryTest
    {
        private NbtFile _file;

        [TestFixtureSetUp]
        public void TagQueryTestSetUp()
        {
            _file = new NbtFile();
            _file.LoadFile("TestFiles/bigtest.nbt.gz");
        }

        [Test]
        public void Tokenization()
        {
            var query = new TagQuery();
            TagQueryToken token;

            // Expected: first
            query = new TagQuery("/first");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Next();
            Assert.IsNull(token);
            Assert.AreEqual(0, query.TokensLeft());

            // Expected: first
            query.SetQuery("/first");
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Next();
            Assert.IsNull(token);
            Assert.AreEqual(0, query.TokensLeft());

            // Expected: first, second, third
            query.SetQuery("/first/second/third");
            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(2, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("second", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(1, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("third", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Next();
            Assert.IsNull(token);
            Assert.AreEqual(0, query.TokensLeft());

            // Expected: first/slash, second\backslash, third!, extended/ÅÄÖ
            query.SetQuery(@"/first\/slash/second\\backslash/third!/extended\/ÅÄÖ");
            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(4, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first/slash", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(3, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual(@"second\backslash", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(2, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("third!", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(1, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("extended/ÅÄÖ", token.Name);
            Assert.AreSame(query, token.Query);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Next();
            Assert.IsNull(token);
            Assert.AreEqual(0, query.TokensLeft());
        }

        [Test]
        public void MovingBetweenTokens()
        {
            var query = new TagQuery();
            TagQueryToken token;

            query.SetQuery("/first/second/third/fourth/fifth/sixth");
            Assert.AreEqual(6, query.Count());
            Assert.AreEqual(6, query.TokensLeft());
            token = query.Previous();
            Assert.IsNull(token);
            Assert.AreEqual(6, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first", token.Name);
            Assert.AreEqual(5, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("second", token.Name);
            Assert.AreEqual(4, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("third", token.Name);
            Assert.AreEqual(3, query.TokensLeft());
            token = query.Previous();
            Assert.AreEqual("second", token.Name);
            Assert.AreEqual(4, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("third", token.Name);
            Assert.AreEqual(3, query.TokensLeft());
            token = query.Peek();
            Assert.AreEqual("fourth", token.Name);
            Assert.AreEqual(3, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("fourth", token.Name);
            Assert.AreEqual(2, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("fifth", token.Name);
            Assert.AreEqual(1, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("sixth", token.Name);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Next();
            Assert.IsNull(token);
            Assert.AreEqual(0, query.TokensLeft());
            token = query.Previous();
            Assert.AreEqual("fifth", token.Name);
            Assert.AreEqual(1, query.TokensLeft());
            query.MoveFirst();
            Assert.AreEqual(6, query.TokensLeft());
            token = query.Next();
            Assert.AreEqual("first", token.Name);
            Assert.AreEqual(5, query.TokensLeft());
        }

        [Test]
        public void Querying()
        {
            // To see the structure of this file take a look at TestFiles/bigtest.nbt.gz

            var query = new TagQuery();
            NbtTag tag;

            // Try to get the root node
            tag = _file.Query("/Level");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtCompound>(tag);
            Assert.AreEqual("Level", tag.Name);

            tag = _file.Query("/Level/longTest");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtLong>(tag);
            Assert.AreEqual("longTest", tag.Name);

            tag = _file.Query("/Level/shortTest");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtShort>(tag);
            Assert.AreEqual("shortTest", tag.Name);

            tag = _file.Query("/Level/stringTest");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtString>(tag);
            Assert.AreEqual("stringTest", tag.Name);

            tag = _file.Query("/Level/floatTest");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtFloat>(tag);
            Assert.AreEqual("floatTest", tag.Name);

            tag = _file.Query("/Level/intTest");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtInt>(tag);
            Assert.AreEqual("intTest", tag.Name);

            tag = _file.Query("/Level/nested compound test");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtCompound>(tag);
            Assert.AreEqual("nested compound test", tag.Name);

            tag = _file.Query("/Level/nested compound test/ham/name");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtString>(tag);
            Assert.AreEqual("name", tag.Name);

            tag = _file.Query("/Level/nested compound test/egg/name");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtString>(tag);
            Assert.AreEqual("name", tag.Name);

            tag = _file.Query("/Level/listTest (long)/2");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtLong>(tag);
            Assert.AreEqual(13, ((NbtLong) tag).Value);

            tag = _file.Query("/Level/listTest (compound)/1/name");
            Assert.IsNotNull(tag);
            Assert.IsInstanceOf<NbtString>(tag);
            Assert.AreEqual("Compound tag #1", ((NbtString) tag).Value);
        }
    }
}
