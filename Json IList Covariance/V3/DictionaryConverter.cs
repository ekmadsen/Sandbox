﻿// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance.V3
{
    public class DictionaryConverter<IKey, IValue, TKey, TValue> : JsonConverter where TKey : IKey where TValue : IValue
    {
        public override bool CanConvert(Type ObjectType) => true;


        public override object ReadJson(JsonReader Reader, Type ObjectType, object ExistingValue, JsonSerializer Serializer)
        {
            // Deserialize as type.
            var dictionary = Serializer.Deserialize<Dictionary<TKey, TValue>>(Reader);
            if (dictionary == null) return null;
            // Convert to interface.
            var returnDictionary = new Dictionary<IKey, IValue>();
            foreach (var (key, value) in dictionary) returnDictionary.Add(key, value);
            return returnDictionary;
        }


        public override void WriteJson(JsonWriter Writer, object Value, JsonSerializer Serializer) => Serializer.Serialize(Writer, Value);
    }
}