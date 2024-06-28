using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CoatingRepository : Repository<Coating>, ICoatingRepository
    {

        private readonly PMTsDbContext _context;
        public CoatingRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }



        public IEnumerable<Coating> GetCoatingByMaterialNumber(string factoryCode, string materialNo)
        {
            return _context.Coating.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode).ToList();

        }
    }
}
