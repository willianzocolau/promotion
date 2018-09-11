using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
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
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromHeader] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return BadRequest(new { error = "Missing header: authorization" });

            if (authorization.StartsWith("Basic "))
            {
                string encodedString = authorization.Substring(6);
                byte[] data = Convert.FromBase64String(encodedString);
                string decodedString = Encoding.UTF8.GetString(data);

                if (!decodedString.Contains(':'))
                    return BadRequest(new { error = "Invalid authorization" });

                string[] splitString = decodedString.Split(':', 2);
                string email = splitString[0];
                string password = splitString[1];

                string body;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                    body = await reader.ReadToEndAsync();

                RegisterUser userData = JsonConvert.DeserializeObject<RegisterUser>(body);
                if (userData == null)
                    return BadRequest(new { error = "Invalid json" });

                if (!Utils.IsValidEmail(email))
                    return BadRequest(new { error = "Invalid email" });
                if (!Utils.IsValidPassword(password))
                    return BadRequest(new { error = "Invalid password" });
                if (!Utils.IsValidNickname(userData.Nickname))
                    return BadRequest(new { error = "Invalid nickname" });
                if (!Utils.IsValidName(userData.Name))
                    return BadRequest(new { error = "Invalid name" });
                if (!Utils.IsValidCpf(userData.Cpf))
                    return BadRequest(new { error = "Invalid cpf" });

                bool alreadyUsedEmail = await _context.Users.AnyAsync(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                if (alreadyUsedEmail)
                    return BadRequest(new { error = "Already used email" });

                bool alreadyUsedNickname = await _context.Users.AnyAsync(x => x.Nickname.Equals(userData.Nickname, StringComparison.InvariantCultureIgnoreCase));
                if (alreadyUsedNickname)
                    return BadRequest(new { error = "Already used nickname" });

                string token = Token.Generate();
                _context.Users.Add(new User
                {
                    Nickname = userData.Nickname,
                    Email = email,
                    Password = password,
                    Name = userData.Name,
                    Credit = 0,
                    StateFK = null,
                    Type = 0,
                    RegisterDate = DateTimeOffset.UtcNow,
                    Token = token,
                    Cpf = userData.Cpf,
                    ImageUrl = null,
                    Cellphone = null,
                    Telephone = null,
                });
                await _context.SaveChangesAsync();

                return Ok(new { token });
            }

            return BadRequest(new { error = "Invalid authorization" });
        }

        // POST api/<controller>/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromHeader] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return BadRequest(new { error = "Missing header: authorization" });

            if (authorization.StartsWith("Basic "))
            {
                string encodedString = authorization.Substring(6);
                byte[] data = Convert.FromBase64String(encodedString);
                string decodedString = Encoding.UTF8.GetString(data);

                if (!decodedString.Contains(':'))
                    return BadRequest(new { error = "Invalid authorization" });

                string[] splitString = decodedString.Split(':', 2);
                string email = splitString[0];
                string password = splitString[1];

                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);

                if (user == null)
                    return NotFound(new { error = "Login not found" });
                else
                {
                    string token = Token.Generate();
                    user.Token = token;
                    await _context.SaveChangesAsync();

                    return Ok(new { id = user.Id, token });
                }
            }

            return BadRequest(new { error = "Invalid authorization" });
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
                return NotFound(new { error = "Token not found" });
            else
            {
                if (!Token.IsValid(validation.Token))
                {
                    user.Token = null;
                    await _context.SaveChangesAsync();

                    return BadRequest(new { error = "Invalid token" });
                }
                else
                {
                    string newtoken = Token.Generate();

                    user.Token = newtoken;
                    await _context.SaveChangesAsync();

                    return Ok(new { token = newtoken });
                }
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
    }
}