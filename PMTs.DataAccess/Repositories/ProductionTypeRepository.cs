using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class ProductionTypeRepository : Repository<ProductionType>, IProductionTypeRepository
    {
        public ProductionTypeRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<ProductionType> GetProductionTypeAll()
        {
            return PMTsDbContext.ProductionType.ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
