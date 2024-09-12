using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasMany(a => a.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Author>().HasData(new Author { Id = 1, Name = "George Orwell" });
            modelBuilder.Entity<Book>().HasData(new Book { Id = 1, Title = "1984", Price = 19.99m, StockQuantity = 50, AuthorId = 1 });

            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 10, Name = "John Doe", IsVip = true },
                new Customer { Id = 20, Name = "Jane Doe", IsVip = false }
            );
        }
    }
}
