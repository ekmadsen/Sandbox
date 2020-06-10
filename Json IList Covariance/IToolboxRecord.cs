using System.Collections.Generic;


namespace ErikTheCoder.Sandbox.Covariance
{
    public interface IToolboxRecord
    {
        IList<ISprocketRecord> Sprockets { get; set; }
        IDictionary<Orientation, IWidgetRecord> Widgets { get; set; }
        //IDictionary<Orientation, IList<IThingamajigRecord>> Thingamajigs { get; set; }
    }
}
