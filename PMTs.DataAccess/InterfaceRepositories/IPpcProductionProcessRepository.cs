using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPpcProductionProcessRepository : IRepository<PpcProductionProcess>
    {
        PpcProductionProcess GetProductionProcess(string plant, string planCode, int qty, int workType);
    }
}
