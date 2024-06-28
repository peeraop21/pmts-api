using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class ScoreTypeRepository : Repository<ScoreType>, IScoreTypeRepository
    {
        public ScoreTypeRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<ScoreType> GetScoreTypesByScoreTypeIds(List<string> scoreTypeIds, string factoryCode)
        {
            var scoreTypes = new List<ScoreType>();

            foreach (var scoreTypeId in scoreTypeIds)
            {
                scoreTypes.AddRange(PMTsDbContext.ScoreType.Where(m => m.ScoreTypeId == scoreTypeId && m.FactoryCode == factoryCode).AsNoTracking().ToList());
            }

            return scoreTypes;
        }
    }

}
