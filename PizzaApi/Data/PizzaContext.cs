using Microsoft.EntityFrameworkCore;
using PizzaApi.Models;

namespace PizzaApi.Data
{
    public class PizzaContext : DbContext
    {
        public PizzaContext(DbContextOptions<PizzaContext> options) : base(options)
        {
        }

        public DbSet<PizzaItem>? PizzaItems { get; set; } = null;
    }
}
