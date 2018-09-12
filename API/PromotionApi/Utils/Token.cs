using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace PromotionApi
{
    internal static class Token
    {
        private static readonly ushort _workerId = 1;
        private static readonly Regex _regexToken = new Regex("^[A-Za-z0-9+/=]{1,16}\\.[0-9a-fA-F]{1,20}[0-9]{4}[0-9]{4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly TimeSpan _tokenLifeSpan = TimeSpan.FromDays(1);

        // Key (45): {epoch(1,16)}.{randomNumber(1,20)}{workerId(4)}{threadId{4}}
        internal static string Generate()
        {
            long epoch = (long)(DateTimeOffset.UtcNow - Utils.PromotionEpoch).TotalSeconds;

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[8];
            provider.GetBytes(byteArray);
            var randomInteger = BitConverter.ToUInt64(byteArray, 0);

            return $"{Utils.EncodeBase64(epoch.ToString())}.{randomInteger.ToString("X")}{_workerId.ToString("0000")}{Thread.CurrentThread.ManagedThreadId.ToString("0000")}";
        }

        internal static bool IsValid(string token)
        {
            if (_regexToken.IsMatch(token))
            {
                string epochBase64 = token.Substring(0, token.IndexOf('.'));
                string epochString = Utils.DecodeBase64(epochBase64);
                long secondsSinceEpoch = long.Parse(epochString);
                DateTimeOffset generationDate = Utils.PromotionEpoch.AddSeconds(secondsSinceEpoch);

                return (DateTimeOffset.UtcNow - generationDate) <= _tokenLifeSpan;
            }
            return false;
        }

        internal static TokenResponse ValidateAuthorization(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return new TokenResponse(new BadRequestObjectResult(new { error = "Missing header: authorization" }));
            else
            {
                if (!authorization.StartsWith("Bearer "))
                    return new TokenResponse(new BadRequestObjectResult(new { error = "Invalid authorization" }));
                else
                {
                    string token = authorization.Substring(7);
                    if (!IsValid(token))
                        return new TokenResponse(new BadRequestObjectResult(new { error = "Invalid authorization" }));
                    return new TokenResponse(token);
                }
            }
        }
    }

    internal class TokenResponse
    {
        public bool IsValid { get; }
        public string Token { get; }
        public IActionResult Result { get; }

        public TokenResponse(IActionResult result)
        {
            IsValid = false;
            Result = result;
        }

        public TokenResponse(string token)
        {
            IsValid = true;
            Token = token;
        }
    }
}
