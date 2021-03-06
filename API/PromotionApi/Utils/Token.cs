﻿using Microsoft.AspNetCore.Mvc;
using PromotionApi.Models;
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

            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[8];
                provider.GetBytes(byteArray);
                var randomInteger = BitConverter.ToUInt64(byteArray, 0);
                return $"{Utils.EncodeBase64(epoch.ToString())}.{randomInteger.ToString("X")}{_workerId.ToString("0000")}{Thread.CurrentThread.ManagedThreadId.ToString("0000")}";
            }
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

        internal static TokenValidationResult ValidateAuthorization(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return new TokenValidationResult(new ErrorResponse { Error = "Missing header: authorization" });
            else
            {
                if (!authorization.StartsWith("Bearer "))
                    return new TokenValidationResult(new ErrorResponse { Error = "Invalid authorization" });
                else
                {
                    string token = authorization.Substring(7);
                    if (!IsValid(token))
                        return new TokenValidationResult(new ErrorResponse { Error = "Invalid authorization" });
                    return new TokenValidationResult(token);
                }
            }
        }
    }

    internal class TokenValidationResult
    {
        public bool IsValid { get; }
        public string Token { get; }
        public object Result { get; }

        public TokenValidationResult(object result)
        {
            IsValid = false;
            Result = result;
        }

        public TokenValidationResult(string token)
        {
            IsValid = true;
            Token = token;
        }
    }
}
