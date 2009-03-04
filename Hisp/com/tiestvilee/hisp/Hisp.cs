using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
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

        public string ToHtml(Dictionary<string, object> context)
        {
            return new Evaluator(new HtmlRenderer()).Eval(context, "", root).GetText();
        }

        public XmlDocument ToXml(Dictionary<string, object> context)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(((XmlNode)new Evaluator(new XmlRenderer(xmlDocument)).Eval(context, "", root)).GetXml());

            return xmlDocument;
        }

        public class Evaluator
        {
            private Hisp.Renderer renderer;

            public Evaluator(Renderer renderer)
            {
                this.renderer = renderer;
            }

            public Node Eval(Dictionary<string, object> context, string indent, ListNode nodes)
            {
                Node head = nodes.Head;

                if (head.GetType() == typeof(ListNode))
                {
                    head = Eval(context, indent, (ListNode)head);
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
                TagContents subTagContents = renderer.NewTagContents();

                RenderTag_ProcessParameters(nodes, context, indent + "  ", subTagContents);

                return renderer.RenderTag(indent, subTagContents, ((AtomNode) head).GetText());
            }

            private void RenderTag_ProcessParameters(IList<Node> nodes, Dictionary<string, object> context, string indent, TagContents subTagContents)
            {
                foreach (Node node in nodes)
                {
                    Node resultantNode = node;
                    bool poo = false;
                    if (node.GetType() == typeof(ListNode))
                    {
                        resultantNode = Eval(context, indent, (ListNode)node);
                        poo = true;
                    }

                    resultantNode.updateTagContents(subTagContents, indent, poo);
                }
            }
        }

        public interface Renderer
        {
            Node RenderTag(string indent, TagContents subTagContents, string tagName);

            TagContents NewTagContents();
        }

        public class HtmlRenderer : Renderer
        {
            public Node RenderTag(string indent, TagContents subTagContents, string tagName)
            {
                HtmlTagContents tagContents = (HtmlTagContents) subTagContents;
                StringBuilder result = new StringBuilder();

                result.Append(indent).Append('<').Append(tagName).Append(tagContents.ToString());

                if (tagContents.HasChildren)
                {
                    result.Append(">\r\n");
                    foreach (string child in tagContents.Children)
                    {
                        result.Append(child);
                    }
                    result.Append(indent).Append("</").Append(tagName).Append(">\r\n");
                }
                else
                {
                    result.Append("/>\r\n");
                }
                return new StringNode(result.ToString());
            }

            public TagContents NewTagContents()
            {
                return new HtmlTagContents();
            }
        }

        public class XmlRenderer : Hisp.Renderer
        {
            private readonly XmlDocument document;

            public XmlRenderer(XmlDocument document)
            {
                this.document = document;
            }

            public Node RenderTag(string indent, TagContents subTagContents, string tagName)
            {
                XmlTagContents tagContents = (XmlTagContents)subTagContents;
                XmlElement result = document.CreateElement(tagName);

                tagContents.updateAttributes(document, result);

                if (tagContents.HasChildren)
                {
                    foreach (Node child in tagContents.Children)
                    {
                        Type childType = child.GetType();
                        if(childType == typeof(XmlNode))
                        {
                            result.AppendChild(((XmlNode) child).GetXml());
                        }
                        else
                        {
                            result.AppendChild(document.CreateTextNode(child.GetText()));
                        }
                    }
                }
                return new XmlNode(result);
            }

            public TagContents NewTagContents()
            {
                return new XmlTagContents();
            }
        }
    }

    public abstract class TagContents
    {
        public abstract void updateFrom(IdNode node, string indent, bool headWasList);
        public abstract void updateFrom(ClassNode node, string indent, bool headWasList);
        public abstract void updateFrom(AttributeNode node, string indent, bool headWasList);
        public abstract void updateFrom(StringNode node, string indent, bool headWasList);
        public abstract void updateFrom(VariableNode node, string indent, bool headWasList);
        public abstract void updateFrom(XmlNode node, string indent, bool headWasList);
    }

    public class HtmlTagContents : TagContents
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

        public void AddChild(string text)
        {
            children.Add(text);
        }

        public void AddChild(string indent, string text)
        {
            children.Add(indent + text + "\r\n");
        }

        public override void updateFrom(IdNode node, string indent, bool headWasList)
        {
            Id = node.GetText();
        }

        public override void updateFrom(ClassNode node, string indent, bool headWasList)
        {
            addAttributeValue("class", node.GetText());
        }

        public override void updateFrom(AttributeNode node, string indent, bool headWasList)
        {
            addAttributeValue(node.GetText(), node.GetValue());
        }

        public override void updateFrom(StringNode node, string indent, bool headWasList)
        {
            if (headWasList)
            {
                AddChild(node.GetText());
            }
            else
            {
                AddChild(indent, node.GetText());
            }
        }

        public override void updateFrom(VariableNode node, string indent, bool headWasList)
        {
            AddChild(indent, node.GetText());
        }

        public override void updateFrom(XmlNode node, string indent, bool headWasList)
        {
            throw new NotImplementedException();
        }
    }
    public class XmlTagContents : TagContents
    {
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        IList<Node> children = new List<Node>();

        public void addAttributeValue(string attribute, string value)
        {
            string oldValue = "";
            if (attributes.TryGetValue(attribute, out oldValue))
            {
                attributes[attribute] = oldValue + " " + value;
            }
            else
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

        public void AddChild(Node node)
        {
            children.Add(node);
        }

        public void AddChild(string indent, Node node)
        {
            children.Add(node);
        }

        public override void updateFrom(IdNode node, string indent, bool headWasList)
        {
            Id = node.GetText();
        }

        public override void updateFrom(ClassNode node, string indent, bool headWasList)
        {
            addAttributeValue("class", node.GetText());
        }

        public override void updateFrom(AttributeNode node, string indent, bool headWasList)
        {
            addAttributeValue(node.GetText(), node.GetValue());
        }

        public override void updateFrom(StringNode node, string indent, bool headWasList)
        {
            AddChild(node);
        }

        public override void updateFrom(VariableNode node, string indent, bool headWasList)
        {
            AddChild(node);
        }

        public override void updateFrom(XmlNode node, string indent, bool headWasList)
        {
            AddChild(node);
        }

        public void updateAttributes(XmlDocument document, XmlElement element)
        {
            foreach (var pair in attributes)
            {
                element.SetAttribute(pair.Key, pair.Value);
            }
        }
    }
}
