using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class SalesViewRepository : Repository<SalesView>, ISaleViewRepository
    {
        public SalesViewRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public SalesView GetSaleViewByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.SalesView.FirstOrDefault(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.Channel.Equals(10) && w.PdisStatus != "X");
        }

        public SalesView GetSaleViewBySaleOrg(string factoryCode, string materialNo, string saleOrg)
        {
            return PMTsDbContext.SalesView.FirstOrDefault(w => w.MaterialNo.Equals(materialNo) && w.SaleOrg == saleOrg && w.PdisStatus != "X");
        }
        public SalesView GetSaleViewBySaleOrgChannel(string factoryCode, string materialNo, string saleOrg, byte channel)
        {
            return PMTsDbContext.SalesView.FirstOrDefault(w => w.MaterialNo.Equals(materialNo) && w.SaleOrg == saleOrg && w.Channel == channel && w.PdisStatus != "X");
        }


        public IEnumerable<SalesView> GetSaleViewsByMaterialNo(string factoryCode, string materialNo)
        {
            //return PMTsDbContext.SalesView.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.PdisStatus != "X").ToList();
            return PMTsDbContext.SalesView.Where(w => w.MaterialNo.Equals(materialNo) && w.PdisStatus != "X").ToList();
        }

        public IEnumerable<SalesView> GetSaleViewsByMaterialNoAndFactoryCode(string factoryCode, string materialNo)
        {
            //return PMTsDbContext.SalesView.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.PdisStatus != "X").ToList();
            return PMTsDbContext.SalesView.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode.Equals(factoryCode) && w.PdisStatus != "X").ToList();
        }

        //public IEnumerable<SalesView> GetSaleViewByhandshake(SqlConnection conn, string factoryCode, string materialNo)
        //{
        //    DataTable dt = new DataTable();
        //    string sqlQuery = "select Sales_View.id,Sales_View.FactoryCode, Sales_View.Material_No,Sales_View.Channel, Sales_View.Sale_Org,Sales_View.Order_Type," +
        //        " Sales_View.Cust_Code ,Sales_View.Dev_Plant,Sales_View.Min_Qty,Sales_View.PDIS_Status,MasterData.PDIS_Status as MasterPDIS_Status" +
        //        " from Sales_View inner join masterdata on" +
        //        " Sales_View.Material_No = masterdata.Material_No  where Sales_View.FactoryCode = '" + factoryCode + "' and Sales_View.Material_No in (" + materialNo + ")";

        //    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //    }
        //    //  List<SalesView> sales = dt.Rows.();
        //    var saleview = (from DataRow row in dt.Rows
        //                    select new SalesView
        //                    {
        //                        FactoryCode = row["FactoryCode"].ToString(),
        //                        MaterialNo = row["Material_No"].ToString(),
        //                        SaleOrg = row["Sale_Org"].ToString(),
        //                        Channel = Convert.ToInt32(row["Channel"]),
        //                        DevPlant = row["Dev_Plant"].ToString(),
        //                        MinQty = row["Min_Qty"] == null ? 0 : Convert.ToInt32(row["Min_Qty"]),
        //                        OrderType = row["Order_Type"].ToString(),
        //                        PdisStatus = row["PDIS_Status"].ToString() != "X" ? row["MasterPDIS_Status"].ToString() : row["PDIS_Status"].ToString()
        //                    }).ToList();

        //    return saleview;
        //}
        //Tassanai Update interface handshake 17022020 
        public IEnumerable<SalesView> GetSaleViewByhandshake(SqlConnection conn, string factoryCode, string materialNo)
        {
            DataTable dt = new DataTable();
            //string sqlQuery = "select distinct Sales_View.id,Sales_View.FactoryCode, Sales_View.Material_No,Sales_View.Channel, Sales_View.Sale_Org,Sales_View.Order_Type," +
            //    " Sales_View.Cust_Code ,Sales_View.Dev_Plant,Sales_View.Min_Qty,Sales_View.PDIS_Status,MasterData.PDIS_Status as MasterPDIS_Status" +
            //    " from Sales_View inner join masterdata on" +
            //    " Sales_View.Material_No = masterdata.Material_No and Sales_View.FactoryCode = masterdata.FactoryCode  where  Sales_View.Material_No in (" + materialNo + ")";



            //string sqlQuery = "select distinct Sales_View.id,Sales_View.FactoryCode, Sales_View.Material_No,Sales_View.Channel, Sales_View.Sale_Org,Sales_View.Order_Type," +
            //" Sales_View.Cust_Code ,Sales_View.Dev_Plant,isnull(Sales_View.Min_Qty,0) Min_Qty,Sales_View.PDIS_Status,MasterData.PDIS_Status as MasterPDIS_Status" +
            //" from Sales_View inner join masterdata on" +
            //" Sales_View.Material_No = masterdata.Material_No and Sales_View.FactoryCode = masterdata.FactoryCode  where  Sales_View.Material_No in (" + materialNo + ") " +
            //" Order by  Sales_View.Material_No asc,Sales_View.id asc";

            string sqlQuery = "select distinct Sales_View.id,Sales_View.FactoryCode, Sales_View.Material_No,Sales_View.Channel, Sales_View.Sale_Org,Sales_View.Order_Type," +
           " Sales_View.Cust_Code ,Sales_View.Dev_Plant,isnull(Sales_View.Min_Qty,0) Min_Qty,Sales_View.PDIS_Status,MasterData.PDIS_Status as MasterPDIS_Status" +
           " from Sales_View inner join masterdata on" +
           " Sales_View.Material_No = masterdata.Material_No and Sales_View.FactoryCode = masterdata.FactoryCode  where  Sales_View.Material_No in (" + materialNo + ") " +
           " and Sales_View.PDIS_Status <> 'N' " +
           " Order by  Sales_View.Material_No asc,Sales_View.id asc";


            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            //  List<SalesView> sales = dt.Rows.();
            var saleview = (from DataRow row in dt.Rows
                            select new SalesView
                            {
                                FactoryCode = row["FactoryCode"].ToString(),
                                MaterialNo = row["Material_No"].ToString(),
                                SaleOrg = row["Sale_Org"].ToString(),
                                Channel = Convert.ToInt32(row["Channel"]),
                                DevPlant = row["Dev_Plant"].ToString(),
                                MinQty = row["Min_Qty"] == null ? 0 : Convert.ToInt32(row["Min_Qty"]),
                                OrderType = row["Order_Type"].ToString(),
                                PdisStatus = row["PDIS_Status"].ToString() != "X" ? row["MasterPDIS_Status"].ToString() : row["PDIS_Status"].ToString()
                            }).ToList();

            return saleview;
        }

        public IEnumerable<SalesView> GetReuseSaleViewsByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var saleViews = new List<SalesView>();
            foreach (var materialNo in materialNos)
            {
                saleViews.AddRange(PMTsDbContext.SalesView.Where(p => p.FactoryCode == factoryCode && p.PdisStatus.ToUpper() == "X" && p.MaterialNo == materialNo));
            }

            return saleViews;
        }
    }
}
