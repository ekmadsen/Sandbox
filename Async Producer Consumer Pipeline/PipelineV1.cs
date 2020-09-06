using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using ErikTheCoder.ServiceProxy;


namespace ErikTheCoder.Sandbox.AsyncPipeline
{
    public class PipelineV1 : PipelineBase
    {
        public override async Task<int[][]> Run(Proxy<IMathService> MathService, int[] InputValues, int[] StepValues)
        {
            // Create output array.
            int[][] outputValues = new int[InputValues.Length][];
            for (int index = 0; index < outputValues.Length; index++)
            {
                outputValues[index] = new int[StepValues.Length];
            }
            // To establish baseline performance, call math service methods sequentially.
            for (int index = 0; index < InputValues.Length; index++)
            {
                int inputValue = InputValues[index];
                // Step 1 : Factorial
                int stepIndex = 0;
                int outputValue = await MathService.AsUser.Factorial(inputValue);
                outputValues[index][stepIndex] = outputValue;
                inputValue = outputValue;
                // Step 2 : Add
                stepIndex++;
                int stepValue = StepValues[stepIndex];
                outputValue = await MathService.AsUser.Add(inputValue, stepValue);
                outputValues[index][stepIndex] = outputValue;
                inputValue = outputValue;
                // Step 3 : Multiply
                stepIndex++;
                stepValue = StepValues[stepIndex];
                outputValue = await MathService.AsUser.Multiply(inputValue, stepValue);
                outputValues[index][stepIndex] = outputValue;
                inputValue = outputValue;
                // Step 4 : Modulo
                stepIndex++;
                stepValue = StepValues[stepIndex];
                outputValue = await MathService.AsUser.Modulo(inputValue, stepValue);
                outputValues[index][stepIndex] = outputValue;
                // Display values.
                DisplayValues(InputValues, StepValues, outputValues);
            }
            return outputValues;
        }
    }
}
