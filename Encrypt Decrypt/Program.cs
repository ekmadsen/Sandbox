using System;
using System.Collections.Generic;
using System.Numerics;
using ErikTheCoder.Utilities;


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
                var restoreColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.GetSummary(true, true));
                Console.ForegroundColor = restoreColor;
            }
        }


        private static void Run(IReadOnlyList<string> Arguments)
        {
            Console.WriteLine();
            var (createCipher, keyLength) = ParseCommandLine(Arguments);
            // Create client and server.
            using (var client = new Client(createCipher, keyLength))
            {
                using (var server = new Server(createCipher, keyLength))
                {
                    // Perform Diffie-Hellman key exchange.
                    client.InitiateKeyExchange(server);
                    while (true)
                    {
                        // Prompt for message and send to server.
                        Console.WriteLine();
                        Console.Write("Enter a message to send to server:  ");
                        var message = Console.ReadLine();
                        if (message.IsNullOrEmpty()) return;
                        Console.WriteLine();
                        client.SendMessageToServer(message, server);
                    }
                }
            }
        }


        private static (Func<BigInteger, CipherBase> CreateCipher, int KeyLength) ParseCommandLine(IReadOnlyList<string> Arguments)
        {
            // Parse cipher name.
            var cipherName = (Arguments.Count > 0) ? Arguments[0].ToLower() : null;
            Func<BigInteger, CipherBase> createCipher = cipherName switch
            {
                "xor" => CreateXorCipher,
                "aes" => CreateAesCipher,
                _ => throw new ArgumentException(cipherName is null
                    ? "Specify a cipher name."
                    : $"{cipherName} Cipher not supported.")
            };
            // Parse key length.
            if (Arguments.Count < 2) throw new ArgumentException("Specify a key length.");
            var keyLengthText = Arguments[1];
            if (!int.TryParse(keyLengthText, out var keyLength)) throw new ArgumentException($"Key length {keyLengthText} not supported.");
            return (createCipher, keyLength);
        }


        private static CipherBase CreateXorCipher(BigInteger SharedKey) => new XorCipher(SharedKey);


        private static CipherBase CreateAesCipher(BigInteger SharedKey) => new AesCipher(SharedKey);
    }
}
