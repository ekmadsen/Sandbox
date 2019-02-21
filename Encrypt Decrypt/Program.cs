using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public static class Program
    {
        public static void Main(string[] Arguments)
        {
            Console.WriteLine();
            // Read cipher name from command line arguments.
            Func<BigInteger, CipherBase> createCipher;
            string cipherName = (Arguments.Length > 0) ? Arguments[0] : null;
            switch (cipherName)
            {
                case "xor":
                    createCipher = CreateXorCipher;
                    break;
                case "aes":
                    createCipher = CreateAesCipher;
                    break;
                default:
                    throw new ArgumentException(cipherName is null ? "Specify a cipher name." : $"{cipherName} Cipher not supported." );
            }
            // Read key length from command line arguments.
            if (Arguments.Length < 2) throw new ArgumentException("Specify a key length.");
            if (!int.TryParse(Arguments[1], out int keyLength)) throw new ArgumentException("Key length not supported.");
            // Create client and server.
            using (Client client = new Client(keyLength, createCipher))
            {
                using (Server server = new Server(keyLength, createCipher))
                {
                    // Perform Diffie-Hellman key exchange.
                    client.InitiateKeyExchange(server);
                    while (true)
                    {
                        // Prompt for message and send to server.
                        Console.WriteLine();
                        Console.Write("Enter a message to send to server:  ");
                        string message = Console.ReadLine();
                        Console.WriteLine();
                        client.SendMessageToServer(message, server);
                    }
                }
            }
        }


        private static CipherBase CreateXorCipher(BigInteger SharedKey) => new XorCipher(SharedKey);


        private static CipherBase CreateAesCipher(BigInteger SharedKey) => new AesCipher(SharedKey);
    }
}
