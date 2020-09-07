using System;
using System.Threading.Tasks;
using ErikTheCoder.Sandbox.Math.Contract;
using Microsoft.AspNetCore.Mvc;


namespace ErikTheCoder.Sandbox.Math.Service.Controllers
{
    // Each method delays to simulate latency of complex calculation or I/O.
    public class MathController : Controller, IMathService
    {
        public async Task<long> Power(long InputValue, long Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            return (long)System.Math.Pow(InputValue, Value);
        }


        public async Task<long> Add(long InputValue, long Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            return InputValue + Value;
        }


        public async Task<long> Multiply(long InputValue, long Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            return InputValue * Value;
        }


        public async Task<long> Modulo(long InputValue, long Value)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            return InputValue % Value;
        }
    }
}
