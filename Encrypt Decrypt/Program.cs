using System;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine();
            // Perform Diffie-Hellman key exchange.
            Client client = new Client();
            Server server = new Server();
            client.InitiateKeyExchange(server);
            // Prompt for message and send to server.
            Console.WriteLine();
            Console.Write("Enter a message to send to server:  ");
            string message = Console.ReadLine();
            Console.WriteLine();
            client.SendMessageToServer(message, server);
            Console.WriteLine();
        }
    }
}
