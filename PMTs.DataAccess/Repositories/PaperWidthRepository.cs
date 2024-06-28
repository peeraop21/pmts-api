using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PaperWidthRepository : Repository<PaperWidth>, IPaperWidthRepository
    {
        public PaperWidthRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<PaperWidth> GetPaperWidth(string factoryCode)
        {
            return PMTsDbContext.PaperWidth.Where(o => o.FactoryCode == factoryCode).OrderBy(o => o.Width).ToList();
        }
    }
}
