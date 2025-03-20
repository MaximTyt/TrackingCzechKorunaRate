using Microsoft.EntityFrameworkCore;

namespace Entities.Entity.EF
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<RateEntity> Rates { get; set; }
    }
}
