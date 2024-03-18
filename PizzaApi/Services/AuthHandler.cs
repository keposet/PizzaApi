using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaApi.Services
{
    public class AuthHandler
    {
        private readonly UserContext _userCtx;
        private IConfiguration _config;

        public AuthHandler(UserContext userCtx, IConfiguration config)
        {
            _userCtx = userCtx ?? throw new ArgumentNullException(nameof(userCtx));
            _config = config;
        }

        public async Task<ErrorMessage> Authenticate(LoginDTO loginDTO)
        {
            var status = new ErrorMessage { IsError = true };
            //retrieve user & validate
            var userItem = await FindUserByName(loginDTO.Name);
            if (userItem == null) 
            {
                status.Message = "User Not Found";
                return status;
            }
            if (userItem.Credential == null)
            {
                status.Message = "Authentication Error";
                return status;
            }
            if (userItem.Name == null)
            {
                status.Message = "Authentication Error";
                return status;
            }
            if(userItem.Role == null)
            {
                status.Message = "Authentication Error";
                return status;
            }
            
            //validate password
            var hashResult = VerifyPassword(userItem, loginDTO.Password);
            if (hashResult == PasswordVerificationResult.Failed) 
            {
                status.Message = "Authentication Error";
                return status;
            }
            status.IsError = false;
            return status;
        }

        public async Task<string> Authorize(LoginDTO loginDTO)
        {

            var userItem = await FindUserByName(loginDTO.Name);
            if (userItem == null) throw new NullReferenceException(nameof(userItem));
            if(userItem.Name == null) throw new NullReferenceException(nameof(userItem.Name));
            if (userItem.Role == null) throw new NullReferenceException(nameof(userItem.Role));

            //create claims
            var userClaims = new Dictionary<string, string>
            {
                { "Name", userItem.Name },
                { "Role", userItem.Role }
            };
            var tokenClaims = CompileClaims(userClaims);

            //create token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: tokenClaims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials
              );

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return token;
        }

        public string? HashPassword(UserItem userItem)
        {
            if (userItem.Credential == null) throw new NullReferenceException(nameof(userItem.Credential));
            var passwordHasher = new PasswordHasher<UserItem>();
            var hashedCred = passwordHasher.HashPassword(userItem, userItem.Credential);
            return hashedCred;
        }

        private List<Claim> CompileClaims(Dictionary<string,string> userClaims)
        {
            var claims = new List<Claim>();
            foreach (var claim in userClaims) 
            { 
                claims.Add(new Claim(claim.Key, claim.Value));
            }
            return claims;
        }

        private PasswordVerificationResult VerifyPassword(UserItem userItem, string password)
        {
            var hasher = new PasswordHasher<UserItem>();
            var hashResult = hasher.VerifyHashedPassword(userItem, userItem.Credential, password);
            return hashResult;
        }

        private async Task<UserItem?> FindUserByName(string name)
        {
            if (_userCtx.UserItems == null) throw new NullReferenceException(nameof(_userCtx.UserItems));
            var userItem = await _userCtx.UserItems.FirstOrDefaultAsync(usr => usr.Name == name);

            return userItem;
        }
    }
}
