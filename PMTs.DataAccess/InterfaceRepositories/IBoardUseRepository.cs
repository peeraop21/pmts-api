using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardUseRepository : IRepository<BoardUse>
    {
        BoardUse GetBoardUseByMaterialNo(string factoryCode, string materialNo);

        IEnumerable<string> GetPaperItemByMaterialNo(string factoryCode, string materialNo);

        IEnumerable<BoardUse> GetBoardUsesByMaterialNos(string factoryCode, List<string> materialNos);
    }
}
