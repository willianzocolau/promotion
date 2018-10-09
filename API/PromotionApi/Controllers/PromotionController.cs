using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
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
    public class PromotionController : Controller
    {
        private readonly DatabaseContext _context;

        public PromotionController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        /// <summary>
        /// Search promotions
        /// </summary>
        /// <remarks>
        /// Order by requires specific values, check them at the parameter.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="limit">Max amount of promotions that should be returned (must be a value between [1,100])</param>
        /// <param name="afterId">Id of the promotion that will be before the first object returned (used to paginate)</param>
        /// <param name="userId">User id that posted the promotion</param>
        /// <param name="storeId">Store id that the promotion is related to</param>
        /// <param name="stateId">State id that the promotion is located</param>
        /// <param name="priceLessThan">Promotion price will be less than this value</param>
        /// <param name="priceGreaterThan">Promotion price will be greater than this value</param>
        /// <param name="name">Promotion name or part of it</param>
        /// <param name="orderBy">Order results by (valid values: ...)</param> //TODO: Add values
        /// <returns>List of promotions</returns>
        /// <response code="200">Returns list of promotions that match the parameters</response>
        /// <response code="400">If invalid authorization, or invalid limit</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no promotion is found</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HashSet<PromotionResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<PromotionResponse>>> GetPromotionsAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromQuery] int limit = 25, [FromQuery(Name = "after")] long? afterId = null, [FromQuery(Name = "user_id")] long? userId = null, [FromQuery(Name = "store_id")] long? storeId = null, [FromQuery(Name = "state_id")] long? stateId = null, [FromQuery] int? priceLessThan = null, [FromQuery] int? priceGreaterThan = null, [FromQuery] string name = null, [FromQuery] string orderBy = null)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (limit < 1 || limit > 100)
                return BadRequest(new ErrorResponse { Error = "Invalid limit" });

            IQueryable<Promotion> promotionQuery = _context.Promotions.Where(x => x.Active);

            if (orderBy != null)
            {
                promotionQuery = promotionQuery.OrderByDescending(x => x.RegisterDate);
                //TODO: order by
            }

            if (afterId != null)
                promotionQuery = promotionQuery.Where(x => x.Id > afterId.Value);

            if (userId != null)
                promotionQuery = promotionQuery.Where(x => x.UserFK == userId.Value);

            if (storeId != null)
                promotionQuery = promotionQuery.Where(x => x.StoreFK == storeId.Value);

            if (stateId != null)
                promotionQuery = promotionQuery.Where(x => x.StateFK == stateId.Value);

            if (priceLessThan != null)
                promotionQuery = promotionQuery.Where(x => x.Price < priceLessThan.Value);

            if (priceGreaterThan != null)
                promotionQuery = promotionQuery.Where(x => x.Price > priceGreaterThan.Value);

            if (name != null)
                promotionQuery = promotionQuery.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));

            List<Promotion> promotions = await promotionQuery.Take(limit).ToListAsync();

            if (!promotions.Any())
                return NotFound(new ErrorResponse { Error = "No promotion found" });
            else
                return Ok(promotions.Select(x => new PromotionResponse { Id = x.Id, Name = x.Name, RegisterDate = x.RegisterDate, ImageUrl = x.ImageUrl, Price = x.Price, UserFK = x.UserFK, StoreFK = x.StoreFK, StateFK = x.StateFK, ExpireDate = x.ExpireDate, Active = x.Active }));
        }

        // POST api/<controller>/register
        /// <summary>
        /// Register a promotion
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="promotionData">Data related to the promotion to create</param>
        /// <returns>Promotion</returns>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization, invalid name, invalid price, invalid image url, or invalid state id</response>
        /// <response code="401">If token is invalid</response>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> RegisterAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromBody] RegisterPromotionBody promotionData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(promotionData.Name))
                return BadRequest(new ErrorResponse { Error = "Invalid name" });
            if (promotionData.Price <= 0)
                return BadRequest(new ErrorResponse { Error = "Invalid price" });
            if (string.IsNullOrWhiteSpace(promotionData.ImageUrl))
                return BadRequest(new ErrorResponse { Error = "Invalid image url" });
            if (!await _context.States.AnyAsync(x => x.Id == promotionData.StateFK))
                return BadRequest(new ErrorResponse { Error = "Invalid state id" });
            //TODO: Uncomment following section
            /*if (!await _context.Stores.AnyAsync(x => x.Id == promotionData.StoreFK))
                return BadRequest(new ErrorResponse { Error = "Invalid state id" });*/
            //TODO: Add expire_date

            _context.Promotions.Add(new Promotion
            {
                Name = promotionData.Name,
                Price = promotionData.Price,
                RegisterDate = DateTimeOffset.UtcNow,
                ExpireDate = DateTimeOffset.UtcNow.AddDays(7),
                Active = true,
                ImageUrl = promotionData.ImageUrl,
                StoreFK = promotionData.StoreFK,
                UserFK = user.Id,
                StateFK = promotionData.StateFK,
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/{id}
        /// <summary>
        /// Get promotion information by id
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Promotion id</param>
        /// <returns>Promotion</returns>
        /// <response code="200">Returns promotion</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no promotion is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PromotionResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<PromotionResponse>> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return NotFound("Promotion not found");

            return Ok(new PromotionResponse { Id = promotion.Id, Name = promotion.Name, Price = promotion.Price, ImageUrl = promotion.ImageUrl, RegisterDate = promotion.RegisterDate, UserFK = promotion.UserFK, StateFK = promotion.StateFK, StoreFK = promotion.StoreFK, ExpireDate = promotion.ExpireDate, Active = promotion.Active });
        }

        // DELETE api/<controller>/{id}
        /// <summary>
        /// Delete promotion
        /// </summary>
        /// <remarks>
        /// Requires the owner or permission to delete promotions from other users.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Promotion id</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or no permission to delete</response>
        /// <response code="404">If no promotion is found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> DeleteAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return NotFound("Promotion not found");

            if (promotion.UserFK != user.Id && !Utils.CanDeletePromotion(user.Type))
                return Unauthorized();

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
