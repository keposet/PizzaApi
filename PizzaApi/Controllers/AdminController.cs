using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Services;
using PizzaApi.Utilities;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]/pizza")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly PizzaItemHandler _pizzaItemHandler;

        public AdminController(PizzaItemHandler pizzaItemHandler )
        {
            _pizzaItemHandler = pizzaItemHandler;
        }

        // POST: api/Admin/pizza
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostPizzaItem(PizzaItemAdminDTO pizzaItemAdminDto)
        {
            if (pizzaItemAdminDto == null) return BadRequest("null object");
            var isUpdate = pizzaItemAdminDto.IsUpdate ?? false;
            var isDelete = pizzaItemAdminDto.IsDelete ?? false;
            if(isUpdate)
            {
                var updateDTO = pizzaItemAdminDto.ToDTO();
                var status = await _pizzaItemHandler.UpdatePizzaItemById(updateDTO.Id, updateDTO);
                return ErrorMessageHandler(status);
            }
            if (isDelete)
            {   
                var status = await _pizzaItemHandler.DeletePizzaItemById(pizzaItemAdminDto.Id);
                return ErrorMessageHandler(status);
            }
            if(!isUpdate && !isDelete)
            {
                var submitDTO =  pizzaItemAdminDto.ToDTO();
                var savedPizza = await _pizzaItemHandler.AddPizza(submitDTO);
                return Ok(savedPizza);
            }

            return Ok();
        }

        // PUT: api/Admin/pizza/5
        //NOT to spec, but not more correct. 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPizzaItem(long id, PizzaItemDTO pizzaItemDTO)
        {
            var status = await _pizzaItemHandler.UpdatePizzaItemById(id, pizzaItemDTO);
            return ErrorMessageHandler(status);
        }

        // DELETE: api/Admin/pizza/5
        // NOT to spec, but not more correct.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaItem(long id)
        {
            var status = await _pizzaItemHandler.DeletePizzaItemById(id);
            return ErrorMessageHandler(status);
        }

        private IActionResult ErrorMessageHandler(ErrorMessage status)
        {
            if (status.IsError) return BadRequest(status.Message);
            return Ok();
        }
    }
}
