using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaApi.Models;
using PizzaApi.Services;
using PizzaApi.Utilities;

namespace PizzaApi.Controllers
{
    [Authorize("AdminOnly")]
    [Route("api/[controller]/pizza")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly PizzaItemHandler _pizzaItemHandler;
        private readonly ErrorHandler _errorHandler;

        public AdminController(PizzaItemHandler pizzaItemHandler, ErrorHandler errorHandler )
        {
            _pizzaItemHandler = pizzaItemHandler;
            _errorHandler = errorHandler;
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
                if (pizzaItemAdminDto.Id == null || pizzaItemAdminDto.Id == 0) return BadRequest("Must supply valid id");
                var updateDTO = pizzaItemAdminDto.ToDTO();
                var status = await _pizzaItemHandler.UpdatePizzaItemById(updateDTO.Id, updateDTO);
                return _errorHandler.ErrorMessageHandler(status);
            }
            if (isDelete)
            {
                if (pizzaItemAdminDto.Id == null || pizzaItemAdminDto.Id == 0) return BadRequest("Must supply valid id");
                var status = await _pizzaItemHandler.DeletePizzaItemById(pizzaItemAdminDto.Id);
                return _errorHandler.ErrorMessageHandler(status);
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
        //NOT to spec
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPizzaItem(long id, PizzaItemDTO pizzaItemDTO)
        {
            var status = await _pizzaItemHandler.UpdatePizzaItemById(id, pizzaItemDTO);
            return _errorHandler.ErrorMessageHandler(status);
        }

        // DELETE: api/Admin/pizza/5
        // NOT to spec
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaItem(long id)
        {
            var status = await _pizzaItemHandler.DeletePizzaItemById(id);
            return _errorHandler.ErrorMessageHandler(status);
        }


    }
}
