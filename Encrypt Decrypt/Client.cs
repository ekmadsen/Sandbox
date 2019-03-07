using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class Client : DiffieHellmanBase
    {
        private Func<BigInteger, CipherBase> _createCipher;
        private bool _disposed;


        public Client(Func<BigInteger, CipherBase> CreateCipher, int KeyLength) : base(KeyLength)
        {
            _createCipher = CreateCipher;
        }


        ~Client() => Dispose(false);


        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // Dispose managed objects.
                _createCipher = null;
            }
            // Dispose unmanaged objects.
            base.Dispose(Disposing);
            _disposed = true;
        }


        // Perform Diffie-Hellman key exchange.
        public void InitiateKeyExchange(Server Server)
        {
            // Send public key to server.
            BigInteger g = GetRandomPositiveBigInteger();
            BigInteger n = GetRandomPositiveBigInteger();
            BigInteger a = GetRandomPositiveBigInteger();
            BigInteger m1 = BigInteger.ModPow(g, a, n);
            PublicKey clientPublicKey = new PublicKey
            {
                G = g,
                N = n,
                M = m1
            };
            WriteLine(clientPublicKey.ToString(), ConsoleColor.Yellow);
            PublicKey serverPublicKey = Server.ReceiveClientPublicKey(clientPublicKey);
            // Compute shared key and create cipher.
            BigInteger sharedKey = ComputeSharedKey(serverPublicKey.M, a, n);
            WriteLine($"Shared Key = {sharedKey}.");
            Cipher = _createCipher(sharedKey);
        }


        public void SendMessageToServer(string Message, Server Server)
        {
            string encryptedMessage = EncryptMessage(Message);
            WriteLine($"Sending encrypted message  \"{encryptedMessage}\"", ConsoleColor.Yellow);
            string encryptedResponseMessage = Server.ReceiveClientMessage(encryptedMessage);
            WriteLine($"Received encrypted message \"{encryptedResponseMessage}\"");
            string responseMessage = DecryptMessage(encryptedResponseMessage);
            WriteLine($"Decrypted message is       \"{responseMessage}\"");
        }
    }
}
