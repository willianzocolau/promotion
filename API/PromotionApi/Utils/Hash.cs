using System;
using System.Security.Cryptography;
using System.Text;

namespace PromotionApi
{
    internal static class Hash
    {
        internal static string Process(string str, string salt)
        {
            if(string.IsNullOrWhiteSpace(str))
                return null;
            if (string.IsNullOrWhiteSpace(salt))
                return null;

            using (var sha = new SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(str+salt);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        internal static string GenerateSalt()
        {
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[8];
                provider.GetBytes(byteArray);
                return BitConverter.ToString(byteArray);
            }
        }
    }
}
