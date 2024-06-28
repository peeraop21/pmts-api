using Dapper;
using Microsoft.EntityFrameworkCore;
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
    public class RoutingRepository : Repository<Routing>, IRoutingRepository
    {
        public RoutingRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<Routing> GetRoutingByMaterialNo(string factoryCode, string materialNo)
        {
            //return PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.PdisStatus != "X");
            return PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode);
        }
        public IEnumerable<Routing> GetRoutingsByMaterialNoContain(string factoryCode, string materialNo)
        {
            return PMTsDbContext.Routing.Where(w => w.MaterialNo.Contains(materialNo) && w.FactoryCode == factoryCode).OrderBy(o => o.SeqNo);
        }
        public IEnumerable<Routing> GetRoutingByMaterialNoFactorycodeAndPlant(string factoryCode, string plant, string materialNo)
        {
            //return PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.Plant == plant && w.PdisStatus != "X");
            return PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.Plant == plant);

        }

        public int GetNumberOfRoutingByShipBlk(string factoryCode, string materialNo, bool semiBlk)
        {
            return PMTsDbContext.Routing.Where(x => (x.MaterialNo == materialNo && x.FactoryCode == factoryCode && x.ShipBlk == true && x.PdisStatus != "X") || (x.MaterialNo == materialNo && x.FactoryCode == factoryCode && x.SemiBlk == true && x.PdisStatus != "X")).Count();
        }
        //public IEnumerable<Routing> GetRoutingByhandshake(string factoryCode, string materialNo)
        //{

        //    // return PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode).ToList();



        //}

        //Tasssani Update

        public IEnumerable<Routing> GetRoutingByhandshake(IConfiguration config, string factoryCode, string materialNo)
        {


            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
               SELECT  Id
              ,FactoryCode
              ,Seq_No as SeqNo
              ,Plant
              ,Material_No as  MaterialNo
              ,Mat_Code as MatCode
              ,Plan_Code as PlanCode
              ,Machine
              ,Alternative1
              ,Alternative2
              ,Std_Process as StdProcess
              ,Speed
              ,Colour_Count as ColourCount
              ,MC_Move as MCMove
              ,HandHold
              ,Plate_No as PlateNo
              ,Myla_No as MylaNo
              ,Paper_Width as PaperWidth
              ,Cut_No as CutNo
              ,Trim
              ,PercenTrim
              ,Waste_Leg as WasteLeg
              ,Waste_Wid as WasteWid
              ,Sheet_in_Leg as SheetInLeg
              ,Sheet_in_Wid as SheetInWid
              ,Sheet_out_Leg as SheetOutLeg
              ,Sheet_out_Wid as SheetOutWid
              ,Weight_in as WeightIn
              ,Weight_out as WeightOut
              ,No_Open_in as NoOpenIn
              ,No_Open_out as NoOpenOut
              ,Color1
              ,Shade1
              ,Color2
              ,Shade2
              ,Color3
              ,Shade3
              ,Color4
              ,Shade4
              ,Color5
              ,Shade5
              ,Color6
              ,Shade6
              ,Color7
              ,Shade7
              ,Color_Area1 as ColorArea1
              ,Color_Area2 as ColorArea2
              ,Color_Area3 as ColorArea3
              ,Color_Area4 as ColorArea4
              ,Color_Area5 as ColorArea5
              ,Color_Area6 as ColorArea6
              ,Color_Area7 as ColorArea7
              ,Platen
              ,Rotary
              ,TearTape
              ,None_Blk as NoneBlk
              ,Stan_Blk as StanBlk
              ,Semi_Blk as SemiBlk
              ,Ship_Blk as ShipBlk
              ,Block_No as BlockNo
              ,Join_Mat_no as JoinMatNo
              ,Separat_Mat_no as SeparatMatNo
              ,Remark_Inprocess as RemarkInprocess
              ,Hardship
              ,PDIS_Status as PDISStatus
              ,Tran_Status as TranStatus
              ,SAP_Status as SAPStatus
              ,Alternative3
              ,Alternative4
              ,Alternative5
              ,Alternative6
              ,Alternative7
              ,Alternative8
              ,Rotate_In as RotateIn
              ,Rotate_Out as RotateOut
              ,Stack_Height as StackHeight
              ,Setup_tm as SetupTm
              ,Setup_waste as SetupWaste
              ,Prepare_tm as PrepareTm
              ,Post_tm as PostTm
              ,Run_waste as RunWaste
              ,Human
              ,Color_count as ColorCount
              ,UnUpgrad_Board as UnUpgradBoard
              ,Score_type as ScoreType
              ,Score_Gap as ScoreGap
              ,Coating
              ,BlockNo2
              ,BlockNoPlant2
              ,BlockNo3
              ,BlockNoPlant3
              ,BlockNo4
              ,BlockNoPlant4
              ,BlockNo5
              ,BlockNoPlant5
              ,PlateNo2
              ,PlateNoPlant2
              ,MylaNo2
              ,MylaNoPlant2
              ,PlateNo3
              ,PlateNoPlant3
              ,MylaNo3
              ,MylaNoPlant3
              ,PlateNo4
              ,PlateNoPlant4
              ,MylaNo4
              ,MylaNoPlant4
              ,PlateNo5
              ,PlateNoPlant5
              ,MylaNo5
              ,MylaNoPlant5
              ,TearTapeQty
              ,TearTapeDistance
              ,MylaSize
              ,RepeatLength
              ,CustBarcodeNo
              ,ControllerCode
              ,PlanProgramCode
              FROM Routing where  {0} Material_No in ({1})
                        ";
            string data = materialNo;//"'Z031103580','Z031103580'";
            if (string.IsNullOrEmpty(data))
            {
                data = "''";
            }
            string message = string.Format(sql, string.IsNullOrEmpty(factoryCode) ? "" : string.Format(" FactoryCode in ('{0}') and ", factoryCode), data);
            return db.Query<Routing>(message).ToList();

        }




        public void UpdatePdisStatus(string FactoryCode, string MaterialNo, string Status)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var someRouting = PMTsDbContext.Routing.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    someRouting.ForEach(a => a.PdisStatus = Status);
                    var someMasterData = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    someMasterData.ForEach(a => a.PdisStatus = Status);
                    var somePlantview = PMTsDbContext.PlantView.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    somePlantview.ForEach(a => a.PdisStatus = Status);
                    var someSaleView = PMTsDbContext.SalesView.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    someSaleView.ForEach(a => a.PdisStatus = Status);
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

        public List<Routing> GetDapperRoutingByMat(IConfiguration config, string fac, string condition)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
              SELECT  Id
              ,FactoryCode
              ,Seq_No as SeqNo
              ,Plant
              ,Material_No as  MaterialNo
              ,Mat_Code as MatCode
              ,Plan_Code as PlanCode
              ,Machine
              ,Alternative1
              ,Alternative2
              ,Std_Process as StdProcess
              ,Speed
              ,Colour_Count as ColourCount
              ,MC_Move as MCMove
              ,HandHold
              ,Plate_No as PlateNo
              ,Myla_No as MylaNo
              ,Paper_Width as PaperWidth
              ,Cut_No as CutNo
              ,Trim
              ,PercenTrim
              ,Waste_Leg as WasteLeg
              ,Waste_Wid as WasteWid
              ,Sheet_in_Leg as SheetInLeg
              ,Sheet_in_Wid as SheetInWid
              ,Sheet_out_Leg as SheetOutLeg
              ,Sheet_out_Wid as SheetOutWid
              ,Weight_in as WeightIn
              ,Weight_out as WeightOut
              ,No_Open_in as NoOpenIn
              ,No_Open_out as NoOpenOut
              ,Color1
              ,Shade1
              ,Color2
              ,Shade2
              ,Color3
              ,Shade3
              ,Color4
              ,Shade4
              ,Color5
              ,Shade5
              ,Color6
              ,Shade6
              ,Color7
              ,Shade7
              ,Color_Area1 as ColorArea1
              ,Color_Area2 as ColorArea2
              ,Color_Area3 as ColorArea3
              ,Color_Area4 as ColorArea4
              ,Color_Area5 as ColorArea5
              ,Color_Area6 as ColorArea6
              ,Color_Area7 as ColorArea7
              ,Platen
              ,Rotary
              ,TearTape
              ,None_Blk as NoneBlk
              ,Stan_Blk as StanBlk
              ,Semi_Blk as SemiBlk
              ,Ship_Blk as ShipBlk
              ,Block_No as BlockNo
              ,Join_Mat_no as JoinMatNo
              ,Separat_Mat_no as SeparatMatNo
              ,Remark_Inprocess as RemarkInprocess
              ,Hardship
              ,PDIS_Status as PDISStatus
              ,Tran_Status as TranStatus
              ,SAP_Status as SAPStatus
              ,Alternative3
              ,Alternative4
              ,Alternative5
              ,Alternative6
              ,Alternative7
              ,Alternative8
              ,Rotate_In as RotateIn
              ,Rotate_Out as RotateOut
              ,Stack_Height as StackHeight
              ,Setup_tm as SetupTm
              ,Setup_waste as SetupWaste
              ,Prepare_tm as PrepareTm
              ,Post_tm as PostTm
              ,Run_waste as RunWaste
              ,Human
              ,Color_count as ColorCount
              ,UnUpgrad_Board as UnUpgradBoard
              ,Score_type as ScoreType
              ,Score_Gap as ScoreGap
              ,Coating
              ,BlockNo2
              ,BlockNoPlant2
              ,BlockNo3
              ,BlockNoPlant3
              ,BlockNo4
              ,BlockNoPlant4
              ,BlockNo5
              ,BlockNoPlant5
              ,PlateNo2
              ,PlateNoPlant2
              ,MylaNo2
              ,MylaNoPlant2
              ,PlateNo3
              ,PlateNoPlant3
              ,MylaNo3
              ,MylaNoPlant3
              ,PlateNo4
              ,PlateNoPlant4
              ,MylaNo4
              ,MylaNoPlant4
              ,PlateNo5
              ,PlateNoPlant5
              ,MylaNo5
              ,MylaNoPlant5
              ,TearTapeQty
              ,TearTapeDistance
              ,MylaSize
              ,RepeatLength
              ,CustBarcodeNo
              ,ControllerCode
              ,PlanProgramCode
              FROM Routing where  {0} Material_No in ({1})
                        ";
            string data = condition;//"'Z031103580','Z031103580'";
            if (string.IsNullOrEmpty(data))
            {
                data = "''";
            }
            string message = string.Format(sql, string.IsNullOrEmpty(fac) ? "" : string.Format(" FactoryCode in ({0}) and ", fac), data);
            return db.Query<Routing>(message).ToList();

        }

        public List<Routing> GetRoutingsByMaterialNos(string factoryCode, List<string> materialNOs)
        {
            var routings = new List<Routing>();

            //foreach (var materialNO in materialNOs)
            if (materialNOs != null && materialNOs.Count > 0)
            {
                routings.AddRange(PMTsDbContext.Routing.Where(m => m.FactoryCode == factoryCode && materialNOs.Contains(m.MaterialNo)).AsNoTracking().ToList());
            }

            return routings;
        }

        public void UpdateRoutings(List<Routing> routings)
        {
            PMTsDbContext.Routing.UpdateRange(routings);
            PMTsDbContext.SaveChanges();
        }

        public void UpdatePdisStatusEmployment(string FactoryCode, string MaterialNo, string SaleOrg, string username, string Status)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var findCompanyProfile = PMTsDbContext.CompanyProfile.Where(x => x.SaleOrg == SaleOrg).FirstOrDefault();
                    ///if (findCompanyProfile.Plant != FactoryCode)
                    //{
                    var someRouting = PMTsDbContext.Routing.Where(s => s.FactoryCode == findCompanyProfile.Plant && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    someRouting.ForEach(a => a.PdisStatus = "C");
                    someRouting.ForEach(a => a.TranStatus = false);
                    someRouting.ForEach(a => a.UpdatedBy = username);
                    someRouting.ForEach(a => a.UpdatedDate = DateTime.Now);

                    var someMasterData = PMTsDbContext.MasterData.Where(s => s.FactoryCode == findCompanyProfile.Plant && s.MaterialNo == MaterialNo && s.SaleOrg == SaleOrg && s.PdisStatus != "X").ToList();
                    someMasterData.ForEach(a => a.TranStatus = false);
                    if (someMasterData.Count >= 1)
                    {
                        someMasterData.ForEach(a => a.TranStatus = false);
                        someMasterData.ForEach(a => a.PdisStatus = (a.SapStatus ? "M" : a.PdisStatus));
                    }
                    else
                    {
                        someMasterData.ForEach(a => a.TranStatus = true);
                    }

                    someRouting.ForEach(a => a.UpdatedBy = username);
                    someRouting.ForEach(a => a.UpdatedDate = DateTime.Now);
                    //  }



                    //var findmaster = PMTsDbContext.MasterData()



                    //var someRouting = PMTsDbContext.Routing.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    //someRouting.ForEach(a => a.PdisStatus = Status);
                    //var someMasterData = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    //someMasterData.ForEach(a => a.PdisStatus = Status);
                    //var somePlantview = PMTsDbContext.PlantView.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    //somePlantview.ForEach(a => a.PdisStatus = Status);
                    //var someSaleView = PMTsDbContext.SalesView.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();
                    //someSaleView.ForEach(a => a.PdisStatus = Status);
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

        public List<ReCalculateTrimModel> UpdateReCalculateTrim(IConfiguration configuration, string factoryCode, List<ReCalculateTrimModel> reCalculateTrimModels)
        {
            var result = new List<ReCalculateTrimModel>();
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            foreach (var reCalculateTrimModel in reCalculateTrimModels)
            {
                var existRouting = PMTsDbContext.Routing.Where(r => r.MaterialNo == reCalculateTrimModel.MaterialNo
                && r.FactoryCode == factoryCode && r.Machine == reCalculateTrimModel.Machine).ToList();
                existRouting.ForEach(r => r.CutNo = reCalculateTrimModel.CutNo);
                existRouting.ForEach(r => r.PaperWidth = reCalculateTrimModel.PaperWidth);
                existRouting.ForEach(r => r.Trim = reCalculateTrimModel.Trim);
                existRouting.ForEach(r => r.PercenTrim = reCalculateTrimModel.PercenTrim);
                existRouting.ForEach(r => r.UpdatedBy = reCalculateTrimModel.UpdatedBy);
                existRouting.ForEach(r => r.UpdatedDate = reCalculateTrimModel.UpdatedDate);
                using (IDbTransaction transactionScope = db.BeginTransaction(IsolationLevel.Serializable))
                {
                    string updateQuery = @"
                            UPDATE [dbo].[Routing]
                            SET [Paper_Width] = @PaperWidth
                              ,[Cut_No] = @CutNo
                              ,[Trim] = @Trim
                              ,[PercenTrim] = @PercenTrim
                              ,[UpdatedDate] = @UpdatedDate
                              ,[UpdatedBy] = @UpdatedBy
                            WHERE Id = @Id";
                    try
                    {
                        if (existRouting != null)
                        {
                            reCalculateTrimModel.UpdateStatus = true;
                            db.Execute(updateQuery, existRouting, transactionScope);
                            transactionScope.Commit();
                        }
                        else
                        {
                            reCalculateTrimModel.ErrorMessase = "routing doesn't exist with materialNo and Machine";
                            reCalculateTrimModel.UpdateStatus = false;
                        }
                        result.Add(reCalculateTrimModel);
                    }
                    catch
                    {
                        transactionScope.Rollback();
                        reCalculateTrimModel.UpdateStatus = false;
                        reCalculateTrimModel.ErrorMessase = "update ReCalculateTrim failed.";
                        result.Add(reCalculateTrimModel);
                        continue;
                    }
                }
            }

            return result;
        }
        public IEnumerable<Routing> GetRoutingListByDateTime(string factoryCode, string DateFrom, string DateTo)
        {
            DateTime from;
            DateTime to;

            DateTime.TryParseExact(DateFrom, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out from);
            DateTime.TryParseExact(DateTo, "yyyy-MM-dd HHmmss", null, System.Globalization.DateTimeStyles.None, out to);

            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                return PMTsDbContext.Routing.Where(m => m.FactoryCode == factoryCode && m.UpdatedDate >= from && m.UpdatedDate <= to).AsNoTracking().ToList();
            }
            else
            {
                return new List<Routing>();
            }

        }
    }
}
