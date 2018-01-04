using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace PasswordHasher
{

    //referencing Tinix.Content to use the Hasher class from there, project fails to buuild with "Cannot find project info"
    public class Hasher
    {
        public string Hash(string password, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, Encoding.Unicode.GetBytes(salt), prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}