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

            Hisp hisp = compiler.compile(@"<html <head title> <body <h1 #top-heading .heading .red> #body1>>");
            TagNode html = (TagNode)hisp.Root;
            AssertBasicStructureOK(html);

            IdNode headingId = (IdNode) html[1][0][0];
            Assert.AreEqual(headingId.getText(), "top-heading");
            ClassNode headingClass = (ClassNode)html[1][0][1];
            Assert.AreEqual(headingClass.getText(), "heading");
            ClassNode redClass = (ClassNode)html[1][0][2];
            Assert.AreEqual(redClass.getText(), "red");
            IdNode bodyId = (IdNode)html[1][1];
            Assert.AreEqual(bodyId.getText(), "body1");
        }

        private void AssertBasicStructureOK(TagNode html)
        {
            Assert.AreEqual("html", html.getText());

            TagNode head = (TagNode)html[0];
            Assert.AreEqual("head", head.getText());

            TagNode title = (TagNode)head[0];
            Assert.AreEqual("title", title.getText());

            TagNode body = (TagNode)html[1];
            Assert.AreEqual("body", body.getText());

            TagNode h1 = (TagNode)body[0];
            Assert.AreEqual("h1", h1.getText());
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
            Assert.AreEqual("html", html.getText());

            TagNode head = (TagNode)html[0];
            Assert.AreEqual("head", head.getText());

            TagNode title = (TagNode)head[0];
            Assert.AreEqual("title", title.getText());

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
            Assert.AreEqual("html", html.getText());

            TagNode body = (TagNode)html[1];
            Assert.AreEqual("body", body.getText());

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


    }

}
