using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class Server : DiffieHellmanBase
    {
        private Func<BigInteger, CipherBase> _createCipher;
        private bool _disposed;


        public Server(int KeyLength, Func<BigInteger, CipherBase> CreateCipher) : base(KeyLength)
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
            _disposed = true;
            base.Dispose(Disposing);
        }


        // Perform Diffie-Hellman key exchange.
        public PublicKey ReceiveClientPublicKey(PublicKey ClientPublicKey)
        {
            // Compute shared key and create cipher.
            BigInteger b = GetRandomPositiveBigInteger();
            BigInteger m2 = BigInteger.ModPow(ClientPublicKey.G, b, ClientPublicKey.N);
            BigInteger sharedKey = ComputeSharedKey(ClientPublicKey.M, b, ClientPublicKey.N);
            WriteLine($"Shared Key = {sharedKey}.");
            Cipher = _createCipher(sharedKey);
            // Send public key to client.
            PublicKey serverPublicKey = new PublicKey
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
            string message = DecryptMessage(EncryptedMessage);
            WriteLine($"Decrypted message is       \"{message}\"");
            string responseMessage = GetResponseMessage(message);
            string encryptedResponseMessage = EncryptMessage(responseMessage);
            WriteLine($"Sending encrypted message  \"{encryptedResponseMessage}\"", ConsoleColor.Yellow);
            return encryptedResponseMessage;
        }


        private static string GetResponseMessage(string Message)
        {
            switch (Message)
            {
                case "Roger.":
                    return "Huh?";
                case "Request vector.  Over.":
                    return "What?";
                case "We have clearance, Clarence.":
                    return "Roger, Roger.  What's our vector, Victor?";
                case "Surely you can't be serious?":
                    return "I am serious.  And don't call me Shirley.";
                default:
                    return $"{Message}  Airplane!";
            }
        }
    }
}
