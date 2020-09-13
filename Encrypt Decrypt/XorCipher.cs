using System;
using System.Numerics;
using System.Text;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class XorCipher : CipherBase
    {
        public XorCipher(BigInteger SharedKey) : base(SharedKey)
        {
        }


        public override string Encrypt(string Message)
        {
            // Convert message to byte array.
            var messageBytes = Encoding.UTF8.GetBytes(Message);
            if (messageBytes.Length > SharedKey.Length) throw new ArgumentException($"{nameof(Message)} is too long.  Increase key length.");
            // XOR message and shared key.
            // XOR is a reversible operation (if c = a XOR b then a = c XOR b).
            var encryptedMessageBytes = new byte[messageBytes.Length];
            for (var index = 0; index < messageBytes.Length; index++) encryptedMessageBytes[index] = messageBytes[index] ^= SharedKey[index];
            // Convert encrypted message bytes to Base64 text (to eliminate control characters).
            return Convert.ToBase64String(encryptedMessageBytes);
        }


        public override string Decrypt(string EncryptedMessage)
        {
            // Convert encrypted message to byte array.
            var encryptedMessageBytes = Convert.FromBase64String(EncryptedMessage);
            if (encryptedMessageBytes.Length > SharedKey.Length) throw new ArgumentException($"{nameof(EncryptedMessage)} is too long.");
            // XOR message and shared key.
            // XOR is a reversible operation (if c = a XOR b then a = c XOR b).
            var messageBytes = new byte[encryptedMessageBytes.Length];
            for (var index = 0; index < encryptedMessageBytes.Length; index++) messageBytes[index] = encryptedMessageBytes[index] ^= SharedKey[index];
            // Convert message bytes to text.
            return Encoding.UTF8.GetString(messageBytes);
        }
    }
}
