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
            byte[] saltBytes = new byte[SaltLength];
            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes, 0, SaltLength);
                string salt = Convert.ToBase64String(saltBytes);
                // Get derived bytes from the combined salt and password, using the specified number of iterations.
                using (Rfc2898DeriveBytes derivedBytes = new Rfc2898DeriveBytes(Password, saltBytes, Iterations))
                {
                    byte[] hashBytes = derivedBytes.GetBytes(HashLength);
                    string hash = Convert.ToBase64String(hashBytes);
                    return (salt, hash);
                }
            }
        }


        public static bool Validate(string Password, string Salt, string Hash, int Iterations)
        {
            byte[] saltBytes = Convert.FromBase64String(Salt);
            int hashLength = Convert.FromBase64String(Hash).Length;
            using (Rfc2898DeriveBytes derivedBytes = new Rfc2898DeriveBytes(Password, saltBytes, Iterations))
            {
                byte[] hashBytes = derivedBytes.GetBytes(hashLength);
                string hash = Convert.ToBase64String(hashBytes);
                return hash == Hash;
            }
        }
    }
}
