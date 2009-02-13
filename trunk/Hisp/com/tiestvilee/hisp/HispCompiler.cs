using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using antlr;
using com.tiestvilee.hisp.parser;

namespace com.tiestvilee.hisp
{
    public class HispCompiler
    {
        public Hisp compile(string hispString)
        {
            hispString = new WhitespaceToBrackets().convert(hispString);
            Console.WriteLine(hispString);
            HispLexer lexer = new HispLexer(new System.IO.StringReader(hispString));
            HispParser parser = new HispParser(lexer);
            parser.expression();
            return new Hisp( (CommonAST)parser.getAST() );
        }
    }
}
