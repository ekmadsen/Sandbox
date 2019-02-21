using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class AesCipher : CipherBase
    {
        private AesCng _cipher;
        private bool _disposed;


        public AesCipher(BigInteger SharedKey) : base(SharedKey)
        {
            _cipher = new AesCng();
        }


        ~AesCipher() => Dispose(false);


        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // Dispose managed objects.
            }
            // Dispose unmanaged objects.
            _cipher.Dispose();
            _cipher = null;
            _disposed = true;
            base.Dispose(Disposing);
        }


        public override string Encrypt(string Message)
        {
            // Generate new initialization vector for each encryption to prevent identical plaintexts from producing identical ciphertexts when encrypted using the same key.
            _cipher.GenerateIV();
            using (MemoryStream stream = new MemoryStream())
            {
                // Write initialization vector to stream.
                stream.Write(_cipher.IV, 0, _cipher.IV.Length);
                using (ICryptoTransform encryptor = _cipher.CreateEncryptor(SharedKey, _cipher.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) { streamWriter.Write(Message); }
                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
        }


        public override string Decrypt(string EncryptedMessage)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(EncryptedMessage)))
            {
                // Read initialization vector from beginning of encrypted message bytes.
                byte[] initializationVector = new byte[_cipher.IV.Length];
                stream.Read(initializationVector, 0, initializationVector.Length);
                using (ICryptoTransform decryptor = _cipher.CreateDecryptor(SharedKey, initializationVector))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) { return streamReader.ReadToEnd(); }
                    }
                }
            }
        }
    }
}
