using System;

namespace PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {

            string password, salt;

            pass:
            Console.WriteLine("Enter password :");
            string line = Console.ReadLine();

            if (!string.IsNullOrEmpty(line))
            {
                password = line;
            }
            else
            {
                goto pass;
            }


            salty:
            Console.WriteLine("Enter salt :");
            string line2 = Console.ReadLine();

            if (!string.IsNullOrEmpty(line2))
            {
                salt = line2;
            }
            else
            {
                goto salty;
            }


            Console.WriteLine($"Hash is {new Hasher().Hash(password, salt)}");

            Console.ReadLine();

        }
    }
}
