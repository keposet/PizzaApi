using PizzaApi.Models;

namespace PizzaApi.Utilities
{
    public class OrderStatusDTO
    {
        public OrderItemDTO? OrderItemDto { get; set; }
        public string? Message { get; set; }
        public bool? IsError { get; set; }
    }
}
