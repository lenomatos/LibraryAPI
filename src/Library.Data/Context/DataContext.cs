using Library.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Context
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
    }
}
