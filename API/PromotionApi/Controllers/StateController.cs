using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromotionApi.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : Controller
    {
        private readonly DatabaseContext _context;

        public StateController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(_context.States.Select(x => new { id = x.Id, name = x.Name }));
        }

        // GET api/<controller>/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var state = await _context.States.FindAsync(id);
            if (state == null)
                return NotFound("State not found");

            return Ok(new { id = state.Id, name = state.Name });
        }
    }
}
