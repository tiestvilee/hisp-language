using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace com.tiestvilee.hisp
{
    [TestFixture]
    public class BasicHispXmlTest
    {
        [Test]
        public void TestTags ()
        {
            AssertThatHispRendersXmlCorrectly("testfiles\\test1.hisp", "testfiles\\test1.xml", new Dictionary<string, object>());
        }

        [Test]
        public void TestDefaultAttributes()
        {
            AssertThatHispRendersXmlCorrectly("testfiles\\test2.hisp", "testfiles\\test2.xml", new Dictionary<string, object>());
        }

        [Test]
        public void TestCustomAttributes()
        {
            AssertThatHispRendersXmlCorrectly("testfiles\\test3.hisp", "testfiles\\test3.xml", new Dictionary<string, object>());
        }

        [Test]
        public void TestLayoutChanges()
        {
            AssertThatHispRendersXmlCorrectly("testfiles\\test4.hisp", "testfiles\\test4.xml", new Dictionary<string, object>());
        }

        [Test]
        public void TestBasicVariables()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["astring"] = "this is a string";
            context["anumber"] = 34.98;
            context["aclass"] = new ClassNode("red");
            context["anobject"] = new DummyObject();
            AssertThatHispRendersXmlCorrectly("testfiles\\test5.hisp", "testfiles\\test5.xml", context);
        }

        [Test]
        public void TestProperties()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["anobject"] = new DummyObject();
            AssertThatHispRendersXmlCorrectly("testfiles\\test6.hisp", "testfiles\\test6.xml", context);
        }

//        [Test]
//        public void TestNestedGets()
//        {
//            Dictionary<string, object> context = new Dictionary<string, object>();
//            context["anobject"] = new DummyObject();
//            HispCompiler compiler = new HispCompiler();
//            Hisp hisp = compiler.compile("html\r\n    <<<anobject Itself> Itself> Itself>");
//            Assert.AreEqual("<html>\r\n  ToString called for object\r\n</html>\r\n", hisp.ToHtml(context));
//        }
//
//        [Test]
//        public void TestNestedGetsReturningClass()
//        {
//            Dictionary<string, object> context = new Dictionary<string, object>();
//            context["anobject"] = new DummyObject();
//            HispCompiler compiler = new HispCompiler();
//            Hisp hisp = compiler.compile("html\r\n    <<<anobject Itself> Itself> AddClass>");
//            Assert.AreEqual("<html class=\"red\"/>\r\n", hisp.ToHtml(context));
//        }
//
//        [Test]
//        public void TestEq()
//        {
//            Dictionary<string, object> context = new Dictionary<string, object>();
//            HispCompiler compiler = new HispCompiler();
//            Hisp hisp = compiler.compile("html\r\n    <eq \"a\" \"a\">");
//            Assert.AreEqual("<html>\r\n  true\r\n</html>", hisp.ToHtml(context));
//        }
//
//        [Test]
//        public void TestCondReturningClass()
//        {
//            Dictionary<string, object> context = new Dictionary<string, object>();
//            HispCompiler compiler = new HispCompiler();
//            Hisp hisp = compiler.compile(
//@"html
//  cond
//    <eq ""a"" ""b""> <.class>");
//            Console.WriteLine(hisp.ToHtml(context));
//            Assert.AreEqual("<html>\r\n  <body class=\"class\"/>\r\n</html>", hisp.ToHtml(context));
//        }

        public class DummyObject
        {
            public override string ToString()
            {
                return "ToString called for object";
            }

            public string Property { get{return "a property";}}
            public DummyObject Itself { get { return this; } }

            public string GetAccessor()
            {
                return "an accessor";
            }

            public object AddClass()
            {
                return new ClassNode("red");
            }
        }

        private void AssertThatHispRendersXmlCorrectly(string hispFile, string resultFile, Dictionary<string, object> context)
        {
            string hispString = new System.IO.StreamReader(hispFile).ReadToEnd();
            string resultString = new System.IO.StreamReader(resultFile).ReadToEnd();

            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile(hispString);
            XmlDocument actual = hisp.ToXml(context);
            Console.WriteLine(actual.OuterXml);
            Assert.AreEqual(resultString, actual.OuterXml);
        }
    }
}
