using System;
using System.IO;


namespace ErikTheCoder.Sandbox.XmlParserCoreRt
{
    public class CharParser
    {
        private const int _maxDepth = 16;
        private const int _elementNameMaxLength = 16;
		private readonly char[][] _buffer;
        private readonly int[] _elementNameLengths;


		public CharParser()
		{
            _buffer = new char[_maxDepth][];
            _elementNameLengths = new int[_maxDepth];
            for (int depth = 0; depth < _maxDepth; depth++) _buffer[depth] = new char[_elementNameMaxLength];
		}


		public int CountNodes(string Filename, string XPath)
        {
			int count = 0;
			string[] xPathNames = XPath.StartsWith('/')
				? XPath.Substring(1).Split('/')
				: XPath.Split('/');
			int depth = 0;
			int xPathDepth = xPathNames.Length - 1;
			using (FileStream fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    do
                    {
                        int elementNameLength = GetNextElementName(streamReader, depth);
                        if (elementNameLength < 0) return count; // End of stream.
                        if (_buffer[depth][0] == '/')
                        {
                            // End element
                            depth--;
                            continue;
                        }
                        _elementNameLengths[depth] = elementNameLength;
                        if ((depth == xPathDepth) && PathsMatch(xPathNames, depth)) count++;
                        depth++;
                    } while (true);
                }
            }
        }


        private int GetNextElementName(StreamReader StreamReader, int Depth)
        {
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
                    if (character == '>') return index;
                    _buffer[Depth][index] = character;
                    index++;
                }
            }
            return -1;
        }


        // ReSharper disable SuggestBaseTypeForParameter
        private bool PathsMatch(string[] XPathNames, int Depth)
        {
            for (int depth = 0; depth <= Depth; depth++)
            {
                int length = Math.Min(_elementNameLengths[depth], XPathNames[depth].Length);
                for (int index = 0; index < length; index++) if (_buffer[depth][index] != XPathNames[depth][index]) return false;
            }
            return true;
        }
        // ReSharper restore SuggestBaseTypeForParameter
    }
}
