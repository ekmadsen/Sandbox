using System;
using System.IO;

namespace ErikTheCoder.Sandbox.XmlParser
{
    public class SpanParser : IParser
    {
		private const int _elementNameMaxLength = 16;
		private const int _maxDepth = 16;
		private readonly char[] _buffer;


		public SpanParser()
		{
			_buffer = new char[_elementNameMaxLength];
		}


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
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    do
                    {
                        Span<char> elementName = GetNextElementName(streamReader);
                        if (elementName == ReadOnlySpan<char>.Empty) break;
						if (elementName[0] == '/')
						{
							// End Element
							depth--;
							continue;
						}
						else
						{
							elementNames[depth] = new string(elementName);
							if ((depth == xPathDepth) && PathsMatch(elementNames, depth, xPathNames)) count++;
							depth++;
						}
                    } while (true);
                }
            }
            return count;
        }


        private Span<char> GetNextElementName(StreamReader StreamReader)
        {
            Span<char> buffer = _buffer;
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
                    buffer[index] = character;
                    index++;
                }
            }
            return Span<char>.Empty;
        }


		private static bool PathsMatch(string[] ElementNames, int Depth, string[] XPathNames) {
			// ReSharper restore SuggestBaseTypeForParameter
			for (int index = 0; index <= Depth; index++) if (ElementNames[index] != XPathNames[index]) return false;
			return true;
		}
	}
}
