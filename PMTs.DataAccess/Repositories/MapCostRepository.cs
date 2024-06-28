using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MapCostRepository : Repository<MapCost>, IMapCostRepository
    {
        public MapCostRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<MapCost> GetMapCostAll()
        {
            return PMTsDbContext.MapCost.ToList();
        }

        public MapCost GetCostField(string lv2, string lv3, string lv4)
        {
            var result = PMTsDbContext.MapCost.Where(m => m.Hierarchy2 == lv2 && m.Hierarchy3 == lv3 && m.Hierarchy4 == lv4).FirstOrDefault();

            if (result == null)
                result = PMTsDbContext.MapCost.Where(m => m.Hierarchy2 == lv2 && m.Hierarchy3 == lv3).FirstOrDefault();

            if (result == null)
                result = PMTsDbContext.MapCost.Where(m => m.Hierarchy2 == lv2).FirstOrDefault();

            return result;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
