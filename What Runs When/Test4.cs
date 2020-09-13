using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class Test4
    {
        private static Stopwatch _stopwatch;
        private static Dictionary<string, Widget> _widgets;


        public static async Task Run(Stopwatch Stopwatch, int InitialInput)
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
            // Use initial input to frob the widgets asynchronously.
            var frobRequests = new Dictionary<string, Task<int>>(_widgets.Count);
            foreach (var widget in _widgets.Values) frobRequests[widget.Name] = widget.Frob4(InitialInput);
            await Task.WhenAll(frobRequests.Values);
            // Use the output of each widget's frob method to bork the widget asynchronously.
            var borkRequests = new Dictionary<string, Task<int>>(_widgets.Count);
            foreach (var widget in _widgets.Values)
            {
                var frobValue = frobRequests[widget.Name].Result;
                borkRequests[widget.Name] = widget.Bork4(frobValue);
            }
            await Task.WhenAll(borkRequests.Values);
            // Use the output of each widget's bork method to zap the widget asynchronously.
            var zapRequests = new Dictionary<string, Task<int>>(_widgets.Count);
            foreach (var widget in _widgets.Values)
            {
                var borkValue = borkRequests[widget.Name].Result;
                zapRequests[widget.Name] = widget.Zap4(borkValue);
            }
            // Await zapping.
            await Task.WhenAll(zapRequests.Values);
        }
    }
}
