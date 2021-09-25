using Library.Business.Interfaces;
using Library.Business.Models;
using Library.Data.Context;

namespace Library.Data.Repositorys
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(DataContext context) : base(context) { }
    }
}
