using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using antlr;
using NUnit.Framework;

namespace com.tiestvilee.hisp.parser
{
    [TestFixture]
    public class HispLexerTest
    {
        public const int EOF = HispLexerTokenTypes.EOF;
        public const int NULL_TREE_LOOKAHEAD = HispLexerTokenTypes.NULL_TREE_LOOKAHEAD;
        public const int UNQUOTED_STRING = HispLexerTokenTypes.UNQUOTED_STRING;
        public const int STRING = HispLexerTokenTypes.STRING;
        public const int LPAREN = HispLexerTokenTypes.LPAREN;
        public const int RPAREN = HispLexerTokenTypes.RPAREN;
        public const int CLASS = HispLexerTokenTypes.CLASS;
        public const int HASH = HispLexerTokenTypes.HASH;
        public const int ATTRIBUTE = HispLexerTokenTypes.ATTRIBUTE;
        public const int EQUALS = HispLexerTokenTypes.EQUALS;
        public const int NEWLINE = HispLexerTokenTypes.NEWLINE;
        public const int WHITESPACE = HispLexerTokenTypes.WHITESPACE;

        [Test]
        public void ANTLRParsesSimpleString()
        {
            assertThatTreeMatchesParsing(
                "< hello goodbye >",
                new int[] {LPAREN, WHITESPACE, UNQUOTED_STRING, WHITESPACE, UNQUOTED_STRING, WHITESPACE, RPAREN, EOF},
                new string[] {null, null, "hello", null, "goodbye", null, null, null});
        }

        [Test]
        public void ANTLRParsesNestedString()
        {
            assertThatTreeMatchesParsing(
                "< hello goodbye\n <matt\n sux> >",
                new int[] {    LPAREN, WHITESPACE, UNQUOTED_STRING, WHITESPACE, UNQUOTED_STRING, NEWLINE, WHITESPACE, LPAREN, UNQUOTED_STRING, NEWLINE, WHITESPACE, UNQUOTED_STRING, RPAREN, WHITESPACE, RPAREN, EOF },
                new string[] { null,   null,       "hello",         null,       "goodbye",       null,    null,       null,   "matt",          null,    null,       "sux",           null,   null,       null,   null });
        }

        [Test]
        public void ANTLRParsesStringWithOtherElements()
        {
            assertThatTreeMatchesParsing(
                "<html < head > <body #abc .abc>>",
                new int[] {   LPAREN, UNQUOTED_STRING, WHITESPACE, LPAREN, WHITESPACE, UNQUOTED_STRING, WHITESPACE, RPAREN, WHITESPACE, LPAREN, UNQUOTED_STRING, WHITESPACE, HASH, UNQUOTED_STRING, WHITESPACE, CLASS, UNQUOTED_STRING, RPAREN, RPAREN, EOF },
                new string[] { null,  "html",          null,       null,   null,       "head",          null,       null,   null,       null,   "body",          null,       null, "abc",           null,       null,  "abc",           null,   null,   null });
        }

//        [Test]
//        public void ANTLRParsesStringWithNestedOtherElements()
//        {
//            assertThatTreeMatchesParsing(
//                "<html < head > <body <#abc> <.abc>>>",
//                " ( html head ( body ( # abc ) ( . abc ) ) )");
//        }
//
//        [Test]
//        public void ANTLRParsesStringWithAttributes()
//        {
//            assertThatTreeMatchesParsing(
//                "<html <body @john=\"mary is hairy\" >>",
//                " ( html ( body ( @ john \"mary is hairy\" ) ) )");
//
//            assertThatTreeMatchesParsing(
//                "<html <body @john=mary >>",
//                " ( html ( body ( @ john mary ) ) )");
//        }
//
//        [Test]
//        public void ANTLRParsesStringWithMultipleSpansAtSameLevel()
//        {
//            assertThatTreeMatchesParsing(
//                "<html <body<span><span>>>",
//                " ( html ( body span span ) )");
//        }

        private void assertThatTreeMatchesParsing(string testString, int[] expectedTokenTypes, string[] expectedText)
        {
            HispLexer lexer = new HispLexer(new System.IO.StringReader(testString));

            IToken token;
            int i = 0; 
            while( (token = lexer.nextToken()).Type != EOF)
            {
                Assert.AreEqual(expectedTokenTypes[i], token.Type);
                if(expectedText[i] != null)
                    Assert.AreEqual(expectedText[i], token.getText());
                i++;
            }
        }
    }
}
