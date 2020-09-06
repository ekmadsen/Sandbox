using System.IO;
using System.Xml;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XmlDocumentParser : IParser
    {
        public int CountNodes(string Filename, string XPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                xmlDocument.Load(fileStream);
                // ReSharper disable once PossibleNullReferenceException
                return xmlDocument.SelectNodes(XPath).Count;
            }
        }
    }
}
