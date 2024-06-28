using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class HierarchyLV3Repository : Repository<HierarchyLv3>, IHierarchyLV3Repository
    {
        public HierarchyLV3Repository(PMTsDbContext context)
            : base(context)
        {
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<HierarchyLv3> GetHierarchyLV3All()
        {
            return PMTsDbContext.HierarchyLv3.ToList();
        }
    }
}
