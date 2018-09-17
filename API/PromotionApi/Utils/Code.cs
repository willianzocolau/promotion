using System;

namespace PromotionApi
{
    internal static class Code
    {
        internal static TimeSpan LifeSpan { get; } = TimeSpan.FromHours(1);

        internal static string Generate(int size)
        {
            //TODO: generate 6-digit token
            return "";
        }
    }
}
