using Microsoft.EntityFrameworkCore;
using PizzaApi.Models;

namespace PizzaApi.Data
{
    public class OrderContext : DbContext
    {
        protected OrderContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OrderItem>? OrderItems { get; set; } = null;
    }
}
