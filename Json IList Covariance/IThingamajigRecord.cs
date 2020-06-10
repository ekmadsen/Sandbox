using System;


namespace ErikTheCoder.Sandbox.Covariance
{
    public interface IThingamajigRecord
    {
        Orientation Orientation { get; set; }
        DateTime Frob { get; set; }
        long Bork { get; set; }
    }
}