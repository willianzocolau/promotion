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
    public class StoreController : Controller
    {
        private readonly DatabaseContext _context;

        public StoreController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        /// <summary>
        /// Get all stores
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <returns>List of stores</returns>
        /// <response code="200">Returns list of stores</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HashSet<StoreResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<StoreResponse>>> GetAllAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(_context.Stores.Select(x => new StoreResponse { Id = x.Id, Name = x.Name }));
        }

        // GET api/<controller>/{id}
        /// <summary>
        /// Get store with the specific id
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Store id</param>
        /// <returns>Store information</returns>
        /// <response code="200">Returns store information</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no store with this id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(StoreResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<StoreResponse>> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return NotFound(new ErrorResponse { Error = "Store not found" });

            return Ok(new StoreResponse { Id = store.Id, Name = store.Name });
        }
    }
}
