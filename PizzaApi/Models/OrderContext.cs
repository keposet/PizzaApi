using Microsoft.EntityFrameworkCore;

namespace PizzaApi.Models
{
    public class OrderContext : DbContext
    {
        protected OrderContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OrderItem>? OrderItems { get; set; } = null;
    }
}
