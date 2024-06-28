using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class HireMappingRepository : Repository<HireMapping>, IHireMappingRepository
    {
        public HireMappingRepository(PMTsDbContext context) : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
