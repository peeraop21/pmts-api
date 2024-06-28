using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardCombineRepository : IRepository<BoardCombine>
    {
        List<BoardViewModel> GetBoard(string factoryCode, string costField, string lv2, string lv3);
        List<SearchBoardAlt> GetBoardSearch();
        BoardCombine GetBoardByCode(string code);
        IEnumerable<BoardCombine> GetBoardByFlute(string flute);
        BoardCombine GetBoardByBoard(string board, string flute);
        List<BoardCombine> GetBoardsByBoard(string board, string flute);
        List<BoardSpecWeight> GetBoardSpecWeightByCode(string factoryCode, string code);
        IEnumerable<BoardCombine> GetBoardsByCodes(string factoryCode, List<string> codes);
        string GenerateCode();
        ExportDataForSAPResponse GenerateDataForSAP(ExportDataForSAPRequest request);
    }
}
