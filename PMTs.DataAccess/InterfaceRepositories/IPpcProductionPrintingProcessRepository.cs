using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPpcProductionPrintingProcessRepository : IRepository<PpcProductionPrintingProcess>
    {
        PpcProductionPrintingProcess GetProductionPrintingProcess(string plant, string planCode, int qty);
    }
}
