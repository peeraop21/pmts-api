using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IScoreTypeRepository : IRepository<ScoreType>
    {
        IEnumerable<ScoreType> GetScoreTypesByScoreTypeIds(List<string> scoreTypeIds, string factoryCode);
    }
}
