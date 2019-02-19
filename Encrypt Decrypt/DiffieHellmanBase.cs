using System;
using System.Numerics;
using System.Text;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public abstract class DiffieHellmanBase
    {
        private const int _bigIntBufferLength = 128;
        private readonly byte[] _buffer;
        private readonly Random _random;
        

        protected byte[] SharedKey { private get; set; }


        protected DiffieHellmanBase()
        {
            _buffer = new byte[_bigIntBufferLength];
            _random = new Random();
        }


        protected BigInteger GetRandomPositiveBigInteger()
        {
            _random.NextBytes(_buffer);
            _buffer[_bigIntBufferLength - 1] &= 0x7F; // Force sign bit to positive.  See https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor.
            return new BigInteger(_buffer);
        }


        protected static BigInteger ComputeSharedKey(BigInteger M, BigInteger Secret, BigInteger N) => BigInteger.ModPow(M, Secret, N);


        protected string EncryptMessage(string Message)
        {
            // Convert message to byte array.
            byte[] messageBytes = Encoding.UTF8.GetBytes(Message);
            if (messageBytes.Length > SharedKey.Length) throw new ArgumentException($"{nameof(Message)} is too long.");
            // XOR message and shared key.
            // XOR is a reversible operation (if c = a XOR b then a = c XOR y).
            byte[] encryptedMessageBytes = new byte[messageBytes.Length];
            for (int index = 0; index < messageBytes.Length; index++) encryptedMessageBytes[index] = messageBytes[index] ^= SharedKey[index];
            // Convert encrypted message bytes to Base64 text (to eliminate control characters).
            return Convert.ToBase64String(encryptedMessageBytes);
        }


        protected string DecryptMessage(string EncryptedMessage)
        {
            // Convert encrypted message to byte array.
            byte[] encryptedMessageBytes = Convert.FromBase64String(EncryptedMessage);
            if (encryptedMessageBytes.Length > SharedKey.Length) throw new ArgumentException($"{nameof(EncryptedMessage)} is too long.");
            // XOR message and shared key.
            // XOR is a reversible operation (if c = a XOR b then a = c XOR y).
            byte[] messageBytes = new byte[encryptedMessageBytes.Length];
            for (int index = 0; index < encryptedMessageBytes.Length; index++) messageBytes[index] = encryptedMessageBytes[index] ^= SharedKey[index];
            // Convert message bytes to text.
            return Encoding.UTF8.GetString(messageBytes);
        }


        protected void WriteLine(string Message) => WriteLine(Message, Console.ForegroundColor);


        protected void WriteLine(string Message, ConsoleColor Color)
        {
            // This code is not thread safe.  But this program uses a single thread, so no issue.
            ConsoleColor restoreColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($"{GetType().Name}:  {Message}{Environment.NewLine}");
            Console.ForegroundColor = restoreColor;
        }
    }
}
