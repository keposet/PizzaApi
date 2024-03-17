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
        public async Task<ActionResult<IEnumerable<PizzaItemDTO>>> GetPizzaItems()
        {
            return await _pizzaItemHandler.GetAllPizzaItems();
        }

        // GET: api/PizzaItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaItemDTO>> GetPizzaItem(long id)
        {
            var pizzaItem = _pizzaItemHandler.GetPizzaItemById(id);
            if(pizzaItem == null) return BadRequest("Item could not be found");
            return Ok(pizzaItem);
        }     
    }
}
