//using System;
//using System.Collections.Generic;
//using System.IO;
//using ErikTheCoder.Utilities;
//using Newtonsoft.Json;


//namespace ErikTheCoder.Sandbox.Baseball.Library.V1
//{
//    internal class TeamFile : TeamBase
//    {
//        private readonly JsonSerializerSettings _jsonSerializerSettings;


//        public TeamFile()
//        {
//            _jsonSerializerSettings = new JsonSerializerSettings
//            {
//                TypeNameHandling = TypeNameHandling.Auto, // Enable serialization of interfaces.
//                PreserveReferencesHandling = PreserveReferencesHandling.All // Enable circular references in object graph.
//            };
//            AssistantCoaches = new List<ICoach>();
//            Players = new List<IPlayer>();
//        }


//        protected override void CreateInternal()
//        {
//            var filename = GetFilename();
//            if (File.Exists(filename)) throw new Exception($"{filename} file already exists.");
//            // Add logic here that auto-increments IDs.  For this demo, just hard-code a new ID.
//            Id = 1;
//            UpdateInternal();
//        }


//        protected override void ReadInternal()
//        {
//            // Deserialize team from JSON saved in text file.
//            var filename = GetFilename();
//            if (filename.IsNullOrEmpty() || !File.Exists(filename)) throw new FileNotFoundException(filename);
//            var json = File.ReadAllText(filename);
//            if (json.IsNullOrWhiteSpace()) throw new FileLoadException(filename);
//            var team = JsonConvert.DeserializeObject<TeamFile>(json, _jsonSerializerSettings);
//            if (team == null) throw new FileLoadException(filename);
//            Id = team.Id;
//            Name = team.Name;
//            Players = team.Players;
//            HeadCoach = team.HeadCoach;
//            AssistantCoaches = team.AssistantCoaches;
//        }


//        protected override void UpdateInternal()
//        {
//            // Serialize to JSON and save to text file.
//            var json = JsonConvert.SerializeObject(this, GetType(), _jsonSerializerSettings);
//            var filename = GetFilename();
//            File.WriteAllText(filename, json);
//        }


//        protected override void DeleteInternal()
//        {
//            var filename = GetFilename();
//            if (File.Exists(filename)) File.Delete(filename);
//        }


//        private string GetFilename() => $@"C:\Users\Erik\Documents\Temp\Team{Id}.json";
//    }
//}