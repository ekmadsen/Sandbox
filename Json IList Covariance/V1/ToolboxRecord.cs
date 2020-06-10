using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Covariance.V1
{
    internal class ToolboxRecord : IToolboxRecord
    {
        public IList<ISprocketRecord> Sprockets { get; set; }
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
