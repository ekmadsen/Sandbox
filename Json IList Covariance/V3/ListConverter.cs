// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance.V3
{
    public class ListConverter<I, T> : JsonConverter where T : I
    {
        public override bool CanConvert(Type ObjectType) => true;


        public override object ReadJson(JsonReader Reader, Type ObjectType, object ExistingValue, JsonSerializer Serializer)
        {
            // Deserialize as type.
            var list = Serializer.Deserialize<List<T>>(Reader);
            if (list == null) return null;
            // Convert to interface.
            var returnList = new List<I>();
            foreach (var item in list) returnList.Add(item);
            return returnList;
        }


        public override void WriteJson(JsonWriter Writer, object Value, JsonSerializer Serializer) => Serializer.Serialize(Writer, Value);
    }
}