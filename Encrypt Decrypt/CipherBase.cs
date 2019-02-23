using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public abstract class CipherBase : IDisposable
    {
        private bool _disposed;


        protected byte[] SharedKey { get; private set; }


        protected CipherBase(BigInteger SharedKey)
        {
            this.SharedKey = SharedKey.ToByteArray();
        }


        ~CipherBase() => Dispose(false);


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
                SharedKey = null;
            }
            // This base class contains no unmanaged objects to dispose.
            _disposed = true;
        }


        public abstract string Encrypt(string Message);
        public abstract string Decrypt(string EncryptedMessage);
    }
}
