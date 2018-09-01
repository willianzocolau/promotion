using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromotionApi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> GetOwnAsync([FromHeader] string authorization) //TODO: Add estados no State
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var user = await _context.Users/*.Include(x => x.State)*/.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(new { id = user.Id, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type, credit = user.Credit, email = user.Email, name = user.Name/*, stateName = user.State.Name*/ });
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

            var equalUser = await _context.Users.FirstOrDefaultAsync(x => x.Nickname.Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
            if (equalUser == null)
            {
                var approxUsers = _context.Users.Where(x => x.Nickname.Contains(nickname, StringComparison.InvariantCultureIgnoreCase)).Take(10);
                if (!approxUsers.Any())
                    return NotFound(new { error = "No users found" });
                else
                    return Ok(approxUsers.Select(x => new { id = x.Id, nickname = x.Nickname }));
            }
            else
                return Ok(new[] { equalUser }.Select(x => new { id = x.Id, nickname = x.Nickname }));
        }

        // GET api/<controller>/5
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
