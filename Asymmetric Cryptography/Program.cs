using System;
using System.Numerics;
using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.AsymmetricCryptography
{
    public static class Program
    {
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


            // Test using small, positive integers.
            Console.WriteLine("Testing using small, positive integers.");
            const long smallG = 11;
            Console.WriteLine($"g = {smallG}.");
            const long smallN = 23;
            Console.WriteLine($"n = {smallN}.");
            Console.WriteLine();
            // Send public key from A to B.
            const long smallA = 4;
            Math.DivRem((long)Math.Pow(smallG, smallA), smallN, out long smallM1);
            Console.WriteLine($"m1 = {smallM1}.");
            Console.WriteLine();
            // Send public key from B to A.
            const long smallB = 9;
            Math.DivRem((long)Math.Pow(smallG, smallB), smallN, out long smallM2);
            Console.WriteLine($"m2 = {smallM2}.");
            Console.WriteLine();
            // Compute shared key.
            Math.DivRem((long)Math.Pow(smallM2, smallA), smallN, out long smallAk);
            Console.WriteLine($"ak = {smallAk}.");
            Math.DivRem((long)Math.Pow(smallM1, smallB), smallN, out long smallBk);
            Console.WriteLine($"bk = {smallBk}.");
            Console.WriteLine();
            Console.WriteLine(smallAk == smallBk ? "Shared keys match." : "Shared keys do not match.");
            Console.ReadLine();


            // Test using big, positive integers.
            Console.WriteLine("Testing using big, positive integers.");
            BigInteger g = GetRandomPositiveBigInteger();
            Console.WriteLine($"g = {g}.");
            Console.WriteLine();
            BigInteger n = GetRandomPositiveBigInteger();
            Console.WriteLine($"n = {n}.");
            Console.WriteLine();
            // Send public key from A to B.
            BigInteger a = GetRandomPositiveBigInteger();
            BigInteger m1 = BigInteger.ModPow(g, a, n);
            Console.WriteLine($"m1 = {m1}.");
            Console.WriteLine();
            // Send public key from B to A.
            BigInteger b = GetRandomPositiveBigInteger();
            BigInteger m2 = BigInteger.ModPow(g, b, n);
            Console.WriteLine($"m2 = {m2}.");
            Console.WriteLine();
            // Compute shared key.
            BigInteger ak = BigInteger.ModPow(m2, a, n);
            Console.WriteLine($"ak = {ak}.");
            Console.WriteLine();
            BigInteger bk = BigInteger.ModPow(m1, b, n);
            Console.WriteLine($"bk = {bk}.");
            Console.WriteLine();
            Console.WriteLine(ak == bk ? "Shared keys match." : "Shared keys do not match.");
            Console.ReadLine();
        }


        private static BigInteger GetRandomPositiveBigInteger()
        {
            _random.NextBytes(_buffer);
            _buffer[_bigIntBufferLength - 1] &= 0x7F; // Force sign bit to positive.
            return new BigInteger(_buffer);
        }
    }
}
