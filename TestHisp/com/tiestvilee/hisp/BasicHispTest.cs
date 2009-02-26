using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace com.tiestvilee.hisp
{
    [TestFixture]
    public class BasicHispTest
    {
        [Test]
        public void TestTags ()
        {
            AssertThatHispRendersCorrectly("testfiles\\test1.hisp", "testfiles\\test1.html", new Dictionary<string, object>());
        }

        [Test]
        public void TestDefaultAttributes()
        {
            AssertThatHispRendersCorrectly("testfiles\\test2.hisp", "testfiles\\test2.html", new Dictionary<string, object>());
        }

        [Test]
        public void TestCustomAttributes()
        {
            AssertThatHispRendersCorrectly("testfiles\\test3.hisp", "testfiles\\test3.html", new Dictionary<string, object>());
        }

        [Test]
        public void TestLayoutChanges()
        {
            AssertThatHispRendersCorrectly("testfiles\\test4.hisp", "testfiles\\test4.html", new Dictionary<string, object>());
        }

        [Test]
        public void TestBasicVariables()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["astring"] = "this is a string";
            context["anumber"] = 34.98;
            context["aclass"] = new ClassNode("red");
            context["anobject"] = new DummyObject();
            AssertThatHispRendersCorrectly("testfiles\\test5.hisp", "testfiles\\test5.html", context);
        }

        [Test]
        public void TestProperties()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["anobject"] = new DummyObject();
            AssertThatHispRendersCorrectly("testfiles\\test6.hisp", "testfiles\\test6.html", context);
        }

        [Test]
        public void TestNestedGets()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["anobject"] = new DummyObject();
            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile("html\r\n    <<<anobject Itself> Itself> Itself>");
            Assert.AreEqual("<html>\r\n  ToString called for object\r\n</html>\r\n", hisp.Render(context));
        }

        [Test]
        public void TestNestedGetsReturningClass()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            context["anobject"] = new DummyObject();
            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile("html\r\n    <<<anobject Itself> Itself> AddClass>");
            Assert.AreEqual("<html class=\"red\"/>\r\n", hisp.Render(context));
        }

        [Test]
        public void TestCondReturningClass()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile(
@"html
  cond
    <eq ""a"" ""b""> <body .class>");
            Console.WriteLine(hisp.Render(context));
            Assert.AreEqual("<html>\r\n  <body class=\"class\"/>\r\n</html>", hisp.Render(context));
        }

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

        private void AssertThatHispRendersCorrectly(string hispFile, string resultFile, Dictionary<string, object> context)
        {
            string hispString = new System.IO.StreamReader(hispFile).ReadToEnd();
            string resultString = new System.IO.StreamReader(resultFile).ReadToEnd();

            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile(hispString);
            string actual = hisp.Render(context);
            Console.WriteLine(actual);
            Assert.AreEqual(resultString, actual);
        }
    }
}
