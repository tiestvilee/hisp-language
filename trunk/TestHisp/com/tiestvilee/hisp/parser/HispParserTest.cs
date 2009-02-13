using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using antlr;
using NUnit.Framework;

namespace com.tiestvilee.hisp.parser
{
    [TestFixture]
    public class HispParserTest
    {
        [Test]
        public void ANTLRParsesSimpleString()
        {
            assertThatTreeMatchesParsing(
                "< hello goodbye >",
                " ( hello goodbye )");
        }

        [Test]
        public void ANTLRParsesNestedString()
        {
            assertThatTreeMatchesParsing(
                "< hello goodbye\n <matt\n sux> >", 
                " ( hello goodbye ( matt sux ) )");
        }

        [Test]
        public void ANTLRParsesStringWithOtherElements()
        {
            assertThatTreeMatchesParsing(
                "<html < head > <body #abc .abc>>",
                " ( html head ( body ( # abc ) ( . abc ) ) )");
        }

        [Test]
        public void ANTLRParsesStringWithAttributes()
        {
            assertThatTreeMatchesParsing(
                "<html <body @john=\"mary is hairy\" >>",
                " ( html ( body ( @ john \"mary is hairy\" ) ) )");

            assertThatTreeMatchesParsing(
                "<html <body @john=mary >>",
                " ( html ( body ( @ john mary ) ) )");
        }

        [Test]
        public void ANTLRParsesStringWithMultipleSpansAtSameLevel()
        {
            assertThatTreeMatchesParsing(
                "<html <body<span><span>>>",
                " ( html ( body span span ) )");
        }

        private void assertThatTreeMatchesParsing(string testString, string expectedTree)
        {
            HispLexer lexer = new HispLexer(new System.IO.StringReader(testString));
            HispParser parser = new HispParser(lexer);
            parser.expression();
            CommonAST t = (CommonAST)parser.getAST();
            Assert.AreEqual(expectedTree, t.ToStringTree());
        }
    }
}
