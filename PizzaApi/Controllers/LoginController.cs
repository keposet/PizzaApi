using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Services;
using PizzaApi.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AuthHandler _authHandler;

        //TODO: remove
        private readonly ErrorHandler _errorHandler;

        public LoginController(AuthHandler authHandler, ErrorHandler errorHandler)
        {
            _authHandler = authHandler;
            _errorHandler = errorHandler;
        }

        //TODO: change login req to own model
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequestModel loginRequest)
        {
            var authenticateStatus = await _authHandler.Authenticate(loginRequest);
            if (authenticateStatus.IsError)
            {
                return BadRequest(authenticateStatus.Message);
            }
            var token = await _authHandler.Authorize(loginRequest); 

            //include in header if desired
            Response.Headers.Append("Authorization", $"Bearer {token}");

            //include in cookie if desired
            Response.Cookies.Append("AccessToken",$"{token}", new CookieOptions() { HttpOnly= true, SameSite = SameSiteMode.Strict });
            
            return Ok(token);
        }        
    }
}
