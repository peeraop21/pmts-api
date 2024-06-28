using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class EOrderingLogRepository : Repository<EorderingLog>, IEOrderingLogRepository
    {
        private readonly PMTsDbContext _context;

        public EOrderingLogRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public EorderingLog GetLastEOrderingLog()
        {
            return _context.EorderingLog.OrderBy(e => e.Id).LastOrDefault();
        }
    }
}