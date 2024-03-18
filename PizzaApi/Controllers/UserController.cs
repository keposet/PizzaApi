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
using PizzaApi.Services;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly UserHandler _userHandler;
        private readonly AuthHandler _authHandler;
        public UserController(UserHandler userHandler, AuthHandler authHandler)
        {
            _authHandler = authHandler;
            _userHandler = userHandler;
        }

        //GET PUT DELETE removed according to spec. 

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserItemDTO>> PostUserItem(UserItem userItem)
        {
            //verify inputs. 
            var inputStatus = _userHandler.ValidateUserData(userItem);
            if(inputStatus.IsError)
            {
                return BadRequest(inputStatus.Message);
            }
            
            // salt & hash password for storage 
            var hashedCred = _authHandler.HashPassword(userItem);
            userItem.Credential = hashedCred;

            //save user
            var userItemDTO = await _userHandler.WriteUserItem(userItem);
            
            return Ok(userItemDTO);
        }

        
    }
}
