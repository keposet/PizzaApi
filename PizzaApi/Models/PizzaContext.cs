using Microsoft.EntityFrameworkCore;

namespace PizzaApi.Models
{
    public class PizzaContext : DbContext
    {
        public PizzaContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<PizzaItem> PizzaItems { get; set; } = null;
    }
}
