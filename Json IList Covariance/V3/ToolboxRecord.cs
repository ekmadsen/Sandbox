using System.Collections.Generic;
using Newtonsoft.Json;


namespace ErikTheCoder.Sandbox.Covariance.V3
{
    internal class ToolboxRecord : IToolboxRecord
    {
        [JsonConverter(typeof(ListConverter<ISprocketRecord, SprocketRecord>))]
        public IList<ISprocketRecord> Sprockets { get; set; }


        [JsonConverter(typeof(DictionaryConverter<Orientation, IWidgetRecord, Orientation, WidgetRecord>))]
        public IDictionary<Orientation, IWidgetRecord> Widgets { get; set; }


        //public IDictionary<Orientation, IList<IThingamajigRecord>> Thingamajigs { get; set; }


        public ToolboxRecord()
        {
            Sprockets = new List<ISprocketRecord>();
            Widgets = new Dictionary<Orientation, IWidgetRecord>();
            //Thingamajigs = new Dictionary<Orientation, IList<IThingamajigRecord>>();
        }
    }
}