using Microsoft.EntityFrameworkCore;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Utilities;

namespace PizzaApi.Services
{
    public class PizzaItemHandler
    {
        private readonly PizzaContext _pizzaCtx;

        public PizzaItemHandler(PizzaContext pizzaCtx)
        {
            _pizzaCtx = pizzaCtx ?? throw new ArgumentNullException(nameof(pizzaCtx));
        }

        public async Task<List<PizzaItemDTO>> GetAllPizzaItems()
        {
            if(_pizzaCtx.PizzaItems == null) throw new NullReferenceException(nameof(_pizzaCtx.PizzaItems));

            return await _pizzaCtx.PizzaItems
                .Select(x => PizzaItemToDTO(x))
                .ToListAsync();

        }

        public async Task<List<MenuItemDTO>> GetAllMenuItems()
        {
            if (_pizzaCtx.PizzaItems == null) throw new NullReferenceException(nameof(_pizzaCtx.PizzaItems));

            return await _pizzaCtx.PizzaItems
                .Select(x => PizzaItemToMenuDTO(x))
                .ToListAsync();
        }

        public async Task<PizzaItemDTO> GetPizzaItemById(long id)
        {
            if (_pizzaCtx.PizzaItems == null) throw new NullReferenceException(nameof(_pizzaCtx.PizzaItems));
            var pizzaItem = await _pizzaCtx.PizzaItems.FindAsync(id);

            if (pizzaItem == null) return null;
            return PizzaItemToDTO(pizzaItem);
        }

        public async Task<PizzaItem> AddPizza(PizzaItemDTO pizzaItemDTO)
        {
            if (_pizzaCtx.PizzaItems == null) throw new NullReferenceException(nameof(_pizzaCtx.PizzaItems));

            // this is NOT ideal.
            //I'd usually use SOL's Identity attribute on a column to generate the Key
            // I'm sure EF has that ability, but my unfamiarity with the tool was making research challenging.
            var id = (long)_pizzaCtx.PizzaItems.ToArray().Length + 1;


            var pizzaItem = new PizzaItem
            {
                Id = id,
                Name = pizzaItemDTO.Name,
                Price = pizzaItemDTO.Price,
                Description = pizzaItemDTO.Description,
            };

            _pizzaCtx.PizzaItems.Add(pizzaItem);
            await _pizzaCtx.SaveChangesAsync();
            return pizzaItem;
        }

        public async Task<ErrorMessage> UpdatePizzaItemById(long id, PizzaItemDTO pizzaItemDTO)
        {
            var status = new ErrorMessage
            {
                IsError = true
            };

            if (id != pizzaItemDTO.Id)
            {
                status.Message = "Id does not match supplied data";
                return status;
            }

            _pizzaCtx.Entry(pizzaItemDTO).State = EntityState.Modified;

            try
            {
                await _pizzaCtx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaItemExists(id))
                {
                    status.Message = "Item could not be found";
                    return status;
                }
                else
                {
                    throw;
                }
            }
            status.IsError = false;
            return status;
        }

        public async Task<ErrorMessage> DeletePizzaItemById(long id)
        {
            var status = new ErrorMessage { IsError = true };
            if (_pizzaCtx.PizzaItems == null) throw new NullReferenceException(nameof(_pizzaCtx.PizzaItems));

            var pizzaItem = await _pizzaCtx.PizzaItems.FindAsync(id);
            if (pizzaItem == null)
            {
                status.Message = "Id not found";
                return status;
            }

            _pizzaCtx.PizzaItems.Remove(pizzaItem);
            await _pizzaCtx.SaveChangesAsync();
            status.IsError=false;
            return status;
        }

        private MenuItemDTO PizzaItemToMenuDTO(PizzaItem pizzaItem)
        {
            return new MenuItemDTO
            {
                Id = pizzaItem.Id,
                Name = pizzaItem.Name,
                Price = pizzaItem.Price,
            };
        }

        private PizzaItemDTO PizzaItemToDTO(PizzaItem pizzaItem)
        {
            return new PizzaItemDTO
            {
                Id = pizzaItem.Id,
                Price = pizzaItem.Price,
                Name = pizzaItem.Name,
                Description = pizzaItem.Description
            };
        }

        private bool PizzaItemExists(long id)
        {
            return _pizzaCtx.PizzaItems.Any(e => e.Id == id);
        }


    }
}
