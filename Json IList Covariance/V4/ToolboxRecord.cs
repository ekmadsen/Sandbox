using System.Collections.Generic;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance.V4
{
    internal class ToolboxRecord : IToolboxRecord
    {
        [JsonConverter(typeof(ListConverter<ISprocketRecord, SprocketRecord>))]
        public IList<ISprocketRecord> Sprockets { get; set; }


        [JsonConverter(typeof(DictionaryConverter<Orientation, IWidgetRecord, Orientation, WidgetRecord>))]
        public IDictionary<Orientation, IWidgetRecord> Widgets { get; set; }


        [JsonConverter(typeof(DictionaryOfListsConverter<Orientation, IThingamajigRecord, Orientation, ThingamajigRecord>))]
        public IDictionary<Orientation, IList<IThingamajigRecord>> Thingamajigs { get; set; }


        public ToolboxRecord()
        {
            Sprockets = new List<ISprocketRecord>();
            Widgets = new Dictionary<Orientation, IWidgetRecord>();
            Thingamajigs = new Dictionary<Orientation, IList<IThingamajigRecord>>();
        }
    }
}