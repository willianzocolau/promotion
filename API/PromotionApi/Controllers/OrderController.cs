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
        /// Search all orders
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
        /// <response code="200">Returns the list of orders that match the parameters</response>
        /// <response code="400">If invalid authorization, or invalid limit</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no order matchs the parameters</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(HashSet<OrderResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetOrdersAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromQuery] int limit = 25, [FromQuery(Name = "after")] long? afterId = null, [FromQuery(Name = "user_id")] long? userId = null, [FromQuery(Name = "store_id")] long? storeId = null, [FromQuery(Name = "promotion_id")] long? promotionId = null, [FromQuery] bool? approved = null)
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
        [HttpPost]
        public async Task<IActionResult> AddOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var store = await _context.Stores.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (store == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            AddOrderBody data = JsonConvert.DeserializeObject<AddOrderBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            Promotion promotion = await _context.Promotions.FindAsync(data.PromotionId);
            if (promotion == null)
                return NotFound(new ErrorResponse { Error = "Promotion not found" });

            if (promotion.StoreFK != store.Id)
                return BadRequest(new ErrorResponse { Error = "Promotion not from your store" });

            User user = await _context.Users.FindAsync(data.UserId);
            if (user == null)
                return NotFound(new ErrorResponse { Error = "User not found" });

            _context.Orders.Add(new Order
            {
                RegisterDate = DateTimeOffset.UtcNow,
                ApprovedByUserFK = null,
                PromotionFK = data.PromotionId,
                UserFK = data.UserId
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PATCH api/<controller>/{id}/approve
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null || !Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            Order order = await _context.Orders.Include(x => x.User).Include(x => x.Promotion).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return BadRequest("Invalid order");

            if (order.ApprovedByUserFK != null)
                return BadRequest("Order already approved");

            if (!order.Promotion.CashbackPercentage.HasValue)
                return BadRequest("Promotion linked to this order has no cashback percentage");

            double totalCashback = order.Promotion.Price * order.Promotion.CashbackPercentage.Value;
            order.User.Credit += Utils.GetBuyerCashback(totalCashback);
            order.Promotion.User.Credit += Utils.GetSellerCashback(totalCashback);

            order.ApprovedByUserFK = user.Id;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PATCH api/<controller>/{id}/disapprove
        [HttpPatch("{id}/disapprove")]
        public async Task<IActionResult> DisapproveOrderAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null || !Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            Order order = await _context.Orders.Include(x => x.User).Include(x => x.Promotion).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return BadRequest("Invalid order");

            if (order.ApprovedByUserFK == null)
                return BadRequest("Order isn't approved");

            double totalCashback = order.Promotion.Price * order.Promotion.CashbackPercentage.Value;
            order.User.Credit -= Utils.GetBuyerCashback(totalCashback);
            order.Promotion.User.Credit -= Utils.GetSellerCashback(totalCashback);

            order.ApprovedByUserFK = null;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<controller>/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
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
    }
}
