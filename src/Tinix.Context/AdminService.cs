using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace Tinix.Context
{
    public class AdminService : IAdminService
    {
        public AdminService()
        {
        }

        public bool ValidateCredentials(string userName, string password)
        {
            string hashed = new Hasher().Hash(password, AdministratorContext.Salt);

            if (string.Compare(hashed, AdministratorContext.Hash, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                //password is fine, compare username
                if (string.Compare(userName, AdministratorContext.Username, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }

            return false;

            //return true;
        }
    }
}