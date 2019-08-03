using System;
using System.Diagnostics;
using System.IO;


namespace ErikTheCoder.Sandbox.XmlParserCoreRt
{
    public static class Program
    {
        public static void Main()
        {
            const string inputFilename = @"C:\Users\Erik\Documents\Temp\LargeDocument.xml";
            const string xPath = @"/icjzlzuydtq/foo/bar/baz";
            if (!Stopwatch.IsHighResolution) throw new Exception("Stopwatch is not high resolution.");
            long startTicks = Stopwatch.GetTimestamp();
            CharParser parser = new CharParser();
            int nodes = parser.CountNodes(inputFilename, xPath);
            long stopTicks = Stopwatch.GetTimestamp();
            long durationTicks = stopTicks - startTicks;
            Console.WriteLine($"Duration = {durationTicks} ticks.");
            Console.WriteLine($"Timer frequency is {Stopwatch.Frequency} ticks per second.");
            int fileSizeMb = (int)((new FileInfo(inputFilename)).Length / (1024d * 1024d));
            Console.WriteLine($"Found {nodes} nodes.");
            Console.WriteLine($"Parsing of {fileSizeMb} MB file took {TimeSpan.FromTicks(durationTicks).TotalSeconds:0.000} seconds.");
        }
    }
}
