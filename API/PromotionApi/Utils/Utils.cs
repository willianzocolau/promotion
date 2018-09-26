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
        internal static readonly Regex _regexName       = new Regex("(^[A-Z][a-z]+( ([A-Z]{1}\\.|[a-zA-Z]{2,}))*$){1,150}", RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexNickname   = new Regex("[a-z0-9_]{1,45}", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexEmail      = new Regex("^(\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*){1,255}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Compiled | RegexOptions.ECMAScript);
        //Pelo menos 1 caracter maiúsculo, 1 minúsculo e 1 número, de 6 até 25 caracteres
        internal static readonly Regex _regexPassword   = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])[\\w#?!@$%^&*-]{6,25}$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexImageUrl   = new Regex("^(http(s?):)([/|.|\\w|\\s|-])*\\.(?:jpg|gif|png)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        internal static readonly Regex _regexTelephone  = new Regex("^[1-9]{2}[2-9][0-9]{7,8}$", RegexOptions.Compiled | RegexOptions.ECMAScript);



        private static readonly string _emailDomain     = "smtp.gmail.com";
        private static readonly int    _emailPort       = 587;
        private static readonly string _emailUsername   = "promotion.suporte@gmail.com";
        private static readonly string _emailPassword   = "<Uiop}+hjkl)";



        private static readonly Random _random         = new Random();
        private static readonly object _randomLock     = new object();



        private static readonly double _sellerCashback = 0.1;
        private static readonly double _buyerCashback  = 0.2;



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

        internal static bool IsValidTelephone(string telephone)
            => telephone == null ? false : _regexTelephone.IsMatch(telephone);

        internal static bool IsValidImageUrl(string url)
            => url == null ? false : _regexImageUrl.IsMatch(url);



        internal static string EncodeBase64(string str)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(str)).TrimEnd('=');

        internal static string DecodeBase64(string str)
        {
            if (str.Length % 4 != 0)
                str += ("===").Substring(0, 4 - (str.Length % 4));

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }



        internal static bool CanAdministrateOrders(UserType user)
            => user != UserType.Normal;

        internal static bool CanDeletePromotion(UserType user)
            => user != UserType.Normal;



        internal static double GetSellerCashback(double totalCashback)
            => totalCashback * _sellerCashback;

        internal static double GetBuyerCashback(double totalCashback)
            => totalCashback * _buyerCashback;



        internal static async Task SendEmailAsync(string email, string subject, string message)
        {
            //TODO: Test email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_emailUsername, "Promotion");
            mail.To.Add(new MailAddress(email));
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(_emailDomain, _emailPort))
            {
                smtp.Credentials = new NetworkCredential(_emailUsername, _emailPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }



        internal static int Random(int minValue, int maxValue)
        {
            lock (_randomLock)
            {
                return _random.Next(minValue, maxValue);
            }
        }
        internal static int Random(int maxValue)
            => Random(0, maxValue);
    }
}
