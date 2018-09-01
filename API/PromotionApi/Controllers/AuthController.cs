using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
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
        [HttpPost("register/{nickname}")]
        public async Task<IActionResult> RegisterAsync([FromHeader] string authorization, [FromRoute] string nickname)
        {
            if (string.IsNullOrWhiteSpace(authorization))
                return BadRequest(new { error = "Missing header: authorization" });
            if (string.IsNullOrWhiteSpace(nickname))
                return BadRequest(new { error = "Missing parameter: nickname" });

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

                if (!Utils.IsValidEmail(email))
                    return BadRequest(new { error = "Invalid email" });
                if (!Utils.IsValidNickname(nickname))
                    return BadRequest(new { error = "Invalid nickname" });
                if (!Utils.IsValidPassword(password))
                    return BadRequest(new { error = "Invalid password" });

                bool alreadyUsedEmail = await _context.Users.AnyAsync(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                if (alreadyUsedEmail)
                    return BadRequest(new { error = "Already used email" });

                bool alreadyUsedNickname = await _context.Users.AnyAsync(x => x.Nickname.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
                if (alreadyUsedNickname)
                    return BadRequest(new { error = "Already used nickname" });

                string token = Token.Generate();
                _context.Users.Add(new User
                {
                    Nickname = nickname,
                    Email = email,
                    Password = password,
                    Name = null,
                    Credit = 0,
                    StateFK = 0,
                    Type = 0,
                    RegisterDate = DateTimeOffset.UtcNow,
                    Token = token,
                    Cpf = null,
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