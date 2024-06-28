using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class ChangeHistoryRepository : Repository<ChangeHistory>, IChangeHistoryRepository
    {
        public ChangeHistoryRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<ChangeHistory> GetChangeHistoryByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.ChangeHistory.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode).ToList();
        }
    }
}
