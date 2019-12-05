using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Simple.Common.Extensions
{
    public static class XmlExtension
    {
        public static string GetNodeText(this XmlNode node, string xpath)
        {
            var singleNode = node.SelectSingleNode(xpath);
            if (singleNode == null)
            {
                return string.Empty;
            }

            return singleNode.InnerText;
        }

        public static string GetNodeText(this XmlNode node, string xpath, XmlNamespaceManager nsmgr)
        {
            var singleNode = node.SelectSingleNode(xpath, nsmgr);
            if (singleNode == null)
            {
                return string.Empty;
            }

            return singleNode.InnerText;
        }

        public static List<XElement> AddElement(this List<XElement> list, string name, string value)
        {
            if (list == null)
            {
                return list;
            }
            if (string.IsNullOrEmpty(value))
            {
                return list;
            }

            list.Add(new XElement(name, value));
            return list;
        }

    }
}
