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
            var g = GetRandomPositiveBigInteger();
            var n = GetRandomPositiveBigInteger();
            var a = GetRandomPositiveBigInteger();
            var m1 = BigInteger.ModPow(g, a, n);
            var clientPublicKey = new PublicKey
            {
                G = g,
                N = n,
                M = m1
            };
            WriteLine(clientPublicKey.ToString(), ConsoleColor.Yellow);
            var serverPublicKey = Server.ReceiveClientPublicKey(clientPublicKey);
            // Compute shared key and create cipher.
            var sharedKey = ComputeSharedKey(serverPublicKey.M, a, n);
            WriteLine($"Shared Key = {sharedKey}.");
            Cipher = _createCipher(sharedKey);
        }


        public void SendMessageToServer(string Message, Server Server)
        {
            var encryptedMessage = EncryptMessage(Message);
            WriteLine($"Sending encrypted message  \"{encryptedMessage}\"", ConsoleColor.Yellow);
            var encryptedResponseMessage = Server.ReceiveClientMessage(encryptedMessage);
            WriteLine($"Received encrypted message \"{encryptedResponseMessage}\"");
            var responseMessage = DecryptMessage(encryptedResponseMessage);
            WriteLine($"Decrypted message is       \"{responseMessage}\"");
        }
    }
}
