using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class LogPrintMoRepository : Repository<LogPrintMo>, ILogPrintMoRepository
    {
        private readonly PMTsDbContext context;
        public LogPrintMoRepository(PMTsDbContext context) : base(context)
        {
            this.context = context;
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
