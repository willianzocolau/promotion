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
    public class PromotionController : Controller
    {
        private readonly DatabaseContext _context;

        public PromotionController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetPromotionsAsync([FromHeader] string authorization, [FromQuery] int limit = 50, [FromQuery(Name = "after")] long? afterId = null, [FromQuery(Name = "user_id")] long? userId = null, [FromQuery(Name = "store_id")] long? storeId = null, [FromQuery(Name = "state_id")] long? stateId = null)
        {
            //TODO: Add price (<x, >x, x-y), add name?
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (limit < 0 || limit > 100)
                return BadRequest(new { error = "Invalid limit" });

            IQueryable<Promotion> promotionQuery = _context.Promotions;

            if (afterId != null)
                promotionQuery = promotionQuery.Where(x => x.Id > afterId.Value);

            if (userId != null)
                promotionQuery = promotionQuery.Where(x => x.UserFK == userId.Value);

            if (storeId != null)
                promotionQuery = promotionQuery.Where(x => x.StoreFK == storeId.Value);

            if (stateId != null)
                promotionQuery = promotionQuery.Where(x => x.StateFK == stateId.Value);

            List<Promotion> promotions = await promotionQuery.Take(limit).ToListAsync();

            if (!promotions.Any())
                return NotFound(new { error = "No promotion found" });
            else
                return Ok(promotions.Select(x => new { id = x.Id, name = x.Name, image_url = x.ImageUrl, price = x.Price, user_id = x.UserFK, store_id = x.StoreFK, state_id = x.StateFK }));
        }

        // POST api/<controller>/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromHeader] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            RegisterPromotionBody promotionData = JsonConvert.DeserializeObject<RegisterPromotionBody>(body);
            if (promotionData == null)
                return BadRequest(new { error = "Invalid json" });

            if (string.IsNullOrWhiteSpace(promotionData.Name))
                return BadRequest(new { error = "Invalid name" });
            if (promotionData.Price <= 0)
                return BadRequest(new { error = "Invalid price" });
            if (string.IsNullOrWhiteSpace(promotionData.ImageUrl))
                return BadRequest(new { error = "Invalid image url" });
            //TODO: Add expire_date
            //TODO: Uncomment following section
            /*if (!await _context.States.AnyAsync(x => x.Id == promotionData.StateFK))
                return BadRequest(new { error = "Invalid state id" });
            if (!await _context.Stores.AnyAsync(x => x.Id == promotionData.StoreFK))
                return BadRequest(new { error = "Invalid state id" });*/

            _context.Promotions.Add(new Promotion
            {
                Name = promotionData.Name,
                Price = promotionData.Price,
                RegisterDate = DateTimeOffset.UtcNow,
                ExpireDate = DateTimeOffset.UtcNow.AddDays(7),
                ImageUrl = promotionData.ImageUrl,
                StoreFK = promotionData.StoreFK,
                UserFK = user.Id,
                StateFK = promotionData.StateFK,
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: api/<controller>/search/{name}
        [HttpGet("search/{name}")]
        public async Task<IActionResult> SearchAsync([FromHeader] string authorization, [FromRoute] string name)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return validation.Result;

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var promotions = new List<Promotion>();
            var equalPromotion = await _context.Promotions.FirstOrDefaultAsync(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (equalPromotion != null)
            {
                promotions.Add(equalPromotion);
                promotions.AddRange(_context.Promotions.Where(x => x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase) && x.Id != equalPromotion.Id).Take(9).ToList());
            }
            else
                promotions.AddRange(_context.Promotions.Where(x => x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase)).Take(10).ToList());
            
            if (!promotions.Any())
                return NotFound(new { error = "No promotion found" });
            else
                return Ok(promotions.Select(x => new { id = x.Id, name = x.Name, image_url = x.ImageUrl, price = x.Price }));
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

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return NotFound("Promotion not found");

            return Ok(new { id = promotion.Id, price = promotion.Price, image_url = promotion.ImageUrl, register_date = promotion.RegisterDate, expire_date = promotion.ExpireDate });
        }
    }
}
