using System;
using System.Numerics;
using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.AsymmetricCryptography
{
    public static class Program
    {
        private const string _integerFormat = "0000";
        private const int _bigIntBufferLength = 256;
        private static readonly byte[] _buffer;
        private static readonly Random _random;


        static Program()
        {
            _buffer = new byte[_bigIntBufferLength];
            _random = new Random();
        }


        public static void Main([UsedImplicitly] string[] Args)
        {
            /*
            See https://www.cs.cornell.edu/courses/cs5430/2013sp/TL04.asymmetric.html
            1976: Diffie-Hellman key exchange.
             
            This operation allows two principals to set up a shared key given a public-key system.  It uses a computational assumption called, unsurprisingly,
            the Computational Diffie-Hellman (CDH) assumption.  CDH states that, given g, a generator for a finite field of size n and randomly chosen values
            a and b in this field, it is hard for an adversary to construct g pow (a*b) given only g, g pow a, and g pow b. If this assumption is true, then
            it can be used to exchange shared keys between two principals A and B as follows:
                * A chooses random a, sets M1 = g pow a mod n, and sends M1 to B
                * B chooses random b, sets M2 = g pow b mod n, and send M2 to A
                * A computes K = M2 pow a mod n = g pow (a*b) mod n
                * B computes K = M1 pow b mod n = g pow (a*b) mod n
                * Now A and B share a key K, but CDH implies that no eavesdropper can construct K given only the information that was transmitted between A and B.
            */

            Console.WriteLine();
            if (Args.Length > 0)
            {
                var technique = Args[0].Trim();
                if (string.Equals(technique, "small", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (Args.Length > 1)
                    {
                        var maxValue = int.Parse(Args[1].Trim());
                        TestWithSmallIntegers(maxValue);
                        return;
                    }
                    Console.WriteLine("Provide a maximum integer value as the second parameter.");
                    return;
                }
                if (string.Equals(technique, "big", StringComparison.CurrentCultureIgnoreCase))
                {
                    TestWithBigIntegers();
                    return;
                }
            }
            Console.WriteLine("Provide a \"small\" or \"big\" command line parameter.");
        }


        private static void TestWithSmallIntegers(int MaxValue)
        {
            checked
            {
                if (MaxValue <= 0) throw new ArgumentException($"{nameof(MaxValue)} must be greater than zero.", nameof(MaxValue));
                Console.WriteLine("Testing using small, positive integers.");
                Console.WriteLine();
                long g = GetRandomPositiveInteger(MaxValue);
                Console.WriteLine($" g = {g.ToString(_integerFormat)}.  This is the encryption generator.");
                long n = GetRandomPositiveInteger(MaxValue);
                Console.WriteLine($" n = {n.ToString(_integerFormat)}.  This is the block size of the generator.  Large messages are encrypted in multiple blocks.");
                Console.WriteLine();
                // Send public key from A to B.
                long a = GetRandomPositiveInteger(MaxValue);
                Console.WriteLine($" a = {a.ToString(_integerFormat)}.  This is a random integer chosen by Principal A.");
                Math.DivRem((long) Math.Pow(g, a), n, out var m1);
                Console.WriteLine($"m1 = {m1.ToString(_integerFormat)}.  This is a public key transmitted from Principal A to Principal B.");
                Console.WriteLine();
                // Send public key from B to A.
                long b = GetRandomPositiveInteger(MaxValue);
                Console.WriteLine($" b = {b.ToString(_integerFormat)}.  This is a random integer chosen by Principal B.");
                Math.DivRem((long) Math.Pow(g, b), n, out var m2);
                Console.WriteLine($"m2 = {m2.ToString(_integerFormat)}.  This is a public key transmitted from Principal B to Principal A.");
                Console.WriteLine();
                // Compute shared keys.
                Math.DivRem((long) Math.Pow(m2, a), n, out var ak);
                Console.WriteLine($"ak = {ak.ToString(_integerFormat)}.  This is the shared key Principal A uses to encrypt and decrypt messages sent to / received from Principal B.");
                Math.DivRem((long) Math.Pow(m1, b), n, out var bk);
                Console.WriteLine($"bk = {bk.ToString(_integerFormat)}.  This is the shared key Principal B uses to encrypt and decrypt messages sent to / received from Principal A.");
                Console.WriteLine();
                Console.WriteLine(ak == bk ? "Shared keys (for use by Principals A and B using generator g) match." : "Shared keys (for generator g) do not match.");
                Console.WriteLine();
            }
        }


        private static void TestWithBigIntegers()
        {
            Console.WriteLine("Testing using big, positive integers.");
            Console.WriteLine();
            var g = GetRandomPositiveBigInteger();
            Console.WriteLine($"g = {g.ToString(_integerFormat)}.  This is the encryption generator.");
            var n = GetRandomPositiveBigInteger();
            Console.WriteLine($"n = {n.ToString(_integerFormat)}.  This is the block size of the generator.  Large messages are encrypted in multiple blocks.");
            Console.WriteLine();
            // Send public key from A to B.
            var a = GetRandomPositiveBigInteger();
            Console.WriteLine($"a = {a.ToString(_integerFormat)}.  This is a random integer chosen by Principal A.");
            var m1 = BigInteger.ModPow(g, a, n);
            Console.WriteLine($"m1 = {m1}.  This is a public key transmitted from Principal A to Principal B.");
            Console.WriteLine();
            // Send public key from B to A.
            var b = GetRandomPositiveBigInteger();
            Console.WriteLine($"b = {b.ToString(_integerFormat)}.  This is a random integer chosen by Principal B.");
            var m2 = BigInteger.ModPow(g, b, n);
            Console.WriteLine($"m2 = {m2}.  This is a public key transmitted from Principal B to Principal A.");
            Console.WriteLine();
            // Compute shared keys.
            var ak = BigInteger.ModPow(m2, a, n);
            Console.WriteLine($"ak = {ak}.  This is the shared key Principal A uses to encrypt and decrypt messages sent to / received from Principal B.");
            Console.WriteLine();
            var bk = BigInteger.ModPow(m1, b, n);
            Console.WriteLine($"bk = {bk}.  This is the shared key Principal B uses to encrypt and decrypt messages sent to / received from Principal A.");
            Console.WriteLine();
            Console.WriteLine(ak == bk ? "Shared keys (for use by Principals A and B using generator g) match." : "Shared keys (for generator g) do not match.");
            Console.WriteLine();
        }


        private static int GetRandomPositiveInteger(int MaxValue) => _random.Next(1, MaxValue + 1);


        private static BigInteger GetRandomPositiveBigInteger()
        {
            _random.NextBytes(_buffer);
            _buffer[_bigIntBufferLength - 1] &= 0x7F; // Force sign bit to positive.  See https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor.
            return new BigInteger(_buffer);
        }
    }
}
