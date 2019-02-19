using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class Client : DiffieHellmanBase
    {
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
            // Compute shared key.
            BigInteger sharedKey = ComputeSharedKey(serverPublicKey.M, a, n);
            WriteLine($"{nameof(SharedKey)} = {sharedKey}.");
            SharedKey = sharedKey.ToByteArray();
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
