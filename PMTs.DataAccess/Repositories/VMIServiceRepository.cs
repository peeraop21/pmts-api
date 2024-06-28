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
    public class VMIServiceRepository : Repository<MasterData>, IVMIServiceRepository
    {
        public VMIServiceRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<VMIAllMasterDataModel> GetAllMasterDataByCustInvType(IConfiguration config, string CustInvType)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                            SELECT 
                           FactoryCode
                          ,Material_No as MaterialNo                         
                          FROM MasterData where {0}
                        ";
            string message = string.Format(sql, string.IsNullOrEmpty(CustInvType) ? "CustInvType in  ('VMI','JIT', 'GEN')" : string.Format("CustInvType = '{0}'", CustInvType));
            return db.Query<VMIAllMasterDataModel>(message).ToList();

        }

        public List<MasterData> GetMasterDataByCustInvType(IConfiguration config, string CustInvType)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                            SELECT  Id
                          ,FactoryCode
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
                          ,Ind_Grp as IndGrp
                          ,Ind_Des as IndDes
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
                          ,RSC_Style as RSCStyle
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
                          ,FGPic_Path as FgpicPath
                          ,CreateDate
                          ,CreatedBy
                          ,LastUpdate
                          ,UpdatedBy
                          ,User
                          ,Plt_Leg_Double as  PltLegDouble
                          ,Plt_Double_axle  as PltDoubleAxle
                          ,Plt_Leg_Single as PltLegSingle
                          ,Plt_Single_axle as PltSingleAxle
                          ,Plt_Floor_Above  as PltFloorAbove
                          ,Plt_Floor_Under as PltFloorUnder
                          ,Plt_Beam as PltBeam
                          ,Plt_Axle_Height as PltAxleHeight
                          ,EanCode
                          ,PDIS_Status as PdisStatus
                          ,Tran_Status as TranStatus
                          ,SAP_Status as SapStatus
                          ,NewH
                          ,Pur_Txt1 as PurTxt1 
                          ,Pur_Txt2 as PurTxt2
                          ,Pur_Txt3 as PurTxt3
                          ,Pur_Txt4 as PurTxt4
                          ,UnUpgrad_Board as UnUpgradBoard
                          ,High_Group as HighGroup
                          ,High_Value  as HighValue
                          ,ChangeInfo
                          ,Piece_Patch as PiecePatch
                          ,BoxHandle
                          ,PSM_ID as PsmId
                          ,PicPallet
                          ,ChangeHistory
                          ,CustComment
                          ,MaterialComment
                          ,CutSheetWid_Inch as CutSheetWidInch
                          ,CutSheetLeng_Inch as CutSheetLengInch
                          ,Joint_ID as JointId
                          ,CustInvType
                          ,CIPInvType 
                      FROM MasterData where ({0})
                        ";

            string message = string.Format(sql, CustInvType);
            return db.Query<MasterData>(message).ToList();

        }

        public List<Routing> GetRoutingByListMaterialNo(IConfiguration config, string MaterialList)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
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
                          FROM Routing where ({0})
                        ";
            string message = string.Format(sql, MaterialList);
            return db.Query<Routing>(message).ToList();

        }

        public List<MoData> GetMoDataByListMaterialNo(IConfiguration config, string MaterialList)
        {
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
                          FROM MO_DATA
                          WHERE (Due_Date > DATEADD(day, -30, GETDATE())) and  (Due_Date < DATEADD(day, 30, GETDATE())) and  ({0})
                        ";
            string data = MaterialList;//"'Z031103580','Z031103580'";
            if (string.IsNullOrEmpty(data))
            {
                data = "''";
            }
            string message = string.Format(sql, MaterialList);
            return db.Query<MoData>(message).ToList();

        }

        //Tassanai Update

        public List<BomStruct> GetBomStructs(IConfiguration config, string MaterialList)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            string sql = @"
                        SELECT Id
                              ,FactoryCode
                              ,Material_No as MaterialNo
                              ,Plant
                              ,Bom_Usage as BomUsage
                              ,Weigh_Bom as WeighBom
                              ,Previous_Bom as PreviousBom
                              ,Follower
                              ,Amount
                              ,Unit
                              ,PDIS_Status as PDISStatus
                              ,Tran_Status as TranStatus
                              ,SAP_Status as SAPStatus
                          FROM Bom_Struct
                          WHERE (PDIS_Status != 'X') and  ({0})
                        ";
            string data = MaterialList;//"'Z031103580','Z031103580'";
            if (string.IsNullOrEmpty(data))
            {
                data = "''";
            }
            string message = string.Format(sql, MaterialList);
            return db.Query<BomStruct>(message).ToList();

        }

    }
}

