using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAdditiveRepository : IRepository<Additive>
    {
        bool AddAdditive(Additive model);
        bool UpdateAdditive(Additive model);
    }
}

