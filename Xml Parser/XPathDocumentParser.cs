using System.IO;
using System.Xml.XPath;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XPathDocumentParser : IParser
    {
        public int CountNodes(string Filename, string XPath)
        {
            var count = 0;
            using (var fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xPathDocument = new XPathDocument(fileStream);
                var xPathNavigator = xPathDocument.CreateNavigator();
                // ReSharper disable once PossibleNullReferenceException
                var xPathNodeIterator = xPathNavigator.Select(XPath);
                while (xPathNodeIterator.MoveNext()) count++;
                return count;
            }
        }
    }
}