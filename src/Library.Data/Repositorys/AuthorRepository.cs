using Library.Business.Interfaces;
using Library.Business.Models;
using Library.Data.Context;

namespace Library.Data.Repositorys
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(DataContext context) : base(context) { }

    }
}
