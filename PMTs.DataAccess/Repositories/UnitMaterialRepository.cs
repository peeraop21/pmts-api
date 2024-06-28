using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class UnitMaterialRepository : Repository<UnitMaterial>, IUnitMaterialRepository
    {
        public UnitMaterialRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public UnitMaterial GetUnitMaterialByName(string MaterialName)
        {
            return PMTsDbContext.UnitMaterial.SingleOrDefault(w => w.Name.Equals(MaterialName));
        }
    }
}
