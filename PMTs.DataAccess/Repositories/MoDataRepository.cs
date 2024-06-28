using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static StackExchange.Redis.Role;

namespace PMTs.DataAccess.Repositories
{
    public partial class MoDataRepository(PMTsDbContext context) : Repository<MoData>(context), IMoDataRepository
    {
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<MoData> GetMasterDataListBySaleOrders(IConfiguration configuration, string factoryCode, List<string> saleOrders)
        {
            //var moDatas = new List<MoData>();
            //foreach (var saleOrder in saleOrders)
            //{
            //    moDatas.AddRange(PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.OrderItem == saleOrder && m.MoStatus.ToLower().ToString() != "x").AsNoTracking().ToList());
            //}
            //return moDatas;
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && saleOrders.Contains(m.OrderItem) && m.MoStatus.ToLower().ToString() != "x").AsNoTracking().ToList();
        }

        public IEnumerable<MoData> GetMasterDataListBySO(string factoryCode, string stratSO, string endSO)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && (m.OrderItem.CompareTo(stratSO) >= 0 && m.OrderItem.CompareTo(endSO) <= 0)).AsNoTracking().ToList();
        }

        public IEnumerable<MoData> GetMoDataListBySBOExNo(string factoryCode, string stratSO, string endSO)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && (m.SboExternalNumber.CompareTo(stratSO) >= 0 && m.SboExternalNumber.CompareTo(endSO) <= 0)).AsNoTracking().ToList();
        }

        public IEnumerable<MoData> GetMoDataListBySaleOrdersByDapper(IConfiguration configuration, string factoryCode, List<string> saleOrders)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT  [Id] as Id
                           ,[FactoryCode] as FactoryCode
                           ,[MO_Status] as MoStatus
                           ,[OrderItem] as OrderItem
                           ,[Material_No] as MaterialNo
                           ,[Name] as Name
                           ,[Order_Quant] as OrderQuant
                           ,[Tolerance_Over] as ToleranceOver
                           ,[Tolerance_Under] as ToleranceUnder
                           ,[Due_Date] as DueDate
                           ,[Target_Quant] as TargetQuant
                           ,[Item_Note] as ItemNote
                           ,[District] as District
                           ,[PO_No] as PoNo
                           ,[DateTimeStamp] as DateTimeStamp
                           ,[Printed] as Printed
                           ,[Batch] as Batch
                           ,[Due_Text] as DueText
                           ,[Sold_to] as SoldTo
                           ,[Ship_to] as ShipTo
                           ,[PlanStatus] as PlanStatus
                           ,[StockQty] as StockQty
                           ,[IsCreateManual] as IsCreateManual
                           ,[SentKIWI] as SentKIWI
                           ,[OriginalDueDate] as OriginalDueDate
                           ,[CreatedDate] as CreatedDate
                           ,[CreatedBy] as CreatedBy
                           ,[UpdatedDate] as UpdatedDate
                           ,[UpdatedBy] as UpdatedBy
                           ,[MORNo] as MORNo
                           ,[SO_Kiwi] as SOKiwi
                           ,[Square_INCH] as SquareINCH
                           ,[Interface_TIPs] as InterfaceTIPs
                           ,[SBO_External_Number] as SBOExternalNumber
                    FROM MO_DATA
                    WHERE FactoryCode ='{0}' and OrderItem IN " + $"('{string.Join("','", saleOrders.ToArray())}') ";

            string message = string.Format(sql, factoryCode);

            return db.Query<MoData>(message).ToList();
        }

        public IEnumerable<MoDatas> GetMODataListBySONonX(IConfiguration configuration, string factoryCode, string stratSO, string endSO)
        {
            string condition = string.Empty;
            if (!string.IsNullOrEmpty(endSO))
                condition = !string.IsNullOrEmpty(stratSO) ? $" md.OrderItem between '{stratSO}' and '{endSO}'" : condition;
            else
                condition = !string.IsNullOrEmpty(stratSO) ? condition + $" md.OrderItem like '%{stratSO}%'" : condition;

            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            //string sql = @"
            //    SELECT  [Id] as Id
            //           ,[FactoryCode] as FactoryCode
            //           ,[MO_Status] as MoStatus
            //           ,[OrderItem] as OrderItem
            //           ,[Material_No] as MaterialNo
            //           ,[Name] as Name
            //           ,[Order_Quant] as OrderQuant
            //           ,[Tolerance_Over] as ToleranceOver
            //           ,[Tolerance_Under] as ToleranceUnder
            //           ,[Due_Date] as DueDate
            //           ,[Target_Quant] as TargetQuant
            //           ,[Item_Note] as ItemNote
            //           ,[District] as District
            //           ,[PO_No] as PoNo
            //           ,[DateTimeStamp] as DateTimeStamp
            //           ,[Printed] as Printed
            //           ,[Batch] as Batch
            //           ,[Due_Text] as DueText
            //           ,[Sold_to] as SoldTo
            //           ,[Ship_to] as ShipTo
            //           ,[PlanStatus] as PlanStatus
            //           ,[StockQty] as StockQty
            //           ,[IsCreateManual] as IsCreateManual
            //           ,[SentKIWI] as SentKIWI
            //           ,[UpdatedDate] as UpdatedDate
            //           ,[UpdatedBy] as UpdatedBy
            //           ,[MORNo] as MORNo
            //           ,[SO_Kiwi] as SOKiwi
            //           ,[Square_INCH] as SquareINCH
            //           ,[Interface_TIPs] as InterfaceTIPs
            //    FROM MO_DATA
            //    where OrderItem between '{0}' and '{1}' and MO_Status != 'X' and FactoryCode ='{2}'";

            string sql = @"
                    SELECT  md.[Id] as Id
                           ,md.[FactoryCode] as FactoryCode
                           ,md.[MO_Status] as MoStatus
                           ,md.[OrderItem] as OrderItem
                           ,md.[Material_No] as MaterialNo
                           ,md.[Name] as Name
                           ,md.[Order_Quant] as OrderQuant
                           ,md.[Tolerance_Over] as ToleranceOver
                           ,md.[Tolerance_Under] as ToleranceUnder
                           ,md.[Due_Date] as DueDate
                           ,md.[Target_Quant] as TargetQuant
                           ,md.[Item_Note] as ItemNote
                           ,md.[District] as District
                           ,md.[PO_No] as PoNo
                           ,md.[DateTimeStamp] as DateTimeStamp
                           ,md.[Printed] as Printed
                           ,md.[Batch] as Batch
                           ,md.[Due_Text] as DueText
                           ,md.[Sold_to] as SoldTo
                           ,md.[Ship_to] as ShipTo
                           ,md.[PlanStatus] as PlanStatus
                           ,md.[StockQty] as StockQty
                           ,md.[IsCreateManual] as IsCreateManual
                           ,md.[SentKIWI] as SentKIWI
                           ,md.[OriginalDueDate] as OriginalDueDate
                           ,md.[CreatedDate] as CreatedDate
                           ,md.[CreatedBy] as CreatedBy
                           ,md.[UpdatedDate] as UpdatedDate
                           ,md.[UpdatedBy] as UpdatedBy
                           ,md.[MORNo] as MORNo
                           ,md.[SO_Kiwi] as SOKiwi
                           ,md.[Square_INCH] as SquareINCH
                           ,md.[Interface_TIPs] as InterfaceTIPs
                           ,ms.PC as PC
                           ,ms.TagBundle as TagBundle
                           ,ms.TagPallet as TagPallet
                           ,md.SBO_External_Number as SBOExternalNumber
                    FROM MO_DATA md left outer join MO_Spec ms on ms.OrderItem = md.OrderItem and ms.FactoryCode = md.FactoryCode
                    where " + condition + " and md.MO_Status != 'X' and md.FactoryCode ='{0}'";

            string message = string.Format(sql, factoryCode);

            return db.Query<MoDatas>(message).ToList();
        }

        public IEnumerable<MoDatas> GetMODataListBySearchTypeNonX(IConfiguration configuration, string factoryCode, string searchType, string searchText)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            StringBuilder sql = new StringBuilder(@"
                    SELECT  md.[Id] as Id
                           ,md.[FactoryCode] as FactoryCode
                           ,md.[MO_Status] as MoStatus
                           ,md.[OrderItem] as OrderItem
                           ,md.[Material_No] as MaterialNo
                           ,md.[Name] as Name
                           ,md.[Order_Quant] as OrderQuant
                           ,md.[Tolerance_Over] as ToleranceOver
                           ,md.[Tolerance_Under] as ToleranceUnder
                           ,md.[Due_Date] as DueDate
                           ,md.[Target_Quant] as TargetQuant
                           ,md.[Item_Note] as ItemNote
                           ,md.[District] as District
                           ,md.[PO_No] as PoNo
                           ,md.[DateTimeStamp] as DateTimeStamp
                           ,md.[Printed] as Printed
                           ,md.[Batch] as Batch
                           ,md.[Due_Text] as DueText
                           ,md.[Sold_to] as SoldTo
                           ,md.[Ship_to] as ShipTo
                           ,md.[PlanStatus] as PlanStatus
                           ,md.[StockQty] as StockQty
                           ,md.[IsCreateManual] as IsCreateManual
                           ,md.[SentKIWI] as SentKIWI
                           ,md.[OriginalDueDate] as OriginalDueDate
                           ,md.[CreatedDate] as CreatedDate
                           ,md.[CreatedBy] as CreatedBy
                           ,md.[UpdatedDate] as UpdatedDate
                           ,md.[UpdatedBy] as UpdatedBy
                           ,md.[MORNo] as MORNo
                           ,md.[SO_Kiwi] as SOKiwi
                           ,md.[Square_INCH] as SquareINCH
                           ,md.[Interface_TIPs] as InterfaceTIPs
                           ,ms.PC as PC
                           ,ms.TagBundle as TagBundle
                           ,ms.TagPallet as TagPallet
                           ,md.SBO_External_Number as SBOExternalNumber
                    FROM MO_DATA md left outer join MO_Spec ms on ms.OrderItem = md.OrderItem and ms.FactoryCode = md.FactoryCode
                    where md.MO_Status != 'X' ");

            if (!string.IsNullOrEmpty(factoryCode))
            {
                sql.AppendFormat(" and md.FactoryCode ='{0}'", factoryCode);
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                if (("OrderItem").Equals(searchType))
                {
                    sql.AppendFormat(" and md.OrderItem like '%{0}%'", searchText);
                }
                else if (("Material_No").Equals(searchType))
                {
                    sql.AppendFormat(" and md.Material_No like '%{0}%'", searchText);
                }
                else if (("PC").Equals(searchType))
                {
                    sql.AppendFormat(" and ms.PC like '%{0}%'", searchText);
                }
                else if (("Cust_Name").Equals(searchType))
                {
                    sql.AppendFormat(" and ms.Cust_Name like '%{0}%'", searchText);
                }
                else if (("Sold_to").Equals(searchType))
                {
                    sql.AppendFormat(" and md.Sold_to like '%{0}%'", searchText);
                }
            }

            return db.Query<MoDatas>(sql.ToString()).ToList();
        }

        public IEnumerable<MoData> GetMODataListBySONonXAndH(IConfiguration configuration, string factoryCode, string stratSO, string endSO)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT  [Id] as Id
                           ,[FactoryCode] as FactoryCode
                           ,[MO_Status] as MoStatus
                           ,[OrderItem] as OrderItem
                           ,[Material_No] as MaterialNo
                           ,[Name] as Name
                           ,[Order_Quant] as OrderQuant
                           ,[Tolerance_Over] as ToleranceOver
                           ,[Tolerance_Under] as ToleranceUnder
                           ,[Due_Date] as DueDate
                           ,[Target_Quant] as TargetQuant
                           ,[Item_Note] as ItemNote
                           ,[District] as District
                           ,[PO_No] as PoNo
                           ,[DateTimeStamp] as DateTimeStamp
                           ,[Printed] as Printed
                           ,[Batch] as Batch
                           ,[Due_Text] as DueText
                           ,[Sold_to] as SoldTo
                           ,[Ship_to] as ShipTo
                           ,[PlanStatus] as PlanStatus
                           ,[StockQty] as StockQty
                           ,[IsCreateManual] as IsCreateManual
                           ,[SentKIWI] as SentKIWI
                           ,[OriginalDueDate] as OriginalDueDate
                           ,[CreatedDate] as CreatedDate
                           ,[CreatedBy] as CreatedBy
                           ,[UpdatedDate] as UpdatedDate
                           ,[UpdatedBy] as UpdatedBy
                           ,[MORNo] as MORNo
                           ,[SO_Kiwi] as SOKiwi
                           ,[Square_INCH] as SquareINCH
                           ,[Interface_TIPs] as InterfaceTIPs
                           ,[PrintRoundNo] as PrintRoundNo
                           ,[AllowancePrintNo] as AllowancePrintNo
                           ,[AfterPrintNo] as AfterPrintNo
                           ,[DrawAmountNo] as DrawAmountNo
                           ,[SBO_External_Number] as SBOExternalNumber
                    FROM MO_DATA
                    where OrderItem between '{0}' and '{1}' and MO_Status != 'X' and FactoryCode ='{2}'";

            string message = string.Format(sql, stratSO, endSO, factoryCode);

            return db.Query<MoData>(message).ToList();
        }

        public IEnumerable<MasterDataRoutingModel> SearchMODataListBySONonXAndH(IConfiguration configuration, string factoryCode, string stratSO, string endSO)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    select case when d.PDIS_Status = 'X' then 0 when d.PDIS_Status = null and s.Hierarchy like '%SB%' then 2 else 1 end MasterDataStatus,
                    case when isnull(s.TagBundle,'') <> '' then 1 else 0 end TagBunbleStatus, case when isnull(s.TagPallet,'') <> '' then 1 else 0 end TagPalletStatus,
                    m.MO_Status MoStatus, m.OrderItem SaleOrder, m.Material_No MaterialNo, s.PC, m.Order_Quant OrderQuant, m.Due_Text DueDate, m.Batch,
                    m.Printed, m.Name CustName, s.Description, s.Board, s.Box_Type BoxType,
                    Machine = STUFF((SELECT TOP 5 machine + ', '
                               FROM Mo_Routing r
                               WHERE r.OrderItem = m.OrderItem and r.FactoryCode = m.FactoryCode
                               ORDER BY r.Seq_No
                               FOR XML PATH('')), 1, 0, '')
                    from (select * from MO_DATA where OrderItem between '{0}' and '{1}' and FactoryCode = '{2}' and MO_Status <> 'X') m
                    left outer join MO_Spec s on s.orderItem = m.orderItem and s.FactoryCode = m.FactoryCode
                    left outer join MasterData d on d.Material_No = m.Material_No and d.FactoryCode = m.FactoryCode
                    order by SaleOrder";

            string message = string.Format(sql, stratSO, endSO, factoryCode);

            return db.Query<MasterDataRoutingModel>(message).ToList();
        }

        public IEnumerable<MasterDataRoutingModel> GetMasterCardMOsBySaleOrders(IConfiguration configuration, string factoryCode, List<string> saleOrders)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    select case when d.PDIS_Status = 'X' then 0 when d.PDIS_Status = null and s.Hierarchy like '%SB%' then 2 else 1 end MasterDataStatus,
                    case when isnull(s.TagBundle,'') <> '' then 1 else 0 end TagBunbleStatus, case when isnull(s.TagPallet,'') <> '' then 1 else 0 end TagPalletStatus,
                    m.MO_Status MoStatus, m.OrderItem SaleOrder, m.Material_No MaterialNo, s.PC, m.Order_Quant OrderQuant, m.Due_Text DueDate, m.Batch,
                    m.Printed, m.Name CustName, s.Description, s.Board, s.Box_Type BoxType,
                    Machine = STUFF((SELECT TOP 5 machine + ', '
                               FROM Mo_Routing r
                               WHERE r.OrderItem = m.OrderItem and r.FactoryCode = m.FactoryCode
                               ORDER BY r.Seq_No
                               FOR XML PATH('')), 1, 0, '')
                    from (select * from MO_DATA where OrderItem IN " + $"('{string.Join("','", [.. saleOrders])}') " + @" and FactoryCode = '{0}' and MO_Status <> 'X') m
                    left outer join MO_Spec s on s.orderItem = m.orderItem and s.FactoryCode = m.FactoryCode
                    left outer join MasterData d on d.Material_No = m.Material_No and d.FactoryCode = m.FactoryCode
                    order by SaleOrder"
            ;

            string message = string.Format(sql, factoryCode);

            return db.Query<MasterDataRoutingModel>(message).ToList();
        }

        public BasePrintMastercardData GetBaseOfMasterCardMOsBySaleOrders(IConfiguration config, IMapper mapper, string factoryCode, bool isUserTIPs, List<string> saleOrderList)
        {
            var result = new BasePrintMastercardData();
            var materialNosOfMODatas = new List<string>();
            var materialNosOfMasterDatas = new List<string>();
            var codesOfMasterDatas = new List<string>();
            var lv2sOfMasterDatas = new List<string>();
            var planCodes = new List<string>();
            var flutes = new List<string>();
            var codesOfBoardCombines = new List<string>();

            result.MoDatas = mapper.Map<List<MoDataPrintMastercard>>(PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && saleOrderList.Contains(m.OrderItem) && m.MoStatus.ToLower().ToString() != "x").AsNoTracking().ToList());

            if (result.MoDatas.Count > 0)
            {
                materialNosOfMODatas = result.MoDatas.Select(m => m.MaterialNo).ToList();
                if (materialNosOfMODatas.Count > 0)
                {
                    result.MasterDatas = PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && materialNosOfMODatas.Contains(m.MaterialNo)).AsNoTracking().ToList();
                    result.BoardAlternatives = PMTsDbContext.BoardAlternative.Where(b => b.FactoryCode == factoryCode && materialNosOfMODatas.Contains(b.MaterialNo)).AsNoTracking().ToList();
                }
                materialNosOfMasterDatas = result.MasterDatas.Select(m => m.MaterialNo).ToList();
                codesOfMasterDatas = result.MasterDatas.Select(m => m.Code).ToList();
                lv2sOfMasterDatas = result.MasterDatas.Select(m => m.Hierarchy.Substring(2, 2)).ToList();
                if (materialNosOfMasterDatas.Count > 0)
                {
                    result.QualitySpecs = PMTsDbContext.QualitySpec.Where(m => m.FactoryCode == factoryCode && materialNosOfMODatas.Contains(m.MaterialNo)).AsNoTracking().ToList();
                    result.BoardUses = PMTsDbContext.BoardUse.Where(b => materialNosOfMODatas.Contains(b.MaterialNo) && b.FactoryCode == factoryCode).AsNoTracking().ToList();
                }

                if (codesOfMasterDatas.Count > 0)
                {
                    result.BoardCombines = PMTsDbContext.BoardCombine.Where(m => codesOfMasterDatas.Contains(m.Code)).AsNoTracking().ToList();
                }

                if (lv2sOfMasterDatas.Count > 0)
                {
                    result.ProductTypes = PMTsDbContext.ProductType.Where(p => lv2sOfMasterDatas.Contains(p.HierarchyLv2)).AsNoTracking().ToList();
                }
            }

            result.MoSpecs = PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && saleOrderList.Contains(m.OrderItem)).AsNoTracking().ToList();
            result.AttachFileMOs = PMTsDbContext.AttachFileMo.Where(m => saleOrderList.Contains(m.OrderItem)).AsNoTracking().ToList();
            result.MoRoutings = mapper.Map<List<MoRoutingPrintMastercard>>(PMTsDbContext.MoRouting.Where(m => m.FactoryCode == factoryCode && saleOrderList.Contains(m.OrderItem)).AsNoTracking().ToList());
            result.PmtsConfigs = PMTsDbContext.PmtsConfig.Where(p => p.FactoryCode == factoryCode).ToList();

            if (result.MoRoutings.Count > 0)
            {
                if (!isUserTIPs)
                {
                    planCodes = result.MoRoutings.Select(m => m.PlanCode).ToList();
                }

                result.Machines = PMTsDbContext.Machine.Where(m => m.FactoryCode == factoryCode && planCodes.Contains(m.PlanCode)).AsNoTracking().ToList();
            }

            if (result.MoSpecs.Count > 0)
            {
                flutes = result.MoSpecs.Select(m => m.Flute).ToList();
                if (result.BoardCombines != null && result.BoardCombines.Count > 0)
                {
                    flutes.AddRange(result.BoardCombines.Select(b => b.Flute).ToList());
                }

                flutes = flutes.GroupBy(x => x).Select(g => g.First()).ToList();
                result.FluteTrs = PMTsDbContext.FluteTr.Where(f => flutes.Contains(f.FluteCode) && f.FactoryCode == factoryCode).AsNoTracking().ToList();
            }

            if (result.BoardCombines != null && result.BoardCombines.Count > 0)
            {
                codesOfBoardCombines = result.BoardCombines.Select(b => b.Code).ToList();
                result.BoardSpecs = PMTsDbContext.BoardSpec.Where(m => codesOfBoardCombines.Contains(m.Code)).AsNoTracking().ToList();
            }
            return result;
        }

        public MoData GetMoDataBySuffixSO(string factoryCode, string SO)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.OrderItem.Trim().EndsWith(SO)).AsNoTracking().FirstOrDefault();
        }

        public string AddManualMOData(MoData moData, MoSpec moSpec, List<MoRouting> moRoutings, List<RunningNo> runningNos, AttachFileMo attachFileMO)
        {
            var errorMessage = String.Empty;
            moData.CreatedDate = DateTime.Now;
            moData.CreatedBy = moData.UpdatedBy;
            moData.OriginalDueDate = moData.DueDate;
            PMTsDbContext.MoData.Add(moData);
            PMTsDbContext.MoSpec.Add(moSpec);
            PMTsDbContext.MoRouting.AddRange(moRoutings);
            if (attachFileMO.OrderItem != null)
            {
                PMTsDbContext.AttachFileMo.Add(attachFileMO);
            }
            else
            {
                attachFileMO = null;
            }
            int isExistattachFile = attachFileMO == null ? 0 : 1;
            if (runningNos != null && runningNos.Count > 0)
            {
                PMTsDbContext.RunningNo.UpdateRange(runningNos);
            }

            var transaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var update = PMTsDbContext.SaveChanges();

                if (update == (2 + moRoutings.Count + runningNos.Count + isExistattachFile))
                {
                    transaction.Commit();
                    return errorMessage;
                }
                else
                {
                    moData.OrderItem = string.Empty;
                    throw new Exception("Unable to Save: An error occurred while saving data.");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ex.Message;
            }
        }

        public IEnumerable<MoData> GetMoDataListBySuffixSO(string factoryCode, string SO)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.OrderItem.Trim().EndsWith(SO)).AsNoTracking().ToList();
        }

        public List<MoData> GetMoDataListByOrderItem(string factoryCode, string orderItem)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).ToList();
        }

        public IEnumerable<MoData> SaveMODAtaFromExcelFile(string factoryCode, string manualMOData)
        {
            var moDatas = new List<MoData>();
            return moDatas;
        }

        public void UpdateMoDataSentKIWI(string FactoryCode, string SaleOrder, string UserBy)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var MoData = PMTsDbContext.MoData.Where(z => z.FactoryCode == FactoryCode && z.OrderItem == SaleOrder).FirstOrDefault();
                    MoData.SentKiwi = false;
                    MoData.UpdatedBy = UserBy;
                    MoData.UpdatedDate = DateTime.Now;

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public IEnumerable<MoData> GetMoDataManualListToSendKIWI(string factoryCode)
        {
            return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.IsCreateManual == true && m.SentKiwi == false).AsNoTracking().ToList();
        }

        public void UpdateMoDataSentKIWI(string FactoryCode, string SaleOrder, bool SentKIWI, string UpdateBy)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var MoData = PMTsDbContext.MoData.Where(z => z.FactoryCode == FactoryCode && z.OrderItem == SaleOrder).FirstOrDefault();
                    MoData.SentKiwi = SentKIWI;
                    MoData.UpdatedBy = UpdateBy;
                    MoData.UpdatedDate = DateTime.Now;

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public IEnumerable<MoData> GetMoDataListByDateTime(string factoryCode, string DateFrom, string DateTo)
        {
            DateTime.TryParseExact(DateFrom, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out var from);
            DateTime.TryParseExact(DateTo, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out var to);

            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                return PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.UpdatedDate >= from && m.UpdatedDate <= to).AsNoTracking().ToList();
            }
            else
            {
                return new List<MoData>();
            }
        }

        public List<CheckRepeatOrder> CheckRepeatOrder(IConfiguration config, string factoryCode, string dateFrom, string dateTo, int repeatCount)
        {
            // 11.3
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                select mo.FactoryCode, md.OrderItem, mo.Due_Date as DueDate, mo.Material_no as MaterialNo, ma.PC, ma.Description, md.Name, mo.PO_No as PoNo, mo.Order_Quant as OrderQuant, md.Item_Note as ItemNote, mo.repeatOrder as repeatCount
                from
                    (select FactoryCode, Due_Date, Material_no, PO_No, Order_Quant, count(*) repeatOrder
                    from MO_DATA
                    where MO_Status <> 'X' and MO_Status <> 'S' and Material_No not like 'Z02%' and Due_Date between '{0}' and '{1}' and year(Due_Date) not like '25%' and FactoryCode = '{2}'
                    group by FactoryCode, Due_Date, Material_no, PO_No, Order_Quant
                    having count(*) >= '{3}') mo
                    left outer join
                    (select * from MO_DATA where MO_Status <> 'X' and MO_Status <> 'S' and Due_Date between '{4}' and '{5}' and year(Due_Date) not like '25%' ) md
                    on md.Material_No = mo.Material_No and md.FactoryCode = mo.FactoryCode and md.Due_Date = mo.Due_Date and md.Order_Quant = mo.Order_Quant and md.PO_No = mo.PO_No
                    left outer join MO_Spec ma on ma.OrderItem = md.OrderItem and ma.FactoryCode = md.FactoryCode
                order by mo.Due_Date, mo.Material_No, PoNo, OrderQuant, md.OrderItem
                ";

            string message = string.Format(sql, dateFrom, dateTo, factoryCode, repeatCount, dateFrom, dateTo);

            return db.Query<CheckRepeatOrder>(message).ToList();
        }

        public List<CheckDiffDueDate> CheckDiffDueDate(IConfiguration config, string factoryCode, int datediff, DateTime dateFrom, DateTime dateTo)
        {
            // 11.4
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                select mo.FactoryCode, mo.OrderItem, mo.Material_No as MaterialNo, m.PC, mo.Name, m.Box_Type as BoxType, mo.Due_Date as DueDate,
                case when mx1.maxdue is not null then mx1.maxdue when mx2.maxdue is not null then mx2.maxdue else null end MaxDue, m.CreateDate as CreatedDate,
                case when mx1.maxdue is not null then datediff(day, mx1.maxdue, mo.Due_Date)
	                 when mx2.maxdue is not null then datediff(day, mx2.maxdue, mo.Due_Date)
                     else datediff(day, m.CreateDate, mo.Due_Date) end diff
                from
                    (select * from MO_DATA
                    where Due_Date between '{0} 00:00:00' and '{1} 00:00:00' and MO_Status <> 'S' and MO_Status <> 'X' and FactoryCode = '{2}') mo
                    left outer join masterData m on m.Material_No = mo.Material_No and m.FactoryCode = mo.FactoryCode
                    left outer join
                    (select FactoryCode, Material_No, max(due_date) maxdue from MO_DATA
                    where Due_Date < '{3}' and FactoryCode = '{4}'
                    group by FactoryCode, Material_No) mx1 on mx1.Material_No = mo.Material_No and mx1.FactoryCode = mo.FactoryCode
	                left outer join
                    (select FactoryCode, Material_No, max(due_date) maxdue from MO_DATA_Archive
                    where Due_Date < '{5}' and FactoryCode = '{6}' and Material_No not like 'Z02%'
                    group by FactoryCode, Material_No) mx2 on mx2.Material_No = mo.Material_No and mx2.FactoryCode = mo.FactoryCode
                where case when mx1.maxdue is not null then datediff(day, mx1.maxdue, mo.Due_Date)
	                       when mx2.maxdue is not null then datediff(day, mx2.maxdue, mo.Due_Date)
                      else datediff(day, m.CreateDate, mo.Due_Date) end >= '{7}' and m.Material_No not like 'Z02%' and mo.FactoryCode = '{8}'
                order by DueDate
                ";

            //var a = DateTime.ParseExact(dateFrom.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var b = DateTime.ParseExact(dateTo.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var c = dateFrom.ToString("yyyy-MM-dd");
            var d = dateTo.ToString("yyyy-MM-dd");
            string message = string.Format(sql, c, d, factoryCode, c, factoryCode, c, factoryCode, datediff, factoryCode);

            return db.Query<CheckDiffDueDate>(message).ToList();
        }

        public List<CheckDueDateToolong> CheckDueDateToolong(IConfiguration config, string factoryCode, int dayCount)
        {
            // 11.5
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                select mo.FactoryCode, mo.OrderItem, mo.Material_No as MaterialNo, ma.PC, mo.PO_No as PoNo, mo.Name, mo.Order_Quant as OrderQuant,
                mo.Target_Quant as TargetQuant, mo.Due_Date as DueDate, mo.Item_Note as ItemNote, mo.Batch, mo.DateTimeStamp--, ma.PDIS_Status as PdisStatus
                from
                    (select * from MO_DATA
                    where Due_Date > DATEADD(DAY, {0}, Getdate()) and MO_Status <> 'X' and MO_Status <> 'S' and
                    year(Due_Date) between '2020' and '2099' and FactoryCode = '{1}') mo
                    left outer join MO_Spec ma on ma.OrderItem = mo.OrderItem and ma.FactoryCode = mo.FactoryCode
                order by mo.Due_Date, mo.OrderItem
                ";

            string message = string.Format(sql, dayCount, factoryCode);

            return db.Query<CheckDueDateToolong>(message).ToList();
        }

        public List<CheckOrderQtyTooMuch> CheckOrderQtyTooMuch(IConfiguration config, string factoryCode, string dateFrom, string dateTo, int multiplier)
        {
            // 11.6
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                select mo.FactoryCode, mo.OrderItem, mo.Material_No as MaterialNo, ma.PC, ma.Description, mo.Name, mo.Due_Date as DueDate, mo.Order_Quant as OrderQuant,
                case when isnull(mp.sumQty,0) <> 0 and isnull(mp.countTime,0) <> 0 then isnull(mp.sumQty,0)/isnull(mp.countTime,0) else 0 end avgQty,
                isnull(mp.sumQty,0) SumQty, isnull(mp.countTime,0) CountTime
                from
                    (select * from MO_DATA
                    where Due_Date between '{0}' and '{1}' and MO_Status <> 'S' and MO_Status <> 'X' and Material_No not like 'Z02%' and FactoryCode = '{2}') mo
                    left outer join
                    (select FactoryCode, Material_No, COUNT(*) countTime, sum(Order_Quant) sumQty --, sum(Order_Quant)/COUNT(*) avgQty
                    from MO_DATA
                    --where Due_Date < '{3}' and FactoryCode = '{2}'
                    where Due_Date between DATEADD(MONTH, -6,'{3}') and '{4}' and FactoryCode = '{2}'
                    group by FactoryCode, Material_No) mp on mp.Material_No = mo.Material_No and mp.FactoryCode = mo.FactoryCode
                    left outer join MO_Spec ma on ma.OrderItem = mo.OrderItem and ma.FactoryCode = mo.FactoryCode
                where mo.Order_Quant >= '{6}'*(case when isnull(mp.sumQty,0) <> 0 and isnull(mp.countTime,0) <> 0 then isnull(mp.sumQty,0)/isnull(mp.countTime,0) else 0 end) and isnull(mp.sumQty,0) <> 0
                order by mo.Due_Date
                ";

            string message = string.Format(sql, dateFrom, dateTo, factoryCode, dateFrom, dateFrom, factoryCode, multiplier);

            return db.Query<CheckOrderQtyTooMuch>(message).ToList();
        }

        public IEnumerable<MoDatas> GetMoDatasByDueDateRange(IConfiguration config, string factoryCode, DateTime startDueDate, DateTime endDueDate)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT mo.Id
                    ,mo.FactoryCode
                    ,mo.OrderItem
                    ,mo.Material_No as MaterialNo
                    ,ms.PC as PC
                    ,mo.Name
                    ,mo.Order_Quant as OrderQuant
                    ,mo.Due_Date as DueDate
                    ,mo.Target_Quant as TargetQuant
                    ,mo.Item_Note as ItemNote
                    ,mo.PO_No as PoNo
                    ,mo.Batch
                    ,mo.SO_Kiwi as SOKiwi
                    ,mo.CreatedDate
                    From
                        (select * from MO_DATA where MO_Status <> 'X' and MO_Status <> 'S' and FactoryCode ='{0}' and Due_Date between '{1}' and '{2}') mo
                    left outer join MO_Spec ms on ms.OrderItem = mo.OrderItem and ms.FactoryCode = mo.FactoryCode
                    order by mo.UpdatedDate desc
                ";

            string message = string.Format(sql, factoryCode, startDueDate.ToString("yyyy-MM-dd"), endDueDate.ToString("yyyy-MM-dd"));

            return db.Query<MoDatas>(message).ToList();
        }

        #region [tmp]

        //public List<string> GetReportCheck(IConfiguration config, string factoryCode, DateTime startDueDate, DateTime endDueDate)
        //{
        //    var dataSap =  GetDataReportCheckBySap(config, factoryCode, startDueDate, endDueDate);
        //    var dataKiwi = GetDataReportCheckByKiwi(config, factoryCode, startDueDate, endDueDate);
        //    return new List<string>();
        //}

        //private List<string> GetDataReportCheckBySap(IConfiguration config, string factoryCode, DateTime startDueDate, DateTime endDueDate)
        //{
        //    using (IDbConnection db = new SqlConnection(config.GetConnectionString("SapDataWarehouseConnect")))
        //    {
        //        if (db.State == ConnectionState.Closed)
        //            db.Open();
        //        //Execute sql query
        //        string sql = @"
        //            SELECT Id
        //                  ,FactoryCode
        //                  ,OrderItem
        //                  ,Material_No as MaterialNo
        //                  ,Name
        //                  ,Order_Quant as OrderQuant
        //                  ,Due_Date as DueDate
        //                  ,Target_Quant as TargetQuant
        //                  ,Item_Note as ItemNote
        //                  ,PO_No as PoNo
        //                  ,Batch
        //              FROM MO_DATA
        //              where FactoryCode ='{0}' and Due_Date between '{1}' and '{2}'
        //              order by UpdatedDate desc
        //        ";

        //        string message = string.Format(sql, factoryCode, startDueDate, endDueDate);

        //        return db.Query<string>(message).ToList();
        //    }
        //}

        //private List<string> GetDataReportCheckByKiwi(IConfiguration config, string factoryCode, DateTime startDueDate, DateTime endDueDate)
        //{
        //    using (IDbConnection db = new SqlConnection(config.GetConnectionString("KiwiConnect")))
        //    {
        //        if (db.State == ConnectionState.Closed)
        //            db.Open();
        //        //Execute sql query
        //        string sql = @"
        //            SELECT Id
        //                  ,FactoryCode
        //                  ,OrderItem
        //                  ,Material_No as MaterialNo
        //                  ,Name
        //                  ,Order_Quant as OrderQuant
        //                  ,Due_Date as DueDate
        //                  ,Target_Quant as TargetQuant
        //                  ,Item_Note as ItemNote
        //                  ,PO_No as PoNo
        //                  ,Batch
        //              FROM MO_DATA
        //              where FactoryCode ='{0}' and Due_Date between '{1}' and '{2}'
        //              order by UpdatedDate desc
        //        ";

        //        string message = string.Format(sql, factoryCode, startDueDate, endDueDate);

        //        return db.Query<string>(message).ToList();
        //    }
        //}

        #endregion [tmp]

        public ReportCheck GetReportCheck(IConfiguration config, string username, string factoryCode, string startDueDate, string endDueDate)
        {
            var UserData = PMTsDbContext.MasterUser.FirstOrDefault(x => x.UserName == username);
            var dataSap = GetDataReportCheckBySap(config, factoryCode, startDueDate, endDueDate);

            var conditionlist = GetConditionSearch(dataSap, "KIWI");
            List<GetOrderItemData> lstkiwi = new List<GetOrderItemData>();
            int indexKiwi = 0;
            foreach (var item in conditionlist)
            {
                var dataKiwi = GetDataReportCheckByKiwi(config, UserData.FactoryCode, conditionlist[indexKiwi]);
                lstkiwi.AddRange(dataKiwi);
                indexKiwi++;
            }

            var conditionlistMO = GetConditionSearch(dataSap, "MO");
            List<GetOrderItemData> lstMO = new List<GetOrderItemData>();
            int indexMO = 0;
            foreach (var item in conditionlistMO)
            {
                var dataMO = GetDataReportCheckByMoData(config, UserData.FactoryCode, conditionlistMO[indexMO]);
                lstMO.AddRange(dataMO);
                indexMO++;
            }

            ReportCheck model = new ReportCheck();

            //model.ReportFinal = (from s in dataSap
            //                     join m in lstMO
            //                     on s.OrderItem equals m.OrderItem
            //                     join k in lstkiwi
            //                     on s.Orderkiwi equals k.OrderItem
            //                     where s.DueDate != m.DueDate || Math.Floor(Convert.ToDouble(s.SaleQty) / m.PieceSet) != Convert.ToDouble(m.OrderQty) || s.DueDate != k.DueDate || Math.Floor(Convert.ToDouble(s.SaleQty) / m.PieceSet) != Convert.ToDouble(k.OrderQty)
            //                     select new ReportOrderItem
            //                     {
            //                         OrderItem = s.OrderItem,
            //                         SapOrderQty = string.IsNullOrEmpty(s.SaleQty) ? s.SaleQty : Convert.ToString(Convert.ToDouble(s.SaleQty) / m.PieceSet).Split('.')[0],
            //                         SapDueDate = s.DueDate.ToString(),
            //                         MoOrderQty = m.OrderQty,// string.IsNullOrEmpty(m.OrderQty)?m.OrderQty:Convert.ToInt32(m.OrderQty).ToString(),
            //                         MoDueDate = m.DueDate.ToString(),
            //                         KiwiOrderQty = k.OrderQty,//string.IsNullOrEmpty(k.OrderQty)?k.OrderQty:Convert.ToInt32(k.OrderQty).ToString(),
            //                         KiwiDueDate = k.DueDate.ToString()
            //                         /// Columns
            //                     }
            //              ).ToList();

            model.ReportFinal = (from s in dataSap
                                 join m in lstMO
                                 on s.OrderItem equals m.OrderItem
                                 //join k in lstkiwi
                                 //on s.Orderkiwi equals k.OrderItem
                                 where s.DueDate != m.DueDate || Math.Floor(Convert.ToDouble(s.SaleQty)) != Convert.ToDouble(m.OrderQty)
                                 select new ReportOrderItem
                                 {
                                     OrderItem = s.OrderItem,
                                     SapOrderQty = string.IsNullOrEmpty(s.SaleQty) ? s.SaleQty : Convert.ToString(Convert.ToDouble(s.SaleQty)).Split('.')[0],
                                     SapDueDate = s.DueDate.ToString(),
                                     MoOrderQty = m.OrderQty,// string.IsNullOrEmpty(m.OrderQty)?m.OrderQty:Convert.ToInt32(m.OrderQty).ToString(),
                                     MoDueDate = m.DueDate.ToString()
                                     //KiwiOrderQty = k.OrderQty,//string.IsNullOrEmpty(k.OrderQty)?k.OrderQty:Convert.ToInt32(k.OrderQty).ToString(),
                                     //KiwiDueDate = k.DueDate.ToString()
                                     /// Columns
                                 }
                          ).ToList();

            //on s.OrderItem == m.OrderItem && s.or not orderqty and duedate != duedate )

            ////List<GetOrderItemData> lstDiffSap = new List<GetOrderItemData>();
            //List<GetOrderItemDataSap> tmpDiffSap = new List<GetOrderItemDataSap>();
            //var diffsapkiwi = dataSap.Where(a => !lstkiwi.Any(a1 => a1.OrderItem == a.Orderkiwi)).ToList();
            //var diffsapmo = dataSap.Where(a => !lstMO.Any(a1 => a1.OrderItem == a.OrderItem)).ToList();
            //tmpDiffSap.AddRange(diffsapkiwi);
            //tmpDiffSap.AddRange(diffsapmo);
            //var lstDiffSap = tmpDiffSap.Select(x => new { x.OrderItem, x.DueDate }).Distinct().ToList();
            //// lstDiffSap.AddRange(distinctSap);

            //List<GetOrderItemData> tmpDiffKiwi = new List<GetOrderItemData>();
            //var diffkiwisap = lstkiwi.Where(a => !dataSap.Any(a1 => a1.Orderkiwi == a.OrderItem)).ToList();
            //var diffkiwimo = lstkiwi.Where(a => !lstMO.Any(a1 => a1.OrderItem == a.OrderItem)).ToList();
            //tmpDiffKiwi.AddRange(diffkiwisap);
            //tmpDiffKiwi.AddRange(diffkiwimo);
            //var lstDiffKiwi = tmpDiffKiwi.Select(x => new { x.OrderItem, x.DueDate }).Distinct().ToList();

            //List<GetOrderItemData> tmpDiffMO = new List<GetOrderItemData>();
            //var diffmosap = lstMO.Where(a => !dataSap.Any(a1 => a1.OrderItem == a.OrderItem)).ToList();
            //var diffmokiwi = lstMO.Where(a => !lstkiwi.Any(a1 => a1.OrderItem == a.OrderItem)).ToList();
            //tmpDiffMO.AddRange(diffkiwisap);
            //tmpDiffMO.AddRange(diffkiwimo);
            //var lstDiffMo = tmpDiffMO.Select(x => new { x.OrderItem, x.DueDate }).Distinct().ToList();

            //int maxdatalst = lstDiffSap.Count();
            //if (maxdatalst < lstDiffKiwi.Count())
            //{
            //    maxdatalst = lstDiffKiwi.Count();
            //}
            //if (maxdatalst < lstDiffMo.Count())
            //{
            //    maxdatalst = lstDiffMo.Count();
            //}

            //List<ReportOrderItemData> model = new List<ReportOrderItemData>();
            //for (int i = 0; i < maxdatalst; i++)
            //{
            //    ReportOrderItemData tmp = new ReportOrderItemData();
            //    try { tmp.OrderItemSap = lstDiffSap[i].OrderItem;  } catch { tmp.OrderItemSap = null; }
            //    try { tmp.DueDateSap = lstDiffSap[i].DueDate; } catch { tmp.DueDateSap = null; }

            //    try { tmp.OrderItemKiwi = lstDiffKiwi[i].OrderItem; } catch { tmp.OrderItemKiwi = null; }
            //    try { tmp.DueDateKiwi = lstDiffKiwi[i].DueDate; } catch { tmp.DueDateKiwi = null; }

            //    try { tmp.OrderItemMoData = lstDiffMo[i].OrderItem; } catch { tmp.OrderItemKiwi = null; }
            //    try { tmp.DueDateMoData = lstDiffMo[i].DueDate; } catch { tmp.DueDateKiwi = null; }

            //    model.Add(tmp);

            //}

            //return model;
            //ReportCheck model = new ReportCheck();
            //model.ReportSap = (from s in lstDiffSap select new GetOrderItemDataReport { DueDate = s.DueDate.ToString(), OrderItem = s.OrderItem }).ToList();
            //model.ReportKiwi = (from s in lstDiffKiwi select new GetOrderItemDataReport {DueDate = s.DueDate.ToString() , OrderItem = s.OrderItem }).ToList();
            //model.ReportMo = (from s in lstDiffMo select new GetOrderItemDataReport { DueDate = s.DueDate.ToString(), OrderItem = s.OrderItem }).ToList();
            return model;
        }

        public List<string> GetConditionSearch(List<GetOrderItemDataSap> dataSap, string flag)
        {
            List<string> conditionlist = new List<string>();
            string ss = string.Empty;
            for (int i = 1; i <= dataSap.Count; i++)
            {
                string mat;
                if (flag == "KIWI")
                {
                    mat = "'" + dataSap[i - 1].Orderkiwi.ToString() + "',";
                }
                else
                {
                    mat = "'" + dataSap[i - 1].OrderItem.ToString() + "',";
                }

                ss += mat;
                if ((i % 1000) == 0)
                {
                    string tmpss = ss[..^1];
                    conditionlist.Add(tmpss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss))
            {
                string tmpss2 = ss[..^1];
                conditionlist.Add(tmpss2);
            }
            return conditionlist;
        }

        private List<GetOrderItemDataSap> GetDataReportCheckBySap(IConfiguration config, string factoryCode, string startDueDate, string endDueDate)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("SAPConnectR"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                        SELECT Case When ss.[item no] <= 99 Then CONCAT(RTRIM(LTRIM(ss.[sales document no])),RTRIM(LTRIM(REVERSE(ss.[item no]))))
                            ELSE CONCAT(RTRIM(LTRIM(ss.[sales document no])),RTRIM(LTRIM(left(ss.[item no], len(ss.[item no])-1)))) End AS OrderItem,
                            Case When ss.[item no] <= 99 Then CONCAT(Right(trim(ss.[sales document no]), 8),RTRIM(LTRIM(REVERSE(ss.[item no]))))
                            ELSE CONCAT(Right(trim(ss.[sales document no]), 8),RTRIM(LTRIM(left(ss.[item no], len(ss.[item no])-1)))) End AS OrderKiwi,
                            si.[sales qty] as SaleQty, ss.qty, ss.dueDate as DueDate
                          FROM (select [sales document no], [item no], sum([sales qty]) qty, min([due date]) dueDate from [salesdocscheduleline]
                            where [due date] between '{0} 00:00:00.000' and '{1} 23:59:59.000' group by [sales document no], [item no]) ss
                            left outer join [salesdocitem] si on si.[sales document no] = ss.[sales document no] and si.[item no] = ss.[item no]
                        where si.plant = '{2}' and left(si.[Item Note],3) <> 'SSS' and si.[reject reason] <> '93'
                ";

            string message = string.Format(sql, startDueDate, endDueDate, factoryCode);

            return db.Query<GetOrderItemDataSap>(message).ToList();
        }

        private List<GetOrderItemData> GetDataReportCheckByKiwi(IConfiguration config, string factoryCode, string condition)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("KIWIConnectR"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                         SELECT Distinct [SalesOrder] as OrderItem, Due_Date as DueDate ,Orders_Quantity as OrderQty
                        FROM [dbo].[LINE_UP]
                        WHERE (Plant = '{0}')
                        AND (SalesOrder IN ({1}))
                        AND (SeriesNo = '1') AND (SeqNo = '1')
                        -- ORDER BY UpdatedDate DESC
                ";

            string message = string.Format(sql, factoryCode, condition);

            return db.Query<GetOrderItemData>(message).ToList();
        }

        private List<GetOrderItemData> GetDataReportCheckByMoData(IConfiguration config, string factoryCode, string condition)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            //string sql = @"
            //                  SELECT
            //                   [OrderItem] as OrderItem
            //                  ,[Order_Quant]    as OrderQty
            //                  ,[Due_Date]    as    DueDate
            //              FROM [MO_DATA]
            //               where FactoryCode = '{0}'  and OrderItem in ({1})
            //";
            string sql = @" SELECT mo.FactoryCode, mo.OrderItem as OrderItem, mo.Material_No, mo.Order_Quant, mo.Target_Quant,
                                case when ms.Piece_Set = 0 or ms.Piece_Set is null then 1 else ms.Piece_Set end PieceSet,
                                case when ms.Piece_Set = 0 or ms.Piece_Set is null then mo.Order_Quant else mo.Order_Quant/ms.Piece_Set end OrderQty,
                                mo.Due_Date as DueDate
                                FROM [MO_DATA] mo left outer join MO_Spec ms on ms.OrderItem = mo.OrderItem
                                and ms.FactoryCode = mo.FactoryCode
                               where mo.FactoryCode = '{0}'  and mo.OrderItem in ({1})
                ";

            string message = string.Format(sql, factoryCode, condition);

            return db.Query<GetOrderItemData>(message).ToList();
        }

        public List<CheckRepeatOrder> GetDataReportMoManual(IConfiguration config, string factoryCode, string materialNo, string custName, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus)
        {
            string condition = string.Empty;
            //condition = $" and Due_Date between '2021-01-08' and '2021-06-08'";
            condition = !string.IsNullOrEmpty(startDueDate) ? condition + $" and Due_Date between '{startDueDate}' and '{endDueDate}'" : condition;
            condition = !string.IsNullOrEmpty(startUpdateDate) ? condition + $" and UpdatedDate between '{startUpdateDate}' and '{endUpdateDate}'" : condition;
            condition = !string.IsNullOrEmpty(materialNo) ? condition + $" and md.Material_No like '%{materialNo}%'" : condition;
            condition = !string.IsNullOrEmpty(custName) ? condition + $" and md.Name like '%{custName}%'" : condition;
            condition = !string.IsNullOrEmpty(pc) ? condition + $" and ms.PC like '%{pc}%'" : condition;
            condition = !string.IsNullOrEmpty(po) ? condition + $" and md.PO_No like '%{po}%'" : condition;
            so = so == null ? "" : so.Contains('*') ? so.Replace("*", "%") : so;
            condition = !string.IsNullOrEmpty(so) ? condition + $" and md.OrderItem like '%{so}%'" : condition;
            note = note == null ? "" : note.Contains('*') ? note.Replace("*", "%") : note;
            condition = !string.IsNullOrEmpty(note) ? condition + $" and isnull(md.Item_Note,'')+isnull(md.Batch,'') like '%{note}%'" : condition;
            condition = !string.IsNullOrEmpty(soStatus) ? condition + $" and md.MO_Status in ({soStatus})" : condition;
            condition = !string.IsNullOrEmpty(startCreateDate) ? condition + $" and convert(datetime, left(md.DateTimeStamp,6), 12) between '{startCreateDate}' and '{endCreateDate}'" : condition;
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
            {
                db.Open();
            }

            string sql = @"select md.FactoryCode, md.MO_Status MOStatus,
                                md.OrderItem,
                                md.Material_No as MaterialNo,
                                ms.PC,
                                md.Name,
                                ms.Description,
                                md.Order_Quant as OrderQuant,
                                isnull(md.Target_Quant,0) as TargetQuant,
                                isnull(md.OriginalDueDate, md.Due_Date) as OriginalDueDate,
                                md.Due_Date as DueDate,
                                md.Item_Note as ItemNote,
                                md.Batch,
                                md.PO_No as PoNo,
                                md.DateTimeStamp,
                                isnull(md.CreatedDate,cast('20' + SUBSTRING(DateTimeStamp, 1, 2) + '-' + SUBSTRING(DateTimeStamp, 3, 2) + '-' + SUBSTRING(DateTimeStamp, 5, 2) as datetime)) CreateDate, UpdatedDate
                                from MO_DATA md left outer join MO_Spec ms on ms.OrderItem = md.OrderItem and ms.FactoryCode = md.FactoryCode
                                where md.FactoryCode = '{0}' " + condition + " order by md.OrderItem";

            string message = string.Format(sql, factoryCode);

            return db.Query<CheckRepeatOrder>(message).ToList();
        }

        public IEnumerable<MoData> GetMoDatasByDueDateRangeAndStatus(IConfiguration config, string factoryCode, string status, DateTime dateFrom, DateTime dateTo)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT Id
                    ,FactoryCode
                    ,OrderItem
                    ,Material_No as MaterialNo
                    ,Name
                    ,Order_Quant as OrderQuant
                    ,Due_Date as DueDate
                    ,Target_Quant as TargetQuant
                    ,Item_Note as ItemNote
                    ,PO_No as PoNo
                    ,Batch
                    ,SO_Kiwi as SOKiwi
                      FROM MO_DATA
                      where Upper(MO_Status) = 'S' and FactoryCode ='{0}' and Due_Date between '{1}' and '{2}'
                      order by UpdatedDate desc
                ";

            string message = string.Format(sql, factoryCode, dateFrom, dateTo);

            return db.Query<MoData>(message).ToList();
        }

        public IEnumerable<MoData> GetMoDataByInterface_TIPs(IConfiguration configuration, bool Interface_TIPs)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                        SELECT  [Id] as Id
                        ,[FactoryCode] as FactoryCode
                        ,[MO_Status] as MoStatus
                        ,[OrderItem] as OrderItem
                        ,[Material_No] as MaterialNo
                        ,[Name] as Name
                        ,[Order_Quant] as OrderQuant
                        ,[Tolerance_Over] as ToleranceOver
                        ,[Tolerance_Under] as ToleranceUnder
                        ,[Due_Date] as DueDate
                        ,[Target_Quant] as TargetQuant
                        ,[Item_Note] as ItemNote
                        ,[District] as District
                        ,[PO_No] as PoNo
                        ,[DateTimeStamp] as DateTimeStamp
                        ,[Printed] as Printed
                        ,[Batch] as Batch
                        ,[Due_Text] as DueText
                        ,[Sold_to] as SoldTo
                        ,[Ship_to] as ShipTo
                        ,[PlanStatus] as PlanStatus
                        ,[StockQty] as StockQty
                        ,[IsCreateManual] as IsCreateManual
                        ,[SentKIWI] as SentKIWI
                        ,[OriginalDueDate] as OriginalDueDate
                        ,[CreatedDate] as CreatedDate
                        ,[CreatedBy] as CreatedBy
                        ,[UpdatedDate] as UpdatedDate
                        ,[UpdatedBy] as UpdatedBy
                        ,[MORNo] as MORNo
                        ,[SO_Kiwi] as SOKiwi
                        ,[Square_INCH] as SquareINCH
                        ,[SBO_External_Number] as SBOExternalNumber
                    FROM MO_DATA
                    WHERE Interface_TIPs = '{0}'";

            string message = string.Format(sql, Interface_TIPs);

            return db.Query<MoData>(message).ToList();
        }

        public IEnumerable<MoData> GetMoDataByInterface_TIPs(IConfiguration configuration, string FactoryCode, bool Interface_TIPs)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                        SELECT  [Id] as Id
                        ,[FactoryCode] as FactoryCode
                        ,[MO_Status] as MoStatus
                        ,[OrderItem] as OrderItem
                        ,[Material_No] as MaterialNo
                        ,[Name] as Name
                        ,[Order_Quant] as OrderQuant
                        ,[Tolerance_Over] as ToleranceOver
                        ,[Tolerance_Under] as ToleranceUnder
                        ,[Due_Date] as DueDate
                        ,[Target_Quant] as TargetQuant
                        ,[Item_Note] as ItemNote
                        ,[District] as District
                        ,[PO_No] as PoNo
                        ,[DateTimeStamp] as DateTimeStamp
                        ,[Printed] as Printed
                        ,[Batch] as Batch
                        ,[Due_Text] as DueText
                        ,[Sold_to] as SoldTo
                        ,[Ship_to] as ShipTo
                        ,[PlanStatus] as PlanStatus
                        ,[StockQty] as StockQty
                        ,[IsCreateManual] as IsCreateManual
                        ,[SentKIWI] as SentKIWI
                        ,[OriginalDueDate] as OriginalDueDate
                        ,[CreatedDate] as CreatedDate
                        ,[CreatedBy] as CreatedBy
                        ,[UpdatedDate] as UpdatedDate
                        ,[UpdatedBy] as UpdatedBy
                        ,[MORNo] as MORNo
                        ,[SO_Kiwi] as SOKiwi
                        ,[Square_INCH] as SquareINCH
                    FROM MO_DATA
                    WHERE Interface_TIPs = '{0}' AND FactoryCode = '{1}'";

            string message = string.Format(sql, Interface_TIPs, FactoryCode);

            return db.Query<MoData>(message).ToList();
        }

        public void UpdateMoDataFlagInterfaceTips(string factoryCode, string orderItem, string interface_TIPs)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var MoData = PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).FirstOrDefault();
                    MoData.InterfaceTips = Convert.ToBoolean(interface_TIPs);
                    MoData.UpdatedBy = "TIPs";
                    MoData.UpdatedDate = DateTime.Now;

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        //boo editblock
        public EditBlockPlatenModel GetBlockPlatenMaster(string factorycode, string material, string pc)
        {
            EditBlockPlatenModel model = new EditBlockPlatenModel();
            var data = PMTsDbContext.MasterData.Where(x => (x.FactoryCode == factorycode && x.MaterialNo.Contains(material) || x.Pc.Contains(pc)) && x.PdisStatus != "X").ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    EditBlockPlatenMaster tmp = new EditBlockPlatenMaster();
                    tmp.factorycode = item.FactoryCode;
                    tmp.materialno = item.MaterialNo;
                    tmp.pc = item.Pc;
                    tmp.custname = item.CustName;
                    tmp.saletext = item.SaleText1;
                    model.editBlockPlatenMasters.Add(tmp);
                }
            }
            return model;
        }

        public EditBlockPlatenModel GetBlockPlatenRouting(string factorycode, string material)
        {
            EditBlockPlatenModel model = new EditBlockPlatenModel();
            var data = PMTsDbContext.Routing.Where(x => x.FactoryCode == factorycode && x.MaterialNo.Contains(material)).ToList();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    if (CheckMachineGroup(factorycode, item.Machine))
                    {
                        EditBlockPlatenRouting tmp = new EditBlockPlatenRouting();
                        tmp.factorycode = item.FactoryCode;
                        tmp.materialno = item.MaterialNo;
                        tmp.machine = item.Machine;
                        tmp.mylano = item.MylaNo;
                        tmp.printingplate = item.BlockNo;
                        tmp.cuttingdieno = item.PlateNo;
                        tmp.seq = item.SeqNo.ToString();
                        model.editBlockPlatenRouting.Add(tmp);
                    }
                }
            }
            return model;
        }

        private bool CheckMachineGroup(string factorycode, string machine)
        {
            var tmp = PMTsDbContext.Machine.Where(m => m.FactoryCode == factorycode && m.Machine1 == machine).FirstOrDefault();
            if (tmp.MachineGroup == "2")
            {
                return true;
            }
            else if (tmp.MachineGroup == "3")
            {
                return true;
            }
            else if (tmp.MachineGroup == "4")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateBlockPlatenRouting(string factoryCode, string username, List<EditBlockPlatenRouting> model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in model)
                    {
                        var tmp = PMTsDbContext.Routing.Where(m => m.FactoryCode == factoryCode && m.MaterialNo == item.materialno && m.SeqNo == Convert.ToInt16(item.seq)).FirstOrDefault();
                        tmp.MylaNo = item.mylano;
                        tmp.BlockNo = item.printingplate;
                        tmp.PlateNo = item.cuttingdieno;
                        tmp.UpdatedBy = username;
                        tmp.UpdatedDate = DateTime.Now;

                        PMTsDbContext.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public MoData GetMoDataBySOKiwi(string factoryCode, string sO_Kiwi)
        {
            return PMTsDbContext.MoData.FirstOrDefault(m => m.FactoryCode.Equals(factoryCode) && m.SoKiwi.Equals(sO_Kiwi));
        }

        public bool CheckMaterialNo(string MaterialNo)
        {
            return PMTsDbContext.MoData.Count(m => m.MaterialNo.Equals(MaterialNo)) > 0;
        }

        public void UpdateMoDataPrintNo(string factoryCode, string orderItem, int printRoundNo, int allowancePrintNo, int afterPrintNo, int drawAmountNo, string userBy)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var MoData = PMTsDbContext.MoData.Where(z => z.FactoryCode == factoryCode && z.OrderItem == orderItem).FirstOrDefault();
                    MoData.PrintRoundNo = printRoundNo;
                    MoData.AllowancePrintNo = allowancePrintNo;
                    MoData.AfterPrintNo = afterPrintNo;
                    MoData.DrawAmountNo = drawAmountNo;
                    MoData.UpdatedBy = userBy;
                    MoData.UpdatedDate = DateTime.Now;

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public PrintMasterCardMOModel GetDataForMasterCard(PrintMastercardMO printMastercardMO, string factoryCode, IMapper mapper, IStringLocalizer _localizer)
        {
            var mastercardModel = new PrintMasterCardMOModel();
            mastercardModel.MasterCardMOs = new List<MasterCardMO>();
            try
            {
                if (printMastercardMO.MoDatas != null && printMastercardMO.MoDatas.Count > 0)
                {
                    #region Thai

                    List<string> saleOrders = printMastercardMO.MoDatas.Select(m => m.OrderItem).ToList();
                    List<string> factoryCodes = printMastercardMO.MoDatas.Select(m => m.FactoryCode.Trim()).Distinct().ToList();
                    List<string> flutes = printMastercardMO.MoSpecs.Select(m => m.Flute.Trim()).Distinct().ToList();
                    List<string> planCodes = printMastercardMO.MoRoutings.Select(m => m.PlanCode.ToUpper().Trim()).Distinct().ToList();
                    List<string> materialNos = printMastercardMO.MoDatas.Select(m => m.MaterialNo).Distinct().ToList();
                    List<string> scoreTypes = printMastercardMO.MoRoutings.Select(m => m.ScoreType).Distinct().ToList();

                    #region Get init datas for mastercard

                    var baseOfFluteTrs = new List<FluteTr>();
                    var baseOfPmts = new List<PmtsConfig>();
                    var baseOfMachines = new List<DataAccess.Models.Machine>();
                    var baseOfScoreTypes = new List<ScoreType>();
                    var baseOfQualitySpecs = new List<QualitySpec>();
                    var baseOfCoatings = new List<Coating>();
                    var baseOfBoardUses = new List<BoardUse>();

                    baseOfFluteTrs = PMTsDbContext.FluteTr.Where(f => flutes.Contains(f.FluteCode.Trim())).Where(f => factoryCodes.Contains(f.FactoryCode.Trim())).ToList();
                    baseOfPmts = PMTsDbContext.PmtsConfig.Where(p => factoryCodes.Contains(p.FactoryCode.Trim())).ToList();
                    baseOfMachines = PMTsDbContext.Machine.Where(m => planCodes.Contains(m.PlanCode.ToUpper().Trim())).ToList();
                    baseOfMachines = baseOfMachines.Where(m => m.FactoryCode == factoryCode).ToList();
                    baseOfScoreTypes = PMTsDbContext.ScoreType.Where(s => scoreTypes.Contains(s.ScoreTypeId)).ToList();
                    baseOfQualitySpecs = PMTsDbContext.QualitySpec.Where(q => materialNos.Contains(q.MaterialNo)).Where(q => q.FactoryCode == factoryCode).ToList();
                    baseOfCoatings = PMTsDbContext.Coating.Where(c => materialNos.Contains(c.MaterialNo)).Where(c => c.FactoryCode == factoryCode).ToList();
                    baseOfBoardUses = PMTsDbContext.BoardUse.Where(b => materialNos.Contains(b.MaterialNo)).Where(b => b.FactoryCode == factoryCode).ToList();

                    #endregion Get init datas for mastercard

                    foreach (var OrderItem in saleOrders)
                    {
                        //MO_DATA
                        var moData = new MoDataPrintMastercard();
                        var modataForUpdate = printMastercardMO.MoDatas.FirstOrDefault(m => m.OrderItem.Trim() == OrderItem);
                        moData = mapper.Map<MoData, MoDataPrintMastercard>(modataForUpdate);

                        #region Update printed

                        var originalMOData = PMTsDbContext.MoData.AsNoTracking().FirstOrDefault(m => m.OrderItem.Trim() == OrderItem);
                        if (originalMOData != null)
                        {
                            var printed = originalMOData.Printed == null ? 1 : originalMOData.Printed + 1;
                            originalMOData.Printed = printed;
                            moData.Printed = printed;
                            PMTsDbContext.MoData.Update(originalMOData);
                            var logPrintMO = new LogPrintMo
                            {
                                Id = 0,
                                FactoryCode = factoryCode,
                                OrderItem = originalMOData.OrderItem,
                                Printed = printed,
                                PrintedBy = "PrintFromAPI",
                                PrintedDate = DateTime.Now,
                            };

                            PMTsDbContext.LogPrintMo.Add(logPrintMO);
                            PMTsDbContext.SaveChanges();
                        }

                        #endregion Update printed

                        //MO_SPEC
                        var moSpec = new MoSpec();
                        var moSpecForUpdate = printMastercardMO.MoSpecs.FirstOrDefault(m => m.OrderItem.Trim() == OrderItem);
                        moSpec = moSpecForUpdate;

                        var isSheetBoard = moSpec.MaterialNo.ToLower().Contains("s/b") || moSpec.Hierarchy.Substring(2, 2).ToLower().Contains("sb");
                        //MO_ROUTING >> First
                        MoRouting moRouting = new MoRouting();
                        var moRoutings = new List<MoRoutingPrintMastercard>();
                        var moRoutingForUpdate = new List<MoRouting>();
                        moRoutingForUpdate = printMastercardMO.MoRoutings.Where(r => r.OrderItem.Trim() == OrderItem).OrderBy(r => r.SeqNo).ToList();
                        var moBindRoutings = moRoutingForUpdate.OrderBy(r => r.SeqNo).ToList();
                        foreach (var item in moBindRoutings)
                        {
                            moRoutings.Add(mapper.Map<MoRoutingPrintMastercard>(item));
                        }
                        var mainMORoutings = new List<MasterCardMoRouting>();
                        var partOfMORoutings = new List<MasterCardMoRouting>();
                        var hierarchyLv2 = string.Empty;
                        var formGroup = string.Empty;
                        string myFactory = string.Empty, ISO_DocDate = string.Empty, ISO_DocName = string.Empty;
                        double tolerance_Over = 0, tolerance_Under = 0;
                        var tempStations = new List<Station>();
                        var stations = new List<Station>();
                        string Setcut = "", SetLeng = "";
                        MasterCardMO result = new MasterCardMO();
                        var firstMachine = 0;
                        var countRoutings = 0;
                        var lineCount = 0;
                        double? weightOut = null;
                        var conditionPPCPrint = false;

                        #region Find myFactory PmtsConfig

                        var pmts = baseOfPmts.Where(b => b.FactoryCode == moData.FactoryCode);
                        foreach (var x in pmts)
                        {
                            switch (x.FucName)
                            {
                                case "Company":
                                    myFactory = x.FucValue;
                                    break;

                                case "ISO_DocDate":
                                    ISO_DocDate = x.FucValue;
                                    break;

                                case "ISO_DocName":
                                    ISO_DocName = x.FucValue;
                                    break;
                            }
                        }

                        #endregion Find myFactory PmtsConfig

                        #region Set Stations

                        var boardName = moSpec == null ? "" : moSpec.Board;
                        var fluteTrs = baseOfFluteTrs.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).OrderBy(s => s.Item).ToList();
                        boardName = clean(boardName);
                        var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();
                        stations.Clear();
                        var boardNo = 0;
                        foreach (var fluteTr in fluteTrs)
                        {
                            var item = fluteTr.Item.Value;
                            var paperGrade = "";
                            if (fluteTr.FluteCode[0].ToString().Equals("o", StringComparison.CurrentCultureIgnoreCase) && fluteTr.Station.ToString().ToLower().Equals("top"))
                            {
                                item = 999;
                            }

                            if (boardNo >= boardSplit.Count)
                            {
                                paperGrade = "";
                            }
                            else
                            {
                                paperGrade = boardSplit[boardNo];
                            }

                            stations.Add(new Station
                            {
                                item = item,
                                TypeOfStation = fluteTr.Station,
                                PaperGrade = paperGrade,
                                Flute = fluteTr.FluteCode
                            });
                            boardNo++;
                        }

                        #endregion Set Stations

                        if (isSheetBoard)
                        {
                            #region Set MO Routing

                            if (moRoutings.Count > 0)
                            {
                                var hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
                                //has machine "COR" in MO routing
                                if (hasCORR)
                                {
                                    moRouting = moRoutings.FirstOrDefault(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor"));
                                    if (moRouting.CutNo != null && moRouting.CutNo.Value > 0)
                                    {
                                        //set จน.ตัด & ยาวเมตร
                                        if (moData.TargetQuant % moRouting.CutNo > 0)
                                        {
                                            Setcut = String.Format("{0:N0}", Convert.ToDouble(moData.TargetQuant / moRouting.CutNo) + 1);
                                        }
                                        else
                                        {
                                            Setcut = String.Format("{0:N0}", Convert.ToDouble(moData.TargetQuant / moRouting.CutNo));
                                        }

                                        int? t = 0;
                                        if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
                                        {
                                            t = moData.OrderQuant * moSpec.CutSheetLeng / 1000;
                                            SetLeng = String.Format("{0:N0}", t);
                                        }
                                        else
                                        {
                                            if ((moSpec.CutSheetLeng * moData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
                                            {
                                                //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
                                                t = (((moSpec.CutSheetLeng * moData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
                                                SetLeng = String.Format("{0:N0}", t);
                                            }
                                            else
                                            {
                                                SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * moData.TargetQuant) / (moRouting.CutNo * 1000)));
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (var routing in moRoutings)
                            {
                                countRoutings++;
                                var machineName = "";
                                var machineGroup = "";
                                bool showProcess = true;
                                var machine = baseOfMachines.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
                                machineGroup = machine != null ? machine.MachineGroup : null;
                                showProcess = machine != null ? machine.ShowProcess : true;
                                if (showProcess)
                                {
                                    if (routing.McMove.Value)
                                    {
                                        machineName = ReMachineName(null, routing);
                                    }
                                    else
                                    {
                                        machineName = routing.Machine + _localizer["Don't move the machine"].Value;
                                    }

                                    routing.Machine = machineName;
                                    if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
                                    {
                                        var scoreType = new ScoreType();
                                        scoreType = baseOfScoreTypes.FirstOrDefault(m => m.ScoreTypeId == routing.ScoreType && m.FactoryCode == factoryCode);

                                        routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
                                    }
                                    else
                                    {
                                        routing.ScoreType = routing.ScoreType;
                                    }
                                    routing.Coatings = new List<Coating>();
                                    lineCount += MORoutingLineCount(routing);
                                    conditionPPCPrint = lineCount > 42;

                                    if (conditionPPCPrint)
                                    {
                                        if (firstMachine == 0)
                                        {
                                            partOfMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                            });
                                        }
                                        else
                                        {
                                            partOfMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = false
                                            });
                                        }

                                        firstMachine++;
                                    }
                                    else
                                    {
                                        if (firstMachine == 0)
                                        {
                                            mainMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = machine == null && routing.Machine.Contains("COR") ? true : machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                            });
                                        }
                                        else
                                        {
                                            mainMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = false
                                            });
                                        }
                                        firstMachine++;
                                    }
                                }

                                if (countRoutings == moRoutings.Count)
                                {
                                    weightOut = routing.WeightOut.Value;
                                }
                            }

                            #endregion Set MO Routing

                            #region Set Stations

                            //var boardName = moSpec == null ? "" : moSpec.Board;
                            //// replace
                            //boardName = clean(boardName);
                            //var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                            //var fluteTrs = PMTsDbContext.FluteTr.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).ToList();

                            //stations.Clear();
                            //var boardNo = 0;
                            //foreach (var fluteTr in fluteTrs)
                            //{
                            //    var item = fluteTr.Item.Value;
                            //    var paperGrade = "";
                            //    if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                            //    {
                            //        item = 999;
                            //    }

                            //    if (boardNo >= boardSplit.Count)
                            //    {
                            //        paperGrade = "";
                            //    }
                            //    else
                            //    {
                            //        paperGrade = boardSplit[boardNo];
                            //    }

                            //    stations.Add(new Station
                            //    {
                            //        item = item,
                            //        TypeOfStation = fluteTr.Station,
                            //        PaperGrade = paperGrade,
                            //        Flute = fluteTr.FluteCode
                            //    });
                            //    boardNo++;
                            //}

                            #endregion Set Stations

                            #region Set Mastercard

                            var ymd = moData.DateTimeStamp;
                            result.ProductType = moSpec.ProType;
                            result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
                            result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
                            result.FactoryCode = factoryCode;
                            result.Factory = myFactory;
                            result.DocName = ISO_DocName;
                            result.DocDate = ISO_DocDate;
                            result.OrderItem = moData.OrderItem;
                            result.Material_No = moData.MaterialNo;
                            result.Part_No = moSpec.PartNo;
                            result.PC = moSpec.Pc;
                            result.Cust_Name = moData.Name.Length > 40 ? string.Concat(moData.Name[..40], $" ({moData.SoldTo})") : string.Concat(moData.Name, $" ({moData.SoldTo})");
                            result.CustNameNOSoldto = moData.Name.Length > 40 ? moData.Name[..40] : moData.Name;
                            result.CustNameNOSoldto = moData.Name.Length > 40 ? moData.Name[..40] : moData.Name;
                            //result.CustomerContact = $"{_firstName} {_telephone}";
                            result.Description = moSpec.Description;
                            result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
                            result.EanCode = moSpec.EanCode;
                            result.Box_Type = moSpec.BoxType;
                            result.RSC_Style = moSpec.RscStyle;
                            result.JoinType = moSpec.JoinType;
                            result.Print_Method = moSpec.PrintMethod;
                            result.PalletSize = moSpec.PalletSize;
                            result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
                            result.BunLayer = moSpec.BunLayer == null ? null : moSpec.BunLayer;
                            result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
                            result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
                            result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
                            result.Material_Type = moSpec.MaterialType;
                            result.Status_Flag = moSpec.StatusFlag;
                            result.Wire = moSpec.Wire;
                            result.Wid = moSpec.Wid;
                            result.Leg = moSpec.Leg;
                            result.Hig = moSpec.Hig;
                            result.CutSheetWid = moSpec.CutSheetWid;
                            result.CutSheetLeng = moSpec.CutSheetLeng;
                            result.Flute = moSpec.Flute;
                            result.Batch = moData.Batch;
                            result.ItemNote = moData.ItemNote;
                            result.Due_Text = moData.DueText;
                            result.Tolerance_Over = tolerance_Over;
                            result.Tolerance_Under = tolerance_Under;
                            result.Order_Quant = moData.OrderQuant != 0 ? moData.OrderQuant : 0;
                            result.ScoreW1 = moSpec.ScoreW1;
                            result.Scorew2 = moSpec.Scorew2;
                            result.Scorew3 = moSpec.Scorew3;
                            result.Scorew4 = moSpec.Scorew4;
                            result.Scorew5 = moSpec.Scorew5;
                            result.Scorew6 = moSpec.Scorew6;
                            result.Scorew7 = moSpec.Scorew7;
                            result.Scorew8 = moSpec.Scorew8;
                            result.Scorew9 = moSpec.Scorew9;
                            result.Scorew10 = moSpec.Scorew10;
                            result.Scorew11 = moSpec.Scorew11;
                            result.Scorew12 = moSpec.Scorew12;
                            result.Scorew13 = moSpec.Scorew13;
                            result.Scorew14 = moSpec.Scorew14;
                            result.Scorew15 = moSpec.Scorew15;
                            result.Scorew16 = moSpec.Scorew16;
                            result.ScoreL2 = moSpec.ScoreL2;
                            result.ScoreL3 = moSpec.ScoreL3;
                            result.ScoreL4 = moSpec.ScoreL4;
                            result.ScoreL5 = moSpec.ScoreL5;
                            result.ScoreL6 = moSpec.ScoreL6;
                            result.ScoreL7 = moSpec.ScoreL7;
                            result.ScoreL8 = moSpec.ScoreL8;
                            result.ScoreL9 = moSpec.ScoreL9;
                            result.FormGroup = "SB";
                            result.Palletization_Path = moSpec.PalletizationPath;
                            result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) || string.IsNullOrWhiteSpace(moSpec.PalletizationPath) ? "" : _ConvertPictureToBase64(moSpec.PalletizationPath);
                            result.DiecutPict_Path = moSpec.DiecutPictPath;
                            result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) || string.IsNullOrWhiteSpace(moSpec.DiecutPictPath) ? "" : _ConvertPictureToBase64(moSpec.DiecutPictPath);
                            result.Change = moSpec.Change;
                            result.Printed = moData.Printed == null ? 0 : moData.Printed.Value;
                            result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
                            result.StockQty = moData.StockQty;
                            result.Distinct = string.IsNullOrEmpty(moData.District) ? moData.District : moData.District.Trim();
                            result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
                            result.Slit = ConvertInt16ToShort(moSpec.Slit);
                            result.Target_Quant = Convert.ToString(moData.TargetQuant) == null ? "0" : Convert.ToString(moData.TargetQuant);
                            result.CutNo = Setcut != null ? Setcut : "0";
                            result.Leng = SetLeng;
                            result.High_Value = moSpec.HighValue;
                            result.Hierarchy = moSpec.Hierarchy;
                            result.PoNo = moData.PoNo;
                            result.SquareINCH = moData.SquareInch;
                            result.Stations = stations;
                            result.CutSheetLengInch = moSpec.CutSheetLeng.HasValue ? moSpec.CutSheetLeng.Value : 0;
                            result.CutSheetWidInch = moSpec.CutSheetWid.HasValue ? moSpec.CutSheetWid.Value : 0;
                            result.GlWid = false;
                            result.Piece_Patch = moSpec.PiecePatch.HasValue ? moSpec.PiecePatch.Value : 1;
                            var boardAlt = PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo.Trim() == moData.MaterialNo.Trim()).ToList();
                            result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
                            result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
                            result.CustInvType = moSpec.CustInvType;
                            result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
                            result.MoRout = new List<MasterCardMoRouting>();
                            result.PartOfMoRout = new List<MasterCardMoRouting>();
                            var transDetail = PMTsDbContext.TransactionsDetail.FirstOrDefault(t => t.MaterialNo == moData.MaterialNo && t.FactoryCode == factoryCode && t.PdisStatus != "X");

                            result.CGType = transDetail != null ?
                                transDetail.Cgtype == "B" ? "Base" :
                                transDetail.Cgtype == "L" ? "L Shape" :
                                transDetail.Cgtype == "U" ? "U Shape" : string.Empty
                                : string.Empty;
                            if (transDetail != null)
                            {
                                result.NewPrintPlate = transDetail.NewPrintPlate;
                                result.OldPrintPlate = transDetail.OldPrintPlate;
                                result.NewBlockDieCut = transDetail.NewBlockDieCut;
                                result.OldBlockDieCut = transDetail.OldBlockDieCut;
                                result.ExampleColor = transDetail.ExampleColor;
                                result.CoatingType = transDetail.CoatingType;
                                result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
                                result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
                                result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
                                result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
                                result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
                            }
                            MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings, baseOfMachines, factoryCode);
                            MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings, baseOfMachines, factoryCode);

                            result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
                            result.MoBomRawmats = new List<MoBomRawMat>();
                            var moBomRawmats = PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.FgMaterial.Equals(moData.MaterialNo) && m.OrderItem.Equals(moData.OrderItem)).AsNoTracking().ToList();
                            var ppcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();

                            if (!string.IsNullOrEmpty(moData.MaterialNo))
                            {
                                ppcRawMaterialProductionBoms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(moData.MaterialNo)).ToList();
                            }
                            result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
                            result.MoBomRawmats.AddRange(moBomRawmats);

                            result.MoRout = mainMORoutings;
                            result.PartOfMoRout = partOfMORoutings;

                            result.AllowancePrintNo = moData.AllowancePrintNo.HasValue ? moData.AllowancePrintNo.Value : 0;
                            result.PrintRoundNo = moData.PrintRoundNo.HasValue ? moData.PrintRoundNo.Value : 0;
                            result.AfterPrintNo = moData.AfterPrintNo.HasValue ? moData.AfterPrintNo.Value : 0;
                            result.DrawAmountNo = moData.DrawAmountNo.HasValue ? moData.DrawAmountNo.Value : 0;

                            //get attach file from sale order
                            result.AttchFilesBase64 = string.Empty;
                            var attachfiles = PMTsDbContext.AttachFileMo.Where(a => a.Status == true && a.FactoryCode == factoryCode && a.OrderItem == OrderItem.Trim()).ToList();

                            if (attachfiles.Count > 0)
                            {
                                result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
                            }

                            #endregion Set Mastercard
                        }
                        else
                        {
                            #region Set Coating And Routing

                            hierarchyLv2 = moSpec.Hierarchy.Substring(2, 2);
                            var productTypeByHieLv2 = !string.IsNullOrEmpty(hierarchyLv2) ? PMTsDbContext.ProductType.FirstOrDefault(p => p.HierarchyLv2 == hierarchyLv2) : null;
                            formGroup = productTypeByHieLv2 == null ? "RSC" : productTypeByHieLv2.FormGroup;
                            moRouting = moRoutings.FirstOrDefault();
                            var hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
                            var coatings = new List<Coating>();
                            coatings = baseOfCoatings.Where(w => w.MaterialNo.Equals(moData.MaterialNo) && w.FactoryCode == factoryCode).ToList();

                            foreach (var coating in coatings)
                            {
                                var itemCoating = new Coating
                                {
                                    Station = coating.Station,
                                    Type = coating.Type,
                                    Name = coating.Name,
                                    Id = coating.Id
                                };

                                if (!String.IsNullOrEmpty(itemCoating.Name) && !String.IsNullOrWhiteSpace(itemCoating.Name))
                                {
                                    coatings.Add(itemCoating);
                                }
                            }
                            coatings.OrderBy(c => c.Id).ToList();

                            foreach (var routing in moRoutings)
                            {
                                countRoutings++;
                                var machineName = "";
                                var machineGroup = "";
                                var machine = baseOfMachines.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
                                machineGroup = routing.Machine != null && machine != null ? machine.MachineGroup : null;

                                if (machine.ShowProcess)
                                {
                                    if (routing.McMove.Value)
                                    {
                                        machineName = ReMachineName(null, routing);
                                    }
                                    else
                                    {
                                        machineName = routing.Machine + _localizer["Don't move the machine"].Value;
                                    }

                                    routing.Machine = machineName;
                                    if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
                                    {
                                        var scoreType = baseOfScoreTypes.FirstOrDefault(s => s.ScoreTypeId.Equals(routing.ScoreType) && s.FactoryCode == factoryCode);
                                        routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
                                    }
                                    else
                                    {
                                        routing.ScoreType = routing.ScoreType;
                                    }

                                    lineCount += MORoutingLineCount(routing);

                                    if (routing.Machine.Contains("COR"))
                                    {
                                        lineCount = coatings.Count > 0 ? lineCount + coatings.Count + 2 : lineCount;
                                        routing.Coatings = coatings.Count == 0 ? new List<Coating>() : coatings;
                                    }

                                    if (routing.Machine.Contains("COA"))
                                    {
                                        var qualitySpecs = new List<QualitySpec>();
                                        if (moData != null)
                                        {
                                            qualitySpecs = baseOfQualitySpecs.Where(a => a.FactoryCode == factoryCode && a.MaterialNo == moData.MaterialNo).ToList();
                                        }
                                        else
                                        {
                                            qualitySpecs = null;
                                        }

                                        if (qualitySpecs != null)
                                        {
                                            lineCount = lineCount + 1 + qualitySpecs.Count / 3;
                                            routing.QualitySpecs = QualitySpecsFromModel(qualitySpecs);
                                        }
                                    }

                                    routing.Coatings ??= new List<Coating>();

                                    conditionPPCPrint = lineCount > 42;
                                    if (conditionPPCPrint)
                                    {
                                        if (firstMachine == 0)
                                        {
                                            partOfMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                            });
                                        }
                                        else
                                        {
                                            partOfMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = false
                                            });
                                        }

                                        firstMachine++;
                                    }
                                    else
                                    {
                                        if (firstMachine == 0)
                                        {
                                            mainMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                            });
                                        }
                                        else
                                        {
                                            mainMORoutings.Add(new MasterCardMoRouting
                                            {
                                                MachineGroup = machineGroup,
                                                Routing = routing,
                                                IsProp_Cor = false
                                            });
                                        }
                                        firstMachine++;
                                    }

                                    if (countRoutings == moRoutings.Count)
                                    {
                                        weightOut = routing.WeightOut.Value;
                                    }
                                }
                            }

                            #endregion Set Coating And Routing

                            #region Set Stations

                            //var boardCombine = moSpec != null && moSpec.Code != null ? PMTsDbContext.BoardCombine.FirstOrDefault(b => b.Code == moSpec.Code) : null;
                            //if (boardCombine != null)
                            //{
                            //    if (boardCombine.StandardBoard.Value)
                            //    {
                            //        var boardUse = baseOfBoardUses.FirstOrDefault(b => b.FactoryCode == factoryCode && b.MaterialNo == moData.MaterialNo);
                            //        var boardName = boardUse == null ? "" : boardUse.BoardName;
                            //        // replace
                            //        boardName = clean(boardName);
                            //        var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                            //        var fluteTrs = baseOfFluteTrs.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).ToList();

                            //        stations.Clear();
                            //        var boardNo = 0;
                            //        foreach (var fluteTr in fluteTrs)
                            //        {
                            //            var item = fluteTr.Item.Value;
                            //            var paperGrade = "";
                            //            if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                            //            {
                            //                item = 999;
                            //            }

                            //            if (boardNo >= boardSplit.Count)
                            //            {
                            //                paperGrade = "";
                            //            }
                            //            else
                            //            {
                            //                paperGrade = boardSplit[boardNo];
                            //            }

                            //            stations.Add(new Station
                            //            {
                            //                item = item,
                            //                TypeOfStation = fluteTr.Station,
                            //                PaperGrade = paperGrade,
                            //                Flute = fluteTr.FluteCode
                            //            });
                            //            boardNo++;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        stations.Clear();
                            //        var boardSpecs = PMTsDbContext.BoardSpec.Where(b => b.Code == boardCombine.Code).ToList();
                            //        foreach (var boardSpec in boardSpecs)
                            //        {
                            //            stations.Add(new Station
                            //            {
                            //                item = boardSpec.Item.Value,
                            //                TypeOfStation = boardSpec.Station,
                            //                PaperGrade = boardSpec.Grade,
                            //                Flute = null
                            //            });
                            //        }
                            //    }
                            //}

                            #endregion Set Stations

                            //has machine "COR" in MO routing
                            if (hasCORR)
                            {
                                if (moRouting.CutNo.Value > 0)
                                {
                                    //set จน.ตัด & ยาวเมตร
                                    if (moData.TargetQuant % moRouting.CutNo > 0)
                                    {
                                        Setcut = String.Format("{0:N0}", Convert.ToDouble(moData.TargetQuant / moRouting.CutNo) + 1);
                                    }
                                    else
                                    {
                                        Setcut = String.Format("{0:N0}", Convert.ToDouble(moData.TargetQuant / moRouting.CutNo));
                                    }

                                    int? t = 0;
                                    if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
                                    {
                                        t = moData.OrderQuant * moSpec.CutSheetLeng / 1000;
                                        SetLeng = String.Format("{0:N0}", t);
                                    }
                                    else
                                    {
                                        if ((moSpec.CutSheetLeng * moData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
                                        {
                                            //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
                                            t = (((moSpec.CutSheetLeng * moData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
                                            SetLeng = String.Format("{0:N0}", t);
                                        }
                                        else
                                        {
                                            SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * moData.TargetQuant) / (moRouting.CutNo * 1000)));
                                        }
                                    }
                                }
                            }

                            #region Set masterCard

                            tolerance_Over = moData.ToleranceOver.HasValue && moData.ToleranceOver.Value > 0 ? (moData.OrderQuant * Math.Truncate(moData.ToleranceOver.Value)) / 100 : 0;
                            tolerance_Under = moData.ToleranceUnder.HasValue && moData.ToleranceUnder.Value > 0 ? (moData.OrderQuant * Math.Truncate(moData.ToleranceUnder.Value)) / 100 : 0;
                            tolerance_Over = CellingFloat(tolerance_Over.ToString());
                            tolerance_Under = CellingFloat(tolerance_Under.ToString());
                            result.ProductType = moSpec.ProType;
                            result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
                            result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
                            result.FactoryCode = factoryCode;
                            result.Factory = myFactory;
                            result.DocName = ISO_DocName;
                            result.DocDate = ISO_DocDate;
                            result.OrderItem = moData.OrderItem;
                            result.Material_No = moData.MaterialNo;
                            result.Part_No = moSpec.PartNo;
                            result.PC = moSpec.Pc;
                            result.Cust_Name = moData.Name.Length > 40 ? string.Concat(moData.Name[..40], $" ({moData.SoldTo})") : moData.Name + $" ({moData.SoldTo})";
                            result.CustNameNOSoldto = moData.Name.Length > 40 ? moData.Name[..40] : moData.Name;
                            //result.CustomerContact = $"{_firstName} {_telephone}";
                            result.Description = moSpec.Description;
                            result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
                            result.EanCode = moSpec.EanCode;
                            result.Box_Type = moSpec.BoxType;
                            result.RSC_Style = moSpec.RscStyle;
                            result.JoinType = moSpec.JoinType;
                            result.Print_Method = moSpec.PrintMethod;
                            result.PalletSize = moSpec.PalletSize;
                            result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
                            result.BunLayer = moSpec.BunLayer == null ? 0 : moSpec.BunLayer;
                            result.Material_Type = moSpec.MaterialType;
                            result.Status_Flag = moSpec.StatusFlag;
                            result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
                            result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
                            result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
                            result.Wire = moSpec.Wire;
                            result.Wid = moSpec.Wid;
                            result.Leg = moSpec.Leg;
                            result.Hig = moSpec.Hig;
                            result.CutSheetWid = moSpec.CutSheetWid;
                            result.CutSheetLeng = moSpec.CutSheetLeng;
                            result.CutSheetLengInch = moSpec.CutSheetLengInch.HasValue ? (moSpec.CutSheetLengInch.Value) : 0;
                            result.CutSheetWidInch = moSpec.CutSheetWidInch.HasValue ? (moSpec.CutSheetWidInch.Value) : 0;
                            result.Flute = moSpec.Flute;
                            result.Batch = moData.Batch;
                            result.ItemNote = moData.ItemNote;
                            result.Due_Text = moData.DueText;
                            result.Tolerance_Over = tolerance_Over;
                            result.Tolerance_Under = tolerance_Under;
                            result.Order_Quant = moData.OrderQuant != 0 ? moData.OrderQuant : 0;
                            result.ScoreW1 = moSpec.ScoreW1;
                            result.Scorew2 = moSpec.Scorew2;
                            result.Scorew3 = moSpec.Scorew3;
                            result.Scorew4 = moSpec.Scorew4;
                            result.Scorew5 = moSpec.Scorew5;
                            result.Scorew6 = moSpec.Scorew6;
                            result.Scorew7 = moSpec.Scorew7;
                            result.Scorew8 = moSpec.Scorew8;
                            result.Scorew9 = moSpec.Scorew9;
                            result.Scorew10 = moSpec.Scorew10;
                            result.Scorew11 = moSpec.Scorew11;
                            result.Scorew12 = moSpec.Scorew12;
                            result.Scorew13 = moSpec.Scorew13;
                            result.Scorew14 = moSpec.Scorew14;
                            result.Scorew15 = moSpec.Scorew15;
                            result.Scorew16 = moSpec.Scorew16;
                            result.ScoreL2 = moSpec.ScoreL2;
                            result.ScoreL3 = moSpec.ScoreL3;
                            result.ScoreL4 = moSpec.ScoreL4;
                            result.ScoreL5 = moSpec.ScoreL5;
                            result.ScoreL6 = moSpec.ScoreL6;
                            result.ScoreL7 = moSpec.ScoreL7;
                            result.ScoreL8 = moSpec.ScoreL8;
                            result.ScoreL9 = moSpec.ScoreL9;
                            result.Stations = stations;
                            result.Palletization_Path = moSpec.PalletizationPath;
                            result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) || string.IsNullOrWhiteSpace(moSpec.PalletizationPath) ? "" : _ConvertPictureToBase64(moSpec.PalletizationPath);
                            result.DiecutPict_Path = moSpec.DiecutPictPath;
                            result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) || string.IsNullOrWhiteSpace(moSpec.DiecutPictPath) ? "" : _ConvertPictureToBase64(moSpec.DiecutPictPath);
                            result.Change = moSpec.Change;

                            result.MoRout = new List<MasterCardMoRouting>();
                            result.PartOfMoRout = new List<MasterCardMoRouting>();
                            result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
                            result.MoBomRawmats = new List<MoBomRawMat>();
                            var moBomRawmats = PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.FgMaterial.Equals(moData.MaterialNo) && m.OrderItem.Equals(moData.OrderItem)).AsNoTracking().ToList();
                            var ppcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();

                            if (!string.IsNullOrEmpty(moData.MaterialNo))
                            {
                                ppcRawMaterialProductionBoms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(moData.MaterialNo)).ToList();
                            }
                            result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
                            result.MoBomRawmats.AddRange(moBomRawmats);

                            MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings, baseOfMachines, factoryCode);
                            MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings, baseOfMachines, factoryCode);

                            result.MoRout = mainMORoutings;
                            result.PartOfMoRout = partOfMORoutings;
                            result.Target_Quant = Convert.ToString(moData.TargetQuant) == null ? "0" : Convert.ToString(moData.TargetQuant);
                            result.CutNo = Setcut != null ? Setcut : "0";
                            result.Leng = SetLeng;
                            result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
                            result.Slit = moSpec.Slit;
                            result.Piece_Patch = moSpec != null ? moSpec.PiecePatch : null;
                            result.StockQty = moData.StockQty;
                            var transDetail = PMTsDbContext.TransactionsDetail.FirstOrDefault(t => t.MaterialNo == moData.MaterialNo && t.FactoryCode == factoryCode && t.PdisStatus != "X");
                            if (transDetail != null)
                            {
                                result.NewPrintPlate = transDetail.NewPrintPlate;
                                result.OldPrintPlate = transDetail.OldPrintPlate;
                                result.NewBlockDieCut = transDetail.NewBlockDieCut;
                                result.OldBlockDieCut = transDetail.OldBlockDieCut;
                                result.ExampleColor = transDetail.ExampleColor;
                                result.CoatingType = transDetail.CoatingType;
                                result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
                                result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
                                result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
                                result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
                                result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
                            }
                            result.GlWid = transDetail == null || transDetail.Glwid == null ? false : transDetail.Glwid.Value;
                            result.Distinct = string.IsNullOrEmpty(moData.District) ? moData.District : moData.District.Trim();
                            var boardAlt = PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo.Trim() == moData.MaterialNo.Trim()).ToList();
                            result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
                            result.FormGroup = formGroup.ToString().Trim();
                            result.High_Value = moSpec != null ? moSpec.HighValue : string.Empty;
                            result.Hierarchy = moSpec != null ? moSpec.Hierarchy : string.Empty;
                            result.Printed = moData.Printed == null ? 0 : moData.Printed.Value;
                            result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
                            result.IsXStatus = moData.MoStatus.ToLower().Trim().Contains('x') == true ? true : false;
                            result.NoSlot = formGroup.ToString().Trim() == "AC" ? moSpec.NoSlot.Value : 0;
                            result.PoNo = moData.PoNo;
                            result.SquareINCH = moData.SquareInch;
                            result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
                            result.CustInvType = moSpec.CustInvType;
                            result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
                            //get attach file from sale order
                            result.AttchFilesBase64 = string.Empty;
                            var attachfiles = PMTsDbContext.AttachFileMo.Where(a => a.Status == true && a.FactoryCode == factoryCode && a.OrderItem == OrderItem.Trim()).ToList();
                            result.NoTagBundle = moSpec.NoTagBundle;
                            result.TagBundle = moSpec.TagBundle;
                            result.TagPallet = moSpec.TagPallet;
                            result.CGType = transDetail != null ?
                                transDetail.Cgtype == "B" ? "Base" :
                                transDetail.Cgtype == "L" ? "L Shape" :
                                transDetail.Cgtype == "U" ? "U Shape" : string.Empty
                                : string.Empty;
                            if (attachfiles.Count > 0)
                            {
                                result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
                            }

                            result.CustCode = moSpec.CustCode;
                            result.AllowancePrintNo = moData.AllowancePrintNo.HasValue ? moData.AllowancePrintNo.Value : 0;
                            result.PrintRoundNo = moData.PrintRoundNo.HasValue ? moData.PrintRoundNo.Value : 0;
                            result.AfterPrintNo = moData.AfterPrintNo.HasValue ? moData.AfterPrintNo.Value : 0;
                            result.DrawAmountNo = moData.DrawAmountNo.HasValue ? moData.DrawAmountNo.Value : 0;

                            #endregion Set masterCard
                        }

                        //result.IsPreview = isPreview;
                        result.CustCode = moSpec.CustCode;
                        mastercardModel.MasterCardMOs.Add(result);
                    }

                    if (mastercardModel.MasterCardMOs != null && mastercardModel.MasterCardMOs.Count > 0)
                    {
                        mastercardModel.MasterCardMOs = mastercardModel.MasterCardMOs.OrderBy(m => saleOrders.FindIndex(s => s == m.OrderItem)).ToList();
                    }

                    #endregion Thai
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return mastercardModel;
        }

        #region PDF Oversea

        //public PrintMasterCardMOModel GetDataForMasterCardOverSea(PrintMastercardMO printMastercardMO, string factoryCode, IMapper mapper, IStringLocalizer _localizer)
        //{
        //    var mastercardModel = new PrintMasterCardMOModel();
        //    mastercardModel.MasterCardMOs = new List<MasterCardMO>();
        //    try
        //    {
        //        if (printMastercardMO.MoDatas != null && printMastercardMO.MoDatas.Count > 0)
        //        {
        //            #region Oversea
        //            List<string> saleOrders = printMastercardMO.MoDatas.Select(m => m.OrderItem).ToList();

        //            foreach (var Orderitem in saleOrders)
        //            {
        //                //MO_DATA
        //                var masterMOData = new MoDataPrintMastercard();
        //                var modataForUpdate = printMastercardMO.MoDatas.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);
        //                masterMOData = mapper.Map<MoData, MoDataPrintMastercard>(modataForUpdate != null ? modataForUpdate : PMTsDbContext.MoData.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem));

        //                var printed = 1;
        //                masterMOData.Printed = printed;
        //                var logPrintMO = new LogPrintMo
        //                {
        //                    Id = 0,
        //                    FactoryCode = factoryCode,
        //                    OrderItem = masterMOData.OrderItem,
        //                    Printed = printed,
        //                    PrintedBy = "PrintFromAPI",
        //                    PrintedDate = DateTime.Now,
        //                };

        //                PMTsDbContext.LogPrintMo.Add(logPrintMO);
        //                PMTsDbContext.SaveChanges();

        //                //MasterData
        //                MasterData master = new MasterData();
        //                master = PMTsDbContext.MasterData.FirstOrDefault(m => m.MaterialNo.ToUpper().Trim() == masterMOData.MaterialNo.ToUpper().Trim() && m.PdisStatus != "X");

        //                //MO_SPEC
        //                var moSpec = new MoSpec();
        //                var moSpecForUpdate = printMastercardMO.MoSpecs.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);
        //                moSpec = moSpecForUpdate != null ? moSpecForUpdate : PMTsDbContext.MoSpec.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);

        //                var isSheetBoard = moSpec.MaterialNo.ToLower().Contains("s/b") || moSpec.Hierarchy.Substring(2, 2).ToLower().Contains("sb") || master == null;
        //                //MO_ROUTING >> First
        //                MoRouting moRouting = new MoRouting();
        //                var moRoutings = new List<MoRoutingPrintMastercard>();
        //                var moRoutingForUpdate = new List<MoRouting>();
        //                moRoutingForUpdate = printMastercardMO.MoRoutings.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.SeqNo).ToList();
        //                var moBindRoutings = moRoutingForUpdate != null ? moRoutingForUpdate : PMTsDbContext.MoRouting.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.SeqNo).ToList();
        //                foreach (var item in moBindRoutings)
        //                {
        //                    moRoutings.Add(mapper.Map<MoRoutingPrintMastercard>(item));
        //                }
        //                var mainMORoutings = new List<MasterCardMoRouting>();
        //                var partOfMORoutings = new List<MasterCardMoRouting>();
        //                var hierarchyLv2 = string.Empty;
        //                var formGroup = string.Empty;
        //                string myFactory = string.Empty, ISO_DocDate = string.Empty, ISO_DocName = string.Empty;
        //                double tolerance_Over = 0, tolerance_Under = 0;
        //                var tempStations = new List<Station>();
        //                var stations = new List<Station>();
        //                string Setcut = "", SetLeng = "";
        //                MasterCardMO result = new MasterCardMO();
        //                var firstMachine = 0;
        //                var countRoutings = 0;
        //                var lineCount = 0;
        //                double? weightOut = null;
        //                var conditionPPCPrint = false;

        //                #region Find myFactory PmtsConfig
        //                List<PmtsConfig> pmts = new List<PmtsConfig>();
        //                pmts = PMTsDbContext.PmtsConfig.Where(p => p.FactoryCode.Trim() == factoryCode).ToList();
        //                foreach (var x in pmts)
        //                {
        //                    switch (x.FucName)
        //                    {
        //                        case "Company":
        //                            myFactory = x.FucValue;
        //                            break;
        //                        case "ISO_DocDate":
        //                            ISO_DocDate = x.FucValue;
        //                            break;
        //                        case "ISO_DocName":
        //                            ISO_DocName = x.FucValue;
        //                            break;

        //                    }
        //                }
        //                #endregion

        //                if (isSheetBoard)
        //                {
        //                    #region Set MO Routing
        //                    if (moRoutings.Count > 0)
        //                    {
        //                        var hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
        //                        //has machine "COR" in MO routing
        //                        if (hasCORR)
        //                        {
        //                            moRouting = moRoutings.FirstOrDefault(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor"));
        //                            if (moRouting.CutNo != null && moRouting.CutNo.Value > 0)
        //                            {
        //                                //set จน.ตัด & ยาวเมตร
        //                                if (masterMOData.TargetQuant % moRouting.CutNo > 0)
        //                                {
        //                                    Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo) + 1);
        //                                }
        //                                else
        //                                {
        //                                    Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo));
        //                                }

        //                                int? t = 0;
        //                                if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
        //                                {
        //                                    t = masterMOData.OrderQuant * moSpec.CutSheetLeng / 1000;
        //                                    SetLeng = String.Format("{0:N0}", t);
        //                                }
        //                                else
        //                                {
        //                                    if ((moSpec.CutSheetLeng * masterMOData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
        //                                    {
        //                                        //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
        //                                        t = (((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
        //                                        SetLeng = String.Format("{0:N0}", t);
        //                                    }
        //                                    else
        //                                    {
        //                                        SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)));
        //                                    }

        //                                }
        //                            }
        //                        }
        //                    }

        //                    foreach (var routing in moRoutings)
        //                    {
        //                        countRoutings++;
        //                        var machineName = "";
        //                        var machineGroup = "";
        //                        bool showProcess = true;
        //                        var machine = PMTsDbContext.Machine.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
        //                        //var machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, routing.PlanCode));
        //                        machineGroup = machine != null ? machine.MachineGroup : null;
        //                        showProcess = machine != null ? machine.ShowProcess.Value : true;
        //                        if (showProcess)
        //                        {
        //                            if (routing.McMove.Value)
        //                            {
        //                                machineName = ReMachineName(null, routing);
        //                            }
        //                            else
        //                            {
        //                                machineName = routing.Machine + _localizer["Don't move the machine"].Value;
        //                            }

        //                            routing.Machine = machineName;
        //                            if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
        //                            {
        //                                var scoreType = new ScoreType();
        //                                scoreType = PMTsDbContext.ScoreType.FirstOrDefault(m => m.ScoreTypeId == routing.ScoreType && m.FactoryCode == factoryCode);

        //                                routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
        //                            }
        //                            else
        //                            {
        //                                routing.ScoreType = routing.ScoreType;
        //                            }
        //                            routing.Coatings = new List<Coating>();
        //                            lineCount = lineCount + MORoutingLineCount(routing);
        //                            conditionPPCPrint = lineCount > 42;

        //                            if (conditionPPCPrint)
        //                            {
        //                                if (firstMachine == 0)
        //                                {
        //                                    partOfMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
        //                                    });
        //                                }
        //                                else
        //                                {
        //                                    partOfMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = false
        //                                    });
        //                                }

        //                                firstMachine++;
        //                            }
        //                            else
        //                            {
        //                                if (firstMachine == 0)
        //                                {
        //                                    mainMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = machine == null && routing.Machine.Contains("COR") ? true : machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
        //                                    });
        //                                }
        //                                else
        //                                {
        //                                    mainMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = false
        //                                    });
        //                                }
        //                                firstMachine++;
        //                            }
        //                        }

        //                        if (countRoutings == moRoutings.Count)
        //                        {
        //                            weightOut = routing.WeightOut.Value;
        //                        }
        //                    }
        //                    #endregion

        //                    #region Set Stations
        //                    var boardName = moSpec == null ? "" : moSpec.Board;
        //                    // replace
        //                    boardName = clean(boardName);
        //                    var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

        //                    var fluteTrs = PMTsDbContext.FluteTr.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).ToList();

        //                    stations.Clear();
        //                    var boardNo = 0;
        //                    foreach (var fluteTr in fluteTrs)
        //                    {
        //                        var item = fluteTr.Item.Value;
        //                        var paperGrade = "";
        //                        if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
        //                        {
        //                            item = 999;
        //                        }

        //                        if (boardNo >= boardSplit.Count)
        //                        {
        //                            paperGrade = "";
        //                        }
        //                        else
        //                        {
        //                            paperGrade = boardSplit[boardNo];
        //                        }

        //                        stations.Add(new Station
        //                        {
        //                            item = item,
        //                            TypeOfStation = fluteTr.Station,
        //                            PaperGrade = paperGrade,
        //                            Flute = fluteTr.FluteCode
        //                        });
        //                        boardNo++;
        //                    }
        //                    #endregion

        //                    #region Set Mastercard
        //                    var ymd = masterMOData.DateTimeStamp;
        //                    result.ProductType = moSpec.ProType;
        //                    result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
        //                    result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
        //                    result.FactoryCode = factoryCode;
        //                    result.Factory = myFactory;
        //                    result.DocName = ISO_DocName;
        //                    result.DocDate = ISO_DocDate;
        //                    result.OrderItem = masterMOData.OrderItem;
        //                    result.Material_No = masterMOData.MaterialNo;
        //                    result.Part_No = moSpec.PartNo;
        //                    result.PC = moSpec.Pc;
        //                    result.Cust_Name = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) + $" ({masterMOData.SoldTo})" : masterMOData.Name + $" ({masterMOData.SoldTo})";
        //                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
        //                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
        //                    result.CustomerContact = masterMOData.CreatedBy;
        //                    result.Description = moSpec.Description;
        //                    result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
        //                    result.EanCode = moSpec.EanCode;
        //                    result.Box_Type = moSpec.BoxType;
        //                    result.RSC_Style = moSpec.RscStyle;
        //                    result.JoinType = moSpec.JoinType;
        //                    result.Print_Method = moSpec.PrintMethod;
        //                    result.PalletSize = moSpec.PalletSize;
        //                    result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
        //                    result.BunLayer = moSpec.BunLayer == null ? null : moSpec.BunLayer;
        //                    result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
        //                    result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
        //                    result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
        //                    result.Material_Type = moSpec.MaterialType;
        //                    result.Status_Flag = moSpec.StatusFlag;
        //                    result.Wire = moSpec.Wire;
        //                    result.Wid = moSpec.Wid;
        //                    result.Leg = moSpec.Leg;
        //                    result.Hig = moSpec.Hig;
        //                    result.CutSheetWid = moSpec.CutSheetWid;
        //                    result.CutSheetLeng = moSpec.CutSheetLeng;
        //                    result.Flute = moSpec.Flute;
        //                    result.Batch = masterMOData.Batch;
        //                    result.ItemNote = masterMOData.ItemNote;
        //                    result.Due_Text = masterMOData.DueText;
        //                    result.Tolerance_Over = tolerance_Over;
        //                    result.Tolerance_Under = tolerance_Under;
        //                    result.Order_Quant = masterMOData.OrderQuant != 0 ? masterMOData.OrderQuant : 0;
        //                    result.ScoreW1 = moSpec.ScoreW1;
        //                    result.Scorew2 = moSpec.Scorew2;
        //                    result.Scorew3 = moSpec.Scorew3;
        //                    result.Scorew4 = moSpec.Scorew4;
        //                    result.Scorew5 = moSpec.Scorew5;
        //                    result.Scorew6 = moSpec.Scorew6;
        //                    result.Scorew7 = moSpec.Scorew7;
        //                    result.Scorew8 = moSpec.Scorew8;
        //                    result.Scorew9 = moSpec.Scorew9;
        //                    result.Scorew10 = moSpec.Scorew10;
        //                    result.Scorew11 = moSpec.Scorew11;
        //                    result.Scorew12 = moSpec.Scorew12;
        //                    result.Scorew13 = moSpec.Scorew13;
        //                    result.Scorew14 = moSpec.Scorew14;
        //                    result.Scorew15 = moSpec.Scorew15;
        //                    result.Scorew16 = moSpec.Scorew16;
        //                    result.ScoreL2 = moSpec.ScoreL2;
        //                    result.ScoreL3 = moSpec.ScoreL3;
        //                    result.ScoreL4 = moSpec.ScoreL4;
        //                    result.ScoreL5 = moSpec.ScoreL5;
        //                    result.ScoreL6 = moSpec.ScoreL6;
        //                    result.ScoreL7 = moSpec.ScoreL7;
        //                    result.ScoreL8 = moSpec.ScoreL8;
        //                    result.ScoreL9 = moSpec.ScoreL9;
        //                    result.FormGroup = "SB";
        //                    result.Palletization_Path = moSpec.PalletizationPath;
        //                    result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) ? string.Empty : moSpec.PalletizationPath;
        //                    result.DiecutPict_Path = moSpec.DiecutPictPath;
        //                    result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) ? string.Empty : moSpec.DiecutPictPath;
        //                    result.Change = moSpec.Change;
        //                    result.Printed = masterMOData.Printed == null ? 0 : masterMOData.Printed.Value;
        //                    result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
        //                    result.StockQty = masterMOData.StockQty;
        //                    result.Distinct = string.IsNullOrEmpty(masterMOData.District) ? masterMOData.District : masterMOData.District.Trim();
        //                    result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
        //                    result.Slit = ConvertInt16ToShort(moSpec.Slit);
        //                    result.Target_Quant = Convert.ToString(masterMOData.TargetQuant) == null ? "0" : Convert.ToString(masterMOData.TargetQuant);
        //                    result.CutNo = Setcut != null ? Setcut : "0";
        //                    result.Leng = SetLeng;
        //                    result.High_Value = moSpec.HighValue;
        //                    result.Hierarchy = moSpec.Hierarchy;
        //                    result.PoNo = masterMOData.PoNo;
        //                    result.SquareINCH = masterMOData.SquareInch;

        //                    //not sure
        //                    result.Stations = stations;
        //                    result.CutSheetLengInch = moSpec.CutSheetLeng.HasValue ? moSpec.CutSheetLeng.Value : 0;
        //                    result.CutSheetWidInch = moSpec.CutSheetWid.HasValue ? moSpec.CutSheetWid.Value : 0;

        //                    result.GlWid = false;
        //                    result.Piece_Patch = moSpec.PiecePatch.HasValue ? moSpec.PiecePatch.Value : 1;

        //                    var boardAlt = PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo.Trim() == masterMOData.MaterialNo.Trim()).ToList();
        //                    result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
        //                    result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
        //                    result.CustInvType = moSpec.CustInvType;
        //                    result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
        //                    result.MoRout = new List<MasterCardMoRouting>();
        //                    result.PartOfMoRout = new List<MasterCardMoRouting>();
        //                    var transDetail = master != null ? PMTsDbContext.TransactionsDetail.FirstOrDefault(t => t.MaterialNo == masterMOData.MaterialNo && t.FactoryCode == factoryCode && t.PdisStatus != "X") : null;

        //                    result.CGType = transDetail != null ?
        //                        transDetail.Cgtype == "B" ? "Base" :
        //                        transDetail.Cgtype == "L" ? "L Shape" :
        //                        transDetail.Cgtype == "U" ? "U Shape" : string.Empty
        //                        : string.Empty;
        //                    if (transDetail != null)
        //                    {
        //                        result.NewPrintPlate = transDetail.NewPrintPlate;
        //                        result.OldPrintPlate = transDetail.OldPrintPlate;
        //                        result.NewBlockDieCut = transDetail.NewBlockDieCut;
        //                        result.OldBlockDieCut = transDetail.OldBlockDieCut;
        //                        result.ExampleColor = transDetail.ExampleColor;
        //                        result.CoatingType = transDetail.CoatingType;
        //                        result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
        //                        result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
        //                        result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
        //                        result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
        //                        result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
        //                    }
        //                    MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings, factoryCode);
        //                    MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings, factoryCode);

        //                    result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
        //                    result.MoBomRawmats = new List<MoBomRawMat>();
        //                    var moBomRawmats = PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.FgMaterial.Equals(masterMOData.MaterialNo) && m.OrderItem.Equals(masterMOData.OrderItem)).AsNoTracking().ToList();
        //                    var ppcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();

        //                    if (!string.IsNullOrEmpty(masterMOData.MaterialNo))
        //                    {
        //                        ppcRawMaterialProductionBoms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(masterMOData.MaterialNo)).ToList();
        //                    }
        //                    result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
        //                    result.MoBomRawmats.AddRange(moBomRawmats);

        //                    result.MoRout = mainMORoutings;
        //                    result.PartOfMoRout = partOfMORoutings;

        //                    result.AllowancePrintNo = masterMOData.AllowancePrintNo.HasValue ? masterMOData.AllowancePrintNo.Value : 0;
        //                    result.PrintRoundNo = masterMOData.PrintRoundNo.HasValue ? masterMOData.PrintRoundNo.Value : 0;
        //                    result.AfterPrintNo = masterMOData.AfterPrintNo.HasValue ? masterMOData.AfterPrintNo.Value : 0;
        //                    result.DrawAmountNo = masterMOData.DrawAmountNo.HasValue ? masterMOData.DrawAmountNo.Value : 0;

        //                    //get attach file from sale order
        //                    result.AttchFilesBase64 = string.Empty;
        //                    var attachfiles = PMTsDbContext.AttachFileMo.Where(a => a.Status == true && a.FactoryCode == factoryCode && a.OrderItem == Orderitem.Trim()).ToList();

        //                    if (attachfiles.Count > 0)
        //                    {
        //                        result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
        //                    }

        //                    #endregion
        //                }
        //                else
        //                {
        //                    hierarchyLv2 = master != null ? master.Hierarchy.Substring(2, 2) : string.Empty;
        //                    var productTypeByHieLv2 = !string.IsNullOrEmpty(hierarchyLv2) ? PMTsDbContext.ProductType.FirstOrDefault(p => p.HierarchyLv2 == hierarchyLv2) : null;
        //                    formGroup = productTypeByHieLv2 == null ? "RSC" : productTypeByHieLv2.FormGroup;
        //                    moRouting = moRoutings.FirstOrDefault();
        //                    var hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
        //                    var coatings = new List<Coating>();
        //                    coatings = PMTsDbContext.Coating.Where(w => w.MaterialNo.Equals(masterMOData.MaterialNo) && w.FactoryCode == factoryCode).ToList();

        //                    foreach (var coating in coatings)
        //                    {
        //                        var itemCoating = new Coating
        //                        {
        //                            Station = coating.Station,
        //                            Type = coating.Type,
        //                            Name = coating.Name,
        //                            Id = coating.Id

        //                        };

        //                        if (!String.IsNullOrEmpty(itemCoating.Name) && !String.IsNullOrWhiteSpace(itemCoating.Name))
        //                        {
        //                            coatings.Add(itemCoating);
        //                        }
        //                    }
        //                    coatings.OrderBy(c => c.Id).ToList();

        //                    foreach (var routing in moRoutings)
        //                    {
        //                        countRoutings++;
        //                        var machineName = "";
        //                        var machineGroup = "";
        //                        var machine = PMTsDbContext.Machine.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
        //                        machineGroup = routing.Machine != null && machine != null ? machine.MachineGroup : null;

        //                        if (machine.ShowProcess.Value)
        //                        {
        //                            if (routing.McMove.Value)
        //                            {
        //                                machineName = ReMachineName(null, routing);
        //                            }
        //                            else
        //                            {
        //                                machineName = routing.Machine + _localizer["Don't move the machine"].Value;
        //                            }

        //                            routing.Machine = machineName;
        //                            if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
        //                            {
        //                                var scoreType = PMTsDbContext.ScoreType.FirstOrDefault(s => s.ScoreTypeId.Equals(routing.ScoreType) && s.FactoryCode == factoryCode);
        //                                routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
        //                            }
        //                            else
        //                            {
        //                                routing.ScoreType = routing.ScoreType;
        //                            }

        //                            lineCount = lineCount + MORoutingLineCount(routing);

        //                            if (routing.Machine.Contains("COR"))
        //                            {
        //                                lineCount = coatings.Count > 0 ? lineCount + coatings.Count + 2 : lineCount;
        //                                routing.Coatings = coatings.Count() == 0 ? new List<Coating>() : coatings;
        //                            }

        //                            if (routing.Machine.Contains("COA"))
        //                            {
        //                                var qualitySpecs = new List<QualitySpec>();
        //                                if (master != null)
        //                                {
        //                                    qualitySpecs = PMTsDbContext.QualitySpec.Where(a => a.FactoryCode == factoryCode && a.MaterialNo == masterMOData.MaterialNo).ToList();
        //                                }
        //                                else
        //                                {
        //                                    qualitySpecs = null;
        //                                }

        //                                if (qualitySpecs != null)
        //                                {
        //                                    lineCount = lineCount + 1 + qualitySpecs.Count / 3;
        //                                    routing.QualitySpecs = QualitySpecsFromModel(qualitySpecs);
        //                                }
        //                            }
        //                            if (routing.Coatings == null)
        //                            {
        //                                routing.Coatings = new List<Coating>();
        //                            }

        //                            conditionPPCPrint = lineCount > 42;
        //                            if (conditionPPCPrint)
        //                            {
        //                                if (firstMachine == 0)
        //                                {
        //                                    partOfMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
        //                                    });
        //                                }
        //                                else
        //                                {
        //                                    partOfMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = false
        //                                    });
        //                                }

        //                                firstMachine++;
        //                            }
        //                            else
        //                            {
        //                                if (firstMachine == 0)
        //                                {
        //                                    mainMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
        //                                    });
        //                                }
        //                                else
        //                                {
        //                                    mainMORoutings.Add(new MasterCardMoRouting
        //                                    {
        //                                        MachineGroup = machineGroup,
        //                                        Routing = routing,
        //                                        IsProp_Cor = false
        //                                    });
        //                                }
        //                                firstMachine++;
        //                            }

        //                            if (countRoutings == moRoutings.Count)
        //                            {
        //                                weightOut = routing.WeightOut.Value;
        //                            }
        //                        }

        //                    }
        //                    var boardCombine = master != null && master.Code != null ? PMTsDbContext.BoardCombine.FirstOrDefault(b => b.Code == master.Code) : null;

        //                    if (boardCombine != null)
        //                    {
        //                        if (boardCombine.StandardBoard.Value)
        //                        {
        //                            var boardUse = master != null ? PMTsDbContext.BoardUse.FirstOrDefault(b => b.FactoryCode == factoryCode && b.MaterialNo == master.MaterialNo) : null;
        //                            var boardName = boardUse == null ? "" : boardUse.BoardName;
        //                            // replace
        //                            boardName = clean(boardName);
        //                            var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

        //                            var fluteTrs = PMTsDbContext.FluteTr.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).ToList();

        //                            stations.Clear();
        //                            var boardNo = 0;
        //                            foreach (var fluteTr in fluteTrs)
        //                            {
        //                                var item = fluteTr.Item.Value;
        //                                var paperGrade = "";
        //                                if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
        //                                {
        //                                    item = 999;
        //                                }

        //                                if (boardNo >= boardSplit.Count)
        //                                {
        //                                    paperGrade = "";
        //                                }
        //                                else
        //                                {
        //                                    paperGrade = boardSplit[boardNo];
        //                                }

        //                                stations.Add(new Station
        //                                {
        //                                    item = item,
        //                                    TypeOfStation = fluteTr.Station,
        //                                    PaperGrade = paperGrade,
        //                                    Flute = fluteTr.FluteCode
        //                                });
        //                                boardNo++;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            stations.Clear();
        //                            var boardSpecs = PMTsDbContext.BoardSpec.Where(b => b.Code == boardCombine.Code).ToList();
        //                            foreach (var boardSpec in boardSpecs)
        //                            {
        //                                stations.Add(new Station
        //                                {
        //                                    item = boardSpec.Item.Value,
        //                                    TypeOfStation = boardSpec.Station,
        //                                    PaperGrade = boardSpec.Grade,
        //                                    Flute = null
        //                                });
        //                            }
        //                        }
        //                    }

        //                    //has machine "COR" in MO routing
        //                    if (hasCORR)
        //                    {
        //                        if (moRouting.CutNo.Value > 0)
        //                        {
        //                            //set จน.ตัด & ยาวเมตร
        //                            if (masterMOData.TargetQuant % moRouting.CutNo > 0)
        //                            {
        //                                Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo) + 1);
        //                            }
        //                            else
        //                            {
        //                                Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo));
        //                            }

        //                            int? t = 0;
        //                            if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
        //                            {
        //                                t = masterMOData.OrderQuant * moSpec.CutSheetLeng / 1000;
        //                                SetLeng = String.Format("{0:N0}", t);
        //                            }
        //                            else
        //                            {
        //                                if ((moSpec.CutSheetLeng * masterMOData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
        //                                {
        //                                    //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
        //                                    t = (((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
        //                                    SetLeng = String.Format("{0:N0}", t);
        //                                }
        //                                else
        //                                {
        //                                    SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)));
        //                                }
        //                            }
        //                        }
        //                    }

        //                    tolerance_Over = masterMOData.ToleranceOver.HasValue && masterMOData.ToleranceOver.Value > 0 ? (masterMOData.OrderQuant * Math.Truncate(masterMOData.ToleranceOver.Value)) / 100 : 0;
        //                    tolerance_Under = masterMOData.ToleranceUnder.HasValue && masterMOData.ToleranceUnder.Value > 0 ? (masterMOData.OrderQuant * Math.Truncate(masterMOData.ToleranceUnder.Value)) / 100 : 0;

        //                    tolerance_Over = CellingFloat(tolerance_Over.ToString());
        //                    tolerance_Under = CellingFloat(tolerance_Under.ToString());

        //                    #region Set masterCard
        //                    result.ProductType = moSpec.ProType;
        //                    result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
        //                    result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
        //                    result.FactoryCode = factoryCode;
        //                    result.Factory = myFactory;
        //                    result.DocName = ISO_DocName;
        //                    result.DocDate = ISO_DocDate;
        //                    result.OrderItem = masterMOData.OrderItem;
        //                    result.Material_No = masterMOData.MaterialNo;
        //                    result.Part_No = moSpec.PartNo;
        //                    result.PC = moSpec.Pc;
        //                    result.Cust_Name = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) + $" ({masterMOData.SoldTo})" : masterMOData.Name + $" ({masterMOData.SoldTo})";
        //                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
        //                    result.CustomerContact = masterMOData.CreatedBy;
        //                    result.Description = moSpec.Description;
        //                    result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
        //                    result.EanCode = moSpec.EanCode;
        //                    result.Box_Type = moSpec.BoxType;
        //                    result.RSC_Style = moSpec.RscStyle;
        //                    result.JoinType = moSpec.JoinType;
        //                    result.Print_Method = moSpec.PrintMethod;
        //                    result.PalletSize = moSpec.PalletSize;
        //                    result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
        //                    result.BunLayer = moSpec.BunLayer == null ? 0 : moSpec.BunLayer;
        //                    result.Material_Type = moSpec.MaterialType;
        //                    result.Status_Flag = moSpec.StatusFlag;
        //                    result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
        //                    result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
        //                    result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
        //                    result.Wire = moSpec.Wire;
        //                    result.Wid = moSpec.Wid;
        //                    result.Leg = moSpec.Leg;
        //                    result.Hig = moSpec.Hig;
        //                    result.CutSheetWid = moSpec.CutSheetWid;
        //                    result.CutSheetLeng = moSpec.CutSheetLeng;
        //                    result.CutSheetLengInch = moSpec.CutSheetLengInch.HasValue ? (moSpec.CutSheetLengInch.Value) : 0;
        //                    result.CutSheetWidInch = moSpec.CutSheetWidInch.HasValue ? (moSpec.CutSheetWidInch.Value) : 0;
        //                    result.Flute = moSpec.Flute;
        //                    result.Batch = masterMOData.Batch;
        //                    result.ItemNote = masterMOData.ItemNote;
        //                    result.Due_Text = masterMOData.DueText;
        //                    result.Tolerance_Over = tolerance_Over;
        //                    result.Tolerance_Under = tolerance_Under;
        //                    result.Order_Quant = masterMOData.OrderQuant != 0 ? masterMOData.OrderQuant : 0;
        //                    result.ScoreW1 = moSpec.ScoreW1;
        //                    result.Scorew2 = moSpec.Scorew2;
        //                    result.Scorew3 = moSpec.Scorew3;
        //                    result.Scorew4 = moSpec.Scorew4;
        //                    result.Scorew5 = moSpec.Scorew5;
        //                    result.Scorew6 = moSpec.Scorew6;
        //                    result.Scorew7 = moSpec.Scorew7;
        //                    result.Scorew8 = moSpec.Scorew8;
        //                    result.Scorew9 = moSpec.Scorew9;
        //                    result.Scorew10 = moSpec.Scorew10;
        //                    result.Scorew11 = moSpec.Scorew11;
        //                    result.Scorew12 = moSpec.Scorew12;
        //                    result.Scorew13 = moSpec.Scorew13;
        //                    result.Scorew14 = moSpec.Scorew14;
        //                    result.Scorew15 = moSpec.Scorew15;
        //                    result.Scorew16 = moSpec.Scorew16;
        //                    result.ScoreL2 = moSpec.ScoreL2;
        //                    result.ScoreL3 = moSpec.ScoreL3;
        //                    result.ScoreL4 = moSpec.ScoreL4;
        //                    result.ScoreL5 = moSpec.ScoreL5;
        //                    result.ScoreL6 = moSpec.ScoreL6;
        //                    result.ScoreL7 = moSpec.ScoreL7;
        //                    result.ScoreL8 = moSpec.ScoreL8;
        //                    result.ScoreL9 = moSpec.ScoreL9;
        //                    result.Stations = stations;
        //                    result.Palletization_Path = moSpec.PalletizationPath;
        //                    result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) ? string.Empty : moSpec.PalletizationPath;
        //                    result.DiecutPict_Path = moSpec.DiecutPictPath;
        //                    result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) ? string.Empty : moSpec.DiecutPictPath;
        //                    result.Change = moSpec.Change;

        //                    result.MoRout = new List<MasterCardMoRouting>();
        //                    result.PartOfMoRout = new List<MasterCardMoRouting>();
        //                    result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
        //                    result.MoBomRawmats = new List<MoBomRawMat>();
        //                    var moBomRawmats = PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.FgMaterial.Equals(masterMOData.MaterialNo) && m.OrderItem.Equals(masterMOData.OrderItem)).AsNoTracking().ToList();
        //                    var ppcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();

        //                    if (!string.IsNullOrEmpty(masterMOData.MaterialNo))
        //                    {
        //                        ppcRawMaterialProductionBoms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(masterMOData.MaterialNo)).ToList();
        //                    }
        //                    result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
        //                    result.MoBomRawmats.AddRange(moBomRawmats);

        //                    MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings, factoryCode);
        //                    MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings, factoryCode);

        //                    result.MoRout = mainMORoutings;
        //                    result.PartOfMoRout = partOfMORoutings;
        //                    result.Target_Quant = Convert.ToString(masterMOData.TargetQuant) == null ? "0" : Convert.ToString(masterMOData.TargetQuant);
        //                    result.CutNo = Setcut != null ? Setcut : "0";
        //                    result.Leng = SetLeng;
        //                    result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
        //                    result.Slit = moSpec.Slit;
        //                    result.Piece_Patch = master != null ? master.PiecePatch : null;
        //                    result.StockQty = masterMOData.StockQty;
        //                    var transDetail = master != null ? PMTsDbContext.TransactionsDetail.FirstOrDefault(t => t.MaterialNo == masterMOData.MaterialNo && t.FactoryCode == factoryCode && t.PdisStatus != "X") : null;
        //                    if (transDetail != null)
        //                    {
        //                        result.NewPrintPlate = transDetail.NewPrintPlate;
        //                        result.OldPrintPlate = transDetail.OldPrintPlate;
        //                        result.NewBlockDieCut = transDetail.NewBlockDieCut;
        //                        result.OldBlockDieCut = transDetail.OldBlockDieCut;
        //                        result.ExampleColor = transDetail.ExampleColor;
        //                        result.CoatingType = transDetail.CoatingType;
        //                        result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
        //                        result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
        //                        result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
        //                        result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
        //                        result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
        //                    }
        //                    result.GlWid = transDetail == null || transDetail.Glwid == null ? false : transDetail.Glwid.Value;
        //                    result.Distinct = string.IsNullOrEmpty(masterMOData.District) ? masterMOData.District : masterMOData.District.Trim();
        //                    var boardAlt = PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo.Trim() == masterMOData.MaterialNo.Trim()).ToList();
        //                    result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
        //                    result.FormGroup = formGroup.ToString().Trim();
        //                    result.High_Value = master != null ? master.HighValue : string.Empty;
        //                    result.Hierarchy = master != null ? master.Hierarchy : string.Empty;
        //                    result.Printed = masterMOData.Printed == null ? 0 : masterMOData.Printed.Value;
        //                    result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
        //                    result.IsXStatus = masterMOData.MoStatus.ToLower().Trim().Contains('x') == true ? true : false;
        //                    result.NoSlot = formGroup.ToString().Trim() == "AC" && master != null ? master.NoSlot.Value : 0;
        //                    result.PoNo = masterMOData.PoNo;
        //                    result.SquareINCH = masterMOData.SquareInch;
        //                    result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
        //                    result.CustInvType = moSpec.CustInvType;
        //                    result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
        //                    //get attach file from sale order
        //                    result.AttchFilesBase64 = string.Empty;
        //                    var attachfiles = PMTsDbContext.AttachFileMo.Where(a => a.Status == true && a.FactoryCode == factoryCode && a.OrderItem == Orderitem.Trim()).ToList();
        //                    result.NoTagBundle = moSpec.NoTagBundle;
        //                    result.TagBundle = moSpec.TagBundle;
        //                    result.TagPallet = moSpec.TagPallet;
        //                    result.CGType = transDetail != null ?
        //                        transDetail.Cgtype == "B" ? "Base" :
        //                        transDetail.Cgtype == "L" ? "L Shape" :
        //                        transDetail.Cgtype == "U" ? "U Shape" : string.Empty
        //                        : string.Empty;
        //                    if (attachfiles.Count > 0)
        //                    {
        //                        result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
        //                    }

        //                    #endregion
        //                    result.CustCode = moSpec.CustCode;
        //                    result.AllowancePrintNo = masterMOData.AllowancePrintNo.HasValue ? masterMOData.AllowancePrintNo.Value : 0;
        //                    result.PrintRoundNo = masterMOData.PrintRoundNo.HasValue ? masterMOData.PrintRoundNo.Value : 0;
        //                    result.AfterPrintNo = masterMOData.AfterPrintNo.HasValue ? masterMOData.AfterPrintNo.Value : 0;
        //                    result.DrawAmountNo = masterMOData.DrawAmountNo.HasValue ? masterMOData.DrawAmountNo.Value : 0;
        //                }

        //                //result.IsPreview = isPreview;
        //                result.CustCode = moSpec.CustCode;
        //                mastercardModel.MasterCardMOs.Add(result);
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return mastercardModel;
        //}

        #endregion PDF Oversea

        #region function for mastercardMO

        private string ReMachineName(Routing routing, MoRouting moRouting)
        {
            string machineName;
            var partOfMachineName = "";

            List<string> alternatives = [];

            if (routing != null)
            {
                alternatives = new List<string>{
                routing.Alternative1,
                routing.Alternative2,
                routing.Alternative3,
                routing.Alternative4,
                routing.Alternative5,
                routing.Alternative6,
                routing.Alternative7,
                routing.Alternative8};
            }
            else
            {
                alternatives = new List<string>{
                moRouting.Alternative1,
                moRouting.Alternative2,
                moRouting.Alternative3,
                moRouting.Alternative4,
                moRouting.Alternative5,
                moRouting.Alternative6,
                moRouting.Alternative7,
                moRouting.Alternative8};
            }

            foreach (var alternative in alternatives)
            {
                if (!string.IsNullOrEmpty(alternative) && !string.IsNullOrWhiteSpace(alternative))
                {
                    partOfMachineName = partOfMachineName + " /" + alternative;
                }
            }

            machineName = routing != null ? routing.Machine + partOfMachineName : moRouting.Machine + partOfMachineName;

            return machineName;
        }

        private int MORoutingLineCount(MoRouting moRouting)
        {
            int lineCount = 4;

            lineCount = (!String.IsNullOrEmpty(moRouting.JoinMatNo) || !String.IsNullOrEmpty(moRouting.SeparatMatNo)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.ScoreType)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.MylaNo)) ? lineCount + 1 : lineCount;
            lineCount = moRouting.RepeatLength.HasValue && moRouting.RepeatLength.Value != 0 ? lineCount + 1 : lineCount;

            lineCount = (!String.IsNullOrEmpty(moRouting.PlateNo) || !String.IsNullOrEmpty(moRouting.BlockNo) || !String.IsNullOrEmpty(moRouting.MylaNo)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color1) || !String.IsNullOrEmpty(moRouting.Color2) || !String.IsNullOrEmpty(moRouting.Color3)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color4) || !String.IsNullOrEmpty(moRouting.Color5) || !String.IsNullOrEmpty(moRouting.Color6)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color7)) ? lineCount + 1 : lineCount;

            var lineOfRemarkInprocess = 0;
            if (!String.IsNullOrEmpty(moRouting.RemarkInprocess))
            {
                if (RemarkRegex().Matches(moRouting.RemarkInprocess).Count > 0)
                {
                    lineOfRemarkInprocess = RemarkRegex().Matches(moRouting.RemarkInprocess).Count;
                }
                else
                {
                    if (moRouting.RemarkInprocess.Length > 35)
                    {
                        lineOfRemarkInprocess = moRouting.RemarkInprocess.Length / 35 > 0 ?
                            moRouting.RemarkInprocess.Length % 35 > 0 ? (moRouting.RemarkInprocess.Length / 35) + 1 : (moRouting.RemarkInprocess.Length / 35)
                            : 1;
                    }
                }
            }

            lineCount += lineOfRemarkInprocess;

            return lineCount;
        }

        // replace “FSC”, “H01”, “H02”, “H03”
        public static string clean(string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("FSC/", "");
            sb.Replace("H01/", "");
            sb.Replace("H02/", "");
            sb.Replace("H03/", "");
            sb.Replace("FSC,", "");
            sb.Replace("H01,", "");
            sb.Replace("H02,", "");
            sb.Replace("H03,", "");

            return sb.ToString();
        }

        private Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        public void MoRoutingSetDetailOfMachineAndColor(ref List<MasterCardMoRouting> masterCardMoRoutings, List<DataAccess.Models.Machine> machines, string factoryCode)
        {
            var moRoutings = new List<MoRouting>();
            for (int i = 0; i < masterCardMoRoutings.Count; i++)
            {
                var masterCardRouting = masterCardMoRoutings[i];
                var routingMachine = string.Empty;
                var routingColor = string.Empty;
                var machine = machines.FirstOrDefault(m => m.PlanCode.Equals(masterCardRouting.Routing.PlanCode) && factoryCode.Equals(m.FactoryCode));

                //machine
                if (masterCardRouting.Routing.PlateNo != null && masterCardRouting.Routing.PlateNo != "")
                {
                    routingMachine = routingMachine + "Plate No. " + masterCardRouting.Routing.PlateNo + ", ";
                }

                var blockBlk = "";
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.BlockNo))
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print" + ", ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    routingMachine += blockBlk;
                    //routingMachine = routingMachine + "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                }
                else
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print , ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. , ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. , ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. , ";
                    }
                    routingMachine += blockBlk;
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaNo) && machine != null && machine.IsPropPrint.HasValue && machine.IsPropPrint.Value)
                {
                    routingMachine = routingMachine + "Myla No. " + masterCardRouting.Routing.MylaNo + ", ";
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaSize))
                {
                    routingMachine = routingMachine + "Myla Size " + masterCardRouting.Routing.MylaSize + ", ";
                }

                var lengthOfMachine = routingMachine.Length;
                if (lengthOfMachine != 0)
                {
                    routingMachine = routingMachine[..(lengthOfMachine - 2)];
                }

                //color
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color1) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade1))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color1 + " " + masterCardRouting.Routing.Shade1 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color2) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade2))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color2 + " " + masterCardRouting.Routing.Shade2 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color3) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade3))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color3 + " " + masterCardRouting.Routing.Shade3 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color4) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade4))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color4 + " " + masterCardRouting.Routing.Shade4 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color5) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade5))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color5 + " " + masterCardRouting.Routing.Shade5 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color6) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade6))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color6 + " " + masterCardRouting.Routing.Shade6 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color7) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade7))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color7 + " " + masterCardRouting.Routing.Shade7 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color8) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade8))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color8 + " " + masterCardRouting.Routing.Shade8 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color9) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade9))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color9 + " " + masterCardRouting.Routing.Shade9 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color10) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade10))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color10 + " " + masterCardRouting.Routing.Shade10 + ", ";
                }

                var lengthOfColor = routingColor.Length;
                if (lengthOfColor != 0)
                {
                    routingColor = routingColor[..(lengthOfColor - 2)];
                }

                if (!string.IsNullOrEmpty(routingMachine))
                {
                    masterCardRouting.Routing.MachineDetail = routingMachine;
                }

                if (!string.IsNullOrEmpty(routingColor))
                {
                    masterCardRouting.Routing.MachineColorDetail = "Color : " + routingColor;
                }

                masterCardMoRoutings[i].Routing = masterCardRouting.Routing;
            }
        }

        public static string _ConvertPictureToBase64(string path)
        {
            try
            {
                //Bitmap bitmap1 = new Bitmap(path);
                //MemoryStream streamQR = new MemoryStream();
                //bitmap1.Save(streamQR, System.Drawing.Imaging.ImageFormat.Jpeg);
                //var imgStr = "data:image/jpg;base64," + Convert.ToBase64String(streamQR.ToArray());
                //bitmap1.Dispose();
                //return imgStr;

                byte[] imageArray = File.ReadAllBytes(path);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                base64ImageRepresentation = "data:image/jpg;base64," + base64ImageRepresentation;
                return base64ImageRepresentation;
            }
            catch
            {
                return "";
            }
        }

        private string QualitySpecsFromModel(List<QualitySpec> qualitySpecs)
        {
            var qualitySpecStr = string.Empty;
            foreach (var qualitySpec in qualitySpecs)
            {
                qualitySpecStr += $"{qualitySpec.Name} : {qualitySpec.Value} {qualitySpec.Unit}, ";
            }

            qualitySpecStr = !String.IsNullOrEmpty(qualitySpecStr) ? qualitySpecStr[..^2] : null;

            return qualitySpecStr;
        }

        public int CellingFloat(string value)
        {
            double doubleValue = Double.Parse(value);
            var tolerance = (Int32)Math.Ceiling(doubleValue);

            return tolerance;
        }

        [GeneratedRegex(@"\r\n")]
        private static partial Regex RemarkRegex();

        public MODataWithBomRawMatsModel GetMODataWithBomRawMatsByOrderItem(string orderItem)
        {
            var moDataWithBomRawMatsModel = new MODataWithBomRawMatsModel();
            moDataWithBomRawMatsModel.MoData = new MoData();
            moDataWithBomRawMatsModel.MoBomRawMats = new List<MoBomRawMat>();
            moDataWithBomRawMatsModel.MoData = PMTsDbContext.MoData.FirstOrDefault(m => m.OrderItem == orderItem);
            if (moDataWithBomRawMatsModel.MoData != null)
            {
                moDataWithBomRawMatsModel.MoBomRawMats = PMTsDbContext.MoBomRawMat.Where(m => m.OrderItem == orderItem && m.FactoryCode == moDataWithBomRawMatsModel.MoData.FactoryCode).ToList();
            }

            return moDataWithBomRawMatsModel;
        }

        public void UpdateMODatasInterfaceTIPsByOrderItems(string factorycode, bool interface_tips, List<string> orderItems)
        {
            List<MoData> moDatas = [.. PMTsDbContext.MoData.Where(m => m.FactoryCode == factorycode && orderItems.Contains(m.OrderItem))];
            moDatas.ForEach(m =>
            {
                m.InterfaceTips = interface_tips;
                m.UpdatedBy = "TIPsSystem";
                m.UpdatedDate = DateTime.Now;
            });
            PMTsDbContext.MoData.UpdateRange(moDatas);
            PMTsDbContext.SaveChanges();
        }

        public void UpdatePrintedMODataByOrderItems(string factoryCode, string username, List<string> orderItems)
        {
            var logPrintMOs = new List<LogPrintMo>();
            var moDatas = PMTsDbContext.MoData.Where(m => m.FactoryCode == factoryCode && orderItems.Contains(m.OrderItem)).ToList();
            foreach (var moData in moDatas)
            {
                var printed = moData.Printed == null ? 1 : moData.Printed + 1;
                moData.Printed = printed;
                moData.UpdatedBy = username;
                moData.UpdatedDate = DateTime.Now;
                var logPrintMO = new LogPrintMo
                {
                    Id = 0,
                    FactoryCode = factoryCode,
                    OrderItem = moData.OrderItem,
                    Printed = printed,
                    PrintedBy = username,
                    PrintedDate = DateTime.Now,
                };
                logPrintMOs.Add(logPrintMO);
            }

            PMTsDbContext.MoData.UpdateRange(moDatas);
            PMTsDbContext.LogPrintMo.AddRange(logPrintMOs);
            PMTsDbContext.SaveChanges();
        }

        #endregion function for mastercardMO
    }
}