using System;

namespace PromotionApi
{
    internal static class Code
    {
        internal static TimeSpan LifeSpan { get; } = TimeSpan.FromHours(1);

        private static readonly char[] _validChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        internal static string Generate(int size = 6)
        {
            string code = "";
            for (int i = 0; i < size; i++)
                code += _validChars[Utils.Random(_validChars.Length)];
            return code;
        }
    }
}
