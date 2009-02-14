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
        private IToken oldToken;

        public Hisp compile(string hispString)
        {
            HispLexer lexer = new HispLexer(new System.IO.StringReader(hispString));

            lexer.nextToken();
            return new Hisp(CompileNode(lexer, 0));
        }

        private Node CompileNode(HispLexer lexer, int indent)
        {
            IToken token = lexer.getTokenObject();

            if(lexer.getTokenObject().Type == HispLexerTokenTypes.LPAREN)
            {
                token = lexer.nextToken();
            }

            while (token.Type == HispLexerTokenTypes.WHITESPACE || token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();

            string tagName = token.getText();
            IList<Node> children = new List<Node>();

            token = lexer.nextToken();
            bool reachedEndOfNode = false;
            while ( ! reachedEndOfNode)
            {
                switch (token.Type)
                {
                    case HispLexerTokenTypes.UNQUOTED_STRING:
                        throw new Exception("shouldn't be here");
                        TagNode childTagNode = (TagNode) CompileNode(lexer, currentLineIndent); // new TagNode(token.getText(), new List<Node>());
                        children.Add(childTagNode);
                        break;
                    case HispLexerTokenTypes.LPAREN:
                        TagNode childTagNode2 = (TagNode)CompileNode(lexer, currentLineIndent); // new TagNode(token.getText(), new List<Node>());
                        children.Add(childTagNode2);
                        break;
                    case HispLexerTokenTypes.HASH:
                        token = lexer.nextToken();
                        IdNode childIdNode = new IdNode(token.getText());
                        children.Add(childIdNode);
                        break;
                    case HispLexerTokenTypes.CLASS:
                        token = lexer.nextToken();
                        ClassNode childClassNode = new ClassNode(token.getText());
                        children.Add(childClassNode);
                        break;
                    case HispLexerTokenTypes.NEWLINE:
                        SkipNewlines(lexer);
                        currentLineIndent = CountSpaces(lexer);
                        reachedEndOfNode = ProcessLineBasedOnIndent(lexer, children, indent);
                        break;
                }
                if (!reachedEndOfNode)
                {
                    if (oldToken != null)
                    {
                        token = oldToken;
                        oldToken = null;
                    } 
                    else 
                    {
                        token = lexer.nextToken();
                    }
                    if (token.Type == HispLexerTokenTypes.RPAREN || token.Type == HispLexerTokenTypes.EOF)
                    {
                        reachedEndOfNode = true;
                    }
                }
            }

            return new TagNode(tagName, children);
        }

        private bool ProcessLineBasedOnIndent(HispLexer lexer, IList<Node> children, int indent)
        {
            if (currentLineIndent > indent)
            {
                children.Add(CompileNode(lexer, currentLineIndent));
                if(currentLineIndent <= indent)
                {
                    return true;
                }
            }
            else if (currentLineIndent <= indent)
            {
                return true;
                // what about going up by more than 1...
            }
            return false;
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
            oldToken = token;
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
