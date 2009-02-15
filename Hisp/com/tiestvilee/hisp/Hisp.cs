using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using antlr;
using antlr.collections;
using com.tiestvilee.hisp.parser;

namespace com.tiestvilee.hisp
{
    public class Hisp
    {
        private Node root;
        public Node Root { get{ return root;}}

        public Hisp(Node root)
        {
            this.root = root;
        }

        public string Render()
        {
            RenderVisitor visitor = new RenderVisitor();
            StringBuilder result = new StringBuilder(256);
            root.Accept(visitor, "", result, null);

            return result.ToString();
        }

        public class RenderVisitor : HispVisitor
        {
            public override void Visit(TagNode tagNode, string indent, StringBuilder result, Attributes attributes)
            {
                StringBuilder subResult = new StringBuilder(256);
                Attributes subAttributes = new Attributes();
                string newIndent = indent + "  ";
                foreach (Node node in tagNode.Children)
                {
                    node.Accept(this, newIndent, subResult, subAttributes);
                }

                result.Append(indent).Append('<').Append(tagNode.GetText()).Append(subAttributes.ToString());

                if (subResult.Length == 0)
                {
                    result.Append("/>\r\n");
                }
                else
                {

                    result.Append(">\r\n");
                    result.Append(subResult);
                    result.Append(indent).Append("</").Append(tagNode.GetText()).Append(">\r\n");
                }
            }

            public override void Visit(IdNode node, string indent, StringBuilder result, Attributes attributes)
            {
                attributes.Id = node.GetText();
            }

            public override void Visit(ClassNode node, string indent, StringBuilder result, Attributes attributes)
            {
                attributes.addAttributeValue("class", node.GetText());
            }

            public override void Visit(AttributeNode node, string indent, StringBuilder result, Attributes attributes)
            {
                attributes.addAttributeValue(node.GetText(), node.GetValue());
            }

            public override void Visit(StringNode node, string indent, StringBuilder result, Attributes attributes)
            {
                result.Append(indent).Append(node.GetText()).Append("\r\n");
            }

        }

    }

    public class Attributes
    {
        private Dictionary<string, string> attributes = new Dictionary<string, string>();

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var pair in attributes)
            {
                result.Append(' ').Append(pair.Key).Append("=\"").Append(pair.Value).Append('"');
            }
            return result.ToString();
        }

        public void addAttributeValue(string attribute, string value)
        {
            string oldValue = "";
            if(attributes.TryGetValue(attribute, out oldValue))
            {
                attributes[attribute] = oldValue + " " + value;
            } else
            {
                attributes[attribute] = value;
            }
        }

        public string Id { set { addAttributeValue("id", value); } }

    }
}
