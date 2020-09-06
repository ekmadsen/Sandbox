using System.Collections.Generic;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;
using ErikTheCoder.Utilities;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class PipelineV2 : PipelineBase
    {
        public override async Task<int[][]> Run(Proxy<IMathService> MathService, int[] InputValues, int[] StepValues)
        {
            // Create output array.
            var outputValues = new int[InputValues.Length][];
            for (var index = 0; index < outputValues.Length; index++)
            {
                outputValues[index] = new int[StepValues.Length];
            }
            // Step 1 : Factorial
            // Call service method asynchronously.
            var step1Tasks = new HashSet<Task<(int Index, int OutputValue)>>();
            for (var index = 0; index < InputValues.Length; index++)
            {
                var localIndex = index; // Prevent lambda from capturing last index value of containing for loop.
                var inputValue = InputValues[index];
                // Capture task.  Don't block by awaiting.
                step1Tasks.Add(AsyncHelper.MaterializeTask(async () =>
                {
                    var outputValue = await MathService.AsUser.Factorial(inputValue);
                    return (localIndex, outputValue);
                }));
            }

            // Step 2 : Add
            // Process step 1 results as they arrive.  Use result of step 1 to call step 2 service method asynchronously.
            var step2Tasks = new HashSet<Task<(int Index, int OutputValue)>>();
            while (step1Tasks.Count > 0)
            {
                if (step1Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var step1Task = await Task.WhenAny(step1Tasks);
                var (index, outputValue) = await step1Task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                outputValues[index][0] = outputValue;
                step1Tasks.Remove(step1Task); // Remove from task collection so we don't consume it again.
                var inputValue = outputValue;
                var stepValue = StepValues[1];
                // Capture task.  Don't block by awaiting.
                step2Tasks.Add(AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await MathService.AsUser.Add(inputValue, stepValue);
                    return (index, outputValue);
                }));
            }
            DisplayValues(InputValues, StepValues, outputValues);
            
            // Step 3 : Multiply
            // Process step 2 results as they arrive.  Use result of step 2 to call step 3 service method asynchronously.
            var step3Tasks = new HashSet<Task<(int Index, int OutputValue)>>();
            while ((step1Tasks.Count + step2Tasks.Count) > 0)
            {
                if (step2Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var step2Task = await Task.WhenAny(step2Tasks);
                var (index, outputValue) = await step2Task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                outputValues[index][1] = outputValue;
                step2Tasks.Remove(step2Task); // Remove from task collection so we don't consume it again.
                var inputValue = outputValue;
                var stepValue = StepValues[2];
                // Capture task.  Don't block by awaiting.
                step3Tasks.Add(AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await MathService.AsUser.Multiply(inputValue, stepValue);
                    return (index, outputValue);
                }));
            }
            DisplayValues(InputValues, StepValues, outputValues);

            // Step 4 : Modulo
            // Process step 3 results as they arrive.  Use result of step 3 to call step 4 service method asynchronously.
            var step4Tasks = new HashSet<Task<(int Index, int OutputValue)>>();
            while ((step2Tasks.Count + step3Tasks.Count) > 0)
            {
                if (step3Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var step3Task = await Task.WhenAny(step3Tasks);
                var (index, outputValue) = await step3Task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                outputValues[index][2] = outputValue;
                step3Tasks.Remove(step3Task); // Remove from task collection so we don't consume it again.
                var inputValue = outputValue;
                var stepValue = StepValues[3];
                // Capture task.  Don't block by awaiting.
                step4Tasks.Add(AsyncHelper.MaterializeTask(async () =>
                {
                    outputValue = await MathService.AsUser.Modulo(inputValue, stepValue);
                    return (index, outputValue);
                }));
            }
            DisplayValues(InputValues, StepValues, outputValues);


            // Final Results
            while ((step3Tasks.Count + step4Tasks.Count) > 0)
            {
                if (step4Tasks.Count == 0) continue; // Avoid exception caused by awaiting empty collection in next line.
                var step4Task = await Task.WhenAny(step4Tasks);
                var (index, outputValue) = await step4Task; // Network or internal service method exceptions may occur here.  Ignore in this demo program.
                outputValues[index][3] = outputValue;
                step4Tasks.Remove(step4Task); // Remove from task collection so we don't consume it again.
            }
            DisplayValues(InputValues, StepValues, outputValues);
            return outputValues;
        }
    }
}
