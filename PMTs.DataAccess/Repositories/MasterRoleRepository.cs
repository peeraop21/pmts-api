using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class MasterRoleRepository : Repository<MasterRole>, IMasterRoleRepository
    {
        public MasterRoleRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
