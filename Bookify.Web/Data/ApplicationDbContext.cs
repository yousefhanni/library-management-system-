using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookCategory> BookCategories { get; set; }

        public DbSet<Category> Categories { get; set; }


        //Write Fluent API To put Configs on Domain model and apply their configurations at DB Or create specific Configs classes and put line of apply configs
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //to represent Composite P.K 
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            base.OnModelCreating(builder);
        }

    }
}
