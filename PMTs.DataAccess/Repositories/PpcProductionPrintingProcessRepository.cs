using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PpcProductionPrintingProcessRepository : Repository<PpcProductionPrintingProcess>, IPpcProductionPrintingProcessRepository
    {
        public PpcProductionPrintingProcessRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PpcProductionPrintingProcess GetProductionPrintingProcess(string plant, string planCode, int qty)
        {
            return PMTsDbContext.PpcProductionPrintingProcess.Where(p => p.Plant == plant && p.PlanCode == planCode && p.QuantityStart <= qty && p.QuantityTo >= qty).FirstOrDefault();
        }
    }
}
