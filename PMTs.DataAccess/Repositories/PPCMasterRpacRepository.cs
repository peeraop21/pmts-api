using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class PPCMasterRpacRepository : Repository<PpcMasterRpac>, IPPCMasterRpacRepository
    {

        public PPCMasterRpacRepository(PMTsDbContext context)
            : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
