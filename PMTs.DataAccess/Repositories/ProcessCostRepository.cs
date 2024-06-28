using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class ProcessCostRepository : Repository<ProcessCost>, IProcessCostRepository
    {
        public ProcessCostRepository(PMTsDbContext context) : base(context)
        {

        }
    }
}
