using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class HoneyPaperRepository : Repository<HoneyPaper>, IHoneyPaperRepository
    {
        public HoneyPaperRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        //public IEnumerable<HoneyPaper> GetHoneyPaperAll()
        //{
        //    return PMTsDbContext.HoneyPaper.ToList();
        //}

        public HoneyPaper GetPaperByGrade(string grade)
        {
            return PMTsDbContext.HoneyPaper.Where(h => h.Grade == grade).FirstOrDefault();
        }
    }
}
