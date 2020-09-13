using System;
using System.IO;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class CharParser : IParser
    {
        private const int _maxDepth = 16;
        private const int _elementNameMaxLength = 16;
		private readonly char[][] _buffer;
        private readonly int[] _elementNameLengths;


		public CharParser()
		{
            _buffer = new char[_maxDepth][];
            _elementNameLengths = new int[_maxDepth];
            for (var depth = 0; depth < _maxDepth; depth++) _buffer[depth] = new char[_elementNameMaxLength];
		}


		public int CountNodes(string Filename, string XPath)
        {
            var count = 0;
            var xPathNames = XPath.StartsWith('/')
				? XPath.Substring(1).Split('/')
				: XPath.Split('/');
            var depth = 0;
            var xPathDepth = xPathNames.Length - 1;
			using (var fileStream = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    do
                    {
                        var elementNameLength = GetNextElementName(streamReader, depth);
                        if (elementNameLength < 0) return count; // End of stream.
                        if (_buffer[depth][0] == '/')
                        {
                            // End element
                            depth--;
                            continue;
                        }
                        _elementNameLengths[depth] = elementNameLength;
                        if ((depth == xPathDepth) && PathsMatch(xPathNames)) count++;
                        depth++;
                    } while (true);
                }
            }
        }


        private int GetNextElementName(StreamReader StreamReader, int Depth)
        {
            var inElementName = false;
            var index = 0;
            while (!StreamReader.EndOfStream)
            {
                var character = (char) StreamReader.Read();
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
        private bool PathsMatch(string[] XPathNames)
        {
            for (var depth = 0; depth < XPathNames.Length; depth++)
            {
                var length = Math.Min(_elementNameLengths[depth], XPathNames[depth].Length);
                for (var index = 0; index < length; index++) if (_buffer[depth][index] != XPathNames[depth][index]) return false;
            }
            return true;
        }
        // ReSharper restore SuggestBaseTypeForParameter
    }
}
