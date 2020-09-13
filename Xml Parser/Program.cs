using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.XmlParser
{
    public static class Program
    {
        public static void Main(string[] Arguments)
        {
            try
            {
                Run(Arguments);
            }
            catch (Exception exception) 
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
        }


        private static void Run(IReadOnlyList<string> Arguments)
        {
            var (inputFilename, xPath, parsingTechnique, outputFilename, fileSizeMb) = ParseCommandLine(Arguments);
            IThreadsafeRandom random = new ThreadsafeRandom();
            var stopwatch = Stopwatch.StartNew();
            if (inputFilename != null)
            {
                var parser = GetParser(parsingTechnique.Value);
                var nodes = parser.CountNodes(inputFilename, xPath);
                stopwatch.Stop();
                fileSizeMb = (int?)((new FileInfo(inputFilename)).Length / (1024d * 1024d));
                Console.WriteLine($"Found {nodes} nodes.");
                Console.WriteLine($"Parsing of {fileSizeMb.Value} MB file took {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
            }
            if (outputFilename != null)
            {
                var xmlGenerator = new XmlGenerator(random);
                xmlGenerator.CreateFile(outputFilename, fileSizeMb.Value);
                stopwatch.Stop();
                Console.WriteLine($"Creation of {fileSizeMb.Value} MB file took {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
            }
        }


        private static (string InputFilename, string XPath, ParsingTechnique? ParsingTechnique, string OutputFilename, int? FileSizeMb) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            // Retrieve arguments.
            string inputFilename = null;
            ParsingTechnique? parsingTechnique = null;
            string xPath = null;
            string outputFilename = null;
            int? fileSizeMb = null;
            if (Arguments.Count % 2 != 0) throw new ArgumentException("Invalid number of arguments.  Arguments must be passed in a pair: -argumentName argumentValue or /argumentName argumentValue.");
            for (var index = 0; index < Arguments.Count; index++)
            {
                var argumentName = Arguments[index];
                index++;
                var argumentValue = Arguments[index];
                switch (argumentName?.ToLower())
                {
                    case "-i":
                    case "/i":
                    case "-input":
                    case "/input":
                        inputFilename = argumentValue;
                        break;
                    case "-x":
                    case "/x":
                    case "-xpath":
                    case "/xpath":
                        xPath = argumentValue;
                        break;
                    case "-p":
                    case "/p":
                    case "-parser":
                    case "/parser":
                        parsingTechnique = Enum.Parse<ParsingTechnique>(argumentValue, true);
                        break;
                    case "-o":
                    case "/o":
                    case "-output":
                    case "/output":
                        outputFilename = argumentValue;
                        break;
                    case "-s":
                    case "/s":
                    case "-size":
                    case "/size":
                        fileSizeMb = int.Parse(argumentValue);
                        break;
                    default:
                        throw new ArgumentException($"{argumentName} not supported.");
                }
            }
            // Validate arguments.
            if ((inputFilename is null) && (outputFilename is null)) throw new ArgumentException("Specify an input filename or output filename.");
            if ((inputFilename != null) && (xPath is null)) throw new ArgumentException("When specifying an input filename also specify XPath via -x argument.");
            if ((inputFilename != null) && !parsingTechnique.HasValue) throw new ArgumentException("When specifying an input filename also specify a parser via -p argument.");
            if ((outputFilename != null) && !fileSizeMb.HasValue) throw new ArgumentException("When specifying an output filename also specify a file size in MB via -s argument.");
            return (inputFilename, xPath, parsingTechnique, outputFilename, fileSizeMb);
        }


        private static IParser GetParser(ParsingTechnique ParsingTechnique)
        {
            return ParsingTechnique switch
            {
                ParsingTechnique.XmlDocument => new XmlDocumentParser(),
                ParsingTechnique.XPathDocument => new XPathDocumentParser(),
                ParsingTechnique.XmlReader => new XmlReaderParser(),
                ParsingTechnique.Char => new CharParser(),
                _ => throw new NotImplementedException($"{ParsingTechnique} parsing technique not supported.")
            };
        }
    }
}
