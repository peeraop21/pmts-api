using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class AllowanceProcessRepository : Repository<AllowanceProcess>, IAllowanceProcessRepository
    {
        public AllowanceProcessRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
