using System.Security.Cryptography;
using System.Text;
using UserAuthentication.Dto;
using UserAuthentication.Models;

namespace UserAuthentication.Hashing
{
    public class HashPassword :IHashPassword
    {
        public  void CraeteHashPassword(string password, out byte[] passwordHash, out byte[] passwordSlot)
        {
            using (var Hash = new HMACSHA512())
            {
                passwordSlot = Hash.Key;

                passwordHash = Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            };
        }

       

        public bool varifyPassword(string password ,byte[] passwordhash,byte[] passwordSlot)
        {
            using (var hash = new HMACSHA512(passwordSlot))
            {
                var HashPass = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                return HashPass.SequenceEqual(passwordhash);
            }
        }
        public bool varifyAdmin()
        {
            return true;
        }
    }
}
