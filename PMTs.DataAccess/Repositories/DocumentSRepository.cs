using Dapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class DocumentSRepository(PMTsDbContext context) : Repository<DocumentS>(context), IDocumentSRepository
    {
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<DocumentS> GetDocumentS(string FactoryCode, string SNumber)
        {
            if (!string.IsNullOrEmpty(SNumber))
            {
                return PMTsDbContext.DocumentS.Where(m => m.Snumber.Contains(SNumber)).ToList();
            }
            else
            {
                return new List<DocumentS>();
            }
        }

        public DocumentS CreateDocumentS(string FactoryCode, string Username)
        {
            string newSnumber = string.Empty;
            var getlast = PMTsDbContext.DocumentS.Where(m => m.FactoryCode == FactoryCode).OrderByDescending(x => x.Id).FirstOrDefault();
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            if (month.Length == 1)
            {
                month = "0" + month;
            }

            var company = PMTsDbContext.CompanyProfile.Where(x => x.Plant == FactoryCode).FirstOrDefault();
            var prefix = company.ShortName.Substring(company.ShortName.Length - 2, 2);
            string checkYearMonth = prefix + year + month;
            //int lastAutoNumber = 1;
            if (getlast != null)
            {
                var split = getlast.Snumber.Split("-");
                if (split[0] == checkYearMonth)
                {
                    var check = (Convert.ToInt16(split[1]) + 1).ToString();
                    if (check.Length == 1)
                    {
                        newSnumber = checkYearMonth + "-000" + check;
                    }
                    else if (check.Length == 2)
                    {
                        newSnumber = checkYearMonth + "-00" + check;
                    }
                    else if (check.Length == 3)
                    {
                        newSnumber = checkYearMonth + "-0" + check;
                    }
                    else if (check.Length == 4)
                    {
                        newSnumber = checkYearMonth + "-" + check;
                    }
                }
                else
                {
                    newSnumber = checkYearMonth + "-0001";
                }
            }
            else
            {
                newSnumber = checkYearMonth + "-0001";
            }

            DocumentS modelCreate = new DocumentS();
            modelCreate.FactoryCode = FactoryCode;
            modelCreate.Snumber = newSnumber;
            modelCreate.CreatedBy = Username;
            modelCreate.CreatedDate = DateTime.Now;
            PMTsDbContext.DocumentS.Add(modelCreate);
            PMTsDbContext.SaveChanges();
            return modelCreate;
        }

        public IEnumerable<DocumentSlist> GetDocumentSList(string FactoryCode, string OrderItem)
        {
            if (!string.IsNullOrEmpty(OrderItem))
            {
                //return PMTsDbContext.DocumentSlist.Where(m => m.FactoryCode == FactoryCode && m.Snumber == OrderItem).ToList();
                return PMTsDbContext.DocumentSlist.Where(m => m.Snumber == OrderItem).ToList();
            }
            else
            {
                return new List<DocumentSlist>();
            }
        }

        public DocumentSData GetDataByMo(string FactoryCode, string OrderItem, IConfiguration _configuration)
        {
            if (!string.IsNullOrEmpty(OrderItem))
            {
                DocumentSData DocumentSData = new DocumentSData();
                using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PMTsConnect"));
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sql = @"
                     select d.OrderItem as OrderItem
                        ,d.Name as CustomerName
                        ,d.PO_No as PO
                        ,d.Material_No as MateriailNo
                        ,s.PC as PC
                        ,s.Sale_Text1 as SaleText1
                        ,s.Flute as  Flute
                        ,d.Order_Quant as OrderQty
                        ,convert(varchar, d.Due_Date, 103) as DueDate
                        ,s.Box_Type as BoxType
                        ,STUFF((SELECT ',' + cast(US.[Machine] as nvarchar(10))
                                            FROM [MO_Routing] US
                                            WHERE US.FactoryCode = d.FactoryCode and US.[OrderItem] = d.OrderItem
                                            FOR XML PATH('')), 1, 1, '')  Process
                        from MO_DATA d 
                        inner join MO_Spec s 
                        on d.OrderItem = s.OrderItem
                        -- where d.FactoryCode = '{0}' and  d.OrderItem = '{1}'
                        where   d.OrderItem = '{2}'
                        ";

                //string message = string.Format(sql, FactoryCode, OrderItem);
                string message = string.Format(sql, OrderItem, OrderItem, OrderItem);

                //var tmp = PMTsDbContext.MoRouting.Where(x => x.FactoryCode == FactoryCode && x.OrderItem == OrderItem).ToList();
                var tmp = PMTsDbContext.MoRouting.Where(x => x.OrderItem == OrderItem).ToList();

                string Non_Print = string.Empty;
                string Standard = string.Empty;
                string Semi = string.Empty;
                string Shipping_Mark = string.Empty;

                foreach (var item in tmp)
                {
                    var checkMatchineGroup2 = PMTsDbContext.Machine.Where(x => x.Code == item.MatCode).FirstOrDefault();

                    if (checkMatchineGroup2 == null)
                    {

                    }
                    else if (checkMatchineGroup2.MachineGroup == "2")
                    {

                        if (item.NoneBlk == true)
                        {
                            Non_Print = ",Non-Print";
                        }
                        if (item.SemiBlk == true)
                        {
                            Non_Print = ",Semi";
                        }
                        if (item.StanBlk == true)
                        {
                            Non_Print = ",Standard";
                        }
                        if (item.ShipBlk == true)
                        {
                            Shipping_Mark = ",Shipping-Mark";
                        }
                    }
                }
                string boxCheck = Non_Print + Standard + Semi + Shipping_Mark;
                if (boxCheck.Trim() != "")
                {
                    boxCheck = boxCheck.Substring(1, boxCheck.Length - 1);
                }

                var boxtype = PMTsDbContext.MoSpec.Where(x => x.OrderItem == OrderItem).Select(x => x.MaterialType).FirstOrDefault();
                DocumentSData datamodel = new DocumentSData();
                datamodel = db.Query<DocumentSData>(message).FirstOrDefault();
                datamodel.BoxType = boxCheck;
                datamodel.MatType = boxtype;
                return datamodel;
            }
            else
            {
                return new DocumentSData();
            }
        }

        public void SaveDocumentS(string FactoryCode, CreateDocumentSModel model, IConfiguration _configuration)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PMTsConnect"));

            var boxtype = PMTsDbContext.MoSpec.Where(x => x.FactoryCode == FactoryCode && x.OrderItem == model.ManageData.OrderItem).Select(x => x.MaterialType).FirstOrDefault();
            if (boxtype == "82")
            {
                model.ManageData.BoxStatus = "";
                model.ManageData.PartStatus = model.ManageData.PartStatus;
            }
            else
            {
                model.ManageData.BoxStatus = model.ManageData.PartStatus;
                model.ManageData.PartStatus = "";
            }


            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                INSERT INTO [dbo].[DocumentSList]
                    ([FactoryCode]
                    ,[SNumber]
                    ,[OrderItem]
                    ,[PC]
                    ,[MaterialNo]
                    ,[Flute]
                    ,[DuedateOld]
                    ,[DuedateNew]
                    ,[OrderQtyOld]
                    ,[OrderQtyNew]
                    ,[Cancel]
                    ,[Hold]
                    ,[BoxStatus]
                    ,[PartStatus]
                    ,[Process]
                    ,[Remark]
                    ,[CreatedDate]
                    ,[CreatedBy]
                    ,[CustomerName]
                    )
                    VALUES
                    ('{0}'
                    ,'{1}'
                    ,'{2}'
                    ,'{3}'
                    ,'{4}'
                    ,'{5}'
                    ,'{6}'
                    ,'{7}'
                    ,'{8}'
                    ,{9}
                    ,{10}
                    ,{11}
                    ,'{12}'
                    ,'{13}'
                    ,'{14}'
                    ,'{15}'
                    ,GETDATE()
                    ,'{16}'
                    ,'{17}'
                    )
                ";

            string message = string.Format(sql,
                FactoryCode
                , model.ManageData.Snumber
                , model.ManageData.OrderItem
                , model.ManageData.Pc
                , model.ManageData.MaterialNo
                , model.ManageData.Flute
                , model.ManageData.DuedateOld
                , model.ManageData.DuedateNew
                , model.ManageData.OrderQtyOld
                , model.ManageData.OrderQtyNew == null ? 0 : model.ManageData.OrderQtyNew
                , model.ManageData.Cancel == true ? 1 : 0
                , model.ManageData.Hold == true ? 1 : 0
                , model.ManageData.BoxStatus
                , model.ManageData.PartStatus
                , model.ManageData.Process
                , model.ManageData.Remark
                , model.ManageData.Username
                , model.ManageData.Customer
                );
            db.Execute(message);

            string sqldocmain = @"
                    UPDATE [dbo].[DocumentS]
                       SET 
                          [UpdatedDate] = GETDATE()
                          ,[UpdatedBy] =  '{0}'
                     WHERE [FactoryCode] = '{1}' and [SNumber] = '{2}'
                    ";

            string messageDocMain = string.Format(sqldocmain,
                model.ManageData.Username
                   , FactoryCode
                   , model.ManageData.Snumber
               );
            db.Execute(messageDocMain);
        }

        public void UpdateDocumentS(string FactoryCode, CreateDocumentSModel model, IConfiguration _configuration)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PMTsConnect"));
            var boxtype = PMTsDbContext.MoSpec.Where(x => x.FactoryCode == FactoryCode && x.OrderItem == model.ManageData.OrderItem).Select(x => x.MaterialType).FirstOrDefault();
            if (boxtype == "82")
            {
                model.ManageData.BoxStatus = "";
                model.ManageData.PartStatus = model.ManageData.PartStatus;
            }
            else
            {
                model.ManageData.BoxStatus = model.ManageData.PartStatus;
                model.ManageData.PartStatus = "";
            }

            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                UPDATE [dbo].[DocumentSList]
                    SET
                        [DuedateNew] = '{0}'
                        ,[OrderQtyNew] = {1}
                        ,[Cancel] = {2}
                        ,[Hold] = {3}
                        ,[BoxStatus] ='{4}'
                        ,[PartStatus] = '{5}'  
                        ,[Remark] ='{6}'
                        ,[UpdatedDate] = GETDATE()
                        ,[UpdatedBy] = '{7}'
                     WHERE [FactoryCode] = '{8}'  and [SNumber] = '{9}' and [OrderItem] = '{10}'
                ";

            string message = string.Format(sql
                    , model.ManageData.DuedateNew
                    , model.ManageData.OrderQtyNew == null ? 0 : model.ManageData.OrderQtyNew
                    , model.ManageData.Cancel == true ? 1 : 0
                    , model.ManageData.Hold == true ? 1 : 0
                     , model.ManageData.BoxStatus
                    , model.ManageData.PartStatus
                    , model.ManageData.Remark
                    , model.ManageData.Username
                    , FactoryCode
                    , model.ManageData.Snumber
                    , model.ManageData.OrderItem
                );
            db.Execute(message);

            string sqldocmain = @"
                    UPDATE [dbo].[DocumentS]
                       SET 
                        [UpdatedDate] = GETDATE()
                        ,[UpdatedBy] =  '{0}'
                     WHERE [FactoryCode] = '{1}' and [SNumber] = '{2}'
                    ";

            string messageDocMain = string.Format(sqldocmain,
                model.ManageData.Username
                   , FactoryCode
                   , model.ManageData.Snumber
               );
            db.Execute(messageDocMain);
        }

        public void DeleteDocumentS(string Id, IConfiguration _configuration)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                DELETE FROM [dbo].[DocumentSList]                   
                     WHERE [Id] = '{0}' 
                ";
            string message = string.Format(sql
                , Id
                );
            db.Execute(message);
        }

        public ReportDocumentS ReportDocumentS(string factorycode, string snumber, string user, IConfiguration _configuration)
        {
            ReportDocumentS model = new ReportDocumentS();
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string _SDocName = @" SELECT[Fuc_Value]
                      FROM [dbo].[PMTsConfig]
                      where FactoryCode = '{0}' and Fuc_Name in ('SDocName')";
                string message_SDocName = string.Format(_SDocName
                  , factorycode
                  );
                var _SDocNameTmp = db.Query<string>(message_SDocName).First();

                string _SDocDate = @" SELECT[Fuc_Value]
                      FROM [dbo].[PMTsConfig]
                      where FactoryCode = '{0}' and Fuc_Name in ('SDocDate')";
                string message_SDocDate = string.Format(_SDocDate
                  , factorycode
                  );
                var _SDocDateTmp = db.Query<string>(message_SDocDate).First();


                string _CompanyEn = @" SELECT[Fuc_Value]
                      FROM [dbo].[PMTsConfig]
                      where FactoryCode = '{0}' and Fuc_Name in ('CompanyEn')";
                string message_CompanyEn = string.Format(_CompanyEn
                  , factorycode
                  );
                var _CompanyEnTmp = db.Query<string>(message_CompanyEn).First();


                string _CompanyTh = @" select CompanyName from CompanyProfile where Plant = '{0}'";
                string message_CompanyTh = string.Format(_CompanyTh
                  , factorycode
                  );
                var _CompanyThtmp = db.Query<string>(message_CompanyTh).First();

                string _UserCreate = @" select (FirstNameTh + ' ' + LastNameTh) as username from MasterUser where FactoryCode = '{0}' and UserName = '{1}'";
                string message__UserCreate = string.Format(_UserCreate
                 , factorycode
                  , user
                 );
                var _UserCreateTmp = db.Query<string>(message__UserCreate).First();

                model.CompanyEn = _CompanyEnTmp;
                model.SDocName = _SDocNameTmp;
                model.SDocDate = _SDocDateTmp;

                model.CompanyTh = _CompanyThtmp;
                model.Docsnumber = snumber;
                model.UserCreate = _UserCreateTmp;

                //model.reportDocumentSlists = PMTsDbContext.DocumentSlist.Where(x => x.FactoryCode == factorycode && x.Snumber == snumber).ToList();
                model.reportDocumentSlists = PMTsDbContext.DocumentSlist.Where(x => x.Snumber == snumber).ToList();
            }
            return model;
        }

        public List<DocumentsMOData> GetDocumentsAndMODataByOrderItem(string factoryCode, string orderItem, string sNumber, IConfiguration config)
        {
            var documentsMOData = new List<DocumentsMOData>();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"Select
                    mo.FactoryCode
                    ,ds.SNumber
                    ,mo.OrderItem
                    ,ms.PC as Pc
                    ,mo.Material_No as MaterialNo
                    ,mo.Name as CustomerName
                    ,ms.Flute
                    ,ds.DuedateOld
                    ,ds.DuedateNew
                    ,mo.Order_Quant as OrderQuantity
                    ,ds.OrderQtyOld
                    ,ds.OrderQtyNew
                    ,ds.Cancel
                    ,ds.Hold
                    ,ds.BoxStatus
                    ,ds.PartStatus
                    ,STUFF((SELECT ',' + cast(US.[Machine] as nvarchar(10))
                        FROM [MO_Routing] US
                        WHERE US.FactoryCode = mo.FactoryCode and US.[OrderItem] = mo.OrderItem
                        FOR XML PATH('')), 1, 1, '')  Process
                    ,ds.Remark
                    ,ds.CreatedDate
                    ,ds.CreatedBy
                    ,ds.UpdatedDate
                    ,ds.UpdatedBy
                    ,mo.Due_Date as DueDate
                    ,ms.Box_Type as BoxType
                    ,mo.MO_Status as MoStatus
                    ,ms.Piece_Set as PieceSet
                    ,ms.Pro_Type as ProductType
                    ,ms.Pro_Type as ProductType
                    ,ms.Material_Type as MaterialType
                    ,mo.PO_No as PoNo
                    ,ms.Sale_Text1 as SaleText1
                    ,ds.BoxStatus as BoxStatus
                    From
                        (select * from MO_DATA
                        where OrderItem like '%{0}%' and MO_Status != 'X') mo 
                        left outer join DocumentSList ds on ds.OrderItem = mo.OrderItem and ds.FactoryCode = mo.FactoryCode and ds.SNumber = '{2}'
                        left outer join MO_Spec ms on ms.Material_No = mo.Material_No and ms.FactoryCode = mo.FactoryCode and ms.OrderItem = mo.OrderItem
                        Order By mo.OrderItem
                    ";

            string message = string.Format(sql, orderItem, factoryCode, sNumber);
            documentsMOData = db.Query<DocumentsMOData>(message).ToList();
            documentsMOData.ForEach(d => d.isBox =
            (d.ProductType == "Carton" || d.ProductType == "Corrugated Containers")
            && (d.PieceSet == 1 || d.Pc.Contains("-00") && d.MaterialType == "81")
            ? true : false);
            //documentsMOData.ForEach(d => d.BoxStatus = d.MaterialType == "82" ? "" : d.PartStatus);
            //documentsMOData.ForEach(d => d.PartStatus = d.MaterialType == "82" ? d.PartStatus : "");
            documentsMOData.ForEach(d => d.ChangeQuantiry = null);
            var orderItems = new List<string>();
            orderItems = documentsMOData.Select(m => m.OrderItem).ToList();
            foreach (var orderItemStr in orderItems)
            {
                var moRoutings = PMTsDbContext.MoRouting.Where(x => x.OrderItem == orderItemStr).ToList();

                var Non_Print = string.Empty;
                var Standard = string.Empty;
                var Semi = string.Empty;
                var Shipping_Mark = string.Empty;

                foreach (var moRouting in moRoutings)
                {
                    var machine = PMTsDbContext.Machine.Where(x => x.Code == moRouting.MatCode).FirstOrDefault();

                    if (machine != null && machine.MachineGroup == "2")
                    {
                        if (moRouting.NoneBlk == true)
                        {
                            Non_Print = ",Non-Print";
                        }
                        if (moRouting.SemiBlk == true)
                        {
                            Non_Print = ",Semi";
                        }
                        if (moRouting.StanBlk == true)
                        {
                            Non_Print = ",Standard";
                        }
                        if (moRouting.ShipBlk == true)
                        {
                            Shipping_Mark = ",Shipping-Mark";
                        }
                    }
                }

                string blockType = Non_Print + Standard + Semi + Shipping_Mark;
                if (blockType.Trim() != "")
                {
                    blockType = blockType.Substring(1, blockType.Length - 1);
                    var setDocuments = documentsMOData.Where(d => d.OrderItem == orderItemStr).ToList();
                    setDocuments.ForEach(d => d.BlockType = blockType);
                }
            }

            return documentsMOData;
        }

        public void SaveChangeDocuments(string factoryCode, List<DocumentSlist> modelDocuments, IConfiguration config)
        {
            var numberOfProcess = 0;
            var curentSNumber = string.Empty;
            modelDocuments = modelDocuments.OrderBy(d => d.Snumber).ToList();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
            {
                db.Open();
            }

            foreach (var item in modelDocuments)
            {
                var existDocumentsList = PMTsDbContext.DocumentSlist.FirstOrDefault(d => d.OrderItem == item.OrderItem && d.Snumber == item.Snumber);

                #region Update Documents
                if (curentSNumber != item.Snumber)
                {
                    var existDocuments = PMTsDbContext.DocumentS.FirstOrDefault(d => d.Snumber == item.Snumber);
                    if (existDocuments == null)
                    {
                        var documents = new DocumentS
                        {
                            FactoryCode = item.FactoryCode,
                            Snumber = item.Snumber,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = item.UpdatedBy,
                            UpdatedDate = DateTime.Now
                        };
                        PMTsDbContext.DocumentS.Add(documents);
                    }
                    else
                    {
                        existDocuments.CreatedBy = string.IsNullOrEmpty(existDocuments.CreatedBy) ? item.UpdatedBy : existDocuments.CreatedBy;
                        existDocuments.UpdatedBy = item.UpdatedBy;
                        existDocuments.UpdatedDate = DateTime.Now;
                        PMTsDbContext.DocumentS.Update(existDocuments);
                    }
                    numberOfProcess++;
                }
                #endregion

                #region Update DocumentsList
                if (existDocumentsList == null)
                {
                    item.CreatedDate = item.CreatedDate.HasValue ? item.CreatedDate : DateTime.Now;
                    item.UpdatedDate = item.UpdatedDate.HasValue ? item.UpdatedDate : DateTime.Now;
                    if (string.IsNullOrEmpty(item.MatDesc))
                    {
                        var masterdata = PMTsDbContext.MasterData.FirstOrDefault(m => m.MaterialNo == item.MaterialNo);
                        item.MatDesc = masterdata != null ? masterdata.Description : item.MatDesc;
                    }

                    PMTsDbContext.DocumentSlist.Add(item);
                }
                else
                {
                    existDocumentsList.BoxStatus = item.BoxStatus;
                    existDocumentsList.Cancel = item.Cancel;
                    existDocumentsList.CustomerName = item.CustomerName;
                    existDocumentsList.DuedateNew = item.DuedateNew;
                    existDocumentsList.DuedateOld = item.DuedateOld;
                    existDocumentsList.FactoryCode = item.FactoryCode;
                    existDocumentsList.Flute = item.Flute;
                    existDocumentsList.Hold = item.Hold;
                    existDocumentsList.OrderQtyOld = item.OrderQtyOld;
                    existDocumentsList.OrderQtyNew = item.OrderQtyNew;
                    existDocumentsList.PartStatus = item.PartStatus;
                    existDocumentsList.Pc = item.Pc;
                    existDocumentsList.Process = item.Process;
                    existDocumentsList.Remark = item.Remark;
                    existDocumentsList.Snumber = item.Snumber;
                    existDocumentsList.MaterialNo = item.MaterialNo;
                    existDocumentsList.OrderItem = item.OrderItem;
                    existDocumentsList.CreatedBy = string.IsNullOrEmpty(item.CreatedBy) ? item.CreatedBy : existDocumentsList.CreatedBy;
                    existDocumentsList.CreatedDate = item.CreatedDate.HasValue ? item.CreatedDate : DateTime.Now;
                    existDocumentsList.UpdatedBy = string.IsNullOrEmpty(item.UpdatedBy) ? item.UpdatedBy : existDocumentsList.UpdatedBy;
                    existDocumentsList.UpdatedDate = item.UpdatedDate.HasValue ? item.UpdatedDate : DateTime.Now;
                    if (string.IsNullOrEmpty(existDocumentsList.MatDesc))
                    {
                        var masterdata = PMTsDbContext.MasterData.FirstOrDefault(m => m.MaterialNo == existDocumentsList.MaterialNo);
                        existDocumentsList.MatDesc = masterdata != null ? masterdata.Description : existDocumentsList.MatDesc;
                    }
                    PMTsDbContext.DocumentSlist.Update(existDocumentsList);

                }
                numberOfProcess++;
                #endregion

                curentSNumber = item.Snumber;
            }

            #region Check update
            var transaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var update = PMTsDbContext.SaveChanges();

                if (update == numberOfProcess)
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception("Unable to Save: An error occurred while saving data.");
                }

            }
            catch
            {
                transaction.Rollback();
                throw new Exception("Unable to Save: An error occurred while saving data.");
            }
            #endregion

        }

        public List<DocumentSlist> GetDocumentSListForReportDocument(IConfiguration configuration, string factoryCode, string materialNO, string sO, string custName, string pC)
        {
            var results = new List<DocumentSlist>();
            string condition = string.Empty;
            condition = !string.IsNullOrEmpty(materialNO) ? $"and MaterialNo like '%{materialNO}%'" : condition;
            condition = !string.IsNullOrEmpty(pC) ? condition + $"and PC like '%{pC}%'" : condition;
            condition = !string.IsNullOrEmpty(custName) ? condition + $" and CustomerName like '%{custName}%'" : condition;
            condition = !string.IsNullOrEmpty(sO) ? condition + $" and OrderItem like '%{sO}%'" : condition;

            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string sql = @"Select *
                        FROM DocumentSList
                        Where FactoryCode like '%{0}%' " + condition + " Order By OrderItem";


                string message = string.Format(sql, factoryCode);
                results = db.Query<DocumentSlist>(message).ToList();
            }

            return results;
        }
    }
}
