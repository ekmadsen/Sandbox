using System;
using System.Numerics;
using System.Security.Cryptography;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public abstract class DiffieHellmanBase : IDisposable
    {
        private readonly int _keyLength;
        private byte[] _buffer;
        private RNGCryptoServiceProvider _random;
        private bool _disposed;


        protected CipherBase Cipher { private get; set; }


        protected DiffieHellmanBase(int KeyLength)
        {
            _keyLength = KeyLength;
            _buffer = new byte[KeyLength];
            _random = new RNGCryptoServiceProvider();
        }


        ~DiffieHellmanBase() => Dispose(false);


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // Dispose managed objects.
                _buffer = null;
                _random = null;
            }
            // Dispose unmanaged objects.
            _random?.Dispose();
            _random = null;
            Cipher?.Dispose();
            Cipher = null;
            _disposed = true;
        }


        protected BigInteger GetRandomPositiveBigInteger()
        {
            _random.GetBytes(_buffer);
            _buffer[_keyLength - 1] &= 0x7F; // Force sign bit to positive.  See https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor.
            return new BigInteger(_buffer);
        }


        protected static BigInteger ComputeSharedKey(BigInteger M, BigInteger Secret, BigInteger N) => BigInteger.ModPow(M, Secret, N);


        protected string EncryptMessage(string Message) => Cipher.Encrypt(Message);


        protected string DecryptMessage(string EncryptedMessage) => Cipher.Decrypt(EncryptedMessage);


        protected void WriteLine(string Message) => WriteLine(Message, Console.ForegroundColor);


        protected void WriteLine(string Message, ConsoleColor Color)
        {
            // This code is not thread safe.  But this program uses a single thread, so no issue.
            var restoreColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($"{GetType().Name}:  {Message}{Environment.NewLine}");
            Console.ForegroundColor = restoreColor;
        }
    }
}
