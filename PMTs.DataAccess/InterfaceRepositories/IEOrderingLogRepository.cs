using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IEOrderingLogRepository : IRepository<EorderingLog>
    {
        EorderingLog GetLastEOrderingLog();
    }
}
