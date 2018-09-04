using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PromotionApi
{
    internal static class Utils
    {
        internal static readonly Regex _regexName = new Regex("(^[A-Z][a-z]+( ([A-Z]{1}\\.|[a-zA-Z]{2,}))*$){1,150}", RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexNickname = new Regex("[a-z0-9_]{1,45}", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexEmail = new Regex("^(\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*){1,255}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexPassword = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])[\\w#?!@$%^&*-]{6,25}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);

        internal static bool IsValidNickname(string nickname)
            => nickname == null ? false : _regexNickname.IsMatch(nickname);

        internal static bool IsValidEmail(string email)
            => email == null ? false : email.Length <= 255 && _regexEmail.IsMatch(email);

        internal static bool IsValidPassword(string password)
            => password == null ? false : _regexPassword.IsMatch(password);

        internal static bool IsValidName(string name)
            => name == null ? false : _regexName.IsMatch(name);

        internal static bool IsValidCpf(string cpf)
            => cpf == null ? false : cpf.Length == 11 && ulong.TryParse(cpf, out _);

        internal static string EncodeBase64(string str)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(str)).TrimEnd('=');

        internal static string DecodeBase64(string str)
        {
            if (str.Length % 4 != 0)
                str += ("===").Substring(0, 4 - (str.Length % 4));

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
