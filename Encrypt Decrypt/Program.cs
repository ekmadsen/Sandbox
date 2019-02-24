using System;
using System.Collections.Generic;
using System.Numerics;
using ErikTheCoder.Logging;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public static class Program
    {
        public static void Main(string[] Arguments)
        {
            try
            {
                Run(Arguments);
            }
            catch (Exception exception)
            {
                // This code is not thread safe.  But this program uses a single thread, so no issue.
                ConsoleColor restoreColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.GetSummary(true, true));
                Console.ForegroundColor = restoreColor;
            }
        }


        private static void Run(IReadOnlyList<string> Arguments)
        {
            Console.WriteLine();
            (Func<BigInteger, CipherBase> createCipher, int keyLength) = ParseCommandLine(Arguments);
            // Create client and server.
            using (Client client = new Client(createCipher, keyLength))
            {
                using (Server server = new Server(createCipher, keyLength))
                {
                    // Perform Diffie-Hellman key exchange.
                    client.InitiateKeyExchange(server);
                    while (true)
                    {
                        // Prompt for message and send to server.
                        Console.WriteLine();
                        Console.Write("Enter a message to send to server:  ");
                        string message = Console.ReadLine();
                        if (string.IsNullOrEmpty(message)) return;
                        Console.WriteLine();
                        client.SendMessageToServer(message, server);
                    }
                }
            }
        }


        private static (Func<BigInteger, CipherBase> CreateCipher, int KeyLength) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            // Parse cipher name.
            Func<BigInteger, CipherBase> createCipher;
            string cipherName = (Arguments.Count > 0) ? Arguments[0] : null;
            switch (cipherName)
            {
                case "xor":
                    createCipher = CreateXorCipher;
                    break;
                case "aes":
                    createCipher = CreateAesCipher;
                    break;
                default:
                    throw new ArgumentException(cipherName is null ? "Specify a cipher name." : $"{cipherName} Cipher not supported.");
            }
            // Parse key length.
            if (Arguments.Count < 2) throw new ArgumentException("Specify a key length.");
            string keyLengthText = Arguments[1];
            if (!int.TryParse(keyLengthText, out int keyLength)) throw new ArgumentException($"Key length {keyLengthText} not supported.");
            return (createCipher, keyLength);
        }


        private static CipherBase CreateXorCipher(BigInteger SharedKey) => new XorCipher(SharedKey);


        private static CipherBase CreateAesCipher(BigInteger SharedKey) => new AesCipher(SharedKey);
    }
}
