namespace PizzaApi.Models
{
    public class OrderItemDTO
    {
        public string? CustomerName { get; set; }
        public long? OrderNumber { get; set; }
        public DateTime? OrderTimeStamp { get; set; }
        public List<long>? PizzaItems { get; set; }
        public double? OrderTotal { get; set; }
        public double? OrderTip { get; set; }
        public bool? IsDelivery { get; set; }
        public string? DeliveryAddress { get; set; }
        public bool? IsComplete { get; set; }

    }
}
