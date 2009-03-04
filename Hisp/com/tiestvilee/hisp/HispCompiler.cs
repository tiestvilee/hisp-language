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

        private ListNode CompileTagNode(HispLexer lexer, int indent, ref int currentLineIndent)
        {
            IToken token = lexer.getTokenObject();

            while (token.Type == HispLexerTokenTypes.WHITESPACE || token.Type == HispLexerTokenTypes.NEWLINE)
                token = lexer.nextToken();

            IList<Node> children = new List<Node>();

            ProcessTokensInTagNode(lexer, indent, ref currentLineIndent, children);

            return new ListNode(children);
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
                        children.Add(CompileTagNode(lexer, currentLineIndent, ref currentLineIndent));
                        break;
                    case HispLexerTokenTypes.UNQUOTED_STRING:
                        if (currentLineIndent > indent)
                        {
                            children.Add(CompileTagNode(lexer, currentLineIndent, ref currentLineIndent));
                        }
                        else
                        {
                            children.Add(new AtomNode(token.getText()));
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


    public abstract class Node
    {
        protected string text;

        public virtual string GetText()
        {
            return text;
        }

        public virtual Node this[int i]
        {
            get { throw new Exception("not implemented"); }
        }

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
            if (indent == null)
                return text + " ";
            return indent + text + "\r\n";
        }

        public virtual Node Eval(Hisp.Evaluator evaluator, Dictionary<string, object> context, IList<Node> parameters, string indent)
        {
            return this;
        }

        public virtual void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            throw new NotImplementedException();
        }
    }

    public class ListNode : Node
    {
        private IList<Node> children;
        public IList<Node> Children { get { return children; } }

        private IList<Node> tail;
        public Node Head { get { return children[0]; } }
        public IList<Node> Tail { get { return tail; } }

        public override Node this[int i]
        {
            get { return children[i]; }
        }

        public ListNode(IList<Node> children)
        {
            this.children = children;
            this.text = "<<LIST>>";
            this.tail = new List<Node>(children);
            this.tail.RemoveAt(0);
        }

        public override string Describe(string indent)
        {
            string result;
            if (indent == null)
            {
                result = "<";
                foreach (Node node in children)
                {
                    result += node.Describe(null);
                }
                result += ">";
            } else
            {
                result = indent + "\r\n";
                foreach (Node node in children)
                {
                    result += node.Describe(indent + "    ");
                }
            }

            return result;
        }
    }

    public class AtomNode : Node
    {
        public AtomNode(string text)
        {
            this.text = text;
        }

        public override Node Eval(Hisp.Evaluator evaluator, Dictionary<string, object> context, IList<Node> parameters, string indent)
        {
            object variable;
            if (context.TryGetValue(text, out variable))
            {
                return evaluator.ProcessVariable(context, variable, parameters, indent);
            }
            return evaluator.RenderTag(context, this, parameters, indent);
        }

    }

    public class IdNode : Node
    {
        public IdNode(string text)
        {
            this.text = text;
        }

        public override void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            tagContents.Id = text;
        }
    }

    public class ClassNode : Node
    {
        public ClassNode(string text)
        {
            this.text = text;
        }

        public override void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            tagContents.addAttributeValue("class", text);
        }
    }

    public class StringNode : Node
    {
        public StringNode(string text)
        {
            this.text = StripInvertedCommas(text);
        }

        public override void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            if (headWasList)
            {
                tagContents.AddChild(text);
            }
            else
            {
                tagContents.AddChild(indent + text + "\r\n");
            }
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

        public override void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            tagContents.addAttributeValue(text, value);
        }
    }

    public class VariableNode : Node
    {
        private readonly object variable;
        public object Value{get { return variable;}}

        public override string GetText()
        {
            return variable.ToString();
        }

        public VariableNode(object variable)
        {
            this.variable = variable;
            this.text = "VARIABLE!!!!";
        }

        public override Node Eval(Hisp.Evaluator evaluator, Dictionary<string, object> context, IList<Node> parameters, string indent)
        {
            return evaluator.ProcessVariable(context, variable, parameters, indent);
        }

        public override void updateTagContents(TagContents tagContents, string indent, bool headWasList)
        {
            tagContents.AddChild(indent + variable.ToString() + "\r\n");
        }
    }
}
