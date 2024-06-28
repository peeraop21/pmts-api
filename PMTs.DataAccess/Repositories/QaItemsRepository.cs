using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class QaItemsRepository : Repository<QaItems>, IQaItemsRepository
    {
        public QaItemsRepository(PMTsDbContext context) : base(context)
        {

        }
    }
}
