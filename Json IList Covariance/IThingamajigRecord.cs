using System;


namespace ErikTheCoder.Sandbox.Covariance
{
    public interface IThingamajigRecord
    {
        // ReSharper disable UnusedMemberInSuper.Global
        // ReSharper disable UnusedMember.Global
        Orientation Orientation { get; set; }
        DateTime Frob { get; set; }
        long Bork { get; set; }
        // ReSharper restore UnusedMember.Global
        // ReSharper restore UnusedMemberInSuper.Global
    }
}