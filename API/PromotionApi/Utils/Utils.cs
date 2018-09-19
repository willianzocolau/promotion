using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PromotionApi
{
    internal static class Utils
    {
        internal static DateTimeOffset PromotionEpoch { get; } = new DateTimeOffset(2018, 9, 1, 0, 0, 0, 0, TimeSpan.Zero);

        //Formato (1 à 150): Example Name F. of Someone
        internal static readonly Regex _regexName = new Regex("(^[A-Z][a-z]+( ([A-Z]{1}\\.|[a-zA-Z]{2,}))*$){1,150}", RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexNickname = new Regex("[a-z0-9_]{1,45}", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexEmail = new Regex("^(\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*){1,255}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        //Pelo menos 1 caracter maiúsculo, 1 minúsculo e 1 número, de 6 até 25 caracteres
        internal static readonly Regex _regexPassword = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])[\\w#?!@$%^&*-]{6,25}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);

        private static string EmailDomain   { get; } = "smtp.gmail.com";
        private static int    EmailPort     { get; } = 587;
        private static string EmailUsername { get; } = "promotion.suporte@gmail.com";
        private static string EmailPassword { get; } = "<Uiop}+hjkl)";

        private static Random _random = new Random();

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

        internal static async Task SendEmailAsync(string email, string subject, string message)
        {
            //TODO: Test email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(EmailUsername, "Promotion");
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(EmailDomain, EmailPort))
            {
                smtp.Credentials = new NetworkCredential(EmailUsername, EmailPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }

        internal static int Random(int minValue, int maxValue)
            => _random.Next(minValue, maxValue);
        internal static int Random(int maxValue)
            => _random.Next(0, maxValue);
    }
}
