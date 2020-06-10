using System;
using System.Collections.Generic;
using System.IO;
using ErikTheCoder.Utilities;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance
{
    public static class Program
    {
        private const string _filename = @"C:\Users\Erik\Documents\Temp\Toolbox.json";


        public static void Main(string[] Arguments)
        {
            try
            {
                Console.WriteLine();
                if ((Arguments == null) || (Arguments.Length == 0)) throw new ArgumentException($"{nameof(Arguments)} must specify an integer version.", nameof(Arguments));
                Run(Arguments);
            }
            catch (Exception exception)
            {
                ThreadsafeConsole.WriteLine(exception.GetSummary(true, true), ConsoleColor.Red);
            }
            finally
            {
                Console.WriteLine();
            }
        }


        private static void Run(IReadOnlyList<string> Arguments)
        {
            // Run a particular version of the code.
            int version = int.Parse(Arguments[0]);
            // Create and populate toolbox record.
            (IToolboxRecord toolboxRecordToWrite, JsonSerializerSettings jsonSerializerSettings) = CreateToolboxRecord(version);
            Type toolboxType = toolboxRecordToWrite.GetType();
            PopulateToolboxRecord(toolboxRecordToWrite);
            // Serialize toolbox record as JSON and save to local disk.
            string jsonToWrite = JsonConvert.SerializeObject(toolboxRecordToWrite, jsonSerializerSettings);
            File.WriteAllText(_filename, jsonToWrite);
            // Read JSON from local disk and de-serialize to toolbox record.
            string jsonRead = File.ReadAllText(_filename);
            IToolboxRecord toolboxRecordRead;
            try
            {
                toolboxRecordRead = (IToolboxRecord)JsonConvert.DeserializeObject(jsonRead, toolboxType, jsonSerializerSettings);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to de-serialize JSON to {nameof(IToolboxRecord)}.", exception);
            }
            if (toolboxRecordRead == null)
            {
                Console.WriteLine($"{nameof(toolboxRecordRead)} is null.");
                return;
            }
            Console.WriteLine($"Successfully deserialized JSON from {_filename} to {toolboxType} in {nameof(toolboxRecordRead)} variable.");
            Console.WriteLine($"Toolbox record has {toolboxRecordRead.Sprockets.Count} sprocket records.");
            Console.WriteLine($"Toolbox record has {toolboxRecordRead.Widgets.Count} widget records.");
            //Console.WriteLine($"Toolbox record has {toolboxRecordRead.Thingamajig.Count} thingamajig records.");
        }


        private static (IToolboxRecord ToolboxRecord, JsonSerializerSettings JsonSerializerSettings) CreateToolboxRecord(int Version)
        {
            return Version switch
            {
                1 => (new V1.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented }),
                2 => (new V2.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.Auto}),
                3 => (new V3.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented}),
                _ => throw new NotImplementedException($"Version {Version} not implemented.")
            };
        }


        private static void PopulateToolboxRecord(IToolboxRecord ToolboxRecord)
        {
            // Add sprockets to list.
            ToolboxRecord.Sprockets.Add(new SprocketRecord {Foo = "Blah", Bar = 99});
            ToolboxRecord.Sprockets.Add(new SprocketRecord {Foo = "Yada", Bar = 101});
            // Add widgets to dictionary mapping orientation to widget.
            ToolboxRecord.Widgets.Add(Orientation.Straight, new WidgetRecord { Baz = true, Zot = 202.22d });
            ToolboxRecord.Widgets.Add(Orientation.RightAngle, new WidgetRecord { Baz = false, Zot = 303.33d });
            // Add thingamajigs to dictionary mapping orientation to a list of thingamajigs.
        }
    }
}
