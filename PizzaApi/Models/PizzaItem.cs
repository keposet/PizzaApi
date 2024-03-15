using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models
{
    public class PizzaItem
    {

        [Key]
        public long Id {  get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
    }
}
