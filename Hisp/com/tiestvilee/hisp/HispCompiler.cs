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
        private int currentLineIndent = 0;


        public Hisp compile(string hispString)
        {
            HispLexer lexer = new HispLexer(new System.IO.StringReader(hispString));

            lexer.nextToken();
            return new Hisp(CompileTagNode(lexer, 0));
        }

        private Node CompileTagNode(HispLexer lexer, int indent)
        {
            int currentTagIndent = -1;
            IToken token = lexer.getTokenObject();

            if(lexer.getTokenObject().Type == HispLexerTokenTypes.LPAREN)
            {
                token = lexer.nextToken();
            }

            while (token.Type == HispLexerTokenTypes.WHITESPACE || token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();

            string tagName = token.getText();
            Console.WriteLine("compiling new node " + tagName);
            IList<Node> children = new List<Node>();

            token = lexer.nextToken();
            bool reachedEndOfNode = false;

            while ( ! reachedEndOfNode)
            {
                Console.WriteLine("round and round for " + tagName + " '" + token.getText() + "'");
                switch (token.Type)
                {
                    case HispLexerTokenTypes.LPAREN:
                    case HispLexerTokenTypes.UNQUOTED_STRING:
                        TagNode childTagNode = (TagNode) CompileTagNode(lexer, currentLineIndent);
                        children.Add(childTagNode);
                        break;
                    case HispLexerTokenTypes.HASH:
                        token = lexer.nextToken();
                        IdNode childIdNode = new IdNode(token.getText());
                        children.Add(childIdNode);
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.CLASS:
                        token = lexer.nextToken();
                        ClassNode childClassNode = new ClassNode(token.getText());
                        children.Add(childClassNode);
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.NEWLINE:
                        SkipNewlines(lexer);
                        currentLineIndent = CountSpaces(lexer);
                        if (currentTagIndent < 0)
                        {
                            currentTagIndent = currentLineIndent;
                            if (currentTagIndent <= indent)
                            {
                                reachedEndOfNode = true;
                            }
                        }
                        else
                        {
                            if (currentLineIndent > currentTagIndent)
                            {
                                CompileTagNode(lexer, currentTagIndent);
                            }
                        }
                        break;
                    case HispLexerTokenTypes.EOF:
                    case HispLexerTokenTypes.RPAREN:
                        reachedEndOfNode = true;
                        lexer.nextToken();
                        break;
                    default:
                        lexer.nextToken();
                        break;
                }

                if (currentLineIndent < currentTagIndent)
                {
                    reachedEndOfNode = true;
                }
                token = lexer.getTokenObject();
            }

            Console.WriteLine("compiled node " + tagName + " '" + token.getText() + "' " + currentLineIndent + " vs " + currentTagIndent);

            return new TagNode(tagName, children);
        }

        private void SkipNewlines(HispLexer lexer)
        {
            IToken token = lexer.getTokenObject();
            while (token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();
        }

        private int CountSpaces(HispLexer lexer)
        {
            IToken token = lexer.getTokenObject();
            int result = 0;
            while (token.Type == HispLexerTokenTypes.WHITESPACE)
            {
                token = lexer.nextToken();
                result++;
            }
            //oldToken = token;
            return result;
        }
    }


    public class Node
    {
        protected string text;

        public string getText()
        {
            return text;
        }

        public virtual Node this[int i]
        {
            get { return null; }
        }
    }

    public class TagNode : Node
    {
        private IList<Node> children;

        public TagNode(string text, IList<Node> children)
        {
            this.text = text;
            this.children = children;
        }

        public override Node this[int i]
        {
            get { return children[i]; }
        }
    }

    public class IdNode : Node
    {
        public IdNode(string text)
        {
            this.text = text;
        }
    }

    public class ClassNode : Node
    {
        public ClassNode(string text)
        {
            this.text = text;
        }
    }
}
