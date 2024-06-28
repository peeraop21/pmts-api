using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMoDataLogRepository : IRepository<MoDatalog>
    {
        IEnumerable<MoDatalog> GetMoDatalogListBySO(string factoryCode, string stratSO, string endSO);
    }
}
