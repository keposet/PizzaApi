using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private IConfiguration _config;
        private readonly UserContext _context;

        public LoginController(IConfiguration config, UserContext context)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest loginRequest)
        {
            if (_context.UserItems == null) return BadRequest(); 
            //check request username exists. 
            var userItem = await _context.UserItems.FirstOrDefaultAsync(usr => usr.Name == loginRequest.Email);

            if (userItem == null) return BadRequest();

            //validate password 
            if (userItem.Credential == null) return BadRequest();
            var hasher = new PasswordHasher<UserItem>();
            hasher.VerifyHashedPassword(userItem, userItem.Credential, loginRequest.Password);
            //add claims
            var user = User as ClaimsPrincipal;
            var userClaims = new List<Claim>();
            if (userItem.Name == null) return BadRequest();
            if (userItem.Role == null) return BadRequest();

            userClaims.Add(new Claim("Name", userItem.Name));
            var roleClaim = new Claim("Role", userItem.Role);

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaims(userClaims);

            // token creation
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: userClaims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials
              );
            

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return Ok(token);
        }        
    }
}
