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
        public async Task<IActionResult> GetOwnAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.Include(x => x.State).FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(new { id = user.Id, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type, credit = user.Credit, email = user.Email, name = user.Name, state = user.StateFK });
        }

        // GET api/<controller>/search/{nickname}
        [HttpGet("search/{nickname}")]
        public async Task<IActionResult> SearchAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] string nickname)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var users = new List<User>();
            var equalUser = await _context.Users.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Nickname, $"{nickname}"));
            if (equalUser != null)
            {
                users.Add(equalUser);
                users.AddRange(_context.Users.Where(x => x.Id != equalUser.Id && EF.Functions.ILike(x.Nickname, $"%{nickname}%")).Take(9).ToList());
            }
            else
                users.AddRange(_context.Users.Where(x => EF.Functions.ILike(x.Nickname, $"%{nickname}%")).Take(10).ToList());
            if (!users.Any())
                return NotFound(new ErrorResponse { Error = "No users found" });
            else
                return Ok(users.Select(x => new { id = x.Id, nickname = x.Nickname }));
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

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new ErrorResponse { Error = "User not found" });

            return Ok(new { id = user.Id, nickname = user.Nickname, image_url = user.ImageUrl, register_date = user.RegisterDate, type = user.Type });
        }

        // PATCH api/<controller>/edit
        [HttpPatch("edit")]
        public async Task<IActionResult> EditAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            EditUserBody data = JsonConvert.DeserializeObject<EditUserBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            if (data.Name != null)
            {
                if (!Utils.IsValidName(data.Name))
                    return BadRequest(new ErrorResponse { Error = "Invalid name" });
                user.Name = data.Name;
            }

            if (data.Cpf != null)
            {
                if (!Utils.IsValidCpf(data.Cpf))
                    return BadRequest(new ErrorResponse { Error = "Invalid cpf" });
                user.Cpf = data.Cpf;
            }

            if (data.Telephone != null)
            {
                if (!Utils.IsValidTelephone(data.Telephone))
                    return BadRequest(new ErrorResponse { Error = "Invalid telephone" });
                user.Telephone = data.Telephone;
            }

            if (data.Cellphone != null)
            {
                if (!Utils.IsValidTelephone(data.Cellphone))
                    return BadRequest(new ErrorResponse { Error = "Invalid cellphone" });
                user.Cellphone = data.Cellphone;
            }

            if (data.ImageUrl != null)
            {
                if (!Utils.IsValidImageUrl(data.ImageUrl))
                    return BadRequest(new ErrorResponse { Error = "Invalid image url" });
                user.ImageUrl = data.ImageUrl;
            }

            if (data.Nickname != null)
            {
                if (!Utils.IsValidNickname(data.Nickname))
                    return BadRequest(new ErrorResponse { Error = "Invalid nickname" });
                bool alreadyUsedNickname = await _context.Users.AnyAsync(x => EF.Functions.ILike(x.Nickname, data.Nickname));
                if (alreadyUsedNickname)
                    return BadRequest(new ErrorResponse { Error = "Already used nickname" });
                user.Nickname = data.Nickname;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/wishlist
        [HttpGet("wishlist")]
        public async Task<IActionResult> GetOwnWishListAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            ICollection<WishItem> wishList = await user.GetWishListAsync();

            return Ok(wishList.Select(x => new { id = x.Id, name = x.Name, register_date = x.RegisterDate }));
        }

        // POST api/<controller>/wishlist
        [HttpPost("wishlist")]
        public async Task<IActionResult> AddWishItemAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            AddWishItemBody data = JsonConvert.DeserializeObject<AddWishItemBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            ICollection<WishItem> wishList = await user.GetWishListAsync();
            wishList.Add(new WishItem
            {
                Name = data.Name,
                RegisterDate = DateTimeOffset.UtcNow,
                UserFK = user.Id
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/wishlist/{id}
        [HttpGet("wishlist/{id}")]
        public async Task<IActionResult> GetOwnWishListAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            var wishItem = await _context.WishList.FirstOrDefaultAsync(x => x.Id == id);
            if (wishItem == null)
                return NotFound(new ErrorResponse { Error = "Id not found" });

            if (user.Id != wishItem.UserFK)
                return Unauthorized();

            return Ok(new { id = wishItem.Id, name = wishItem.Name, register_date = wishItem.RegisterDate });
        }

        // PATCH api/<controller>/wishlist/{id}
        [HttpPatch("wishlist/{id}")]
        public async Task<IActionResult> EditWishItemAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            AddWishItemBody data = JsonConvert.DeserializeObject<AddWishItemBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            var wishItem = await _context.WishList.FirstOrDefaultAsync(x => x.Id == id);
            if (wishItem == null)
                return NotFound(new ErrorResponse { Error = "Id not found" });

            if (user.Id != wishItem.UserFK)
                return Unauthorized();

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                wishItem.Name = data.Name;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<controller>/wishlist/{id}
        [HttpDelete("wishlist/{id}")]
        public async Task<IActionResult> DeleteWishItemAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            AddWishItemBody data = JsonConvert.DeserializeObject<AddWishItemBody>(body);
            if (data == null)
                return BadRequest(new ErrorResponse { Error = "Invalid json" });

            var wishItem = await _context.WishList.FirstOrDefaultAsync(x => x.Id == id);
            if (wishItem == null)
                return NotFound(new ErrorResponse { Error = "Id not found" });

            if (user.Id != wishItem.UserFK)
                return Unauthorized();

            _context.WishList.Remove(wishItem);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
