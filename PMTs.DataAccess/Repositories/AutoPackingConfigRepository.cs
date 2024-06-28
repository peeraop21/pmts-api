using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AutoPackingConfigRepository : Repository<AutoPackingConfig>, IAutoPackingConfigRepository
    {
        public AutoPackingConfigRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public AutoPackingConfig GetAutoPackingConfigByID(string factoryCode, string id)
        {
            return PMTsDbContext.AutoPackingConfig.FirstOrDefault(a => a.Id == Convert.ToInt32(id));
        }
    }
}
