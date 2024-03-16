using Microsoft.AspNetCore.Http.HttpResults;
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
            var hashResult = hasher.VerifyHashedPassword(userItem, userItem.Credential, loginRequest.Password);
            if(hashResult == PasswordVerificationResult.Failed) return BadRequest();
            //add claims
            var userClaims = new List<Claim>();
            if (userItem.Name == null) return BadRequest();
            if (userItem.Role == null) return BadRequest();
            userClaims.Add(new Claim("Name", userItem.Name));
            userClaims.Add(new Claim("Role", userItem.Role));
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
            //include in header if desired
            Response.Headers.Append("Authorization", $"Bearer {token}");
            //include in cookie if desired
            Response.Cookies.Append("AccessToken",$"{token}", new CookieOptions() { HttpOnly= true, SameSite = SameSiteMode.Strict });
            
            return Ok(token);
        }        
    }
}
