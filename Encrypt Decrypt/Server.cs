using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class Server : DiffieHellmanBase
    {
        private Func<BigInteger, CipherBase> _createCipher;
        private bool _disposed;


        public Server(Func<BigInteger, CipherBase> CreateCipher, int KeyLength) : base(KeyLength)
        {
            _createCipher = CreateCipher;
        }


        ~Server() => Dispose(false);


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
        public PublicKey ReceiveClientPublicKey(PublicKey ClientPublicKey)
        {
            // Compute shared key and create cipher.
            var b = GetRandomPositiveBigInteger();
            var m2 = BigInteger.ModPow(ClientPublicKey.G, b, ClientPublicKey.N);
            var sharedKey = ComputeSharedKey(ClientPublicKey.M, b, ClientPublicKey.N);
            WriteLine($"Shared Key = {sharedKey}.");
            Cipher = _createCipher(sharedKey);
            // Send public key to client.
            var serverPublicKey = new PublicKey
            {
                G = ClientPublicKey.G,
                N = ClientPublicKey.N,
                M = m2
            };
            WriteLine(serverPublicKey.ToString(), ConsoleColor.Yellow);
            return serverPublicKey;
        }


        public string ReceiveClientMessage(string EncryptedMessage)
        {
            WriteLine($"Received encrypted message \"{EncryptedMessage}\"");
            var message = DecryptMessage(EncryptedMessage);
            WriteLine($"Decrypted message is       \"{message}\"");
            var responseMessage = GetResponseMessage(message);
            var encryptedResponseMessage = EncryptMessage(responseMessage);
            WriteLine($"Sending encrypted message  \"{encryptedResponseMessage}\"", ConsoleColor.Yellow);
            return encryptedResponseMessage;
        }


        private static string GetResponseMessage(string Message)
        {
            return Message switch
            {
                "Roger." => "Huh?",
                "Request vector.  Over." => "What?",
                "We have clearance, Clarence." => "Roger, Roger.  What's our vector, Victor?",
                "Surely you can't be serious?" => "I am serious.  And don't call me Shirley.",
                _ => $"{Message}  Airplane!"
            };
        }
    }
}
