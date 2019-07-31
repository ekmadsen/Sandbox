using System;
using System.IO;

namespace ErikTheCoder.Sandbox.XmlParser
{
    public class SpanParser : IParser
    {
        private const int _elementNameMaxLength = 16;


        public int CountNodes(string Filename, string XPath)
        {
            int count = 0;
            string[] xPathNames = XPath.StartsWith('/')
                ? XPath.Substring(1).Split('/')
                : XPath.Split('/');
            int depth = -1;
            int xPathDepth = xPathNames.Length - 1;
            int matchingElementNames = 0;
            char[] buffer = new char[_elementNameMaxLength];
            using (FileStream fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    do
                    {
                        ReadOnlySpan<char> elementName = GetNextElementName(streamReader, buffer);
                        if (elementName == ReadOnlySpan<char>.Empty) break;
                        if (elementName[0] == '/')
                        {
                            // End element
                            depth--;
                            matchingElementNames = Math.Max(--matchingElementNames, 0);
                            continue;
                        }
                        else depth++;
                        if ((depth <= xPathDepth) && ElementNamesMatch(elementName, xPathNames[depth])) matchingElementNames++;
                        if ((matchingElementNames == xPathNames.Length) && (depth == xPathDepth)) count++;
                    } while (true);
                }
            }
            return count;
        }


        private ReadOnlySpan<char> GetNextElementName(StreamReader StreamReader, char[] Buffer)
        {
            ReadOnlySpan<char> buffer = Buffer;
            bool inElementName = false;
            int index = 0;
            while (!StreamReader.EndOfStream)
            {
                char character = (char) StreamReader.Read();
                if (!inElementName && (character == '<'))
                {
                    inElementName = true;
                    continue;
                }
                if (inElementName)
                {
                    if (character == '>') return buffer.Slice(0, index);
                    Buffer[index] = character;
                    index++;
                }
            }
            return ReadOnlySpan<char>.Empty;
        }


        private static bool ElementNamesMatch(ReadOnlySpan<char> ElementName, string XPathName)
        {
            int maxLength = Math.Min(ElementName.Length, XPathName.Length);
            for (int index = 0; index < maxLength; index++) if (ElementName[index] != XPathName[index]) return false;
            return true;
        }
    }
}
