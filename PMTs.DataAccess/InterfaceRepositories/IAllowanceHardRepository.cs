using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAllowanceHardRepository : IRepository<AllowanceHard>
    {

        AllowanceHard GetAllowanceHardByHardship(string factoryCode, string hardship);

    }
}
