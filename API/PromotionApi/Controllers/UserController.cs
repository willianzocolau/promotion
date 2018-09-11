using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetOwnAsync([FromHeader] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(new { id = user.Id, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type, credit = user.Credit, email = user.Email, name = user.Name, state = (user.StateFK == null ? null : new { id = user.StateFK, name = user.State?.Name }) });
        }

        // GET: api/<controller>/search/{nickname}
        [HttpGet("search/{nickname}")]
        public async Task<IActionResult> SearchAsync([FromHeader] string authorization, [FromRoute] string nickname)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var users = new List<User>();
            var equalUser = await _context.Users.FirstOrDefaultAsync(x => x.Nickname.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
            if (equalUser != null)
            {
                users.Add(equalUser);
                users.AddRange(_context.Users.Where(x => x.Nickname.Contains(nickname, StringComparison.InvariantCultureIgnoreCase) && x.Id != equalUser.Id).Take(9).ToList());
            }
            else
                users.AddRange(_context.Users.Where(x => x.Nickname.Contains(nickname, StringComparison.InvariantCultureIgnoreCase)).Take(10).ToList());
            if (!users.Any())
                return NotFound(new { error = "No users found" });
            else
                return Ok(users.Select(x => new { id = x.Id, nickname = x.Nickname }));
        }

        // GET api/<controller>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromHeader] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(new { id = user.Id, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type });
        }
    }
}
