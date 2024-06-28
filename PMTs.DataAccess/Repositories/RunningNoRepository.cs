using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class RunningNoRepository : Repository<RunningNo>, IRunningNoRepository
    {
        public RunningNoRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public RunningNo GetRunningNoByGroupId(string factoryCode, string groupId)
        {
            return PMTsDbContext.RunningNo.Where(w => w.GroupId == groupId && w.FactoryCode == factoryCode).FirstOrDefault();
        }
    }
}
