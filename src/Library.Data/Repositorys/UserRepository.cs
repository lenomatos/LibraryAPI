using Library.Business.Interfaces;
using Library.Business.Models;
using Library.Data.Context;

namespace Library.Data.Repositorys
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) { }
    }
}
