﻿using System;
using System.Collections.Generic;
using System.IO;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public class XmlGenerator
    {
        private const int _elementNameMinLength = 4;
        private const int _elementNameMaxLength = 12;
        private const int _elementValueMinWords = 1;
        private const int _elementValueMaxWords = 8;
        private const int _wordMinLength = 1;
        private const int _wordMaxLength = 12;
        private const int _indentSpaces = 4;
        private readonly IThreadsafeRandom _random;
        private readonly Stack<string> _elementNames;
        private readonly int[] _actionPercentages;
        private readonly char[] _lowercaseChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        

        public XmlGenerator(IThreadsafeRandom Random)
        {
            _random = Random;
            _elementNames = new Stack<string>();
            _actionPercentages = new[] {60, 80}; // Close, open, write element.
        }


        public void CreateFile(string Filename, int FileSizeMb)
        {
			Console.Write("Creating file... ");
            var targetFileSizeBytes = FileSizeMb * 1024 * 1024;
            using (var fileStream = File.Open(Filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    // Open root element.
                    var fileSizeBytes = 0;
                    OpenElement(streamWriter, ref fileSizeBytes, false);
                    while (fileSizeBytes < targetFileSizeBytes)
                    {
                        // Determine action.
                        var actionPercent = _random.Next(100);
                        if ((actionPercent < _actionPercentages[0]) && (_elementNames.Count > 1)) CloseElement(streamWriter, ref fileSizeBytes);
                        else if (actionPercent < _actionPercentages[1]) OpenElement(streamWriter, ref fileSizeBytes);
                        else WriteElement(streamWriter, ref fileSizeBytes);
                    }
                    // Close all elements.
                    CloseAllElements(streamWriter, ref fileSizeBytes);
                }
            }
		}


        private void WriteElement(TextWriter StreamWriter, ref int FileSizeBytes)
        {
            StreamWriter.WriteLine();
            FileSizeBytes += 2;  // Carriage Return + Line Feed
            var elementName = GetRandomText(_elementNameMinLength, _elementNameMaxLength);
            var indent = _elementNames.Count * _indentSpaces;
            var xml = $"{new string(' ', indent)}<{elementName}>";
            StreamWriter.Write(xml);
            FileSizeBytes += xml.Length;
            var words = _random.Next(_elementValueMinWords, _elementValueMaxWords);
            while (words > 0)
            {
                var space = words == 1 ? "" : " ";
                xml = $"{GetRandomText(_wordMinLength, _wordMaxLength)}{space}";
                StreamWriter.Write(xml);
                FileSizeBytes += xml.Length;
                words--;
            }
            xml = $"</{elementName}>";
            StreamWriter.Write(xml);
            FileSizeBytes += xml.Length;
        }


        private void OpenElement(TextWriter StreamWriter, ref int FileSizeBytes, bool NewLine = true)
        {
            if (NewLine)
            {
                StreamWriter.WriteLine();
                FileSizeBytes += 2; // Carriage Return + Line Feed
            }
            var elementName = GetRandomText(_elementNameMinLength, _elementNameMaxLength);
            var indent = _elementNames.Count * _indentSpaces;
            _elementNames.Push(elementName);
            var xml = $"{new string(' ', indent)}<{elementName}>";
            StreamWriter.Write(xml);
            FileSizeBytes += xml.Length;
            // Guarantee element contains at least one child element with content.
            WriteElement(StreamWriter, ref FileSizeBytes);
        }


        private void CloseElement(TextWriter StreamWriter, ref int FileSizeBytes)
        {
            StreamWriter.WriteLine();
            FileSizeBytes += 2; // Carriage Return + Line Feed
            var elementName = _elementNames.Pop();
            var indent = _elementNames.Count * _indentSpaces;
            var xml = $"{new string(' ', indent)}</{elementName}>";
            StreamWriter.Write(xml);
            FileSizeBytes += xml.Length;
        }


        private void CloseAllElements(TextWriter StreamWriter, ref int FileSizeBytes)
        {
            while (_elementNames.Count > 0) CloseElement(StreamWriter, ref FileSizeBytes);
        }


        private string GetRandomText(int MinLength, int MaxLength)
        {
            var length = _random.Next(MinLength, MaxLength + 1);
            var textChars = new char[length];
            for (var index = 0; index < textChars.Length; index++) textChars[index] = _lowercaseChars[_random.Next(_lowercaseChars.Length)];
            return new string(textChars);
        }
    }
}
