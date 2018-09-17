using System;
using System.Security.Cryptography;

namespace PromotionApi
{
    internal static class Hash
    {
        internal static string Process(string str)
        {
            using (var sha = new SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(str);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
