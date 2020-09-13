// ReSharper disable MemberCanBePrivate.Global
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ErikTheCoder.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace ErikTheCoder.Sandbox.Baseball.Library.V2
{
    internal class TeamFile : TeamBase
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;


        public TeamFile() : this(new TeamRecord())
        {
        }


        public TeamFile(TeamRecord Record) : base(Record)
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto, // Enable serialization of interfaces.
                PreserveReferencesHandling = PreserveReferencesHandling.All, // Enable circular references in object graph.
                ContractResolver = new FieldContractResolver() // Serialize public fields instead of public properties.
            };
        }


        protected override void CreateInternal()
        {
            var filename = GetFilename();
            if (File.Exists(filename)) throw new Exception($"{filename} file already exists.");
            // Add logic here that auto-increments IDs.  For this demo, just hard-code a new ID.
            Id = 1;
            UpdateInternal();
        }


        protected override void ReadInternal()
        {
            // Deserialize team record from JSON saved in text file.
            var filename = GetFilename();
            if (filename.IsNullOrEmpty() || !File.Exists(filename)) throw new FileNotFoundException(filename);
            var json = File.ReadAllText(filename);
            if (json.IsNullOrWhiteSpace()) throw new FileLoadException(filename);
            Record = JsonConvert.DeserializeObject<TeamRecord>(json, _jsonSerializerSettings);
            Initialize();
        }


        protected override void UpdateInternal()
        {
            // Serialize to JSON and save to text file.
            var json = JsonConvert.SerializeObject(Record, Record.GetType(), _jsonSerializerSettings);
            var filename = GetFilename();
            File.WriteAllText(filename, json);
        }


        protected override void DeleteInternal()
        {
            var filename = GetFilename();
            if (File.Exists(filename)) File.Delete(filename);
        }


        private string GetFilename() => $@"C:\Users\Erik\Documents\Temp\Team{Id}.json";


        // By default, Json.NET serializes public properties.
        // This class instructs Json.NET to serialize public fields instead.
        private class FieldContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type Type, MemberSerialization MemberSerialization)
            {
                var jsonProperties = new List<JsonProperty>();
                var fields = Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var jsonProperty = base.CreateProperty(field, MemberSerialization);
                    jsonProperty.Ignored = false;
                    jsonProperty.Readable = true;
                    jsonProperty.Writable = true;
                    jsonProperties.Add(jsonProperty);
                }
                return jsonProperties;
            }
        }
    }
}