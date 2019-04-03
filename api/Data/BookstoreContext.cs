using Fisher.Bookstore.Models;
using Microsoft.EntityFrameworkCore; 

namespace Fisher.Bookstore.Api.Data 
{
    public class BookstoreContext : DbContext  // This is the glue between our cs code and the database
    {
        public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder) => 
        base.OnModelCreating(builder);
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors {get; set; }
    }
}