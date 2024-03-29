﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly OrderHandler _orderHandler;

        public OrderController(OrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderItemDTO>> PostOrderItem(OrderItemDTO orderItemDTO)
        {
            // validate biz. logic
            var validate = _orderHandler.ValidateOrder(orderItemDTO);
            if (validate.IsError) return BadRequest(validate.Message);

            //submit order
            var submittedOrder = await _orderHandler.SubmitOrder(orderItemDTO);

            //return data
            return  Ok(submittedOrder);
        }

    }
}
