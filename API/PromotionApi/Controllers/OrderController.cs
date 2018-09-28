using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PromotionApi.Data;
using PromotionApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionApi.Controllers
{
    [Produces("application/json")]
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
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync([FromHeader] string authorization, [FromQuery] int limit = 25, [FromQuery(Name = "after")] long? afterId = null, [FromQuery(Name = "user_id")] long? userId = null, [FromQuery(Name = "store_id")] long? storeId = null, [FromQuery(Name = "promotion_id")] long? promotionId = null, [FromQuery] bool? approved = null)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null || !Utils.CanAdministrateOrders(user.Type))
                return Unauthorized();

            if (limit < 0 || limit > 100)
                return BadRequest(new { error = "Invalid limit" });

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
                return NotFound(new { error = "No promotion found" });
            else
                return Ok(orders.Select(x => new { id = x.Id, date = x.RegisterDate, approved_by = x.ApprovedByUserFK, user_id = x.UserFK, promotion_id = x.PromotionFK }));
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> AddOrderAsync([FromHeader] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var store = await _context.Stores.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (store == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            AddOrderBody data = JsonConvert.DeserializeObject<AddOrderBody>(body);
            if (data == null)
                return BadRequest(new { error = "Invalid json" });

            Promotion promotion = await _context.Promotions.FindAsync(data.PromotionId);
            if (promotion == null)
                return NotFound(new { error = "Promotion not found" });

            if (promotion.StoreFK != store.Id)
                return BadRequest(new { error = "Promotion not from your store" });

            User user = await _context.Users.FindAsync(data.UserId);
            if (user == null)
                return NotFound(new { error = "User not found" });

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
        public async Task<IActionResult> ApproveOrderAsync([FromHeader] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

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
        public async Task<IActionResult> DisapproveOrderAsync([FromHeader] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

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
        public async Task<IActionResult> DeleteAsync([FromHeader] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

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
