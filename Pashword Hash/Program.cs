using System;


namespace ErikTheCoder.Sandbox.PasswordHash
{
    public static class Program
    {
        public static void Main(string[] Args)
        {
            const int saltLength = 16;
            const int hashLength = 32;
            const int iterations = 1000;
            const string password = "OpenSeysMe";
            (string salt, string hash) = Password.Hash(password, saltLength, hashLength, iterations);
            Console.WriteLine($"Salt = {salt}.");
            Console.WriteLine($"Hash = {hash}.");
            Console.WriteLine($"Password valid = {Password.Validate(password, "3RYC5Ex6UCokBTRwawJ5tw==", "xFtit2Q447+E2hR7ilmAlWIFjOINgqgrg0pesQ0Vyd0=", iterations)}.");
            Console.ReadLine();
        }
    }
}
