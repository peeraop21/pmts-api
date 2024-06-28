using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MaterialTypeRepository : Repository<MaterialType>, IMaterialTypeRepository
    {
        private readonly PMTsDbContext _context;
        public MaterialTypeRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public MaterialType GetMaterialTypeByMatCode(string matCode)
        {
            return _context.MaterialType.FirstOrDefault(m => m.MatCode == matCode);
        }
    }
}
