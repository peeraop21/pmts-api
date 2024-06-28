using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class JointRepository : Repository<Joint>, IJointRepository
    {
        public JointRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<Joint> GetJointAll()
        {
            return PMTsDbContext.Joint.ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
