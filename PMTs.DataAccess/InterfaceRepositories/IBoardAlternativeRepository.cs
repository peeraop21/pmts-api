using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardAlternativeRepository : IRepository<BoardAlternative>
    {
        IEnumerable<BoardAlternative> GetByMat(string factoryCode, string mat);
        IEnumerable<BoardAlternative> GetBoardAlternativesByMaterialNos(string factoryCode, List<string> materialNos);
    }
}
