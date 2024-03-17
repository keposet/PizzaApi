using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;

namespace PizzaApi.Services
{
    public class OrderHandler
    {
        private readonly PizzaContext _pizzaCtx;
        private readonly OrderContext _orderCtx;
        private readonly UserContext _userCtx;

        public OrderHandler(PizzaContext pizzaCtx, OrderContext orderCtx, UserContext userCtx)
        {
            _pizzaCtx = pizzaCtx ?? throw new ArgumentNullException(nameof(pizzaCtx));
            _orderCtx = orderCtx ?? throw new ArgumentNullException(nameof(orderCtx));
            _userCtx = userCtx ?? throw new ArgumentNullException(nameof(userCtx));
        }

        public async Task<ErrorMessage> ValidateOrder(OrderItemDTO orderItemDTO)
        {
            if (orderItemDTO.IsDelivery == null)
            {
                return new ErrorMessage
                {
                    IsError = true,
                    Message = "Must Select Delivery or Pickup"
                };
            }
            if (orderItemDTO.IsDelivery == true && orderItemDTO.DeliveryAddress.IsNullOrEmpty())
            {
                return new ErrorMessage
                {
                    IsError = true,
                    Message = "Must input a delivery address if Order Type is Delivery"
                };
            }
            return new ErrorMessage { IsError = false};
        }
    }
}
