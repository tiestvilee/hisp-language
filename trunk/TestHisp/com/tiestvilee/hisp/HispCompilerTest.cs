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
        public void CanCompileStringIntoHisp()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title
    body
        h1");

        }

        [Test]
        public void CanRenderHispString()
        {
            HispCompiler compiler = new HispCompiler();

            Hisp hisp = compiler.compile(
@"html
    head
        title
    body
        h1");

            Assert.AreEqual(
@"<html>
    <head>
        <title/>
    </head>
    <body>
        <h1/>
    </body>
</html>
", hisp.Render());

        }


    }
}
