using Library.Business.Interfaces;
using Library.Business.Models;
using Library.Data.Context;

namespace Library.Data.Repositorys
{
    public class ReserveRepository : Repository<Reserve>, IReserveRepository
    {
        public ReserveRepository(DataContext context) : base(context) { }
    }
}
