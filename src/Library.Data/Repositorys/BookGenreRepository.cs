using Library.Business.Interfaces;
using Library.Business.Models;
using Library.Data.Context;

namespace Library.Data.Repositorys
{
    public class BookGenreRepository : Repository<BookGenre>, IBookGenreRepository
    {
        public BookGenreRepository(DataContext context) : base(context) { }
    }
}
