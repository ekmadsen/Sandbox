using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class PipelineV1 : PipelineBase
    {
        public override async Task<long[][]> Run(IMathService MathService, long[] InputValues, long[] StepValues)
        {
            // Create output array.
            var outputValues = new long[InputValues.Length][];
            for (var index = 0; index < outputValues.Length; index++)
            {
                outputValues[index] = new long[StepValues.Length];
            }
            // To establish baseline performance, call math service methods sequentially.
            for (var index = 0; index < InputValues.Length; index++)
            {
                var inputValue = InputValues[index];
                var stepValue = StepValues[0];
                // Step 1 : Power
                var outputValue = await MathService.Power(inputValue, stepValue);
                outputValues[index][0] = outputValue;
                inputValue = outputValue;
                // Step 2 : Add
                stepValue = StepValues[1];
                outputValue = await MathService.Add(inputValue, stepValue);
                outputValues[index][1] = outputValue;
                inputValue = outputValue;
                // Step 3 : Multiply
                stepValue = StepValues[2];
                outputValue = await MathService.Multiply(inputValue, stepValue);
                outputValues[index][2] = outputValue;
                inputValue = outputValue;
                // Step 4 : Modulo
                stepValue = StepValues[3];
                outputValue = await MathService.Modulo(inputValue, stepValue);
                outputValues[index][3] = outputValue;
                // Display values.
                DisplayValues(InputValues, StepValues, outputValues);
            }
            return outputValues;
        }
    }
}
