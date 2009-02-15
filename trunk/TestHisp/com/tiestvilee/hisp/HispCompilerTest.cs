using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tiestvilee.hisp;
using NUnit.Framework;

namespace ClassLibrary1.com.tiestvilee.hisp
{
    [TestFixture]
    public class HispCompilerTest
    {
        [Test]
        public void CanCompileSingleExpressionIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html>");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());
        }

        [Test]
        public void CanCompileTwoLevelTreeIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head>>");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            TagNode head = (TagNode) html[0];
            Assert.AreEqual("head", head.GetText());
        }

        [Test]
        public void CanCompileSimpleStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head <title>> <body <h1>>>");
            TagNode html = (TagNode)hisp.Root;
            AssertBasicStructureOK(html);

        }

        [Test]
        public void CanCompileStringWithDefaultAttributesIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head <title>> <body <h1 #top-heading .heading .red> #body1>>");
            TagNode html = (TagNode)hisp.Root;
            AssertBasicStructureOK(html);

            IdNode headingId = (IdNode) html[1][0][0];
            Assert.AreEqual(headingId.GetText(), "top-heading");
            ClassNode headingClass = (ClassNode)html[1][0][1];
            Assert.AreEqual(headingClass.GetText(), "heading");
            ClassNode redClass = (ClassNode)html[1][0][2];
            Assert.AreEqual(redClass.GetText(), "red");
            IdNode bodyId = (IdNode)html[1][1];
            Assert.AreEqual(bodyId.GetText(), "body1");
        }

        private void AssertBasicStructureOK(TagNode html)
        {
            Assert.AreEqual("html", html.GetText());

            TagNode head = (TagNode)html[0];
            Assert.AreEqual("head", head.GetText());

            TagNode title = (TagNode)head[0];
            Assert.AreEqual("title", title.GetText());

            TagNode body = (TagNode)html[1];
            Assert.AreEqual("body", body.GetText());

            TagNode h1 = (TagNode)body[0];
            Assert.AreEqual("h1", h1.GetText());
        }

        [Test]
        public void CanCompileSimpleBracketlessStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            TagNode head = (TagNode)html[0];
            Assert.AreEqual("head", head.GetText());

            TagNode title = (TagNode)head[0];
            Assert.AreEqual("title", title.GetText());

        }

        [Test]
        public void CanCompileBracketlessStringWithIdIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    #abc");
            TagNode html = (TagNode)hisp.Root; 
            Assert.AreEqual("html", html.GetText());

            IdNode id = (IdNode)html[0];
            Assert.AreEqual("abc", id.GetText());

        }

        [Test]
        public void CanCompileBracketlessStringWithClassIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    .abc");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            ClassNode classNode = (ClassNode)html[0];
            Assert.AreEqual("abc", classNode.GetText());
        }

        [Test]
        public void CanCompileBracketlessStringWithArbitraryAttributeIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    @abc=xyz");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            AttributeNode attributeNode = (AttributeNode)html[0];
            Assert.AreEqual("abc", attributeNode.GetText());
            Assert.AreEqual("xyz", attributeNode.GetValue());
        }

        [Test]
        public void CanCompileBracketlessStringWithInternalStringsIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    ""a string""");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            StringNode attributeNode = (StringNode)html[0];
            Assert.AreEqual("a string", attributeNode.GetText());
        }

        [Test]
        public void CanCompileSimpleBracketlessStringWithOneBackstepIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title
    body");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("html", html.GetText());

            TagNode body = (TagNode)html[1];
            Assert.AreEqual("body", body.GetText());

        }

        [Test]
        public void CanCompileBracketlessStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title
    body
        h1");
            TagNode html = (TagNode) hisp.Root;
            AssertBasicStructureOK(html);
        }


        [Test]
        public void CanCompileBracketlessStringWithLotsOfBacksteppingIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title
            somethingelse
    body");
            TagNode html = (TagNode)hisp.Root;
            Assert.AreEqual("body", html[1].GetText());
        }

    }

}
