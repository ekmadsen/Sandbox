using System;
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


        private static void Run(string[] Arguments)
        {

        }


        private static (string InputFilename, string OutputFilename) ParseCommandLine(string[] Arguments)
        {
            string inputFilename = null;
            string outputFilename = null;
            if (Arguments.Length % 2 != 0) throw new ArgumentException("Invalid number of arguments.  Arguments must be passed in a pair: -argumentName argumentValue or /argumentName argumentValue.");
            for (int index = 0; index < Arguments.Length; index++)
            {
                string argumentName = Arguments[index];
                index++;
                string argumentValue = Arguments[index];
                switch (argumentName?.ToLower())
                {
                    case "-i":
                    case "/i":
                    case "-input":
                    case "/input":
                        inputFilename = argumentValue;
                        break;
                    case "-o":
                    case "/o":
                    case "-output":
                    case "/output":
                        outputFilename = argumentValue;
                        break;
                    default:
                        throw new ArgumentException($"{argumentName} not supported.");
                }
            }
            return (inputFilename, outputFilename);
        }
    }
}
