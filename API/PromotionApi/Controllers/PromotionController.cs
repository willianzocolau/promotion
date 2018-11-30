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
        /// <param name="orderBy">Order results by (valid values: REGISTERDATE_DESC (default), REGISTERDATE_ASC, PRICE_DESC, PRICE_ASC, EXPIREDATE_DESC, EXPIREDATE_ASC)</param>
        /// <returns>List of promotions</returns>
        /// <response code="200">Returns list of promotions that match the parameters</response>
        /// <response code="400">If invalid authorization, invalid limit, or invalid order by</response>
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

            if (orderBy == null)
            {
                promotionQuery = promotionQuery.OrderByDescending(x => x.RegisterDate);
            }
            else
            {
                switch(orderBy)
                {
                    case "REGISTERDATE_DESC":
                        promotionQuery = promotionQuery.OrderByDescending(x => x.RegisterDate);
                        break;
                    case "REGISTERDATE_ASC":
                        promotionQuery = promotionQuery.OrderBy(x => x.RegisterDate);
                        break;
                    case "PRICE_DESC":
                        promotionQuery = promotionQuery.OrderByDescending(x => x.Price);
                        break;
                    case "PRICE_ASC":
                        promotionQuery = promotionQuery.OrderBy(x => x.Price);
                        break;
                    case "EXPIREDATE_DESC":
                        promotionQuery = promotionQuery.OrderByDescending(x => x.ExpireDate);
                        break;
                    case "EXPIREDATE_ASC":
                        promotionQuery = promotionQuery.OrderBy(x => x.ExpireDate);
                        break;
                    default:
                        return BadRequest(new ErrorResponse { Error = "Invalid order by" });
                }
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

            List<Promotion> promotions = await promotionQuery.Include(x => x.Orders).Take(limit).ToListAsync();

            if (!promotions.Any())
                return NotFound(new ErrorResponse { Error = "No promotion found" });
            else
            {
                var result = new List<PromotionResponse>();
                foreach (var promotion in promotions)
                {
                    var totalVotes = promotion.Orders.Count(x => x.IsVotePositive != null);
                    var positiveVotes = promotion.Orders.Count(x => x.IsVotePositive == true);
                    result.Add(new PromotionResponse
                    {
                        Id = promotion.Id,
                        Name = promotion.Name,
                        RegisterDate = promotion.RegisterDate,
                        ImageUrl = promotion.ImageUrl,
                        Price = promotion.Price,
                        UserFK = promotion.UserFK,
                        StoreFK = promotion.StoreFK,
                        StateFK = promotion.StateFK,
                        ExpireDate = promotion.ExpireDate,
                        Active = promotion.Active,
                        TotalOrders = promotion.Orders.Count,
                        OrderUpvotes = positiveVotes,
                        OrderDownvotes = totalVotes - positiveVotes,
                        CashbackPercentage = promotion.CashbackPercentage
                    });
                }
                return Ok(result);
            }
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

            Promotion created;
            _context.Promotions.Add(created = new Promotion
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

            _ = Task.Run(async () =>
            {
                //TODO: Check better approach?
                var allWishItems = _context.Wishlist.Where(x => promotionData.Name.ToLower().Contains(x.Name.ToLower()));
                foreach (var wish in allWishItems)
                {
                    _context.Matchs.Add(new MatchItem
                    {
                        IsActive = true,
                        RegisterDate = DateTimeOffset.UtcNow,
                        PromotionFK = created.Id,
                        UserFK = wish.UserFK
                    });
                }

                await _context.SaveChangesAsync();
            });

            return Ok();
        }

        // GET api/<controller>/{id}
        /// <summary>
        /// Get promotion information by id
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Promotion id</param>
        /// <returns>Promotion</returns>
        /// <response code="200">Returns promotion with votes</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no promotion is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PromotionWithVotesResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<PromotionWithVotesResponse>> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var promotion = await _context.Promotions.Include(x => x.Orders).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (promotion == null)
                return NotFound("Promotion not found");

            var totalVotes = promotion.Orders.Count(x => x.IsVotePositive != null);
            var positiveVotes = promotion.Orders.Count(x => x.IsVotePositive == true);

            return Ok(new PromotionWithVotesResponse
            {
                Id = promotion.Id,
                Name = promotion.Name,
                Price = promotion.Price,
                ImageUrl = promotion.ImageUrl,
                RegisterDate = promotion.RegisterDate,
                UserFK = promotion.UserFK,
                StateFK = promotion.StateFK,
                StoreFK = promotion.StoreFK,
                ExpireDate = promotion.ExpireDate,
                Active = promotion.Active,
                TotalOrders = promotion.Orders.Count,
                OrderUpvotes = positiveVotes,
                OrderDownvotes = totalVotes - positiveVotes,
                Votes = promotion.Orders.Where(x => x.IsVotePositive != null).Select(x => new VoteResponse
                {
                    IsPositive = x.IsVotePositive.Value,
                    Comment = x.Comment,
                    CommentRegisterDate = x.CommentRegisterDate.Value,
                    Answer = x.Answer,
                    AnswerRegisterDate = x.AnswerRegisterDate,
                    OrderId = x.Id,
                    User = new UserResponse
                    {
                        Id = x.User.Id,
                        Nickname = x.User.Nickname,
                        ImageUrl = x.User.ImageUrl,
                        RegisterDate = x.User.RegisterDate,
                        Type = x.User.Type
                    }
                }).ToHashSet()
            });
        }

        // DELETE api/<controller>/{id}
        /// <summary>
        /// Delete promotion
        /// </summary>
        /// <remarks>
        /// Requires permission to delete promotions.
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

            if (!Utils.CanDeletePromotion(user.Type))
                return Unauthorized();

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/{id}/orders
        /// <summary>
        /// Get orders from a promotion
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Promotion id</param>
        /// <returns>Promotion Orders</returns>
        /// <response code="200">Returns the orders of a promotion with users (without seller profile)</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or not your promotion</response>
        /// <response code="404">If no promotion is found</response>
        [HttpGet("{id}/orders")]
        [ProducesResponseType(200, Type = typeof(HashSet<OrderWithUserResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<HashSet<OrderWithUserResponse>>> GetOrdersAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var promotion = await _context.Promotions.Include(x => x.Orders).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (promotion == null)
                return NotFound("Promotion not found");

            if (promotion.UserFK != user.Id)
                return Unauthorized();

            return Ok(promotion.Orders.Select(x => new OrderWithUserResponse
            {
                Id = x.Id,
                RegisterDate = x.RegisterDate,
                PromotionFK = x.PromotionFK,
                UserFK = x.UserFK,
                User = new UserResponse
                {
                    Id = x.User.Id,
                    Nickname = x.User.Nickname,
                    ImageUrl = x.User.ImageUrl,
                    RegisterDate = x.User.RegisterDate,
                    Type = x.User.Type,
                    SellerProfile = null
                },
                ApprovedByUserFK = x.ApprovedByUserFK
            }).ToHashSet());
        }

        // PATCH api/<controller>/{id}
        /// <summary>
        /// Change promotion
        /// </summary>
        /// <remarks>
        /// Requires to be the owner of the promotion.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Promotion id</param>
        /// <param name="editPromotionData">Information to edit from this promotion</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or no permission to delete</response>
        /// <response code="404">If no promotion is found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> ModifyAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id, [FromBody] EditPromotionBody editPromotionData)
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

            if (promotion.UserFK != user.Id)
                return Unauthorized();

            if (editPromotionData.Active != null)
            {
                promotion.Active = editPromotionData.Active.Value;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
