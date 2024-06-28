using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CorConfigRepository : Repository<CorConfig>, ICorConfigRepository
    {
        public CorConfigRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public CorConfig GetPMTsConfigByFactoryName(string factoryCode)
        {
            return PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode).FirstOrDefault();
        }

        public CorConfig GetPMTsConfigByMachine(string factoryCode, string Machine)
        {
            CorConfig cc = PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == Machine).FirstOrDefault();
            if (cc == null)
            {
                cc = PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == "ELSE").FirstOrDefault();
            }
            return cc;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
