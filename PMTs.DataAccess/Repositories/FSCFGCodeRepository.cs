using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class FSCFGCodeRepository : Repository<PpcFscFgCode>, IFSCFGCodeRepository
    {
        public FSCFGCodeRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PpcFscFgCode GetFSCFGCodeByFSCFGCode(string fSCFGCode)
        {
            return PMTsDbContext.PpcFscFgCode.FirstOrDefault(f => f.FscFgCode.Equals(fSCFGCode));
        }
    }
}
