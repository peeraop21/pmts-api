using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class HierarchyLV4Repository : Repository<HierarchyLv4>, IHierarchyLV4Repository
    {
        public HierarchyLV4Repository(PMTsDbContext context)
            : base(context)
        {
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<HierarchyLv4> GetHierarchyLV4All()
        {
            return PMTsDbContext.HierarchyLv4.ToList();
        }
    }
}
