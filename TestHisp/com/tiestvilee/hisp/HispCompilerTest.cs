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
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());
        }

        [Test]
        public void CanCompileTwoLevelTreeIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head>>");
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            ListNode head = (ListNode) html[1];
            Assert.AreEqual("head", head[0].GetText());
        }

        [Test]
        public void CanCompileSimpleStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head <title>> <body <h1>>>");
            ListNode html = hisp.Root;
            AssertBasicStructureOK(html);

        }

        [Test]
        public void CanCompileStringWithDefaultAttributesIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(@"<html <head <title>> <body <h1 #top-heading .heading .red> #body1>>");
            ListNode html = hisp.Root;
            AssertBasicStructureOK(html);

            IdNode headingId = (IdNode)html[2][1][1];
            Assert.AreEqual(headingId.GetText(), "top-heading");

            ClassNode headingClass = (ClassNode)html[2][1][2];
            Assert.AreEqual(headingClass.GetText(), "heading");

            ClassNode redClass = (ClassNode)html[2][1][3];
            Assert.AreEqual(redClass.GetText(), "red");

            IdNode bodyId = (IdNode)html[2][2];
            Assert.AreEqual(bodyId.GetText(), "body1");
        }

        private void AssertBasicStructureOK(ListNode html)
        {
            Assert.AreEqual("html", html[0].GetText());

            ListNode head = (ListNode)html[1];
            Assert.AreEqual("head", head[0].GetText());

            ListNode title = (ListNode)head[1];
            Assert.AreEqual("title", title[0].GetText());

            ListNode body = (ListNode)html[2];
            Assert.AreEqual("body", body[0].GetText());

            ListNode h1 = (ListNode)body[1];
            Assert.AreEqual("h1", h1[0].GetText());
        }

        [Test]
        public void CanCompileSimpleBracketlessStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title");
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            ListNode head = (ListNode)html[1];
            Assert.AreEqual("head", head[0].GetText());

            ListNode title = (ListNode)head[1];
            Assert.AreEqual("title", title[0].GetText());

        }

        [Test]
        public void CanCompileBracketlessStringWithIdIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    #abc");
            ListNode html = hisp.Root; 
            Assert.AreEqual("html", html[0].GetText());

            IdNode id = (IdNode)html[1];
            Assert.AreEqual("abc", id.GetText());

        }

        [Test]
        public void CanCompileBracketlessStringWithClassIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    .abc");
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            ClassNode classNode = (ClassNode)html[1];
            Assert.AreEqual("abc", classNode.GetText());
        }

        [Test]
        public void CanCompileBracketlessStringWithArbitraryAttributeIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    @abc=xyz");
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            AttributeNode attributeNode = (AttributeNode)html[1];
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
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            StringNode attributeNode = (StringNode)html[1];
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
            ListNode html = hisp.Root;
            Assert.AreEqual("html", html[0].GetText());

            ListNode body = (ListNode)html[2];
            Assert.AreEqual("body", body[0].GetText());

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
            ListNode html = hisp.Root;
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
            ListNode html = hisp.Root;
            Assert.AreEqual("body", html[2][0].GetText());
            Assert.AreEqual("<html <head <title <somethingelse >>><body >>", html.Describe(null));
        }

        [Test]
        public void CanCompileCondStatementIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    if <eq a b>
        <true-case
        <false-case");
            ListNode html = hisp.Root;
            ListNode ifNode = (ListNode)html[1];
            Assert.AreEqual("if", ifNode[0].GetText());

            ListNode condition = (ListNode) ifNode[1];
            Assert.AreEqual("eq", condition[0].GetText());
            Assert.AreEqual("a", condition[1].GetText());
            Assert.AreEqual("b", condition[2].GetText());
            Assert.AreEqual("true-case", ifNode[2][0].GetText());
            Assert.AreEqual("false-case", ifNode[3][0].GetText());
        }


        [Test]
        public void CanCompileBracketlessStringWithDeepNestingOfMethodCallsIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    <<<anobject method1 param1> method2 param2a param2b> method3>");
            ListNode html = hisp.Root;

            ListNode outerMethodCall = (ListNode)html[1];
            ListNode middleMethodCall = (ListNode)outerMethodCall[0];
            ListNode innerMethodCall = (ListNode)middleMethodCall[0];

            Assert.AreEqual("<<LIST>>", outerMethodCall.GetText());

            Assert.AreEqual("<<LIST>>", middleMethodCall.GetText());

            Assert.AreEqual("<<LIST>>", innerMethodCall.GetText());

            Assert.AreEqual("anobject", innerMethodCall[0].GetText());
            Assert.AreEqual("method1", innerMethodCall[1].GetText());
            Assert.AreEqual("param1", innerMethodCall[2].GetText());

            Assert.AreEqual("method2", middleMethodCall[1].GetText());
            Assert.AreEqual("param2a", middleMethodCall[2].GetText());
            Assert.AreEqual("param2b", middleMethodCall[3].GetText());

            Assert.AreEqual("method3", outerMethodCall[1].GetText());
        }
    }

}
