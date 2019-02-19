using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class Server : DiffieHellmanBase
    {
        // Perform Diffie-Hellman key exchange.
        public PublicKey ReceiveClientPublicKey(PublicKey ClientPublicKey)
        {
            // Compute shared key.
            BigInteger b = GetRandomPositiveBigInteger();
            BigInteger m2 = BigInteger.ModPow(ClientPublicKey.G, b, ClientPublicKey.N);
            BigInteger sharedKey = ComputeSharedKey(ClientPublicKey.M, b, ClientPublicKey.N);
            WriteLine($"{nameof(SharedKey)} = {sharedKey}.");
            SharedKey = sharedKey.ToByteArray();
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
            string responseMessage = $"{message}  Roger that.";
            string encryptedResponseMessage = EncryptMessage(responseMessage);
            WriteLine($"Sending encrypted message  \"{encryptedResponseMessage}\"", ConsoleColor.Yellow);
            return encryptedResponseMessage;
        }
    }
}
