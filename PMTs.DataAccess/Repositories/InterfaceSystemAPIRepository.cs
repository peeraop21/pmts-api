using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class InterfaceSystemAPIRepository : Repository<InterfaceSystemApi>, IInterfaceSystemAPIRepository
    {
        public InterfaceSystemAPIRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
