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
            AssertThatHispRendersCorrectly("testfiles\\test1.hisp", "testfiles\\test1.html");
        }

        [Test]
        public void TestDefaultAttributes()
        {
            AssertThatHispRendersCorrectly("testfiles\\test2.hisp", "testfiles\\test2.html");
        }

        [Test]
        public void TestCustomAttributes()
        {
            AssertThatHispRendersCorrectly("testfiles\\test3.hisp", "testfiles\\test3.html");
        }

        [Test]
        public void TestLayoutChanges()
        {
            AssertThatHispRendersCorrectly("testfiles\\test4.hisp", "testfiles\\test4.html");
        }

        private void AssertThatHispRendersCorrectly(string hispFile, string resultFile)
        {
            string hispString = new System.IO.StreamReader(hispFile).ReadToEnd();
            string resultString = new System.IO.StreamReader(resultFile).ReadToEnd();

            HispCompiler compiler = new HispCompiler();
            Hisp hisp = compiler.compile(hispString);
            Assert.AreEqual(resultString, hisp.Render());
        }
    }
}
