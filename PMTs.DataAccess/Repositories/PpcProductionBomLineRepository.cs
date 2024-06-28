using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class PpcProductionBomLineRepository : Repository<PpcProductionBomLine>, IPpcProductionBomLineRepository
    {
        public PpcProductionBomLineRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        //public List<PpcProductionBomLine> GetProductionBomLineByProductionBomNo(string productionBomNo)
        //{
        //    return PMTsDbContext.PpcProductionBomLine.Where(p => p.ProductionBomNo == productionBomNo).ToList();
        //}
    }
}
