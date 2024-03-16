using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaApi.Data;
using PizzaApi.Models;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItemDTO>>> GetUserItems()
        {
            if (_context.UserItems == null) return BadRequest();
            return await _context.UserItems.Select(u => UserItemToDTO(u)).ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserItemDTO>> GetUserItem(long id)
        {
            
            
            var userItem = await _context.UserItems.FindAsync(id);

            if (userItem == null)
            {
                return NotFound();
            }

            return UserItemToDTO(userItem);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserItem(long id, UserItemDTO userItemDTO)
        {
            if (id != userItemDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(userItemDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //[Route("api/[controller]/signup")]
        //[HttpPost]
        //public async Task<ActionResult<UserItemDTO>> PostUserSignup([FromBody] UserItemDTO signupRequest)
        //{
        //    if (signupRequest == null) return BadRequest();
        //    var id = _context.UserItems.ToArray().Length + 1;
        //    userItem.Id = id;


        //    return Ok();
        //}

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserItemDTO>> PostUserItem(UserItem userItem)
        {
            var id = _context.UserItems.ToArray().Length +1;
            userItem.Id = id;
            // salt & hash. 
            var passwordHasher = new PasswordHasher<UserItem>();
            var hashedCred = passwordHasher.HashPassword(userItem, userItem.Credential);
            userItem.Credential = hashedCred;

            _context.UserItems.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserItem", new { id = userItem.Id }, userItem);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserItem(long id)
        {
            var userItem = await _context.UserItems.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }

            _context.UserItems.Remove(userItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserItemExists(long id)
        {
            return _context.UserItems.Any(e => e.Id == id);
        }

        private static UserItemDTO UserItemToDTO(UserItem userItem) => new UserItemDTO
        {
            Id = userItem.Id,
            Name =  userItem.Name,
            Role = userItem.Role,
        };
    }
}
