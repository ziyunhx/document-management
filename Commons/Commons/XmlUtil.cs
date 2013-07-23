namespace Commons
{
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml;

    public class XmlUtil : IDisposable
    {
        private bool _alreadyDispose;
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlElement xmlElem;
        private XmlNode xmlNode;

        public static void CreateXml(string path)
        {
            XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8) {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("root");
            for (int i = 0; i < 5; i++)
            {
                writer.WriteStartElement("Node");
                writer.WriteAttributeString("Text", "这是文章内容！~！！~~");
                writer.WriteAttributeString("ImageUrl", "");
                writer.WriteAttributeString("NavigateUrl", "Url..." + i.ToString());
                writer.WriteAttributeString("Expand", "true");
                for (int j = 0; j < 5; j++)
                {
                    writer.WriteStartElement("Node");
                    writer.WriteAttributeString("Text", "......名称");
                    writer.WriteAttributeString("NavigateUrl", "Url..." + i.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteFullEndElement();
            writer.Close();
        }

        public void CreateXmlRoot(string root)
        {
            this.xmlNode = this.xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            this.xmlDoc.AppendChild(this.xmlNode);
            this.xmlElem = this.xmlDoc.CreateElement("", root, "");
            this.xmlDoc.AppendChild(this.xmlElem);
        }

        public void CreatXmlNode(string mainNode, string node)
        {
            XmlNode node2 = this.xmlDoc.SelectSingleNode(mainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(node);
            node2.AppendChild(newChild);
        }

        public void CreatXmlNode(string mainNode, string node, string content)
        {
            XmlNode node2 = this.xmlDoc.SelectSingleNode(mainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(node);
            newChild.InnerText = content;
            node2.AppendChild(newChild);
        }

        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue)
        {
            XmlNode node = this.xmlDoc.SelectSingleNode(MainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(Node);
            newChild.SetAttribute(Attrib, AttribValue);
            node.AppendChild(newChild);
        }

        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Content)
        {
            XmlNode node = this.xmlDoc.SelectSingleNode(MainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(Node);
            newChild.SetAttribute(Attrib, AttribValue);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
        }

        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Attrib2, string AttribValue2)
        {
            XmlNode node = this.xmlDoc.SelectSingleNode(MainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(Node);
            newChild.SetAttribute(Attrib, AttribValue);
            newChild.SetAttribute(Attrib2, AttribValue2);
            node.AppendChild(newChild);
        }

        public void CreatXmlNode(string MainNode, string Node, string Attrib, string AttribValue, string Attrib2, string AttribValue2, string Content)
        {
            XmlNode node = this.xmlDoc.SelectSingleNode(MainNode);
            XmlElement newChild = this.xmlDoc.CreateElement(Node);
            newChild.SetAttribute(Attrib, AttribValue);
            newChild.SetAttribute(Attrib2, AttribValue2);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!this._alreadyDispose)
            {
                this._alreadyDispose = true;
            }
        }

        ~XmlUtil()
        {
            this.Dispose();
        }

        public static ArrayList GetSubElementByAttribute(string XmlPath, string FatherElenetName, string AttributeName, int AttributeIndex, int ArrayLength)
        {
            ArrayList list = new ArrayList();
            XmlDocument document = new XmlDocument();
            document.Load(XmlPath);
            foreach (XmlElement element in document.DocumentElement.ChildNodes)
            {
                if (element.Name == FatherElenetName)
                {
                    if (element.Attributes.Count < AttributeIndex)
                    {
                        return null;
                    }
                    if (element.Attributes[AttributeIndex].Value == AttributeName)
                    {
                        XmlNodeList childNodes = element.ChildNodes;
                        if (childNodes.Count > 0)
                        {
                            for (int i = 0; (i < ArrayLength) & (i < childNodes.Count); i++)
                            {
                                list.Add(childNodes[i].InnerText);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public static ArrayList GetSubElementByAttribute(string XmlPath, string FatherElement, string AttributeName, string AttributeValue, int ArrayLength)
        {
            ArrayList list = new ArrayList();
            XmlDocument document = new XmlDocument();
            document.Load(XmlPath);
            XmlNodeList childNodes = document.DocumentElement.SelectNodes("//" + FatherElement + "[" + AttributeName + "='" + AttributeValue + "']").Item(0).ChildNodes;
            for (int i = 0; (i < ArrayLength) & (i < childNodes.Count); i++)
            {
                list.Add(childNodes.Item(i).InnerText);
            }
            return list;
        }

        public string GetValue(string path)
        {
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {
                if (reader.Name == "value")
                {
                    reader.ReadString();
                }
            }
            reader.Close();
            return "";
        }

        public static DataSet GetXml(string XmlPath)
        {
            DataSet set = new DataSet();
            set.ReadXml(XmlPath);
            return set;
        }

        public static DataSet GetXmlData(string xmlPath, string XmlPathNode)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            DataSet set = new DataSet();
            StringReader reader = new StringReader(document.SelectSingleNode(XmlPathNode).OuterXml);
            set.ReadXml(reader);
            return set;
        }

        public static string ReadXmlReturnNode(string XmlPath, string Node)
        {
            XmlDocument document = new XmlDocument();
            document.Load(XmlPath);
            string str = "";
            try
            {
                str = document.GetElementsByTagName(Node).Item(0).InnerText.ToString();
            }
            catch
            {
                return (str = "");
            }
            return str;
        }

        public static void XmlInsertElement(string xmlPath, string MainNode, string Element, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            XmlNode node = document.SelectSingleNode(MainNode);
            XmlElement newChild = document.CreateElement(Element);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
            document.Save(xmlPath);
        }

        public static void XmlInsertElement(string xmlPath, string MainNode, string Element, string Attrib, string AttribContent, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            XmlNode node = document.SelectSingleNode(MainNode);
            XmlElement newChild = document.CreateElement(Element);
            newChild.SetAttribute(Attrib, AttribContent);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
            document.Save(xmlPath);
        }

        public static void XmlInsertElement(string xmlPath, string MainNode, string Element, string Attrib1, string AttribContent1, string Attrib2, string AttribContent2, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            XmlNode node = document.SelectSingleNode(MainNode);
            XmlElement newChild = document.CreateElement(Element);
            newChild.SetAttribute(Attrib1, AttribContent1);
            newChild.SetAttribute(Attrib2, AttribContent2);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
            document.Save(xmlPath);
        }

        public static void XmlInsertElement(string xmlPath, string MainNode, string Element, string Attrib1, string AttribContent1, string Attrib2, string AttribContent2, string Attrib3, string AttribContent3, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            XmlNode node = document.SelectSingleNode(MainNode);
            XmlElement newChild = document.CreateElement(Element);
            newChild.SetAttribute(Attrib1, AttribContent1);
            newChild.SetAttribute(Attrib2, AttribContent2);
            newChild.SetAttribute(Attrib3, AttribContent3);
            newChild.InnerText = Content;
            node.AppendChild(newChild);
            document.Save(xmlPath);
        }

        public static void XmlInsertNode(string xmlPath, string MailNode, string ChildNode, string Element, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            XmlNode node = document.SelectSingleNode(MailNode);
            XmlElement newChild = document.CreateElement(ChildNode);
            node.AppendChild(newChild);
            XmlElement element2 = document.CreateElement(Element);
            element2.InnerText = Content;
            newChild.AppendChild(element2);
            document.Save(xmlPath);
        }

        public static void XmlInsertNode(string path, string MainNode, string ChildNode, string Element1, string Content1, string Element2, string Content2)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode node = document.SelectSingleNode(MainNode);
            XmlElement newChild = document.CreateElement(ChildNode);
            XmlElement element2 = document.CreateElement(Element1);
            element2.InnerText = Content1;
            newChild.AppendChild(element2);
            XmlElement element3 = document.CreateElement(Element2);
            element3.InnerText = Content2;
            newChild.AppendChild(element3);
            node.AppendChild(newChild);
            document.Save(path);
        }

        public static void XmlNodeDelete(string xmlPath, string Node)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            string xpath = Node.Substring(0, Node.LastIndexOf("/"));
            document.SelectSingleNode(xpath).RemoveChild(document.SelectSingleNode(Node));
            document.Save(xmlPath);
        }

        public static void XmlNodeReplace(string xmlPath, string Node, string Content)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xmlPath);
            document.SelectSingleNode(Node).InnerText = Content;
            document.Save(xmlPath);
        }

        public void XmlSave(string path)
        {
            this.xmlDoc.Save(path);
        }
    }
}

