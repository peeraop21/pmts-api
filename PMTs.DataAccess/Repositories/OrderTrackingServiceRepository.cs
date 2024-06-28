using Dapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class OrderTrackingServiceRepository : Repository<MasterData>, IOrderTrackingServiceRepository
    {
        public OrderTrackingServiceRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }



        public List<AllOrderTracking> GetAllOrderByDate(IConfiguration config, string UpdateDateFrom, string UpdateDateTo)
        {
            OrderTrackingServiceModel orderTrackingServiceModel = new OrderTrackingServiceModel();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                              SELECT
                              FactoryCode
                              ,OrderItem                           
                            FROM MO_DATA where  (UpdatedDate >= '{0}' AND   UpdatedDate <= '{1}')  and isCreateManual = 0
                        ";

            string message = string.Format(sql, UpdateDateFrom, UpdateDateTo);
            return db.Query<AllOrderTracking>(message).ToList();

        }

        public OrderTrackingServiceModel GetMoByListOrderItems(IConfiguration config, string ListOrder)
        {
            OrderTrackingServiceModel orderTrackingServiceModel = new OrderTrackingServiceModel();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                                SELECT Id
                              ,FactoryCode
                              ,MO_Status as MoStatus
                              ,OrderItem
                              ,Material_No as  MaterialNo
                              ,Name
                              ,Order_Quant as OrderQuant
                              ,Tolerance_Over as ToleranceOver
                              ,Tolerance_Under as ToleranceUnder
                              ,Due_Date as DueDate
                              ,Target_Quant as TargetQuant
                              ,Item_Note as ItemNote
                              ,District 
                              ,PO_No as PoNo
                              ,DateTimeStamp
                              ,Printed
                              ,Batch
                              ,Due_Text as DueText
                              ,Sold_to as SoldTo
                              ,Ship_to as ShipTo
                              ,PlanStatus
                              ,StockQty
                              ,IsCreateManual
                              ,SentKIWI
                              ,UpdatedDate
                              ,UpdatedBy
                              ,MORNo
                            FROM MO_DATA where ({0})
                        ";

            string message = string.Format(sql, ListOrder);
            orderTrackingServiceModel.moDatas = db.Query<MoData>(message).ToList();


            List<MoSpec> MoSpecTemp = new List<MoSpec>();
            List<MoRouting> MoRoutingTemp = new List<MoRouting>();
            string sql1 = @"
                        SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Material_No as MaterialNo
                              ,Part_No as PartNo
                              ,PC
                              ,Hierarchy
                              ,Sale_Org as SaleOrg
                              ,Plant
                              ,Cust_Code as CustCode
                              ,Cus_ID as CusId
                              ,Cust_Name as CustName
                              ,Description
                              ,Sale_Text1 as SaleText1
                              ,Sale_Text2 as SaleText2
                              ,Sale_Text3 as SaleText3
                              ,Sale_Text4 as SaleText4
                              ,Change
                              ,Language
                              ,Ind_Grp as Ind_Grp
                              ,Ind_Des as Ind_Des
                              ,Material_Type as MaterialType
                              ,Print_Method as PrintMethod
                              ,TwoPiece
                              ,Flute
                              ,Code
                              ,Board
                              ,GL
                              ,GLWeigth
                              ,BM
                              ,BMWeigth
                              ,BL
                              ,BLWeigth
                              ,CM
                              ,CMWeigth
                              ,CL
                              ,CLWeigth
                              ,DM
                              ,DMWeigth
                              ,DL
                              ,DLWeigth
                              ,Wid
                              ,Leg
                              ,Hig
                              ,Box_Type as BoxType
                              ,RSC_Style as RscStyle
                              ,Pro_Type as ProType
                              ,JoinType
                              ,Status_Flag as StatusFlag
                              ,Priority_Flag as PriorityFlag
                              ,Wire
                              ,Outer_Join as OuterJoin
                              ,CutSheetLeng
                              ,CutSheetWid
                              ,Sheet_Area as SheetArea
                              ,Box_Area as BoxArea
                              ,ScoreW1
                              ,Scorew2
                              ,Scorew3
                              ,Scorew4
                              ,Scorew5
                              ,Scorew6
                              ,Scorew7
                              ,Scorew8
                              ,Scorew9
                              ,Scorew10
                              ,Scorew11
                              ,Scorew12
                              ,Scorew13
                              ,Scorew14
                              ,Scorew15
                              ,Scorew16
                              ,JointLap
                              ,ScoreL2
                              ,ScoreL3
                              ,ScoreL4
                              ,ScoreL5
                              ,ScoreL6
                              ,ScoreL7
                              ,ScoreL8
                              ,ScoreL9
                              ,Slit
                              ,No_Slot as NoSlot
                              ,Bun
                              ,BunLayer
                              ,LayerPalet
                              ,BoxPalet
                              ,Weight_Sh as WeightSh
                              ,Weight_Box as WeightBox
                              ,SparePercen
                              ,SpareMax
                              ,SpareMin
                              ,LeadTime
                              ,Piece_Set as PieceSet
                              ,Sale_UOM as SaleUom
                              ,BOM_UOM as BomUom
                              ,Hardship
                              ,PalletSize
                              ,Palletization_Path as PalletizationPath
                              ,PrintMaster_Path as PrintMasterPath
                              ,DiecutPict_Path as DiecutPictPath
                              ,CreateDate
                              ,LastUpdate
                              ,User
                              ,Plt_Leg_Double as PltLegDouble
                              ,Plt_Double_axle as PltDoubleAxle
                              ,Plt_Leg_Single as PltLegSingle
                              ,Plt_Single_axle as PltSingleAxle
                              ,Plt_Floor_Above as PltFloorAbove
                              ,Plt_Floor_Under as PltFloorUnder
                              ,Plt_Beam as PltBeam
                              ,Plt_Axle_Height as PltAxleHeight
                              ,EanCode
                              ,NewH
                              ,High_Group as HighGroup
                              ,High_Value as HighValue
                              ,ChangeInfo
                              ,Piece_Patch as PiecePatch
                              ,BoxHandle
                              ,PSM_ID  as PsmId
                              ,PicPallet
                              ,ChangeHistory
                              ,CustComment
                              ,MaterialComment
                              ,CutSheetWid_Inch as CutSheetWidInch
                              ,CutSheetLeng_Inch as CutSheetLengInch
                              ,Joint_ID as JointId
                              ,CustInvType
                              ,CIPInvType
                                FROM MO_Spec where ({0})
                            ";

            string message1 = string.Format(sql1, ListOrder);
            var temp = db.Query<MoSpec>(message1).ToList();
            MoSpecTemp.AddRange(temp);

            string sql2 = @"
                                 SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Seq_No as SeqNo
                              ,Plant
                              ,Material_No as MaterialNo
                              ,Mat_Code as MatCode
                              ,Plan_Code as PlanCode
                              ,Machine
                              ,Alternative1
                              ,Alternative2
                              ,Std_Process as StdProcess
                              ,Speed
                              ,Colour_Count as ColourCount
                              ,MC_Move as McMove
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
                              ,Setup_waste as  SetupWaste
                              ,Prepare_tm as PrepareTm
                              ,Post_tm as PostTm
                              ,Run_waste as RunWaste
                              ,Human
                              ,Color_count as ColorCount
                              ,UnUpgrad_Board as  UnUpgradBoard
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
                              FROM MO_Routing where ({0})
                            ";

            string message2 = string.Format(sql2, ListOrder);
            var TempmoRoutings = db.Query<MoRouting>(message2).ToList();
            MoRoutingTemp.AddRange(TempmoRoutings);

            orderTrackingServiceModel.moSpecs = MoSpecTemp;
            orderTrackingServiceModel.moRoutings = MoRoutingTemp;

            return orderTrackingServiceModel;

        }



        public OrderTrackingServiceModel OrderTrackingService(IConfiguration config, string FactoryCode, string UpdateDateFrom, string UpdateDateTo)
        {
            OrderTrackingServiceModel orderTrackingServiceModel = new OrderTrackingServiceModel();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                                SELECT Id
                              ,FactoryCode
                              ,MO_Status as MoStatus
                              ,OrderItem
                              ,Material_No as  MaterialNo
                              ,Name
                              ,Order_Quant as OrderQuant
                              ,Tolerance_Over as ToleranceOver
                              ,Tolerance_Under as ToleranceUnder
                              ,Due_Date as DueDate
                              ,Target_Quant as TargetQuant
                              ,Item_Note as ItemNote
                              ,District 
                              ,PO_No as PoNo
                              ,DateTimeStamp
                              ,Printed
                              ,Batch
                              ,Due_Text as DueText
                              ,Sold_to as SoldTo
                              ,Ship_to as ShipTo
                              ,PlanStatus
                              ,StockQty
                              ,IsCreateManual
                              ,SentKIWI
                              ,UpdatedDate
                              ,UpdatedBy
                              ,MORNo
                            FROM MO_DATA where {0}  (UpdatedDate >= '{1}' AND   UpdatedDate <= '{2}')  and isCreateManual = 0
                        ";

            string message = string.Format(sql, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), UpdateDateFrom, UpdateDateTo);
            orderTrackingServiceModel.moDatas = db.Query<MoData>(message).ToList();
            List<string> conditionlist = new List<string>();
            string ss = string.Empty;
            for (int i = 1; i <= orderTrackingServiceModel.moDatas.Count; i++)
            {
                string mat = "'" + orderTrackingServiceModel.moDatas[i - 1].MaterialNo.ToString() + "',";
                ss = ss + mat;
                if ((i % 1000) == 0)
                {
                    string tmpss = ss.Substring(0, ss.Length - 1);
                    conditionlist.Add(tmpss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss))
            {
                string tmpss2 = ss.Substring(0, ss.Length - 1);
                conditionlist.Add(tmpss2);
            }


            List<MoSpec> MoSpecTemp = new List<MoSpec>();
            List<MoRouting> MoRoutingTemp = new List<MoRouting>();
            for (int it = 0; it < conditionlist.Count; it++)
            {
                string sql1 = @"
                        SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Material_No as MaterialNo
                              ,Part_No as PartNo
                              ,PC
                              ,Hierarchy
                              ,Sale_Org as SaleOrg
                              ,Plant
                              ,Cust_Code as CustCode
                              ,Cus_ID as CusId
                              ,Cust_Name as CustName
                              ,Description
                              ,Sale_Text1 as SaleText1
                              ,Sale_Text2 as SaleText2
                              ,Sale_Text3 as SaleText3
                              ,Sale_Text4 as SaleText4
                              ,Change
                              ,Language
                              ,Ind_Grp as Ind_Grp
                              ,Ind_Des as Ind_Des
                              ,Material_Type as MaterialType
                              ,Print_Method as PrintMethod
                              ,TwoPiece
                              ,Flute
                              ,Code
                              ,Board
                              ,GL
                              ,GLWeigth
                              ,BM
                              ,BMWeigth
                              ,BL
                              ,BLWeigth
                              ,CM
                              ,CMWeigth
                              ,CL
                              ,CLWeigth
                              ,DM
                              ,DMWeigth
                              ,DL
                              ,DLWeigth
                              ,Wid
                              ,Leg
                              ,Hig
                              ,Box_Type as BoxType
                              ,RSC_Style as RscStyle
                              ,Pro_Type as ProType
                              ,JoinType
                              ,Status_Flag as StatusFlag
                              ,Priority_Flag as PriorityFlag
                              ,Wire
                              ,Outer_Join as OuterJoin
                              ,CutSheetLeng
                              ,CutSheetWid
                              ,Sheet_Area as SheetArea
                              ,Box_Area as BoxArea
                              ,ScoreW1
                              ,Scorew2
                              ,Scorew3
                              ,Scorew4
                              ,Scorew5
                              ,Scorew6
                              ,Scorew7
                              ,Scorew8
                              ,Scorew9
                              ,Scorew10
                              ,Scorew11
                              ,Scorew12
                              ,Scorew13
                              ,Scorew14
                              ,Scorew15
                              ,Scorew16
                              ,JointLap
                              ,ScoreL2
                              ,ScoreL3
                              ,ScoreL4
                              ,ScoreL5
                              ,ScoreL6
                              ,ScoreL7
                              ,ScoreL8
                              ,ScoreL9
                              ,Slit
                              ,No_Slot as NoSlot
                              ,Bun
                              ,BunLayer
                              ,LayerPalet
                              ,BoxPalet
                              ,Weight_Sh as WeightSh
                              ,Weight_Box as WeightBox
                              ,SparePercen
                              ,SpareMax
                              ,SpareMin
                              ,LeadTime
                              ,Piece_Set as PieceSet
                              ,Sale_UOM as SaleUom
                              ,BOM_UOM as BomUom
                              ,Hardship
                              ,PalletSize
                              ,Palletization_Path as PalletizationPath
                              ,PrintMaster_Path as PrintMasterPath
                              ,DiecutPict_Path as DiecutPictPath
                              ,CreateDate
                              ,LastUpdate
                              ,User
                              ,Plt_Leg_Double as PltLegDouble
                              ,Plt_Double_axle as PltDoubleAxle
                              ,Plt_Leg_Single as PltLegSingle
                              ,Plt_Single_axle as PltSingleAxle
                              ,Plt_Floor_Above as PltFloorAbove
                              ,Plt_Floor_Under as PltFloorUnder
                              ,Plt_Beam as PltBeam
                              ,Plt_Axle_Height as PltAxleHeight
                              ,EanCode
                              ,NewH
                              ,High_Group as HighGroup
                              ,High_Value as HighValue
                              ,ChangeInfo
                              ,Piece_Patch as PiecePatch
                              ,BoxHandle
                              ,PSM_ID  as PsmId
                              ,PicPallet
                              ,ChangeHistory
                              ,CustComment
                              ,MaterialComment
                              ,CutSheetWid_Inch as CutSheetWidInch
                              ,CutSheetLeng_Inch as CutSheetLengInch
                              ,Joint_ID as JointId
                              ,CustInvType
                              ,CIPInvType
                                FROM MO_Spec where {0}  Material_No in ({1})
                            ";

                string message1 = string.Format(sql1, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), conditionlist[it]);
                var temp = db.Query<MoSpec>(message1).ToList();
                MoSpecTemp.AddRange(temp);

                string sql2 = @"
                                 SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Seq_No as SeqNo
                              ,Plant
                              ,Material_No as MaterialNo
                              ,Mat_Code as MatCode
                              ,Plan_Code as PlanCode
                              ,Machine
                              ,Alternative1
                              ,Alternative2
                              ,Std_Process as StdProcess
                              ,Speed
                              ,Colour_Count as ColourCount
                              ,MC_Move as McMove
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
                              ,Setup_waste as  SetupWaste
                              ,Prepare_tm as PrepareTm
                              ,Post_tm as PostTm
                              ,Run_waste as RunWaste
                              ,Human
                              ,Color_count as ColorCount
                              ,UnUpgrad_Board as  UnUpgradBoard
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
                              FROM MO_Routing where {0} Material_No in ({1})
                            ";

                string message2 = string.Format(sql2, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), conditionlist[it]);
                var TempmoRoutings = db.Query<MoRouting>(message2).ToList();
                MoRoutingTemp.AddRange(TempmoRoutings);

            }

            orderTrackingServiceModel.moSpecs = MoSpecTemp;
            orderTrackingServiceModel.moRoutings = MoRoutingTemp;

            return orderTrackingServiceModel;

        }


        public List<MoData> OrderTrackingServiceMoData(IConfiguration config, string FactoryCode, string UpdateDateFrom, string UpdateDateTo)
        {
            OrderTrackingServiceModel orderTrackingServiceModel = new OrderTrackingServiceModel();
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                                SELECT Id
                              ,FactoryCode
                              ,MO_Status as MoStatus
                              ,OrderItem
                              ,Material_No as  MaterialNo
                              ,Name
                              ,Order_Quant as OrderQuant
                              ,Tolerance_Over as ToleranceOver
                              ,Tolerance_Under as ToleranceUnder
                              ,Due_Date as DueDate
                              ,Target_Quant as TargetQuant
                              ,Item_Note as ItemNote
                              ,District 
                              ,PO_No as PoNo
                              ,DateTimeStamp
                              ,Printed
                              ,Batch
                              ,Due_Text as DueText
                              ,Sold_to as SoldTo
                              ,Ship_to as ShipTo
                              ,PlanStatus
                              ,StockQty
                              ,IsCreateManual
                              ,SentKIWI
                              ,UpdatedDate
                              ,UpdatedBy
                              ,MORNo
                            FROM MO_DATA where {0}  (UpdatedDate >= '{1}' AND   UpdatedDate <= '{2}')  and isCreateManual = 0
                        ";

            string message = string.Format(sql, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), UpdateDateFrom, UpdateDateTo);
            return db.Query<MoData>(message).ToList();

        }
        public List<MoSpec> OrderTrackingServiceMoSpect(IConfiguration config, string FactoryCode, List<string> orderTrackingServiceModel)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            List<string> conditionlist = new List<string>();
            string ss = string.Empty;
            for (int i = 1; i <= orderTrackingServiceModel.Count; i++)
            {
                string mat = "'" + orderTrackingServiceModel[i - 1].ToString() + "',";
                ss = ss + mat;
                if ((i % 1000) == 0)
                {
                    string tmpss = ss.Substring(0, ss.Length - 1);
                    conditionlist.Add(tmpss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss))
            {
                string tmpss2 = ss.Substring(0, ss.Length - 1);
                conditionlist.Add(tmpss2);
            }




            List<MoSpec> MoSpecTemp = new List<MoSpec>();
            for (int it = 0; it < conditionlist.Count; it++)
            {
                string sql1 = @"
                        SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Material_No as MaterialNo
                              ,Part_No as PartNo
                              ,PC
                              ,Hierarchy
                              ,Sale_Org as SaleOrg
                              ,Plant
                              ,Cust_Code as CustCode
                              ,Cus_ID as CusId
                              ,Cust_Name as CustName
                              ,Description
                              ,Sale_Text1 as SaleText1
                              ,Sale_Text2 as SaleText2
                              ,Sale_Text3 as SaleText3
                              ,Sale_Text4 as SaleText4
                              ,Change
                              ,Language
                              ,Ind_Grp as Ind_Grp
                              ,Ind_Des as Ind_Des
                              ,Material_Type as MaterialType
                              ,Print_Method as PrintMethod
                              ,TwoPiece
                              ,Flute
                              ,Code
                              ,Board
                              ,GL
                              ,GLWeigth
                              ,BM
                              ,BMWeigth
                              ,BL
                              ,BLWeigth
                              ,CM
                              ,CMWeigth
                              ,CL
                              ,CLWeigth
                              ,DM
                              ,DMWeigth
                              ,DL
                              ,DLWeigth
                              ,Wid
                              ,Leg
                              ,Hig
                              ,Box_Type as BoxType
                              ,RSC_Style as RscStyle
                              ,Pro_Type as ProType
                              ,JoinType
                              ,Status_Flag as StatusFlag
                              ,Priority_Flag as PriorityFlag
                              ,Wire
                              ,Outer_Join as OuterJoin
                              ,CutSheetLeng
                              ,CutSheetWid
                              ,Sheet_Area as SheetArea
                              ,Box_Area as BoxArea
                              ,ScoreW1
                              ,Scorew2
                              ,Scorew3
                              ,Scorew4
                              ,Scorew5
                              ,Scorew6
                              ,Scorew7
                              ,Scorew8
                              ,Scorew9
                              ,Scorew10
                              ,Scorew11
                              ,Scorew12
                              ,Scorew13
                              ,Scorew14
                              ,Scorew15
                              ,Scorew16
                              ,JointLap
                              ,ScoreL2
                              ,ScoreL3
                              ,ScoreL4
                              ,ScoreL5
                              ,ScoreL6
                              ,ScoreL7
                              ,ScoreL8
                              ,ScoreL9
                              ,Slit
                              ,No_Slot as NoSlot
                              ,Bun
                              ,BunLayer
                              ,LayerPalet
                              ,BoxPalet
                              ,Weight_Sh as WeightSh
                              ,Weight_Box as WeightBox
                              ,SparePercen
                              ,SpareMax
                              ,SpareMin
                              ,LeadTime
                              ,Piece_Set as PieceSet
                              ,Sale_UOM as SaleUom
                              ,BOM_UOM as BomUom
                              ,Hardship
                              ,PalletSize
                              ,Palletization_Path as PalletizationPath
                              ,PrintMaster_Path as PrintMasterPath
                              ,DiecutPict_Path as DiecutPictPath
                              ,CreateDate
                              ,LastUpdate
                              ,User
                              ,Plt_Leg_Double as PltLegDouble
                              ,Plt_Double_axle as PltDoubleAxle
                              ,Plt_Leg_Single as PltLegSingle
                              ,Plt_Single_axle as PltSingleAxle
                              ,Plt_Floor_Above as PltFloorAbove
                              ,Plt_Floor_Under as PltFloorUnder
                              ,Plt_Beam as PltBeam
                              ,Plt_Axle_Height as PltAxleHeight
                              ,EanCode
                              ,NewH
                              ,High_Group as HighGroup
                              ,High_Value as HighValue
                              ,ChangeInfo
                              ,Piece_Patch as PiecePatch
                              ,BoxHandle
                              ,PSM_ID  as PsmId
                              ,PicPallet
                              ,ChangeHistory
                              ,CustComment
                              ,MaterialComment
                              ,CutSheetWid_Inch as CutSheetWidInch
                              ,CutSheetLeng_Inch as CutSheetLengInch
                              ,Joint_ID as JointId
                              ,CustInvType
                              ,CIPInvType
                                FROM MO_Spec where {0}  Material_No in ({1})
                            ";

                string message1 = string.Format(sql1, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), conditionlist[it]);
                var temp = db.Query<MoSpec>(message1).ToList();
                MoSpecTemp.AddRange(temp);

            }
            return MoSpecTemp;

        }

        public List<MoRouting> OrderTrackingServiceMORouting(IConfiguration config, string FactoryCode, List<string> orderTrackingServiceModel)
        {

            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            List<string> conditionlist = new List<string>();
            string ss = string.Empty;
            for (int i = 1; i <= orderTrackingServiceModel.Count; i++)
            {
                string mat = "'" + orderTrackingServiceModel[i - 1].ToString() + "',";
                ss = ss + mat;
                if ((i % 1000) == 0)
                {
                    string tmpss = ss.Substring(0, ss.Length - 1);
                    conditionlist.Add(tmpss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss))
            {
                string tmpss2 = ss.Substring(0, ss.Length - 1);
                conditionlist.Add(tmpss2);
            }

            List<MoRouting> MoRoutingTemp = new List<MoRouting>();
            for (int it = 0; it < conditionlist.Count; it++)
            {

                string sql2 = @"
                                 SELECT Id
                              ,FactoryCode
                              ,OrderItem
                              ,Seq_No as SeqNo
                              ,Plant
                              ,Material_No as MaterialNo
                              ,Mat_Code as MatCode
                              ,Plan_Code as PlanCode
                              ,Machine
                              ,Alternative1
                              ,Alternative2
                              ,Std_Process as StdProcess
                              ,Speed
                              ,Colour_Count as ColourCount
                              ,MC_Move as McMove
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
                              ,Setup_waste as  SetupWaste
                              ,Prepare_tm as PrepareTm
                              ,Post_tm as PostTm
                              ,Run_waste as RunWaste
                              ,Human
                              ,Color_count as ColorCount
                              ,UnUpgrad_Board as  UnUpgradBoard
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
                              FROM MO_Routing where {0} Material_No in ({1})
                            ";

                string message2 = string.Format(sql2, string.IsNullOrEmpty(FactoryCode) ? "" : string.Format(" FactoryCode = '{0}' and ", FactoryCode), conditionlist[it]);
                var TempmoRoutings = db.Query<MoRouting>(message2).ToList();
                MoRoutingTemp.AddRange(TempmoRoutings);

            }

            return MoRoutingTemp;

        }
    }
}
