using System.ComponentModel.DataAnnotations;

namespace PizzaApi.Models
{
    public class UserItem
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Credential { get; set; }
        public string? Role { get; set; }
    }
}
