using System;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using Microsoft.AspNetCore.Mvc;


namespace ErikTheCoder.Sandbox.Math.Service.Controllers
{
    // Each method delays to simulate latency of complex calculation or I/O.
    public class MathController : Controller, IMathService
    {
        public async Task<int> Factorial(int InputValue)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            int value = 1;
            while (InputValue > 0)
            {
                value *= InputValue;
                InputValue--;
            }
            return value;
        }


        public async Task<int> Add(int InputValue, int Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            return InputValue + Value;
        }


        public async Task<int> Multiply(int InputValue, int Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            return InputValue * Value;
        }


        public async Task<int> Modulo(int InputValue, int Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            return InputValue % Value;
        }
    }
}
