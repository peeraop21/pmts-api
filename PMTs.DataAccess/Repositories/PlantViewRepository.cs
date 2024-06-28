using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PlantViewRepository : Repository<PlantView>, IPlantViewRepository
    {
        public PlantViewRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PlantView GetPlantViewByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.PlantView.Where(p => p.FactoryCode == factoryCode && p.MaterialNo == materialNo && p.PdisStatus != "X").FirstOrDefault();
        }
        //  Tassanai Update 20/11/2019
        //public IEnumerable<PlantView> GetPlantViewsByMaterialNo(string factoryCode, string materialNo)
        //{
        //    return PMTsDbContext.PlantView.Where(p => p.FactoryCode == factoryCode && p.MaterialNo == materialNo && p.PdisStatus != "X").ToList();
        //}

        public IEnumerable<PlantView> GetPlantViewsByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.PlantView.Where(p => p.MaterialNo == materialNo && p.PdisStatus != "X").ToList();
        }

        public PlantView GetPlantViewByPlant(string materialNo, string plant)
        {
            return PMTsDbContext.PlantView.Where(x => x.MaterialNo == materialNo && x.Plant == plant && x.PdisStatus != "X").FirstOrDefault();
        }

        public PlantView GetPlantViewByMaterialNoAndPlant(string materialNo, string plant)
        {
            return PMTsDbContext.PlantView.Where(p => p.Plant == plant && p.MaterialNo == materialNo && p.PdisStatus != "X").FirstOrDefault();
        }

        public void UpdatePlantViewShipBlk(string FactoryCode, string MaterialNo, string Status)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var some = PMTsDbContext.PlantView.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo).ToList();
                    some.ForEach(a => a.ShipBlk = Status);
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

        //public IEnumerable<PlantView> GetPlanViewByhandshake(SqlConnection conn, string factoryCode, string materialNo)
        //{
        //    DataTable dt = new DataTable();
        //    string sqlQuery = "select Plant_View.FactoryCode,Plant_View.PDIS_Status,Plant_View.material_no, Plant_View.plant,purch_code " +
        //        "                            , isnull(Ship_Blk, '') as block, " +
        //        "   cast(convert(decimal(18, 2), isnull(Std_Total_Cost, '')) as nvarchar) as Stdcost, " +
        //        "     cast(convert(decimal(18, 2), isnull(Std_Moving_Cost, '')) as nvarchar) as StdMoving" +
        //        "	 ,MasterData.PDIS_Status as MasterPDIS_Status" +
        //        "     from Plant_View inner join masterdata on Plant_View.Material_No = masterdata.Material_No  where Plant_View.FactoryCode = '" + factoryCode + "' and Plant_View.Material_No in (" + materialNo + ")";

        //    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //    }
        //    //  List<SalesView> sales = dt.Rows.();
        //    var plantview = (from DataRow row in dt.Rows
        //                     select new PlantView
        //                     {
        //                         FactoryCode = row["FactoryCode"].ToString(),
        //                         MaterialNo = row["Material_No"].ToString(),
        //                         Plant = row["plant"].ToString(),
        //                         PurchCode = row["purch_code"].ToString(),
        //                         ShipBlk = row["block"].ToString(),
        //                         StdTotalCost = Convert.ToDouble(row["Stdcost"]),
        //                         StdMovingCost = Convert.ToDouble(row["StdMoving"]),
        //                         PdisStatus = row["PDIS_Status"].ToString() != "X" ? row["MasterPDIS_Status"].ToString() : row["PDIS_Status"].ToString()
        //                     }).ToList();

        //    return plantview;
        //}

        public IEnumerable<PlantView> GetPlanViewByhandshake(SqlConnection conn, string factoryCode, string materialNo)
        {
            DataTable dt = new DataTable();
            string sqlQuery = "select distinct Plant_View.Id,Plant_View.FactoryCode,Plant_View.PDIS_Status,Plant_View.material_no, Plant_View.plant,purch_code " +
                "  , isnull(Ship_Blk, '') as block, " +
                "  cast(convert(decimal(18, 2), isnull(Std_Total_Cost, '')) as nvarchar) as Stdcost, " +
                "  cast(convert(decimal(18, 2), isnull(Std_Moving_Cost, '')) as nvarchar) as StdMoving" +
                " ,MasterData.PDIS_Status as MasterPDIS_Status" +
                " from Plant_View inner join masterdata on Plant_View.Material_No = masterdata.Material_No   and Plant_View.FactoryCode = masterdata.FactoryCode " +
                " where  Plant_View.Material_No in (" + materialNo + ") and Plant_View.PDIS_Status <> 'N'  " +
                " order by Plant_View.Material_No,Plant_View.Id asc";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            //  List<SalesView> sales = dt.Rows.();
            var plantview = (from DataRow row in dt.Rows
                             select new PlantView
                             {
                                 FactoryCode = row["FactoryCode"].ToString(),
                                 MaterialNo = row["Material_No"].ToString(),
                                 Plant = row["plant"].ToString(),
                                 PurchCode = row["purch_code"].ToString(),
                                 ShipBlk = row["block"].ToString(),
                                 StdTotalCost = Convert.ToDouble(row["Stdcost"]),
                                 StdMovingCost = Convert.ToDouble(row["StdMoving"]),
                                 PdisStatus = row["PDIS_Status"].ToString() != "X" ? row["MasterPDIS_Status"].ToString() : row["PDIS_Status"].ToString()
                             }).ToList();

            return plantview;
        }

        public IEnumerable<PlantView> GetReusePlantViewsByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var plantViews = new List<PlantView>();
            foreach (var materialNo in materialNos)
            {
                plantViews.AddRange(PMTsDbContext.PlantView.Where(p => p.FactoryCode == factoryCode && p.PdisStatus.ToUpper() == "X" && p.MaterialNo == materialNo));
            }

            return plantViews;
        }
    }
}
