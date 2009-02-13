using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.tiestvilee.hisp.parser;
using NUnit.Framework;

namespace com.tiestvilee.hisp.parser
{
    [TestFixture]
    public class WhitespaceToBracketsTest
    {
        [Test]
        public void MakesNoChange()
        {
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            string actual = converter.convert("<html <head> <body <h1>>>");

            Assert.AreEqual("<html<head><body<h1>>>", removeWhiteSpace(actual));
        }

        [Test]
        public void SurroundsWithBrackets()
        {
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            string actual = converter.convert(
                @"html <head> <body <h1>>");

            Assert.AreEqual(
                "<html<head><body<h1>>>", removeWhiteSpace(actual));
        }

        [Test]
        public void AddsSimpleBrackets() {
        
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            TestConversion(converter, 
@"html 
    head",
@"<html<head>>");

            TestConversion(converter, 
@"html 
    body
        h1",
@"<html<body<h1>>>");



            TestConversion(converter, 
@"html 
    head
    body",
@"<html<head><body>>");

            TestConversion(converter, 
@"html 
    head
    body
        h1",
@"<html<head><body<h1>>>");
        }


        [Test]
        public void AddsDescendingBrackets()
        {
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            TestConversion(converter, 
@"html 
    head
    body
        div
            h1
            h2
        div
            span
                input
        div
            span", 
@"<html<head><body<div<h1><h2>><div<span<input>>><div<span>>>>>>");
        }


        [Test]
        public void AddsExtraBrackets()
        {
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            TestConversion(converter, 
@"if 
    gt 3 4
    <span
    span>
    span",
@"<if<gt34><<span><span>><span>>");
        }


        [Test]
        public void IgnoresEmptyLines()
        {
            WhitespaceToBrackets converter = new WhitespaceToBrackets();

            TestConversion(converter, 
@"HTML

    HEAD
    
    BODY",
@"<HTML<HEAD><BODY>>");
        }


        private void TestConversion(WhitespaceToBrackets converter, string input, string expected)
        {
            string actual = converter.convert(
                input);

            Assert.AreEqual(
                expected, removeWhiteSpace(actual));
        }

        private object removeWhiteSpace(string actual)
        {
            return new Regex("\\s").Replace(actual, "");
        }


    }
}
