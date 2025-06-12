using E_CommerceNet.DataEntity;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceNet.AdventureContext
{
    public class AdventureDbContext : DbContext
    {
        public AdventureDbContext(DbContextOptions<AdventureDbContext> options) : base(options)
        {
            
        }

        public DbSet<RegistrationDetails> RegistrationDetails { get; set; }
    }
}
