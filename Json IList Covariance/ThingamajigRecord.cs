using System;


namespace ErikTheCoder.Sandbox.Covariance
{
    internal class ThingamajigRecord : IThingamajigRecord
    {
        public Orientation Orientation { get; set; }
        public DateTime Frob { get; set; }
        public long Bork { get; set; }
    }
}
