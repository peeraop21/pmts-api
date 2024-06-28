using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMoDataRepository : IRepository<MoData>
    {
        IEnumerable<MoData> GetMasterDataListBySO(string factoryCode, string stratSO, string endSO);

        IEnumerable<MoData> GetMoDataListBySBOExNo(string factoryCode, string stratSO, string endSO);

        IEnumerable<MoDatas> GetMODataListBySONonX(IConfiguration config, string factoryCode, string stratSO, string endSO);

        IEnumerable<MoDatas> GetMODataListBySearchTypeNonX(IConfiguration config, string factoryCode, string searchType, string searchText);

        MoData GetMoDataBySuffixSO(string factoryCode, string SO);

        IEnumerable<MoData> GetMoDataListBySuffixSO(string factoryCode, string SO);

        List<MoData> GetMoDataListByOrderItem(string factoryCode, string orderItem);

        IEnumerable<MoData> GetMasterDataListBySaleOrders(IConfiguration configuration, string factoryCode, List<string> saleOrders);

        void UpdateMoDataSentKIWI(string FactoryCode, string SaleOrder, string UserBy);

        IEnumerable<MoData> SaveMODAtaFromExcelFile(string factoryCode, string manualMOData);

        string AddManualMOData(MoData moData, MoSpec moSpec, List<MoRouting> moRoutings, List<RunningNo> runningNos, AttachFileMo attachFileMO);

        IEnumerable<MoData> GetMoDataManualListToSendKIWI(string factoryCode);

        void UpdateMoDataSentKIWI(string FactoryCode, string SaleOrder, bool SentKIWI, string UpdateBy);

        IEnumerable<MoData> GetMoDataListByDateTime(string factoryCode, string DateFrom, string DateTo);

        List<CheckRepeatOrder> CheckRepeatOrder(IConfiguration config, string factoryCode, string dateFrom, string dateTo, int repeatCount);

        List<CheckDiffDueDate> CheckDiffDueDate(IConfiguration config, string factoryCode, int datediff, DateTime dateFrom, DateTime dateTo);

        List<CheckDueDateToolong> CheckDueDateToolong(IConfiguration config, string factoryCode, int dayCount);

        List<CheckOrderQtyTooMuch> CheckOrderQtyTooMuch(IConfiguration config, string factoryCode, string dateFrom, string dateTo, int multiplier);

        IEnumerable<MoDatas> GetMoDatasByDueDateRange(IConfiguration config, string factoryCode, DateTime startDueDate, DateTime endDueDate);

        ReportCheck GetReportCheck(IConfiguration config, string username, string factoryCode, string startDueDate, string endDueDate);

        IEnumerable<MoData> GetMoDataListBySaleOrdersByDapper(IConfiguration config, string factoryCode, List<string> saleOrderList);

        IEnumerable<MoData> GetMoDatasByDueDateRangeAndStatus(IConfiguration config, string factoryCode, string status, DateTime dateFrom, DateTime dateTo);

        IEnumerable<MoData> GetMoDataByInterface_TIPs(IConfiguration configuration, bool Interface_TIPs);

        IEnumerable<MoData> GetMoDataByInterface_TIPs(IConfiguration configuration, string factoryCode, bool Interface_TIPs);

        void UpdateMoDataFlagInterfaceTips(string factoryCode, string orderItem, string interface_TIPs);

        //boo editblock platen
        EditBlockPlatenModel GetBlockPlatenMaster(string factorycode, string material, string pc);

        EditBlockPlatenModel GetBlockPlatenRouting(string factorycode, string material);

        void UpdateBlockPlatenRouting(string factoryCode, string username, List<EditBlockPlatenRouting> model);

        List<CheckRepeatOrder> GetDataReportMoManual(IConfiguration config, string factoryCode, string materialNo, string custName, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus);

        IEnumerable<MoData> GetMODataListBySONonXAndH(IConfiguration config, string factoryCode, string stratSO, string endSO);

        MoData GetMoDataBySOKiwi(string factoryCode, string sO_Kiwi);

        bool CheckMaterialNo(string MaterialNo);

        void UpdateMoDataPrintNo(string factoryCode, string orderItem, int printRoundNo, int allowancePrintNo, int afterPrintNo, int drawAmountNo, string userBy);

        PrintMasterCardMOModel GetDataForMasterCard(PrintMastercardMO printMastercardMO, string factoryCode, IMapper mapper, IStringLocalizer _localizer);

        IEnumerable<MasterDataRoutingModel> SearchMODataListBySONonXAndH(IConfiguration configuration, string factoryCode, string stratSO, string endSO);

        IEnumerable<MasterDataRoutingModel> GetMasterCardMOsBySaleOrders(IConfiguration configuration, string factoryCode, List<string> saleOrders);

        BasePrintMastercardData GetBaseOfMasterCardMOsBySaleOrders(IConfiguration config, IMapper mapper, string factoryCode, bool isUserTIPs, List<string> saleOrderList);

        MODataWithBomRawMatsModel GetMODataWithBomRawMatsByOrderItem(string orderItem);

        void UpdateMODatasInterfaceTIPsByOrderItems(string factorycode, bool interface_tips, List<string> orderItems);

        void UpdatePrintedMODataByOrderItems(string factoryCode, string username, List<string> orderItems);

        //PrintMasterCardMOModel GetDataForMasterCardOverSea(PrintMastercardMO printMastercardMO, string factoryCode, IMapper mapper, IStringLocalizer _localizer);
    }
}