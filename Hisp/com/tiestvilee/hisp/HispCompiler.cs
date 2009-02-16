using System;
using System.Collections;
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
            HispLexer lexer = new HispLexer(new System.IO.StringReader(hispString));

            lexer.nextToken();
            
            if (lexer.getTokenObject().Type == HispLexerTokenTypes.LPAREN)
            {
                lexer.nextToken();
            } 
            
            int currentLineIndent = 0;
            return new Hisp(CompileTagNode(lexer, 0, ref currentLineIndent));
        }

        private Node CompileTagNode(HispLexer lexer, int indent, ref int currentLineIndent)
        {
            IToken token = lexer.getTokenObject();

            while (token.Type == HispLexerTokenTypes.WHITESPACE || token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();

            string tagName = token.getText();
            IList<Node> children = new List<Node>();

            if (token.Type == HispLexerTokenTypes.LPAREN)
            {
                tagName = "METHOD CALL";
            }
            else
            {
                lexer.nextToken();
            }
            ProcessTokensInTagNode(lexer, indent, ref currentLineIndent, children);

            return new TagNode(tagName, children);
        }

        private void ProcessTokensInTagNode(HispLexer lexer, int indent, ref int currentLineIndent, IList<Node> children)
        {
            int currentTagIndent = -1;
            while (true)
            {
                IToken token = lexer.getTokenObject();
                switch (token.Type)
                {
                    case HispLexerTokenTypes.LPAREN:
                        lexer.nextToken();
                        children.Add((TagNode)CompileTagNode(lexer, currentLineIndent, ref currentLineIndent));
                        break;
                    case HispLexerTokenTypes.UNQUOTED_STRING:
                        if (currentLineIndent > indent)
                        {
                            children.Add((TagNode)CompileTagNode(lexer, currentLineIndent, ref currentLineIndent));
                        }
                        else
                        {
                            children.Add(new TagNode(token.getText(), new List<Node>()));
                            lexer.nextToken();
                        }
                        break;
                    case HispLexerTokenTypes.HASH:
                        token = lexer.nextToken();
                        IdNode childIdNode = new IdNode(token.getText());
                        children.Add(childIdNode);
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.DOT:
                        token = lexer.nextToken();
                        ClassNode childClassNode = new ClassNode(token.getText());
                        children.Add(childClassNode);
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.ATTRIBUTE:
                        token = lexer.nextToken();
                        string key = token.getText();
                        token = lexer.nextToken();
                        token.getText();
                        token = lexer.nextToken(); // string or unquoted string
                        string value = token.getText();
                        children.Add(new AttributeNode(key, value));
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.STRING:
                        StringNode stringNode = new StringNode(token.getText());
                        children.Add(stringNode);
                        lexer.nextToken();
                        break;
                    case HispLexerTokenTypes.NEWLINE:
                        token = SkipNewlines(lexer);
                        currentLineIndent = 0;
                        if (token.Type == HispLexerTokenTypes.WHITESPACE)
                            currentLineIndent = token.getText().Length;
                            
                        if (currentTagIndent < 0)
                        {
                            currentTagIndent = currentLineIndent;
                            if (currentTagIndent <= indent)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (currentLineIndent > currentTagIndent)
                            {
                                CompileTagNode(lexer, currentTagIndent, ref currentLineIndent);
                            }
                        }
                        break;
                    case HispLexerTokenTypes.EOF:
                    case HispLexerTokenTypes.RPAREN:
                        lexer.nextToken();
                        return;
                    default:
                        lexer.nextToken();
                        break;
                }

                if (currentLineIndent < currentTagIndent)
                {
                    return;
                }
            }
        }

        private IToken SkipNewlines(HispLexer lexer)
        {
            IToken token = lexer.getTokenObject();
            while (token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();
            return token;
        }

    }


    public abstract class HispVisitor
    {
        public abstract void Visit(TagNode node, string indent, StringBuilder result, Attributes attributes);
        public abstract void Visit(IdNode node, string indent, StringBuilder result, Attributes attributes);
        public abstract void Visit(ClassNode node, string indent, StringBuilder result, Attributes attributes);
        public abstract void Visit(AttributeNode node, string indent, StringBuilder result, Attributes attributes);
        public abstract void Visit(StringNode node, string indent, StringBuilder result, Attributes attributes);
        public abstract void Visit(VariableNode node, string indent, StringBuilder result, Attributes attributes);
    }


    public abstract class Node
    {
        protected string text;

        public string GetText()
        {
            return text;
        }

        public virtual Node this[int i]
        {
            get { return null; }
        }

        public abstract void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes);

        public static string StripInvertedCommas(string input)
        {
            if (input[0] == '"')
            {
                return input.Substring(1, input.Length - 2);
            }
            return input;
        }

        public virtual string Describe(string indent)
        {
            return indent + text + "\r\n";
        }

    }


    public class TagNode : Node
    {
        private IList<Node> children;

        public IList<Node> Children { get { return children;  } }

        public TagNode(string text, IList<Node> children)
        {
            this.text = text;
            this.children = children;
        }

        public override Node this[int i]
        {
            get { return children[i]; }
        }


        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }

        public override string Describe(string indent)
        {
            string result = indent + text + "\r\n";
            foreach (Node node in children)
            {
                result += node.Describe(indent + "    ");
            }
            return result;
        }
    }

    public class IdNode : Node
    {
        public IdNode(string text)
        {
            this.text = text;
        }

        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }
    }

    public class ClassNode : Node
    {
        public ClassNode(string text)
        {
            this.text = text;
        }

        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }
    }

    public class StringNode : Node
    {
        public StringNode(string text)
        {
            this.text = StripInvertedCommas(text);
        }

        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }
    }


    public class AttributeNode : Node
    {
        private readonly string value;

        public AttributeNode(string key, string value)
        {
            this.text = key;
            this.value = StripInvertedCommas(value);
        }

        public string GetValue()
        {
            return value;
        }

        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }
    }

    public class VariableNode : Node
    {
        public VariableNode(string text)
        {
            this.text = StripInvertedCommas(text);
        }

        public override void Accept(HispVisitor visitor, string indent, StringBuilder result, Attributes attributes)
        {
            visitor.Visit(this, indent, result, attributes);
        }
    }
}
