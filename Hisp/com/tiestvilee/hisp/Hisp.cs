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
            return new Evaluator().Eval(context, "", root).GetText();
        }

        public class Evaluator
        {

            public Node Eval(Dictionary<string, object> context, string indent, ListNode nodes)
            {
                Node head = nodes.Head;

                if (head.GetType() == typeof(ListNode))
                {
                    head = Eval(context, indent + "  ", (ListNode)head);
                }

                return head.Eval(this, context, nodes.Tail, indent);
            }

            public Node ProcessVariable(Dictionary<string, object> context, object head, IList<Node> tail, string indent)
            {
                if (tail.Count > 0)
                {
                    Node memberNameNode = tail[0];
                    if (memberNameNode.GetType() == typeof(ListNode))
                    {
                        memberNameNode = Eval(context, indent, (ListNode)memberNameNode);
                    }
                    head = MakeReflectiveCall(head, memberNameNode.GetText());
                }

                if (head.GetType().IsSubclassOf(typeof(Node)))
                {
                    return (Node)head;
                }
                return new VariableNode(head);
            }

            private object MakeReflectiveCall(object variable, string memberName)
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


            public Node RenderTag(Dictionary<string, object> context, object head, IList<Node> nodes, string indent)
            {
                StringBuilder result = new StringBuilder();
                TagContents subTagContents = new TagContents();

                string newIndent = indent + "  ";

                RenderTag_ProcessParameters(nodes, context, newIndent, subTagContents);

                RenderTag_CreateText(indent, subTagContents, result, ((AtomNode)head).GetText());

                return new StringNode(result.ToString());
            }

            private void RenderTag_CreateText(string indent, TagContents subTagContents, StringBuilder result, string tagName)
            {
                result.Append(indent).Append('<').Append(tagName).Append(subTagContents.ToString());

                if (subTagContents.HasChildren)
                {
                    result.Append(">\r\n");
                    foreach (string child in subTagContents.Children)
                    {
                        result.Append(child);
                    }
                    result.Append(indent).Append("</").Append(tagName).Append(">\r\n");
                }
                else
                {
                    result.Append("/>\r\n");
                }
            }

            private void RenderTag_ProcessParameters(IList<Node> nodes, Dictionary<string, object> context, string newIndent, TagContents subTagContents)
            {
                foreach (Node node in nodes)
                {
                    Node resultantNode = node;
                    bool poo = false;
                    if (node.GetType() == typeof(ListNode))
                    {
                        resultantNode = Eval(context, newIndent, (ListNode)node);
                        poo = true;
                    }

                    resultantNode.modifyNode(subTagContents, newIndent, poo);
                }
            }
        }
    }

    public class TagContents
    {
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        IList<string> children = new List<string>();

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

        public bool HasChildren
        {
            get { return children.Count > 0; }
        }

        public IEnumerable Children
        {
            get { return children; }
        }

        public void AddChild(string s)
        {
            children.Add(s);
        }
    }
}
