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
            var version = int.Parse(Arguments[0]);
            // Create and populate toolbox record.
            var (toolboxRecord, jsonSerializerSettings) = CreateToolboxRecord(version);
            var toolboxType = toolboxRecord.GetType();
            PopulateToolboxRecord(toolboxRecord);
            // Serialize toolbox record as JSON and save to local disk.
            var jsonToWrite = JsonConvert.SerializeObject(toolboxRecord, jsonSerializerSettings);
            File.WriteAllText(_filename, jsonToWrite);
            // Read JSON from local disk and de-serialize to toolbox record.
            var jsonRead = File.ReadAllText(_filename);
            try
            {
                toolboxRecord = (IToolboxRecord)JsonConvert.DeserializeObject(jsonRead, toolboxType, jsonSerializerSettings);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to de-serialize JSON to {nameof(IToolboxRecord)}.", exception);
            }
            if (toolboxRecord == null)
            {
                Console.WriteLine($"{nameof(toolboxRecord)} is null.");
                return;
            }
            Console.WriteLine($"Successfully deserialized JSON from {_filename} to {toolboxType} in {nameof(toolboxRecord)} variable.");
            Console.WriteLine($"Toolbox record has {toolboxRecord.Sprockets.Count} sprocket records.");
            Console.WriteLine($"Toolbox record has {toolboxRecord.Widgets.Count} widget records.");
            //Console.WriteLine($"Toolbox record has {toolboxRecordRead.Thingamajig.Count} thingamajig records.");
        }


        private static (IToolboxRecord ToolboxRecord, JsonSerializerSettings JsonSerializerSettings) CreateToolboxRecord(int Version)
        {
            return Version switch
            {
                1 => (new V1.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented }),
                2 => (new V2.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.Auto }),
                3 => (new V3.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented }),
                4 => (new V4.ToolboxRecord(), new JsonSerializerSettings { Formatting = Formatting.Indented }),
                _ => throw new NotImplementedException($"Version {Version} not implemented.")
            };
        }


        private static void PopulateToolboxRecord(IToolboxRecord ToolboxRecord)
        {
            // Add sprockets to list.
            ToolboxRecord.Sprockets.Add(new SprocketRecord { Foo = "Blah", Bar = 99 });
            ToolboxRecord.Sprockets.Add(new SprocketRecord { Foo = "Yada", Bar = 101 });
            // Add widgets to dictionary mapping orientation to widget.
            ToolboxRecord.Widgets.Add(Orientation.Straight, new WidgetRecord { Orientation = Orientation.Straight, Baz = true, Zot = 202.22d });
            ToolboxRecord.Widgets.Add(Orientation.RightAngle, new WidgetRecord { Orientation = Orientation.RightAngle, Baz = false, Zot = 303.33d });
            // Add thingamajigs to dictionary mapping orientation to a list of thingamajigs.
            IThingamajigRecord thing1 = new ThingamajigRecord  { Orientation = Orientation.Straight, Frob = DateTime.Parse("1/1/1970"), Bork = 404L };
            IThingamajigRecord thing2 = new ThingamajigRecord { Orientation = Orientation.Straight, Frob = DateTime.Parse("2/2/1980"), Bork = 505L };
            ToolboxRecord.Thingamajigs.Add(Orientation.Straight, new List<IThingamajigRecord>{ thing1, thing2 });
            IThingamajigRecord thing3 = new ThingamajigRecord { Orientation = Orientation.RightAngle, Frob = DateTime.Parse("3/3/1990"), Bork = 606L };
            IThingamajigRecord thing4 = new ThingamajigRecord { Orientation = Orientation.RightAngle, Frob = DateTime.Parse("4/4/2000"), Bork = 707L };
            ToolboxRecord.Thingamajigs.Add(Orientation.RightAngle, new List<IThingamajigRecord> { thing3, thing4 });
        }
    }
}
