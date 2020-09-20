using Microsoft.EntityFrameworkCore;

namespace ChemicleanProject.Models
{
    public class dbContext: DbContext
    {
        public dbContext(DbContextOptions<dbContext> options): base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
