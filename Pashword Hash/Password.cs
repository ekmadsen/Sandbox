using System;
using System.Security.Cryptography;


namespace ErikTheCoder.Sandbox.PasswordHash
{
    public static class Password
    {
        public static (string Salt, string Hash) Hash(string Password, int SaltLength, int HashLength, int Iterations)
        {
            // Create random salt.
            // Storing a salt value with a hashed password prevents identical passwords from hashing to the same stored value.
            // See https://security.stackexchange.com/questions/17421/how-to-store-salt
            var saltBytes = new byte[SaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes, 0, SaltLength);
                var salt = Convert.ToBase64String(saltBytes);
                // Get derived bytes from the combined salt and password, using the specified number of iterations.
                using (var derivedBytes = new Rfc2898DeriveBytes(Password, saltBytes, Iterations))
                {
                    var hashBytes = derivedBytes.GetBytes(HashLength);
                    var hash = Convert.ToBase64String(hashBytes);
                    return (salt, hash);
                }
            }
        }


        public static bool Validate(string Password, string Salt, string Hash, int Iterations)
        {
            var saltBytes = Convert.FromBase64String(Salt);
            var hashLength = Convert.FromBase64String(Hash).Length;
            using (var derivedBytes = new Rfc2898DeriveBytes(Password, saltBytes, Iterations))
            {
                var hashBytes = derivedBytes.GetBytes(hashLength);
                var hash = Convert.ToBase64String(hashBytes);
                return hash == Hash;
            }
        }
    }
}
