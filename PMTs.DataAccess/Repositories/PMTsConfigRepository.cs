using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PMTsConfigRepository : Repository<PmtsConfig>, IPMTsConfigRepository
    {
        public PMTsConfigRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PmtsConfig GetSlit(string FactoryCode)
        {
            return PMTsDbContext.PmtsConfig.Where(config => config.FactoryCode == FactoryCode && config.FucName == "Slit").FirstOrDefault();
        }

        public PmtsConfig GetPMTsConfigByFactoryName(string FactoryCode, string funcName)
        {
            return PMTsDbContext.PmtsConfig.FirstOrDefault(w => w.FucName.Equals(funcName) && w.FactoryCode.Equals(FactoryCode));
        }

        public PmtsConfig GetPMTsConfigByFucName(string FactoryCode, string funcName)
        {
            return PMTsDbContext.PmtsConfig.FirstOrDefault(w => w.FucName.Equals(funcName) && w.FactoryCode.Equals(FactoryCode));
        }
    }
}
