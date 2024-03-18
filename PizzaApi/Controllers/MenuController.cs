using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Services;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly PizzaContext _pizzaCtx;
        private readonly PizzaItemHandler _pizzaItemHandler;

        public MenuController(PizzaItemHandler pizzaItemHandler)
        {
            _pizzaItemHandler= pizzaItemHandler ?? throw new ArgumentNullException(nameof(pizzaItemHandler));
        }

        // GET: api/PizzaItems
        [HttpGet]
        public async Task<IActionResult> GetPizzaItems()
        {
            var menu=  await _pizzaItemHandler.GetAllMenuItems();

            return Ok(menu);
        }

        

        // GET: api/PizzaItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPizzaItem(long id)
        {
            var pizzaItem = await _pizzaItemHandler.GetPizzaItemById(id);
            if(pizzaItem == null) return BadRequest("Item could not be found");
            return Ok(pizzaItem);
        }     
    }
}
