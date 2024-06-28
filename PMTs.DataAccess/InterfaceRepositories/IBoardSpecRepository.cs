using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardSpecRepository : IRepository<BoardSpec>
    {
        IEnumerable<BoardSpec> GetBoardSpecAll();
        List<BoardSpec> GetBoardSpecByBoardId(string boardId);
        List<BoardSpecStation> GetBoardSpecStationByBoardId(string factoryCode, string boardId);
        IEnumerable<BoardSpec> GetBoardSpecsByCodes(List<string> codes);
        //List<BoardSpecWeight> GetBoardSpecWeightByBoardId(string factoryCode, string boardId);
    }
}
