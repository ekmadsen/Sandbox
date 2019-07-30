using System.IO;
using System.Xml.XPath;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XPathDocumentParser : IParser
    {
        public int CountNodes(string Filename, string XPath)
        {
            int count = 0;
            using (FileStream fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XPathDocument xPathDocument = new XPathDocument(fileStream);
                XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(XPath);
                while (xPathNodeIterator.MoveNext()) count++;
                return count;
            }
        }
    }
}