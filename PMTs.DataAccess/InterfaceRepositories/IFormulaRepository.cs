using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IFormulaRepository // : IRepository<MasterData>
    {
        RoutingDataModel CalculateRouting(string machineName, string _factoryCode, string flut, int trans_cutsheetwid, string trans_material, IEnumerable<string> ppItem, int trim, int cut, CorConfig corConfig, PmtsConfig pmtsConfig, Flute flute, List<PaperWidth> RollWidth, List<PaperGrade> grade, MachineFluteTrim machineFluteTrim = null, BoardAlternative boardAlternative = null, BoardUse boardUse = null, bool isTuning = false);
        RoutingDataModel GetFormulaByCut(string FactoryCode, int Cut, double WigthIn, string Flute, string MaterialNo, string machineName);
        RoutingDataModel GetFormulaByPaperWidth(string FactoryCode, int PaperWigth, double WigthIn, string Flute, int cut);

        int CalculateMoTargetQuantity(string factoryCode, double Order_Quant, double Tolerance_Over, string flute, string materialNo, int cut);
        int CalculateTargetQty(string factoryCode, double Order_Quant, double Tolerance_Over, string flute, string orderItem);
        int CalculateMoTargetQtyPPC(string factoryCode, double? orderQuant, double Tolerance_Over, string materialNo);
        RSCResultModel GetRSC(string FactoryCode, RSCModel model);
        RSCResultModel GetRSC1Piece(string FactoryCode, RSCModel model);
        RSCResultModel GetRSC2Piece(string FactoryCode, RSCModel model);
        RSCResultModel GetDC(string FactoryCode, RSCModel model);
        RSCResultModel GetSF(string FactoryCode, RSCModel model);
        RSCResultModel GetHC(string FactoryCode, RSCModel model);
        RSCResultModel GetHB(string FactoryCode, RSCModel model);
        RSCResultModel GetCG(string FactoryCode, RSCModel model);
        RSCResultModel GetAC(string FactoryCode, RSCModel model);

        //List<ReCalculateTrimModel> ReCalculateTrim(IConfiguration configuration, string factoryCode, string flute, string _username, string action, ref DataTable dataTable);
        ChangeReCalculateTrimModel GetReCalculateTrim(IConfiguration configuration, string factoryCode, string flute, string machine, string boxType, string printMethod, string proType);
        List<ReturnCalPaperWidth> CalculateListRouting(List<ParamCalPaperWidth> model, string FactoryCode);
        void ReCalculateUpdateRoutings(IConfiguration configuration, List<Routing> routings);
        CalculateOffsetModel CalculateMoTargetQuantityOffset(string factoryCode, double? orderQuant, string materialNo, string orderItem, string userName);
        string CalSizeDimensions(string factoryCode, string materialNo);
    }
}
