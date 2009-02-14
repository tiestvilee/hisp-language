using System;
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
            StringBuilder result = new StringBuilder(256);

            RenderNode(root, result, "", null);

            return result.ToString();
        }

        private void RenderNode(Node ast, StringBuilder result, string indent, Attributes attributes)
        {
            if(ast == null)
            {
                return;
            }

//            switch (ast.Type)
//            {
////                case HispTokenTypes.UNQUOTED_STRING:
////                    RenderTag(ast, indent, result);
////                    break;
////
////                case HispTokenTypes.HASH:
////                    attributes.Id = ast.getFirstChild().getText();
////                    break;
////
////                case HispTokenTypes.CLASS:
////                    attributes.addAttributeValue("class", ast.getFirstChild().getText());
////                    break;
////
////                case HispTokenTypes.ATTRIBUTE:
////                    string key = ast.getFirstChild().getText();
////                    string value = ast.getFirstChild().getNextSibling().getText();
////                    if(value[0] == '"')
////                    {
////                        value = value.Substring(1, value.Length - 2);
////                    }
////                    attributes.addAttributeValue(key, value);
////                    break;
//
//            }
//
//            RenderNode(ast.getNextSibling(), result, indent, attributes);

            return;
        }

        private void RenderTag(Node ast, string indent, StringBuilder result)
        {
//            StringBuilder subResult = new StringBuilder(256);
//            Attributes subAttributes = new Attributes();
//            RenderNode(ast.getFirstChild(), subResult, indent + "    ", subAttributes);
//
//            result.Append(indent).Append('<').Append(ast.getText()).Append(subAttributes.ToString());
//
//            if (subResult.Length == 0)
//            {
//                result.Append("/>\r\n");
//            }
//            else
//            {
//
//                result.Append(">\r\n");
//                result.Append(subResult);
//                result.Append(indent).Append("</").Append(ast.getText()).Append(">\r\n");
//            }
        }
    }

    internal class Attributes
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
