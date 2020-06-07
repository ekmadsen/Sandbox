// ReSharper disable MemberCanBePrivate.Global
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            string filename = GetFilename();
            if (File.Exists(filename)) throw new Exception($"{filename} file already exists.");
            // Add logic here that auto-increments IDs.  For this demo, just hard-code a new ID.
            Id = 1;
            UpdateInternal();
        }


        protected override void ReadInternal()
        {
            // Deserialize team record from JSON saved in text file.
            string filename = GetFilename();
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) throw new FileNotFoundException(filename);
            string json = File.ReadAllText(filename);
            if (string.IsNullOrWhiteSpace(json)) throw new FileLoadException(filename);
            Record = JsonConvert.DeserializeObject<TeamRecord>(json, _jsonSerializerSettings);
            Initialize();
        }


        protected override void UpdateInternal()
        {
            // Serialize to JSON and save to text file.
            string json = JsonConvert.SerializeObject(Record, Record.GetType(), _jsonSerializerSettings);
            string filename = GetFilename();
            File.WriteAllText(filename, json);
        }


        protected override void DeleteInternal()
        {
            string filename = GetFilename();
            if (File.Exists(filename)) File.Delete(filename);
        }


        private string GetFilename() => $@"C:\Users\Erik\Documents\Temp\Team{Id}.json";


        // By default, Json.NET serializes public properties.
        // This class instructs JSON.NET to serialize public fields instead.
        private class FieldContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type Type, MemberSerialization MemberSerialization)
            {
                List<JsonProperty> jsonProperties = new List<JsonProperty>();
                FieldInfo[] fields = Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    JsonProperty jsonProperty = base.CreateProperty(field, MemberSerialization);
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