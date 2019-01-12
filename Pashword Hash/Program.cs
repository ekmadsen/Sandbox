using System;


namespace ErikTheCoder.Sandbox.PasswordHash
{
    public static class Program
    {
        public static void Main()
        {
            const int saltLength = 16;
            const int hashLength = 32;
            const int iterations = 10000;
            // ReSharper disable once StringLiteralTypo
            const string password = @"Password";
            (string salt, string hash) = Password.Hash(password, saltLength, hashLength, iterations);
            Console.WriteLine($"Salt = {salt}.");
            Console.WriteLine($"Hash = {hash}.");
            Console.WriteLine($"Password valid = {Password.Validate(password, "Salt", "Hash", iterations)}.");
            Console.ReadLine();
        }
    }
}
