using System;
using System.Numerics;


namespace ErikTheCoder.Sandbox.EncryptDecrypt
{
    public class PublicKey
    {
        public BigInteger G { get; set; }
        public BigInteger N { get; set; }
        public BigInteger M { get; set; }


        public override string ToString() => $"{Environment.NewLine}{nameof(G)} = {G}{Environment.NewLine}{nameof(N)} = {N}{Environment.NewLine}{nameof(M)} = {M}";
    }
}
