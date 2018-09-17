namespace PromotionApi
{
    internal static class Encryption
    {
        internal static string Encrypt(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            return str;
        }

        internal static string Decrypt(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            return str;
        }
    }
}
