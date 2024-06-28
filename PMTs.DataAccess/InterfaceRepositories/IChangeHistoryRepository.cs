using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IChangeHistoryRepository : IRepository<ChangeHistory>
    {
        IEnumerable<ChangeHistory> GetChangeHistoryByMaterialNo(string factoryCode, string materialNo);
    }
}
