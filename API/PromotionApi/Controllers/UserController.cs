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
    public class UserController : Controller
    {
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context)
        {
            _context = context;
        }

        // GET api/<controller>
        /// <summary>
        /// Get your own user information
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <returns>User's own information</returns>
        /// <response code="200">Returns the user's own information</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OwnUserResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<OwnUserResponse>> GetOwnAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.Include(x => x.State).FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            return Ok(new OwnUserResponse { Id = user.Id, Nickname = user.Nickname, ImageUrl = user.ImageUrl, RegisterDate = user.RegisterDate, Type = user.Type, Credit = user.Credit, Email = user.Email, Name = user.Name, StateFK = user.StateFK });
        }

        // GET api/<controller>/search/{nickname}
        /// <summary>
        /// Search users that contain the text
        /// </summary>
        /// <remarks>
        /// If there's an exact match, it'll be the first result.
        /// It'll only return 10 users at maximum.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="nickname">Part or exact nickname to search</param>
        /// <returns>List of users that match the search</returns>
        /// <response code="200">Returns a list with users that match the search</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no match is found</response>
        [HttpGet("search/{nickname}")]
        [ProducesResponseType(200, Type = typeof(HashSet<UserSearchResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<UserSearchResponse>>> SearchAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] string nickname)
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
                return Ok(users.Select(x => new UserSearchResponse { Id = x.Id, Nickname = x.Nickname }));
        }

        // GET api/<controller>/{id}
        /// <summary>
        /// Return the minimal user information from the specific id
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">User id to get the information from</param>
        /// <returns>User information</returns>
        /// <response code="200">Returns the user information related with the id</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        /// <response code="404">If no user with that id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(HashSet<UserResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            if (!await _context.Users.AnyAsync(x => x.Token == validation.Token))
                return Unauthorized();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new ErrorResponse { Error = "User not found" });

            return Ok(new UserResponse { Id = user.Id, Nickname = user.Nickname, ImageUrl = user.ImageUrl, RegisterDate = user.RegisterDate, Type = user.Type });
        }

        // PATCH api/<controller>
        /// <summary>
        /// Edit your own user information
        /// </summary>
        /// <remarks>
        /// Just include in <paramref name="editUserData"/> what you are going to edit.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="editUserData">Information to edit from this user</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Returns nothing</response>
        /// <response code="400">If invalid authorization, invalid name, invalid cpf, invalid telephone, invalid cellphone, invalid image url, invalid nickname, or already used nickname</response>
        /// <response code="401">If token is invalid</response>
        [HttpPatch]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> EditAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromBody, Required] EditUserBody editUserData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (editUserData.Name != null)
            {
                if (!Utils.IsValidName(editUserData.Name))
                    return BadRequest(new ErrorResponse { Error = "Invalid name" });
                user.Name = editUserData.Name;
            }

            if (editUserData.Cpf != null)
            {
                if (!Utils.IsValidCpf(editUserData.Cpf))
                    return BadRequest(new ErrorResponse { Error = "Invalid cpf" });
                user.Cpf = editUserData.Cpf;
            }

            if (editUserData.Telephone != null)
            {
                if (!Utils.IsValidTelephone(editUserData.Telephone))
                    return BadRequest(new ErrorResponse { Error = "Invalid telephone" });
                user.Telephone = editUserData.Telephone;
            }

            if (editUserData.Cellphone != null)
            {
                if (!Utils.IsValidTelephone(editUserData.Cellphone))
                    return BadRequest(new ErrorResponse { Error = "Invalid cellphone" });
                user.Cellphone = editUserData.Cellphone;
            }

            if (editUserData.ImageUrl != null)
            {
                if (!Utils.IsValidImageUrl(editUserData.ImageUrl))
                    return BadRequest(new ErrorResponse { Error = "Invalid image url" });
                user.ImageUrl = editUserData.ImageUrl;
            }

            if (editUserData.Nickname != null)
            {
                if (!Utils.IsValidNickname(editUserData.Nickname))
                    return BadRequest(new ErrorResponse { Error = "Invalid nickname" });
                bool alreadyUsedNickname = await _context.Users.AnyAsync(x => EF.Functions.ILike(x.Nickname, editUserData.Nickname));
                if (alreadyUsedNickname)
                    return BadRequest(new ErrorResponse { Error = "Already used nickname" });
                user.Nickname = editUserData.Nickname;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/wishlist
        /// <summary>
        /// Get your own user wishlist
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <returns>User's own wishlist</returns>
        /// <response code="200">Returns user's own wishlist</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid</response>
        [HttpGet("wishlist")]
        [ProducesResponseType(200, Type = typeof(HashSet<WishlistItemResponse>))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<IEnumerable<WishlistItemResponse>>> GetOwnWishListAsync([FromHeader(Name = "Authorization"), Required] string authorization)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            ICollection<WishItem> wishList = await user.GetWishListAsync();

            return Ok(wishList.Select(x => new WishlistItemResponse { Id = x.Id, Name = x.Name, RegisterDate = x.RegisterDate }));
        }

        // POST api/<controller>/wishlist
        /// <summary>
        /// Add item to wishlist
        /// </summary>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="wishlistItemData">Item information to add to wishlist</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Returns nothing</response>
        /// <response code="400">If invalid authorization, invalid item name, or item already exists</response>
        /// <response code="401">If token is invalid</response>
        [HttpPost("wishlist")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> AddWishItemAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromBody, Required] WishlistItemBody wishlistItemData)
        {
            var validation = Token.ValidateAuthorization(authorization);
            if (!validation.IsValid)
                return BadRequest(validation.Result);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Token == validation.Token);
            if (user == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(wishlistItemData.Name))
                return BadRequest(new ErrorResponse { Error = "Invalid item name" });

            ICollection<WishItem> wishList = await user.GetWishListAsync();

            if (wishList.Any(x => x.Name.Equals(wishlistItemData.Name, StringComparison.InvariantCultureIgnoreCase)))
                return BadRequest(new ErrorResponse { Error = "Item already exists" });

            wishList.Add(new WishItem
            {
                Name = wishlistItemData.Name,
                RegisterDate = DateTimeOffset.UtcNow,
                UserFK = user.Id
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/<controller>/wishlist/{id}
        /// <summary>
        /// Get wishlist item
        /// </summary>
        /// <remarks>
        /// Requires the user to be the owner of this wishlist item.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Wishlist item id</param>
        /// <returns>Wishlist item</returns>
        /// <response code="200">Returns a wishlist item</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or item isn't from this user</response>
        /// <response code="404">If no item with this id is found</response>
        [HttpGet("wishlist/{id}")]
        [ProducesResponseType(200, Type = typeof(WishlistItemResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<WishlistItemResponse>> GetOwnWishListAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute] long id)
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

            return Ok(new WishlistItemResponse { Id = wishItem.Id, Name = wishItem.Name, RegisterDate = wishItem.RegisterDate });
        }

        // DELETE api/<controller>/wishlist/{id}
        /// <summary>
        /// Delete wishlist item
        /// </summary>
        /// <remarks>
        /// Requires the user to be the owner of this wishlist item.
        /// </remarks>
        /// <param name="authorization">Bearer Auth format</param>
        /// <param name="id">Wishlist item id</param>
        /// <param name="wishlistItemData">Item information to delete from wishlist</param>
        /// <returns>Nothing</returns>
        /// <response code="200">Returns nothing</response>
        /// <response code="400">If invalid authorization</response>
        /// <response code="401">If token is invalid, or item isn't from this user</response>
        /// <response code="404">If no item with this id is found</response>
        [HttpDelete("wishlist/{id}")]
        [ProducesResponseType(200, Type = typeof(WishlistItemResponse))]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        [ProducesResponseType(404, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> DeleteWishItemAsync([FromHeader(Name = "Authorization"), Required] string authorization, [FromRoute, Required] long id, [FromBody, Required] WishlistItemBody wishlistItemData)
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

            _context.WishList.Remove(wishItem);

            await _context.SaveChangesAsync();

            return Ok();
        }

        /*// PATCH api/<controller>/wishlist/{id}
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
        }*/
    }
}
