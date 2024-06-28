using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PMTs.DataAccess.Repositories
{
    public partial class MasterDataRepository(PMTsDbContext context) : Repository<MasterData>(context), IMasterDataRepository
    {
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<MasterData> GetMasterDataAll(string factoryCode)
        {
            return PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode).ToList();
        }

        public MasterData GetMasterDataByMaterialNumber(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo);
        }

        public MasterData GetMasterDataByMaterialNoAndFactory(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo);
        }

        public MasterData GetMasterDataByMaterialNumberNonX(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo);
        }

        public MasterData GetMasterDataByMaterialNumberNonNotX(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus != "X");
        }

        public MasterData GetMasterDataByMaterialNumberX(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus == "X");
        }

        public IEnumerable<MasterData> GetMasterDatasByMaterialNumber(string materialNo)
        {
            return PMTsDbContext.MasterData.Where(m => m.MaterialNo == materialNo && m.PdisStatus != "X").ToList();
        }

        public MasterData GetMasterDataByDescription(string factoryCode, string description)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode.Equals(factoryCode) && m.Description.Equals(description) && m.PdisStatus != "X");
        }

        public IEnumerable<MasterData> GetMasterDataTop100Update(string factoryCode)
        {
            return PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList().OrderByDescending(m => m.LastUpdate).Take(100);
        }

        public IEnumerable<MasterData> GetMasterDatasByMatSaleOrgNonX(string factoryCode, string MaterialNo)
        {
            var result = (from t in PMTsDbContext.TransactionsDetail.Where(t => t.MatSaleOrg == MaterialNo)
                          join m in PMTsDbContext.MasterData.Where(m => m.FactoryCode != factoryCode) on t.MaterialNo equals m.MaterialNo
                          select m).ToList();

            return result;
        }

        public IEnumerable<MasterDataRoutingModel> GetMasterDataList(IConfiguration configuration, string factoryCode)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT TOP (20)
                    m.Material_No as MaterialNo
                    ,m.PC
	                ,m.Sale_Org as SaleOrg
	                ,m.Plant
	                ,m.Cust_Code as CustCode
	                ,m.Cus_ID as CusId
	                ,m.Cust_Name  as CustName
	                ,m.Description
	                ,m.Flute + ' ' + m.Board as Board
	                ,m.Box_Type as BoxType
	                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				            FROM Routing r
				            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				            ORDER BY r.Seq_No
				            FOR XML PATH('')), 1, 0, '')
	                ,m.LastUpdate
	                ,m.CreateDate
	                ,m.Tran_Status as TranStatus
	                ,m.PDIS_Status as PDISStatus
                    ,t.MatSaleOrg as MatSaleOrg
                    ,m.Part_No as PartNo
                    ,m.UpdatedBy
                    from MasterData m left outer join Transactions_Detail t
                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(t.PDIS_Status) != 'X'
                    where  UPPER(m.PDIS_Status) != 'X' and m.FactoryCode = '{1}'
					order by m.LastUpdate desc
                        ";
            string message = string.Format(sql,
               factoryCode,
               factoryCode
                );
            return db.Query<MasterDataRoutingModel>(message).ToList();
        }

        public IEnumerable<MasterData> GetMasterDataByBomChild(string factoryCode, string MaterialNo, string Custcode, string ProductCode)
        {
            var result = PMTsDbContext.MasterData.Where(m => m.MaterialType != "84" && m.FactoryCode == factoryCode && m.PdisStatus != "X" && m.MaterialNo.Contains(MaterialNo) && m.CustName.Contains(Custcode) && m.Pc.Contains(ProductCode)).ToList();

            return result;
        }

        public MasterData SearchBomStructsByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => (m.MaterialNo.Contains(materialNo) && (m.MaterialType == "84" || m.MaterialType == "14" || m.MaterialType == "24") && m.FactoryCode == factoryCode) && m.PdisStatus != "X");
        }

        public MasterData SearchBomStructsBytxtSearch(string factoryCode, string txtSearch)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => (m.Pc.Contains(txtSearch) && (m.MaterialType == "84" || m.MaterialType == "14" || m.MaterialType == "24") && m.FactoryCode == factoryCode) && m.PdisStatus != "X");
        }

        public IEnumerable<MasterDataRoutingModel> GetMasterDataByKeySearch(IConfiguration configuration, string factoryCode, string typeSearch, string keySearch, string flag)
        {
            string condition = flag == null || flag != "X" ? " and UPPER(m.PDIS_Status) != 'X'" : "";
            switch (typeSearch)
            {
                case "SaleOrg":
                    {
                        var searchData = keySearch.Split(",");

                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                               SELECT TOP (20)
                                  m.Material_No as MaterialNo
                                 ,m.PC
	                             ,m.Sale_Org as SaleOrg
	                             ,m.Plant
	                             ,m.Cust_Code as CustCode
	                             ,m.Cus_ID as CusId
	                             ,m.Cust_Name  as CustName
	                             ,m.Description
	                             ,m.Flute + ' ' + m.Board as Board
	                             ,m.Box_Type as BoxType
	                             ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                             ,m.LastUpdate
	                             ,m.CreateDate
	                             ,m.Tran_Status as TranStatus
	                             ,m.PDIS_Status as PDISStatus
                                 ,t.MatSaleOrg as MatSaleOrg
                                ,m.Part_No as PartNo
                                ,m.UpdatedBy
                                 from MasterData m left outer join Transactions_Detail t
                                 on m.Material_No = t.MaterialNo  and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                 where  m.FactoryCode = '{1}' and m.Sale_Org like '%{2}%' " + condition + @"
                                 order by m.LastUpdate desc
                               ";
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                           searchData[1]
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "Material_No":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();

                        string sql = @"
                               SELECT TOP (20)
                                m.Material_No as MaterialNo
                                ,m.PC
	                            ,m.Sale_Org as SaleOrg
	                            ,m.Plant
	                            ,m.Cust_Code as CustCode
	                            ,m.Cus_ID as CusId
	                            ,m.Cust_Name  as CustName
	                            ,m.Description
	                            ,m.Flute + ' ' + m.Board as Board
	                            ,m.Box_Type as BoxType
	                            ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                        FROM Routing r
				                        WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                        ORDER BY r.Seq_No
				                        FOR XML PATH('')), 1, 0, '')
	                            ,m.LastUpdate
	                            ,m.CreateDate
	                            ,m.Tran_Status as TranStatus
	                            ,m.PDIS_Status as PDISStatus
                                ,t.MatSaleOrg as MatSaleOrg
                                ,m.Part_No as PartNo
                                ,m.UpdatedBy
                                from MasterData m left outer join Transactions_Detail t
                                on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
							    where  m.FactoryCode = '{1}' and m.Material_No Like '%{2}%'" + condition + @"
							    order by m.LastUpdate desc
                               ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "Description":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                               SELECT TOP (20)
                                    m.Material_No as MaterialNo
                                    ,m.PC
	                                ,m.Sale_Org as SaleOrg
	                                ,m.Plant
	                                ,m.Cust_Code as CustCode
	                                ,m.Cus_ID as CusId
	                                ,m.Cust_Name  as CustName
	                                ,m.Description
	                                ,m.Flute + ' ' + m.Board as Board
	                                ,m.Box_Type as BoxType
	                                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                                ,m.LastUpdate
	                                ,m.CreateDate
	                                ,m.Tran_Status as TranStatus
	                                ,m.PDIS_Status as PDISStatus
                                    ,t.MatSaleOrg as MatSaleOrg
                                    ,m.Part_No as PartNo
                                    ,m.UpdatedBy
                                    from MasterData m left outer join Transactions_Detail t
                                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                    where  m.FactoryCode = '{1}' and m.Description Like '%{2}%'" + condition + @"
							        order by m.LastUpdate desc
                                ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "PC":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                                SELECT TOP (20)
                                m.Material_No as MaterialNo
                                ,m.PC
	                            ,m.Sale_Org as SaleOrg
	                            ,m.Plant
	                            ,m.Cust_Code as CustCode
	                            ,m.Cus_ID as CusId
	                            ,m.Cust_Name  as CustName
	                            ,m.Description
	                            ,m.Flute + ' ' + m.Board as Board
	                            ,m.Box_Type as BoxType
	                            ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                        FROM Routing r
				                        WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                        ORDER BY r.Seq_No
				                        FOR XML PATH('')), 1, 0, '')
	                            ,m.LastUpdate
	                            ,m.CreateDate
	                            ,m.Tran_Status as TranStatus
	                            ,m.PDIS_Status as PDISStatus
                                ,t.MatSaleOrg as MatSaleOrg
                                ,m.Part_No as PartNo
                                ,m.UpdatedBy
                                from MasterData m left outer join Transactions_Detail t
                                on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                where  m.FactoryCode = '{1}' and m.PC Like '%{2}%'" + condition + @"
							    order by m.LastUpdate desc
                            ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "PartNo":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                               SELECT TOP (20)
                                m.Material_No as MaterialNo
                                ,m.PC
	                            ,m.Sale_Org as SaleOrg
	                            ,m.Plant
	                            ,m.Cust_Code as CustCode
	                            ,m.Cus_ID as CusId
	                            ,m.Cust_Name  as CustName
	                            ,m.Description
	                            ,m.Flute + ' ' + m.Board as Board
	                            ,m.Box_Type as BoxType
	                            ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                        FROM Routing r
				                        WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                        ORDER BY r.Seq_No
				                        FOR XML PATH('')), 1, 0, '')
	                            ,m.LastUpdate
	                            ,m.CreateDate
	                            ,m.Tran_Status as TranStatus
	                            ,m.PDIS_Status as PDISStatus
                                ,t.MatSaleOrg as MatSaleOrg
                                ,m.Part_No as PartNo
                                ,m.UpdatedBy
                                from MasterData m left outer join Transactions_Detail t
                                on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                where  m.FactoryCode = '{1}' and m.Part_No Like '%{2}%'" + condition + @"
							    order by m.LastUpdate desc
                            ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "Board":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                               SELECT TOP (20)
                                    m.Material_No as MaterialNo
                                    ,m.PC
	                                ,m.Sale_Org as SaleOrg
	                                ,m.Plant
	                                ,m.Cust_Code as CustCode
	                                ,m.Cus_ID as CusId
	                                ,m.Cust_Name  as CustName
	                                ,m.Description
	                                ,m.Flute + ' ' + m.Board as Board
	                                ,m.Box_Type as BoxType
	                                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                                ,m.LastUpdate
	                                ,m.CreateDate
	                                ,m.Tran_Status as TranStatus
	                                ,m.PDIS_Status as PDISStatus
                                    ,t.MatSaleOrg as MatSaleOrg
                                    ,m.Part_No as PartNo
                                    ,m.UpdatedBy
                                    from MasterData m left outer join Transactions_Detail t
                                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                    where  m.FactoryCode = '{1}' and m.Board Like '%{2}%'" + condition + @"
							        order by m.LastUpdate desc
                                ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "Box_Type":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                                SELECT TOP (20)
                                m.Material_No as MaterialNo
                                ,m.PC
	                            ,m.Sale_Org as SaleOrg
	                            ,m.Plant
	                            ,m.Cust_Code as CustCode
	                            ,m.Cus_ID as CusId
	                            ,m.Cust_Name  as CustName
	                            ,m.Description
	                            ,m.Flute + ' ' + m.Board as Board
	                            ,m.Box_Type as BoxType
	                            ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                        FROM Routing r
				                        WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                        ORDER BY r.Seq_No
				                        FOR XML PATH('')), 1, 0, '')
	                            ,m.LastUpdate
	                            ,m.CreateDate
	                            ,m.Tran_Status as TranStatus
	                            ,m.PDIS_Status as PDISStatus
                                ,t.MatSaleOrg as MatSaleOrg
                                ,m.Part_No as PartNo
                                ,m.UpdatedBy
                                from MasterData m left outer join Transactions_Detail t
                                on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                where  m.FactoryCode = '{1}' and m.Box_Type Like '%{2}%'" + condition + @"
							    order by m.LastUpdate desc
                            ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "Cust_Name":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                                    SELECT TOP (20)
                                    m.Material_No as MaterialNo
                                    ,m.PC
	                                ,m.Sale_Org as SaleOrg
	                                ,m.Plant
	                                ,m.Cust_Code as CustCode
	                                ,m.Cus_ID as CusId
	                                ,m.Cust_Name  as CustName
	                                ,m.Description
	                                ,m.Flute + ' ' + m.Board as Board
	                                ,m.Box_Type as BoxType
	                                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                                ,m.LastUpdate
	                                ,m.CreateDate
	                                ,m.Tran_Status as TranStatus
	                                ,m.PDIS_Status as PDISStatus
                                    ,t.MatSaleOrg as MatSaleOrg
                                    ,m.Part_No as PartNo
                                    ,m.UpdatedBy
                                    from MasterData m left outer join Transactions_Detail t
                                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                    where m.FactoryCode = '{1}' and m.Cust_Name Like '%{2}%'" + condition + @"
							        order by m.LastUpdate desc
                                ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                          keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                case "MatSaleOrg":
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                                    SELECT TOP (20)
                                    m.Material_No as MaterialNo
                                    ,m.PC
	                                ,m.Sale_Org as SaleOrg
	                                ,m.Plant
	                                ,m.Cust_Code as CustCode
	                                ,m.Cus_ID as CusId
	                                ,m.Cust_Name  as CustName
	                                ,m.Description
	                                ,m.Flute + ' ' + m.Board as Board
	                                ,m.Box_Type as BoxType
	                                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                                ,m.LastUpdate
	                                ,m.CreateDate
	                                ,m.Tran_Status as TranStatus
	                                ,m.PDIS_Status as PDISStatus
                                    ,t.MatSaleOrg as MatSaleOrg
                                    ,m.Part_No as PartNo
                                    ,m.UpdatedBy
                                    from MasterData m left outer join Transactions_Detail t
                                    on m.Material_No = t.MaterialNo  and m.FactoryCode = t.FactoryCode and UPPER(isnull(t.PDIS_Status,'')) != 'X'
                                    where m.FactoryCode = '{1}' and t.MatSaleOrg = '{2}'" + condition + @"
                                    order by m.LastUpdate desc
                                ";   // m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode,
                           keySearch
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
                default:
                    {
                        using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        //Execute sql query
                        string sql = @"
                                    SELECT TOP (20)
                                    m.Material_No as MaterialNo
                                    ,m.PC
	                                ,m.Sale_Org as SaleOrg
	                                ,m.Plant
	                                ,m.Cust_Code as CustCode
	                                ,m.Cus_ID as CusId
	                                ,m.Cust_Name  as CustName
	                                ,m.Description
	                                ,m.Flute + ' ' + m.Board as Board
	                                ,m.Box_Type as BoxType
	                                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                            FROM Routing r
				                            WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				                            ORDER BY r.Seq_No
				                            FOR XML PATH('')), 1, 0, '')
	                                ,m.LastUpdate
	                                ,m.CreateDate
	                                ,m.Tran_Status as TranStatus
	                                ,m.PDIS_Status as PDISStatus
                                    ,t.MatSaleOrg as MatSaleOrg
                                    ,m.Part_No as PartNo
                                    ,m.UpdatedBy
                                    from MasterData m left outer join Transactions_Detail t
                                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode and UPPER(t.PDIS_Status) != 'X'
                                    where  m.FactoryCode = '{1}'" + condition + @"
							        order by m.LastUpdate desc
                                ";   //m.PDIS_Status != 'x'  and
                        string message = string.Format(sql,
                           factoryCode,
                           factoryCode
                            );
                        return db.Query<MasterDataRoutingModel>(message).ToList();
                    }
            }
        }

        public MasterData GetMasterDataByProdCode(string FactoryCode, string prodCode)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == FactoryCode && m.Pc == prodCode && m.PdisStatus != "X");
        }

        public IEnumerable<MasterData> GetMasterDataAllByKeySearch(string keySearch)
        {
            if (string.IsNullOrEmpty(keySearch) || string.IsNullOrWhiteSpace(keySearch))
            {
                return PMTsDbContext.MasterData.ToList().Take(20);
            }
            else
            {
                return PMTsDbContext.MasterData.Where(m => m.MaterialNo.Contains(keySearch)).ToList().Take(20);
            }
        }

        public void UpdatePDISStatus(string FactoryCode, string MaterialNo, string Status, string Username)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                some.ForEach(a => a.PdisStatus = Status);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
                UpdateIsTranfer(FactoryCode, MaterialNo, Username);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        //Handshake Interface
        public IEnumerable<MasterData> GetMasterDataHandshake(string factoryCode, string saleOrg)
        {
            List<string> MaterialType = new List<string> { "81", "82", "84", "87" };
            return PMTsDbContext.MasterData.OrderBy(m => m.User).ThenBy(m => m.UpdatedBy)
                .Where(m => m.FactoryCode == factoryCode &
                        m.SaleOrg == saleOrg &
                        m.PdisStatus != "N" &
                        m.TranStatus == false &
                        MaterialType.Contains(m.MaterialType))
                .Take(200)
                .ToList();
        }

        //Handshake Interface
        public IEnumerable<MasterData> GetMasterDataHandshakeOCG(string factoryCode, string saleOrg)
        {
            var currDate = DateTime.Now;
            var endDate = new DateTime(currDate.Year, currDate.Month, currDate.Day, 18, 0, 0);
            var starDate = endDate.AddDays(-1);
            List<string> MaterialType = new List<string> { "81", "82", "84", "87" };
            return
                PMTsDbContext.MasterData
                    .OrderBy(m => m.User)
                    .ThenBy(m => m.UpdatedBy)
                    .Where(m => m.FactoryCode == factoryCode &
                            m.SaleOrg == saleOrg &
                            m.PdisStatus != "N" &
                            m.LastUpdate.Value >= starDate && m.LastUpdate <= endDate &&
                            //m.TranStatus == false &
                            MaterialType.Contains(m.MaterialType))
                    .Take(200)
                    .ToList();
        }

        //Update Weigth Box by Routing
        public bool UpdateWeigthBox(string Mat, double? Weigth, string Username, string factoryCode)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var companyall = PMTsDbContext.CompanyProfile.ToList();
                var company = companyall.FirstOrDefault(c => c.Plant == factoryCode);

                //ตรวจสอบหา Mat ตัวเอง
                var itMasterdata = PMTsDbContext.MasterData.Where(x => x.FactoryCode == factoryCode && x.SaleOrg == company.SaleOrg
                 && x.MaterialNo == Mat && x.PdisStatus != "X" && x.SapStatus == false).FirstOrDefault();
                if (itMasterdata != null) //โรงของตัวเอง
                {
                    if (Weigth == null || Weigth == 0)
                    {
                        itMasterdata.WeightBox = itMasterdata.WeightBox;
                    }
                    else
                    {
                        itMasterdata.WeightBox = Weigth == null ? 0 : Weigth;
                    }

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                    UpdateIsTranfer(itMasterdata.FactoryCode, itMasterdata.MaterialNo, Username);
                    return true;
                }
                else
                {
                    var matplant = PMTsDbContext.MasterData.Where(x => x.FactoryCode == factoryCode
                 && x.MaterialNo == Mat && x.PdisStatus != "X").FirstOrDefault();
                    if (matplant != null)
                    {
                        var compOS = companyall.FirstOrDefault(c => c.SaleOrg == matplant.SaleOrg);
                        // ถ้าเป็น mat ที่ถูกจ้างมา ให้หา row ของ ผู้จ้าง แล้วดูว่าถูกส่ง sap ไปรึยัง
                        var MatOS = PMTsDbContext.MasterData.Where(x => x.FactoryCode == compOS.Plant && x.SaleOrg == compOS.SaleOrg
                     && x.MaterialNo == Mat && x.PdisStatus != "X" & x.SapStatus == false).FirstOrDefault();
                        if (MatOS != null)
                        {
                            var MasterdataOS = PMTsDbContext.MasterData.Where(x => x.SaleOrg == MatOS.SaleOrg
                    && x.MaterialNo == Mat && x.PdisStatus != "X" && x.SapStatus == false).ToList();
                            foreach (var item in MasterdataOS)
                            {
                                item.WeightBox = Weigth == null ? 0 : Weigth;
                                PMTsDbContext.SaveChanges();
                            }
                            dbContextTransaction.Commit();

                            //UpdateIsTranfer(itMasterdata.FactoryCode, itMasterdata.MaterialNo, Username);
                            return true;
                        }
                        else
                        {
                            var itMasterdataplant = PMTsDbContext.MasterData.Where(x => x.FactoryCode == factoryCode
                     && x.MaterialNo == Mat && x.PdisStatus != "X" && x.SapStatus == false).FirstOrDefault();
                            if (itMasterdataplant != null) //โรงของตัวเอง
                            {
                                //itMasterdataplant.WeightBox = Weigth == null ? 0 : Weigth;

                                if (Weigth == null || Weigth == 0)
                                {
                                    itMasterdataplant.WeightBox = itMasterdataplant.WeightBox;
                                }
                                else
                                {
                                    itMasterdataplant.WeightBox = Weigth == null ? 0 : Weigth;
                                }

                                PMTsDbContext.SaveChanges();
                                dbContextTransaction.Commit();
                                //UpdateIsTranfer(itMasterdataplant.FactoryCode, itMasterdataplant.MaterialNo, Username);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // // ตรวจสอบว่าข้อมูลถูกส่งไปรึยัง
                //var chksapstatus = PMTsDbContext.MasterData.Where(x => x.FactoryCode == factoryCode
                // && x.MaterialNo == Mat && x.PdisStatus != "X" && x.SapStatus == false).FirstOrDefault();
                // if (chksapstatus != null) // ยังไม่ได้ส่ง
                // {
                //     var weightupdate = PMTsDbContext.MasterData.Where(x => x.MaterialNo == Mat && x.PdisStatus != "X" && x.FactoryCode==factoryCode).FirstOrDefault();

                // }// else ไม่ต้องทำไรต่อ
                // // ตรวจสอบว่าเป็นงานจ้างไหม

                // var company = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);

                // var itMasterdata = PMTsDbContext.MasterData.Where(x=>x.FactoryCode == factoryCode && x.SaleOrg == company.SaleOrg
                // && x.MaterialNo == Mat && x.PdisStatus != "X" && x.SapStatus == false).FirstOrDefault();
                // if (itMasterdata != null) //โรงของตัวเอง
                // {
                //         itMasterdata.WeightBox = Weigth == null ? 0 : Weigth;

                // }else
                // {
                // }

                // ให้เช็คเรื่องของ sap status ด้วย ถ้าโยนแล้วไม่ต้อง update กลับไปที่ ผู้จ้าง กรณี Outsource

                //var it_trans_update = PMTsDbContext.MasterData.Where(IT => IT.MaterialNo == Mat && IT.PdisStatus != "X").FirstOrDefault();
                //if (it_trans_update != null)
                //{
                //    it_trans_update.WeightBox = Weigth == null ? 0 : Weigth;
                //    PMTsDbContext.SaveChanges();
                //    dbContextTransaction.Commit();
                //    UpdateIsTranfer(it_trans_update.FactoryCode, it_trans_update.MaterialNo, Username);
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch
            {
                dbContextTransaction.Rollback();
                return false;
            }
        }

        //Update Transtatus from Handshake
        public void UpdateTranStatus(string FactoryCode, string MaterialNo, bool Status, string Username)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                //var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();

                //var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo).ToList();

                var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && MaterialNo.Contains(s.MaterialNo)).ToList();
                some.ForEach(a => a.TranStatus = Status);
                some.ForEach(a => a.SapStatus = Status);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
                UpdateIsTranfer(FactoryCode, MaterialNo, Username);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        //bomstruct Search parent
        public List<MasterData> SearchBomStructsByMaterialNoAll(string factoryCode, string materialNo)
        {
            var MaterialType = new List<string> { "84", "14", "24" };
            List<MasterData> result =
            [
                .. PMTsDbContext.MasterData
                    .Where(m => (m.MaterialNo.Contains(materialNo) &&
                            MaterialType.Contains(m.MaterialType) &&
                            m.FactoryCode == factoryCode)),
            ];

            return result;
        }

        public List<MasterData> SearchBomStructsBytxtSearchAll(string factoryCode, string txtSearch)
        {
            var result = PMTsDbContext.MasterData.Where(m => (m.Pc.Contains(txtSearch) && (m.MaterialType == "84" || m.MaterialType == "14" || m.MaterialType == "24") && m.FactoryCode == factoryCode)).ToList();
            return result;//PMTsDbContext.MasterData.Select(m => (m.Pc.Contains(txtSearch) && (m.MaterialType == "84" || m.MaterialType == "14" || m.MaterialType == "24") && m.FactoryCode == factoryCode));
        }

        //Update Transtatus from By Bomstruct
        public void UpdateTranStatusByBomStruct(string FactoryCode, string MaterialNo, string Status, string Username)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                some.ForEach(a => a.TranStatus = Status == "1");
                // some.ForEach(a => a.SapStatus = Status);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
                UpdateIsTranfer(FactoryCode, MaterialNo, Username);
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public void UpdateCapImgTransactionDetails(string FactoryCode, string MaterialNo, string Status)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var some = PMTsDbContext.TransactionsDetail.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo).ToList();
                some.ForEach(a => a.CapImg = Status == "1");
                // some.ForEach(a => a.SapStatus = Status);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public List<MasterDataQuery> GetProductCatalog(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel)
        {
            if (ProductCatalogModel.FactoryCodeProduction == "[]")
            {
                ProductCatalogModel.FactoryCodeProduction = string.Empty;
            }
            var conditonRouting = string.IsNullOrEmpty(ProductCatalogModel.plateNo) && string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "and rt.Seq_No = 1 " : string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? " and isnull(rt.Block_No, '') <> '' " : " and isnull(rt.Plate_No, '') <> '' ";
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    select TOP (200)
                    m.Material_No  as MaterialNo
                    ,m.FactoryCode
                    ,m.Part_No as PartNo
                    ,m.PC
                    ,m.Hierarchy
                    ,m.Sale_Org as SaleOrg
                    ,m.Plant
                    ,m.Cust_Code as CustCode
                    ,m.Cus_ID as CusID
	                ,m.Cust_Name as CustName
                    ,Trim(m.Description) as Description
                    ,(CASE WHEN LEN(m.Sale_Text1) > 0 THEN m.Sale_Text1 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text2) > 0 THEN m.Sale_Text2 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text3) > 0 THEN m.Sale_Text3 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text4) > 0 THEN m.Sale_Text4 ELSE '' END) as SaleText
                    ,m.Change
                    ,m.Ind_Grp as IndGrp
                    ,m.Ind_Des as IndDes
                    ,m.Material_Type as MaterialType
                    ,m.Print_Method as PrintMethod
                    ,m.Flute
                    ,m.Board
                    ,m.Wid
                    ,m.Leg
                    ,m.Hig
                    ,m.Box_Type as BoxType
                    ,m.JoinType
                    ,m.CutSheetLeng
                    ,m.CutSheetWid
                    ,m.Bun
                    ,m.BunLayer
                    ,m.LayerPalet
                    ,m.BoxPalet
                    ,round(m.Weight_Box, 3) as WeightBox
                    ,m.Piece_Set as PieceSet
                    ,m.Sale_UOM as SaleUOM
                    ,m.BOM_UOM as BOMUOM
                    ,m.PalletSize
                    ,FORMAT(m.LastUpdate, 'yyyy-MM-dd HH:mm:ss') as LastUpdate
                    ,(CASE WHEN LEN(m.Pur_Txt1) > 0 THEN m.Pur_Txt1 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt2) > 0 THEN m.Pur_Txt2 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt3) > 0 THEN m.Pur_Txt3 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt4) > 0 THEN m.Pur_Txt4 ELSE '' END) as PurTxt
                    ,m.High_Group as HighGroup
                    ,m.High_Value as HighValue
                    ,FORMAT(p.[validity start from], 'yyyy-MM-dd HH:mm:ss') as ValidityStartFrom
                    ,FORMAT(p.[validity end date], 'yyyy-MM-dd HH:mm:ss') as ValidityEndDate
                    ,p.rate as Rate
                    ,p.[currency key] as CurrencyKey
                    ,i.VENDORNAME as VendorName
                    ,i.NETPRICE as NetPrice
                    ,i.UNITOFNETPRICE as UnitOfNetPrice
                    ,FORMAT(i.SOURCELIST_VALIDFROM, 'yyyy-MM-dd HH:mm:ss') as SourceListValidFrom
                    ,FORMAT(i.SOURCELIST_VALIDTO, 'yyyy-MM-dd HH:mm:ss') as SourceListValidTo
                    ,r.Remark
                    ,r.NonMove
                    ,FORMAT(r.NonMoveMonth, 'yyyy-MM-dd HH:mm:ss') as NonMoveMonth
                    ,r.StockWIP
                    ,r.StockFG
					,r.StockQA
	                ,(case Left(m.PC,1) when 'h' then (case h.isLocked when 1 then 'Hold' else '' end ) else '' end )  as Hold
					,m.Piece_Set as PieceSet
                    ,h.HoldRemark
                    ,m.CustInvType
                    ,m.CIPInvType
                    ,m.PDIS_Status PdisStatus
                    ,rt.Block_No BlockNo
                    ,rt.Plate_No PlateNo
                    from MasterData m
                    left outer join Routing rt on rt.Material_No = m.Material_No and rt.FactoryCode = m.FactoryCode " + conditonRouting + @"
                    left outer join (select * from  Remark {0}) r on m.PC = r.PC
                    left outer join (select t.* from InfoRecordSourceList t
					inner join (
						select MATERIALNO, max(RecordedDate) as MaxDate
						from InfoRecordSourceList
						group by MATERIALNO
					) tm on t.MATERIALNO = tm.MATERIALNO and t.RecordedDate = tm.MaxDate {1} )  i
					on i.MATERIALNO = m.Material_No left outer join
                    (Select * from PricingMaster where  GETDATE() BETWEEN [validity start from] and [validity end date]) p
                    on p.[material no] = m.Material_No and p.[sales org] = m.Sale_Org
                    left outer join HoldMaterial h on  h.MaterialNo  = m.Material_No
                    where {2}
                    (
                        {3}
                        {4}
                        {5}
                        {6}
                        {7}
                        {8}
                    )
					and  isnull(m.Part_No,'') like '%{9}%'
                    and  isnull(m.Cust_Code,'') like '%{10}%'
                    and  isnull(m.Cus_ID,'')  like '%{11}%'
                    and  isnull(m.Cust_Name,'')  like '%{12}%'
                    and  isnull(m.Description,'')   like '%{13}%'
                    and  isnull(m.Sale_Text1,'') + isnull(m.Sale_Text2,'') + isnull(m.Sale_Text3,'') + isnull(m.Sale_Text4,'') like '%{14}%'
                    {15}
                    {16}
                    {17}
                    {18}
                    and  isnull(m.Board,'') like '%{19}%'
                    {20}
                    {21}
                    {22}
                    and  isnull(r.Remark , '') like '%{23}%'
                    and  isnull(m.Part_No , '') like '%{24}%'
                    {25}
                    {26}
                    {27}
                    {28}
                    {29}
                    and m.PDIS_Status <> 'X'
	                {37}
					and Trim(m.PC) <> ''
                    {30}
                    {31}
                    {32}
                    {33}
                    {34}
                    {35}
                    {36}
                    order by m.PC
                    ";
            string message = string.Format(sql,
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where t.FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where t.FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format(" m.FactoryCode in ({0}) and ", ProductCatalogModel.FactoryCode) : string.Format(" m.FactoryCode =  '{0}' and ", factory),
                string.IsNullOrEmpty(ProductCatalogModel.pc1) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc1) ? "m.PC like '%'" : string.Format(" m.PC like '%{0}%' ", ProductCatalogModel.pc1),
                string.IsNullOrEmpty(ProductCatalogModel.pc2) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc2) ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc2),
                string.IsNullOrEmpty(ProductCatalogModel.pc3) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc3) ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc3),
                string.IsNullOrEmpty(ProductCatalogModel.pc4) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc4) ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc4),
                string.IsNullOrEmpty(ProductCatalogModel.pc5) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc5) ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc5),
                string.IsNullOrEmpty(ProductCatalogModel.pc6) || string.IsNullOrWhiteSpace(ProductCatalogModel.pc6) ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc6),
                ProductCatalogModel.partNo,
                ProductCatalogModel.custCode,
                ProductCatalogModel.custID,
                ProductCatalogModel.custName,
                ProductCatalogModel.desc,
                ProductCatalogModel.saleText,
                "",
                "",
                "",
                string.IsNullOrEmpty(ProductCatalogModel.flute) ? "" : string.Format("and m.Flute = '{0}'", ProductCatalogModel.flute),
                ProductCatalogModel.board,
                ProductCatalogModel.wid < 1 ? "" : string.Format("and m.Wid = {0}", ProductCatalogModel.wid),
                ProductCatalogModel.leg < 1 ? "" : string.Format("and m.leg = {0}", ProductCatalogModel.leg),
                ProductCatalogModel.hig < 1 ? "" : string.Format("and m.Hig = {0}", ProductCatalogModel.hig),
                ProductCatalogModel.remark,
                ProductCatalogModel.partNo,
                ProductCatalogModel.idNonMove != true ? "" : "and r.NonMove is not null",
                ProductCatalogModel.idStockWIP != true ? "" : "and r.StockWIP is not null",
                ProductCatalogModel.idStockQA != true ? "" : "and r.StockQA is not null",
                ProductCatalogModel.idStockFG != true ? "" : "and r.StockFG is not null",
                ProductCatalogModel.rate == -99 || ProductCatalogModel.rate == 0 ? "" : string.Format("and p.rate = {0}", ProductCatalogModel.rate),
                string.IsNullOrEmpty(ProductCatalogModel.MaterialNo) ? "" : string.Format("and m.Material_No  like '%{0}%' ", ProductCatalogModel.MaterialNo),
                ProductCatalogModel.idHoldFind != true ? "" : "and h.isLocked = 1 and Left(m.PC, 1) = 'h'",
                string.IsNullOrEmpty(ProductCatalogModel.CustInvType) ? "" : string.Format("and m.CustInvType = '{0}'", ProductCatalogModel.CustInvType),
                string.IsNullOrEmpty(ProductCatalogModel.CIPInvType) ? "" : string.Format("and m.CIPInvType = '{0}'", ProductCatalogModel.CIPInvType),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCodeProduction) ? string.Empty : string.Format("and Sale_Org in ({0}) ", ProductCatalogModel.FactoryCodeProduction) : string.Empty,
                string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "" : string.Format("and rt.Block_No  like '%{0}%' ", ProductCatalogModel.blockNo),
                string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? "" : string.Format("and rt.Plate_No  like '%{0}%' ", ProductCatalogModel.plateNo),
                ProductCatalogModel.isXPC ? "" : "and UPPER(left(m.PC,1)) <> 'X'"
                );
            return db.Query<MasterDataQuery>(message).ToList();
        }

        public List<MasterDataQuery> GetProductCatalogNotop(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel)
        {
            var conditonRouting = string.IsNullOrEmpty(ProductCatalogModel.plateNo) && string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "and rt.Seq_No = 1 " : string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? " and isnull(rt.Block_No, '') <> '' " : " and isnull(rt.Plate_No, '') <> '' ";
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (ProductCatalogModel.FactoryCodeProduction == "[]")
            {
                ProductCatalogModel.FactoryCodeProduction = string.Empty;
            }
            else
            {
                string[] arr = JsonConvert.DeserializeObject<string[]>(ProductCatalogModel.FactoryCodeProduction);
                ProductCatalogModel.FactoryCodeProduction = string.Empty;

                foreach (var item in arr)
                {
                    ProductCatalogModel.FactoryCodeProduction = ProductCatalogModel.FactoryCodeProduction + "," + "'" + item + "'";
                }
                if (!string.IsNullOrEmpty(ProductCatalogModel.FactoryCodeProduction))
                {
                    ProductCatalogModel.FactoryCodeProduction = ProductCatalogModel.FactoryCodeProduction[1..];
                }
            }

            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    select
                    m.Material_No  as MaterialNo
                    ,m.FactoryCode
                    ,m.Part_No as PartNo
                    ,m.PC
                    ,m.Hierarchy
                    ,m.Sale_Org as SaleOrg
                    ,m.Plant
                    ,m.Cust_Code as CustCode
                    ,m.Cus_ID as CusID
	                ,m.Cust_Name as CustName
                    ,Trim(m.Description) as Description
                    ,(CASE WHEN LEN(m.Sale_Text1) > 0 THEN m.Sale_Text1 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text2) > 0 THEN m.Sale_Text2 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text3) > 0 THEN m.Sale_Text3 ELSE '' END) + (CASE WHEN LEN(m.Sale_Text4) > 0 THEN m.Sale_Text4 ELSE '' END) as SaleText
                    ,m.Change
                    ,m.Ind_Grp as IndGrp
                    ,m.Ind_Des as IndDes
                    ,m.Material_Type as MaterialType
                    ,m.Print_Method as PrintMethod
                    ,m.Flute
                    ,m.Board
                    ,m.Wid
                    ,m.Leg
                    ,m.Hig
                    ,m.Box_Type as BoxType
                    ,m.JoinType
                    ,m.CutSheetLeng
                    ,m.CutSheetWid
                    ,m.Bun
                    ,m.BunLayer
                    ,m.LayerPalet
                    ,m.BoxPalet
                    ,round(m.Weight_Box, 3) as WeightBox
                    ,m.Piece_Set as PieceSet
                    ,m.Sale_UOM as SaleUOM
                    ,m.BOM_UOM as BOMUOM
                    ,m.PalletSize
                    ,m.LastUpdate
                    ,(CASE WHEN LEN(m.Pur_Txt1) > 0 THEN m.Pur_Txt1 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt2) > 0 THEN m.Pur_Txt2 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt3) > 0 THEN m.Pur_Txt3 ELSE '' END) + (CASE WHEN LEN(m.Pur_Txt4) > 0 THEN m.Pur_Txt4 ELSE '' END) as PurTxt
                    ,m.High_Group as HighGroup
                    ,m.High_Value as HighValue
                    ,p.[validity start from] as ValidityStartFrom
                    ,p.[validity end date] as ValidityEndDate
                    ,p.rate as Rate
                    ,p.[currency key] as CurrencyKey
                    ,i.VENDORNAME as VendorName
                    ,i.NETPRICE as NetPrice
                    ,i.UNITOFNETPRICE as UnitOfNetPrice
                    ,i.SOURCELIST_VALIDFROM as SourceListValidFrom
                    ,i.SOURCELIST_VALIDTO as SourceListValidTo
                    ,r.Remark
                    ,r.NonMove
                    ,r.NonMoveMonth
                    ,r.StockWIP
                    ,r.StockFG
					,r.StockQA
	                ,(case Left(m.PC,1) when 'h' then (case h.isLocked when 1 then 'Hold' else '' end ) else '' end )  as Hold
					,m.Piece_Set as PieceSet
                    ,h.HoldRemark
                    ,m.CustInvType
                    ,m.CIPInvType
                    ,m.PDIS_Status PdisStatus
                    ,rt.Block_No BlockNo
                    ,rt.Plate_No PlateNo
                                            from MasterData m
                    left outer join Routing rt on rt.Material_No = m.Material_No and rt.FactoryCode = m.FactoryCode " + conditonRouting + @"
                    left outer join (select * from  Remark {0}) r on m.PC = r.PC
                    left outer join (select t.* from InfoRecordSourceList t
						inner join (
							select MATERIALNO, max(RecordedDate) as MaxDate
							from InfoRecordSourceList
							group by MATERIALNO
						) tm on t.MATERIALNO = tm.MATERIALNO and t.RecordedDate = tm.MaxDate {1} )  i
					on i.MATERIALNO = m.Material_No left outer join
                    (Select * from PricingMaster where  GETDATE() BETWEEN [validity start from] and [validity end date]) p
                    on p.[material no] = m.Material_No and p.[sales org] = m.Sale_Org
                    left outer join HoldMaterial h on  h.MaterialNo  = m.Material_No
                    where {2}
                    (
                        {3}
                        {4}
                        {5}
                        {6}
                        {7}
                        {8}
                    )
					and  isnull(m.Part_No,'') like '%{9}%'
                    and  isnull(m.Cust_Code,'') like '%{10}%'
                    and  isnull(m.Cus_ID,'')  like '%{11}%'
                    and  isnull(m.Cust_Name,'')  like '%{12}%'
                    and  isnull(m.Description,'')   like '%{13}%'
                    and  isnull(m.Sale_Text1,'') + isnull(m.Sale_Text2,'') + isnull(m.Sale_Text3,'') + isnull(m.Sale_Text4,'') like '%{14}%'
                    {15}
                    {16}
                    {17}
                    {18}
                    and  isnull(m.Board,'') like '%{19}%'
                    {20}
                    {21}
                    {22}
                    and  isnull(r.Remark , '') like '%{23}%'
                    and  isnull(m.Part_No , '') like '%{24}%'
                    {25}
                    {26}
                    {27}
                    {28}
                    {29}
                    and m.PDIS_Status <> 'X'
	                {37}
					and Trim(m.PC) <> ''
                    {30}
                    {31}
                    {32}
                    {33}
                    {34}
                    {35}
                    {36}
                    order by m.PC
                    ";
            string message = string.Format(sql,
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where t.FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where t.FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format(" m.FactoryCode in ({0}) and ", ProductCatalogModel.FactoryCode) : string.Format(" m.FactoryCode =  '{0}' and ", factory),
                ProductCatalogModel.pc1.Length <= 0 ? "m.PC like '%'" : string.Format(" m.PC like '%{0}%' ", ProductCatalogModel.pc1),
                ProductCatalogModel.pc2.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc2),
                ProductCatalogModel.pc3.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc3),
                ProductCatalogModel.pc4.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc4),
                ProductCatalogModel.pc5.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc5),
                ProductCatalogModel.pc6.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc6),
                ProductCatalogModel.partNo,
                ProductCatalogModel.custCode,
                ProductCatalogModel.custID,
                ProductCatalogModel.custName,
                ProductCatalogModel.desc,
                ProductCatalogModel.saleText,
                "",
                "",
                "",
                string.IsNullOrEmpty(ProductCatalogModel.flute) ? "" : string.Format("and m.Flute = '{0}'", ProductCatalogModel.flute),
                ProductCatalogModel.board,
                ProductCatalogModel.wid < 1 ? "" : string.Format("and m.Wid = {0}", ProductCatalogModel.wid),
                ProductCatalogModel.leg < 1 ? "" : string.Format("and m.leg = {0}", ProductCatalogModel.leg),
                ProductCatalogModel.hig < 1 ? "" : string.Format("and m.Hig = {0}", ProductCatalogModel.hig),
                ProductCatalogModel.remark,
                ProductCatalogModel.partNo,
                ProductCatalogModel.idNonMove != true ? "" : "and r.NonMove is not null",
                ProductCatalogModel.idStockWIP != true ? "" : "and r.StockWIP is not null",
                ProductCatalogModel.idStockQA != true ? "" : "and r.StockQA is not null",
                ProductCatalogModel.idStockFG != true ? "" : "and r.StockFG is not null",
                ProductCatalogModel.rate == -99 ? "" : string.Format("and p.rate = {0}", ProductCatalogModel.rate),
                string.IsNullOrEmpty(ProductCatalogModel.MaterialNo) ? "" : string.Format("and m.Material_No  like '%{0}%' ", ProductCatalogModel.MaterialNo),
                ProductCatalogModel.idHoldFind != true ? "" : "and h.isLocked = 1 and Left(m.PC, 1) = 'h'",
                string.IsNullOrEmpty(ProductCatalogModel.CustInvType) ? "" : string.Format("and m.CustInvType = '{0}'", ProductCatalogModel.CustInvType),
                string.IsNullOrEmpty(ProductCatalogModel.CIPInvType) ? "" : string.Format("and m.CIPInvType = '{0}'", ProductCatalogModel.CIPInvType),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCodeProduction) ? string.Empty : string.Format("and Sale_Org in ({0}) ", ProductCatalogModel.FactoryCodeProduction) : string.Empty,
                string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "" : string.Format("and rt.Block_No  like '%{0}%' ", ProductCatalogModel.blockNo),
                string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? "" : string.Format("and rt.Plate_No  like '%{0}%' ", ProductCatalogModel.plateNo),
                ProductCatalogModel.isXPC ? "" : "and UPPER(left(m.PC,1)) <> 'X'"
                );
            return db.Query<MasterDataQuery>(message).ToList();
        }

        public string GetCountProductCatalogNotop(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel)
        {
            var conditonRouting = string.IsNullOrEmpty(ProductCatalogModel.plateNo) && string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "and rt.Seq_No = 1 " : string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? " and isnull(rt.Block_No, '') <> '' " : " and isnull(rt.Plate_No, '') <> '' ";
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (ProductCatalogModel.FactoryCodeProduction == "[]")
            {
                ProductCatalogModel.FactoryCodeProduction = string.Empty;
            }
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"

                        select
                        count(*) as RecordCount
                        from MasterData m
                        left outer join Routing rt on rt.Material_No = m.Material_No and rt.FactoryCode = m.FactoryCode " + conditonRouting + @"
                        left outer join (select * from  Remark {0}) r on m.PC = r.PC
                        left outer join (select t.* from InfoRecordSourceList t
							inner join (
								select MATERIALNO, max(RecordedDate) as MaxDate
								from InfoRecordSourceList
								group by MATERIALNO
							) tm on t.MATERIALNO = tm.MATERIALNO and t.RecordedDate = tm.MaxDate
								{1} )  i
						 on i.MATERIALNO = m.Material_No left outer join
                         (Select * from PricingMaster where  GETDATE() BETWEEN [validity start from] and [validity end date]) p
                         on p.[material no] = m.Material_No and p.[sales org] = m.Sale_Org
                        left outer join HoldMaterial h on  h.MaterialNo  = m.Material_No
                        where {2}
                        (
                            {3}
                            {4}
                            {5}
                            {6}
                            {7}
                            {8}
                        )
						and  isnull(m.Part_No,'') like '%{9}%'
                        and  isnull(m.Cust_Code,'') like '%{10}%'
                        and  isnull(m.Cus_ID,'')  like '%{11}%'
                        and  isnull(m.Cust_Name,'')  like '%{12}%'
                        and  isnull(m.Description,'')   like '%{13}%'
                        and  isnull(m.Sale_Text1,'') + isnull(m.Sale_Text2,'') + isnull(m.Sale_Text3,'') + isnull(m.Sale_Text4,'') like '%{14}%'
                        {15}
                        {16}
                        {17}
                        {18}
                        and  isnull(m.Board,'') like '%{19}%'
                        {20}
                        {21}
                        {22}
                        and  isnull(r.Remark , '') like '%{23}%'
                        and  isnull(m.Part_No , '') like '%{24}%'
                        {25}
                        {26}
                        {27}
                        {28}
                        {29}
                        and m.PDIS_Status <> 'X'
                        {37}
						and Trim(m.PC) <> ''
                        {30}
                        {31}
                        {32}
                        {33}
                        {34}
                        {35}
                        {36}
                        ";
            string message = string.Format(sql,
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format("where t.FactoryCode in ({0}) ", ProductCatalogModel.FactoryCode) : string.Format("where t.FactoryCode =  '{0}' ", factory),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCode) ? "" : string.Format(" m.FactoryCode in ({0}) and ", ProductCatalogModel.FactoryCode) : string.Format(" m.FactoryCode =  '{0}' and ", factory),
                ProductCatalogModel.pc1.Length <= 0 ? "m.PC like '%'" : string.Format(" m.PC like '%{0}%' ", ProductCatalogModel.pc1),
                ProductCatalogModel.pc2.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc2),
                ProductCatalogModel.pc3.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc3),
                ProductCatalogModel.pc4.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc4),
                ProductCatalogModel.pc5.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc5),
                ProductCatalogModel.pc6.Length <= 0 ? "" : string.Format(" or m.PC like '%{0}%' ", ProductCatalogModel.pc6),
                ProductCatalogModel.partNo,
                ProductCatalogModel.custCode,
                ProductCatalogModel.custID,
                ProductCatalogModel.custName,
                ProductCatalogModel.desc,
                ProductCatalogModel.saleText,
                "",
                "",
                "",
                string.IsNullOrEmpty(ProductCatalogModel.flute) ? "" : string.Format("and m.Flute = '{0}'", ProductCatalogModel.flute),
                ProductCatalogModel.board,
                ProductCatalogModel.wid < 1 ? "" : string.Format("and m.Wid = {0}", ProductCatalogModel.wid),
                ProductCatalogModel.leg < 1 ? "" : string.Format("and m.leg = {0}", ProductCatalogModel.leg),
                ProductCatalogModel.hig < 1 ? "" : string.Format("and m.Hig = {0}", ProductCatalogModel.hig),
                ProductCatalogModel.remark,
                ProductCatalogModel.partNo,
                ProductCatalogModel.idNonMove != true ? "" : "and r.NonMove is not null",
                ProductCatalogModel.idStockWIP != true ? "" : "and r.StockWIP is not null",
                ProductCatalogModel.idStockQA != true ? "" : "and r.StockQA is not null",
                ProductCatalogModel.idStockFG != true ? "" : "and r.StockFG is not null",
                ProductCatalogModel.rate == -99 ? "" : string.Format("and p.rate = {0}", ProductCatalogModel.rate),
                string.IsNullOrEmpty(ProductCatalogModel.MaterialNo) ? "" : string.Format("and m.Material_No  like '%{0}%' ", ProductCatalogModel.MaterialNo),
                ProductCatalogModel.idHoldFind != true ? "" : "and h.isLocked = 1 and Left(m.PC, 1) = 'h'",
                string.IsNullOrEmpty(ProductCatalogModel.CustInvType) ? "" : string.Format("and m.CustInvType = '{0}'", ProductCatalogModel.CustInvType),
                string.IsNullOrEmpty(ProductCatalogModel.CIPInvType) ? "" : string.Format("and m.CIPInvType = '{0}'", ProductCatalogModel.CIPInvType),
                ProductCatalogModel.Role == "4" ? string.IsNullOrEmpty(ProductCatalogModel.FactoryCodeProduction) ? string.Empty : string.Format("and Sale_Org in ({0}) ", ProductCatalogModel.FactoryCodeProduction) : string.Empty,
                string.IsNullOrEmpty(ProductCatalogModel.blockNo) ? "" : string.Format("and rt.Block_No  like '%{0}%' ", ProductCatalogModel.blockNo),
                string.IsNullOrEmpty(ProductCatalogModel.plateNo) ? "" : string.Format("and rt.Plate_No  like '%{0}%' ", ProductCatalogModel.plateNo),
                ProductCatalogModel.isXPC ? "" : "and UPPER(left(m.PC,1)) <> 'X'"
                );
            return db.QueryFirstOrDefault<string>(message);
        }

        //// ---- Get Diecutpath -----
        ///
        public string GetMasterDataDiecutPath(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo).DiecutPictPath;
        }

        //// ---- Get palletpath -----
        ///
        public string GetMasterDataPalletPath(string factoryCode, string materialNo)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo).PalletizationPath;
        }

        public IEnumerable<MasterData> GetMasterDatasByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var masterDatas = new List<MasterData>();
            masterDatas.AddRange(PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && materialNos.Contains(m.MaterialNo)).AsNoTracking().ToList());

            return masterDatas;
        }

        public IEnumerable<MasterDataRoutingModel> GetReuseMasterDataRoutingsByMaterialNos(IConfiguration configuration, string factoryCode, List<string> materialNos)
        {
            //var masterDatas = new List<MasterData>();

            //foreach (var materialNo in materialNos)
            //{
            //    masterDatas.AddRange(PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.MaterialNo.Equals(materialNo) && m.PdisStatus == "X").AsNoTracking().ToList());
            //}

            //return masterDatas;

            var masterialNoArr = $"('{string.Join("','", [.. materialNos])}')";
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                               SELECT
                              m.Material_No as MaterialNo
                             ,m.PC
	                         ,m.Sale_Org as SaleOrg
	                         ,m.Plant
	                         ,m.Cust_Code as CustCode
	                         ,m.Cus_ID as CustID
	                         ,m.Cust_Name as CustName
	                         ,m.Description
	                         ,m.Flute + ' ' + m.Board as Board
	                         ,m.Box_Type as BoxType
	                         ,Machine = STUFF((SELECT TOP 4 machine + ', '
				                        FROM Routing r
				                        WHERE r.Material_No = m.Material_No and Upper(r.PDIS_Status) = 'X'  and r.FactoryCode = '{0}'
				                        ORDER BY r.Seq_No
				                        FOR XML PATH('')), 1, 0, '')
	                         ,m.LastUpdate
	                         ,m.CreateDate
	                         ,m.Tran_Status as TranStatus
	                         ,m.PDIS_Status as PDISStatus
							  FROM MasterData m where  Upper(m.PDIS_Status) = 'X'  and m.FactoryCode = '{1}' and m.Material_No IN {2} order by m.LastUpdate desc";
            string message = string.Format(sql, factoryCode, factoryCode, masterialNoArr);
            return db.Query<MasterDataRoutingModel>(message).ToList();
        }

        public void UpdateReuseMaterialNos(List<MasterData> masterDatas, List<Routing> routings, List<PlantView> plantViews, List<SalesView> salesViews, List<TransactionsDetail> transactionsDetails, string Username)
        {
            PMTsDbContext.MasterData.UpdateRange(masterDatas);
            foreach (var item in masterDatas)
            {
                UpdateIsTranfer(item.FactoryCode, item.MaterialNo, Username);
            }

            routings = routings.Where(r => r.Id > 0).ToList();
            PMTsDbContext.Routing.UpdateRange(routings);

            plantViews = plantViews.Where(r => r.Id > 0).ToList();
            PMTsDbContext.PlantView.UpdateRange(plantViews);

            salesViews = salesViews.Where(r => r.Id > 0).ToList();
            PMTsDbContext.SalesView.UpdateRange(salesViews);

            transactionsDetails = transactionsDetails.Where(r => r != null && r.Id > 0).ToList();
            PMTsDbContext.TransactionsDetail.UpdateRange(transactionsDetails);

            PMTsDbContext.SaveChanges();
        }

        public IEnumerable<MasterData> GetReuseMasterDatasByMaterialNos(IConfiguration configuration, string factoryCode, List<string> materialNos)
        {
            var masterialNoArr = $"('{string.Join("','", [.. materialNos])}')";
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                SELECT [Id]
		        ,[FactoryCode]
		        ,[Material_No] as MaterialNo
		        ,[Part_No] as PartNo
		        ,[PC]
		        ,[Hierarchy]
		        ,[Sale_Org] as SaleOrg
		        ,[Plant]
		        ,[Cust_Code] as CustCode
		        ,[Cus_ID] as CusId
		        ,[Cust_Name] as CustName
		        ,[Description]
		        ,[Sale_Text1] as SaleText1
		        ,[Sale_Text2] as SaleText2
		        ,[Sale_Text3] as SaleText3
		        ,[Sale_Text4] as SaleText4
		        ,[Change]
		        ,[Language]
		        ,[Ind_Grp] as IndGrp
		        ,[Ind_Des] as IndDes
		        ,[Material_Type] as MaterialType
		        ,[Print_Method] as PrintMethod
		        ,[TwoPiece]
		        ,[Flute]
		        ,[Code]
		        ,[Board]
		        ,[GL]
		        ,[GLWeigth]
		        ,[BM]
		        ,[BMWeigth]
		        ,[BL]
		        ,[BLWeigth]
		        ,[CM]
		        ,[CMWeigth]
		        ,[CL]
		        ,[CLWeigth]
		        ,[DM]
		        ,[DMWeigth]
		        ,[DL]
		        ,[DLWeigth]
		        ,[Wid]
		        ,[Leg]
		        ,[Hig]
		        ,[Box_Type] as BoxType
		        ,[RSC_Style] as RscStyle
		        ,[Pro_Type] as ProType
		        ,[JoinType]
		        ,[Status_Flag] as StatusFlag
		        ,[Priority_Flag] as PriorityFlag
		        ,[Wire]
		        ,[Outer_Join] as OuterJoin
		        ,[CutSheetLeng]
		        ,[CutSheetWid]
		        ,[Sheet_Area] as SheetArea
		        ,[Box_Area] as BoxArea
		        ,[ScoreW1]
		        ,[Scorew2]
		        ,[Scorew3]
		        ,[Scorew4]
		        ,[Scorew5]
		        ,[Scorew6]
		        ,[Scorew7]
		        ,[Scorew8]
		        ,[Scorew9]
		        ,[Scorew10]
		        ,[Scorew11]
		        ,[Scorew12]
		        ,[Scorew13]
		        ,[Scorew14]
		        ,[Scorew15]
		        ,[Scorew16]
		        ,[JointLap]
		        ,[ScoreL2]
		        ,[ScoreL3]
		        ,[ScoreL4]
		        ,[ScoreL5]
		        ,[ScoreL6]
		        ,[ScoreL7]
		        ,[ScoreL8]
		        ,[ScoreL9]
		        ,[Slit]
		        ,[No_Slot] as NoSlot
		        ,[Bun]
		        ,[BunLayer]
		        ,[LayerPalet]
		        ,[BoxPalet]
		        ,[Weight_Sh] as WeightSh
		        ,[Weight_Box] as WeightBox
		        ,[SparePercen]
		        ,[SpareMax]
		        ,[SpareMin]
		        ,[LeadTime]
		        ,[Piece_Set] as PieceSet
		        ,[Sale_UOM] as SaleUom
		        ,[BOM_UOM] as BomUom
		        ,[Hardship]
		        ,[PalletSize]
		        ,[Palletization_Path] as PalletizationPath
		        ,[PrintMaster_Path] as PrintMasterPath
		        ,[DiecutPict_Path] as DiecutPictPath
		        ,[FGPic_Path] as FgpicPath
		        ,[CreateDate]
		        ,[CreatedBy]
		        ,[LastUpdate]
		        ,[UpdatedBy]
		        ,[User]
		        ,[Plt_Leg_Double] as PltLegDouble
		        ,[Plt_Double_axle] as PltDoubleAxle
		        ,[Plt_Leg_Single]  as PltLegSingle
		        ,[Plt_Single_axle] as PltSingleAxle
		        ,[Plt_Floor_Above] as PltFloorAbove
		        ,[Plt_Floor_Under] as PltFloorUnder
		        ,[Plt_Beam] as PltBeam
		        ,[Plt_Axle_Height] as PltAxleHeight
		        ,[EanCode]
		        ,[PDIS_Status] as PdisStatus
		        ,[Tran_Status] as TranStatus
		        ,[SAP_Status]  as SapStatus
		        ,[NewH]
		        ,[Pur_Txt1] as PurTxt1
		        ,[Pur_Txt2] as PurTxt2
		        ,[Pur_Txt3] as PurTxt3
		        ,[Pur_Txt4] as PurTxt4
		        ,[UnUpgrad_Board] as UnUpgradBoard
		        ,[High_Group] as HighGroup
		        ,[High_Value] as HighValue
		        ,[ChangeInfo] as ChangeInfo
		        ,[Piece_Patch] as PiecePatch
		        ,[BoxHandle]
		        ,[PSM_ID] as PsmId
		        ,[PicPallet]
		        ,[ChangeHistory]
		        ,[CustComment]
		        ,[MaterialComment]
		        ,[CutSheetWid_Inch] as CutSheetWidInch
		        ,[CutSheetLeng_Inch] as CutSheetLengInch
		        ,[Joint_ID] as JointId
		        ,[CustInvType]
		        ,[CIPInvType]
                ,[HireFactory]
                ,[SizeDimensions]
		        FROM [dbo].[MasterData] m where  Upper(m.PDIS_Status) = 'X'  and m.FactoryCode = '{0}' and m.Material_No IN {1} order by m.LastUpdate desc";
            string message = string.Format(sql, factoryCode, masterialNoArr);
            return db.Query<MasterData>(message).ToList();
        }

        public void UpdateProductCodeAndDescriptionFromPresaleNewMat(IConfiguration configuration, string factoryCode, string productCode, string description, string materialOriginal, string Username)
        {
            #region Get All Probability Of Description

            var resDesc = new List<string>();
            if (!string.IsNullOrEmpty(description))
            {
                var lenghtDesc = description.Length;
                for (int i = lenghtDesc; i <= lenghtDesc + 40; i++)
                {
                    if (description.Length >= 40)
                    {
                        description = description[..39];
                    }

                    if (i > 40)
                    {
                        description = description.PadLeft(40, 'x');
                    }
                    else
                    {
                        description = description.PadLeft(i, 'x');
                    }

                    resDesc.Add(description);
                    if (description == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
                    {
                        break;
                    }
                }
            }

            #endregion Get All Probability Of Description

            #region Get All Probability Of ProductCode

            var resPc = new List<string>();
            if (!string.IsNullOrEmpty(productCode))
            {
                var lenghtPc = productCode.Length;
                for (int i = lenghtPc; i <= lenghtPc + 20; i++)
                {
                    if (productCode.Length >= 20)
                    {
                        productCode = productCode[..19];
                    }

                    if (i > 20)
                    {
                        productCode = productCode.PadLeft(20, 'x');
                    }
                    else
                    {
                        productCode = productCode.PadLeft(i, 'x');
                    }
                    resPc.Add(productCode);
                    if (productCode == "xxxxxxxxxxxxxxxxxxxx")
                    {
                        break;
                    }
                }
            }

            #endregion Get All Probability Of ProductCode

            var descArr = $"('{string.Join("','", [.. resDesc])}')";
            var pcArr = $"('{string.Join("','", [.. resPc])}')";

            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            #region sql str pc

            string sql = @"SELECT [Id]
                    ,[FactoryCode]
                    ,[Material_No] as MaterialNo
                    ,[Part_No] as PartNo
                    ,[PC]
                    ,[Hierarchy]
                    ,[Sale_Org] as SaleOrg
                    ,[Plant]
                    ,[Cust_Code] as CustCode
                    ,[Cus_ID] as CusId
                    ,[Cust_Name] as CustName
                    ,[Description]
                    ,[Sale_Text1] as SaleText1
                    ,[Sale_Text2] as SaleText2
                    ,[Sale_Text3] as SaleText3
                    ,[Sale_Text4] as SaleText4
                    ,[Change]
                    ,[Language]
                    ,[Ind_Grp] as IndGrp
                    ,[Ind_Des] as IndDes
                    ,[Material_Type] as MaterialType
                    ,[Print_Method] as PrintMethod
                    ,[TwoPiece]
                    ,[Flute]
                    ,[Code]
                    ,[Board]
                    ,[GL]
                    ,[GLWeigth]
                    ,[BM]
                    ,[BMWeigth]
                    ,[BL]
                    ,[BLWeigth]
                    ,[CM]
                    ,[CMWeigth]
                    ,[CL]
                    ,[CLWeigth]
                    ,[DM]
                    ,[DMWeigth]
                    ,[DL]
                    ,[DLWeigth]
                    ,[Wid]
                    ,[Leg]
                    ,[Hig]
                    ,[Box_Type] as BoxType
                    ,[RSC_Style] as RscStyle
                    ,[Pro_Type] as ProType
                    ,[JoinType]
                    ,[Status_Flag] as StatusFlag
                    ,[Priority_Flag] as PriorityFlag
                    ,[Wire]
                    ,[Outer_Join] as OuterJoin
                    ,[CutSheetLeng]
                    ,[CutSheetWid]
                    ,[Sheet_Area] as SheetArea
                    ,[Box_Area] as BoxArea
                    ,[ScoreW1]
                    ,[Scorew2]
                    ,[Scorew3]
                    ,[Scorew4]
                    ,[Scorew5]
                    ,[Scorew6]
                    ,[Scorew7]
                    ,[Scorew8]
                    ,[Scorew9]
                    ,[Scorew10]
                    ,[Scorew11]
                    ,[Scorew12]
                    ,[Scorew13]
                    ,[Scorew14]
                    ,[Scorew15]
                    ,[Scorew16]
                    ,[JointLap]
                    ,[ScoreL2]
                    ,[ScoreL3]
                    ,[ScoreL4]
                    ,[ScoreL5]
                    ,[ScoreL6]
                    ,[ScoreL7]
                    ,[ScoreL8]
                    ,[ScoreL9]
                    ,[Slit]
                    ,[No_Slot] as NoSlot
                    ,[Bun]
                    ,[BunLayer]
                    ,[LayerPalet]
                    ,[BoxPalet]
                    ,[Weight_Sh] as WeightSh
                    ,[Weight_Box] as WeightBox
                    ,[SparePercen]
                    ,[SpareMax]
                    ,[SpareMin]
                    ,[LeadTime]
                    ,[Piece_Set] as PieceSet
                    ,[Sale_UOM] as SaleUom
                    ,[BOM_UOM] as BomUom
                    ,[Hardship]
                    ,[PalletSize]
                    ,[Palletization_Path] as PalletizationPath
                    ,[PrintMaster_Path] as PrintMasterPath
                    ,[DiecutPict_Path] as DiecutPictPath
                    ,[FGPic_Path] as FgpicPath
                    ,[CreateDate]
                    ,[CreatedBy]
                    ,[LastUpdate]
                    ,[UpdatedBy]
                    ,[User]
                    ,[Plt_Leg_Double] as PltLegDouble
                    ,[Plt_Double_axle] as PltDoubleAxle
                    ,[Plt_Leg_Single]  as PltLegSingle
                    ,[Plt_Single_axle] as PltSingleAxle
                    ,[Plt_Floor_Above] as PltFloorAbove
                    ,[Plt_Floor_Under] as PltFloorUnder
                    ,[Plt_Beam] as PltBeam
                    ,[Plt_Axle_Height] as PltAxleHeight
                    ,[EanCode]
                    ,[PDIS_Status] as PdisStatus
                    ,[Tran_Status] as TranStatus
                    ,[SAP_Status]  as SapStatus
                    ,[NewH]
                    ,[Pur_Txt1] as PurTxt1
                    ,[Pur_Txt2] as PurTxt2
                    ,[Pur_Txt3] as PurTxt3
                    ,[Pur_Txt4] as PurTxt4
                    ,[UnUpgrad_Board] as UnUpgradBoard
                    ,[High_Group] as HighGroup
                    ,[High_Value] as HighValue
                    ,[ChangeInfo] as ChangeInfo
                    ,[Piece_Patch] as PiecePatch
                    ,[BoxHandle]
                    ,[PSM_ID] as PsmId
                    ,[PicPallet]
                    ,[ChangeHistory]
                    ,[CustComment]
                    ,[MaterialComment]
                    ,[CutSheetWid_Inch] as CutSheetWidInch
                    ,[CutSheetLeng_Inch] as CutSheetLengInch
                    ,[Joint_ID] as JointId
                    ,[CustInvType]
                    ,[CIPInvType]
                    FROM [dbo].[MasterData] m
                    WHERE  m.PC IN {0} AND m.FactoryCode = '{1}'";

            #endregion sql str pc

            string message = string.Format(sql, pcArr, factoryCode);
            var masterdatasUpdatePC = db.Query<MasterData>(message).ToList();

            #region sql str desc

            sql = @"SELECT [Id]
                    ,[FactoryCode]
                    ,[Material_No] as MaterialNo
                    ,[Part_No] as PartNo
                    ,[PC]
                    ,[Hierarchy]
                    ,[Sale_Org] as SaleOrg
                    ,[Plant]
                    ,[Cust_Code] as CustCode
                    ,[Cus_ID] as CusId
                    ,[Cust_Name] as CustName
                    ,[Description]
                    ,[Sale_Text1] as SaleText1
                    ,[Sale_Text2] as SaleText2
                    ,[Sale_Text3] as SaleText3
                    ,[Sale_Text4] as SaleText4
                    ,[Change]
                    ,[Language]
                    ,[Ind_Grp] as IndGrp
                    ,[Ind_Des] as IndDes
                    ,[Material_Type] as MaterialType
                    ,[Print_Method] as PrintMethod
                    ,[TwoPiece]
                    ,[Flute]
                    ,[Code]
                    ,[Board]
                    ,[GL]
                    ,[GLWeigth]
                    ,[BM]
                    ,[BMWeigth]
                    ,[BL]
                    ,[BLWeigth]
                    ,[CM]
                    ,[CMWeigth]
                    ,[CL]
                    ,[CLWeigth]
                    ,[DM]
                    ,[DMWeigth]
                    ,[DL]
                    ,[DLWeigth]
                    ,[Wid]
                    ,[Leg]
                    ,[Hig]
                    ,[Box_Type] as BoxType
                    ,[RSC_Style] as RscStyle
                    ,[Pro_Type] as ProType
                    ,[JoinType]
                    ,[Status_Flag] as StatusFlag
                    ,[Priority_Flag] as PriorityFlag
                    ,[Wire]
                    ,[Outer_Join] as OuterJoin
                    ,[CutSheetLeng]
                    ,[CutSheetWid]
                    ,[Sheet_Area] as SheetArea
                    ,[Box_Area] as BoxArea
                    ,[ScoreW1]
                    ,[Scorew2]
                    ,[Scorew3]
                    ,[Scorew4]
                    ,[Scorew5]
                    ,[Scorew6]
                    ,[Scorew7]
                    ,[Scorew8]
                    ,[Scorew9]
                    ,[Scorew10]
                    ,[Scorew11]
                    ,[Scorew12]
                    ,[Scorew13]
                    ,[Scorew14]
                    ,[Scorew15]
                    ,[Scorew16]
                    ,[JointLap]
                    ,[ScoreL2]
                    ,[ScoreL3]
                    ,[ScoreL4]
                    ,[ScoreL5]
                    ,[ScoreL6]
                    ,[ScoreL7]
                    ,[ScoreL8]
                    ,[ScoreL9]
                    ,[Slit]
                    ,[No_Slot] as NoSlot
                    ,[Bun]
                    ,[BunLayer]
                    ,[LayerPalet]
                    ,[BoxPalet]
                    ,[Weight_Sh] as WeightSh
                    ,[Weight_Box] as WeightBox
                    ,[SparePercen]
                    ,[SpareMax]
                    ,[SpareMin]
                    ,[LeadTime]
                    ,[Piece_Set] as PieceSet
                    ,[Sale_UOM] as SaleUom
                    ,[BOM_UOM] as BomUom
                    ,[Hardship]
                    ,[PalletSize]
                    ,[Palletization_Path] as PalletizationPath
                    ,[PrintMaster_Path] as PrintMasterPath
                    ,[DiecutPict_Path] as DiecutPictPath
                    ,[FGPic_Path] as FgpicPath
                    ,[CreateDate]
                    ,[CreatedBy]
                    ,[LastUpdate]
                    ,[UpdatedBy]
                    ,[User]
                    ,[Plt_Leg_Double] as PltLegDouble
                    ,[Plt_Double_axle] as PltDoubleAxle
                    ,[Plt_Leg_Single]  as PltLegSingle
                    ,[Plt_Single_axle] as PltSingleAxle
                    ,[Plt_Floor_Above] as PltFloorAbove
                    ,[Plt_Floor_Under] as PltFloorUnder
                    ,[Plt_Beam] as PltBeam
                    ,[Plt_Axle_Height] as PltAxleHeight
                    ,[EanCode]
                    ,[PDIS_Status] as PdisStatus
                    ,[Tran_Status] as TranStatus
                    ,[SAP_Status]  as SapStatus
                    ,[NewH]
                    ,[Pur_Txt1] as PurTxt1
                    ,[Pur_Txt2] as PurTxt2
                    ,[Pur_Txt3] as PurTxt3
                    ,[Pur_Txt4] as PurTxt4
                    ,[UnUpgrad_Board] as UnUpgradBoard
                    ,[High_Group] as HighGroup
                    ,[High_Value] as HighValue
                    ,[ChangeInfo] as ChangeInfo
                    ,[Piece_Patch] as PiecePatch
                    ,[BoxHandle]
                    ,[PSM_ID] as PsmId
                    ,[PicPallet]
                    ,[ChangeHistory]
                    ,[CustComment]
                    ,[MaterialComment]
                    ,[CutSheetWid_Inch] as CutSheetWidInch
                    ,[CutSheetLeng_Inch] as CutSheetLengInch
                    ,[Joint_ID] as JointId
                    ,[CustInvType]
                    ,[CIPInvType]
                    FROM [dbo].[MasterData] m
                    WHERE m.Description IN {0} AND m.FactoryCode = '{1}'";

            #endregion sql str desc

            message = string.Format(sql, descArr, factoryCode);
            var masterdatasUpdateDesc = db.Query<MasterData>(message).ToList();

            if (!string.IsNullOrEmpty(materialOriginal))
            {
                masterdatasUpdatePC = masterdatasUpdatePC.Where(m => m.MaterialNo != materialOriginal).ToList();
                masterdatasUpdateDesc = masterdatasUpdateDesc.Where(m => m.MaterialNo != materialOriginal).ToList();
            }

            foreach (var masterdata in masterdatasUpdateDesc)
            {
                if (masterdata.Description.Length >= 40)
                {
                    masterdata.Description = string.Concat('x', masterdata.Description[..39]);
                }
                else
                {
                    masterdata.Description = 'x' + masterdata.Description;
                }

                var existPc = masterdatasUpdatePC.FirstOrDefault(m => m.Id == masterdata.Id);

                if (existPc != null)
                {
                    if (masterdata.Pc.Length >= 20)
                    {
                        masterdata.Pc = string.Concat('x', masterdata.Pc[..19]);
                    }
                    else
                    {
                        masterdata.Pc = 'x' + masterdata.Pc;
                    }
                }

                PMTsDbContext.MasterData.Update(masterdata);
                UpdateIsTranfer(masterdata.FactoryCode, masterdata.MaterialNo, Username);
            }

            foreach (var masterdata in masterdatasUpdatePC)
            {
                var existDesc = masterdatasUpdateDesc.FirstOrDefault(m => m.Id == masterdata.Id);

                if (existDesc == null)
                {
                    if (masterdata.Pc.Length >= 20)
                    {
                        masterdata.Pc = string.Concat('x', masterdata.Pc[..19]);
                    }
                    else
                    {
                        masterdata.Pc = 'x' + masterdata.Pc;
                    }
                    PMTsDbContext.MasterData.Update(masterdata);
                    UpdateIsTranfer(masterdata.FactoryCode, masterdata.MaterialNo, Username);
                }
            }

            PMTsDbContext.SaveChanges();

            db.Close();
        }

        //Update After Add and Update Masterdata IsTranstatus
        public void UpdateIsTranfer(string FactoryCode, string MaterialNo, string Username)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo).ToList();
                // some.ForEach(a => a.IsTransfer = Status == "1" ? true : false);
                some.ForEach(a => a.IsTransfer = false);
                some.ForEach(a => a.UpdatedBy = Username);
                some.ForEach(a => a.LastUpdate = DateTime.Now);
                // some.ForEach(a => a.SapStatus = Status);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ChangeBoardNewMaterial> CreateChangeBoardNewMaterial(SqlConnection conn, string factoryCode, string username, bool IsCheckImport, List<ChangeBoardNewMaterial> changeBoardNewMaterials)
        {
            var results = new List<ChangeBoardNewMaterial>();
            var validDatas = new List<ChangeBoardNewMaterial>();
            var isSaveSuccess = true;
            var unitOfWork = new UnitOfWork(PMTsDbContext);

            PmtsConfig pmtsConfig = PMTsDbContext.PmtsConfig.Where(p => p.FactoryCode == factoryCode && p.FucName == "Mintrim").FirstOrDefault();
            var RollWidth = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == factoryCode).OrderBy(o => o.Width).ToList();
            var Grade = PMTsDbContext.PaperGrade.Where(g => g.Active == true).ToList();

            //check number of change datas
            if (changeBoardNewMaterials != null && changeBoardNewMaterials.Count > 0)
            {
                results.Clear();
                results = changeBoardNewMaterials.Where(c => !c.IsCreatedSuccess).ToList();
                validDatas = changeBoardNewMaterials.Where(c => c.IsCreatedSuccess).ToList();

                #region Copy new material

                foreach (var validData in validDatas)
                {
                    try
                    {
                        #region initial param

                        var pdisStatus = "N";
                        var resultCount = 0;

                        var newChangeBoardNewMaterial = new ChangeBoardNewMaterial();
                        var originalMasterdata = new MasterData();
                        var newMasterdata = new MasterData();
                        //var originalRoutings = new List<Routing>();
                        var newRoutings = new List<Routing>();
                        var originalSaleViews = new List<SalesView>();
                        var originalPlantView = new PlantView();
                        var originalTransactionDetail = new TransactionsDetail();
                        var newBoardUse = new BoardUse();
                        var originalBomStructs = new List<BomStruct>();
                        var errorMassage = string.Empty;
                        var newMaterialNo = string.Empty;
                        var newHierarchy = string.Empty;
                        var machineInUse = string.Empty;
                        var flute = new Flute();
                        var board = new BoardCombine();
                        var boards = new List<BoardCombine>();
                        var company = new CompanyProfile();
                        var runningList = new List<RunningNo>();
                        var hvaMasters = new List<HvaMaster>();
                        var hvaParam = new HvaMaster();
                        var boardAlternative = new BoardAlternative();
                        var cost = new Cost();
                        var mapCost = new MapCost();
                        double? basicWeight = 0;
                        var mapcostRepositoyry = new MapCostRepository(PMTsDbContext);
                        var boardCombineAccRepositoyry = new BoardCombineAccRepository(PMTsDbContext);
                        var formulaRepository = new FormulaRepository(PMTsDbContext);
                        var delimiter = ",";
                        isSaveSuccess = true;

                        #endregion initial param

                        #region original data

                        var copyMaterialNo = !string.IsNullOrEmpty(validData.CopyMaterialNo) ? validData.CopyMaterialNo.Trim() : validData.CopyMaterialNo;
                        company = PMTsDbContext.CompanyProfile.AsNoTracking().FirstOrDefault(c => c.Plant == factoryCode);
                        originalMasterdata = PMTsDbContext.MasterData.AsNoTracking().FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == copyMaterialNo && m.SaleOrg == company.SaleOrg);
                        var originalRoutings = PMTsDbContext.Routing.AsNoTracking().Where(r => r.FactoryCode == factoryCode && r.MaterialNo == copyMaterialNo).OrderBy(r => r.SeqNo).ToList();
                        originalSaleViews =
                        [
                            .. PMTsDbContext.SalesView.AsNoTracking().Where(s => s.FactoryCode == factoryCode && s.MaterialNo == copyMaterialNo).AsNoTracking(),
                        ];
                        originalPlantView = PMTsDbContext.PlantView.AsNoTracking().FirstOrDefault(p => p.FactoryCode == factoryCode && p.MaterialNo == copyMaterialNo);
                        originalTransactionDetail = PMTsDbContext.TransactionsDetail.AsNoTracking().FirstOrDefault(t => t.FactoryCode == factoryCode && t.MaterialNo == copyMaterialNo);//&& (!m.Outsource && string.IsNullOrEmpty(m.MatSaleOrg)
                        hvaMasters = [.. PMTsDbContext.HvaMaster.AsNoTracking()];

                        #endregion original data

                        //set pdis status from routing state
                        if (originalRoutings != null && originalRoutings.Count > 0)
                        {
                            pdisStatus = "C";
                        }

                        //calculate basic weight
                        if (originalMasterdata != null)
                        {
                            //check board with new code
                            boards = [.. PMTsDbContext.BoardCombine.Where(b => b.Board == validData.NewBoard && b.Flute == validData.Flute && b.Status == true)];
                            if (boards != null && boards.Count == 1)
                            {
                                board = boards.FirstOrDefault();
                                basicWeight = GetBasisWeight(board.Code, originalMasterdata.Flute, factoryCode);
                                flute = PMTsDbContext.Flute.Where(f => f.Flute1 == board.Flute && f.FactoryCode == factoryCode).FirstOrDefault();
                            }
                            else if (boards.Count == 0)
                            {
                                board = null;
                            }

                            newMaterialNo = GenMatNo(originalMasterdata.MaterialType, factoryCode);

                            mapCost = mapcostRepositoyry.GetCostField(originalMasterdata.Hierarchy.Substring(2, 2), originalMasterdata.Hierarchy.Substring(3, 3), originalMasterdata.Hierarchy.Substring(7, 3));
                            cost = board == null || mapCost == null ? null : boardCombineAccRepositoyry.GetCost(conn, factoryCode, board.Code, mapCost.CostField);

                            if (!string.IsNullOrEmpty(validData.HighValue))
                            {
                                hvaParam = hvaMasters.FirstOrDefault(h => h.HighValue == validData.HighValue);
                            }
                        }

                        var activeBoards = boards.Where(b => b.Status.HasValue && b.Status.Value).ToList();

                        #region case error message

                        if (originalMasterdata == null)
                        {
                            errorMassage = $"Can't find material no. {copyMaterialNo}.";
                            isSaveSuccess = false;
                        }
                        else if (company == null)
                        {
                            errorMassage = $"Don't have a company registration.";
                            isSaveSuccess = false;
                        }
                        else if (originalMasterdata != null && (validData.Flute != originalMasterdata.Flute))
                        {
                            errorMassage = $"Invalid Flute.";
                            isSaveSuccess = false;
                        }
                        else if (mapCost == null)
                        {
                            errorMassage = $"Can't find mapcost from your hierarchy..";
                            isSaveSuccess = false;
                        }
                        else if (board == null)
                        {
                            errorMassage = $"Can't find board {validData.NewBoard} and flute {validData.Flute}.";
                            isSaveSuccess = false;
                        }
                        else if (activeBoards == null || activeBoards.Count > 1)
                        {
                            errorMassage = $"Find duplicate {activeBoards.Count} row in board combine.";
                            isSaveSuccess = false;
                        }
                        else if (basicWeight == null)
                        {
                            errorMassage = $"Can't calculate basic weight for {copyMaterialNo}.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(validData.HighValue) && hvaParam == null)
                        {
                            errorMassage = $"Can't create new material form high value {validData.HighValue}.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(newMaterialNo))
                        {
                            errorMassage = "Can't genarate new material number.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(originalMasterdata.Hierarchy))
                        {
                            errorMassage = "Invalid hierarchy in original material.";
                            isSaveSuccess = false;
                        }
                        else if (cost == null || cost.CostPerTon == 0)
                        {
                            errorMassage = "Invalid Cost value 0.";
                            isSaveSuccess = false;
                        }
                        else if (originalSaleViews == null || originalSaleViews.Count == 0)
                        {
                            errorMassage = "Old material without saleviews.";
                            //isSaveSuccess = false;
                        }
                        else if (originalPlantView == null)
                        {
                            errorMassage = "Old material without plantview.";
                            //isSaveSuccess = false;
                        }
                        else if (board != null && (originalMasterdata.Board == validData.NewBoard))
                        {
                            errorMassage = $"Are you sure? To change board {originalMasterdata.Board} to {validData.NewBoard}.";
                            //errorMassage = $" Can't change board {originalMasterdata.Board} to board {validData.NewBoard}.";
                            //isSaveSuccess = false;
                        }

                        #endregion case error message

                        if (isSaveSuccess)
                        {
                            if (IsCheckImport)
                            {
                                #region Sent result

                                //set new created data
                                newChangeBoardNewMaterial = new ChangeBoardNewMaterial
                                {
                                    IsCreatedSuccess = true,
                                    ErrorMessage = errorMassage,
                                    MaterialNo = originalMasterdata.MaterialNo,
                                    CopyMaterialNo = originalMasterdata.MaterialNo,
                                    PC = validData.PC,
                                    Flute = validData.Flute,
                                    NewBoard = validData.NewBoard,
                                    Change = validData.Change,
                                    Price = validData.Price,
                                    HighValue = validData.HighValue,
                                    CodeNewBoard = boards.Select(b => b.Code).ToList().Aggregate((i, j) => i + delimiter + j),
                                    Cost = cost != null ? cost.CostPerTon : 0,
                                    BoardAlternative = validData.BoardAlternative
                                };

                                results.Add(newChangeBoardNewMaterial);

                                #endregion Sent result
                            }
                            else
                            {
                                originalBomStructs = [.. PMTsDbContext.BomStruct.Where(bs => bs.FactoryCode == factoryCode && bs.Follower == originalMasterdata.MaterialNo)];

                                originalMasterdata.SheetArea = originalMasterdata.SheetArea == 0 ? originalMasterdata.CutSheetWid * originalMasterdata.CutSheetLeng : originalMasterdata.SheetArea;
                                var slot = Convert.ToInt32((originalMasterdata.CutSheetWid - originalMasterdata.Scorew2) * (21.5 + originalMasterdata.JointLap) + (originalMasterdata.Slit * originalMasterdata.CutSheetWid));
                                originalMasterdata.BoxArea = originalMasterdata.BoxArea == 0 ? originalMasterdata.SheetArea - slot : originalMasterdata.BoxArea;
                                var weightBox = Convert.ToDouble(basicWeight * originalMasterdata.BoxArea / 1000000000);
                                var weightSh = Convert.ToDouble(basicWeight * originalMasterdata.SheetArea / 1000000000);
                                var sumChange = (validData.Change + " " + originalMasterdata.Change).Length > 100 ? (validData.Change + " " + originalMasterdata.Change)[..100] : validData.Change + " " + originalMasterdata.Change;
                                var hvaCodeFromHighValue = PMTsDbContext.HvaMaster.FirstOrDefault(h => h.HighValue.Equals(validData.HighValue));
                                var hva = hvaCodeFromHighValue != null ? hvaCodeFromHighValue.HighValue : string.Empty;

                                #region set new masterdata

                                var eanCode = GetEanCode(newMaterialNo, company.SaleOrg);
                                newMasterdata = new MasterData
                                {
                                    FactoryCode = factoryCode,
                                    Plant = factoryCode,
                                    MaterialNo = newMaterialNo,
                                    Board = board.Board,
                                    Code = board.Code,
                                    PdisStatus = pdisStatus,
                                    SapStatus = false,
                                    TranStatus = false,
                                    User = "ZZ_Import",
                                    LastUpdate = DateTime.Now,
                                    UpdatedBy = username,
                                    CreateDate = DateTime.Now,
                                    CreatedBy = username,
                                    HighValue = hva,
                                    WeightBox = Convert.ToDouble(basicWeight * originalMasterdata.BoxArea / 1000000000),
                                    WeightSh = Convert.ToDouble(basicWeight * originalMasterdata.SheetArea / 1000000000),
                                    Hierarchy = new string(originalMasterdata.Hierarchy.Take(10).ToArray()) + validData.CodeNewBoard,
                                    IsTransfer = false,
                                    UnUpgradBoard = originalMasterdata.UnUpgradBoard,
                                    AttachFileMoPath = originalMasterdata.AttachFileMoPath,
                                    Bl = originalMasterdata.Bl,
                                    Blweigth = originalMasterdata.Blweigth,
                                    Bm = originalMasterdata.Bm,
                                    Bmweigth = originalMasterdata.Bmweigth,
                                    BomUom = originalMasterdata.BomUom,
                                    BoxArea = originalMasterdata.BoxArea,
                                    BoxHandle = originalMasterdata.BoxHandle,
                                    BoxPalet = originalMasterdata.BoxPalet,
                                    BoxType = originalMasterdata.BoxType,
                                    Bun = originalMasterdata.Bun,
                                    BunLayer = originalMasterdata.BunLayer,
                                    Change = !string.IsNullOrEmpty(validData.Change) ? sumChange : originalMasterdata.Change,
                                    ChangeHistory = originalMasterdata.ChangeHistory,
                                    ChangeInfo = originalMasterdata.ChangeInfo,
                                    CipinvType = originalMasterdata.CipinvType,
                                    Cl = originalMasterdata.Cl,
                                    Clweigth = originalMasterdata.Clweigth,
                                    Cm = originalMasterdata.Cm,
                                    Cmweigth = originalMasterdata.Cmweigth,
                                    CusId = originalMasterdata.CusId,
                                    CustCode = originalMasterdata.CustCode,
                                    CustComment = originalMasterdata.CustComment,
                                    CustInvType = originalMasterdata.CustInvType,
                                    CutSheetLeng = originalMasterdata.CutSheetLeng,
                                    CustName = originalMasterdata.CustName,
                                    CutSheetLengInch = originalMasterdata.CutSheetLengInch,
                                    CutSheetWid = originalMasterdata.CutSheetWid,
                                    Description = originalMasterdata.Description,
                                    CutSheetWidInch = originalMasterdata.CutSheetWidInch,
                                    DiecutPictPath = originalMasterdata.DiecutPictPath,
                                    Dl = originalMasterdata.Dl,
                                    Dlweigth = originalMasterdata.Dlweigth,
                                    Dm = originalMasterdata.Dm,
                                    Dmweigth = originalMasterdata.Dmweigth,
                                    EanCode = !string.IsNullOrEmpty(eanCode) ? eanCode : originalMasterdata.EanCode,
                                    //FGMaterial = originalMasterdata.FGMaterial,
                                    TopSheetMaterial = originalMasterdata.TopSheetMaterial,
                                    FgpicPath = originalMasterdata.FgpicPath,
                                    Flute = originalMasterdata.Flute,
                                    Gl = originalMasterdata.Gl,
                                    Glweigth = originalMasterdata.Glweigth,
                                    Hardship = originalMasterdata.Hardship,
                                    Hig = originalMasterdata.Hig,
                                    HighGroup = originalMasterdata.HighGroup,
                                    IndDes = originalMasterdata.IndDes,
                                    IndGrp = originalMasterdata.IndGrp,
                                    JointId = originalMasterdata.JointId,
                                    JointLap = originalMasterdata.JointLap,
                                    JoinType = originalMasterdata.JoinType,
                                    Language = originalMasterdata.Language,
                                    LayerPalet = originalMasterdata.LayerPalet,
                                    LeadTime = originalMasterdata.LeadTime,
                                    Leg = originalMasterdata.Leg,
                                    MatCopy = originalMasterdata.MaterialNo,
                                    MaterialComment = originalMasterdata.MaterialComment,
                                    MaterialType = originalMasterdata.MaterialType,
                                    NewH = originalMasterdata.NewH,
                                    NoSlot = originalMasterdata.NoSlot,
                                    OuterJoin = originalMasterdata.OuterJoin,
                                    PalletizationPath = originalMasterdata.PalletizationPath,
                                    PalletSize = originalMasterdata.PalletSize,
                                    PartNo = originalMasterdata.PartNo,
                                    Pc = originalMasterdata.Pc,
                                    PicPallet = originalMasterdata.PicPallet,
                                    PiecePatch = originalMasterdata.PiecePatch,
                                    PieceSet = originalMasterdata.PieceSet,
                                    PltAxleHeight = originalMasterdata.PltAxleHeight,
                                    PltBeam = originalMasterdata.PltBeam,
                                    PltDoubleAxle = originalMasterdata.PltDoubleAxle,
                                    PltFloorAbove = originalMasterdata.PltFloorAbove,
                                    PltFloorUnder = originalMasterdata.PltFloorUnder,
                                    PltLegDouble = originalMasterdata.PltLegDouble,
                                    PltLegSingle = originalMasterdata.PltLegSingle,
                                    PltSingleAxle = originalMasterdata.PltSingleAxle,
                                    PrintMasterPath = originalMasterdata.PrintMasterPath,
                                    PrintMethod = originalMasterdata.PrintMethod,
                                    PriorityFlag = originalMasterdata.PriorityFlag,
                                    ProType = originalMasterdata.ProType,
                                    PsmId = originalMasterdata.PsmId,
                                    PurTxt1 = originalMasterdata.PurTxt1,
                                    PurTxt2 = originalMasterdata.PurTxt2,
                                    PurTxt3 = originalMasterdata.PurTxt3,
                                    PurTxt4 = originalMasterdata.PurTxt4,
                                    RscStyle = originalMasterdata.RscStyle,
                                    SaleOrg = originalMasterdata.SaleOrg,
                                    SaleText1 = originalMasterdata.SaleText1,
                                    SaleText2 = originalMasterdata.SaleText2,
                                    SaleText3 = originalMasterdata.SaleText3,
                                    SaleText4 = originalMasterdata.SaleText4,
                                    SaleUom = originalMasterdata.SaleUom,
                                    ScoreL2 = originalMasterdata.ScoreL2,
                                    ScoreL3 = originalMasterdata.ScoreL3,
                                    ScoreL4 = originalMasterdata.ScoreL4,
                                    ScoreL5 = originalMasterdata.ScoreL5,
                                    ScoreL6 = originalMasterdata.ScoreL6,
                                    ScoreL7 = originalMasterdata.ScoreL7,
                                    ScoreL8 = originalMasterdata.ScoreL8,
                                    ScoreL9 = originalMasterdata.ScoreL9,
                                    ScoreW1 = originalMasterdata.ScoreW1,
                                    Scorew10 = originalMasterdata.Scorew10,
                                    Scorew11 = originalMasterdata.Scorew11,
                                    Scorew12 = originalMasterdata.Scorew12,
                                    Scorew13 = originalMasterdata.Scorew13,
                                    Scorew14 = originalMasterdata.Scorew14,
                                    Scorew15 = originalMasterdata.Scorew15,
                                    Scorew16 = originalMasterdata.Scorew16,
                                    Scorew2 = originalMasterdata.Scorew2,
                                    Scorew3 = originalMasterdata.Scorew3,
                                    Scorew4 = originalMasterdata.Scorew4,
                                    Scorew5 = originalMasterdata.Scorew5,
                                    Scorew6 = originalMasterdata.Scorew6,
                                    Scorew7 = originalMasterdata.Scorew7,
                                    Scorew8 = originalMasterdata.Scorew8,
                                    Scorew9 = originalMasterdata.Scorew9,
                                    SheetArea = originalMasterdata.SheetArea,
                                    Slit = originalMasterdata.Slit,
                                    SpareMax = originalMasterdata.SpareMax,
                                    SpareMin = originalMasterdata.SpareMin,
                                    SparePercen = originalMasterdata.SparePercen,
                                    StatusFlag = originalMasterdata.StatusFlag,
                                    TwoPiece = originalMasterdata.TwoPiece,
                                    Wid = originalMasterdata.Wid,
                                    Wire = originalMasterdata.Wire,
                                    TagBundle = originalMasterdata.TagBundle,
                                    TagPallet = originalMasterdata.TagPallet,
                                    NoTagBundle = originalMasterdata.NoTagBundle,
                                    HeadTagBundle = originalMasterdata.HeadTagBundle,
                                    FootTagBundle = originalMasterdata.FootTagBundle,
                                    HeadTagPallet = originalMasterdata.HeadTagPallet,
                                    FootTagPallet = originalMasterdata.FootTagPallet,
                                    WorkType = originalMasterdata.WorkType,
                                    Boistatus = originalMasterdata.Boistatus,
                                    BoxPacking = originalMasterdata.BoxPacking,
                                    FscCode = originalMasterdata.FscCode,
                                    FscFgCode = originalMasterdata.FscFgCode,
                                    RpacLob = originalMasterdata.RpacLob,
                                    RpacProgram = originalMasterdata.RpacProgram,
                                    RpacBrand = originalMasterdata.RpacBrand,
                                    RpacPackagingType = originalMasterdata.RpacPackagingType,
                                    FluteDesc = originalMasterdata.FluteDesc,
                                    HireFactory = originalMasterdata.HireFactory,
                                    SizeDimensions = originalMasterdata.SizeDimensions
                                };

                                SetPaperGrade(ref newMasterdata, factoryCode, newMasterdata.Flute, board.Board);
                                resultCount++;
                                PMTsDbContext.MasterData.Add(newMasterdata);

                                #endregion set new masterdata

                                //set update data

                                #region set mark x pc,desc in original masterdata

                                originalMasterdata.Pc = originalMasterdata.Pc.Length >= 20 ? string.Concat("x", originalMasterdata.Pc[..19]) : string.Concat("x", originalMasterdata.Pc);
                                originalMasterdata.Description = originalMasterdata.Description.Length >= 40 ? string.Concat("x", originalMasterdata.Description[..39]) : string.Concat("x", originalMasterdata.Description);
                                originalMasterdata.PdisStatus = "M";
                                originalMasterdata.TranStatus = false;
                                originalMasterdata.LastUpdate = DateTime.Now;
                                originalMasterdata.UpdatedBy = username;

                                //new hierarchy for orginal masterdata
                                if (originalMasterdata.Hierarchy.Length > 14)
                                {
                                    originalMasterdata.Hierarchy = originalMasterdata.Hierarchy[^4..] == "0000" ? string.Concat(new string(originalMasterdata.Hierarchy.Take(10).ToArray()), originalMasterdata.Hierarchy.Substring(10, 4)) : string.Concat(new string(originalMasterdata.Hierarchy.Take(10).ToArray()), originalMasterdata.Hierarchy.Substring(10, 8));
                                }
                                else if (originalMasterdata.Hierarchy.Length == 14)
                                {
                                    originalMasterdata.Hierarchy = string.Concat(new string(originalMasterdata.Hierarchy.Take(10).ToArray()), originalMasterdata.Hierarchy.Substring(10, 4));
                                }

                                resultCount++;
                                PMTsDbContext.MasterData.Update(originalMasterdata);

                                #endregion set mark x pc,desc in original masterdata

                                #region set saleviews

                                var orderType = string.Empty;

                                #region set order type

                                string pattern = @"[\d][4]";
                                Regex rgx = new(pattern);
                                var match = rgx.Match(newMasterdata.MaterialType);
                                if (match.Success)
                                {
                                    orderType = "LUMF";
                                }
                                else
                                {
                                    orderType = newMasterdata.MaterialType == "82" ? "BANC" : "ZMTO";
                                }

                                #endregion set order type

                                var saleview = new SalesView()
                                {
                                    MaterialNo = newMaterialNo,
                                    FactoryCode = factoryCode,
                                    SaleOrg = company.SaleOrg,
                                    Channel = 10,
                                    DevPlant = factoryCode,
                                    CustCode = newMasterdata.CustCode,
                                    MinQty = 0,
                                    OrderType = orderType,
                                    PdisStatus = pdisStatus,
                                    SaleUnitPrice = 0,
                                    NewPrice = 0,
                                    OldPrice = 0,
                                    PriceAdj = 0,
                                    TranStatus = false,
                                    SapStatus = false,
                                };

                                resultCount++;
                                PMTsDbContext.SalesView.Add(saleview);

                                #endregion set saleviews

                                #region set plantviews

                                var plantview = new PlantView
                                {
                                    FactoryCode = factoryCode,
                                    MaterialNo = newMaterialNo,
                                    Plant = factoryCode,
                                    PurchCode = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode).PurchasGrp,
                                    ShipBlk = originalPlantView == null ? string.Empty : originalPlantView.ShipBlk,
                                    StdFc = 0,
                                    StdTotalCost = originalMasterdata.MaterialType != "82" && cost != null ? cost.CostPerTon : 0,
                                    StdMovingCost = originalMasterdata.MaterialType == "82" && cost != null ? cost.CostPerTon : 0,
                                    StdVc = 0,
                                    PdisStatus = pdisStatus,
                                    SapStatus = false,
                                    TranStatus = false,
                                    EffectiveDate = null,
                                };

                                resultCount++;

                                PMTsDbContext.PlantView.Add(plantview);

                                #endregion set plantviews

                                #region set routings

                                var indexOfRouting = 0;
                                double? originalCORRWeightIn = 0;

                                foreach (var originalRouting in originalRoutings)
                                {
                                    indexOfRouting++;
                                    if (indexOfRouting == 1)
                                    {
                                        originalCORRWeightIn = originalRouting.WeightIn;
                                    }

                                    //var originalWeightIn = originalRouting.WeightIn;
                                    var paperWidth = originalRouting.PaperWidth = 0;
                                    var cutNo = 0;
                                    var trim = 0;
                                    double? percenTrim = 0;
                                    double? weightIn = 0;
                                    double? weightOut = 0;
                                    List<string> ppItem = [];

                                    if (board != null)
                                    {
                                        string[] ArrBoard;

                                        ArrBoard = board.Board.Split("/");
                                        if (ArrBoard.Length > 0)
                                            ppItem.Add(ArrBoard[0]);
                                        if (ArrBoard.Length > 1)
                                            ppItem.Add(ArrBoard[1]);
                                        if (ArrBoard.Length > 2)
                                            ppItem.Add(ArrBoard[2]);
                                        if (ArrBoard.Length > 3)
                                            ppItem.Add(ArrBoard[3]);
                                        if (ArrBoard.Length > 4)
                                            ppItem.Add(ArrBoard[4]);
                                        if (ArrBoard.Length > 5)
                                            ppItem.Add(ArrBoard[5]);
                                        if (ArrBoard.Length > 6)
                                            ppItem.Add(ArrBoard[6]);
                                    }

                                    if (originalRouting.Machine.Contains("COR", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        CorConfig corConfig = PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == originalRouting.Machine).FirstOrDefault();// JsonConvert.DeserializeObject<CorConfig>(_corConfigAPIRepository.GetCorConfigByFactoryCode(_factoryCode, machineName));
                                        corConfig ??= PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == "ELSE").FirstOrDefault();
                                        var routingDataModel = formulaRepository.CalculateRouting(originalRouting.Machine, factoryCode, board.Flute, newMasterdata.CutSheetWid ?? 0, newMasterdata.MaterialNo, ppItem, 0, 0, corConfig, pmtsConfig, flute, RollWidth, Grade);
                                        paperWidth = Convert.ToInt32(routingDataModel.PaperRollWidth);
                                        cutNo = Convert.ToInt32(routingDataModel.Cut);
                                        trim = Convert.ToInt32(routingDataModel.Trim);
                                        percenTrim = Convert.ToDouble(routingDataModel.PercentTrim);

                                        weightIn = newMasterdata.WeightSh;
                                        weightOut = originalRouting.WeightOut == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                    }
                                    else
                                    {
                                        paperWidth = 0;
                                        cutNo = 0;
                                        trim = 0;
                                        percenTrim = 0;
                                        weightIn = originalRouting.WeightIn == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                        weightOut = originalRouting.WeightOut == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                    }

                                    var routing = new Routing
                                    {
                                        Id = 0,
                                        MaterialNo = newMaterialNo,
                                        FactoryCode = factoryCode,
                                        SeqNo = originalRouting.SeqNo,
                                        Plant = company.Plant,
                                        SapStatus = false,
                                        TranStatus = false,
                                        Trim = trim,
                                        WeightIn = weightIn,
                                        WeightOut = weightOut,
                                        CreatedBy = username,
                                        CreatedDate = DateTime.Now,
                                        PdisStatus = pdisStatus,
                                        UpdatedBy = username,
                                        UpdatedDate = DateTime.Now,
                                        CutNo = cutNo,
                                        PaperWidth = paperWidth,
                                        PercenTrim = percenTrim,

                                        Alternative1 = originalRouting.Alternative1,
                                        Alternative2 = originalRouting.Alternative2,
                                        Alternative3 = originalRouting.Alternative3,
                                        Alternative4 = originalRouting.Alternative4,
                                        Alternative5 = originalRouting.Alternative5,
                                        Alternative6 = originalRouting.Alternative6,
                                        Alternative7 = originalRouting.Alternative7,
                                        Alternative8 = originalRouting.Alternative8,
                                        BlockNo = originalRouting.BlockNo,
                                        BlockNo2 = originalRouting.BlockNo2,
                                        BlockNo3 = originalRouting.BlockNo3,
                                        BlockNo4 = originalRouting.BlockNo4,
                                        BlockNo5 = originalRouting.BlockNo5,
                                        BlockNoPlant2 = originalRouting.BlockNoPlant2,
                                        BlockNoPlant3 = originalRouting.BlockNoPlant3,
                                        BlockNoPlant4 = originalRouting.BlockNoPlant4,
                                        BlockNoPlant5 = originalRouting.BlockNoPlant5,
                                        Coating = originalRouting.Coating,
                                        Color1 = originalRouting.Color1,
                                        Color2 = originalRouting.Color2,
                                        Color3 = originalRouting.Color3,
                                        Color4 = originalRouting.Color4,
                                        Color5 = originalRouting.Color5,
                                        Color6 = originalRouting.Color6,
                                        Color7 = originalRouting.Color7,
                                        ColorArea1 = originalRouting.ColorArea1,
                                        ColorArea2 = originalRouting.ColorArea2,
                                        ColorArea3 = originalRouting.ColorArea3,
                                        ColorArea4 = originalRouting.ColorArea4,
                                        ColorArea5 = originalRouting.ColorArea5,
                                        ColorArea6 = originalRouting.ColorArea6,
                                        ColorArea7 = originalRouting.ColorArea7,
                                        ColorCount = originalRouting.ColorCount,
                                        ColourCount = originalRouting.ColourCount,
                                        ControllerCode = originalRouting.ControllerCode,
                                        CustBarcodeNo = originalRouting.CustBarcodeNo,
                                        HandHold = originalRouting.HandHold,
                                        Hardship = originalRouting.Hardship,
                                        Human = originalRouting.Human,
                                        JoinMatNo = originalRouting.JoinMatNo,
                                        Machine = originalRouting.Machine,
                                        MatCode = originalRouting.MatCode,
                                        McMove = originalRouting.McMove,
                                        MylaNo = originalRouting.MylaNo,
                                        MylaNo2 = originalRouting.MylaNo2,
                                        MylaNo3 = originalRouting.MylaNo3,
                                        MylaNo4 = originalRouting.MylaNo4,
                                        MylaNo5 = originalRouting.MylaNo5,
                                        MylaNoPlant2 = originalRouting.MylaNoPlant2,
                                        MylaNoPlant3 = originalRouting.MylaNoPlant3,
                                        MylaNoPlant4 = originalRouting.MylaNoPlant4,
                                        MylaNoPlant5 = originalRouting.MylaNoPlant5,
                                        MylaSize = originalRouting.MylaSize,
                                        NoneBlk = originalRouting.NoneBlk,
                                        NoOpenIn = originalRouting.NoOpenIn,
                                        NoOpenOut = originalRouting.NoOpenOut,
                                        PlanCode = originalRouting.PlanCode,
                                        PlanProgramCode = originalRouting.PlanProgramCode,
                                        Platen = originalRouting.Platen,
                                        PlateNo = originalRouting.PlateNo,
                                        PlateNo2 = originalRouting.PlateNo2,
                                        PlateNo3 = originalRouting.PlateNo3,
                                        PlateNo4 = originalRouting.PlateNo4,
                                        PlateNo5 = originalRouting.PlateNo5,
                                        PlateNoPlant2 = originalRouting.PlateNoPlant2,
                                        PlateNoPlant3 = originalRouting.PlateNoPlant3,
                                        PlateNoPlant4 = originalRouting.PlateNoPlant4,
                                        PlateNoPlant5 = originalRouting.PlateNoPlant5,
                                        PostTm = originalRouting.PostTm,
                                        PrepareTm = originalRouting.PrepareTm,
                                        RemarkInprocess = originalRouting.RemarkInprocess,
                                        RepeatLength = originalRouting.RepeatLength,
                                        Rotary = originalRouting.Rotary,
                                        RotateIn = originalRouting.RotateIn,
                                        RotateOut = originalRouting.RotateOut,
                                        RunWaste = originalRouting.RunWaste,
                                        ScoreGap = originalRouting.ScoreGap,
                                        ScoreType = originalRouting.ScoreType,
                                        SemiBlk = originalRouting.SemiBlk,
                                        SeparatMatNo = originalRouting.SeparatMatNo,
                                        SetupTm = originalRouting.SetupTm,
                                        SetupWaste = originalRouting.SetupWaste,
                                        Shade1 = originalRouting.Shade1,
                                        Shade2 = originalRouting.Shade2,
                                        Shade3 = originalRouting.Shade3,
                                        Shade4 = originalRouting.Shade4,
                                        Shade5 = originalRouting.Shade5,
                                        Shade6 = originalRouting.Shade6,
                                        Shade7 = originalRouting.Shade7,
                                        SheetInLeg = originalRouting.SheetInLeg,
                                        SheetInWid = originalRouting.SheetInWid,
                                        SheetOutLeg = originalRouting.SheetOutLeg,
                                        SheetOutWid = originalRouting.SheetOutWid,
                                        ShipBlk = originalRouting.ShipBlk,
                                        Speed = originalRouting.Speed,
                                        StackHeight = originalRouting.StackHeight,
                                        StanBlk = originalRouting.StanBlk,
                                        StdProcess = originalRouting.StdProcess,
                                        TearTape = originalRouting.TearTape,
                                        TearTapeDistance = originalRouting.TearTapeDistance,
                                        TearTapeQty = originalRouting.TearTapeQty,
                                        UnUpgradBoard = originalRouting.UnUpgradBoard,
                                        WasteLeg = originalRouting.WasteLeg,
                                        WasteWid = originalRouting.WasteWid,
                                    };

                                    resultCount++;
                                    machineInUse = indexOfRouting == originalRoutings.Count ? machineInUse + routing.Machine : machineInUse + routing.Machine + ", ";

                                    newRoutings.Add(routing);
                                }
                                newRoutings = newRoutings != null && newRoutings.Count > 0 ? [.. newRoutings.OrderBy(r => r.SeqNo)] : newRoutings;

                                PMTsDbContext.Routing.AddRange(newRoutings);
                                //var routingChange = unitOfWork.Routings.AddList(newRoutings);

                                #endregion set routings

                                #region set boarduse

                                newBoardUse = new BoardUse
                                {
                                    Id = 0,
                                    Active = true,
                                    Priority = 1,
                                    MaterialNo = newMaterialNo,
                                    FactoryCode = factoryCode,
                                    Flute = newMasterdata.Flute,
                                    BoardId = board.Code,
                                    BoardName = board.Board,
                                    Kiwi = board.Kiwi,
                                    Weight = Convert.ToDouble(board.Weight),
                                    CreatedBy = username,
                                    CreatedDate = DateTime.Now,
                                    UpdatedBy = username,
                                    UpdatedDate = DateTime.Now,
                                };
                                SetBoardUse(ref newBoardUse, board.Board, board.Flute);
                                resultCount++;

                                PMTsDbContext.BoardUse.Add(newBoardUse);

                                #endregion set boarduse

                                #region set transaction detail

                                if (originalTransactionDetail != null)
                                {
                                    var transactionDetail = new TransactionsDetail
                                    {
                                        Id = 0,
                                        MaterialNo = newMaterialNo,
                                        Outsource = false,
                                        PdisStatus = pdisStatus,
                                        MatSaleOrg = null,
                                        FactoryCode = factoryCode,
                                        HireOrderType = null,
                                        HierarchyLv4 = originalMasterdata.Hierarchy.Substring(7, 3),
                                        MaxStep = 0,
                                        AmountColor = originalTransactionDetail.AmountColor,
                                        CapImg = originalTransactionDetail.CapImg,
                                        Cgtype = originalTransactionDetail.Cgtype,
                                        Gltail = originalTransactionDetail.Gltail,
                                        Glwid = originalTransactionDetail.Glwid,
                                        HoneyCoreSize = originalTransactionDetail.HoneyCoreSize,
                                        HvaGroup1 = string.Empty,
                                        HvaGroup2 = string.Empty,
                                        HvaGroup3 = string.Empty,
                                        HvaGroup4 = string.Empty,
                                        HvaGroup5 = string.Empty,
                                        HvaGroup6 = string.Empty,
                                        HvaGroup7 = string.Empty,
                                        IdKindOfProduct = originalTransactionDetail.IdKindOfProduct,
                                        IdKindOfProductGroup = originalTransactionDetail.IdKindOfProductGroup,
                                        IdMaterialType = originalTransactionDetail.IdMaterialType,
                                        IdProcessCost = originalTransactionDetail.IdProcessCost,
                                        IdProductType = originalTransactionDetail.IdProductType,
                                        IdProductUnit = originalTransactionDetail.IdProductUnit,
                                        IdSaleUnit = originalTransactionDetail.IdSaleUnit,
                                        IsNotch = originalTransactionDetail.IsNotch,
                                        IsWrap = originalTransactionDetail.IsWrap,
                                        NotchArea = originalTransactionDetail.NotchArea,
                                        NotchDegree = originalTransactionDetail.NotchDegree,
                                        NotchSide = originalTransactionDetail.NotchSide,
                                        PalletOverhang = originalTransactionDetail.PalletOverhang,
                                        SideA = originalTransactionDetail.SideA,
                                        SideB = originalTransactionDetail.SideB,
                                        SideC = originalTransactionDetail.SideC,
                                        SideD = originalTransactionDetail.SideD,
                                    };

                                    if (!string.IsNullOrEmpty(validData.HighValue))
                                    {
                                        switch (hvaCodeFromHighValue.Seq)
                                        {
                                            case 1:
                                                transactionDetail.HvaGroup1 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 2:
                                                transactionDetail.HvaGroup2 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 3:
                                                transactionDetail.HvaGroup3 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 4:
                                                transactionDetail.HvaGroup4 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 5:
                                                transactionDetail.HvaGroup5 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 6:
                                                transactionDetail.HvaGroup6 = hvaCodeFromHighValue.HighValue;
                                                break;

                                            case 7:
                                                transactionDetail.HvaGroup7 = hvaCodeFromHighValue.HighValue;
                                                break;
                                        }
                                    }

                                    resultCount++;
                                    PMTsDbContext.TransactionsDetail.Add(transactionDetail);
                                }

                                #endregion set transaction detail

                                #region set BOM

                                if (originalBomStructs != null && originalBomStructs.Count > 0)
                                {
                                    originalBomStructs.ForEach(b => b.Follower = newMaterialNo);
                                    originalBomStructs.ForEach(b => b.SapStatus = false);
                                    originalBomStructs.ForEach(b => b.UpdatedBy = username);
                                    originalBomStructs.ForEach(b => b.UpdatedDate = DateTime.Now);
                                    resultCount += originalBomStructs.Count;
                                    PMTsDbContext.BomStruct.UpdateRange(originalBomStructs);

                                    #region set Bom 84 sent status to sap

                                    var matbomArrs = new HashSet<string>(originalBomStructs.Select(b => b.MaterialNo));
                                    var masterdataBom84s = PMTsDbContext.MasterData.Where(x => matbomArrs.Contains(x.MaterialNo)).ToList();

                                    masterdataBom84s.ForEach(m => m.TranStatus = false);
                                    masterdataBom84s.ForEach(m => m.UpdatedBy = username);
                                    masterdataBom84s.ForEach(m => m.LastUpdate = DateTime.Now);
                                    resultCount += masterdataBom84s.Count;

                                    PMTsDbContext.MasterData.UpdateRange(masterdataBom84s);

                                    #endregion set Bom 84 sent status to sap
                                }

                                #endregion set BOM

                                #region set running No

                                runningList = UpdateRunningNo(newMasterdata.MaterialType, factoryCode, username, company.SaleOrg);
                                PMTsDbContext.RunningNo.UpdateRange(runningList);
                                resultCount += runningList.Count;

                                #endregion set running No

                                #region set board alternative

                                if (!string.IsNullOrEmpty(validData.BoardAlternative))
                                {
                                    boardAlternative = new BoardAlternative
                                    {
                                        Active = true,
                                        BoardName = validData.BoardAlternative,
                                        FactoryCode = factoryCode,
                                        Flute = newMasterdata.Flute,
                                        MaterialNo = newMasterdata.MaterialNo,
                                        Priority = 1,
                                        CreatedBy = username,
                                        CreatedDate = DateTime.Now,
                                    };

                                    PMTsDbContext.BoardAlternative.Add(boardAlternative);
                                    resultCount++;
                                }

                                #endregion set board alternative

                                #region Update all and check update list

                                using (var transaction = PMTsDbContext.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var checkSaveChange = PMTsDbContext.SaveChanges();

                                        if (checkSaveChange < resultCount)
                                            throw new Exception($"update fail prgress : {checkSaveChange} of {resultCount}");

                                        transaction.Commit();
                                    }
                                    catch (Exception)
                                    {
                                        transaction.Rollback();
                                    }
                                }

                                ////update all about copy to database
                                //var checkSaveChange = PMTsDbContext.SaveChanges();// + routingChange;

                                //if (checkSaveChange < resultCount)
                                //    throw new Exception($"update fail prgress : {checkSaveChange} of {resultCount}");

                                #endregion Update all and check update list

                                #region Sent result

                                //set new created data
                                newChangeBoardNewMaterial = new ChangeBoardNewMaterial
                                {
                                    IsCreatedSuccess = true,
                                    ErrorMessage = string.Empty,
                                    MaterialNo = newMaterialNo,
                                    PC = newMasterdata.Pc,
                                    Price = validData.Price,
                                    CustName = newMasterdata.CustName,
                                    Change = validData.Change,
                                    Description = newMasterdata.Description,
                                    Hierarchy = newMasterdata.Hierarchy,
                                    Cost = cost != null ? cost.CostPerTon : 0,
                                    Board = newMasterdata.Board,
                                    BoxType = newMasterdata.BoxType,
                                    Machine = machineInUse,
                                    CopyMaterialNo = copyMaterialNo,
                                    PdisStatus = pdisStatus,
                                    BoardAlternative = !string.IsNullOrEmpty(validData.BoardAlternative) ? boardAlternative.BoardName : string.Empty
                                };

                                results.Add(newChangeBoardNewMaterial);

                                #endregion Sent result
                            }
                        }
                        else
                        {
                            //set faild model to result
                            validData.ErrorMessage = errorMassage;
                            validData.IsCreatedSuccess = false;
                            if (boards != null && boards.Count > 1)
                            {
                                validData.CodeNewBoard = boards.Select(b => b.Code).ToList().Aggregate((i, j) => i + delimiter + j);
                            }
                            validData.Cost = 0;

                            results.Add(validData);
                        }
                    }
                    catch (Exception ex)
                    {
                        validData.ErrorMessage = ex.Message;
                        validData.IsCreatedSuccess = false;
                        validData.Cost = 0;
                        results.Add(validData);
                        continue;
                    }
                }

                #endregion Copy new material
            }

            return results;
        }

        public IEnumerable<ChangeBoardNewMaterial> CreateChangeFactoryNewMaterial(SqlConnection conn, string factoryCode, string username, bool IsCheckImport, List<ChangeBoardNewMaterial> changeFactoryNewMaterials)
        {
            var results = new List<ChangeBoardNewMaterial>();
            var validDatas = new List<ChangeBoardNewMaterial>();
            var isSaveSuccess = true;
            var unitOfWork = new UnitOfWork(PMTsDbContext);

            var pmtsConfig = PMTsDbContext.PmtsConfig.Where(p => p.FactoryCode == factoryCode && p.FucName == "Mintrim").FirstOrDefault();
            var RollWidth = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == factoryCode).OrderBy(o => o.Width).ToList();
            var Grade = PMTsDbContext.PaperGrade.Where(g => g.Active == true).ToList();

            //check number of change datas
            if (changeFactoryNewMaterials != null && changeFactoryNewMaterials.Count > 0)
            {
                results.Clear();
                results = changeFactoryNewMaterials.Where(c => !c.IsCreatedSuccess).ToList();
                validDatas = changeFactoryNewMaterials.Where(c => c.IsCreatedSuccess).ToList();

                #region Copy new material

                foreach (var validData in validDatas)
                {
                    try
                    {
                        #region initial param

                        var pdisStatus = "N";
                        var resultCount = 0;

                        var newChangeBoardNewMaterial = new ChangeBoardNewMaterial();
                        var originalMasterdata = new MasterData();
                        var newMasterdata = new MasterData();
                        var originalRoutings = new List<Routing>();
                        var newRoutings = new List<Routing>();
                        var originalSaleViews = new List<SalesView>();
                        var originalPlantView = new PlantView();
                        var originalTransactionDetail = new TransactionsDetail();
                        var newBoardUse = new BoardUse();
                        var originalBomStructs = new List<BomStruct>();
                        var errorMassage = string.Empty;
                        var newMaterialNo = string.Empty;
                        var newHierarchy = string.Empty;
                        var machineInUse = string.Empty;
                        var flute = new Flute();
                        var board = new BoardCombine();
                        var boards = new List<BoardCombine>();
                        var company = new CompanyProfile();
                        var runningList = new List<RunningNo>();
                        var hvaMasters = new List<HvaMaster>();
                        var hvaParam = new HvaMaster();
                        var boardAlternative = new BoardAlternative();
                        var cost = new Cost();
                        var mapCost = new MapCost();
                        double? basicWeight = 0;
                        var mapcostRepositoyry = new MapCostRepository(PMTsDbContext);
                        var boardCombineAccRepositoyry = new BoardCombineAccRepository(PMTsDbContext);
                        var formulaRepository = new FormulaRepository(PMTsDbContext);
                        isSaveSuccess = true;

                        #endregion initial param

                        #region original data

                        var copyMaterialNo = !string.IsNullOrEmpty(validData.CopyMaterialNo) ? validData.CopyMaterialNo.Trim() : validData.CopyMaterialNo;
                        company = PMTsDbContext.CompanyProfile.AsNoTracking().FirstOrDefault(c => c.Plant == validData.CopyFactoryCode);
                        originalMasterdata = PMTsDbContext.MasterData.AsNoTracking().FirstOrDefault(m => m.FactoryCode == validData.CopyFactoryCode && m.MaterialNo == copyMaterialNo && m.SaleOrg == company.SaleOrg);
                        originalRoutings =
                        [
                            .. PMTsDbContext.Routing.AsNoTracking().Where(r => r.FactoryCode == validData.CopyFactoryCode && r.MaterialNo == copyMaterialNo).OrderBy(r => r.SeqNo),
                        ];
                        originalSaleViews =
                        [
                            .. PMTsDbContext.SalesView.AsNoTracking().Where(s => s.FactoryCode == validData.CopyFactoryCode && s.MaterialNo == copyMaterialNo),
                        ];
                        originalPlantView = PMTsDbContext.PlantView.AsNoTracking().FirstOrDefault(p => p.FactoryCode == validData.CopyFactoryCode && p.MaterialNo == copyMaterialNo);
                        originalTransactionDetail = PMTsDbContext.TransactionsDetail.AsNoTracking().FirstOrDefault(t => t.FactoryCode == validData.CopyFactoryCode && t.MaterialNo == copyMaterialNo);
                        hvaMasters = [.. PMTsDbContext.HvaMaster.AsNoTracking()];

                        #endregion original data

                        //set pdis status from routing state
                        if (originalRoutings != null && originalRoutings.Count > 0)
                        {
                            pdisStatus = "C";
                        }

                        //calculate basic weight
                        if (originalMasterdata != null)
                        {
                            //check board with new code
                            boards = [.. PMTsDbContext.BoardCombine.Where(b => b.Board == originalMasterdata.Board && b.Flute == originalMasterdata.Flute)];
                            if (boards != null && boards.Count == 1)
                            {
                                board = boards.FirstOrDefault();
                                basicWeight = GetBasisWeight(board.Code, originalMasterdata.Flute, factoryCode);
                                flute = PMTsDbContext.Flute.Where(f => f.Flute1 == board.Flute && f.FactoryCode == factoryCode).FirstOrDefault();
                            }
                            else if (boards.Count == 0)
                            {
                                board = null;
                            }

                            var materialType = !string.IsNullOrEmpty(validData.MaterialType) ? validData.MaterialType : originalMasterdata.MaterialType;
                            newMaterialNo = GenMatNo(materialType, factoryCode);

                            mapCost = mapcostRepositoyry.GetCostField(originalMasterdata.Hierarchy.Substring(2, 2), originalMasterdata.Hierarchy.Substring(3, 3), originalMasterdata.Hierarchy.Substring(7, 3));
                            cost = board == null || mapCost == null ? null : boardCombineAccRepositoyry.GetCost(conn, factoryCode, board.Code, mapCost.CostField);

                            if (!string.IsNullOrEmpty(originalMasterdata.HighValue))
                            {
                                hvaParam = hvaMasters.FirstOrDefault(h => h.HighValue == validData.HighValue);
                            }
                        }

                        #region case error message

                        if (originalMasterdata == null)
                        {
                            errorMassage = $"Can't find material no. {copyMaterialNo}.";
                            isSaveSuccess = false;
                        }
                        else if (company == null)
                        {
                            errorMassage = $"Don't have a company registration.";
                            isSaveSuccess = false;
                        }
                        else if (mapCost == null)
                        {
                            errorMassage = $"Can't find mapcost from your hierarchy.";
                            isSaveSuccess = false;
                        }
                        else if (board == null)
                        {
                            errorMassage = $"Can't find board {originalMasterdata.Board} and flute {originalMasterdata.Flute}.";
                            isSaveSuccess = false;
                        }
                        else if (boards == null || boards.Count > 1)
                        {
                            errorMassage = $"Find duplicate {boards.Count} row in board combine.";
                            isSaveSuccess = false;
                        }
                        else if (basicWeight == null)
                        {
                            errorMassage = $"Can't calculate basic weight for {copyMaterialNo}.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(originalMasterdata.HighValue) && hvaParam == null)
                        {
                            errorMassage = $"Can't create new material form high value {originalMasterdata.HighValue}.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(newMaterialNo))
                        {
                            errorMassage = "Can't genarate new material number.";
                            isSaveSuccess = false;
                        }
                        else if (string.IsNullOrEmpty(originalMasterdata.Hierarchy))
                        {
                            errorMassage = "Invalid hierarchy in original material.";
                            isSaveSuccess = false;
                        }
                        else if (cost == null || cost.CostPerTon == 0)
                        {
                            errorMassage = "Invalid Cost value 0.";
                            isSaveSuccess = false;
                        }
                        else if (originalSaleViews == null || originalSaleViews.Count == 0)
                        {
                            errorMassage = "Old material without saleviews.";
                            //isSaveSuccess = false;
                        }
                        else if (originalPlantView == null)
                        {
                            errorMassage = "Old material without plantview.";
                            //isSaveSuccess = false;
                        }

                        #endregion case error message

                        if (isSaveSuccess)
                        {
                            if (IsCheckImport)
                            {
                                #region Sent result

                                //set new created data
                                newChangeBoardNewMaterial = new ChangeBoardNewMaterial
                                {
                                    IsCreatedSuccess = true,
                                    ErrorMessage = string.Empty,
                                    MaterialNo = newMaterialNo,
                                    MaterialType = validData.MaterialType,
                                    CopyFactoryCode = originalMasterdata.FactoryCode,
                                    CopyMaterialNo = originalMasterdata.MaterialNo,
                                    PC = originalMasterdata.Pc,
                                    Flute = originalMasterdata.Flute,
                                    NewBoard = null,
                                    Change = null,
                                    Price = null,
                                    HighValue = originalMasterdata.HighValue,
                                    CodeNewBoard = null,
                                    Cost = cost.CostPerTon,
                                    BoardAlternative = null,
                                };

                                results.Add(newChangeBoardNewMaterial);

                                #endregion Sent result
                            }
                            else
                            {
                                originalBomStructs = [.. PMTsDbContext.BomStruct.Where(bs => bs.FactoryCode == factoryCode && bs.Follower == originalMasterdata.MaterialNo)];

                                var weightBox = Convert.ToDouble(basicWeight * originalMasterdata.BoxArea / 1000000000);
                                var weightSh = Convert.ToDouble(basicWeight * originalMasterdata.SheetArea / 1000000000);

                                #region set new masterdata

                                var eanCode = GetEanCode(newMaterialNo, company.SaleOrg);
                                newMasterdata = new MasterData
                                {
                                    FactoryCode = factoryCode,
                                    Plant = factoryCode,
                                    MaterialNo = newMaterialNo,
                                    Board = board.Board,
                                    Code = board.Code,
                                    PdisStatus = pdisStatus,
                                    SapStatus = false,
                                    TranStatus = false,
                                    User = "ZZ_Import",
                                    LastUpdate = DateTime.Now,
                                    UpdatedBy = username,
                                    CreateDate = DateTime.Now,
                                    CreatedBy = username,
                                    HighValue = originalMasterdata.HighValue,
                                    WeightBox = Convert.ToDouble(basicWeight * originalMasterdata.BoxArea / 1000000000),
                                    WeightSh = Convert.ToDouble(basicWeight * originalMasterdata.SheetArea / 1000000000),
                                    Hierarchy = originalMasterdata.Hierarchy,
                                    IsTransfer = false,
                                    UnUpgradBoard = originalMasterdata.UnUpgradBoard,
                                    AttachFileMoPath = originalMasterdata.AttachFileMoPath,
                                    Bl = originalMasterdata.Bl,
                                    Blweigth = originalMasterdata.Blweigth,
                                    Bm = originalMasterdata.Bm,
                                    Bmweigth = originalMasterdata.Bmweigth,
                                    BomUom = originalMasterdata.BomUom,
                                    BoxArea = originalMasterdata.BoxArea,
                                    BoxHandle = originalMasterdata.BoxHandle,
                                    BoxPalet = originalMasterdata.BoxPalet,
                                    BoxType = originalMasterdata.BoxType,
                                    Bun = originalMasterdata.Bun,
                                    BunLayer = originalMasterdata.BunLayer,
                                    Change = originalMasterdata.Change,
                                    ChangeHistory = originalMasterdata.ChangeHistory,
                                    ChangeInfo = originalMasterdata.ChangeInfo,
                                    CipinvType = originalMasterdata.CipinvType,
                                    Cl = originalMasterdata.Cl,
                                    Clweigth = originalMasterdata.Clweigth,
                                    Cm = originalMasterdata.Cm,
                                    Cmweigth = originalMasterdata.Cmweigth,
                                    CusId = originalMasterdata.CusId,
                                    CustCode = originalMasterdata.CustCode,
                                    CustComment = originalMasterdata.CustComment,
                                    CustInvType = originalMasterdata.CustInvType,
                                    CutSheetLeng = originalMasterdata.CutSheetLeng,
                                    CustName = originalMasterdata.CustName,
                                    CutSheetLengInch = originalMasterdata.CutSheetLengInch,
                                    CutSheetWid = originalMasterdata.CutSheetWid,
                                    Description = originalMasterdata.Description,
                                    CutSheetWidInch = originalMasterdata.CutSheetWidInch,
                                    DiecutPictPath = originalMasterdata.DiecutPictPath,
                                    Dl = originalMasterdata.Dl,
                                    Dlweigth = originalMasterdata.Dlweigth,
                                    Dm = originalMasterdata.Dm,
                                    Dmweigth = originalMasterdata.Dmweigth,
                                    EanCode = !string.IsNullOrEmpty(eanCode) ? eanCode : originalMasterdata.EanCode,
                                    TopSheetMaterial = originalMasterdata.TopSheetMaterial,
                                    FgpicPath = originalMasterdata.FgpicPath,
                                    Flute = originalMasterdata.Flute,
                                    Gl = originalMasterdata.Gl,
                                    Glweigth = originalMasterdata.Glweigth,
                                    Hardship = originalMasterdata.Hardship,
                                    Hig = originalMasterdata.Hig,
                                    HighGroup = originalMasterdata.HighGroup,
                                    IndDes = originalMasterdata.IndDes,
                                    IndGrp = originalMasterdata.IndGrp,
                                    JointId = originalMasterdata.JointId,
                                    JointLap = originalMasterdata.JointLap,
                                    JoinType = originalMasterdata.JoinType,
                                    Language = originalMasterdata.Language,
                                    LayerPalet = originalMasterdata.LayerPalet,
                                    LeadTime = originalMasterdata.LeadTime,
                                    Leg = originalMasterdata.Leg,
                                    MatCopy = originalMasterdata.MaterialNo,
                                    MaterialComment = originalMasterdata.MaterialComment,
                                    MaterialType = originalMasterdata.MaterialType,
                                    NewH = originalMasterdata.NewH,
                                    NoSlot = originalMasterdata.NoSlot,
                                    OuterJoin = originalMasterdata.OuterJoin,
                                    PalletizationPath = originalMasterdata.PalletizationPath,
                                    PalletSize = originalMasterdata.PalletSize,
                                    PartNo = originalMasterdata.PartNo,
                                    Pc = originalMasterdata.Pc,
                                    PicPallet = originalMasterdata.PicPallet,
                                    PiecePatch = originalMasterdata.PiecePatch,
                                    PieceSet = originalMasterdata.PieceSet,
                                    PltAxleHeight = originalMasterdata.PltAxleHeight,
                                    PltBeam = originalMasterdata.PltBeam,
                                    PltDoubleAxle = originalMasterdata.PltDoubleAxle,
                                    PltFloorAbove = originalMasterdata.PltFloorAbove,
                                    PltFloorUnder = originalMasterdata.PltFloorUnder,
                                    PltLegDouble = originalMasterdata.PltLegDouble,
                                    PltLegSingle = originalMasterdata.PltLegSingle,
                                    PltSingleAxle = originalMasterdata.PltSingleAxle,
                                    PrintMasterPath = originalMasterdata.PrintMasterPath,
                                    PrintMethod = originalMasterdata.PrintMethod,
                                    PriorityFlag = originalMasterdata.PriorityFlag,
                                    ProType = originalMasterdata.ProType,
                                    PsmId = originalMasterdata.PsmId,
                                    PurTxt1 = originalMasterdata.PurTxt1,
                                    PurTxt2 = originalMasterdata.PurTxt2,
                                    PurTxt3 = originalMasterdata.PurTxt3,
                                    PurTxt4 = originalMasterdata.PurTxt4,
                                    RscStyle = originalMasterdata.RscStyle,
                                    SaleOrg = originalMasterdata.SaleOrg,
                                    SaleText1 = originalMasterdata.SaleText1,
                                    SaleText2 = originalMasterdata.SaleText2,
                                    SaleText3 = originalMasterdata.SaleText3,
                                    SaleText4 = originalMasterdata.SaleText4,
                                    SaleUom = originalMasterdata.SaleUom,
                                    ScoreL2 = originalMasterdata.ScoreL2,
                                    ScoreL3 = originalMasterdata.ScoreL3,
                                    ScoreL4 = originalMasterdata.ScoreL4,
                                    ScoreL5 = originalMasterdata.ScoreL5,
                                    ScoreL6 = originalMasterdata.ScoreL6,
                                    ScoreL7 = originalMasterdata.ScoreL7,
                                    ScoreL8 = originalMasterdata.ScoreL8,
                                    ScoreL9 = originalMasterdata.ScoreL9,
                                    ScoreW1 = originalMasterdata.ScoreW1,
                                    Scorew10 = originalMasterdata.Scorew10,
                                    Scorew11 = originalMasterdata.Scorew11,
                                    Scorew12 = originalMasterdata.Scorew12,
                                    Scorew13 = originalMasterdata.Scorew13,
                                    Scorew14 = originalMasterdata.Scorew14,
                                    Scorew15 = originalMasterdata.Scorew15,
                                    Scorew16 = originalMasterdata.Scorew16,
                                    Scorew2 = originalMasterdata.Scorew2,
                                    Scorew3 = originalMasterdata.Scorew3,
                                    Scorew4 = originalMasterdata.Scorew4,
                                    Scorew5 = originalMasterdata.Scorew5,
                                    Scorew6 = originalMasterdata.Scorew6,
                                    Scorew7 = originalMasterdata.Scorew7,
                                    Scorew8 = originalMasterdata.Scorew8,
                                    Scorew9 = originalMasterdata.Scorew9,
                                    SheetArea = originalMasterdata.SheetArea,
                                    Slit = originalMasterdata.Slit,
                                    SpareMax = originalMasterdata.SpareMax,
                                    SpareMin = originalMasterdata.SpareMin,
                                    SparePercen = originalMasterdata.SparePercen,
                                    StatusFlag = originalMasterdata.StatusFlag,
                                    TwoPiece = originalMasterdata.TwoPiece,
                                    Wid = originalMasterdata.Wid,
                                    Wire = originalMasterdata.Wire,
                                    HireFactory = factoryCode,
                                    SizeDimensions = originalMasterdata.SizeDimensions
                                };

                                SetPaperGrade(ref newMasterdata, factoryCode, newMasterdata.Flute, board.Board);
                                resultCount++;
                                PMTsDbContext.MasterData.Add(newMasterdata);

                                #endregion set new masterdata

                                //set update data

                                #region set mark x pc,desc in original masterdata

                                //originalMasterdata.Pc = originalMasterdata.Pc.Length >= 20 ? "x" + originalMasterdata.Pc.Substring(0, 19) : "x" + originalMasterdata.Pc;
                                //originalMasterdata.Description = originalMasterdata.Description.Length >= 40 ? "x" + originalMasterdata.Description.Substring(0, 39) : "x" + originalMasterdata.Description;
                                //originalMasterdata.PdisStatus = "M";
                                //originalMasterdata.TranStatus = false;
                                //originalMasterdata.LastUpdate = DateTime.Now;
                                //originalMasterdata.UpdatedBy = username;

                                ////new hierarchy for orginal masterdata
                                //if (originalMasterdata.Hierarchy.Length > 14)
                                //{
                                //    originalMasterdata.Hierarchy = originalMasterdata.Hierarchy.Substring(originalMasterdata.Hierarchy.Length - 4) == "0000" ? new string(originalMasterdata.Hierarchy.Take(4).ToArray()) + "999999" + originalMasterdata.Hierarchy.Substring(10, 4) : new string(originalMasterdata.Hierarchy.Take(4).ToArray()) + "999999" + originalMasterdata.Hierarchy.Substring(10, 8);
                                //}
                                //else if (originalMasterdata.Hierarchy.Length == 14)
                                //{
                                //    originalMasterdata.Hierarchy = new string(originalMasterdata.Hierarchy.Take(4).ToArray()) + "999999" + originalMasterdata.Hierarchy.Substring(10, 4);
                                //}

                                //resultCount++;
                                //PMTsDbContext.MasterData.Update(originalMasterdata);

                                #endregion set mark x pc,desc in original masterdata

                                #region set saleviews

                                var orderType = string.Empty;

                                #region set order type

                                string pattern = @"[\d][4]";
                                Regex rgx = new(pattern);
                                var match = rgx.Match(newMasterdata.MaterialType);
                                if (match.Success)
                                {
                                    orderType = "LUMF";
                                }
                                else
                                {
                                    orderType = newMasterdata.MaterialType == "82" ? "BANC" : "ZMTO";
                                }

                                #endregion set order type

                                var saleview = new SalesView()
                                {
                                    MaterialNo = newMaterialNo,
                                    FactoryCode = factoryCode,
                                    SaleOrg = company.SaleOrg,
                                    Channel = 10,
                                    DevPlant = factoryCode,
                                    CustCode = newMasterdata.CustCode,
                                    MinQty = 0,
                                    OrderType = orderType,
                                    PdisStatus = pdisStatus,
                                    SaleUnitPrice = 0,
                                    NewPrice = 0,
                                    OldPrice = 0,
                                    PriceAdj = 0,
                                    TranStatus = false,
                                    SapStatus = false,
                                };

                                resultCount++;
                                PMTsDbContext.SalesView.Add(saleview);

                                #endregion set saleviews

                                #region set plantviews

                                var plantview = new PlantView
                                {
                                    FactoryCode = factoryCode,
                                    MaterialNo = newMaterialNo,
                                    Plant = factoryCode,
                                    PurchCode = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode).PurchasGrp,
                                    ShipBlk = originalPlantView == null ? string.Empty : originalPlantView.ShipBlk,
                                    StdFc = 0,
                                    StdTotalCost = originalMasterdata.MaterialType != "82" && cost != null ? cost.CostPerTon : 0,
                                    StdMovingCost = originalMasterdata.MaterialType == "82" && cost != null ? cost.CostPerTon : 0,
                                    StdVc = 0,
                                    PdisStatus = pdisStatus,
                                    SapStatus = false,
                                    TranStatus = false,
                                    EffectiveDate = null,
                                };

                                resultCount++;

                                PMTsDbContext.PlantView.Add(plantview);

                                #endregion set plantviews

                                #region set routings

                                var indexOfRouting = 0;
                                double? originalCORRWeightIn = 0;

                                foreach (var originalRouting in originalRoutings)
                                {
                                    indexOfRouting++;
                                    if (indexOfRouting == 1)
                                    {
                                        originalCORRWeightIn = originalRouting.WeightIn;
                                    }

                                    //var originalWeightIn = originalRouting.WeightIn;
                                    var paperWidth = originalRouting.PaperWidth = 0;
                                    var cutNo = 0;
                                    var trim = 0;
                                    double? percenTrim = 0;
                                    double? weightIn = 0;
                                    double? weightOut = 0;
                                    List<string> ppItem = [];

                                    if (board != null)
                                    {
                                        string[] ArrBoard;

                                        ArrBoard = board.Board.Split("/");
                                        if (ArrBoard.Length > 0)
                                            ppItem.Add(ArrBoard[0]);
                                        if (ArrBoard.Length > 1)
                                            ppItem.Add(ArrBoard[1]);
                                        if (ArrBoard.Length > 2)
                                            ppItem.Add(ArrBoard[2]);
                                        if (ArrBoard.Length > 3)
                                            ppItem.Add(ArrBoard[3]);
                                        if (ArrBoard.Length > 4)
                                            ppItem.Add(ArrBoard[4]);
                                        if (ArrBoard.Length > 5)
                                            ppItem.Add(ArrBoard[5]);
                                        if (ArrBoard.Length > 6)
                                            ppItem.Add(ArrBoard[6]);
                                    }

                                    if (originalRouting.Machine.Contains("COR", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        CorConfig corConfig = PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == originalRouting.Machine).FirstOrDefault();// JsonConvert.DeserializeObject<CorConfig>(_corConfigAPIRepository.GetCorConfigByFactoryCode(_factoryCode, machineName));
                                        corConfig ??= PMTsDbContext.CorConfig.Where(w => w.FactoryCode == factoryCode && w.Name == "ELSE").FirstOrDefault();
                                        var routingDataModel = formulaRepository.CalculateRouting(originalRouting.Machine, factoryCode, board.Flute, newMasterdata.CutSheetWid ?? 0, newMasterdata.MaterialNo, ppItem, 0, 0, corConfig, pmtsConfig, flute, RollWidth, Grade);
                                        paperWidth = Convert.ToInt32(routingDataModel.PaperRollWidth);
                                        cutNo = Convert.ToInt32(routingDataModel.Cut);
                                        trim = Convert.ToInt32(routingDataModel.Trim);
                                        percenTrim = Convert.ToDouble(routingDataModel.PercentTrim);

                                        weightIn = newMasterdata.WeightSh;
                                        weightOut = originalRouting.WeightOut == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                    }
                                    else
                                    {
                                        paperWidth = 0;
                                        cutNo = 0;
                                        trim = 0;
                                        percenTrim = 0;
                                        weightIn = originalRouting.WeightIn == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                        weightOut = originalRouting.WeightOut == originalCORRWeightIn ? newMasterdata.WeightSh : newMasterdata.WeightBox;
                                    }

                                    var routing = new Routing
                                    {
                                        Id = 0,
                                        MaterialNo = newMaterialNo,
                                        FactoryCode = factoryCode,
                                        SeqNo = originalRouting.SeqNo,
                                        Plant = company.Plant,
                                        SapStatus = false,
                                        TranStatus = false,
                                        Trim = trim,
                                        WeightIn = weightIn,
                                        WeightOut = weightOut,
                                        CreatedBy = username,
                                        CreatedDate = DateTime.Now,
                                        PdisStatus = pdisStatus,
                                        UpdatedBy = username,
                                        UpdatedDate = DateTime.Now,
                                        CutNo = cutNo,
                                        PaperWidth = paperWidth,
                                        PercenTrim = percenTrim,

                                        Alternative1 = originalRouting.Alternative1,
                                        Alternative2 = originalRouting.Alternative2,
                                        Alternative3 = originalRouting.Alternative3,
                                        Alternative4 = originalRouting.Alternative4,
                                        Alternative5 = originalRouting.Alternative5,
                                        Alternative6 = originalRouting.Alternative6,
                                        Alternative7 = originalRouting.Alternative7,
                                        Alternative8 = originalRouting.Alternative8,
                                        BlockNo = originalRouting.BlockNo,
                                        BlockNo2 = originalRouting.BlockNo2,
                                        BlockNo3 = originalRouting.BlockNo3,
                                        BlockNo4 = originalRouting.BlockNo4,
                                        BlockNo5 = originalRouting.BlockNo5,
                                        BlockNoPlant2 = originalRouting.BlockNoPlant2,
                                        BlockNoPlant3 = originalRouting.BlockNoPlant3,
                                        BlockNoPlant4 = originalRouting.BlockNoPlant4,
                                        BlockNoPlant5 = originalRouting.BlockNoPlant5,
                                        Coating = originalRouting.Coating,
                                        Color1 = originalRouting.Color1,
                                        Color2 = originalRouting.Color2,
                                        Color3 = originalRouting.Color3,
                                        Color4 = originalRouting.Color4,
                                        Color5 = originalRouting.Color5,
                                        Color6 = originalRouting.Color6,
                                        Color7 = originalRouting.Color7,
                                        ColorArea1 = originalRouting.ColorArea1,
                                        ColorArea2 = originalRouting.ColorArea2,
                                        ColorArea3 = originalRouting.ColorArea3,
                                        ColorArea4 = originalRouting.ColorArea4,
                                        ColorArea5 = originalRouting.ColorArea5,
                                        ColorArea6 = originalRouting.ColorArea6,
                                        ColorArea7 = originalRouting.ColorArea7,
                                        ColorCount = originalRouting.ColorCount,
                                        ColourCount = originalRouting.ColourCount,
                                        ControllerCode = originalRouting.ControllerCode,
                                        CustBarcodeNo = originalRouting.CustBarcodeNo,
                                        HandHold = originalRouting.HandHold,
                                        Hardship = originalRouting.Hardship,
                                        Human = originalRouting.Human,
                                        JoinMatNo = originalRouting.JoinMatNo,
                                        Machine = originalRouting.Machine,
                                        MatCode = originalRouting.MatCode,
                                        McMove = originalRouting.McMove,
                                        MylaNo = originalRouting.MylaNo,
                                        MylaNo2 = originalRouting.MylaNo2,
                                        MylaNo3 = originalRouting.MylaNo3,
                                        MylaNo4 = originalRouting.MylaNo4,
                                        MylaNo5 = originalRouting.MylaNo5,
                                        MylaNoPlant2 = originalRouting.MylaNoPlant2,
                                        MylaNoPlant3 = originalRouting.MylaNoPlant3,
                                        MylaNoPlant4 = originalRouting.MylaNoPlant4,
                                        MylaNoPlant5 = originalRouting.MylaNoPlant5,
                                        MylaSize = originalRouting.MylaSize,
                                        NoneBlk = originalRouting.NoneBlk,
                                        NoOpenIn = originalRouting.NoOpenIn,
                                        NoOpenOut = originalRouting.NoOpenOut,
                                        PlanCode = originalRouting.PlanCode,
                                        PlanProgramCode = originalRouting.PlanProgramCode,
                                        Platen = originalRouting.Platen,
                                        PlateNo = originalRouting.PlateNo,
                                        PlateNo2 = originalRouting.PlateNo2,
                                        PlateNo3 = originalRouting.PlateNo3,
                                        PlateNo4 = originalRouting.PlateNo4,
                                        PlateNo5 = originalRouting.PlateNo5,
                                        PlateNoPlant2 = originalRouting.PlateNoPlant2,
                                        PlateNoPlant3 = originalRouting.PlateNoPlant3,
                                        PlateNoPlant4 = originalRouting.PlateNoPlant4,
                                        PlateNoPlant5 = originalRouting.PlateNoPlant5,
                                        PostTm = originalRouting.PostTm,
                                        PrepareTm = originalRouting.PrepareTm,
                                        RemarkInprocess = originalRouting.RemarkInprocess,
                                        RepeatLength = originalRouting.RepeatLength,
                                        Rotary = originalRouting.Rotary,
                                        RotateIn = originalRouting.RotateIn,
                                        RotateOut = originalRouting.RotateOut,
                                        RunWaste = originalRouting.RunWaste,
                                        ScoreGap = originalRouting.ScoreGap,
                                        ScoreType = originalRouting.ScoreType,
                                        SemiBlk = originalRouting.SemiBlk,
                                        SeparatMatNo = originalRouting.SeparatMatNo,
                                        SetupTm = originalRouting.SetupTm,
                                        SetupWaste = originalRouting.SetupWaste,
                                        Shade1 = originalRouting.Shade1,
                                        Shade2 = originalRouting.Shade2,
                                        Shade3 = originalRouting.Shade3,
                                        Shade4 = originalRouting.Shade4,
                                        Shade5 = originalRouting.Shade5,
                                        Shade6 = originalRouting.Shade6,
                                        Shade7 = originalRouting.Shade7,
                                        SheetInLeg = originalRouting.SheetInLeg,
                                        SheetInWid = originalRouting.SheetInWid,
                                        SheetOutLeg = originalRouting.SheetOutLeg,
                                        SheetOutWid = originalRouting.SheetOutWid,
                                        ShipBlk = originalRouting.ShipBlk,
                                        Speed = originalRouting.Speed,
                                        StackHeight = originalRouting.StackHeight,
                                        StanBlk = originalRouting.StanBlk,
                                        StdProcess = originalRouting.StdProcess,
                                        TearTape = originalRouting.TearTape,
                                        TearTapeDistance = originalRouting.TearTapeDistance,
                                        TearTapeQty = originalRouting.TearTapeQty,
                                        UnUpgradBoard = originalRouting.UnUpgradBoard,
                                        WasteLeg = originalRouting.WasteLeg,
                                        WasteWid = originalRouting.WasteWid,
                                    };

                                    resultCount++;
                                    machineInUse = indexOfRouting == originalRoutings.Count ? machineInUse + routing.Machine : machineInUse + routing.Machine + ", ";

                                    newRoutings.Add(routing);
                                }
                                newRoutings = newRoutings != null && newRoutings.Count > 0 ? [.. newRoutings.OrderBy(r => r.SeqNo)] : newRoutings;

                                PMTsDbContext.Routing.AddRange(newRoutings);
                                //var routingChange = unitOfWork.Routings.AddList(newRoutings);

                                #endregion set routings

                                #region set boarduse

                                newBoardUse = new BoardUse
                                {
                                    Id = 0,
                                    Active = true,
                                    Priority = 1,
                                    MaterialNo = newMaterialNo,
                                    FactoryCode = factoryCode,
                                    Flute = newMasterdata.Flute,
                                    BoardId = board.Code,
                                    BoardName = board.Board,
                                    Kiwi = board.Kiwi,
                                    Weight = Convert.ToDouble(board.Weight),
                                    CreatedBy = username,
                                    CreatedDate = DateTime.Now,
                                    UpdatedBy = username,
                                    UpdatedDate = DateTime.Now,
                                };
                                SetBoardUse(ref newBoardUse, board.Board, board.Flute);
                                resultCount++;

                                PMTsDbContext.BoardUse.Add(newBoardUse);

                                #endregion set boarduse

                                #region set transaction detail

                                if (originalTransactionDetail != null)
                                {
                                    var transactionDetail = new TransactionsDetail
                                    {
                                        Id = 0,
                                        MaterialNo = newMaterialNo,
                                        Outsource = false,
                                        PdisStatus = pdisStatus,
                                        MatSaleOrg = null,
                                        FactoryCode = factoryCode,
                                        HireOrderType = null,
                                        HierarchyLv4 = originalMasterdata.Hierarchy.Substring(7, 3),
                                        MaxStep = 0,
                                        AmountColor = originalTransactionDetail.AmountColor,
                                        CapImg = originalTransactionDetail.CapImg,
                                        Cgtype = originalTransactionDetail.Cgtype,
                                        Gltail = originalTransactionDetail.Gltail,
                                        Glwid = originalTransactionDetail.Glwid,
                                        HoneyCoreSize = originalTransactionDetail.HoneyCoreSize,
                                        HvaGroup1 = originalTransactionDetail.HvaGroup1,
                                        HvaGroup2 = originalTransactionDetail.HvaGroup2,
                                        HvaGroup3 = originalTransactionDetail.HvaGroup3,
                                        HvaGroup4 = originalTransactionDetail.HvaGroup4,
                                        HvaGroup5 = originalTransactionDetail.HvaGroup5,
                                        HvaGroup6 = originalTransactionDetail.HvaGroup6,
                                        HvaGroup7 = originalTransactionDetail.HvaGroup7,
                                        IdKindOfProduct = originalTransactionDetail.IdKindOfProduct,
                                        IdKindOfProductGroup = originalTransactionDetail.IdKindOfProductGroup,
                                        IdMaterialType = originalTransactionDetail.IdMaterialType,
                                        IdProcessCost = originalTransactionDetail.IdProcessCost,
                                        IdProductType = originalTransactionDetail.IdProductType,
                                        IdProductUnit = originalTransactionDetail.IdProductUnit,
                                        IdSaleUnit = originalTransactionDetail.IdSaleUnit,
                                        IsNotch = originalTransactionDetail.IsNotch,
                                        IsWrap = originalTransactionDetail.IsWrap,
                                        NotchArea = originalTransactionDetail.NotchArea,
                                        NotchDegree = originalTransactionDetail.NotchDegree,
                                        NotchSide = originalTransactionDetail.NotchSide,
                                        PalletOverhang = originalTransactionDetail.PalletOverhang,
                                        SideA = originalTransactionDetail.SideA,
                                        SideB = originalTransactionDetail.SideB,
                                        SideC = originalTransactionDetail.SideC,
                                        SideD = originalTransactionDetail.SideD,
                                    };

                                    resultCount++;
                                    PMTsDbContext.TransactionsDetail.Add(transactionDetail);
                                }

                                #endregion set transaction detail

                                #region set running No

                                runningList = UpdateRunningNo(newMasterdata.MaterialType, factoryCode, username, company.SaleOrg);
                                PMTsDbContext.RunningNo.UpdateRange(runningList);
                                resultCount += runningList.Count;

                                #endregion set running No

                                #region Update all and check update list

                                using (var transaction = PMTsDbContext.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var checkSaveChange = PMTsDbContext.SaveChanges();

                                        if (checkSaveChange < resultCount)
                                            throw new Exception($"update fail prgress : {checkSaveChange} of {resultCount}");

                                        transaction.Commit();
                                    }
                                    catch (Exception)
                                    {
                                        transaction.Rollback();
                                    }
                                }

                                ////update all about copy to database
                                //var checkSaveChange = PMTsDbContext.SaveChanges();// + routingChange;

                                //if (checkSaveChange < resultCount)
                                //    throw new Exception($"update fail prgress : {checkSaveChange} of {resultCount}");

                                #endregion Update all and check update list

                                #region Sent result

                                //set new created data
                                newChangeBoardNewMaterial = new ChangeBoardNewMaterial
                                {
                                    IsCreatedSuccess = true,
                                    ErrorMessage = string.Empty,
                                    MaterialNo = newMaterialNo,
                                    PC = newMasterdata.Pc,
                                    Price = validData.Price,
                                    CustName = newMasterdata.CustName,
                                    Change = newMasterdata.Change,
                                    Description = newMasterdata.Description,
                                    Hierarchy = newMasterdata.Hierarchy + newMasterdata.Code,
                                    Cost = cost != null ? cost.CostPerTon : 0,
                                    Board = newMasterdata.Board,
                                    BoxType = newMasterdata.BoxType,
                                    Machine = machineInUse,
                                    CopyMaterialNo = copyMaterialNo,
                                    CopyFactoryCode = originalMasterdata.FactoryCode,
                                    MaterialType = validData.MaterialType,
                                    PdisStatus = pdisStatus,
                                    BoardAlternative = string.Empty
                                };

                                results.Add(newChangeBoardNewMaterial);

                                #endregion Sent result
                            }
                        }
                        else
                        {
                            //set faild model to result
                            validData.ErrorMessage = errorMassage;
                            validData.IsCreatedSuccess = false;
                            validData.Cost = 0;
                            results.Add(validData);
                        }
                    }
                    catch (Exception ex)
                    {
                        validData.ErrorMessage = ex.Message;
                        validData.IsCreatedSuccess = false;
                        validData.Cost = 0;
                        results.Add(validData);
                        continue;
                    }
                }

                #endregion Copy new material
            }

            return results;
        }

        public MasterData GetOutsourceFromMaterialNoAndSaleOrg(string factoryCodeOutsource, string materialNo, string saleOrg)
        {
            return PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCodeOutsource && m.MaterialNo == materialNo && m.SaleOrg == saleOrg);
        }

        public IEnumerable<MasterData> UpdateMasterDatasFromExcelFile(List<MasterData> masterDatas)
        {
            var results = new List<MasterData>();
            foreach (var masterdata in masterDatas)
            {
                var existMasterData = PMTsDbContext.MasterData.FirstOrDefault(r => r.FactoryCode == masterdata.FactoryCode && r.MaterialNo == masterdata.MaterialNo);
                if (existMasterData != null)
                {
                    existMasterData.PartNo = string.IsNullOrEmpty(masterdata.PartNo) ? existMasterData.PartNo : masterdata.PartNo;
                    existMasterData.Pc = string.IsNullOrEmpty(masterdata.Pc) ? existMasterData.Pc : masterdata.Pc;
                    existMasterData.Description = string.IsNullOrEmpty(masterdata.Description) ? existMasterData.Description : masterdata.Description;
                    string saleText1 = string.Empty, saleText2 = string.Empty, saleText3 = string.Empty, saleText4 = string.Empty;
                    if (string.IsNullOrEmpty(masterdata.SaleText1))
                    {
                        saleText1 = masterdata.SaleText1.Length > 0 ? masterdata.SaleText1[..(masterdata.SaleText1.Length / 40 >= 1 ? 40 : masterdata.SaleText1.Length % 40)] : existMasterData.SaleText1;
                        saleText2 = masterdata.SaleText1.Length > 40 ? masterdata.SaleText1.Substring(40, masterdata.SaleText1.Length / 80 >= 1 ? 40 : masterdata.SaleText1.Length % 40) : existMasterData.SaleText2;
                        saleText3 = masterdata.SaleText1.Length > 80 ? masterdata.SaleText1.Substring(80, masterdata.SaleText1.Length / 120 >= 1 ? 40 : masterdata.SaleText1.Length % 40) : existMasterData.SaleText3;
                        saleText4 = masterdata.SaleText1.Length > 120 ? masterdata.SaleText1.Substring(120, masterdata.SaleText1.Length / 160 >= 1 ? 40 : masterdata.SaleText1.Length % 40) : existMasterData.SaleText4;
                    }
                    existMasterData.SaleText1 = saleText1;
                    existMasterData.SaleText2 = saleText2;
                    existMasterData.SaleText3 = saleText3;
                    existMasterData.SaleText4 = saleText4;
                    existMasterData.Change = string.IsNullOrEmpty(masterdata.Change) ? existMasterData.Change : masterdata.Change;
                    existMasterData.Hardship = masterdata.Hardship.HasValue ? existMasterData.Hardship : masterdata.Hardship;
                    existMasterData.Bun = masterdata.Bun.HasValue ? existMasterData.Bun : masterdata.Bun;
                    existMasterData.BunLayer = masterdata.BunLayer.HasValue ? existMasterData.BunLayer : masterdata.BunLayer;
                    existMasterData.LayerPalet = masterdata.LayerPalet.HasValue ? existMasterData.LayerPalet : masterdata.LayerPalet;
                    existMasterData.BoxPalet = masterdata.BoxPalet.HasValue ? existMasterData.BoxPalet : masterdata.BoxPalet;
                    existMasterData.UpdatedBy = string.IsNullOrEmpty(masterdata.UpdatedBy) ? existMasterData.UpdatedBy : masterdata.UpdatedBy;
                    existMasterData.LastUpdate = masterdata.LastUpdate.HasValue ? existMasterData.LastUpdate : masterdata.LastUpdate;
                    existMasterData.User = "ZZ_Import";
                    results.Add(existMasterData);
                }

                #region Update all masterdata

                PMTsDbContext.UpdateRange(results);
                using var transaction = PMTsDbContext.Database.BeginTransaction();
                try
                {
                    var checkSaveChange = PMTsDbContext.SaveChanges();

                    if (checkSaveChange < results.Count)
                        throw new Exception($"update fail progress : {checkSaveChange} of {results.Count}");

                    transaction.Commit();
                }
                catch (Exception)
                {
                    results = [];
                }

                #endregion Update all masterdata
            }
            return results;
        }

        public IEnumerable<Routing> UpdateRoutingsFromExcelFile(List<Routing> routings)
        {
            var results = new List<Routing>();
            foreach (var routing in routings)
            {
                var existRouting = PMTsDbContext.Routing.FirstOrDefault(r => r.FactoryCode == routing.FactoryCode && r.MaterialNo == routing.MaterialNo && r.SeqNo == routing.SeqNo);
                if (existRouting != null)
                {
                    existRouting.Alternative1 = string.IsNullOrEmpty(routing.Alternative1) ? existRouting.Alternative1 : routing.Alternative1;
                    existRouting.Alternative2 = string.IsNullOrEmpty(routing.Alternative1) ? existRouting.Alternative1 : routing.Alternative1;
                    existRouting.Alternative3 = string.IsNullOrEmpty(routing.Alternative1) ? existRouting.Alternative1 : routing.Alternative1;
                    existRouting.Alternative4 = string.IsNullOrEmpty(routing.Alternative1) ? existRouting.Alternative1 : routing.Alternative1;
                    existRouting.Alternative5 = string.IsNullOrEmpty(routing.Alternative1) ? existRouting.Alternative1 : routing.Alternative1;
                    existRouting.RemarkInprocess = string.IsNullOrEmpty(routing.RemarkInprocess) ? existRouting.RemarkInprocess : routing.RemarkInprocess;
                    existRouting.PaperWidth = routing.PaperWidth.HasValue ? existRouting.PaperWidth : routing.PaperWidth;
                    existRouting.CutNo = routing.PaperWidth.HasValue ? existRouting.PaperWidth : routing.PaperWidth;
                    existRouting.Trim = routing.PaperWidth.HasValue ? existRouting.PaperWidth : routing.PaperWidth;
                    existRouting.PercenTrim = routing.PaperWidth.HasValue ? existRouting.PaperWidth : routing.PaperWidth;
                    existRouting.UpdatedBy = string.IsNullOrEmpty(routing.UpdatedBy) ? existRouting.UpdatedBy : routing.UpdatedBy;
                    existRouting.UpdatedDate = routing.UpdatedDate.HasValue ? existRouting.UpdatedDate : routing.UpdatedDate;
                    var machine = PMTsDbContext.Routing.FirstOrDefault(m => m.FactoryCode == routing.FactoryCode && m.Machine == routing.Machine);
                    if (machine != null)
                    {
                        existRouting.MatCode = machine.MatCode;
                        existRouting.PlanCode = machine.PlanCode;
                        results.Add(existRouting);
                    }
                }
            }

            #region Update all routing

            PMTsDbContext.UpdateRange(results);
            using (var transaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var checkSaveChange = PMTsDbContext.SaveChanges();

                    if (checkSaveChange < results.Count)
                        throw new Exception($"update fail progress : {checkSaveChange} of {results.Count}");

                    transaction.Commit();
                }
                catch (Exception)
                {
                    results = [];
                }
            }

            #endregion Update all routing

            return results;
        }

        #region function

        private double? GetBasisWeight(string code, string flute, string factoryCode)
        {
            double? bWeight = 0;

            try
            {
                if (flute.Contains('H'))
                {
                    var board = PMTsDbContext.BoardCombine.Where(b => b.Code == code).FirstOrDefault();
                    bWeight = board.Weight;
                }
                else
                {
                    var getBoard = PMTsDbContext.BoardCombine.FirstOrDefault(b => b.Code == code);
                    var board = new List<BoardSpecWeight>();

                    //var fluTr = PMTsDbContext.FluteTr.Where(f => f.FactoryCode == FactoryCode && f.FluteCode == flute)
                    //                                 .Select((j, i) => new { j.FactoryCode, j.FluteCode, rn = i + 1, j.Station, j.Tr });

                    if (getBoard != null)
                    {
                        string[] ArrBoard = getBoard.Board.Split("/");

                        var join = ArrBoard.GroupJoin(PMTsDbContext.PaperGrade
                                                             , spec => spec.ToString(), pp => pp.Grade
                                                             , (spec, pp) => new { spec, pp })
                                                             .SelectMany(x => x.pp.DefaultIfEmpty(), (b, p) => new { b, p });

                        foreach (var item in join)
                        {
                            if (item.b.spec != "")
                            {
                                var boardSpec = new BoardSpecWeight();

                                if (item.p == null)
                                {
                                    if (item.b.spec.ToString().Length == 5)
                                    {
                                        boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(2, 3));
                                    }
                                    else if (item.b.spec.ToString().Length == 6)
                                    {
                                        boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(3, 3));
                                    }
                                    else if (item.b.spec.ToString().Length == 4)
                                    {
                                        boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(2, 2));
                                    }
                                }
                                else
                                {
                                    boardSpec.BasicWeight = item.p.BasicWeight;
                                }
                                boardSpec.Grade = item.b.spec.ToString();
                                boardSpec.PaperDes = item.p == null ? boardSpec.Grade : item.p.PaperDes;
                                boardSpec.Layer = item.p == null ? 1 : item.p.Layer;
                                board.Add(boardSpec);
                            }
                        }
                    }

                    var flu = PMTsDbContext.FluteTr.Where(f => f.FluteCode == flute && f.FactoryCode == factoryCode).OrderBy(f => f.Item).ToList();

                    int i = 0, j = 0;

                    if (board.Count > flu.Count)
                        j = flu.Count;
                    else if (flu.Count > board.Count)
                        j = board.Count;
                    else
                        j = board.Count;

                    if (flu.Count != 0)
                    {
                        foreach (var item in board)
                        {
                            if (i < j)
                            {
                                bWeight += (item.BasicWeight * flu[i].Tr);
                                i++;
                            }
                            else
                            {
                                bWeight += (item.BasicWeight);
                                i++;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null; ;
            }

            return bWeight;
        }

        private string GenMatNo(string materialCode, string factoryCode)
        {
            var materialType = PMTsDbContext.MaterialType.FirstOrDefault(m => m.MatCode == materialCode);
            var groupId = materialType != null ? materialType.GroupId : throw new Exception("Can't genarate new material number.");
            var Running = PMTsDbContext.RunningNo.Where(w => w.GroupId == groupId && w.FactoryCode == factoryCode).FirstOrDefault();

            try
            {
                if (Running == null)
                {
                    //var ex = new ArgumentNullException($"Running No does not exist.");
                    throw new Exception($"Running No does not exist.");
                }
                if (Running.Running >= Running.EndNo)
                {
                    //var ex = new ArgumentOutOfRangeException(nameof(Running), $"Limited Running No. ,Please contact admin to correct.");
                    throw new Exception($"Limited Running No. ,Please contact admin to correct.");
                }

                int mat_no;
                string mat_str, Material_No;

                mat_no = Running.Running + 1;
                mat_str = Convert.ToString(mat_no);
                mat_str = mat_str.PadLeft(Running.Length, '0');
                Material_No = Running.Fix + mat_str;

                return Material_No;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private List<RunningNo> UpdateRunningNo(string materialCode, string fatoryCode, string username, string saleOrg)
        {
            var runningNos = new List<RunningNo>();
            var Running = 0;

            var companyProfiles = new List<CompanyProfile>();
            companyProfiles = [.. PMTsDbContext.CompanyProfile.Where(c => c.SaleOrg == saleOrg)];

            // Get MaterialType GroupId
            var materialType = PMTsDbContext.MaterialType.FirstOrDefault(m => m.MatCode == materialCode);
            var groupId = materialType != null ? materialType.GroupId : throw new Exception("Can't genarate new material number.");
            // Get Original RunningNo
            var runningNoOriginal = PMTsDbContext.RunningNo.FirstOrDefault(r => r.FactoryCode == fatoryCode && r.GroupId == groupId);

            if (runningNoOriginal != null)
            {
                Running = runningNoOriginal.Running + 1;
            }

            foreach (var companyProfile in companyProfiles)
            {
                // Get RunningNo
                var runningNoObject = PMTsDbContext.RunningNo.FirstOrDefault(r => r.FactoryCode == companyProfile.Plant && r.GroupId == groupId);

                // Running Number
                runningNoObject.Running = Running;
                runningNoObject.UpdatedBy = username;
                runningNoObject.UpdatedDate = DateTime.Now;

                //Set Update RunningNo
                runningNos.Add(runningNoObject);
            }

            runningNos.ForEach(r => r.UpdatedDate = DateTime.Now);
            runningNos.ForEach(r => r.UpdatedBy = username);

            return runningNos;
        }

        private void SetPaperGrade(ref MasterData masterData, string factoryCode, string flute, string board)
        {
            if (board != null)
            {
                //var ppg = new List<PaperGrade>();
                //var fluteTr = new List<FluteTr>();
                string[] st = [];
                int i = 0;

                st = board.Split("/");
                var ppg = PMTsDbContext.PaperGrade.ToList();
                var fluteTr = PMTsDbContext.FluteTr.Where(f => f.FluteCode == flute && f.FactoryCode == factoryCode).OrderBy(f => f.Item).ToList();
                masterData.Gl = null;
                masterData.Glweigth = null;
                masterData.Bm = null;
                masterData.Bmweigth = null;
                masterData.Bl = null;
                masterData.Blweigth = null;
                masterData.Cm = null;
                masterData.Cmweigth = null;
                masterData.Cl = null;
                masterData.Clweigth = null;
                masterData.Dm = null;
                masterData.Dmweigth = null;
                masterData.Dl = null;
                masterData.Dlweigth = null;

                #region Check and Set Paper Grade

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 1)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Gl = paper.Paper;
                            masterData.Glweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Gl = st[i][..^3];
                                    masterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Gl = st[i][..^2];
                                    masterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Gl = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Gl = st[i][..^3];
                        masterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 2)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Bm = paper.Paper;
                            masterData.Bmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Bm = st[i][..^3];
                                    masterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Bm = st[i][..^2];
                                    masterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Bm = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Bm = st[i][..^3];
                        masterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 3)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Bl = paper.Paper;
                            masterData.Blweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Bl = st[i][..^3];
                                    masterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Bl = st[i][..^2];
                                    masterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Bl = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Bl = st[i][..^3];
                        masterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 4)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Cm = paper.Paper;
                            masterData.Cmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Cm = st[i][..^3];
                                    masterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Cm = st[i][..^2];
                                    masterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Cm = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Cm = st[i][..^3];
                        masterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 5)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Cl = paper.Paper;
                            masterData.Clweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Cl = st[i][..^3];
                                    masterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Cl = st[i][..^2];
                                    masterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Cl = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Cl = st[i][..^3];
                        masterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 6)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Dm = paper.Paper;
                            masterData.Dmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Dm = st[i][..^3];
                                    masterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Dm = st[i][..^2];
                                    masterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Dm = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Dm = st[i][..^3];
                        masterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Length > i)
                {
                    if (fluteTr[i].Item == 7)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            masterData.Dl = paper.Paper;
                            masterData.Dlweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    masterData.Dl = st[i][..^3];
                                    masterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    masterData.Dl = st[i][..^2];
                                    masterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    masterData.Dl = st[i][..];
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Length > i)
                {
                    if (st[i].Length >= 5)
                    {
                        masterData.Dl = st[i][..^3];
                        masterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                #endregion Check and Set Paper Grade
            }
        }

        private void SetBoardUse(ref BoardUse boardUse, string board, string flute)
        {
            string[] ArrBoard = board.Split("/");

            if (ArrBoard.Length > 0)
                boardUse.Gl = ArrBoard[0];

            if (ArrBoard.Length > 1)
            {
                if (flute == "C")
                {
                    boardUse.Bm = null;
                    boardUse.Cm = ArrBoard[1];
                }
                else
                {
                    boardUse.Bm = ArrBoard[1];
                }
            }

            if (ArrBoard.Length > 2)
            {
                if (flute == "C")
                {
                    boardUse.Bl = null;
                    boardUse.Cl = ArrBoard[2];
                }
                else
                {
                    boardUse.Bl = ArrBoard[2];
                }
            }

            if (ArrBoard.Length > 3)
                boardUse.Cm = ArrBoard[3];

            if (ArrBoard.Length > 4)
                boardUse.Cl = ArrBoard[4];

            if (ArrBoard.Length > 5)
                boardUse.Dm = ArrBoard[5];

            if (ArrBoard.Length > 6)
                boardUse.Dl = ArrBoard[6];
        }

        #region EANBarcode

        private string GetEanCode(string materialNo, string _saleOrg)
        {
            if (string.IsNullOrEmpty(materialNo))
            {
                return string.Empty;
            }

            string MatString = materialNo[^5..];

            string[] Ean = new string[14];
            Ean[0] = "885" + _saleOrg + MatString;
            Ean[1] = Ean[0][..1];
            Ean[2] = Ean[0].Substring(1, 1);
            Ean[3] = Ean[0].Substring(2, 1);
            Ean[4] = Ean[0].Substring(3, 1);
            Ean[5] = Ean[0].Substring(4, 1);
            Ean[6] = Ean[0].Substring(5, 1);
            Ean[7] = Ean[0].Substring(6, 1);

            Ean[8] = Ean[0].Substring(7, 1);
            Ean[9] = Ean[0].Substring(8, 1);
            Ean[10] = Ean[0].Substring(9, 1);
            Ean[11] = Ean[0].Substring(10, 1);
            Ean[12] = Ean[0].Substring(11, 1);
            // Ean[12] = "";

            int[] EanLevel = new int[5];
            EanLevel[0] = 0;
            EanLevel[1] = 0;
            EanLevel[2] = 0;
            //long result;
            for (int i = 1; i < 13; i++)
            {
                if ((i % 2) == 0)
                {
                    // EanLevel[1] = EanLevel[1] + Int32.Parse(Ean[i]);
                    EanLevel[1] = EanLevel[1] + Convert.ToInt32(Ean[i], 16);
                }
                else
                {
                    //result = Convert.ToInt64(Ean[i]);
                    try
                    {
                        EanLevel[0] = EanLevel[0] + Convert.ToInt32(Ean[i], 16);
                    }
                    catch (Exception)
                    {
                        EanLevel[0] = EanLevel[0] + 0; ///Ean[7] ==G //โรงงาน Din
                        //throw;
                    }
                }
            }

            EanLevel[2] = EanLevel[1] * 3;
            EanLevel[3] = EanLevel[0] + EanLevel[2];
            EanLevel[4] = EanLevel[3] % 10;
            if (EanLevel[4] == 0)
            {
                Ean[13] = "0";
            }
            else
            {
                Ean[13] = (10 - EanLevel[4]).ToString();
            }
            var EANCODE = Ean[0] + Ean[13];
            return EANCODE;
        }

        #endregion EANBarcode

        #endregion function

        public IEnumerable<MasterData> GetMasterDataByMaterialAddtag(string factoryCode, string ddlSearch, string inputSerach)
        {
            {
                switch (ddlSearch)
                {
                    case "Material_No":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.MaterialNo.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }
                    case "Description":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.Description.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }
                    case "PC":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.Pc.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }
                    case "Board":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.Board.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }

                    case "Box_Type":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.BoxType.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }
                    case "Cust_Name":
                        {
                            return PMTsDbContext.MasterData.Where(m => m.CustName.Contains(inputSerach) && m.FactoryCode == factoryCode && m.PdisStatus != "X").ToList();
                        }
                    default:
                        {
                            return PMTsDbContext.MasterData.Where(m => m.MaterialNo == "").ToList();
                        }
                }
            }
        }

        public IEnumerable<MasterData> GetMasterDataByUser(string factory, string user)
        {
            return PMTsDbContext.MasterData.Where(m => m.User.Contains(user) && m.FactoryCode.Equals(factory)).OrderByDescending(m => m.LastUpdate).Take(100).AsNoTracking().ToList();
        }

        public void UpdateMasterDataByChangePalletSize(string factoryCode, string userLogin, MasterData masterData)
        {
            var existMasterData = PMTsDbContext.MasterData.Where(m => m.Id.Equals(masterData.Id)).AsNoTracking().FirstOrDefault();
            if (existMasterData != null)
            {
                existMasterData.Bun = masterData.Bun;
                existMasterData.BunLayer = masterData.BunLayer;
                existMasterData.LayerPalet = masterData.LayerPalet;
                existMasterData.BoxPalet = masterData.BoxPalet;
                var directory = Path.GetDirectoryName(existMasterData.PalletizationPath);
                if (string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(masterData.PalletizationPath))
                {
                    throw new Exception("Orginal palletization path is not found.");
                }
                existMasterData.PalletizationPath = string.IsNullOrEmpty(masterData.PalletizationPath) ? existMasterData.PalletizationPath : Path.Combine(directory, string.Join("", PalletPathRegex().Split(masterData.PalletizationPath)));
                existMasterData.UpdatedBy = "pallet:" + userLogin;
                existMasterData.User = "pallet:" + userLogin;
                existMasterData.LastUpdate = DateTime.Now;
                PMTsDbContext.MasterData.Update(existMasterData);
                PMTsDbContext.SaveChanges();
            }
        }

        public IEnumerable<MasterDataRoutingModel> GetMasterDataRoutingsByMaterialNos(IConfiguration configuration, string factoryCode, List<string> materialNos)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT
                    m.Material_No as MaterialNo
                    ,m.PC
	                ,m.Sale_Org as SaleOrg
	                ,m.Plant
	                ,m.Cust_Code as CustCode
	                ,m.Cus_ID as CusId
	                ,m.Cust_Name  as CustName
	                ,m.Description
	                ,m.Flute + ' ' + m.Board as Board
	                ,m.Box_Type as BoxType
	                ,Machine = STUFF((SELECT TOP 4 machine + ', '
				        FROM Routing r
				        WHERE r.Material_No = m.Material_No and r.PDIS_Status != 'x'  and r.FactoryCode = '{0}'
				        ORDER BY r.Seq_No
				        FOR XML PATH('')), 1, 0, '')
	                ,m.LastUpdate
	                ,m.CreateDate
	                ,m.Tran_Status as TranStatus
	                ,m.PDIS_Status as PDISStatus
                    ,t.MatSaleOrg as MatSaleOrg
                    from MasterData m left outer join Transactions_Detail t
                    on m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode
                    where  m.PDIS_Status != 'x'  and m.FactoryCode = '{1}' and m.Material_No IN " + $"('{string.Join("','", [.. materialNos])}')" +
                "order by m.LastUpdate desc";

            string message = string.Format(sql,
               factoryCode,
               factoryCode
                );

            return db.Query<MasterDataRoutingModel>(message).ToList();
        }

        public IEnumerable<MasterData> SearchMasterDataByMaterialNo(string MaterialNo, string factoryCode)
        {
            var materialType = new List<string> { "81", "82" };
            var pdisStatus = new List<string>() { "X" };
            //var pdisStatus = new List<string>() { "N", "X" };
            return PMTsDbContext.MasterData.Where(x => x.MaterialNo.Equals(MaterialNo) && x.FactoryCode.Equals(factoryCode) && materialType.Contains(x.MaterialType) && !pdisStatus.Contains(x.PdisStatus)).ToList();
        }

        public IEnumerable<MasterData> GetMasterDataListByMaterialNoAndPC(IConfiguration configuration, string FactoryCode, string MaterialNo, string PC)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            var sql = new StringBuilder(@"
                    SELECT
                       [Id]
                      ,[FactoryCode]
                      ,[Material_No] as MaterialNo
                      ,[Part_No] as PartNo
                      ,[PC]
                      ,[Hierarchy]
                      ,[Sale_Org] as SaleOrg
                      ,[Plant]
                      ,[Cust_Code] as CustCode
                      ,[Cus_ID] as CusId
                      ,[Cust_Name] as CustName
                      ,[Description]
                      ,[Sale_Text1] as SaleText1
                      ,[Sale_Text2] as SaleText2
                      ,[Sale_Text3] as SaleText3
                      ,[Sale_Text4] as SaleText4
                      ,[Change]
                      ,[Language]
                      ,[Ind_Grp] as IndGrp
                      ,[Ind_Des] as IndDes
                      ,[Material_Type] as MaterialType
                      ,[Print_Method] as PrintMethod
                      ,[TwoPiece]
                      ,[Flute]
                      ,[Code]
                      ,[Board]
                      ,[GL]
                      ,[GLWeigth]
                      ,[BM]
                      ,[BMWeigth]
                      ,[BL]
                      ,[BLWeigth]
                      ,[CM]
                      ,[CMWeigth]
                      ,[CL]
                      ,[CLWeigth]
                      ,[DM]
                      ,[DMWeigth]
                      ,[DL]
                      ,[DLWeigth]
                      ,[Wid]
                      ,[Leg]
                      ,[Hig]
                      ,[Box_Type] as BoxType
                      ,[RSC_Style] as RscStyle
                      ,[Pro_Type] ProType
                      ,[JoinType]
                      ,[Status_Flag] as StatusFlag
                      ,[Priority_Flag] as PriorityFlag
                      ,[Wire]
                      ,[Outer_Join] as OuterJoin
                      ,[CutSheetLeng]
                      ,[CutSheetWid]
                      ,[Sheet_Area] as SheetArea
                      ,[Box_Area] as BoxArea
                      ,[ScoreW1]
                      ,[Scorew2]
                      ,[Scorew3]
                      ,[Scorew4]
                      ,[Scorew5]
                      ,[Scorew6]
                      ,[Scorew7]
                      ,[Scorew8]
                      ,[Scorew9]
                      ,[Scorew10]
                      ,[Scorew11]
                      ,[Scorew12]
                      ,[Scorew13]
                      ,[Scorew14]
                      ,[Scorew15]
                      ,[Scorew16]
                      ,[JointLap]
                      ,[ScoreL2]
                      ,[ScoreL3]
                      ,[ScoreL4]
                      ,[ScoreL5]
                      ,[ScoreL6]
                      ,[ScoreL7]
                      ,[ScoreL8]
                      ,[ScoreL9]
                      ,[Slit]
                      ,[No_Slot] as NoSlot
                      ,[Bun]
                      ,[BunLayer]
                      ,[LayerPalet]
                      ,[BoxPalet]
                      ,[Weight_Sh] as WeightSh
                      ,[Weight_Box] as WeightBox
                      ,[SparePercen]
                      ,[SpareMax]
                      ,[SpareMin]
                      ,[LeadTime]
                      ,[Piece_Set] as PieceSet
                      ,[Sale_UOM] as SaleUom
                      ,[BOM_UOM] as BomUom
                      ,[Hardship]
                      ,[PalletSize]
                      ,[Palletization_Path] as PalletizationPath
                      ,[PrintMaster_Path] as PrintMasterPath
                      ,[DiecutPict_Path] as DiecutPictPath
                      ,[FGPic_Path] as FgpicPath
                      ,[CreateDate]
                      ,[CreatedBy]
                      ,[LastUpdate]
                      ,[UpdatedBy]
                      ,[User]
                      ,[Plt_Leg_Double] as PltLegDouble
                      ,[Plt_Double_axle] as PltDoubleAxle
                      ,[Plt_Leg_Single] as PltLegSingle
                      ,[Plt_Single_axle] as PltSingleAxle
                      ,[Plt_Floor_Above] as PltFloorAbove
                      ,[Plt_Floor_Under] as PltFloorUnder
                      ,[Plt_Beam] as PltBeam
                      ,[Plt_Axle_Height] as PltAxleHeight
                      ,[EanCode]
                      ,[PDIS_Status] as PdisStatus
                      ,[Tran_Status] as TranStatus
                      ,[SAP_Status] as SapStatus
                      ,[NewH]
                      ,[Pur_Txt1] as PurTxt1
                      ,[Pur_Txt2] as PurTxt2
                      ,[Pur_Txt3] as PurTxt3
                      ,[Pur_Txt4] as PurTxt4
                      ,[UnUpgrad_Board] as UnUpgradBoard
                      ,[High_Group] as HighGroup
                      ,[High_Value] as HighValue
                      ,[ChangeInfo]
                      ,[Piece_Patch] as PiecePatch
                      ,[BoxHandle]
                      ,[PSM_ID] as PsmId
                      ,[PicPallet]
                      ,[ChangeHistory]
                      ,[CustComment]
                      ,[MaterialComment]
                      ,[CutSheetWid_Inch] as CutSheetWidInch
                      ,[CutSheetLeng_Inch] as CutSheetLengInch
                      ,[Joint_ID] as JointId
                      ,[CustInvType]
                      ,[CIPInvType]
                      ,[IsTransfer]
                      ,[AttachFileMoPath]
                      ,[MatCopy]
                      ,[TopSheet_Material] as TopSheetMaterial
                      ,[JoinCharacter]
                      ,[Perforate1]
                      ,[Perforate2]
                      ,[Perforate3]
                      ,[Perforate4]
                      ,[Perforate5]
                      ,[Perforate6]
                      ,[Perforate7]
                      ,[Perforate8]
                      ,[Perforate9]
                      ,[Perforate10]
                      ,[Perforate11]
                      ,[Perforate12]
                      ,[Perforate13]
                      ,[Perforate14]
                      ,[Perforate15]
                      ,[Perforate16]
                      ,[Semi1_Path] as Semi1Path
                      ,[Semi2_Path] as Semi2Path
                      ,[Semi3_Path] as Semi3Path
                      ,[SemiFilePdf_Path] as SemiFilePdfPath
                      ,[NoneStandardPaper]
                      ,[TagBundle]
                      ,[TagPallet]
                      ,[NoTagBundle]
                      ,[HeadTagBundle]
                      ,[FootTagBundle]
                      ,[HeadTagPallet]
                      ,[FootTagPallet]
                      ,[PerforateGap]
                      ,[WorkType]
                      ,[BOIStatus]
                      ,[BoxPacking]
                      ,[FscCode]
                      ,[FscFgCode]
                      ,[RpacLob]
                      ,[RpacProgram]
                      ,[RpacBrand]
                      ,[RpacPackagingType]
                    from MasterData
                    where FactoryCode = '@FactoryCode'");

            if (!string.IsNullOrEmpty(FactoryCode))
            {
                sql.Replace("@FactoryCode", FactoryCode);
            }

            if (!string.IsNullOrEmpty(MaterialNo))
            {
                sql.AppendFormat(@" and Material_No like '%{0}%'", MaterialNo);
            }
            if (!string.IsNullOrEmpty(PC))
            {
                sql.AppendFormat(@" and PC like '%{0}%'", PC);
            }

            return db.Query<MasterData>(sql.ToString()).ToList();
        }

        public void UpdateTranStatusFromHandshake(IConfiguration conn, string FactoryCode, string MaterialNo, bool Status, string Username)
        {
            try
            {
                using IDbConnection db = new SqlConnection(conn.GetConnectionString("PMTsConnect"));
                if (db.State == ConnectionState.Closed)
                    db.Open();

                var sqlupdate = @"UPDATE MasterData SET Tran_Status = 1 ,SAP_Status = 1 WHERE FactoryCode = '" + FactoryCode + "' and Material_No in (" + MaterialNo + ") ";
                db.Execute(sqlupdate);

                //using (SqlCommand cmd = new SqlCommand(sqlupdate, conn))
                //{
                //    cmd.CommandText = sqlupdate;

                //    cmd.ExecuteNonQuery();
                //    conn.Close();
                //}

                db.Dispose();
                db.Close();
            }
            catch
            {
            }
        }

        //Update Transtatus from Handshake
        public void UpdateSizeDimension(string FactoryCode, string MaterialNo, string Username, string SizeDimension)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var some = PMTsDbContext.MasterData.FirstOrDefault(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo);
                some.SizeDimensions = SizeDimension;
                some.UpdatedBy = Username;
                some.LastUpdate = DateTime.Now;
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<MasterData> GetMasterDataListByDateTime(string factoryCode, string DateFrom, string DateTo)
        {
            DateTime.TryParseExact(DateFrom, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out var from);
            DateTime.TryParseExact(DateTo, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out var to);

            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                return PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.LastUpdate >= from && m.LastUpdate <= to).AsNoTracking().ToList();
            }
            else
            {
                return new List<MasterData>();
            }
        }

        public IEnumerable<string> GetBoardDistinctFromMasterData(string factoryCode)
        {
            return PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X").Select(s => s.Board).Distinct().AsNoTracking().ToList();
        }

        public object GetCustomerDistinctFromMasterData(string factoryCode)
        {
            return PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X").Select(s => new { CusId = s.CusId, CustCode = s.CustCode, CustName = s.CustName })
                .Distinct().AsNoTracking().ToList();
        }

        public List<ChangeBoardNewMaterial> GetForTemplateChangeBoardNewMaterials(string factoryCode, SearchMaterialTemplateParam searchMaterialTemplateParam)
        {
            List<ChangeBoardNewMaterial> result = new List<ChangeBoardNewMaterial>();
            //if (searchMaterialTemplateParam.TypeSearch == "Board")
            //{
            //    result = PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X" && searchMaterialTemplateParam.Boards.Contains(m.Board))
            //        .GroupJoin(PMTsDbContext.BoardAlternative, m => m.MaterialNo, b => b.MaterialNo, (m, b) => new { MasterData = m, BoardAlternative = b.DefaultIfEmpty() })
            //        .Select(s => new ChangeBoardNewMaterial
            //        {
            //            CopyMaterialNo = s.MasterData.MaterialNo,
            //            PC = s.MasterData.Pc,
            //            Flute = s.MasterData.Flute,
            //            NewBoard = s.MasterData.Board,
            //            HighValue = s.MasterData.HighValue,
            //            BoardAlternative = s.BoardAlternative.Where(w => w.MaterialNo == s.MasterData.MaterialNo).Select(ss => ss.BoardName).FirstOrDefault(),
            //            Change = s.MasterData.Change
            //        }).ToList();
            //}
            //else if (searchMaterialTemplateParam.TypeSearch == "Grade")
            //{
            //    result = PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X" && searchMaterialTemplateParam.Grades.Any(a => m.Board.Contains(a)))
            //        .GroupJoin(PMTsDbContext.BoardAlternative, m => m.MaterialNo, b => b.MaterialNo, (m, b) => new { MasterData = m, BoardAlternative = b.DefaultIfEmpty() })
            //        .Select(s => new ChangeBoardNewMaterial
            //        {
            //            CopyMaterialNo = s.MasterData.MaterialNo,
            //            PC = s.MasterData.Pc,
            //            Flute = s.MasterData.Flute,
            //            NewBoard = s.MasterData.Board,
            //            HighValue = s.MasterData.HighValue,
            //            BoardAlternative = s.BoardAlternative.Where(w => w.MaterialNo == s.MasterData.MaterialNo).Select(ss => ss.BoardName).FirstOrDefault(),
            //            Change = s.MasterData.Change
            //        }).ToList();
            //}
            //else if (searchMaterialTemplateParam.TypeSearch == "Customer")
            //{
            //    result = PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X" && searchMaterialTemplateParam.Customers.Contains(m.CusId))
            //        .GroupJoin(PMTsDbContext.BoardAlternative, m => m.MaterialNo, b => b.MaterialNo, (m, b) => new { MasterData = m, BoardAlternative = b.DefaultIfEmpty() })
            //        .Select(s => new ChangeBoardNewMaterial
            //        {
            //            CopyMaterialNo = s.MasterData.MaterialNo,
            //            PC = s.MasterData.Pc,
            //            Flute = s.MasterData.Flute,
            //            NewBoard = s.MasterData.Board,
            //            HighValue = s.MasterData.HighValue,
            //            BoardAlternative = s.BoardAlternative.Where(w => w.MaterialNo == s.MasterData.MaterialNo).Select(ss => ss.BoardName).FirstOrDefault(),
            //            Change = s.MasterData.Change
            //        }).ToList();
            //}

            IQueryable<MasterData> query = PMTsDbContext.MasterData.Where(m => m.FactoryCode == factoryCode && m.PdisStatus != "X");

            if (searchMaterialTemplateParam.Boards != null && searchMaterialTemplateParam.Boards.Any())
            {
                query = query.Where(m => searchMaterialTemplateParam.Boards.Contains(m.Board));
            }
            if (searchMaterialTemplateParam.Grades != null && searchMaterialTemplateParam.Grades.Any())
            {
                query = query.Where(m => searchMaterialTemplateParam.Grades.Any(a => m.Board.Contains(a)));
            }
            if (searchMaterialTemplateParam.Customers != null && searchMaterialTemplateParam.Customers.Any())
            {
                query = query.Where(m => searchMaterialTemplateParam.Customers.Contains(m.CusId));
            }

            result = query.GroupJoin(PMTsDbContext.BoardAlternative, m => m.MaterialNo, b => b.MaterialNo, (m, b) => new { MasterData = m, BoardAlternative = b.DefaultIfEmpty() })
                .Select(s => new ChangeBoardNewMaterial
                {
                    CopyMaterialNo = s.MasterData.MaterialNo,
                    PC = s.MasterData.Pc,
                    Flute = s.MasterData.Flute,
                    NewBoard = s.MasterData.Board,
                    HighValue = s.MasterData.HighValue,
                    BoardAlternative = s.BoardAlternative.Where(w => w.MaterialNo == s.MasterData.MaterialNo).Select(ss => ss.BoardName).FirstOrDefault(),
                    Change = s.MasterData.Change
                })
                .ToList();

            return result;
        }

        public List<CheckStatusColor> GetReportCheckStatusColor(IConfiguration config, string factoryCode, int colorId)
        {
            var color = PMTsDbContext.Color.FirstOrDefault(c => c.Id == colorId);

            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            //Execute sql query
            var sql = new StringBuilder(@"
                    SELECT m.Material_No  as MaterialNo
                    ,m.PC
	                ,m.Cust_Name as Customer
                    ,m.Description
                    ,r.Color1
                    ,r.Color1
                    ,r.Shade1
                    ,r.Color2
                    ,r.Shade2
                    ,r.Color3
                    ,r.Shade3
                    ,r.Color4
                    ,r.Shade4
                    ,r.Color5
                    ,r.Shade5
                    ,r.Color6
                    ,r.Shade6
                    ,r.Color7
                    ,r.Shade7
                    ,r.Color8
                    ,r.Shade8
                    ,r.Color9
                    ,r.Shade9
                    ,r.Color10
                    ,r.Shade10
                    from MasterData m left outer join Routing r
                    on r.Material_No = m.Material_No and r.FactoryCode = m.FactoryCode where " +
                    $"(r.Color1 = '{color.Ink}' and r.Shade1 = '{color.Shade}') or " +
                    $"(r.Color2 = '{color.Ink}' and r.Shade2 = '{color.Shade}') or " +
                    $"(r.Color3 = '{color.Ink}' and r.Shade3 = '{color.Shade}') or " +
                    $"(r.Color4 = '{color.Ink}' and r.Shade4 = '{color.Shade}') or " +
                    $"(r.Color5 = '{color.Ink}' and r.Shade5 = '{color.Shade}') or " +
                    $"(r.Color6 = '{color.Ink}' and r.Shade6 = '{color.Shade}') or " +
                    $"(r.Color7 = '{color.Ink}' and r.Shade7 = '{color.Shade}') or " +
                    $"(r.Color8 = '{color.Ink}' and r.Shade8 = '{color.Shade}') or " +
                    $"(r.Color9 = '{color.Ink}' and r.Shade9 = '{color.Shade}') or " +
                    $"(r.Color10 = '{color.Ink}' and r.Shade10 = '{color.Shade}')" +
                    $"and r.FactoryCode = '{factoryCode}' ");
            var sqlString = string.Format(sql.ToString(), factoryCode);
            return db.Query<CheckStatusColor>(sqlString).ToList();
        }

        [GeneratedRegex(@"(?:\r\n|\n|\r)")]
        private static partial Regex PalletPathRegex();

        public void UpdateLeadTime(string FactoryCode, string MaterialNo, List<Routing> model, string Username)
        {
            using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
            try
            {
                var sumLeadTime = 0;
                var macs = model.Select(s => s.Machine).ToList();
                var machine = PMTsDbContext.Machine.Where(m => m.FactoryCode == FactoryCode && macs.Contains(m.Machine1)).ToList();
                if (machine.Count > 0)
                {
                    sumLeadTime = machine.Sum(s => (s.Leadtime ?? 0));
                }

                var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && MaterialNo.Contains(s.MaterialNo)).ToList();
                some.ForEach(o => o.LeadTime = sumLeadTime);
                var response = PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
    }
}