using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardCombindMaintainRepository
    {
        BoardCombindMaintainModel GetAllMaxcode(IConfiguration config);
        List<BoardCombind> GetAllBoardcombind(IConfiguration config);
        List<FluteTR> GetAllFluteByFactoryCode(IConfiguration config, string factory);
        List<BoardSpect> GetAllBoardSpect(IConfiguration config);
        List<Option> GetDistinctFluteByFactoryCode(IConfiguration config, string factory);
        bool AddBoard(IConfiguration config, BoardCombindMaintainModel model);
        bool UpdateBoard(IConfiguration config, BoardCombindMaintainModel model);
        List<PaperGrades> GetAllPaperGrade(IConfiguration config, string factory);
        List<BoardSpect> GetAllBoardSpectByCode(IConfiguration config, string code);
    }
}
