using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaApi.Data;
using PizzaApi.Models;

namespace PizzaApi.Controllers
{
    [Route("api/[controller]")]
    [Route("api/Menu")]
    [ApiController]
    public class PizzaItemsController : ControllerBase
    {
        private readonly PizzaContext _context;

        public PizzaItemsController(PizzaContext context)
        {
            _context = context;
        }

        // GET: api/PizzaItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaItemDTO>>> GetPizzaItems()
        {
            return await _context.PizzaItems
                .Select(x => PizzaItemToDTO(x))
                .ToListAsync();
        }

        // GET: api/PizzaItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaItemDTO>> GetPizzaItem(long id)
        {
            var pizzaItem = await _context.PizzaItems.FindAsync(id);

            if (pizzaItem == null)
            {
                return NotFound();
            }

            return PizzaItemToDTO(pizzaItem);
        }

        // PUT: api/PizzaItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPizzaItem(long id, PizzaItemDTO pizzaItemDTO)
        {
            if (id != pizzaItemDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(pizzaItemDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaItemExists(id))
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

        // POST: api/PizzaItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PizzaItemDTO>> PostPizzaItem(PizzaItemDTO pizzaItemDTO)
        {

            // this is NOT ideal.
            //I'd usually use SOL's Identity attribute on a column to generate the Key, but that wansn't working.
            var id = (long) _context.PizzaItems.ToArray().Length +1;
            var pizzaItem = new PizzaItem
            {
                Id = id,
                Name = pizzaItemDTO.Name,
                Price = pizzaItemDTO.Price,
                Description = pizzaItemDTO.Description,
            };
            
            _context.PizzaItems.Add(pizzaItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPizzaItem), new { id = pizzaItem.Id }, PizzaItemToDTO(pizzaItem));
        }

        // DELETE: api/PizzaItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaItem(long id)
        {
            var pizzaItem = await _context.PizzaItems.FindAsync(id);
            if (pizzaItem == null)
            {
                return NotFound();
            }

            _context.PizzaItems.Remove(pizzaItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PizzaItemExists(long id)
        {
            return _context.PizzaItems.Any(e => e.Id == id);
        }

        private static PizzaItemDTO PizzaItemToDTO(PizzaItem pizzaItem) =>
            new PizzaItemDTO
            {
                Id = pizzaItem.Id,
                Price = pizzaItem.Price,
                Name = pizzaItem.Name,
                Description = pizzaItem.Description
            };
    }
}
