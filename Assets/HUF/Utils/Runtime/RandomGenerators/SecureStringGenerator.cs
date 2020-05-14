using System;
using System.Security.Cryptography;
using System.Text;

namespace HUF.Utils.Runtime.RandomGenerators
{
    public static class SecureStringGenerator
    {
        const string VALID_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public static string RandomString(int length)
        {
            var result = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(buffer);
                    var num = BitConverter.ToUInt32(buffer, 0);
                    result.Append(VALID_CHARS[(int) (num % (uint) VALID_CHARS.Length)]);
                }
            }
            return result.ToString();
        }
    }
}