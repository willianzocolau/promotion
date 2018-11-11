using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly DatabaseContext _context;

        public OrderController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        /// <summary>
        /// Search orders
        /// </summary>
        /// <remarks>
        /// The user needs permission to administrate the orders, otherwise the user id parameter will be auto filled.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="limit">Max amount of orders that should be returned (must be a value between [1,100])</param>
        /// <param name="afterId">Id of the order that will be before the first object returned (used to paginate)</param>
        /// <param name="userId">User id that made the order</param>
        /// <param name="storeId">Store id that the promotion related to the order is from</param>
        /// <param name="promotionId">Promotion id related to the order</param>
        /// <param name="approved">If the orders should be approved or not</param>
        /// <returns>List of orders</returns>
        /// <response code="200">Returns list of orders that match the parameters</response>
        /// <response code="400">If invalid authorization, or invalid limit</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no order matchs the parameters</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HashSet<OrderResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromQuery] int limit = 25, [FromQuery(Name = "after")] long? afterId = null, [FromQuery(Name = "user_id")] long? userId = null, [FromQuery(Name = "store_id")] long? storeId = null, [FromQuery(Name = "promotion_id")] long? promotionId = null, [FromQuery] bool? approved = null)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (!Utils.CanAdministrateOrders(user.Type))
                userId = user.Id;

            if (limit < 1 || limit > 100)
                return BadRequest(new ErrorResponse { Error = "Invalid limit" });

            IQueryable<Order> orderQuery = _context.Orders;

            if (approved != null)
            {
                if (approved.Value)
                    orderQuery = orderQuery.Where(x => x.ApprovedByUserFK != null);
                else
                    orderQuery = orderQuery.Where(x => x.ApprovedByUserFK == null);
            }

            if (afterId != null)
                orderQuery = orderQuery.Where(x => x.Id > afterId.Value);

            if (userId != null)
                orderQuery = orderQuery.Where(x => x.UserFK == userId.Value);

            if (promotionId != null)
                orderQuery = orderQuery.Where(x => x.PromotionFK == promotionId.Value);

            if (storeId != null)
                orderQuery = orderQuery.Include(x => x.Promotion).Where(x => x.Promotion.StoreFK == storeId.Value);

            List<Order> orders = await orderQuery.Take(limit).ToListAsync();

            if (!orders.Any())
                return NotFound(new ErrorResponse { Error = "No order found" });
            else
                return Ok(orders.Select(x => new OrderResponse { Id = x.Id, RegisterDate = x.RegisterDate, ApprovedByUserFK = x.ApprovedByUserFK, UserFK = x.UserFK, PromotionFK = x.PromotionFK }));
        }

        // POST api/<controller>
        /// <summary>
        /// Register order
        /// </summary>
        /// <remarks>
        /// The authorization token needs to be one from a store.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format (store)</param>
        /// <param name="orderData">Data related to the order to create</param>
        /// <response code="200">Success, returns Order</response>
        /// <response code="400">If invalid authorization, or promotion not from your store</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If promotion is not found, or user is not found</response>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(OrderResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<OrderResponse>> AddOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromBody, Required] AddOrderBody orderData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var store = await _context.Stores.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (store == null)
                return Unauthorized();

            Promotion promotion = await _context.Promotions.FindAsync(orderData.PromotionId);
            if (promotion == null)
                return NotFound(new ErrorResponse { Error = "Promotion not found" });

            if (promotion.StoreFK != store.Id)
                return BadRequest(new ErrorResponse { Error = "Promotion not from your store" });

            User user = await _context.Users.FindAsync(orderData.UserId);
            if (user == null)
                return NotFound(new ErrorResponse { Error = "User not found" });

            Order newOrder = new Order
            {
                RegisterDate = DateTimeOffset.UtcNow,
                ApprovedByUserFK = null,
                PromotionFK = orderData.PromotionId,
                UserFK = orderData.UserId
            };
            _context.Orders.Add(newOrder);

            await _context.SaveChangesAsync();

            return Ok(new OrderResponse
            {
                Id = newOrder.Id,
                RegisterDate = newOrder.RegisterDate,
                PromotionFK = newOrder.PromotionFK,
                UserFK = newOrder.UserFK,
                ApprovedByUserFK = newOrder.ApprovedByUserFK
            });
        }

        // PATCH api/<controller>/{id}/approve
        /// <summary>
        /// Approve order and distribute credits (cashback)
        /// </summary>
        /// <remarks>
        /// Requires permission to approve orders.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Order id</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization, already approved, or linked promotion without cashback percentage</response>
        /// <response code="401">If token is invalid, or no permission to approve</response>
        /// <response code="404">If order is not found</response>
        [HttpPatch("{id}/approve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> ApproveOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null || !Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound(new ErrorResponse { Error = "Order not found" });

            if (order.ApprovedByUserFK != null)
                return BadRequest(new ErrorResponse { Error = "Order already approved" });

            if (!order.Promotion.CashbackPercentage.HasValue)
                return BadRequest(new ErrorResponse { Error = "Promotion linked to this order has no cashback percentage" });

            double totalCashback = order.Promotion.Price * order.Promotion.CashbackPercentage.Value;
            order.User.Credit += Utils.GetBuyerCashback(totalCashback);
            order.Promotion.User.Credit += Utils.GetSellerCashback(totalCashback);

            order.ApprovedByUserFK = user.Id;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PATCH api/<controller>/{id}/disapprove
        /// <summary>
        /// Disappove order and remove the distributed credits (cashback)
        /// </summary>
        /// <remarks>
        /// Requires permission to disapprove orders.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Order id</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization, or not approved</response>
        /// <response code="401">If token is invalid, or no permission to approve</response>
        /// <response code="404">If order is not found</response>
        [HttpPatch("{id}/disapprove")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> DisapproveOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null || !Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound(new ErrorResponse { Error = "Order not found" });

            if (order.ApprovedByUserFK == null)
                return BadRequest(new ErrorResponse { Error = "Order isn't approved" });

            double totalCashback = order.Promotion.Price * order.Promotion.CashbackPercentage.Value;
            order.User.Credit -= Utils.GetBuyerCashback(totalCashback);
            order.Promotion.User.Credit -= Utils.GetSellerCashback(totalCashback);

            order.ApprovedByUserFK = null;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<controller>/{id}
        /// <summary>
        /// Delete order
        /// </summary>
        /// <remarks>
        /// Requires permission to delete orders.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Order id</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or no permission to delete</response>
        /// <response code="404">If no order is found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found");

            if (!Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/<controller>/{id}/vote
        /// <summary>
        /// Vote for order (upvote/downvote)
        /// </summary>
        /// <remarks>
        /// Requires to be the one that did the order.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Order id</param>
        /// <param name="voteData">Data needed to add the vote</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization, already voted, or comment is too long</response>
        /// <response code="401">If token is invalid, or order user id and token user id don't match</response>
        /// <response code="404">If no order is found</response>
        [HttpPost("{id}/vote")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> VoteAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id, [FromBody, Required] VoteBody voteData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found");

            if (order.UserFK != user.Id)
                return Unauthorized();

            if (order.IsVotePositive != null)
                return BadRequest("Already voted");

            if (voteData.Comment.Length > 400)
                return BadRequest("Comment is too long");

            order.IsVotePositive = voteData.IsPositive;
            order.Comment = voteData.Comment;
            order.CommentRegisterDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/<controller>/{id}/answer
        /// <summary>
        /// Answer a vote (upvote/downvote)
        /// </summary>
        /// <remarks>
        /// Requires to be the one that posted the promotion.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Order id</param>
        /// <param name="answerData">Data needed to add the reply</param>
        /// <response code="200">Success</response>
        /// <response code="400">If invalid authorization, already answered, or comment is too long</response>
        /// <response code="401">If token is invalid, or promotion user id and token user id don't match</response>
        /// <response code="404">If no order is found</response>
        [HttpPost("{id}/answer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AnswerAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id, [FromBody, Required] AnswerBody answerData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found");

            if (order.Promotion.UserFK != user.Id)
                return Unauthorized();

            if (order.Answer != null)
                return BadRequest("Already voted");

            if (answerData.Answer.Length > 400)
                return BadRequest("Answer is too long");

            order.Answer = answerData.Answer;
            order.AnswerRegisterDate = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
