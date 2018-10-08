using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    class HerramientasCifrado
    {
        public static string CifradoPassword(string Password)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] Bytes = (new UnicodeEncoding()).GetBytes(Password);
            byte[] hash = sha1.ComputeHash(Bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
