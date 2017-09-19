using System.Linq;
using System.Security.Cryptography;

namespace DAT.Serialization
{
    public static class MD5Generator
    {
        public static string Generate(byte[] objectToHash)
        {
            using (MD5 mD5 = new MD5CryptoServiceProvider())
            {
                var result = mD5.ComputeHash(objectToHash);

                return string.Join("", result.Select(r => r.ToString("x2")));
            }
        }
    }
}
