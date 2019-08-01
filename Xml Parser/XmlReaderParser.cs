using System.IO;
using System.Xml;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XmlReaderParser : IParser
    {
        private const int _maxDepth = 16;


        public int CountNodes(string Filename, string XPath)
        {
            int count = 0;
            string[] xPathNames = XPath.StartsWith('/')
                ? XPath.Substring(1).Split('/')
                : XPath.Split('/');
            int depth = 0;
            int xPathDepth = xPathNames.Length - 1;
            string[] elementNames = new string[_maxDepth];
            using (FileStream fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (XmlReader xmlReader = XmlReader.Create(fileStream))
                {
                    while (xmlReader.Read())
                    {
                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                elementNames[depth] = xmlReader.LocalName;
                                if ((depth == xPathDepth) && PathsMatch(elementNames, depth, xPathNames)) count++;
                                depth++;
                                break;
                            case XmlNodeType.EndElement:
                                depth--;
                                break;
                        }
                    }
                }
            }
            return count;
        }


        // ReSharper disable SuggestBaseTypeForParameter
        private static bool PathsMatch(string[] ElementNames, int Depth, string[] XPathNames)            
        {
            for (int index = 0; index <= Depth; index++) if (ElementNames[index] != XPathNames[index]) return false;
            return true;
        }
        // ReSharper restore SuggestBaseTypeForParameter
    }
}
