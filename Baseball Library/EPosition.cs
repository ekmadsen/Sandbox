using System.ComponentModel;


namespace ErikTheCoder.Sandbox.Baseball.Library
{
    public enum EPosition
    {
        // Integers correspond to position numbers in traditional scorekeeping.
        [Description("P")]
        Pitcher = 1,

        [Description("C")]
        Catcher,

        [Description("1B")]
        FirstBase,

        [Description("2B")]
        SecondBase,

        [Description("3B")]
        ThirdBase,

        [Description("SS")]
        Shortstop,

        [Description("LF")]
        LeftField,

        [Description("CF")]
        CenterField,

        [Description("RF")]
        RightField
    }
}
