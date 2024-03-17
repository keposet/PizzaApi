using Microsoft.EntityFrameworkCore;
using PizzaApi.Models;

namespace PizzaApi.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<OrderItem>? OrderItems { get; set; } = null;
    }
}
