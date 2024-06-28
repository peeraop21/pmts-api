using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PpcProductionProcessRepository : Repository<PpcProductionProcess>, IPpcProductionProcessRepository
    {
        public PpcProductionProcessRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PpcProductionProcess GetProductionProcess(string plant, string planCode, int qty, int workType)
        {
            return PMTsDbContext.PpcProductionProcess.Where(p => p.Plant == plant && p.PlanCode == planCode && p.QuantityStart <= qty && p.QuantityTo >= qty && p.WorkType == workType).FirstOrDefault();
        }
    }
}
