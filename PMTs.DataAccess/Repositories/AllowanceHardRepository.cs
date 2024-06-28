using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AllowanceHardRepository : Repository<AllowanceHard>, IAllowanceHardRepository
    {
        public AllowanceHardRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public AllowanceHard GetAllowanceHardByHardship(string factoryCode, string hardship)
        {
            var hs = 0;
            int.TryParse(hardship, out hs);
            return PMTsDbContext.AllowanceHard.FirstOrDefault(w => w.FactoryCode == factoryCode && w.Hardship == hs); ;
        }
    }
}
