using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MoTempRepository : Repository<MoTemp>, IMoTempRepository
    {
        public MoTempRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<MoTemp> GetMoTempAll()
        {
            return PMTsDbContext.MoTemp.ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
