using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
    public class ProductCatalogConfigRepository : Repository<ProductCatalogConfig>, IProductCatalogConfigRepository
    {
        public ProductCatalogConfigRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<ProductCatalogConfig> GetProductCatalogConfigs(string Factory, string Username)
        {
            return PMTsDbContext.ProductCatalogConfig.Where(x => x.FactoryCode == Factory && x.UserName == Username).ToList();
        }
        public void RemoveProductCatalogConfigs(string Factory, string Username)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                var getOldData = PMTsDbContext.ProductCatalogConfig.Where(x => x.FactoryCode == Factory && x.UserName == Username).ToList();
                PMTsDbContext.ProductCatalogConfig.RemoveRange(getOldData);
                PMTsDbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
        }

        public void UpdateRemark(IConfiguration config, ProductCatalogRemark productCatalogRemark, string FactoryCode)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            string ccc = productCatalogRemark.NonMoveMonth.ToString();
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            //string sql1 = @"SELECT PC FROM Remark where PC = LEFT('{0}',11) and FactoryCode = '{1}'";
            string sql1 = @"SELECT PC FROM Remark where PC = '{0}' and FactoryCode = '{1}'";
            //string sql2 = @"
            //                INSERT INTO [dbo].[Remark]
            //                           (
            //                            [PC]
            //                           ,[MaterialNo]
            //                           ,[Remark]
            //                           ,[NonMove]
            //                           ,[NonMoveMonth]
            //                           ,[StockWIP]
            //                           ,[StockFG]
            //                           ,[StockQA]
            //                           ,[FactoryCode]
            //                            )
            //                     VALUES
            //                           (
            //                            '{0}',
            //                            '{1}',
            //                                '{2}',
            //                                {3},
            //                                {4},
            //                                {5},
            //                                {6},
            //                                {7},
            //                                '{8}'
            //                            )
            //                ";


            //string sql3 = @"
            //               UPDATE [dbo].[Remark]
            //                    SET 
            //                             [UpdateBy] = '{0}'
            //                            ,[LastUpdate] = GETDATE()
            //                              ,[Remark] = '{1}',
            //                    {2}
            //                    {3}
            //                    {4}
            //                    {5} 
            //                     {6} 

            //                    WHERE LEFT([PC],11) = LEFT('{7}',11)  and [FactoryCode] = '{8}'
            //                ";

            string sql3 = @"
                               UPDATE [dbo].[Remark]
                                    SET 
                                             [UpdateBy] = '{0}'
                                            ,[LastUpdate] = GETDATE()
                                              ,[Remark] = '{1}',
                                    {2}
                                    {3}
                                    {4}
                                    {5} 
                                     {6} 
                                      
                                    WHERE [PC] = '{7}'  and [FactoryCode] = '{8}'
                                ";

            //string sql4 = @"
            //                INSERT INTO [dbo].[Remark]
            //                           (
            //                            [PC]
            //                           ,[MaterialNo]
            //                           ,[Remark]
            //                           ,[NonMove]
            //                           ,[StockWIP]
            //                           ,[StockFG]
            //                           ,[StockQA]
            //                            ,[FactoryCode]
            //                            )
            //                     VALUES
            //                           (
            //                            '{0}',
            //                            '{1}',
            //                                '{2}',
            //                                {3},
            //                                {4},
            //                                {5},
            //                                {6},
            //                                '{7}'
            //                            )
            //                ";


            //string sql5 = @"
            //                INSERT INTO [dbo].[Remark]
            //                           (
            //                            [PC]                                         
            //                           ,[Remark]
            //                           {0}
            //                           {1}
            //                           {2}
            //                           {3}
            //                           {4}
            //                            ,[FactoryCode]
            //                            ,[UpdateBy]
            //                            ,[LastUpdate]
            //                            )
            //                     VALUES
            //                           (
            //                            LEFT('{5}',11),
            //                             '{6}',
            //                              {7}
            //                             {8}
            //                              {9}
            //                              {10}
            //                              {11}
            //                              '{12}'
            //                                ,'{13}'
            //                                ,GETDATE()
            //                            )
            //                ";
            string sql5 = @"
                                INSERT INTO [dbo].[Remark]
                                           (
                                            [PC]                                         
                                           ,[Remark]
                                           {0}
                                           {1}
                                           {2}
                                           {3}
                                           {4}
                                            ,[FactoryCode]
                                            ,[UpdateBy]
                                            ,[LastUpdate]
                                            )
                                     VALUES
                                           (
                                            '{5}',
                                             '{6}',
                                              {7}
                                             {8}
                                              {9}
                                              {10}
                                              {11}
                                              '{12}'
                                                ,'{13}'
                                                ,GETDATE()
                                            )
                                ";


            string query5 = string.Format(sql5,
              productCatalogRemark.NonMove == null ? "" : ",[NonMove]",
               string.IsNullOrEmpty(productCatalogRemark.NonMoveMonth.ToString()) ? "" : ",[NonMoveMonth]",
                productCatalogRemark.StockWIP == null ? "" : ",[StockWIP]",
                productCatalogRemark.StockFG == null ? "" : ",[StockFG]",
                productCatalogRemark.StockQA == null ? "" : ",[StockQA]",

              productCatalogRemark.PC,
              // productCatalogRemark.MaterialNo,
              productCatalogRemark.Remark,

              productCatalogRemark.NonMove == null ? "" : string.Format("{0},", productCatalogRemark.NonMove),
              string.IsNullOrEmpty(productCatalogRemark.NonMoveMonth.ToString()) ? null : "'" + productCatalogRemark.NonMoveMonth.ToString() + "',",
               productCatalogRemark.StockWIP == null ? "" : string.Format("{0},", productCatalogRemark.StockWIP),
                productCatalogRemark.StockFG == null ? "" : string.Format("{0},", productCatalogRemark.StockFG),
               productCatalogRemark.StockQA == null ? "" : string.Format("{0},", productCatalogRemark.StockQA),

              FactoryCode,
              productCatalogRemark.MaterialNo

              );


            string query1 = string.Format(sql1, productCatalogRemark.PC, FactoryCode);
            //string query2 = string.Format(sql2,
            //    productCatalogRemark.PC,
            //    productCatalogRemark.MaterialNo,
            //    productCatalogRemark.Remark,
            //    productCatalogRemark.NonMove,
            //    string.IsNullOrEmpty(productCatalogRemark.NonMoveMonth.ToString()) ? null : "'" + productCatalogRemark.NonMoveMonth.ToString() + "'"
            //    , productCatalogRemark.StockWIP,
            //    productCatalogRemark.StockFG,
            //    productCatalogRemark.StockQA,
            //    FactoryCode
            //    );
            string query3 = string.Format(sql3,
                productCatalogRemark.MaterialNo,
                productCatalogRemark.Remark,
                productCatalogRemark.NonMove == null ? "[NonMove] = NULL," : string.Format("[NonMove] = {0},", productCatalogRemark.NonMove),
                string.IsNullOrEmpty(productCatalogRemark.NonMoveMonth.ToString()) ? "[NonMoveMonth] = NULL," : string.Format("[NonMoveMonth] = '{0}',", productCatalogRemark.NonMoveMonth.ToString()),
                productCatalogRemark.StockWIP == null ? "[StockWIP] = NULL," : string.Format("[StockWIP] = {0},", productCatalogRemark.StockWIP),
                productCatalogRemark.StockFG == null ? "[StockFG] = NULL," : string.Format("[StockFG] = {0},", productCatalogRemark.StockFG),
                productCatalogRemark.StockQA == null ? "[StockQA] = NULL" : string.Format("[StockQA] = {0}", productCatalogRemark.StockQA),
                productCatalogRemark.PC,
                FactoryCode
                );
            //string query4 = string.Format(sql4,
            //            productCatalogRemark.PC,
            //            productCatalogRemark.MaterialNo,
            //            productCatalogRemark.Remark,
            //            productCatalogRemark.NonMove,
            //            productCatalogRemark.StockWIP,
            //            productCatalogRemark.StockFG,
            //            productCatalogRemark.StockQA,
            //            FactoryCode 
            //            );

            var query = db.Query<string>(query1).FirstOrDefault();
            if (query == null)
            {
                //if (productCatalogRemark.NonMoveMonth == "")
                //{
                //    db.Execute(query4);
                //}
                //else
                //{
                //    db.Execute(query2);
                //}
                db.Execute(query5);
            }
            else
            {
                db.Execute(query3);
            }
        }

        public ProductCatalogRemark GetRemark(IConfiguration config, string fac, string pc)
        {
            using (IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                // string sql1 = @"SELECT * FROM Remark where PC = LEFT('{0}',11) and FactoryCode = '{1}'";
                string sql1 = @"SELECT * FROM Remark where PC = '{0}' and FactoryCode = '{1}'";
                string query1 = string.Format(sql1, pc, fac);
                return db.Query<ProductCatalogRemark>(query1).FirstOrDefault();
            }
        }


        public HoldMaterial GetHoldMaterialByMaterial(string material)
        {
            HoldMaterial model = new HoldMaterial();
            model = PMTsDbContext.HoldMaterial.Where(x => x.MaterialNo == material).FirstOrDefault();
            return model;
        }

        public void SaveHoldMaterialByMaterial(HoldMaterial model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    HoldMaterial HoldMaterial = new HoldMaterial();
                    HoldMaterial.MaterialNo = model.MaterialNo;
                    HoldMaterial.IsLocked = model.IsLocked;
                    HoldMaterial.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                    HoldMaterial.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                    HoldMaterial.CreatedBy = model.CreatedBy;
                    HoldMaterial.CreatedDate = DateTime.Now;
                    PMTsDbContext.HoldMaterial.Add(HoldMaterial);
                    PMTsDbContext.SaveChanges();

                    var MasterTemp = PMTsDbContext.MasterData.Where(z => z.MaterialNo == model.MaterialNo).ToList();
                    if (MasterTemp.Count > 0)
                    {
                        foreach (var id in MasterTemp)
                        {
                            var Master = PMTsDbContext.MasterData.Where(z => z.Id == id.Id).FirstOrDefault();
                            if (Master.Pc.Length < 2)
                            {
                                Master.Pc = model.IsLocked == true ? ("h" + Master.Pc) : "";
                            }
                            else
                            {
                                Master.Pc = model.IsLocked == true ? (Master.Pc.Substring(0, 1) == "h" ? Master.Pc : "h" + Master.Pc) : (Master.Pc.Substring(0, 1) == "h" ? Master.Pc.Substring(1, Master.Pc.Length - 1) : Master.Pc);
                            }

                            if (Master.Pc.Length > 20)
                            {
                                Master.Pc = Master.Pc.Substring(0, 20);
                            }


                            string desc = string.Empty;
                            if (!string.IsNullOrEmpty(Master.Description))
                            {
                                if (Master.Description.Length > 5)
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp.Insert(5, "_ปป");
                                    }
                                    else
                                    {
                                        //desc = Master.Description.Substring(5, 3) == "_ปป" ? Master.Description.Replace("_ปป", "") : Master.Description; 
                                        desc = Master.Description.Replace("_ปป", "");
                                    }
                                }
                                else
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp + "_ปป";
                                    }
                                    else
                                    {
                                        desc = Master.Description.Replace("_ปป", "");
                                    }

                                }
                            }

                            if (desc.Length > 40)
                            {
                                Master.Description = desc.Substring(0, 40);
                            }
                            else
                            {
                                Master.Description = desc;
                            }



                            // Master.Description = string.IsNullOrEmpty(Master.Description) ? "" : model.isLocked == true ? "h" + Master.Description : Master.Description.Substring(0, 1) == "h" ? Master.Description.Substring(1, Master.Description.Length - 1) : Master.Description;

                            // if (Master.Description.Length > 40)
                            // {
                            //     Master.Description = Master.Description.Substring(0, 40);
                            // }

                            string change = ((model.ChangeProductNo + model.HoldRemark) + Master.Change);
                            Master.Change = change.Length > 100 ? change.Substring(0, 99) : change;

                            Master.UpdatedBy = model.CreatedBy;
                            Master.LastUpdate = DateTime.Now;

                            var comp = PMTsDbContext.CompanyProfile.Where(zz => zz.SaleOrg == id.SaleOrg && zz.Plant == id.Plant).FirstOrDefault();
                            if (comp != null)
                            {
                                Master.TranStatus = false;
                            }

                            ChangeHistory changeHis = new ChangeHistory();
                            changeHis.FactoryCode = Master.FactoryCode;
                            changeHis.MaterialNo = Master.MaterialNo;
                            changeHis.ChangeInfo = Master.Change;
                            changeHis.ChangeHistoryText = Master.Change;
                            changeHis.Status = true;
                            changeHis.CreatedBy = model.CreatedBy;
                            changeHis.CreatedDate = DateTime.Now;
                            changeHis.UpdatedBy = model.CreatedBy;
                            changeHis.UpdatedDate = DateTime.Now;
                            PMTsDbContext.ChangeHistory.Add(changeHis);
                            PMTsDbContext.SaveChanges();


                        }
                    }

                    //if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    //{
                    //    PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                    //    PMTsDbContext.SaveChanges();
                    //}




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

        public void UpdateHoldMaterialByMaterial(HoldMaterial model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var HoldMaterial = PMTsDbContext.HoldMaterial.Where(z => z.MaterialNo == model.MaterialNo).FirstOrDefault();
                    HoldMaterial.MaterialNo = model.MaterialNo;
                    HoldMaterial.IsLocked = model.IsLocked;
                    HoldMaterial.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                    HoldMaterial.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                    HoldMaterial.UpdatedBy = model.UpdatedBy;
                    HoldMaterial.UpdatedDate = DateTime.Now;

                    var MasterTemp = PMTsDbContext.MasterData.Where(z => z.MaterialNo == model.MaterialNo).ToList();
                    if (MasterTemp.Count > 0)
                    {
                        foreach (var id in MasterTemp)
                        {
                            var Master = PMTsDbContext.MasterData.Where(z => z.Id == id.Id).FirstOrDefault();

                            if (Master.Pc.Length < 2)
                            {
                                Master.Pc = model.IsLocked == true ? ("h" + Master.Pc) : "";
                            }
                            else
                            {
                                Master.Pc = model.IsLocked == true ? (Master.Pc.Substring(0, 1) == "h" ? Master.Pc : "h" + Master.Pc) : (Master.Pc.Substring(0, 1) == "h" ? Master.Pc.Substring(1, Master.Pc.Length - 1) : Master.Pc);
                            }


                            string desc = string.Empty;
                            if (!string.IsNullOrEmpty(Master.Description))
                            {
                                if (Master.Description.Length > 5)
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp.Insert(5, "_ปป");
                                    }
                                    else
                                    {
                                        //desc = Master.Description.Substring(5, 3) == "_ปป" ? Master.Description.Replace("_ปป", "") : Master.Description; 
                                        desc = Master.Description.Replace("_ปป", "");
                                    }
                                }
                                else
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp + "_ปป";
                                    }
                                    else
                                    {
                                        desc = Master.Description.Replace("_ปป", "");
                                    }

                                }
                            }

                            if (desc.Length > 40)
                            {
                                Master.Description = desc.Substring(0, 40);
                            }
                            else
                            {
                                Master.Description = desc;
                            }

                            //Master.Description = string.IsNullOrEmpty(Master.Description) ? "" : model.isLocked == true ? "h" + Master.Description : Master.Description.Substring(0, 1) == "h" ? Master.Description.Substring(1, Master.Description.Length - 1) : Master.Description;
                            //if (Master.Description.Length > 40)
                            //{
                            //    Master.Description = Master.Description.Substring(0, 40);
                            //}

                            string change = ((model.ChangeProductNo + model.HoldRemark) + Master.Change);
                            Master.Change = change.Length > 100 ? change.Substring(0, 99) : change;

                            Master.UpdatedBy = model.UpdatedBy;
                            Master.LastUpdate = DateTime.Now;

                            var comp = PMTsDbContext.CompanyProfile.Where(zz => zz.SaleOrg == id.SaleOrg && zz.Plant == id.Plant).FirstOrDefault();
                            if (comp != null)
                            {
                                Master.TranStatus = false;
                            }

                            if (model.IsLocked == true)
                            {
                                ChangeHistory changeHis = new ChangeHistory();
                                changeHis.FactoryCode = Master.FactoryCode;
                                changeHis.MaterialNo = Master.MaterialNo;
                                changeHis.ChangeInfo = Master.Change;
                                changeHis.ChangeHistoryText = Master.Change;
                                changeHis.Status = true;
                                changeHis.CreatedBy = model.UpdatedBy;
                                changeHis.CreatedDate = DateTime.Now;
                                changeHis.UpdatedBy = model.UpdatedBy;
                                changeHis.UpdatedDate = DateTime.Now;
                                PMTsDbContext.ChangeHistory.Add(changeHis);
                                PMTsDbContext.SaveChanges();
                            }

                        }
                    }
                    //if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    //{
                    //    PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                    //    PMTsDbContext.SaveChanges();
                    //}
                    //PMTsDbContext.SaveChanges();
                    //dbContextTransaction.Commit();

                    //if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    //{
                    //    PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                    //    PMTsDbContext.SaveChanges();
                    //}
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

        public List<HoldMaterialHistory> GetHoldMaterialHistory(string material)
        {
            List<HoldMaterialHistory> model = new List<HoldMaterialHistory>();
            model = PMTsDbContext.HoldMaterialHistory.Where(x => x.MaterialNo == material).OrderByDescending(x => x.CreatedDate).ToList();
            return model;
        }
        public void SaveHoldMaterialHistory(HoldMaterialHistory model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    HoldMaterialHistory HoldMaterialHistory = new HoldMaterialHistory();
                    HoldMaterialHistory.MaterialNo = model.MaterialNo;
                    HoldMaterialHistory.IsLocked = model.IsLocked;
                    HoldMaterialHistory.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                    HoldMaterialHistory.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                    HoldMaterialHistory.CreatedBy = model.CreatedBy;
                    HoldMaterialHistory.CreatedDate = DateTime.Now;
                    PMTsDbContext.HoldMaterialHistory.Add(HoldMaterialHistory);
                    PMTsDbContext.SaveChanges();

                    //if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    //{
                    //    PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                    //    PMTsDbContext.SaveChanges();
                    //}

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

        public string GetOrderItemInMoData(string FactoryCode, string Material)
        {
            var Temp = PMTsDbContext.MoData.Where(x => x.FactoryCode == FactoryCode && x.MaterialNo == Material).FirstOrDefault();
            return Temp == null ? "" : string.IsNullOrEmpty(Temp.OrderItem) ? "" : Temp.OrderItem;
        }


        public List<HoldAndUnHoldMaterialResponseModel> GetResponseHoldMaterial(string MaterialNo)
        {
            List<HoldAndUnHoldMaterialResponseModel> model = new List<HoldAndUnHoldMaterialResponseModel>();
            var MasterTemp = PMTsDbContext.MasterData.Where(z => z.MaterialNo == MaterialNo).ToList();
            if (MasterTemp.Count > 0)
            {
                foreach (var item in MasterTemp)
                {
                    HoldAndUnHoldMaterialResponseModel tmp = new HoldAndUnHoldMaterialResponseModel();
                    tmp.MaterialNo = item.MaterialNo;
                    tmp.Pc = item.Pc;
                    tmp.Description = item.Description;
                    model.Add(tmp);
                }
            }
            return model;
        }

        public List<ScalePriceMatProductModel> GetScalePriceMatProduct(IConfiguration config, string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string MaterialNo)
        {
            var scalePriceMatProductModels = new List<ScalePriceMatProductModel>();
            var scalePriceMatProducQuery = new List<ScalePriceMatProductModel>();
            var masterDataQuery = new List<ScalePriceMatProductModel>();
            var factoryCodeString = string.Empty;
            var queryString = string.Empty;
            var pcString = string.Empty;
            var plantsQuery = string.Empty;
            var saleOrgsQuery = string.Empty;
            var matTypeString = !string.IsNullOrEmpty(materialType) ? $" and Material_Type = '{materialType}' " : string.Empty;
            var custCodeString = !string.IsNullOrEmpty(custCode) ? $" and cust_code like '%{custCode}%' " : string.Empty;
            var custIdString = !string.IsNullOrEmpty(custId) ? $" and Cus_ID like '%{custId}%' " : string.Empty;
            var custNameString = !string.IsNullOrEmpty(custName) ? $" and Cust_Name like '%{custName}%' " : string.Empty;
            var pc1String = !string.IsNullOrEmpty(pc1) ? $" pc like '%{pc1}%'" : string.Empty;
            var pc2String = !string.IsNullOrEmpty(pc2) ? $" pc like '%{pc2}%'" : string.Empty;
            var pc3String = !string.IsNullOrEmpty(pc3) ? $" pc like '%{pc3}%'" : string.Empty;
            var matNoString = !string.IsNullOrEmpty(MaterialNo) ? $" and Material_No like '%{MaterialNo}%'" : string.Empty;
            var pcs = new List<string>();
            pcs.AddRange(new List<string> { pc1String, pc2String, pc3String });
            pcString = string.Join(" or ", pcs.Where(s => !string.IsNullOrEmpty(s)));
            queryString = !string.IsNullOrEmpty(pcString) ? matNoString + matTypeString + custCodeString + custIdString + custNameString + $" and ({pcString})" : matNoString + matTypeString + custCodeString + custIdString + custNameString;

            if (string.IsNullOrEmpty(plantPdts) && string.IsNullOrEmpty(salePlants))
            {
                factoryCodeString = $" and FactoryCode = '{factoryCode}'";
            }
            else
            {

                if (plantPdts == "All")
                {
                    plantsQuery = string.Empty;
                }
                else
                {
                    plantsQuery = string.IsNullOrEmpty(plantPdts) ? string.Empty : $" and FactoryCode IN ('{string.Join("','", JsonConvert.DeserializeObject<string[]>(plantPdts))}') ";
                }

                if (salePlants == "All")
                {
                    saleOrgsQuery = string.Empty;
                }
                else
                {
                    saleOrgsQuery = string.IsNullOrEmpty(salePlants) ? string.Empty : $" and Sale_Org IN ('{string.Join("','", JsonConvert.DeserializeObject<string[]>(salePlants))}') ";
                }
            }



            using (IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                string sql = @"
                        SELECT [Id]
		                ,m.[Material_No] as MaterialNo
		                ,m.[Part_No] as PartNo
		                ,m.[PC] as Pc
		                ,m.[Cust_Code] as CustCode
		                ,m.[Cus_ID] as CusId
		                ,m.[Cust_Name] as CustName
		                ,m.[Sale_Text1] as SaleText1
		                ,m.[Flute]
		                ,m.[Board]
		                ,m.[Wid]
		                ,m.[Leg]
		                ,m.[Hig]
                        ,i.[NETPRICE] NetPrice
                        from (select * from MasterData where PDIS_Status <> 'X' " + factoryCodeString + $"{queryString}" + plantsQuery + saleOrgsQuery + @") m 
                    left outer join 
						(select ii.NETPRICE , ii.FactoryCode, ii.MATERIALNO --, ii.SOURCELIST_VALIDFROM, ii.SOURCELIST_VALIDTO
						 from (select FactoryCode, MATERIALNO, count(*) a, MAX(SOURCELIST_VALIDFROM) bb, MAX(SOURCELIST_VALIDTO) cc
							   from InfoRecordSourceList group by FactoryCode, MATERIALNO) i 
							   left outer join InfoRecordSourceList ii on ii.MATERIALNO = i.MATERIALNO and ii.FactoryCode = i.FactoryCode 
							   and ii.SOURCELIST_VALIDFROM = i.bb)  i on i.MATERIALNO = m.Material_No and i.FactoryCode = m.FactoryCode
                        order by pc";

                string message = string.Format(sql, factoryCode);

                masterDataQuery = db.Query<ScalePriceMatProductModel>(message).ToList();
            }

            var materialNos = masterDataQuery.GroupBy(m => m.MaterialNo).Select(g => g.First().MaterialNo).ToList();
            if (materialNos.Count > 0)
            {

                using (IDbConnection db = new SqlConnection(config.GetConnectionString("SAPConnectR")))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //Execute sql query
                    string sql = @"
                        select 
                        [material no] as MaterialNo,
                        rate as Rate,
                        [validity start from] as StartDate,
                        [validity end date] as EndDate,
                        scaleQuantity as ScaleQty
                        from PricingMaster_Scale
                        where [validity start from] <= GETDATE() and [validity end date] >= GETDATE() and [material no] in " + $"('{string.Join("', '", materialNos.Where(m => !string.IsNullOrEmpty(m)))}')" +
                        "order by [material no], scaleQuantity asc";

                    string message = string.Format(sql);

                    scalePriceMatProducQuery = db.Query<ScalePriceMatProductModel>(message).ToList();
                }

                scalePriceMatProductModels = (from m in masterDataQuery
                                              join s in scalePriceMatProducQuery
                                              on m.MaterialNo.Trim() equals s.MaterialNo.Trim() into ms
                                              from s in ms.DefaultIfEmpty()
                                              select new ScalePriceMatProductModel
                                              {
                                                  MaterialNo = m.MaterialNo,
                                                  PartNo = m.PartNo,
                                                  Pc = m.Pc,
                                                  CusId = m.CusId,
                                                  CustCode = m.CustCode,
                                                  CustName = m.CustName,
                                                  SaleText1 = m.SaleText1,
                                                  Board = m.Board,
                                                  Flute = m.Flute,
                                                  Hig = m.Hig,
                                                  Leg = m.Leg,
                                                  Wid = m.Wid,
                                                  Rate = s != null ? s.Rate : null,
                                                  ScaleQty = s != null ? s.ScaleQty : null,
                                                  StartDate = s != null ? s.StartDate : null,
                                                  EndDate = s != null ? s.EndDate : null,
                                                  NetPrice = m.NetPrice
                                              }).ToList();
            }
            return scalePriceMatProductModels;
        }

        public List<BOMMaterialProductModel> GetBOMMaterialProduct(IConfiguration config, string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3)
        {
            var bOMMaterialProductModels = new List<BOMMaterialProductModel>();
            var queryString = string.Empty;
            var pcString = string.Empty;
            var custCodeString = !string.IsNullOrEmpty(custCode) ? $"and cust_code like '%{custCode}%' " : string.Empty;
            var custIdString = !string.IsNullOrEmpty(custId) ? $"and Cus_ID like '%{custId}%' " : string.Empty;
            var custNameString = !string.IsNullOrEmpty(custName) ? $"and Cust_Name like '%{custName}%' " : string.Empty;
            var pc1String = !string.IsNullOrEmpty(pc1) ? $"pc like '%{pc1}%'" : string.Empty;
            var pc2String = !string.IsNullOrEmpty(pc2) ? $"pc like '%{pc2}%'" : string.Empty;
            var pc3String = !string.IsNullOrEmpty(pc3) ? $"pc like '%{pc3}%'" : string.Empty;
            var pcs = new List<string>();
            pcs.AddRange(new List<string> { pc1String, pc2String, pc3String });
            pcString = string.Join(" or ", pcs.Where(s => !string.IsNullOrEmpty(s)));
            queryString = !string.IsNullOrEmpty(pcString) ? custCodeString + custIdString + custNameString + $" and ({pcString})" : custCodeString + custIdString + custNameString;

            using (IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                string sql = @" select a.Follower as MaterialNo, a.PC as Pc, a.Part_No as PartNo, a.Cus_ID as CusId, a.Cust_Code as CustCode, a.Cust_Name as CustName, a.Sale_Text1 as SaleText1, a.Flute, a.Board, 
                            a.Leg, a.Wid, a.Hig, a.Price, a.Amount as Quantity, a.Total as TotalPrice, a.PDIS_Status
                            from (
                            select m.FactoryCode, m.Material_No, m.Material_Type, b.Follower, mm.PC, mm.Part_No, mm.Cus_ID, mm.Cust_Code, mm.Cust_Name, mm.Sale_Text1,
                            mm.Flute, mm.Board, mm.Leg, mm.Wid, isnull(mm.Hig, 0) Hig, b.Previous_Bom, 
                            isnull(i.rate, 0) [Price], b.Amount, isnull(i.rate, 0) * b.Amount [Total], mm.PDIS_Status
                            from (select * from MasterData where ((Material_Type = 14 or Material_Type = 24 or Material_Type = 84) and PDIS_Status <> 'X' and FactoryCode = '{0}' " + queryString + @")) m 
                            left outer join Bom_Struct b on b.Material_No = m.Material_No and b.FactoryCode = m.FactoryCode
                            left outer join MasterData mm on mm.Material_No = b.Follower and mm.FactoryCode = b.FactoryCode
                            left outer join (select * from PricingMaster where GETDATE() between [validity start from] and [validity end date]) i on i.[material no] = mm.Material_No 
                            where b.Follower is not null and mm.PDIS_Status <> 'X'
                            union select m.FactoryCode, m.Material_No, m.Material_Type, m.Material_No, m.pc, '' a, '' b, '' c, '' d, '' e, '' f, '' g, 
                            '' Leg, '' Wid, '' Hig, '' h, '' i,'' j, sum(isnull(i.rate, 0) * b.Amount) k, m.PDIS_Status
                            from (select * from MasterData where ((Material_Type = 14 or Material_Type = 24 or Material_Type = 84) and PDIS_Status <> 'X' and FactoryCode = '{0}'" + queryString + @")) m 
                            left outer join Bom_Struct b on b.Material_No = m.Material_No and b.FactoryCode = m.FactoryCode
                            left outer join MasterData mm on mm.Material_No = b.Follower and mm.FactoryCode = b.FactoryCode
                            left outer join (select * from PricingMaster where GETDATE() between [validity start from] and [validity end date]) i on i.[material no] = mm.Material_No 
                            where b.Follower is not null and mm.PDIS_Status <> 'X'
                            group by m.FactoryCode, m.Material_No, m.Material_Type, m.PC, m.PDIS_Status) a
                            order by a.Material_No, a.Cus_ID, a.Previous_Bom, a.Follower";

                string message = string.Format(sql, factoryCode);

                bOMMaterialProductModels = db.Query<BOMMaterialProductModel>(message).ToList();
            }

            return bOMMaterialProductModels;
        }
        public void UpdateHoldMaterialPresale(HoldMaterial model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var HoldMaterial = PMTsDbContext.HoldMaterial.Where(z => z.MaterialNo == model.MaterialNo).FirstOrDefault();
                    if (HoldMaterial != null)
                    {
                        HoldMaterial.MaterialNo = model.MaterialNo;
                        HoldMaterial.IsLocked = model.IsLocked;
                        HoldMaterial.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                        HoldMaterial.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                        HoldMaterial.UpdatedBy = model.UpdatedBy;
                        HoldMaterial.UpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        HoldMaterial = new HoldMaterial();
                        HoldMaterial.MaterialNo = model.MaterialNo;
                        HoldMaterial.IsLocked = model.IsLocked;
                        HoldMaterial.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                        HoldMaterial.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                        HoldMaterial.CreatedBy = model.CreatedBy;
                        HoldMaterial.CreatedDate = DateTime.Now;
                        PMTsDbContext.HoldMaterial.Add(HoldMaterial);
                        PMTsDbContext.SaveChanges();
                    }

                    HoldMaterialHistory HoldMaterialHistory = new HoldMaterialHistory();
                    HoldMaterialHistory.MaterialNo = model.MaterialNo;
                    HoldMaterialHistory.IsLocked = model.IsLocked;
                    HoldMaterialHistory.HoldRemark = string.IsNullOrEmpty(model.HoldRemark) ? null : model.HoldRemark;
                    HoldMaterialHistory.ChangeProductNo = string.IsNullOrEmpty(model.ChangeProductNo) ? null : model.ChangeProductNo;
                    HoldMaterialHistory.CreatedBy = model.CreatedBy;
                    HoldMaterialHistory.CreatedDate = DateTime.Now;
                    PMTsDbContext.HoldMaterialHistory.Add(HoldMaterialHistory);
                    PMTsDbContext.SaveChanges();

                    var MasterTemp = PMTsDbContext.MasterData.Where(z => z.MaterialNo == model.MaterialNo).ToList();
                    if (MasterTemp.Count > 0)
                    {
                        foreach (var id in MasterTemp)
                        {
                            var Master = PMTsDbContext.MasterData.Where(z => z.Id == id.Id).FirstOrDefault();

                            if (Master.Pc.Length < 2)
                            {
                                Master.Pc = model.IsLocked == true ? ("h" + Master.Pc) : "";
                            }
                            else
                            {
                                Master.Pc = model.IsLocked == true ? (Master.Pc.Substring(0, 1) == "h" ? Master.Pc : "h" + Master.Pc) : (Master.Pc.Substring(0, 1) == "h" ? Master.Pc.Substring(1, Master.Pc.Length - 1) : Master.Pc);
                            }


                            string desc = string.Empty;
                            if (!string.IsNullOrEmpty(Master.Description))
                            {
                                if (Master.Description.Length > 5)
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp.Insert(5, "_ปป");
                                    }
                                    else
                                    {
                                        //desc = Master.Description.Substring(5, 3) == "_ปป" ? Master.Description.Replace("_ปป", "") : Master.Description; 
                                        desc = Master.Description.Replace("_ปป", "");
                                    }
                                }
                                else
                                {
                                    if (model.IsLocked == true)
                                    {
                                        string pp = Master.Description.Replace("_ปป", "");
                                        desc = pp + "_ปป";
                                    }
                                    else
                                    {
                                        desc = Master.Description.Replace("_ปป", "");
                                    }

                                }
                            }

                            if (desc.Length > 40)
                            {
                                Master.Description = desc.Substring(0, 40);
                            }
                            else
                            {
                                Master.Description = desc;
                            }

                            //Master.Description = string.IsNullOrEmpty(Master.Description) ? "" : model.isLocked == true ? "h" + Master.Description : Master.Description.Substring(0, 1) == "h" ? Master.Description.Substring(1, Master.Description.Length - 1) : Master.Description;
                            //if (Master.Description.Length > 40)
                            //{
                            //    Master.Description = Master.Description.Substring(0, 40);
                            //}

                            string change = ((model.ChangeProductNo + model.HoldRemark) + Master.Change);
                            Master.Change = change.Length > 100 ? change.Substring(0, 99) : change;

                            Master.UpdatedBy = model.UpdatedBy;
                            Master.LastUpdate = DateTime.Now;

                            var comp = PMTsDbContext.CompanyProfile.Where(zz => zz.SaleOrg == id.SaleOrg && zz.Plant == id.Plant).FirstOrDefault();
                            if (comp != null)
                            {
                                Master.TranStatus = false;
                            }

                            if (model.IsLocked == true)
                            {
                                ChangeHistory changeHis = new ChangeHistory();
                                changeHis.FactoryCode = Master.FactoryCode;
                                changeHis.MaterialNo = Master.MaterialNo;
                                changeHis.ChangeInfo = Master.Change;
                                changeHis.ChangeHistoryText = Master.Change;
                                changeHis.Status = true;
                                changeHis.CreatedBy = model.UpdatedBy;
                                changeHis.CreatedDate = DateTime.Now;
                                changeHis.UpdatedBy = model.UpdatedBy;
                                changeHis.UpdatedDate = DateTime.Now;
                                PMTsDbContext.ChangeHistory.Add(changeHis);
                                PMTsDbContext.SaveChanges();
                            }

                        }
                    }
                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}

