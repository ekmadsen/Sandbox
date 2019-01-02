using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace ErikTheCoder.Sandbox.WhatRunsWhen
{
    public static class Test5
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
            Dictionary<string, Task<(string WidgetName, int FrobValue)>> frobRequests = new Dictionary<string, Task<(string, int)>>(_widgets.Count);
            foreach (Widget widget in _widgets.Values)
            {
                frobRequests[widget.Name] = widget.Frob5(InitialInput);
            }
            // Monitor frobbing.
            Dictionary<string, Task<(string WidgetName, int BorkValue)>> borkRequests = new Dictionary<string, Task<(string, int)>>(_widgets.Count);
            while (frobRequests.Count > 0)
            {
                var task = await Task.WhenAny(frobRequests.Values);
                // Remove task from frob collection.
                frobRequests.Remove(task.Result.WidgetName);
                // Use the output of the widget's frob method to bork the widget asynchronously.
                borkRequests[task.Result.WidgetName] = _widgets[task.Result.WidgetName].Bork5(task.Result.FrobValue);
            }
            // Monitor borking.
            Dictionary<string, Task<(string WidgetName, int ZapValue)>> zapRequests = new Dictionary<string, Task<(string, int)>>(_widgets.Count);
            while ((frobRequests.Count + borkRequests.Count) > 0)
            {
                if (borkRequests.Count == 0) continue;
                var task = await Task.WhenAny(borkRequests.Values);
                // Remove task from bork collection.
                borkRequests.Remove(task.Result.WidgetName);
                // Use the output of the widget's bork method to zap the widget asynchronously.
                zapRequests[task.Result.WidgetName] = _widgets[task.Result.WidgetName].Zap5(task.Result.BorkValue);
            }
            // Await zapping.
            await Task.WhenAll(zapRequests.Values);
        }
    }
}
