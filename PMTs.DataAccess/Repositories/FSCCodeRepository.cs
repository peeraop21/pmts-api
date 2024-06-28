using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class FSCCodeRepository : Repository<PpcFscCode>, IFSCCodeRepository
    {
        public FSCCodeRepository(PMTsDbContext context)
            : base(context)
        {

        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PpcFscCode GetFSCCodeByFSCCode(string fSCCode)
        {
            return PMTsDbContext.PpcFscCode.FirstOrDefault(f => f.FscCode.Equals(fSCCode));
        }
    }
}
