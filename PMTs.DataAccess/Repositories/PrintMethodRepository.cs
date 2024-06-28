using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PrintMethodRepository : Repository<PrintMethod>, IPrintMethodRepository
    {
        public PrintMethodRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<PrintMethod> GetPrintMethodAll()
        {
            return PMTsDbContext.PrintMethod.ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
