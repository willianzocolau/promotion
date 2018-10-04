using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromotionApi.Data;
using PromotionApi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
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
        /// <summary>
        /// Get all states
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <returns>List of states</returns>
        /// <response code="200">Returns list of states</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HashSet<StateResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<StateResponse>>> GetAllAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(_context.States.Select(x => new StateResponse { Id = x.Id, Name = x.Name }));
        }

        // GET api/<controller>/{id}
        /// <summary>
        /// Get state with the specific id
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">State id</param>
        /// <returns>State information</returns>
        /// <response code="200">Returns state information</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no state with this id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(StateResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<StateResponse>> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var state = await _context.States.FindAsync(id);
            if (state == null)
                return NotFound(new ErrorResponse { Error = "State not found" });

            return Ok(new StateResponse { Id = state.Id, Name = state.Name });
        }
    }
}
