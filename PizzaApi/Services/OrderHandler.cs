
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Models;
using PizzaApi.Utilities;


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

        public ErrorMessage ValidateOrder(OrderItemDTO orderItemDTO)
        {
            var errorMessage = new ErrorMessage { IsError = true};

            if (orderItemDTO.IsDelivery == null)
            {
                errorMessage.Message = "Must Select Delivery or Pickup";
                return errorMessage;
            }
            if (orderItemDTO.IsDelivery == true && orderItemDTO.DeliveryAddress.IsNullOrEmpty())
            {
                errorMessage.Message = "Must input a delivery address if Order Type is Delivery";
                return errorMessage;
            }
            if( orderItemDTO.PizzaItems.IsNullOrEmpty()) 
            {
                errorMessage.Message = "Order submitted without any items";
                return errorMessage;
            }
            if(orderItemDTO.CustomerName.IsNullOrEmpty())
            {
                errorMessage.Message = "Order must include customer name";
                return errorMessage;
            }
            if(GetCustomerId(orderItemDTO.CustomerName) == null)
            {
                errorMessage.Message = "Name Not Found";
                return errorMessage;
            }
            return new ErrorMessage { IsError = false};
        }

        public async Task<OrderItemDTO> SubmitOrder(OrderItemDTO orderItemDTO)
        {
            if (_orderCtx.OrderItems == null) throw new ArgumentNullException(nameof(_orderCtx.OrderItems));

            var orderId = GetOrderId();
            var orderNumber = GetOrderId(); //reused in this demo because the logic is identical. 
            var orderTotal = CalculateOrderTotal(orderItemDTO);
            var customerId = GetCustomerId(orderItemDTO.CustomerName);
            var orderItem = new OrderItem
            {
                Id = orderId,
                CustomerId = customerId,
                CustomerName = orderItemDTO.CustomerName,
                OrderNumber = orderNumber,
                OrderTimeStamp = DateTime.Now.ToUniversalTime(),
                PizzaItems = orderItemDTO.PizzaItems,
                OrderTotal = orderTotal,
                IsDelivery = orderItemDTO.IsDelivery,
                DeliveryAddress = orderItemDTO.DeliveryAddress,
                IsComplete = orderItemDTO.IsComplete,

            };
            _orderCtx.OrderItems.Add(orderItem);
            await _orderCtx.SaveChangesAsync();
            var submittedOrder = OrderItemToDTO(orderItem);
            return submittedOrder;
        }

        //Realistically, this would be done on the front end. 
        //But it feels weird to skip the total calculation on a bill. 
        public double? CalculateOrderTotal(OrderItemDTO orderItemDTO)
        {
            var pizzaIds = orderItemDTO.PizzaItems ?? throw new ArgumentNullException(nameof(orderItemDTO.PizzaItems));
            if (_pizzaCtx.PizzaItems == null) throw new ArgumentNullException(nameof(_pizzaCtx.PizzaItems));

            double? total = 0.00;

            var pizzaItems = new List<PizzaItem>();
            foreach (var id in pizzaIds)
            {
                // TODO: Brittle selector.
                var price = _pizzaCtx.PizzaItems.Where(x => x.Id == id).First().Price;
                total += price;
            }
            
            double? tip = orderItemDTO.OrderTip ?? 0.00;
            total += tip;
            return total;
        }

        public long? GetCustomerId(string customerName)
        {
            var userItems = _userCtx.UserItems?? throw new NullReferenceException(nameof(_userCtx.UserItems));
            // TODO: Brittle selector.
            var user = userItems.Where(usr => usr.Name == customerName).FirstOrDefault();
            if (user == null) return null;

            return user.Id;
        }

        public long GetOrderId()
        {
            var orderItems = _orderCtx.OrderItems ?? throw new ArgumentNullException(nameof(_orderCtx.OrderItems));
            long id = 1;
            if(orderItems.ToArray().Length > 0)
            {
                id = orderItems.OrderByDescending(x => x.Id).First().Id +1;
            }
            return id;
        }

        public OrderItemDTO OrderItemToDTO(OrderItem orderItem)
        {
            return new OrderItemDTO
            {
                CustomerName = orderItem.CustomerName,
                OrderNumber = orderItem.OrderNumber,
                OrderTotal = orderItem.OrderTotal,
                OrderTip = orderItem.OrderTip,
                IsDelivery = orderItem.IsDelivery,
                DeliveryAddress = orderItem.DeliveryAddress,
                IsComplete = orderItem.IsComplete,
                OrderTimeStamp = orderItem.OrderTimeStamp,
                PizzaItems = orderItem.PizzaItems,
            };
        }

    }
}
