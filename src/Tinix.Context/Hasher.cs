using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Tinix.Context
{
    public class Hasher
    {
        public string Hash(string password, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: Encoding.Unicode.GetBytes(salt), prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}