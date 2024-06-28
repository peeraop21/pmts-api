using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PresaleChangeRoutingRepository : Repository<PresaleChangeRouting>, IPresaleChangeRoutingRepository
    {
        public PresaleChangeRoutingRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<PresaleChangeRouting> GetPresaleChangeRoutingByPsmId(string psmId)
        {
            return PMTsDbContext.PresaleChangeRouting.Where(p => p.PsmId == psmId);
        }

        public void UpdatePresaleRoutings(List<PresaleChangeRouting> presaleChangeRoutings)
        {
            PMTsDbContext.PresaleChangeRouting.UpdateRange(presaleChangeRoutings);
            PMTsDbContext.SaveChanges();
        }
    }
}
