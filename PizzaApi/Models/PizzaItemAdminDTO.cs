using System.Text.Json.Serialization;

namespace PizzaApi.Models
{
    public class PizzaItemAdminDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsUpdate { get; set; }

        public PizzaItemDTO ToDTO ()
        {
            return new PizzaItemDTO { Id = Id, Name = Name, Price = Price, Description = Description };
        }
    }
}
