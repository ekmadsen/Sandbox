// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance.V4
{
    public class DictionaryOfListsConverter<IKey, IListValue, TKey, TListValue> : JsonConverter where TKey : IKey where TListValue : IListValue
    {
        public override bool CanConvert(Type ObjectType) => true;


        public override object ReadJson(JsonReader Reader, Type ObjectType, object ExistingValue, JsonSerializer Serializer)
        {
            // Deserialize as type.
            Dictionary<TKey, List<TListValue>> dictionary = Serializer.Deserialize<Dictionary<TKey, List<TListValue>>>(Reader);
            if (dictionary == null) return null;
            // Convert to interface.
            Dictionary<IKey, IList<IListValue>> returnDictionary = new Dictionary<IKey, IList<IListValue>>();
            foreach ((TKey key, List<TListValue> list) in dictionary)
            {
                IList<IListValue> returnList = new List<IListValue>();
                foreach (TListValue listValue in list) returnList.Add(listValue);
                returnDictionary.Add(key, returnList);
            }
            return returnDictionary;
        }


        public override void WriteJson(JsonWriter Writer, object Value, JsonSerializer Serializer) => Serializer.Serialize(Writer, Value);
    }
}