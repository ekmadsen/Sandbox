using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class PipelineV3 : PipelineBase
    {
        private IMathService _mathService;
        private long[] _inputValues;
        private long[] _stepValues;
        private long[][] _outputValues;
        private ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>> _step1Tasks;
        private ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>> _step2Tasks;
        private ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>> _step3Tasks;
        private ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>> _step4Tasks;


        public override async Task<long[][]> Run(IMathService MathService, long[] InputValues, long[] StepValues)
        {
            _mathService = MathService;
            _inputValues = InputValues;
            _stepValues = StepValues;
            // Create output array and thread-safe collections.
            _outputValues = new long[InputValues.Length][];
            for (var index = 0; index < _outputValues.Length; index++)
            {
                _outputValues[index] = new long[StepValues.Length];
            }
            _step1Tasks = new ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>>();
            _step2Tasks = new ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>>();
            _step3Tasks = new ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>>();
            _step4Tasks = new ConcurrentDictionary<int, Task<(int IndexValue, long OutputValue)>>();
            // Produce and consume output values.
            ProduceStep1OutputValues();
            var tasks = new List<Task>
            {
                ConsumeStep1Tasks(),
                ConsumeStep2Tasks(),
                ConsumeStep3Tasks(),
                ConsumeStep4Tasks()
            };
            await Task.WhenAll(tasks);
            // Display final results.
            DisplayValues(_inputValues, _stepValues, _outputValues);
            return _outputValues;
        }
        
        
        private void ProduceStep1OutputValues()
        {
            // Call service method asynchronously.
            for (var index = 0; index < _inputValues.Length; index++)
            {
                var localIndex = index; // Prevent lambda from capturing last index value of containing for loop.
                var inputValue = _inputValues[index];
                var stepValue = _stepValues[0];
                // Capture task.  Don't block by awaiting.
                _step1Tasks[localIndex] = AsyncHelper.MaterializeTask(async () =>
                {
                    var outputValue = await _mathService.Power(inputValue, stepValue);
                    return (localIndex, outputValue);
                });
            }
        }


        // Step 1 : Power
        private async Task ConsumeStep1Tasks()
        {
            var processedTasks = 0;
            while (processedTasks < _inputValues.Length)
            {
                if (_step1Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var task = await Task.WhenAny(_step1Tasks.Values); // This allocates a values collection each iteration.
                var (index, outputValue) = await task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                _outputValues[index][0] = outputValue;
                processedTasks++;
                // Remove from task collection so we don't consume it again.
                if (!_step1Tasks.TryRemove(index, out _)) throw new Exception($"Failed to remove task with index = {index} from {nameof(_step1Tasks)} dictionary.");
                var inputValue = outputValue;
                var stepValue = _stepValues[1];
                // Capture task.  Don't block by awaiting.
                _step2Tasks[index] = AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await _mathService.Add(inputValue, stepValue);
                    return (index, outputValue);
                });
            }
        }


        // Step 2 : Add
        private async Task ConsumeStep2Tasks()
        {
            var processedTasks = 0;
            while (processedTasks < _inputValues.Length)
            {
                if (_step2Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var task = await Task.WhenAny(_step2Tasks.Values); // This allocates a values list each iteration.
                var (index, outputValue) = await task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                _outputValues[index][1] = outputValue;
                processedTasks++;
                // Remove from task collection so we don't consume it again.
                if (!_step2Tasks.TryRemove(index, out _)) throw new Exception($"Failed to remove task with index = {index} from {nameof(_step2Tasks)} dictionary.");
                var inputValue = outputValue;
                var stepValue = _stepValues[2];
                // Capture task.  Don't block by awaiting.
                _step3Tasks[index] = AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await _mathService.Multiply(inputValue, stepValue);
                    return (index, outputValue);
                });
            }
        }


        // Step 3 : Multiply
        private async Task ConsumeStep3Tasks()
        {
            var processedTasks = 0;
            while (processedTasks < _inputValues.Length)
            {
                if (_step3Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var task = await Task.WhenAny(_step3Tasks.Values); // This allocates a values list each iteration.
                var (index, outputValue) = await task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                _outputValues[index][2] = outputValue;
                processedTasks++;
                // Remove from task collection so we don't consume it again.
                if (!_step3Tasks.TryRemove(index, out _)) throw new Exception($"Failed to remove task with index = {index} from {nameof(_step3Tasks)} dictionary.");
                var inputValue = outputValue;
                var stepValue = _stepValues[3];
                // Capture task.  Don't block by awaiting.
                _step4Tasks[index] = AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await _mathService.Modulo(inputValue, stepValue);
                    return (index, outputValue);
                });
            }
        }


        // Step 4 : Modulo
        private async Task ConsumeStep4Tasks()
        {
            var processedTasks = 0;
            while (processedTasks < _inputValues.Length)
            {
                if (_step4Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var task = await Task.WhenAny(_step4Tasks.Values); // This allocates a values list each iteration.
                var (index, outputValue) = await task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                _outputValues[index][3] = outputValue;
                processedTasks++;
                // Remove from task collection so we don't consume it again.
                if (!_step4Tasks.TryRemove(index, out _)) throw new Exception($"Failed to remove task with index = {index} from {nameof(_step3Tasks)} dictionary.");
            }
        }
    }
}
