using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Services;

namespace PizzaApi.Controllers
{
    //GET PUT & DELETE removed according to spec.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderContext _context;
        private readonly OrderHandler _orderHandler;

        public OrderController(OrderContext context, OrderHandler orderHandler)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _orderHandler = orderHandler;
        }



        // POST: api/Order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderItemDTO>> PostOrderItem(OrderItemDTO orderItemDTO)
        {

            // create id, create orderNum, calc total (items, plus tip)
            var validate = await _orderHandler.ValidateOrder(orderItemDTO);
            if (validate.IsError) return BadRequest(validate.Message);
            

            //Order total Calc. 

            

            var id = _context.OrderItems.ToArray().Length + 1;
            var orderNum = _context.OrderItems.ToArray().Length + 1;
            var orderItem = new OrderItem
            {
                Id = id,
                CustomerName = orderItemDTO.CustomerName,
                OrderNumber = orderNum,
                OrderTimeStamp = DateTime.Now.ToUniversalTime(),
                PizzaItems = orderItemDTO.PizzaItems,
                OrderTotal = orderItemDTO.OrderTotal,
                IsDelivery = orderItemDTO.IsDelivery,
                DeliveryAddress = orderItemDTO.DeliveryAddress,
                IsComplete = orderItemDTO.IsComplete,

            };
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderItem", new { id = orderItem.Id }, orderItem);
        }

        private bool OrderItemExists(long id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }
    }
}
