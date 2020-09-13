using System.Collections.Generic;
using System.Diagnostics;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class Test3
    {
        private static Stopwatch _stopwatch;
        private static Dictionary<string, Widget> _widgets;


        public static void Run(Stopwatch Stopwatch, int InitialInput)
        {
            _stopwatch = Stopwatch;
            _widgets = new Dictionary<string, Widget>
            {
                {"A", new Widget(_stopwatch, "A", 1)},
                {"B", new Widget(_stopwatch, "B", 2)},
                {"C", new Widget(_stopwatch, "C", 3)},
                {"D", new Widget(_stopwatch, "D", 4)},
                {"E", new Widget(_stopwatch, "E", 5)},
                {"F", new Widget(_stopwatch, "F", 6)},
                {"G", new Widget(_stopwatch, "G", 7)},
                {"H", new Widget(_stopwatch, "H", 8)},
                {"I", new Widget(_stopwatch, "I", 9)},
                {"J", new Widget(_stopwatch, "J", 10)},
                {"K", new Widget(_stopwatch, "K", 11)},
                {"L", new Widget(_stopwatch, "L", 12)},
                {"M", new Widget(_stopwatch, "M", 13)}
            };
            foreach (var widget in _widgets.Values)
            {
                // Use initial input to frob the widget.
                var frobValue = widget.Frob3(InitialInput);
                // Use the output of the widget's frob method to bork the widget.
                var borkValue = widget.Bork3(frobValue);
                // Use the output of the widget's bork method to zap the widget.
                widget.Zap3(borkValue);
            }
        }
    }
}
