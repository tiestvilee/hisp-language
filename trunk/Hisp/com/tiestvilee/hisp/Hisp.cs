using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using antlr;
using antlr.collections;
using com.tiestvilee.hisp.parser;

namespace com.tiestvilee.hisp
{
    public class Hisp
    {
        private ListNode root;
        public ListNode Root { get { return root; } }

        public Hisp(ListNode root)
        {
            this.root = root;
        }

        public string Render(Dictionary<string, object> context)
        {
            return Eval(context, "", root).GetText();
        }


        private static Node Eval(Dictionary<string, object> context, string indent, ListNode nodes)
        {
            Node head = nodes.Head;

            if (head.GetType() == typeof(ListNode))
            {
                head = Eval(context, indent + "  ", (ListNode) head);
            }
                
                
            if (head.GetType() == typeof(AtomNode))
            {
                object variable;
                if (context.TryGetValue(head.GetText(), out variable))
                {
                    return ProcessVariable(context, nodes.Tail, variable, indent);
                }
                else
                {
                    return RenderTag(indent, nodes, context);
                }
            }
            else if (head.GetType() == typeof(VariableNode))
            {
                return ProcessVariable(context, nodes.Tail, ((VariableNode) head).Value, indent);
            }
            else
            {
                return head;
            }
        }

        private static Node ProcessVariable(Dictionary<string, object> context, IList<Node> tail, object variable, string indent)
        {
            if (tail.Count > 0)
            {
                Node memberNameNode = tail[0];
                if(memberNameNode.GetType() == typeof(ListNode))
                {
                    memberNameNode = Eval(context, indent, (ListNode) memberNameNode);
                }
                variable = MakeReflectiveCall(variable, memberNameNode.GetText());
            }

            if (variable.GetType().IsSubclassOf(typeof(Node)))
            {
                return (Node) variable;
            }
            else
            {
                return new VariableNode(variable);
            }
        }

        private static object MakeReflectiveCall(object variable, string memberName)
        {
            foreach (MemberInfo info in variable.GetType().GetMember(memberName))
            {
                if (info.GetType().IsSubclassOf(typeof(PropertyInfo)))
                {
                    return ((PropertyInfo)info).GetGetMethod().Invoke(variable, null);
                }

                if (info.GetType().IsSubclassOf(typeof(MethodInfo)))
                {
                    return ((MethodInfo)info).Invoke(variable, null);
                }
            }
            memberName = "Get" + memberName;
            foreach (MemberInfo info in variable.GetType().GetMember(memberName))
            {
                if (info.GetType().IsSubclassOf(typeof(MethodInfo)))
                {
                    return ((MethodInfo)info).Invoke(variable, null);
                }
            }

            return "REFLECTIVE CALL FAILED [" + variable.GetType().Name + "].[" + memberName + "]";
        }


        private static Node RenderTag(string indent, ListNode nodes, Dictionary<string, object> context)
        {
            StringBuilder result = new StringBuilder();
            IList<string> children = new List<string>();
            Attributes subAttributes = new Attributes();

            string newIndent = indent + "  ";
            string tagName = nodes.Head.GetText();

            RenderTag_ProcessParameters(nodes, context, newIndent, subAttributes, children);

            RenderTag_CreateText(indent, subAttributes, children, result, tagName);

            return new StringNode(result.ToString());
        }

        private static void RenderTag_CreateText(string indent, Attributes subAttributes, IList<string> children, StringBuilder result, string tagName)
        {
            result.Append(indent).Append('<').Append(tagName).Append(subAttributes.ToString());

            if (children.Count == 0)
            {
                result.Append("/>\r\n");
            }
            else
            {

                result.Append(">\r\n");
                foreach (string child in children)
                {
                    result.Append(child);
                }
                result.Append(indent).Append("</").Append(tagName).Append(">\r\n");
            }
        }

        private static void RenderTag_ProcessParameters(ListNode nodes, Dictionary<string, object> context, string newIndent, Attributes subAttributes, IList<string> children)
        {
            foreach (Node node in nodes.Tail)
            {
                Node resultantNode = node;
                bool poo = false;
                if(node.GetType() == typeof (ListNode))
                {
                    resultantNode = Eval(context, newIndent, (ListNode)node);
                    poo = true;
                }

                Type type = resultantNode.GetType();
                if (type == typeof(IdNode))
                {
                    subAttributes.Id = resultantNode.GetText();
                }
                else if (type == typeof(ClassNode))
                {
                    subAttributes.addAttributeValue("class", resultantNode.GetText());
                }
                else if (type == typeof(AttributeNode))
                {
                    subAttributes.addAttributeValue(resultantNode.GetText(), ((AttributeNode)resultantNode).GetValue());
                }
                else if (type == typeof(StringNode) || type == typeof(VariableNode))
                {
                    string text = resultantNode.GetText();
                    if (poo && ! (type == typeof(VariableNode)))
                    {
                        children.Add(text);
                    }
                    else
                    {
                        children.Add(newIndent + text + "\r\n");
                    }
                }
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
