using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class MoBoardUseRepository : Repository<MoBoardUse>, IMoBoardUseRepository
    {
        public MoBoardUseRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
