using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Write Fluent API To put Configs on Domain model and apply their configurations at DB Or create specific Configs classes and put line of apply configs
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Category>().Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            base.OnModelCreating(builder);
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Category> Categories { get; set; }



    }
}
