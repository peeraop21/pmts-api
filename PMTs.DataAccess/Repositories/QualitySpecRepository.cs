using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class QualitySpecRepository : Repository<QualitySpec>, IQualitySpecRepository
    {
        public QualitySpecRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<QualitySpec> GetQualitySpecsByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var qualitySpecs = new List<QualitySpec>();
            qualitySpecs.AddRange(PMTsDbContext.QualitySpec.Where(m => m.FactoryCode == factoryCode && materialNos.Contains(m.MaterialNo)).AsNoTracking().ToList());

            return qualitySpecs;
        }
    }
}
