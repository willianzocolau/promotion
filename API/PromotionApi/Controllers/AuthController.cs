using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly DatabaseContext _context;

        public AuthController(DatabaseContext context)
        {
            _context = context;
        }

        // POST api/<controller>/register
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /register
        ///     {
        ///         "nickname": "Nickname",
        ///         "name": "Name Lastname"
        ///         "cpf": "01234567890"
        ///     }
        ///     
        /// </remarks>
        /// <param name="authorization">Basic Auth format</param>
        /// <param name="registerUserData">Register information</param>
        /// <returns>Token</returns>
        /// <response code="200">Returns the token</response>
        /// <response code="400">If invalid authorization, invalid json, invalid email, invalid password, invalid nickname, invalid name, invalid cpf, already used email, or already used nickname</response>
        [HttpPost("register")]
        [ProducesResponseType(200, Type = typeof(RegisterResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromHeader, Required] string authorization, [FromBody, Required] RegisterUserBody registerUserData)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return BadRequest(new ErrorResponse { Error = "Missing header: authorization" });

            if (authorization.StartsWith("Basic "))
            {
                string encodedString = authorization.Substring(6);
                byte[] data = Convert.FromBase64String(encodedString);
                string decodedString = Encoding.UTF8.GetString(data);

                if (!decodedString.Contains(':'))
                    return BadRequest(new ErrorResponse { Error = "Invalid authorization" });

                string[] splitString = decodedString.Split(':', 2);
                string email = splitString[0].ToUpperInvariant();
                string password = Encryption.Decrypt(splitString[1]);

                string body;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                    body = await reader.ReadToEndAsync();

                /*RegisterUserBody userData = JsonConvert.DeserializeObject<RegisterUserBody>(body);
                if (userData == null)
                    return BadRequest(new ErrorResponse { Error = "Invalid json" });*/

                if (!Utils.IsValidEmail(email))
                    return BadRequest(new ErrorResponse { Error = "Invalid email" });
                if (!Utils.IsValidPassword(password))
                    return BadRequest(new ErrorResponse { Error = "Invalid password" });
                if (!Utils.IsValidNickname(registerUserData.Nickname))
                    return BadRequest(new ErrorResponse { Error = "Invalid nickname" });
                if (!Utils.IsValidName(registerUserData.Name))
                    return BadRequest(new ErrorResponse { Error = "Invalid name" });
                if (!Utils.IsValidCpf(registerUserData.Cpf))
                    return BadRequest(new ErrorResponse { Error = "Invalid cpf" });

                bool alreadyUsedEmail = await _context.Users.AnyAsync(x => x.Email == email);
                if (alreadyUsedEmail)
                    return BadRequest(new ErrorResponse { Error = "Already used email" });

                bool alreadyUsedNickname = await _context.Users.AnyAsync(x => EF.Functions.ILike(registerUserData.Nickname, x.Nickname));
                if (alreadyUsedNickname)
                    return BadRequest(new ErrorResponse { Error = "Already used nickname" });

                string token = Token.Generate();
                string salt = Hash.GenerateSalt();
                _context.Users.Add(new User
                {
                    Nickname = registerUserData.Nickname,
                    Email = email,
                    Password = Hash.Process(password, salt),
                    PasswordSalt = salt,
                    Name = registerUserData.Name,
                    Credit = 0,
                    StateFK = null,
                    Type = 0,
                    RegisterDate = DateTimeOffset.UtcNow,
                    Token = token,
                    Cpf = registerUserData.Cpf,
                    ImageUrl = null,
                    Cellphone = null,
                    Telephone = null,
                });
                await _context.SaveChangesAsync();

                return Ok(new RegisterResponse { Token = token });
            }

            return BadRequest(new ErrorResponse { Error = "Invalid authorization" });
        }

        // POST api/<controller>/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromHeader] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return BadRequest(new ErrorResponse { Error = "Missing header: authorization" });

            if (authorization.StartsWith("Basic "))
            {
                string encodedString = authorization.Substring(6);
                byte[] data = Convert.FromBase64String(encodedString);
                string decodedString = Encoding.UTF8.GetString(data);

                if (!decodedString.Contains(':'))
                    return BadRequest(new ErrorResponse { Error = "Invalid authorization" });

                string[] splitString = decodedString.Split(':', 2);
                string email = splitString[0].ToUpperInvariant();

                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                    return NotFound(new ErrorResponse { Error = "Email or password wrong" });

                string password = Hash.Process(Encryption.Decrypt(splitString[1]), user.PasswordSalt);

                if (user.Password != password)
                    return NotFound(new ErrorResponse { Error = "Email or password wrong" });

                string token = Token.Generate();
                user.Token = token;
                await _context.SaveChangesAsync();

                return Ok(new { id = user.Id, token, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type, credit = user.Credit, email = user.Email, name = user.Name, state = user.StateFK });
            }

            return BadRequest(new ErrorResponse { Error = "Invalid authorization" });
        }

        // POST api/<controller>/extend
        [HttpPost("extend")]
        public async Task<IActionResult> ExtendAsync([FromHeader] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return NotFound(new ErrorResponse { Error = "Token not found" });
            else
            {
                string newtoken = Token.Generate();

                user.Token = newtoken;
                await _context.SaveChangesAsync();

                return Ok(new { token = newtoken });
            }
        }

        // POST api/<controller>/logout
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromHeader] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user != null)
            {
                user.Token = null;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // POST api/<controller>/reset
        [HttpPost("reset")]
        public async Task<IActionResult> ResetAsync()
        {
            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            ResetPasswordBody data = JsonConvert.DeserializeObject<ResetPasswordBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            if (!Utils.IsValidEmail(data.Email))
                return BadRequest(new ErrorResponse { Error = "Invalid email" });

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == data.Email.ToUpperInvariant());
            if (user == null)
                return NotFound(new ErrorResponse { Error = "Email not found" });

            string code = Code.Generate();

            _context.ForgotPasswordRequests.Add(new ForgotPasswordRequest
            {
                Ip = this.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Code = code,
                RequestDate = DateTimeOffset.UtcNow,
                UserFK = user.Id
            });
            await _context.SaveChangesAsync();

            await Utils.SendEmailAsync(
                data.Email,
                "[ProMotion] Mudança de senha",
                $"Para alterar sua senha, utilize o seguinte código: {code}<br />Caso não tenha sido você que fez essa requisição, desconsidere este e-mail ou entre em contato com o suporte."
            );

            return Ok();
        }

        // POST api/<controller>/change
        [HttpPost("change")]
        public async Task<IActionResult> ChangeAsync([FromHeader] string authorization = null)
        {
            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            ChangePasswordBody data = JsonConvert.DeserializeObject<ChangePasswordBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            string newPass = Encryption.Decrypt(data.NewPassword);
            string oldPass = Encryption.Decrypt(data.OldPassword);

            if (!Utils.IsValidPassword(data.NewPassword))
                return BadRequest(new ErrorResponse { Error = "Invalid new password" });

            bool withOldPass = oldPass != null;
            bool withResetCode = !string.IsNullOrWhiteSpace(data.ResetCode);

            if (!withOldPass && !withResetCode)
                return BadRequest(new ErrorResponse { Error = "Missing old password or reset code" });

            if (withOldPass && withResetCode)
                return BadRequest(new ErrorResponse { Error = "Requests shouldn't contain old password and reset code" });

            if (withResetCode && string.IsNullOrWhiteSpace(data.Email))
                return BadRequest(new ErrorResponse { Error = "Requests with reset code require email" });

            User user;
            if (withOldPass)
            {
                var validation = Token.ValidateAuthorization(authorization);
                if (!validation.IsValid)
                    return validation.Result;

                user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
                if (user == null)
                    return NotFound(new ErrorResponse { Error = "Token not found" });

                if (user.Password != Hash.Process(oldPass, user.PasswordSalt))
                    return BadRequest(new ErrorResponse { Error = "Old password is wrong" });
            }
            else
            {
                if (Utils.IsValidEmail(data.Email))
                    return BadRequest(new ErrorResponse { Error = "Invalid email" });

                var resetRequest = await _context.ForgotPasswordRequests.Include(x => x.User).FirstOrDefaultAsync(x => x.Code == data.ResetCode);
                if (resetRequest == null)
                    return BadRequest(new ErrorResponse { Error = "Invalid reset code" });

                user = resetRequest.User;
                if (!user.Email.Equals(data.Email, StringComparison.InvariantCultureIgnoreCase))
                    return BadRequest(new ErrorResponse { Error = "Invalid reset code" });

                if (resetRequest.RequestDate < DateTimeOffset.UtcNow.Subtract(Code.LifeSpan))
                    return BadRequest(new ErrorResponse { Error = "Invalid reset code" });

                _context.ForgotPasswordRequests.Remove(resetRequest);
            }

            string salt = Hash.GenerateSalt();
            user.Password = Hash.Process(newPass, salt);
            user.PasswordSalt = salt;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}