using System;
using System.IO;
using System.Xml;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XmlReaderParser : IParser
    {
        private const int _maxDepth = 16;


        public int CountNodes(string Filename, string XPath)
        {
            var count = 0;
            var xPathNames = XPath.StartsWith('/')
                ? XPath.Substring(1).Split('/')
                : XPath.Split('/');
            var depth = 0;
            var xPathDepth = xPathNames.Length - 1;
            var elementNames = new string[_maxDepth];
            using (var fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var xmlReader = XmlReader.Create(fileStream))
                {
                    while (xmlReader.Read())
                    {
                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                elementNames[depth] = xmlReader.LocalName;
                                if ((depth == xPathDepth) && PathsMatch(elementNames, xPathNames)) count++;
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
        private static bool PathsMatch(string[] ElementNames, string[] XPathNames)
        {
            var length = Math.Min(ElementNames.Length, XPathNames.Length);
            for (var index = 0; index < length; index++) if (ElementNames[index] != XPathNames[index]) return false;
            return true;
        }
        // ReSharper restore SuggestBaseTypeForParameter
    }
}
