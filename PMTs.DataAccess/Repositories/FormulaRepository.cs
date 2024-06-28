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
    public class FormulaRepository(PMTsDbContext context) : IFormulaRepository// Repository<MasterData>, IFormulaRepository
    {
        readonly PMTsDbContext PMTsDbContext = context;

        private List<string> GetPaperItemByMaterialNo(string factoryCode, string materialNo, BoardAlternative BoardAlt = null, BoardUse BoardUse = null, bool isTuning = false)
        {
            if (BoardAlt == null && isTuning == false) BoardAlt = PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo == materialNo && b.FactoryCode == factoryCode).FirstOrDefault();

            List<string> result = [];

            if (BoardAlt != null)
            {
                string[] ArrBoard;

                ArrBoard = BoardAlt.BoardName.Split("/");
                if (ArrBoard.Length > 0)
                    result.Add(ArrBoard[0]);
                if (ArrBoard.Length > 1)
                    result.Add(ArrBoard[1]);
                if (ArrBoard.Length > 2)
                    result.Add(ArrBoard[2]);
                if (ArrBoard.Length > 3)
                    result.Add(ArrBoard[3]);
                if (ArrBoard.Length > 4)
                    result.Add(ArrBoard[4]);
                if (ArrBoard.Length > 5)
                    result.Add(ArrBoard[5]);
                if (ArrBoard.Length > 6)
                    result.Add(ArrBoard[6]);
            }
            else
            {
                if (BoardUse == null && isTuning == false) BoardUse = PMTsDbContext.BoardUse.Where(b => b.MaterialNo == materialNo && b.FactoryCode == factoryCode).FirstOrDefault();

                if (BoardUse != null)
                {
                    if (BoardUse.Gl != null)
                    {
                        result.Add(BoardUse.Gl);
                    }
                    if (BoardUse.Bm != null)
                    {
                        result.Add(BoardUse.Bm);
                    }
                    if (BoardUse.Bl != null)
                    {
                        result.Add(BoardUse.Bl);
                    }
                    if (BoardUse.Cm != null && BoardUse.Cm != "")
                    {
                        result.Add(BoardUse.Cm);
                    }
                    if (BoardUse.Cl != null && BoardUse.Cl != "")
                    {
                        result.Add(BoardUse.Cl);
                    }
                    if (BoardUse.Dm != null && BoardUse.Dm != "")
                    {
                        result.Add(BoardUse.Dm);
                    }
                    if (BoardUse.Dl != null && BoardUse.Dl != "")
                    {
                        result.Add(BoardUse.Dl);
                    }
                }
                else
                {
                    var moSpec = PMTsDbContext.MoSpec.Where(b => b.MaterialNo == materialNo && b.FactoryCode == factoryCode).FirstOrDefault();
                    if (moSpec != null)
                    {
                        string[] ArrBoard;

                        ArrBoard = moSpec.Board.Split("/");
                        if (ArrBoard.Length > 0)
                            result.Add(ArrBoard[0]);
                        if (ArrBoard.Length > 1)
                            result.Add(ArrBoard[1]);
                        if (ArrBoard.Length > 2)
                            result.Add(ArrBoard[2]);
                        if (ArrBoard.Length > 3)
                            result.Add(ArrBoard[3]);
                        if (ArrBoard.Length > 4)
                            result.Add(ArrBoard[4]);
                        if (ArrBoard.Length > 5)
                            result.Add(ArrBoard[5]);
                        if (ArrBoard.Length > 6)
                            result.Add(ArrBoard[6]);
                    }
                    return result;
                }
            }

            return result;
        }

        public RoutingDataModel CalculateRouting(string machineName, string _factoryCode, string flut, int trans_cutsheetwid, string trans_material, IEnumerable<string> ppItem, int trim, int cut, CorConfig corConfig, PmtsConfig pmtsConfig, Flute flute, List<PaperWidth> RollWidth, List<PaperGrade> grade, MachineFluteTrim machineFluteTrim = null, BoardAlternative boardAlternative = null, BoardUse boardUse = null, bool isTuning = false)
        {
            var ret = new RoutingDataModel();

            try
            {
                int sheetIn_W = trans_cutsheetwid; // (tran.modelProductSpec.CutSheetWid).GetValueOrDefault();
                int maxCut = corConfig == null ? 6 : corConfig.CutOff;
                if (machineFluteTrim == null && isTuning == false) machineFluteTrim = PMTsDbContext.MachineFluteTrim.Where(m => m.FactoryCode == _factoryCode && m.Machine == machineName && m.Flute == flut).FirstOrDefault();

                int? trimMachine = machineFluteTrim == null ? 0 : machineFluteTrim.Trim;
                int trimWaste = trim == 0 ? trimMachine == 0 ? (int)flute.Trim : (int)trimMachine : trim;
                int? sizeWidth = 0;
                int pageMin = corConfig.MinOut;
                int? pageMax = corConfig.MaxOut;
                //int? pageMax = 0;
                //Mintrim
                var PaperItem = ppItem ?? GetPaperItemByMaterialNo(_factoryCode, trans_material, boardAlternative, boardUse, isTuning);//JsonConvert.DeserializeObject<List<string>>(_boardUseAPIRepository.GetPaperItemByMaterialNo(_factoryCode, tran.MaterialNo));
                PaperGrade PaperGrad;
                int? GroupItem = 10000;

                if (PaperItem.Any())
                {
                    foreach (var Item in PaperItem)
                    {
                        if (Item != "")
                        {
                            //PaperGrad = PMTsDbContext.PaperGrade.Where(w => w.Grade == Item && w.Active == true).FirstOrDefault(); //JsonConvert.DeserializeObject<PaperGrade>(_paperGradeAPIRepository.GetPaperGradeByGrade(_factoryCode, Item));
                            PaperGrad = grade.Where(w => w.Grade == Item.Trim()).FirstOrDefault();
                            if (PaperGrad != null)
                            {
                                if (PaperGrad.MaxPaperWidth != null)
                                {
                                    pageMax = pageMax > PaperGrad.MaxPaperWidth ? PaperGrad.MaxPaperWidth : pageMax;
                                }
                            }
                        }
                    }
                }

                //ไม่ได้ใช้นะ by พี่แอด พี่เสริม *ใช้ page max ด้านบนแทน
                //if (GroupItem == 1 && pageMax == 0)
                //{
                //    //pageMax = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group1);
                //    pageMax = RollWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group1);
                //}
                //else if (GroupItem == 2 && pageMax == 0)
                //{
                //    //pageMax = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group2);
                //    pageMax = RollWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group2);
                //}
                //else if (GroupItem == 3 && pageMax == 0)
                //{
                //    //pageMax = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group3);
                //    pageMax = RollWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group3);
                //}
                //else if (GroupItem == 4 && pageMax == 0)
                //{
                //    //pageMax = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group4);
                //    pageMax = RollWidth.Where(x => x.FactoryCode == _factoryCode).Max(x => x.Group4);
                //}

                if (pmtsConfig.FucValue == "True")//(corConfig.Mintrim) //Min Trim
                {

                    double[,] RollSize = new double[6, 4];
                    int X, M;

                    ret.PercentTrim = "1000";//กำหนดค่าหลอกเพื่อไปเปรียบเทียบกับ % Trimน้อยสุด

                    var Roll = RollWidth.FirstOrDefault(w => w.Group1 == pageMin || w.Group2 == pageMin || w.Group3 == pageMin || w.Group4 == pageMin);

                    if (Roll != null)
                    {
                        pageMin = GroupItem == 1 ? ConvertInt16ToShort(Roll.Group1) : GroupItem == 2 ? ConvertInt16ToShort(Roll.Group2) : GroupItem == 3 ? ConvertInt16ToShort(Roll.Group3) : ConvertInt16ToShort(Roll.Group4);

                    }
                    int z = 0;
                    for (X = 0; X < maxCut; X++) //คำนวนหน้ากว้าง + Standard Trim
                    {
                        RollSize[X, 1] = (sheetIn_W * (X + 1)) + trimWaste;

                        if (RollSize[X, 1] < pageMin)
                        {
                            RollSize[X, 0] = pageMin;   //น้อยกว่าหน้าน้อยสุด
                        }
                        else if (RollSize[X, 1] > pageMax)
                        {
                            X = X == 0 ? 0 : X - 1;
                            break;
                        }
                        else
                        {
                            for (M = 0; M < RollWidth.Count; M++)
                            {
                                if (ConvertInt16ToShort(RollWidth[M].Width) >= pageMin && ConvertInt16ToShort(RollWidth[M].Width) <= pageMax)
                                {
                                    if (ConvertInt16ToShort(RollWidth[M].Width) >= RollSize[X, 1])
                                    {
                                        RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Width);  //ตุีกตาตัวที่ 1
                                        GroupItem = 10000;
                                        foreach (var Item in PaperItem) //ตุีกตาตัวที่ 2 เทียบกันใน grade แต่ละตัวว่า groupPaperWidth ไหนน้อยสุด
                                        {
                                            if (Item != "")
                                            {
                                                //PaperGrad = PMTsDbContext.PaperGrade.Where(w => w.Grade == Item && w.Active == true).FirstOrDefault(); //JsonConvert.DeserializeObject<PaperGrade>(_paperGradeAPIRepository.GetPaperGradeByGrade(_factoryCode, Item));
                                                PaperGrad = grade.Where(w => w.Grade == Item.Trim()).FirstOrDefault();
                                                if (PaperGrad != null)
                                                {
                                                    switch (PaperGrad.Group)
                                                    {
                                                        case 1:
                                                            GroupItem = RollWidth[M].Group1 <= GroupItem && RollWidth[M].Group1 != 0 ? RollWidth[M].Group1 : GroupItem;
                                                            z = RollWidth[M].Group1 <= GroupItem && RollWidth[M].Group1 != 0 ? 1 : z;
                                                            break;
                                                        case 2:
                                                            GroupItem = RollWidth[M].Group2 <= GroupItem && RollWidth[M].Group2 != 0 ? RollWidth[M].Group2 : GroupItem;
                                                            z = RollWidth[M].Group2 <= GroupItem && RollWidth[M].Group2 != 0 ? 2 : z;
                                                            break;
                                                        case 3:
                                                            GroupItem = RollWidth[M].Group3 <= GroupItem && RollWidth[M].Group3 != 0 ? RollWidth[M].Group3 : GroupItem;
                                                            z = RollWidth[M].Group3 <= GroupItem && RollWidth[M].Group3 != 0 ? 3 : z;
                                                            break;
                                                        case 4:
                                                            GroupItem = RollWidth[M].Group4 <= GroupItem && RollWidth[M].Group4 != 0 ? RollWidth[M].Group4 : GroupItem;
                                                            z = RollWidth[M].Group4 <= GroupItem && RollWidth[M].Group4 != 0 ? 4 : z;
                                                            break;
                                                    }
                                                }
                                            }
                                        }

                                        //RollSize[X, 0] = RollSize[X, 0] <= ConvertInt16ToShort(GroupItem) && GroupItem != 10000 ? ConvertInt16ToShort(GroupItem) : RollWidth[M+1].Width; 
                                        if (RollSize[X, 1] <= GroupItem && GroupItem != 10000)
                                        {
                                            RollSize[X, 0] = Convert.ToDouble(GroupItem);
                                        }
                                        else if (GroupItem != 10000)
                                        {
                                            if (M < RollWidth.Count - 1)
                                            {
                                                RollSize[X, 0] = z == 1 ? Convert.ToDouble(RollWidth[M + 1].Group1)
                                                          : z == 2 ? Convert.ToDouble(RollWidth[M + 1].Group2)
                                                          : z == 3 ? Convert.ToDouble(RollWidth[M + 1].Group3)
                                                          : z == 4 ? Convert.ToDouble(RollWidth[M + 1].Group4)
                                                          : Convert.ToDouble(RollWidth[M + 1].Width);
                                            }
                                            else
                                            {
                                                RollSize[X, 0] = 0;
                                                RollSize[X, 1] = 0;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //if(z != 0) break;
                    }

                    if (X >= maxCut) X = maxCut - 1;

                    if (RollSize[X, 0] > 0)
                    {
                        for (var Z = X; Z >= 0; Z--)
                        {
                            RollSize[Z, 2] = (RollSize[Z, 0] - (RollSize[Z, 1] - trimWaste)) / RollSize[Z, 0] * 100; //คำนวน % Trim

                            if (Convert.ToDouble(ret.PercentTrim) >= Math.Round(RollSize[Z, 2], 2))//เลือก % Trim น้อยที่สุด
                            {
                                ret.PaperRollWidth = RollSize[Z, 0].ToString();
                                ret.Cut = (Z + 1).ToString();
                                ret.Trim = (RollSize[Z, 0] - RollSize[Z, 1] + trimWaste).ToString(); //เศษ
                                ret.PercentTrim = (Math.Round(RollSize[Z, 2], 2)).ToString();
                            }
                        }
                    }
                    else
                    {
                        X = X == 0 ? 0 : X - 1;
                        for (var Z = X; Z >= 0; Z--)
                        {
                            RollSize[Z, 2] = (RollSize[Z, 0] - (RollSize[Z, 1] - trimWaste)) / RollSize[Z, 0] * 100; //คำนวน % Trim

                            if (Convert.ToDouble(ret.PercentTrim) >= Math.Round(RollSize[Z, 2], 2))//เลือก % Trim น้อยที่สุด
                            {
                                ret.PaperRollWidth = RollSize[Z, 0].ToString();
                                ret.Cut = (Z + 1).ToString();
                                ret.Trim = (RollSize[Z, 0] - RollSize[Z, 1] + trimWaste).ToString(); //เศษ
                                ret.PercentTrim = (Math.Round(RollSize[Z, 2], 2)).ToString();
                            }
                        }
                    }
                }
                else //Max Out
                {
                    maxCut = cut == 0 ? maxCut : cut;
                    sizeWidth = (sheetIn_W * maxCut) + trimWaste;

                    //ตรวจหาหน้ากว้างสุดและแคบสุด
                    if (sizeWidth < pageMin)
                    {
                        ret.PaperRollWidth = pageMin.ToString();                                //Paper Width
                        ret.Cut = maxCut.ToString();                                            //จำนวนตัด
                        ret.Trim = (pageMin - sheetIn_W * maxCut).ToString();                            //เศษตัดริม
                        ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(pageMin) * 100), 2).ToString();   //% Waste
                        return ret;
                    }
                    else if (sheetIn_W + trimWaste > pageMax)
                    {
                        ret.PaperRollWidth = pageMax.ToString();                                //Paper Width
                        ret.Cut = "1";                                            //จำนวนตัด
                        ret.Trim = (pageMax - sheetIn_W).ToString();                        //เศษตัดริม
                        ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(pageMax) * 100), 2).ToString();   //% Waste
                        return ret;
                    }
                    else
                    {
                        int k = maxCut;
                        for (k = maxCut; k > 0; k--)
                        {
                            if (sizeWidth > pageMin)
                            {
                                sizeWidth = sheetIn_W * k + trimWaste;
                                if (sizeWidth <= pageMax)
                                {
                                    break;
                                }
                            }
                            else break;
                        }
                        int b = 0;
                        int z = 0;
                        foreach (var rollWidth in RollWidth)
                        {
                            if (rollWidth.Width >= sizeWidth)
                            {
                                //sizeWidth = rollWidth.Width;   //ตุีกตาตัวที่ 1

                                if (PaperItem.Any())
                                {
                                    foreach (var Item in PaperItem) //ตุีกตาตัวที่ 2 เทียบกันใน grade แต่ละตัวว่า groupPaperWidth ไหนน้อยสุด
                                    {
                                        if (Item != "")
                                        {
                                            //PaperGrad = PMTsDbContext.PaperGrade.Where(w => w.Grade == Item && w.Active == true).FirstOrDefault(); //JsonConvert.DeserializeObject<PaperGrade>(_paperGradeAPIRepository.GetPaperGradeByGrade(_factoryCode, Item));
                                            PaperGrad = grade.Where(w => w.Grade == Item.Trim()).FirstOrDefault();
                                            if (PaperGrad != null)
                                            {
                                                switch (PaperGrad.Group)
                                                {
                                                    case 1:
                                                        GroupItem = rollWidth.Group1 <= GroupItem && rollWidth.Group1 != 0 ? rollWidth.Group1 : GroupItem;
                                                        z = rollWidth.Group1 <= GroupItem && rollWidth.Group1 != 0 ? 1 : z;
                                                        break;
                                                    case 2:
                                                        GroupItem = rollWidth.Group2 <= GroupItem && rollWidth.Group2 != 0 ? rollWidth.Group2 : GroupItem;
                                                        z = rollWidth.Group2 <= GroupItem && rollWidth.Group2 != 0 ? 2 : z;
                                                        break;
                                                    case 3:
                                                        GroupItem = rollWidth.Group3 <= GroupItem && rollWidth.Group3 != 0 ? rollWidth.Group3 : GroupItem;
                                                        z = rollWidth.Group3 <= GroupItem && rollWidth.Group3 != 0 ? 3 : z;
                                                        break;
                                                    case 4:
                                                        GroupItem = rollWidth.Group4 <= GroupItem && rollWidth.Group4 != 0 ? rollWidth.Group4 : GroupItem;
                                                        z = rollWidth.Group4 <= GroupItem && rollWidth.Group4 != 0 ? 4 : z;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    if (sizeWidth <= GroupItem && GroupItem != 10000)
                                    {
                                        sizeWidth = GroupItem;
                                    }
                                    else if (GroupItem != 10000)
                                    {
                                        if (b < RollWidth.Count - 1)
                                        {
                                            sizeWidth = z == 1 ? RollWidth[b + 1].Group1
                                                      : z == 2 ? RollWidth[b + 1].Group2
                                                      : z == 3 ? RollWidth[b + 1].Group3
                                                      : z == 4 ? RollWidth[b + 1].Group4 : RollWidth[b + 1].Width;
                                        }
                                    }
                                }
                                else
                                {
                                    sizeWidth = rollWidth.Width;
                                }
                                break;
                            }
                            b++;
                        }

                        if (ret.PaperRollWidth == null)
                        {
                            ret.PaperRollWidth = sizeWidth.ToString();                                        //Paper Width
                            ret.Cut = k.ToString();                                                                  //จำนวนตัด
                            ret.Trim = (sizeWidth - sheetIn_W * k).ToString();                                //เศษตัดริม
                            ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(sizeWidth) * 100), 2).ToString();           //% Waste
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;

        }

        public RoutingDataModel GetFormulaByCut(string FactoryCode, int Cut, double WigthIn, string Flute, string MaterialNo, string machineName)
        {
            var model = new RoutingDataModel();
            var mintrim = PMTsDbContext.Flute.Where(x => x.FactoryCode == FactoryCode && x.Flute1 == Flute).Select(y => y.Trim).FirstOrDefault();
            var machineFluteTrim = PMTsDbContext.MachineFluteTrim.Where(m => m.FactoryCode == FactoryCode && m.Machine == machineName && m.Flute == Flute).FirstOrDefault();
            int? trimMachine = machineFluteTrim == null ? 0 : machineFluteTrim.Trim;
            int? trimWaste = trimMachine == 0 ? mintrim : trimMachine;
            var sizeWidth = (Cut * WigthIn) + trimWaste;
            //var paperwidth = PMTsDbContext.PaperWidth.Where(x => x.Group2 >= calwidth && x.FactoryCode == FactoryCode).OrderBy(w => w.Group2).Select(z => z.Group2).FirstOrDefault();
            var RollWidth = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == FactoryCode).OrderBy(o => o.Width).ToList();
            var PaperItem = GetPaperItemByMaterialNo(FactoryCode, MaterialNo);
            PaperGrade PaperGrad;
            int? GroupItem = 10000;
            int z = 0;
            var rollWidth = RollWidth.Where(x => x.Width >= sizeWidth).FirstOrDefault();
            //sizeWidth = rollWidth.Width;   //ตุีกตาตัวที่ 1
            if (PaperItem.Count > 0)
            {
                foreach (var Item in PaperItem) //ตุีกตาตัวที่ 2 เทียบกันใน grade แต่ละตัวว่า groupPaperWidth ไหนน้อยสุด
                {
                    if (Item != "")
                    {
                        PaperGrad = PMTsDbContext.PaperGrade.Where(w => w.Grade == Item && w.Active == true).FirstOrDefault(); //JsonConvert.DeserializeObject<PaperGrade>(_paperGradeAPIRepository.GetPaperGradeByGrade(_factoryCode, Item));
                        if (PaperGrad != null)
                        {
                            switch (PaperGrad.Group)
                            {
                                case 1:
                                    GroupItem = rollWidth.Group1 <= GroupItem && rollWidth.Group1 > 0 ? rollWidth.Group1 : GroupItem;
                                    z = rollWidth.Group1 <= GroupItem && rollWidth.Group1 > 0 ? 1 : z;
                                    break;
                                case 2:
                                    GroupItem = rollWidth.Group2 <= GroupItem && rollWidth.Group2 > 0 ? rollWidth.Group2 : GroupItem;
                                    z = rollWidth.Group2 <= GroupItem && rollWidth.Group2 > 0 ? 2 : z;
                                    break;
                                case 3:
                                    GroupItem = rollWidth.Group3 <= GroupItem && rollWidth.Group3 > 0 ? rollWidth.Group3 : GroupItem;
                                    z = rollWidth.Group3 <= GroupItem && rollWidth.Group3 > 0 ? 3 : z;
                                    break;
                                case 4:
                                    GroupItem = rollWidth.Group4 <= GroupItem && rollWidth.Group4 > 0 ? rollWidth.Group4 : GroupItem;
                                    z = rollWidth.Group4 <= GroupItem && rollWidth.Group4 > 0 ? 4 : z;
                                    break;
                            }
                        }
                    }
                }
                //sizeWidth = sizeWidth <= GroupItem ? GroupItem : RollWidth[b+1].Width; 
                if (sizeWidth <= GroupItem)
                {
                    sizeWidth = GroupItem;
                }
                else
                {
                    var rollWidth2 = RollWidth.Where(x => x.Width > rollWidth.Width).FirstOrDefault();
                    if (rollWidth2 != null)
                    {
                        sizeWidth = z == 1 ? rollWidth2.Group1 == 0 ? rollWidth2.Width : rollWidth2.Group1
                             : z == 2 ? rollWidth2.Group2 == null || rollWidth2.Group2 == 0 ? rollWidth2.Width : rollWidth2.Group2
                             : z == 3 ? rollWidth2.Group3 == null || rollWidth2.Group3 == 0 ? rollWidth2.Width : rollWidth2.Group3
                             : z == 4 ? rollWidth2.Group4 == null || rollWidth2.Group4 == 0 ? rollWidth2.Width : rollWidth2.Group4 : rollWidth2.Width;
                    }
                }
            }
            else
            {
                sizeWidth = rollWidth.Width;
            }

            var trim = sizeWidth - (WigthIn * Cut);
            var percenttrim = Math.Round((Convert.ToDouble(trim) / Convert.ToDouble(sizeWidth.ToString())) * 100, 2).ToString();// trim / paperwidth;

            model.PaperRollWidth = sizeWidth.ToString();
            model.Cut = Cut.ToString();
            model.Trim = trim.ToString();
            model.PercentTrim = percenttrim.ToString();

            return model;
        }

        public RoutingDataModel GetFormulaByPaperWidth(string FactoryCode, int PaperWigth, double WigthIn, string Flute, int cut)
        {
            var model = new RoutingDataModel();
            var mintrim = PMTsDbContext.Flute.Where(x => x.FactoryCode == FactoryCode && x.Flute1 == Flute).Select(z => z.Trim).FirstOrDefault();
            cut = (int)Math.Floor(Convert.ToDouble(PaperWigth / WigthIn));
            var trim = PaperWigth - (WigthIn * cut);    //Nut Edit 8 July 2022
            var percenttrim = Math.Round((Convert.ToDouble(trim) / Convert.ToDouble(PaperWigth.ToString())) * 100, 2).ToString();// trim / PaperWigth;

            model.PaperRollWidth = PaperWigth.ToString();
            model.Cut = cut.ToString();
            model.Trim = trim.ToString();
            model.PercentTrim = percenttrim.ToString();

            return model;
        }

        private static int ConvertStringToShort(string Input)
        {
            return string.IsNullOrEmpty(Input) ? 0 : Convert.ToInt16(Input);
        }

        private static Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        public int CalculateMoTargetQuantity(string factoryCode, double Order_Quant, double Tolerance_Over, string flute, string materialNo, int cut)
        {
            try
            {
                int NumberOfOpen = 0;
                bool NoPiece = false;

                int hardShip, piece = 0;

                int noCut = 1;

                int piecePatch = 1;
                string saleUom = "";
                int speTranf = 0;

                /* Set Master Data */
                var machinesName = new List<string>();

                MasterData masterData = PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus != "X");

                List<PmtsConfig> pMTsConfig = [.. PMTsDbContext.PmtsConfig.Where(w => w.FactoryCode.Equals(factoryCode))];

                PmtsConfig custAllowanceConfig = pMTsConfig.Where(x => x.FucName == "Cust_Allowance").FirstOrDefault() ?? new PmtsConfig();
                PmtsConfig sbMatTypeConfig = pMTsConfig.Where(x => x.FucName == "SB_Mat_Type").FirstOrDefault() ?? new PmtsConfig();
                PmtsConfig sbMatDegitConfig = pMTsConfig.Where(x => x.FucName == "SB_Mat_Digit").FirstOrDefault() ?? new PmtsConfig();

                string materialType = materialNo[..Convert.ToInt32(sbMatDegitConfig.FucValue)];
                bool isSBMat = materialType.IndexOf(sbMatTypeConfig.FucValue) > 0;

                List<AllowanceProcess> allowanceProcess = [.. PMTsDbContext.AllowanceProcess.Where(w => w.FactoryCode.Equals(factoryCode))];
                List<Machine> machines = [.. PMTsDbContext.Machine.Where(w => w.FactoryCode.Equals(factoryCode))];

                var flute_data = PMTsDbContext.Flute.FirstOrDefault(w => w.FactoryCode.Equals(factoryCode) && w.Flute1 == flute);

                if (isSBMat)
                {
                    noCut = cut;
                    List<SbRouting> routings = [.. PMTsDbContext.SbRouting.Where(w => w.FactoryCode.Equals(factoryCode))];

                    if (routings != null && routings.Count > 0)
                    {
                        NumberOfOpen = routings.FirstOrDefault().NoOpenOut ?? 0;
                        //Machine = routing.Machine;
                        //CutNo = routing.CutNo ?? 0;

                        machinesName = routings.Select(x => x.Machine).ToList();
                    }

                    NoPiece = false;

                    hardShip = 5;
                    piece = 0;
                }
                else
                {
                    List<Routing> routings = [.. PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode && w.PdisStatus != "X")];

                    if (routings != null && routings.Count > 0)
                    {
                        NumberOfOpen = routings.OrderBy(x => x.SeqNo).FirstOrDefault().NoOpenOut ?? 0;
                        //Machine = routing.Machine;
                        //var _corRouting = routings.Where(x => x.Machine.ToUpper().StartsWith("COR") || (x.SeqNo == 1 && x.CutNo > 0)).FirstOrDefault();
                        //noCut = _corRouting != null ? ((_corRouting.CutNo == 0 ? 1 : _corRouting.CutNo) ?? 0) : 0;

                        machinesName = routings.OrderBy(x => x.SeqNo).Select(x => x.Machine).ToList();

                        foreach (var mach in machinesName)
                        {
                            var thisMachine = machines.Where(x => x.Machine1 == mach).FirstOrDefault();
                            if (thisMachine != null)
                            {
                                if (thisMachine.IsCalPaperwidth == true)
                                {
                                    var _corRouting = routings.Where(x => x.Machine == mach).FirstOrDefault();
                                    noCut = _corRouting != null ? ((_corRouting.CutNo == 0 ? 1 : _corRouting.CutNo) ?? 0) : 0;
                                }
                                speTranf += thisMachine.SpeTranf ?? 0;
                            }
                        }
                    }

                    NoPiece = masterData.TwoPiece ?? false;

                    hardShip = masterData.Hardship ?? 0;
                    piece = masterData.PieceSet ?? 0;

                    piecePatch = masterData.PiecePatch.HasValue && masterData.PiecePatch.Value > 0 ? masterData.PiecePatch.Value : 1;

                    saleUom = masterData.SaleUom;
                }

                var hard_data = PMTsDbContext.AllowanceHard.FirstOrDefault(x => x.FactoryCode.Equals(factoryCode) && x.Hardship == hardShip);

                /* Set Master Data */

                NumberOfOpen = NumberOfOpen == 0 ? 1 : NumberOfOpen;

                double newOrderQuant = 0;

                int quantity = Convert.ToInt32(Order_Quant);

                // add 2021-11-01 PiecePatch
                if (!string.IsNullOrEmpty(saleUom) && saleUom.Equals("PAC", StringComparison.CurrentCultureIgnoreCase))
                {
                    quantity *= piecePatch;
                }

                #region ================== หาจำนวนสั่งผลิต โดยตรวจสอบ 2 ชิ้นต่อ ===========================

                quantity = NoPiece ? quantity * 2 : quantity;
                double _quantity = Math.Round((double)quantity / (double)NumberOfOpen);
                quantity = Convert.ToInt32(_quantity);

                #endregion ========================================================================

                #region ====================== คำนวนจำนวนเผื่อ by material ============================

                int materialAllowance = 0;

                if (!isSBMat)
                {
                    int spareMin = masterData.SpareMin ?? 0,
                        spareMax = masterData.SpareMax ?? 0,
                        sparePercen = masterData.SparePercen ?? 0;

                    if (Convert.ToDouble(sparePercen) > 0)
                    {
                        materialAllowance = Convert.ToInt32(((double)quantity * (double)sparePercen) / (double)100d);

                        if (materialAllowance < spareMin) materialAllowance = spareMin;
                        else if (materialAllowance > spareMax) materialAllowance = spareMax;
                    }
                }

                #endregion =======================================================================

                #region ========================== หาค่า จำนวนเผื่อลูกค้า ================================

                int custAllowance = 0;

                if (Tolerance_Over > 0)
                {
                    if (isSBMat)
                    {
                        custAllowance = Convert.ToInt32(Tolerance_Over);
                    }
                    else
                    {
                        double _custAllowance = Math.Round(((double)quantity * Tolerance_Over) / 100);
                        custAllowance = Convert.ToInt32(_custAllowance);
                    }
                }

                #endregion ========================================================================

                #region ============================= แผ่นเสียต่อตั้ง ===================================

                int waste = 0, stack = 0;

                if (flute_data != null)
                {
                    waste = flute_data.WasteStack ?? 0;
                    stack = flute_data.Stack ?? 0;
                }

                #endregion ========================================================================

                #region =============================== คำนวนหาค่าความยาก ===========================

                int difficult = 0;

                if (hard_data != null)
                {
                    decimal percent = hard_data.Percen;
                    int sheetMin = hard_data.SheetMin,
                        sheetMax = hard_data.SheetMax;

                    difficult = Convert.ToInt32(Convert.ToDecimal(quantity) * percent / 100);

                    if (difficult < 0)
                    {
                        if (Math.Abs(difficult) < sheetMin)
                        {
                            difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMin);
                        }
                        else if (Math.Abs(difficult) > sheetMax)
                        {
                            difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMax);
                        }
                    }
                    else
                    {
                        if (difficult < sheetMin)
                        {
                            difficult = Convert.ToInt32(sheetMin);
                        }
                        else if (difficult > sheetMax)
                        {
                            difficult = Convert.ToInt32(sheetMax);
                        }
                    }
                }

                #endregion ========================================================================

                #region =================================== จำนวนชิ้นต่อชุด ===========================

                int numSet = piece > 0 ? masterData.PieceSet ?? 1 : 1;

                #endregion ========================================================================

                #region ============================== ค่าเผือแต่ละ process =============================

                double machineAllowance = 0;
                //int speTranf = 0;

                foreach (var machine in machinesName)
                {
                    double mpercent = 0, msheetMin = 0, msheetMax = 0;
                    var thisProcess = allowanceProcess.Where(x => x.Machine == machine).ToList();
                    if (thisProcess != null && thisProcess.Count > 0)
                    {
                        var p = thisProcess.Where(x => x.Range >= quantity).OrderBy(x => x.Range).FirstOrDefault();

                        if (p != null)
                        {
                            mpercent = p.Percen;
                            msheetMin = p.SheetMin;
                            msheetMax = p.SheetMax;
                        }
                        else
                        {
                            p = thisProcess.OrderBy(x => x.Range).LastOrDefault();
                            mpercent = p.Percen;
                            msheetMin = p.SheetMin;
                            msheetMax = p.SheetMax;
                        }
                    }

                    double _machineAllowance = 0;
                    _machineAllowance = (quantity * mpercent) / 100;

                    if (_machineAllowance < msheetMin) _machineAllowance = msheetMin;
                    else if (_machineAllowance > msheetMax) _machineAllowance = msheetMax;
                    machineAllowance += _machineAllowance;

                    //var thisMachine = machines.Where(x => x.Machine1 == machine).FirstOrDefault();
                    //if (thisMachine != null)
                    //{
                    //    if(thisMachine.IsCalPaperwidth == true)
                    //    {
                    //        var _corRouting = routings.Where(x => x.Machine == machine).FirstOrDefault();
                    //        noCut = _corRouting != null ? ((_corRouting.CutNo == 0 ? 1 : _corRouting.CutNo) ?? 0) : 0;
                    //    }
                    //    speTranf += thisMachine.SpeTranf ?? 0;
                    //}


                }

                #endregion ========================================================================

                #region ============================= หาแผ่นเสียต่อตั้ง =================================

                int stackWaste = 0;

                if (stack == 0) stack = 1;

                int _p = speTranf / 2;

                if (noCut == 0) noCut = 1;

                int _stackWaste = quantity % noCut;
                stackWaste = (quantity + Convert.ToInt32(machineAllowance) + difficult) / noCut;

                if (_stackWaste > 0)
                {
                    stackWaste += 1;
                }



                if (stackWaste % stack > 0)
                {
                    stackWaste = (stackWaste / stack) + 1;
                }
                else
                {
                    stackWaste /= stack;
                }

                stackWaste *= noCut;

                #endregion ========================================================================

                // ตัวแปรตามโปรแกรมเก่า
                // Q = quantity
                // C = machineAllowance
                // S = stackWaste
                // T = speTranf
                // D = difficult
                // W = waste
                // F = stack
                // M = materialAllowance;
                // A = custAllowance

                if (materialAllowance == 0)
                {
                    switch (int.Parse(custAllowanceConfig.FucValue))
                    {
                        case 1: // Full Customer Allowance
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + custAllowance;
                            break;
                        case 2: // Compare Customer Allowance
                            if (custAllowance > (machineAllowance + (waste * stackWaste * _p) + difficult))
                            {
                                newOrderQuant = quantity + custAllowance;
                            }
                            else
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            }
                            break;
                        case 3: // Half Customer Allowance
                            if (isSBMat)
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + (custAllowance);
                            }
                            else
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + (custAllowance / 2);
                            }
                            break;
                        default:
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            break;
                    }
                }
                else
                {
                    switch (int.Parse(custAllowanceConfig.FucValue))
                    {
                        case 1: // Full Customer Allowance
                            newOrderQuant = quantity + materialAllowance + custAllowance;
                            break;
                        case 2: // Compare Customer Allowance
                            if (custAllowance > materialAllowance)
                            {
                                newOrderQuant = quantity + custAllowance;
                            }
                            else
                            {
                                newOrderQuant = quantity + materialAllowance;
                            }
                            break;
                        case 3: // Half Customer Allowance
                            if (isSBMat)
                            {
                                newOrderQuant = quantity + materialAllowance + (custAllowance);
                            }
                            else
                            {
                                newOrderQuant = quantity + materialAllowance + (custAllowance / 2);
                            }
                            break;
                        default:
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            break;
                    }
                }



                int _newOrderQuant = Convert.ToInt32(newOrderQuant);

                if (newOrderQuant - _newOrderQuant > 0)
                {
                    _newOrderQuant += 1;

                }

                var CQ = _newOrderQuant % noCut;

                if (CQ > 0)
                {

                    quantity = (_newOrderQuant / noCut) + 1;

                    _newOrderQuant = quantity * noCut;


                }

                return _newOrderQuant;
            }
            catch
            {

                return -1;
            }
        }

        public int CalculateTargetQty(string factoryCode, double Order_Quant, double Tolerance_Over, string flute, string orderItem)
        {
            try
            {
                int NumberOfOpen = 0;
                bool NoPiece = false;

                int hardShip, piece = 0;

                int noCut = 1;

                int piecePatch = 1;
                string saleUom = "";
                int speTranf = 0;

                /* Set Master Data */
                List<string> machinesName = new List<string>();

                MoSpec moSpec = PMTsDbContext.MoSpec.FirstOrDefault(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem);

                List<PmtsConfig> pMTsConfig = PMTsDbContext.PmtsConfig.Where(w => w.FactoryCode.Equals(factoryCode)).ToList();

                PmtsConfig custAllowanceConfig = pMTsConfig.Where(x => x.FucName == "Cust_Allowance").FirstOrDefault() ?? new PmtsConfig();
                PmtsConfig sbMatTypeConfig = pMTsConfig.Where(x => x.FucName == "SB_Mat_Type").FirstOrDefault() ?? new PmtsConfig();
                PmtsConfig sbMatDegitConfig = pMTsConfig.Where(x => x.FucName == "SB_Mat_Digit").FirstOrDefault() ?? new PmtsConfig();

                string materialType = moSpec.MaterialNo.Substring(0, Convert.ToInt32(sbMatDegitConfig.FucValue));
                bool isSBMat = materialType.IndexOf(sbMatTypeConfig.FucValue) > 0 ? true : false;

                List<AllowanceProcess> allowanceProcess = PMTsDbContext.AllowanceProcess.Where(w => w.FactoryCode.Equals(factoryCode)).ToList();
                List<Machine> machines = PMTsDbContext.Machine.Where(w => w.FactoryCode.Equals(factoryCode)).ToList();

                var flute_data = PMTsDbContext.Flute.FirstOrDefault(w => w.FactoryCode.Equals(factoryCode) && w.Flute1 == flute);

                if (isSBMat)
                {
                    //noCut = cut;
                    List<MoRouting> routings = PMTsDbContext.MoRouting.Where(w => w.OrderItem.Equals(orderItem) && w.FactoryCode == factoryCode).ToList();

                    if (routings != null && routings.Count > 0)
                    {
                        NumberOfOpen = routings.FirstOrDefault().NoOpenOut ?? 0;
                        machinesName = routings.Select(x => x.Machine).ToList();

                        foreach (var mach in machinesName)
                        {
                            var thisMachine = machines.Where(x => x.Machine1 == mach).FirstOrDefault();
                            if (thisMachine != null)
                            {
                                if (thisMachine.IsCalPaperwidth == true)
                                {
                                    var _corRouting = routings.Where(x => x.Machine == mach).FirstOrDefault();
                                    noCut = _corRouting != null ? ((_corRouting.CutNo == 0 ? 1 : _corRouting.CutNo) ?? 0) : 0;
                                }
                                speTranf += thisMachine.SpeTranf ?? 0;
                            }
                        }
                    }

                    NoPiece = false;

                    hardShip = 5;
                    piece = 0;
                }
                else
                {
                    List<MoRouting> routings = PMTsDbContext.MoRouting.Where(w => w.OrderItem.Equals(orderItem) && w.FactoryCode == factoryCode).ToList();

                    if (routings != null && routings.Count > 0)
                    {
                        NumberOfOpen = routings.OrderBy(x => x.SeqNo).FirstOrDefault().NoOpenOut ?? 0;
                        machinesName = routings.OrderBy(x => x.SeqNo).Select(x => x.Machine).ToList();

                        foreach (var mach in machinesName)
                        {
                            var thisMachine = machines.Where(x => x.Machine1 == mach).FirstOrDefault();
                            if (thisMachine != null)
                            {
                                if (thisMachine.IsCalPaperwidth == true)
                                {
                                    var _corRouting = routings.Where(x => x.Machine == mach).FirstOrDefault();
                                    noCut = _corRouting != null ? ((_corRouting.CutNo == 0 ? 1 : _corRouting.CutNo) ?? 0) : 0;
                                }
                                speTranf += thisMachine.SpeTranf ?? 0;
                            }
                        }
                    }

                    NoPiece = moSpec.TwoPiece ?? false;

                    hardShip = moSpec.Hardship ?? 0;
                    piece = moSpec.PieceSet ?? 0;

                    piecePatch = moSpec.PiecePatch.HasValue && moSpec.PiecePatch.Value > 0 ? moSpec.PiecePatch.Value : 1;

                    saleUom = moSpec.SaleUom;
                }

                var hard_data = PMTsDbContext.AllowanceHard.FirstOrDefault(x => x.FactoryCode.Equals(factoryCode) && x.Hardship == hardShip);

                /* Set Master Data */

                NumberOfOpen = NumberOfOpen == 0 ? 1 : NumberOfOpen;

                double newOrderQuant = 0;

                int quantity = Convert.ToInt32(Order_Quant);

                // add 2021-11-01 PiecePatch
                if (!string.IsNullOrEmpty(saleUom) && saleUom.ToUpper() == "PAC")
                {
                    quantity *= piecePatch;
                }

                #region ================== หาจำนวนสั่งผลิต โดยตรวจสอบ 2 ชิ้นต่อ ===========================

                quantity = NoPiece ? quantity * 2 : quantity;
                double _quantity = Math.Round((double)quantity / (double)NumberOfOpen);
                quantity = Convert.ToInt32(_quantity);

                #endregion ========================================================================

                #region ====================== คำนวนจำนวนเผื่อ by material ============================

                int materialAllowance = 0;

                if (!isSBMat)
                {
                    int spareMin = moSpec.SpareMin ?? 0,
                        spareMax = moSpec.SpareMax ?? 0,
                        sparePercen = moSpec.SparePercen ?? 0;

                    if (Convert.ToDouble(sparePercen) > 0)
                    {
                        materialAllowance = Convert.ToInt32(((double)quantity * (double)sparePercen) / (double)100d);

                        if (materialAllowance < spareMin) materialAllowance = spareMin;
                        else if (materialAllowance > spareMax) materialAllowance = spareMax;
                    }
                }

                #endregion =======================================================================

                #region ========================== หาค่า จำนวนเผื่อลูกค้า ================================

                int custAllowance = 0;

                if (Tolerance_Over > 0)
                {
                    if (isSBMat)
                    {
                        custAllowance = Convert.ToInt32(Tolerance_Over);
                    }
                    else
                    {
                        double _custAllowance = Math.Round(((double)quantity * Tolerance_Over) / 100);
                        custAllowance = Convert.ToInt32(_custAllowance);
                    }
                }

                #endregion ========================================================================

                #region ============================= แผ่นเสียต่อตั้ง ===================================

                int waste = 0, stack = 0;

                if (flute_data != null)
                {
                    waste = flute_data.WasteStack ?? 0;
                    stack = flute_data.Stack ?? 0;
                }

                #endregion ========================================================================

                #region =============================== คำนวนหาค่าความยาก ===========================

                int difficult = 0;

                if (hard_data != null)
                {
                    decimal percent = hard_data.Percen;
                    int sheetMin = hard_data.SheetMin,
                        sheetMax = hard_data.SheetMax;

                    difficult = Convert.ToInt32(Convert.ToDecimal(quantity) * percent / 100);

                    if (difficult < 0)
                    {
                        if (Math.Abs(difficult) < sheetMin)
                        {
                            difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMin);
                        }
                        else if (Math.Abs(difficult) > sheetMax)
                        {
                            difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMax);
                        }
                    }
                    else
                    {
                        if (difficult < sheetMin)
                        {
                            difficult = Convert.ToInt32(sheetMin);
                        }
                        else if (difficult > sheetMax)
                        {
                            difficult = Convert.ToInt32(sheetMax);
                        }
                    }
                }

                #endregion ========================================================================

                #region =================================== จำนวนชิ้นต่อชุด ===========================

                int numSet = piece > 0 ? moSpec.PieceSet ?? 1 : 1;

                #endregion ========================================================================

                #region ============================== ค่าเผือแต่ละ process =============================

                double machineAllowance = 0;
                //int speTranf = 0;

                foreach (var machine in machinesName)
                {
                    double mpercent = 0, msheetMin = 0, msheetMax = 0;
                    var thisProcess = allowanceProcess.Where(x => x.Machine == machine).ToList();
                    if (thisProcess != null && thisProcess.Count > 0)
                    {
                        var p = thisProcess.Where(x => x.Range >= quantity).OrderBy(x => x.Range).FirstOrDefault();

                        if (p != null)
                        {
                            mpercent = p.Percen;
                            msheetMin = p.SheetMin;
                            msheetMax = p.SheetMax;
                        }
                        else
                        {
                            p = thisProcess.OrderBy(x => x.Range).LastOrDefault();
                            mpercent = p.Percen;
                            msheetMin = p.SheetMin;
                            msheetMax = p.SheetMax;
                        }
                    }

                    double _machineAllowance = 0;
                    _machineAllowance = (quantity * mpercent) / 100;

                    if (_machineAllowance < msheetMin) _machineAllowance = msheetMin;
                    else if (_machineAllowance > msheetMax) _machineAllowance = msheetMax;
                    machineAllowance += _machineAllowance;

                }

                #endregion ========================================================================

                #region ============================= หาแผ่นเสียต่อตั้ง =================================

                int stackWaste = 0;

                if (stack == 0) stack = 1;

                int _p = speTranf / 2;

                if (noCut == 0) noCut = 1;

                int _stackWaste = quantity % noCut;
                stackWaste = (quantity + Convert.ToInt32(machineAllowance) + difficult) / noCut;

                if (_stackWaste > 0)
                {
                    stackWaste += 1;
                }



                if (stackWaste % stack > 0)
                {
                    stackWaste = (stackWaste / stack) + 1;
                }
                else
                {
                    stackWaste = (stackWaste / stack);
                }

                stackWaste *= noCut;

                #endregion ========================================================================

                // ตัวแปรตามโปรแกรมเก่า
                // Q = quantity
                // C = machineAllowance
                // S = stackWaste
                // T = speTranf
                // D = difficult
                // W = waste
                // F = stack
                // M = materialAllowance;
                // A = custAllowance

                if (materialAllowance == 0)
                {
                    switch (int.Parse(custAllowanceConfig.FucValue))
                    {
                        case 1: // Full Customer Allowance
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + custAllowance;
                            break;
                        case 2: // Compare Customer Allowance
                            if (custAllowance > (machineAllowance + (waste * stackWaste * _p) + difficult))
                            {
                                newOrderQuant = quantity + custAllowance;
                            }
                            else
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            }
                            break;
                        case 3: // Half Customer Allowance
                            if (isSBMat)
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + (custAllowance);
                            }
                            else
                            {
                                newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult + (custAllowance / 2);
                            }
                            break;
                        default:
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            break;
                    }
                }
                else
                {
                    switch (int.Parse(custAllowanceConfig.FucValue))
                    {
                        case 1: // Full Customer Allowance
                            newOrderQuant = quantity + materialAllowance + custAllowance;
                            break;
                        case 2: // Compare Customer Allowance
                            if (custAllowance > materialAllowance)
                            {
                                newOrderQuant = quantity + custAllowance;
                            }
                            else
                            {
                                newOrderQuant = quantity + materialAllowance;
                            }
                            break;
                        case 3: // Half Customer Allowance
                            if (isSBMat)
                            {
                                newOrderQuant = quantity + materialAllowance + (custAllowance);
                            }
                            else
                            {
                                newOrderQuant = quantity + materialAllowance + (custAllowance / 2);
                            }
                            break;
                        default:
                            newOrderQuant = quantity + machineAllowance + (waste * stackWaste * _p) + difficult;
                            break;
                    }
                }



                int _newOrderQuant = Convert.ToInt32(newOrderQuant);

                if (newOrderQuant - _newOrderQuant > 0)
                {
                    _newOrderQuant += 1;

                }

                var CQ = _newOrderQuant % noCut;

                if (CQ > 0)
                {

                    quantity = (_newOrderQuant / noCut) + 1;

                    _newOrderQuant = quantity * noCut;


                }

                return _newOrderQuant;
            }
            catch (Exception)
            {

                return -1;
            }
        }

        public RSCResultModel GetRSC(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? w = model.Wid;
                int? l = model.Leg;
                int? h = model.Hig;

                var factoryCode = model.PlantCode ?? FactoryCode;
                Flute flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == factoryCode).FirstOrDefault();

                int? a = flu.A;
                int? b = flu.B;
                int? c = flu.C;
                int? d1 = flu.D1;
                int? d2 = flu.D2;
                int? join = flu.JoinSize;
                int? slit = model.Slit;

                int? hbl = 0;
                int? hbr = 0;
                int? hblx = 0;
                int? hbrx = 0;
                int? ll = 0;
                int? wl = 0;
                int? lr = 0;
                int? wr = 0;
                int lid = 0;
                int lid1 = 0;
                int lid2 = 0;
                int? h1 = 0;
                int? shWid = 0;
                int? shLen = 0;

                int? sheet = 0;
                int? box = model.BoxArea;
                int slot = 0;
                double basicw = model.Weight;

                if (model.JoinSize != null && model.JoinSize != 0)
                    join = model.JoinSize;
                if (slit == null || slit < 0 || (model.spcLen == 0 && slit != 0))
                    slit = Convert.ToInt32(PMTsDbContext.PmtsConfig.Where(config => config.FactoryCode == FactoryCode && config.FucName == "Slit").FirstOrDefault().FucValue);

                if (model.rscStyle == "Full Overlap")
                    lid = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w) + c));
                else
                {
                    lid = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w / 2) + c));
                }

                if (model.rscStyle.Contains("Tele"))
                    h1 = h + d2;
                else
                    h1 = h + d1;

                if (model.TwoPiece == true)
                {
                    hbl = l + a + join;
                    hbr = w + b + slit;
                    hblx = l + a + w + b;
                    hbrx = 0;
                    ll = l + a;
                    wl = w + b;
                    lr = 0;
                    wr = 0;

                    shWid = 2 * lid + h1;
                    shLen = l + w + a + b + join + slit;
                }
                else
                {
                    hbl = w + l + (2 * a) + join;
                    hbr = w + l + a + b + slit;
                    hblx = w + l + (2 * a);
                    hbrx = w + l + a + b;
                    ll = l + a;
                    wl = w + a;
                    lr = l + a;
                    wr = w + b;

                    shWid = 2 * lid + h1;
                    shLen = (2 * l) + (2 * w) + (3 * a) + b + join + slit;
                }

                if (model.spcLen >= shLen - slit)
                {
                    slit = model.spcLen - (shLen - slit);
                    hbr = hbrx + slit;
                    shLen = model.spcLen;
                }

                if (model.rscStyle == "Tele Top Lid")
                {
                    lid1 = lid;
                    lid2 = 0;
                    shWid -= lid;
                }
                else if (model.rscStyle == "Tele Bottom Lid")
                {
                    lid1 = 0;
                    lid2 = lid;
                    shWid -= lid;
                }
                else if (model.rscStyle == "Sleeve")
                {
                    lid1 = 0;
                    lid2 = 0;
                    shWid = h1;
                    //h1 = 0;
                }
                else
                {
                    lid1 = lid;
                    lid2 = lid;
                }

                sheet = shWid * shLen;
                slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));

                if (model.Flag != 1)
                    box = sheet - slot;

                rsc.ScoreW1 = lid1;
                rsc.Scorew2 = h1;
                //rsc.Scorew2 = model.rscStyle == "Sleeve" ? 0 : h1;
                rsc.Scorew3 = lid2;
                rsc.JointLap = join;
                rsc.ScoreL2 = ll;
                rsc.ScoreL3 = wl;
                rsc.ScoreL4 = lr;
                rsc.ScoreL5 = wr;
                rsc.ScoreL6 = hblx;
                rsc.ScoreL7 = hbrx;
                rsc.ScoreL8 = hbl;
                rsc.ScoreL9 = hbr;
                rsc.Slit = slit;
                rsc.CutSheetLeng = shLen;
                rsc.CutSheetWid = shWid;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetRSC1Piece(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? w = model.Wid;
                int? l = model.Leg;
                int? h = model.Hig;

                var factoryCode = model.PlantCode ?? FactoryCode;
                Flute flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == factoryCode).FirstOrDefault();

                int? a = flu.A;
                int? b = flu.B;
                int? c = flu.C;
                int? d1 = flu.D1;
                int? d2 = flu.D2;
                int? join = flu.JoinSize;
                int? slit = model.Slit;

                int? hbl = 0;
                int? hbr = 0;
                int? hblx = 0;
                int? hbrx = 0;
                int? ll = 0;
                int? wl = 0;
                int? lr = 0;
                int? wr = 0;
                int? h1 = 0;
                int? shWid = model.CutSheetWid;
                int? shLen = 0;

                int? sheet = 0;
                int? box = 0;
                int slot = 0;
                double basicw = model.Weight;

                if (model.JoinSize != null && model.JoinSize != 0)
                    join = model.JoinSize;

                if (model.rscStyle.Contains("Tele"))
                    h1 = h + d2;
                else
                    h1 = h + d1;

                hbl = w + l + (2 * a) + join;
                hbr = w + l + a + b + slit;
                hblx = w + l + (2 * a);
                hbrx = w + l + a + b;
                ll = l + a;
                wl = w + a;
                lr = l + a;
                wr = w + b;

                shLen = (2 * l) + (2 * w) + (3 * a) + b + join + slit;

                if (model.spcLen >= shLen - slit)
                {
                    slit = model.spcLen - (shLen - slit);
                    hbr = hbrx + slit;
                    shLen = model.spcLen;
                }

                sheet = shWid * shLen;
                slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));

                //if (model.Flag != 1)
                box = sheet - slot;

                rsc.ScoreL2 = ll;
                rsc.ScoreL3 = wl;
                rsc.ScoreL4 = lr;
                rsc.ScoreL5 = wr;
                rsc.ScoreL6 = hblx;
                rsc.ScoreL7 = hbrx;
                rsc.ScoreL8 = hbl;
                rsc.ScoreL9 = hbr;
                rsc.Slit = slit;
                rsc.CutSheetLeng = shLen;

                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetRSC2Piece(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? w = model.Wid;
                int? l = model.Leg;
                int? h = model.Hig;

                var factoryCode = model.PlantCode ?? FactoryCode;
                Flute flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == factoryCode).FirstOrDefault();

                int? a = flu.A;
                int? b = flu.B;
                int? c = flu.C;
                int? d1 = flu.D1;
                int? d2 = flu.D2;
                int? join = flu.JoinSize;
                int? slit = model.Slit;

                int? hbl = 0;
                int? hbr = 0;
                int? hblx = 0;
                int? hbrx = 0;
                int? ll = 0;
                int? wl = 0;
                int? lr = 0;
                int? wr = 0;
                int? lid1 = model.ScoreW1 == null ? 0 : model.ScoreW1;
                int? lid2 = model.Scorew3 == null ? 0 : model.Scorew3;
                int? h1 = model.Scorew2 == null ? 0 : model.Scorew2;
                int? shWid = 0;
                int? shLen = 0;

                int? sheet = 0;
                int? box = 0;
                int slot = 0;

                double basicw = model.Weight;

                if (model.JoinSize != 0)
                    join = model.JoinSize;

                if (lid1 == 0 && h1 == 0 && lid2 == 0)
                {
                    if (model.rscStyle == "Full Overlap")
                        lid1 = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w) + c));
                    else
                        lid1 = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w / 2) + c));

                    lid2 = lid1;
                }

                if (model.rscStyle.Contains("Tele"))
                    h1 = h + d2;
                else
                    h1 = h1 == 0 ? h + d1 : h1;

                if (model.TwoPiece == true)
                {
                    if (model.GLWid == true)
                    {
                        ll = w + a;
                        wl = l + b;
                    }
                    else
                    {
                        ll = model.ScoreL2 != 0 && model.ScoreL2 == model.ScoreL4 ? model.ScoreL2 : l + a;
                        wl = model.ScoreL3 != 0 && model.ScoreL3 == model.ScoreL5 ? model.ScoreL3 : w + b;
                        //ll = l + a;
                        //wl = w + b;
                    }

                    lr = 0;
                    wr = 0;

                    hblx = ll + wl;
                    hbrx = 0;

                    hbl = ll + join;
                    hbr = wl + slit;

                    shWid = lid1 + lid2 + h1;
                    shLen = ll + wl + join + slit;
                }

                sheet = shWid * shLen;
                slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));
                box = sheet - slot;

                rsc.ScoreW1 = lid1;
                rsc.Scorew2 = h1;
                rsc.Scorew3 = lid2;
                rsc.JointLap = join;
                rsc.ScoreL2 = ll;
                rsc.ScoreL3 = wl;
                rsc.ScoreL4 = lr;
                rsc.ScoreL5 = wr;
                rsc.ScoreL6 = hblx;
                rsc.ScoreL7 = hbrx;
                rsc.ScoreL8 = hbl;
                rsc.ScoreL9 = hbr;
                rsc.Slit = slit;
                rsc.CutSheetLeng = shLen;
                rsc.CutSheetWid = shWid;

                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetDC(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                //int? slit = 0;
                //int join = 0;

                //int? h1 = 0;
                int? shWid = model.CutSheetWid;
                int? shLen = model.CutSheetLeng;

                int? sheet = 0;
                int? sheetx = model.SheetArea;
                int? box = model.BoxArea;
                //int? slot = 0;
                double basicw = model.Weight;

                //slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));
                sheet = shWid * shLen;
                box = box == null || box == 0 || box == sheetx ? sheet : box;

                //sheetw = (basicw * sheet / 1000000000);
                //boxw = (basicw * box / 1000000000);

                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetSF(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? shWid = model.CutSheetWid;
                int? shLen = model.CutSheetLeng;

                int? sheet = 0;
                int? sheetx = model.SheetArea;
                int? box = model.BoxArea;

                double basicw = model.Weight;
                var kgLen = model.Flag == 1 ? shLen : Convert.ToInt32(1000000000 / (basicw * shWid));

                sheet = shWid * kgLen;
                //box = sheet;
                box = box == null || box == 0 || box == sheetx ? sheet : box;

                rsc.CutSheetLeng = kgLen;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetHC(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? sheet = 0;
                int? box = 0;
                double? sheetw = 0;
                double? boxw = 0;

                var factoryCode = model.PlantCode ?? FactoryCode;
                Flute flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == factoryCode).FirstOrDefault();

                int? join = flu.JoinSize;
                int? height = flu.Height;
                int? rangeGL = flu.B;
                double? wheelGL = flu.C == 3 ? 3.5 : Convert.ToDouble(flu.C);

                int? shWid = model.CutSheetWid;
                int? shLen = model.CutSheetLeng;

                var _boardCombineRepository = new BoardCombineRepository(PMTsDbContext);
                List<BoardSpecWeight> board = _boardCombineRepository.GetBoardSpecWeightByCode(factoryCode, model.Code);

                var grade = board[0].PaperDes;

                if (grade == "--000")
                    grade = board[2].PaperDes;

                HoneyPaper paper = PMTsDbContext.HoneyPaper.Where(h => h.Grade == grade).FirstOrDefault();

                var E = 73.63;
                var So = paper.SolidContent; //0.65;
                var A = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(shWid) / (rangeGL + wheelGL))) - 2);   //จำนวนจุดทากาวแต่ละหน้ากว้าง เศษปัดลง

                var kgPPm = (Convert.ToDouble(paper.Weight) / 1000) * (Convert.ToDouble(shWid) / 1000) * (Convert.ToDouble(height) / 1000) * paper.PaperAmt;
                var Weight_DryGL2 = Convert.ToDouble(E * wheelGL * height * A * paper.PaperAmt / 1000000000) / So;

                sheetw = kgPPm + Weight_DryGL2;
                boxw = kgPPm + (Weight_DryGL2 * So);

                shLen = Convert.ToInt32(Convert.ToDouble(1000) / boxw);
                //boxw = 1;
                //sheetw = 1;

                sheet = shWid * shLen;
                box = sheet;

                rsc.CutSheetLeng = shLen;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = 1;
                rsc.WeightBox = 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetHB(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? sheet = 0;
                int? box = 0;
                double? sheetw = 0;
                double? boxw = 0;

                var factoryCode = model.PlantCode ?? FactoryCode;
                Flute flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == factoryCode).FirstOrDefault();

                int? join = flu.JoinSize;
                int? height = flu.Height;
                int? rangeGL = flu.B;
                double? wheelGL = flu.C == 3 ? 3.5 : Convert.ToDouble(flu.C);

                int? shWid = model.CutSheetWid;
                int? shLen = model.CutSheetLeng;

                var _boardCombineRepository = new BoardCombineRepository(PMTsDbContext);
                List<BoardSpecWeight> board = _boardCombineRepository.GetBoardSpecWeightByCode(factoryCode, model.Code);

                var grade = board[0].PaperDes;

                if (grade == "--000")
                    grade = board[2].PaperDes;

                HoneyPaper paper = PMTsDbContext.HoneyPaper.Where(h => h.Grade == grade).FirstOrDefault();

                var E = 73.63;
                var So = paper.SolidContent; //0.65;
                var percentGL3 = 0.1173;
                //var A = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(shWid) / (rangeGL + wheelGL))) - 2);   //จำนวนจุดทากาวแต่ละหน้ากว้าง เศษปัดลง

                var Wx = Convert.ToInt32(shWid / paper.Shrink);
                var Lx = Convert.ToInt32(Convert.ToDouble(shLen) / paper.Stretch);
                var Ax = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(Wx) / (rangeGL + wheelGL))) - 2);

                if (board[4].PaperDes == "--000")
                    board[4].PaperDes = board[3].PaperDes;

                var B1 = board[1].PaperDes.Substring(board[1].PaperDes.Length - 3, 3);
                var B4 = board[4].PaperDes.Substring(board[4].PaperDes.Length - 3, 3);
                var PPLx = paper.PaperAmt * (Convert.ToDouble(Lx) / 1000);

                var Weight_HCore = (paper.Weight * Wx * height * PPLx) / 1000000;
                var Weight_DryGL2 = (E * (Convert.ToDouble(wheelGL) / 1000) * (Convert.ToDouble(height) / 1000) * Ax * PPLx);
                var Weight_WetGL2 = Weight_DryGL2 / So;

                var TopSheet = (Convert.ToInt32(B1) + Convert.ToInt32(B4)) * (Convert.ToDouble(shWid * shLen) / 1000000);
                var ppWeight = Weight_HCore + Weight_WetGL2 + TopSheet;
                var DryGL3 = Convert.ToDouble(percentGL3 * ppWeight) / 1000;
                var Weight_DryGL3 = DryGL3 * Wx * Lx / 1000;
                var Weight_WetGL3 = Weight_DryGL3 / So;

                sheetw = (Weight_HCore + TopSheet + Weight_WetGL2 + Weight_WetGL3) / 1000;
                boxw = (Weight_HCore + TopSheet + Weight_DryGL2 + Weight_DryGL3) / 1000;

                sheet = shWid * shLen;
                box = sheet;

                rsc.shrink = paper.Shrink;
                rsc.stretch = paper.Stretch;
                rsc.widHC = Wx;
                rsc.lenHC = Lx;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = sheetw;
                rsc.WeightBox = boxw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetCG(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                model.SideA = model.SideA == null ? 0 : model.SideA;
                model.SideC = model.SideC == null ? 0 : model.SideC;
                int? shWid = model.SideA + model.SideB + model.SideC;       //A+B+C
                int? shLen = model.CutSheetLeng;                            //L

                int? sheet = 0;
                int? box = 0;
                //int? slot = 0;
                double? sheetw = 0;
                double? boxw = 0;
                double basicw = model.Weight;
                int? paperGram4Layer = 0;
                int? paperLayer = 0;
                //int wrapLayer = 1;

                var factoryCode = model.PlantCode ?? FactoryCode;

                var _boardCombineRepository = new BoardCombineRepository(PMTsDbContext);
                List<BoardSpecWeight> board = _boardCombineRepository.GetBoardSpecWeightByCode(factoryCode, model.Code);

                foreach (var item in board)
                {
                    if (item.BasicWeight != 1)
                    {
                        paperGram4Layer += item.BasicWeight;
                        paperLayer += item.Layer;
                    }
                    else
                    {
                        break;
                    }
                }

                var patchArea = shWid - (model.SideD * 2);            //(A+B+C) - (D*2)
                var wrapGram = board.Last().BasicWeight * board.Last().Layer;   //E*e
                var z = (Convert.ToDouble(paperGram4Layer + wrapGram) / (paperLayer + board.Last().Layer * 2 - 1) * (11.38 / 100)) / 1000;
                sheet = shWid * shLen;
                box = sheet;

                double? paperWeight = 0;
                double? glDryWeight = 0;

                if (model.IsWrap == "Wrap")
                {
                    paperWeight = (Convert.ToDouble(paperGram4Layer) / 1000 * patchArea * shLen / 1000000) + (Convert.ToDouble(wrapGram) / 1000 * ((patchArea + model.SideD / 0.707) * 2 + 10) * shLen / 1000000);      //L-U หุ้มรอบ
                    glDryWeight = ((Convert.ToDouble((paperLayer - 1) * patchArea * shLen) / 1000000) + (board.Last().Layer * ((patchArea + model.SideD / 0.707) * 2 + 10) * shLen / 1000000)) * z;   //L-U หุ้มรอบ
                }
                else
                {
                    paperWeight = (Convert.ToDouble(paperGram4Layer) / 1000 * patchArea * shLen / 1000000) + (Convert.ToDouble(wrapGram) / 1000 * (patchArea * 2) * shLen / 1000000);                        //L-U หุ้ม 2 ด้าน
                    glDryWeight = ((Convert.ToDouble((paperLayer - 1) * patchArea * shLen) / 1000000) + (board.Last().Layer * (patchArea * 2) * shLen / 1000000)) * z;                    //L-U หุ้ม 2 ด้าน
                }

                var glWetWeight = glDryWeight / 0.65;
                double degree = Convert.ToDouble(model.NotchDegree * 3.1416) / 360;

                int? mmNotchHig = 0;
                if (model.NotchSide == "A")
                    mmNotchHig = model.SideA;
                else if (model.NotchSide == "B")
                    mmNotchHig = model.SideB;
                else
                    mmNotchHig = model.SideB;

                int areaA = 0, areaC = 0;

                if (model.IsNotch == true)
                {
                    if (model.CGType == "L")
                    {
                        //model.NotchArea = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(mmNotchHig - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
                        model.NotchArea = Convert.ToInt32(Math.Pow(Convert.ToDouble(mmNotchHig - model.SideD - 2), 2) * Math.Tan(degree));
                    }
                    else if (model.CGType == "U")
                    {
                        //areaA = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(model.SideA - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
                        //areaC = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(model.SideC - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
                        areaA = Convert.ToInt32(Math.Pow(Convert.ToDouble(model.SideA - model.SideD - 2), 2) * Math.Tan(degree));
                        areaC = Convert.ToInt32(Math.Pow(Convert.ToDouble(model.SideC - model.SideD - 2), 2) * Math.Tan(degree));
                        model.NotchArea = Convert.ToInt32(areaA + areaC);
                    }
                }

                var ratio = Convert.ToDouble(model.NotchArea * model.No_Slot) / sheet;

                sheetw = glWetWeight + paperWeight;
                boxw = glDryWeight + paperWeight;

                if (model.IsNotch == true)
                {
                    //sheet = sheet - (model.NotchArea * model.No_Slot);
                    box -= (model.NotchArea * model.No_Slot);      //TypeU No_Slot = 1 แปลว่า 1 คู่
                                                                   //sheetw = sheetw - (sheetw * ratio);
                    boxw -= (boxw * ratio);
                }

                rsc.NotchArea = model.NotchArea;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = sheetw;
                rsc.WeightBox = boxw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public RSCResultModel GetAC(string FactoryCode, RSCModel model)
        {
            var rsc = new RSCResultModel();

            try
            {
                int? slit = model.Slit;
                int? n = model.No_Slot;

                int? hbl = 0;
                int? hbr = 0;
                int? h1 = 0;
                int? shWid = model.CutSheetWid;
                int? shLen = model.CutSheetLeng;

                int? sheet = 0;
                int? box = 0;
                int? slot = 0;
                double basicw = model.Weight;


                h1 = model.Scorew16 != null ? model.Scorew16 : shWid / 2;
                hbl = model.ScoreL8 != null ? model.ScoreL8 : shLen / (n + 1);
                hbr = model.ScoreL9 != null ? model.ScoreL9 : hbl;
                sheet = shWid * shLen;
                slot = h1 * slit * n;
                box = (model.BoxArea == null || model.BoxArea == 0) && model.Wid != null && model.Leg != null ? model.Wid * model.Leg - slot : model.BoxArea;
                //box = sheet - slot;

                rsc.Scorew16 = h1;
                rsc.ScoreL8 = hbl;
                rsc.ScoreL9 = hbr;
                rsc.SheetArea = sheet;
                rsc.BoxArea = box;
                rsc.WeightSh = (basicw * sheet / 1000000000);
                rsc.WeightBox = (basicw * box / 1000000000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rsc;
        }

        public ChangeReCalculateTrimModel GetReCalculateTrim(IConfiguration configuration, string factoryCode, string flute, string machine, string boxType, string printMethod, string proType)
        {
            var result = new ChangeReCalculateTrimModel
            {
                DataTable = new DataTable(),
                Flute = flute,
                ReCalculateTrimModels = [],
                Routings = []
            };

            try
            {
                var reCalculateTrimModels = new List<ReCalculateTrimModel>();
                var routings = new List<Routing>();
                string condition = boxType == null ? "" : boxType == "SO" ? " and substring(Hierarchy, 3, 2) = 'SO'" : " and substring(Hierarchy, 3, 2) <> 'SO'";
                condition = printMethod == null ? condition : printMethod == "Other" ? condition + " and Print_Method not in ('Solid 1 สี','Solid 2 สี')" : condition + " and Print_Method like '%" + printMethod + "%'";
                condition = proType == null ? condition : proType == "Other" ? condition + " and Pro_Type not like 'Cor%' " : condition + " and Pro_Type like 'Cor%' ";

                using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                if (db.State == ConnectionState.Closed)
                    db.Open();

                //Execute sql query //m.Material_No as MasterData, 
                #region sql query string 
                string sql = @"   
                            select m.FactoryCode,
	                            m.Flute, 
	                            f.Trim as TrimOfFlute, 
	                            m.Board, 
	                            b.GroupPaperWidth, 
	                            m.CutSheetWid, 
	                            c.Mintrim as MinTrim, 
	                            c.Cut_off as CutOff, 
	                            c.Min_Out pageMin,
	                            r.Id ,
	                            r.FactoryCode ,
	                            r.Seq_No as SeqNo ,
	                            r.Plant ,
	                            r.Material_No as MaterialNo ,
	                            r.Mat_Code as MatCode ,
	                            r.Plan_Code as PlanCode ,
	                            r.Machine ,
	                            r.Alternative1 ,
	                            r.Alternative2 ,
	                            r.Std_Process as StdProcess ,
	                            r.Speed ,
	                            r.Colour_Count as ColourCount ,
	                            r.MC_Move as McMove ,
	                            r.HandHold ,
	                            r.Plate_No as PlateNo ,
	                            r.Myla_No as MylaNo ,
	                            r.Paper_Width as PaperWidth ,
	                            r.Cut_No as CutNo ,
	                            r.Trim ,
	                            r.PercenTrim ,
	                            r.Waste_Leg as WasteLeg ,
	                            r.Waste_Wid as WasteWid ,
	                            r.Sheet_in_Leg as SheetInLeg ,
	                            r.Sheet_in_Wid as SheetInWid ,
	                            r.Sheet_out_Leg as SheetOutLeg ,
	                            r.Sheet_out_Wid as SheetOutWid ,
	                            r.Weight_in as WeightIn ,
	                            r.Weight_out as WeightOut ,
	                            r.No_Open_in as NoOpenIn ,
	                            r.No_Open_out as NoOpenOut ,
	                            r.Color1 ,
	                            r.Shade1 ,
	                            r.Color2 ,
	                            r.Shade2 ,
	                            r.Color3 ,
	                            r.Shade3 ,
	                            r.Color4 ,
	                            r.Shade4 ,
	                            r.Color5 ,
	                            r.Shade5 ,
	                            r.Color6 ,
	                            r.Shade6 ,
	                            r.Color7 ,
	                            r.Shade7 ,
	                            r.Color8 ,
	                            r.Shade8 ,
	                            r.Color9 ,
	                            r.Shade9 ,
	                            r.Color10 ,
	                            r.Shade10 ,
	                            r.Color_Area1  as ColorArea1 ,
	                            r.Color_Area2  as ColorArea2 ,
	                            r.Color_Area3  as ColorArea3 ,
	                            r.Color_Area4  as ColorArea4 ,
	                            r.Color_Area5  as ColorArea5 ,
	                            r.Color_Area6  as ColorArea6 ,
	                            r.Color_Area7  as ColorArea7 ,
	                            r.Platen ,
	                            r.Rotary ,
	                            r.TearTape ,
	                            r.None_Blk as NoneBlk ,
	                            r.Stan_Blk as StanBlk ,
	                            r.Semi_Blk as SemiBlk ,
	                            r.Ship_Blk as ShipBlk ,
	                            r.Block_No as BlockNo ,
	                            r.Join_Mat_no as JoinMatNo ,
	                            r.Separat_Mat_no as SeparatMatNo ,
	                            r.Remark_Inprocess as RemarkInprocess ,
	                            r.Hardship ,
	                            r.PDIS_Status as PdisStatus ,
	                            r.Tran_Status as TranStatus ,
	                            r.SAP_Status as SapStatus ,
	                            r.Alternative3 ,
	                            r.Alternative4 ,
	                            r.Alternative5 ,
	                            r.Alternative6 ,
	                            r.Alternative7 ,
	                            r.Alternative8 ,
	                            r.Rotate_In as RotateIn ,
	                            r.Rotate_Out as RotateOut ,
	                            r.Stack_Height as StackHeight ,
	                            r.Setup_tm as SetupTm ,
	                            r.Setup_waste as SetupWaste ,
	                            r.Prepare_tm as PrepareTm ,
	                            r.Post_tm as PostTm ,
	                            r.Run_waste as RunWaste ,
	                            r.Human as Human ,
	                            r.Color_count as ColorCount ,
	                            r.UnUpgrad_Board as UnUpgradBoard ,
	                            r.Score_type as ScoreType ,
	                            r.Score_Gap as ScoreGap ,
	                            r.Coating ,
	                            r.BlockNo2 ,
	                            r.BlockNoPlant2 ,
	                            r.BlockNo3 ,
	                            r.BlockNoPlant3 ,
	                            r.BlockNo4 ,
	                            r.BlockNoPlant4 ,
	                            r.BlockNo5 ,
	                            r.BlockNoPlant5 ,
	                            r.PlateNo2 ,
	                            r.PlateNoPlant2 ,
	                            r.MylaNo2 ,
	                            r.MylaNoPlant2 ,
	                            r.PlateNo3 ,
	                            r.PlateNoPlant3 ,
	                            r.MylaNo3 ,
	                            r.MylaNoPlant3 ,
	                            r.PlateNo4 ,
	                            r.PlateNoPlant4 ,
	                            r.MylaNo4 ,
	                            r.MylaNoPlant4 ,
	                            r.PlateNo5 ,
	                            r.PlateNoPlant5 ,
	                            r.MylaNo5 ,
	                            r.MylaNoPlant5 ,
	                            r.TearTapeQty ,
	                            r.TearTapeDistance ,
	                            r.MylaSize ,
	                            r.RepeatLength ,
	                            r.CustBarcodeNo ,
	                            r.ControllerCode ,
	                            r.PlanProgramCode ,
	                            r.CreatedDate ,
	                            r.CreatedBy ,
	                            r.UpdatedDate ,
	                            r.UpdatedBy ,
                            case 
	                            when b.GroupPaperWidth = 1 then pp.a1
	                            when b.GroupPaperWidth = 2 then pp.a2
	                            when b.GroupPaperWidth = 3 then pp.a3
	                            when b.GroupPaperWidth = 4 then pp.a4 end PageMinTrim ,
                            case 
	                            when b.GroupPaperWidth = 1 then p.a1
	                            when b.GroupPaperWidth = 2 then p.a2
	                            when b.GroupPaperWidth = 3 then p.a3
	                            when b.GroupPaperWidth = 4 then p.a4 end PageMax
                            from (
	                            SELECT  FactoryCode, Material_No, Flute, Code, Board, CutSheetWid, Box_Type, PDIS_Status
	                            FROM  MasterData
	                            WHERE FactoryCode = '{0}' and Flute = '{1}' and PDIS_Status <> 'X' " + condition + @") m 
	                            left outer join (select * from Routing where FactoryCode = '{2}' and Machine = '{3}') r 
	                            on r.Material_No = m.Material_No and r.FactoryCode = m.FactoryCode 
	                            left outer join Cor_Config c on c.Name = r.Machine and c.FactoryCode = r.FactoryCode
	                            left outer join Flute f on f.Flute = m.Flute and f.FactoryCode = m.FactoryCode
	                            left outer join Board_Combine b on b.Code = m.Code
	                            left outer join (select FactoryCode, Max(Group1) a1, Max(Group2) a2, Max(Group3) a3, Max(Group4) a4 
		                            from PaperWidth group by FactoryCode) p on p.FactoryCode = m.FactoryCode
	                            left outer join (select FactoryCode, Min(Group1) a1, Min(Group2) a2, Min(Group3) a3, Min(Group4) a4 
		                            from PaperWidth group by FactoryCode) pp on pp.FactoryCode = m.FactoryCode
	                            where r.Seq_No is not null and c.Name is not null
	                            order by c.Mintrim
                        ";

                #endregion

                string message = string.Format(sql, factoryCode, flute, factoryCode, machine);
                reCalculateTrimModels = db.Query<ReCalculateTrimModel>(message).ToList();
                result.ReCalculateTrimModels.AddRange(reCalculateTrimModels);
                result.ReCalculateTrimModels = [.. result.ReCalculateTrimModels.OrderBy(r => r.Id)];

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ReCalculateUpdateRoutings(IConfiguration configuration, List<Routing> routings)
        {
            //using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        PMTsDbContext.Routing.UpdateRange(routings);
            //        PMTsDbContext.SaveChanges();
            //        dbContextTransaction.Commit();

            //    }
            //    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            //    {
            //        dbContextTransaction.Rollback();
            //        throw ex;
            //    }
            //}

            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            using IDbTransaction transactionScope = db.BeginTransaction(IsolationLevel.Serializable);
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
                db.Execute(updateQuery, routings, transactionScope);
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public List<ReturnCalPaperWidth> CalculateListRouting(List<ParamCalPaperWidth> model, string FactoryCode)
        {
            try
            {
                var ListPaperWidth = new List<ReturnCalPaperWidth>();
                CorConfig corConfig = PMTsDbContext.CorConfig.Where(w => w.FactoryCode == FactoryCode && w.Name == model[0].Machine).FirstOrDefault();// JsonConvert.DeserializeObject<CorConfig>(_corConfigAPIRepository.GetCorConfigByFactoryCode(_factoryCode, machineName));
                PmtsConfig pmtsConfig = PMTsDbContext.PmtsConfig.Where(p => p.FactoryCode == FactoryCode && p.FucName == "Mintrim").FirstOrDefault();
                corConfig ??= PMTsDbContext.CorConfig.Where(w => w.FactoryCode == FactoryCode && w.Name == "ELSE").FirstOrDefault();
                Flute flute = PMTsDbContext.Flute.Where(f => f.Flute1 == model[0].Flute && f.FactoryCode == FactoryCode).FirstOrDefault();//JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, tran.modelProductSpec.Flute));
                List<PaperWidth> RollWidth = [.. PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == FactoryCode).OrderBy(o => o.Width)]; //   JsonConvert.DeserializeObject<List<PaperWidth>>(_paperWidthAPIRepository.GetPaperWidthList(_factoryCode)).OrderBy(o => o.Group2).ToList();
                List<PaperGrade> Grade = [.. PMTsDbContext.PaperGrade.Where(g => g.Active == true)];
                var machineFluteTrims = PMTsDbContext.MachineFluteTrim.Where(m => model.Select(s => s.FactoryCode).ToList().Contains(m.FactoryCode) && model.Select(s => s.Machine).ToList().Contains(m.Machine) && model.Select(s => s.Flute).ToList().Contains(m.Flute)).ToList();
                var boardAlts = PMTsDbContext.BoardAlternative.Where(b => model.Select(s => s.MaterialNo).ToList().Contains(b.MaterialNo) && model.Select(s => s.FactoryCode).ToList().Contains(b.FactoryCode)).ToList();
                var boardUses = PMTsDbContext.BoardUse.Where(b => model.Select(s => s.MaterialNo).ToList().Contains(b.MaterialNo) && model.Select(s => s.FactoryCode).ToList().Contains(b.FactoryCode)).ToList();

                foreach (var i in model)
                {
                    var machineFluteTrim = machineFluteTrims.Where(w => w.FactoryCode == i.FactoryCode && w.Machine == i.Machine && w.Flute == i.Flute).FirstOrDefault();
                    var boardAlt = boardAlts.Where(w => w.MaterialNo == i.MaterialNo && w.FactoryCode == i.FactoryCode).FirstOrDefault();
                    var boardUse = boardUses.Where(w => w.MaterialNo == i.MaterialNo && w.FactoryCode == i.FactoryCode).FirstOrDefault();

                    var calPaperWidth = CalculateRouting(i.Machine, i.FactoryCode, i.Flute, i.SheetInWid, i.MaterialNo, null, i.TrimOfFlute, 0, corConfig, pmtsConfig, flute, RollWidth, Grade, machineFluteTrim, boardAlt, boardUse, true);
                    ListPaperWidth.Add(new ReturnCalPaperWidth
                    {
                        MaterialNo = i.MaterialNo,
                        MachineName = i.Machine,
                        SheetInWid = i.SheetInWid,
                        Flute = i.Flute,
                        PaperWidthOld = i.PaperWidth,
                        CutOld = i.CutNo,
                        TrimOld = i.Trim,
                        PercentTrimOld = i.PercenTrim,
                        PaperWidth = calPaperWidth.PaperRollWidth,
                        Cut = calPaperWidth.Cut,
                        Trim = calPaperWidth.Trim,
                        PercentTrim = calPaperWidth.PercentTrim
                    });
                }

                return ListPaperWidth;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region ReCalculateTrim Old version
        public List<ReCalculateTrimModel> ReCalculateTrim(IConfiguration configuration, string factoryCode, string flute, string _username, string action, ref DataTable dataTable)
        {
            try
            {
                var reCalculateTrimModels = new List<ReCalculateTrimModel>();
                var routings = new List<Routing>();
                dataTable = new DataTable();

                dataTable.Columns.AddRange(new DataColumn[] {
                    new ("FactoryCode"),
                    new("MaterialNo"),
                    new ("CutSheetWid"),
                    new ("Flute"),
                    new ("PaperWidth"),
                    new ("CutNo"),
                    new ("TrimOfRouting"),
                    new ("PercenTrim"),
                    new ("New_TrimOfFlute"),
                    new ("New_PaperWidth"),
                    new ("New_CutNo"),
                    new ("New_TrimOfRouting"),
                    new ("New_PercenTrim"),
                });

                var sqlConnection = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
                using IDbConnection db = sqlConnection;
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query //m.Material_No as MasterData, 
                string sql = @"   
                    select m.FactoryCode,
	                    m.Flute, 
	                    f.Trim as TrimOfFlute, 
	                    m.Board, 
	                    b.GroupPaperWidth, 
	                    m.CutSheetWid, 
	                    c.Mintrim as MinTrim, 
	                    c.Cut_off as CutOff, 
	                    c.Min_Out pageMin,
	                    r.Id ,
	                    r.FactoryCode ,
	                    r.Seq_No as SeqNo ,
	                    r.Plant ,
	                    r.Material_No as MaterialNo ,
	                    r.Mat_Code as MatCode ,
	                    r.Plan_Code as PlanCode ,
	                    r.Machine ,
	                    r.Alternative1 ,
	                    r.Alternative2 ,
	                    r.Std_Process as StdProcess ,
	                    r.Speed ,
	                    r.Colour_Count as ColourCount ,
	                    r.MC_Move as McMove ,
	                    r.HandHold ,
	                    r.Plate_No as PlateNo ,
	                    r.Myla_No as MylaNo ,
	                    r.Paper_Width as PaperWidth ,
	                    r.Cut_No as CutNo ,
	                    r.Trim ,
	                    r.PercenTrim ,
	                    r.Waste_Leg as WasteLeg ,
	                    r.Waste_Wid as WasteWid ,
	                    r.Sheet_in_Leg as SheetInLeg ,
	                    r.Sheet_in_Wid as SheetInWid ,
	                    r.Sheet_out_Leg as SheetOutLeg ,
	                    r.Sheet_out_Wid as SheetOutWid ,
	                    r.Weight_in as WeightIn ,
	                    r.Weight_out as WeightOut ,
	                    r.No_Open_in as NoOpenIn ,
	                    r.No_Open_out as NoOpenOut ,
	                    r.Color1 ,
	                    r.Shade1 ,
	                    r.Color2 ,
	                    r.Shade2 ,
	                    r.Color3 ,
	                    r.Shade3 ,
	                    r.Color4 ,
	                    r.Shade4 ,
	                    r.Color5 ,
	                    r.Shade5 ,
	                    r.Color6 ,
	                    r.Shade6 ,
	                    r.Color7 ,
	                    r.Shade7 ,
	                    r.Color8 ,
	                    r.Shade8 ,
	                    r.Color9 ,
	                    r.Shade9 ,
	                    r.Color10 ,
	                    r.Shade10 ,
	                    r.Color_Area1  as ColorArea1 ,
	                    r.Color_Area2  as ColorArea2 ,
	                    r.Color_Area3  as ColorArea3 ,
	                    r.Color_Area4  as ColorArea4 ,
	                    r.Color_Area5  as ColorArea5 ,
	                    r.Color_Area6  as ColorArea6 ,
	                    r.Color_Area7  as ColorArea7 ,
	                    r.Platen ,
	                    r.Rotary ,
	                    r.TearTape ,
	                    r.None_Blk as NoneBlk ,
	                    r.Stan_Blk as StanBlk ,
	                    r.Semi_Blk as SemiBlk ,
	                    r.Ship_Blk as ShipBlk ,
	                    r.Block_No as BlockNo ,
	                    r.Join_Mat_no as JoinMatNo ,
	                    r.Separat_Mat_no as SeparatMatNo ,
	                    r.Remark_Inprocess as RemarkInprocess ,
	                    r.Hardship ,
	                    r.PDIS_Status as PdisStatus ,
	                    r.Tran_Status as TranStatus ,
	                    r.SAP_Status as SapStatus ,
	                    r.Alternative3 ,
	                    r.Alternative4 ,
	                    r.Alternative5 ,
	                    r.Alternative6 ,
	                    r.Alternative7 ,
	                    r.Alternative8 ,
	                    r.Rotate_In as RotateIn ,
	                    r.Rotate_Out as RotateOut ,
	                    r.Stack_Height as StackHeight ,
	                    r.Setup_tm as SetupTm ,
	                    r.Setup_waste as SetupWaste ,
	                    r.Prepare_tm as PrepareTm ,
	                    r.Post_tm as PostTm ,
	                    r.Run_waste as RunWaste ,
	                    r.Human as Human ,
	                    r.Color_count as ColorCount ,
	                    r.UnUpgrad_Board as UnUpgradBoard ,
	                    r.Score_type as ScoreType ,
	                    r.Score_Gap as ScoreGap ,
	                    r.Coating ,
	                    r.BlockNo2 ,
	                    r.BlockNoPlant2 ,
	                    r.BlockNo3 ,
	                    r.BlockNoPlant3 ,
	                    r.BlockNo4 ,
	                    r.BlockNoPlant4 ,
	                    r.BlockNo5 ,
	                    r.BlockNoPlant5 ,
	                    r.PlateNo2 ,
	                    r.PlateNoPlant2 ,
	                    r.MylaNo2 ,
	                    r.MylaNoPlant2 ,
	                    r.PlateNo3 ,
	                    r.PlateNoPlant3 ,
	                    r.MylaNo3 ,
	                    r.MylaNoPlant3 ,
	                    r.PlateNo4 ,
	                    r.PlateNoPlant4 ,
	                    r.MylaNo4 ,
	                    r.MylaNoPlant4 ,
	                    r.PlateNo5 ,
	                    r.PlateNoPlant5 ,
	                    r.MylaNo5 ,
	                    r.MylaNoPlant5 ,
	                    r.TearTapeQty ,
	                    r.TearTapeDistance ,
	                    r.MylaSize ,
	                    r.RepeatLength ,
	                    r.CustBarcodeNo ,
	                    r.ControllerCode ,
	                    r.PlanProgramCode ,
	                    r.CreatedDate ,
	                    r.CreatedBy ,
	                    r.UpdatedDate ,
	                    r.UpdatedBy ,
                    case 
	                    when b.GroupPaperWidth = 1 then pp.a1
	                    when b.GroupPaperWidth = 2 then pp.a2
	                    when b.GroupPaperWidth = 3 then pp.a3
	                    when b.GroupPaperWidth = 4 then pp.a4 end PageMinTrim ,
                    case 
	                    when b.GroupPaperWidth = 1 then p.a1
	                    when b.GroupPaperWidth = 2 then p.a2
	                    when b.GroupPaperWidth = 3 then p.a3
	                    when b.GroupPaperWidth = 4 then p.a4 end PageMax
                    from (
	                    SELECT  FactoryCode, Material_No, Flute, Code, Board, CutSheetWid, Box_Type, PDIS_Status
	                    FROM  MasterData
	                    WHERE FactoryCode = '{0}' and Flute = '{1}' and PDIS_Status <> 'X') m 
	                    left outer join (select * from Routing where FactoryCode = '{2}' and Mat_Code = 'CORR') r 
	                    on r.Material_No = m.Material_No and r.FactoryCode = m.FactoryCode 
	                    left outer join Cor_Config c on c.Name = r.Machine and c.FactoryCode = r.FactoryCode
	                    left outer join Flute f on f.Flute = m.Flute and f.FactoryCode = m.FactoryCode
	                    left outer join Board_Combine b on b.Code = m.Code
	                    left outer join (select FactoryCode, Max(Group1) a1, Max(Group2) a2, Max(Group3) a3, Max(Group4) a4 
		                    from PaperWidth group by FactoryCode) p on p.FactoryCode = m.FactoryCode
	                    left outer join (select FactoryCode, Min(Group1) a1, Min(Group2) a2, Min(Group3) a3, Min(Group4) a4 
		                    from PaperWidth group by FactoryCode) pp on pp.FactoryCode = m.FactoryCode
	                    where r.Seq_No is not null and c.Name is not null
	                    order by c.Mintrim
                ";
                string message = string.Format(sql, factoryCode, flute, factoryCode);
                reCalculateTrimModels = db.Query<ReCalculateTrimModel>(message).ToList();


                foreach (var reCalculateTrimModel in reCalculateTrimModels)
                {
                    var routing = new Routing();
                    int? sizeWidth = 0;                                                                                                 //   TransactionDataModel tran = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                    var RollWidth = PMTsDbContext.PaperWidth.Where(x => x.FactoryCode == factoryCode).OrderBy(o => o.Group2).ToList(); //   JsonConvert.DeserializeObject<List<PaperWidth>>(_paperWidthAPIRepository.GetPaperWidthList(_factoryCode)).OrderBy(o => o.Group2).ToList();
                    var pageMin = reCalculateTrimModel.MinTrim ? reCalculateTrimModel.PageMinTrim : reCalculateTrimModel.PageMin;
                    var pageMax = reCalculateTrimModel.PageMax;

                    #region Set Routing Model

                    routing.Id = reCalculateTrimModel.Id;
                    routing.FactoryCode = reCalculateTrimModel.FactoryCode;
                    routing.SeqNo = reCalculateTrimModel.SeqNo;
                    routing.Plant = reCalculateTrimModel.Plant;
                    routing.MaterialNo = reCalculateTrimModel.MaterialNo;
                    routing.MatCode = reCalculateTrimModel.MatCode;
                    routing.PlanCode = reCalculateTrimModel.PlanCode;
                    routing.Machine = reCalculateTrimModel.Machine;
                    routing.Alternative1 = reCalculateTrimModel.Alternative1;
                    routing.Alternative2 = reCalculateTrimModel.Alternative2;
                    routing.StdProcess = reCalculateTrimModel.StdProcess;
                    routing.Speed = reCalculateTrimModel.Speed;
                    routing.ColourCount = reCalculateTrimModel.ColourCount;
                    routing.McMove = reCalculateTrimModel.McMove;
                    routing.HandHold = reCalculateTrimModel.HandHold;
                    routing.PlateNo = reCalculateTrimModel.PlateNo;
                    routing.MylaNo = reCalculateTrimModel.MylaNo;
                    routing.PaperWidth = reCalculateTrimModel.PaperWidth;
                    routing.CutNo = reCalculateTrimModel.CutNo;
                    routing.Trim = reCalculateTrimModel.Trim;
                    routing.PercenTrim = reCalculateTrimModel.PercenTrim;
                    routing.WasteLeg = reCalculateTrimModel.WasteLeg;
                    routing.WasteWid = reCalculateTrimModel.WasteWid;
                    routing.SheetInLeg = reCalculateTrimModel.SheetInLeg;
                    routing.SheetInWid = reCalculateTrimModel.SheetInWid;
                    routing.SheetOutLeg = reCalculateTrimModel.SheetOutLeg;
                    routing.SheetOutWid = reCalculateTrimModel.SheetOutWid;
                    routing.WeightIn = reCalculateTrimModel.WeightIn;
                    routing.WeightOut = reCalculateTrimModel.WeightOut;
                    routing.NoOpenIn = reCalculateTrimModel.NoOpenIn;
                    routing.NoOpenOut = reCalculateTrimModel.NoOpenOut;
                    routing.Color1 = reCalculateTrimModel.Color1;
                    routing.Shade1 = reCalculateTrimModel.Shade1;
                    routing.Color2 = reCalculateTrimModel.Color2;
                    routing.Shade2 = reCalculateTrimModel.Shade2;
                    routing.Color3 = reCalculateTrimModel.Color3;
                    routing.Shade3 = reCalculateTrimModel.Shade3;
                    routing.Color4 = reCalculateTrimModel.Color4;
                    routing.Shade4 = reCalculateTrimModel.Shade4;
                    routing.Color5 = reCalculateTrimModel.Color5;
                    routing.Shade5 = reCalculateTrimModel.Shade5;
                    routing.Color6 = reCalculateTrimModel.Color6;
                    routing.Shade6 = reCalculateTrimModel.Shade6;
                    routing.Color7 = reCalculateTrimModel.Color7;
                    routing.Shade7 = reCalculateTrimModel.Shade7;
                    routing.ColorArea1 = reCalculateTrimModel.ColorArea1;
                    routing.ColorArea2 = reCalculateTrimModel.ColorArea2;
                    routing.ColorArea3 = reCalculateTrimModel.ColorArea3;
                    routing.ColorArea4 = reCalculateTrimModel.ColorArea4;
                    routing.ColorArea5 = reCalculateTrimModel.ColorArea5;
                    routing.ColorArea6 = reCalculateTrimModel.ColorArea6;
                    routing.ColorArea7 = reCalculateTrimModel.ColorArea7;
                    routing.Platen = reCalculateTrimModel.Platen;
                    routing.Rotary = reCalculateTrimModel.Rotary;
                    routing.TearTape = reCalculateTrimModel.TearTape;
                    routing.NoneBlk = reCalculateTrimModel.NoneBlk;
                    routing.StanBlk = reCalculateTrimModel.StanBlk;
                    routing.SemiBlk = reCalculateTrimModel.SemiBlk;
                    routing.ShipBlk = reCalculateTrimModel.ShipBlk;
                    routing.BlockNo = reCalculateTrimModel.BlockNo;
                    routing.JoinMatNo = reCalculateTrimModel.JoinMatNo;
                    routing.SeparatMatNo = reCalculateTrimModel.SeparatMatNo;
                    routing.RemarkInprocess = reCalculateTrimModel.RemarkInprocess;
                    routing.Hardship = reCalculateTrimModel.Hardship;
                    routing.PdisStatus = reCalculateTrimModel.PdisStatus;
                    routing.TranStatus = reCalculateTrimModel.TranStatus;
                    routing.SapStatus = reCalculateTrimModel.SapStatus;
                    routing.Alternative3 = reCalculateTrimModel.Alternative3;
                    routing.Alternative4 = reCalculateTrimModel.Alternative4;
                    routing.Alternative5 = reCalculateTrimModel.Alternative5;
                    routing.Alternative6 = reCalculateTrimModel.Alternative6;
                    routing.Alternative7 = reCalculateTrimModel.Alternative7;
                    routing.Alternative8 = reCalculateTrimModel.Alternative8;
                    routing.RotateIn = reCalculateTrimModel.RotateIn;
                    routing.RotateOut = reCalculateTrimModel.RotateOut;
                    routing.StackHeight = reCalculateTrimModel.StackHeight;
                    routing.SetupTm = reCalculateTrimModel.SetupTm;
                    routing.SetupWaste = reCalculateTrimModel.SetupWaste;
                    routing.PrepareTm = reCalculateTrimModel.PrepareTm;
                    routing.PostTm = reCalculateTrimModel.PostTm;
                    routing.RunWaste = reCalculateTrimModel.RunWaste;
                    routing.Human = reCalculateTrimModel.Human;
                    routing.ColorCount = reCalculateTrimModel.ColorCount;
                    routing.UnUpgradBoard = reCalculateTrimModel.UnUpgradBoard;
                    routing.ScoreType = reCalculateTrimModel.ScoreType;
                    routing.ScoreGap = reCalculateTrimModel.ScoreGap;
                    routing.Coating = reCalculateTrimModel.Coating;
                    routing.BlockNo2 = reCalculateTrimModel.BlockNo2;
                    routing.BlockNoPlant2 = reCalculateTrimModel.BlockNoPlant2;
                    routing.BlockNo3 = reCalculateTrimModel.BlockNo3;
                    routing.BlockNoPlant3 = reCalculateTrimModel.BlockNoPlant3;
                    routing.BlockNo4 = reCalculateTrimModel.BlockNo4;
                    routing.BlockNoPlant4 = reCalculateTrimModel.BlockNoPlant4;
                    routing.BlockNo5 = reCalculateTrimModel.BlockNo5;
                    routing.BlockNoPlant5 = reCalculateTrimModel.BlockNoPlant5;
                    routing.PlateNo2 = reCalculateTrimModel.PlateNo2;
                    routing.PlateNoPlant2 = reCalculateTrimModel.PlateNoPlant2;
                    routing.MylaNo2 = reCalculateTrimModel.MylaNo2;
                    routing.MylaNoPlant2 = reCalculateTrimModel.MylaNoPlant2;
                    routing.PlateNo3 = reCalculateTrimModel.PlateNo3;
                    routing.PlateNoPlant3 = reCalculateTrimModel.PlateNoPlant3;
                    routing.MylaNo3 = reCalculateTrimModel.MylaNo3;
                    routing.MylaNoPlant3 = reCalculateTrimModel.MylaNoPlant3;
                    routing.PlateNo4 = reCalculateTrimModel.PlateNo4;
                    routing.PlateNoPlant4 = reCalculateTrimModel.PlateNoPlant4;
                    routing.MylaNo4 = reCalculateTrimModel.MylaNo4;
                    routing.MylaNoPlant4 = reCalculateTrimModel.MylaNoPlant4;
                    routing.PlateNo5 = reCalculateTrimModel.PlateNo5;
                    routing.PlateNoPlant5 = reCalculateTrimModel.PlateNoPlant5;
                    routing.MylaNo5 = reCalculateTrimModel.MylaNo5;
                    routing.MylaNoPlant5 = reCalculateTrimModel.MylaNoPlant5;
                    routing.TearTapeQty = reCalculateTrimModel.TearTapeQty;
                    routing.TearTapeDistance = reCalculateTrimModel.TearTapeDistance;
                    routing.MylaSize = reCalculateTrimModel.MylaSize;
                    routing.RepeatLength = reCalculateTrimModel.RepeatLength;
                    routing.CustBarcodeNo = reCalculateTrimModel.CustBarcodeNo;
                    routing.ControllerCode = reCalculateTrimModel.ControllerCode;
                    routing.PlanProgramCode = reCalculateTrimModel.PlanProgramCode;
                    routing.CreatedDate = reCalculateTrimModel.CreatedDate;
                    routing.CreatedBy = reCalculateTrimModel.CreatedBy;
                    #endregion

                    if (reCalculateTrimModel.MinTrim) //Min Trim
                    {

                        double[,] RollSize = new double[6, 4];
                        int X, M;

                        var Roll = RollWidth.FirstOrDefault(w => w.Group1 == pageMin || w.Group2 == pageMin || w.Group3 == pageMin || w.Group4 == pageMin);

                        if (Roll != null)
                        {
                            pageMin = reCalculateTrimModel.GroupPaperWidth == 1 ? ConvertInt16ToShort(Roll.Group1) : reCalculateTrimModel.GroupPaperWidth == 2 ? ConvertInt16ToShort(Roll.Group2) : reCalculateTrimModel.GroupPaperWidth == 3 ? ConvertInt16ToShort(Roll.Group3) : ConvertInt16ToShort(Roll.Group4);

                        }

                        for (X = 0; X < reCalculateTrimModel.CutOff; X++) //คำนวนหน้ากว้าง + Standard Trim
                        {
                            RollSize[X, 1] = (reCalculateTrimModel.CutSheetWid.Value * (X + 1)) + reCalculateTrimModel.TrimOfFlute.Value;

                            if (RollSize[X, 1] < pageMin)
                            {
                                RollSize[X, 0] = pageMin;   //น้อยกว่าหน้าน้อยสุด
                            }
                            else if (RollSize[X, 1] > pageMax)
                            {
                                X--;
                                //RollSize[X, 0] = RollSize[X - 1, 0];     //มากกว่าหน้าสูงสุด
                                break;
                            }
                            else
                            {
                                switch (reCalculateTrimModel.GroupPaperWidth)
                                {
                                    case 1:
                                        for (M = 0; M < RollWidth.Count; M++)
                                        {
                                            if (ConvertInt16ToShort(RollWidth[M].Group1) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group1) <= pageMax)
                                            {
                                                if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group1))
                                                {
                                                    RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group1);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        for (M = 0; M < RollWidth.Count; M++)
                                        {
                                            if (ConvertInt16ToShort(RollWidth[M].Group2) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group2) <= pageMax)
                                            {
                                                if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group2))
                                                {
                                                    RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group2);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        for (M = 0; M < RollWidth.Count; M++)
                                        {
                                            if (ConvertInt16ToShort(RollWidth[M].Group3) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group3) <= pageMax)
                                            {
                                                if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group3))
                                                {
                                                    RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group3);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        for (M = 0; M < RollWidth.Count; M++)
                                        {
                                            if (ConvertInt16ToShort(RollWidth[M].Group4) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group4) <= pageMax)
                                            {
                                                if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group4))
                                                {
                                                    RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group4);
                                                    break;
                                                }
                                            }
                                        }
                                        break;

                                }
                            } // อันเก่า


                        }

                        if (X >= reCalculateTrimModel.CutOff) X = reCalculateTrimModel.CutOff - 1;

                        if (RollSize[X, 0] > 0)
                        {
                            for (var Z = X; Z >= 0; Z--)
                            {
                                RollSize[Z, 2] = (RollSize[Z, 0] - (RollSize[Z, 1] - reCalculateTrimModel.TrimOfFlute.Value)) / RollSize[Z, 0] * 100; //คำนวน % Trim

                                if (Convert.ToDouble(routing.PercenTrim) >= Math.Round(RollSize[Z, 2], 2))//เลือก % Trim น้อยที่สุด
                                {
                                    routing.PaperWidth = Convert.ToInt32(RollSize[Z, 0]);
                                    routing.CutNo = Convert.ToInt32((Z + 1).ToString());
                                    routing.Trim = Convert.ToInt32((RollSize[Z, 0] - RollSize[Z, 1] + reCalculateTrimModel.TrimOfFlute.Value).ToString()); //เศษ
                                    routing.PercenTrim = Convert.ToDouble((Math.Round(RollSize[Z, 2], 2)).ToString());
                                }
                            }
                        }
                    }
                    else //Max Out
                    {
                        sizeWidth = (reCalculateTrimModel.CutSheetWid * reCalculateTrimModel.CutOff) + reCalculateTrimModel.TrimOfFlute;

                        //ตรวจหาหน้ากว้างสุดและแคบสุด
                        if (sizeWidth < pageMin)
                        {
                            routing.PaperWidth = Convert.ToInt32(reCalculateTrimModel.PageMin.ToString());                                //Paper Width
                            routing.CutNo = reCalculateTrimModel.CutOff;                                                          //จำนวนตัด
                            routing.Trim = Convert.ToInt32((reCalculateTrimModel.PageMin - reCalculateTrimModel.CutSheetWid * reCalculateTrimModel.CutOff).ToString());                            //เศษตัดริม
                            routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim.Value) / Convert.ToDouble(pageMin) * 100), 2);   //% Waste
                        }
                        /////////////////////////////////////////////////////////////

                        if (reCalculateTrimModel.CutSheetWid + reCalculateTrimModel.TrimOfFlute > pageMax)
                        {
                            routing.PaperWidth = pageMax;                                //Paper Width
                            routing.CutNo = 1;                                            //จำนวนตัด
                            routing.Trim = (pageMax - reCalculateTrimModel.CutSheetWid);                        //เศษตัดริม
                            routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim.Value) / Convert.ToDouble(pageMax) * 100), 2);   //% Waste
                        }

                        /////////////////////////////////////////////////////////////

                        int k = reCalculateTrimModel.CutOff;
                        for (k = reCalculateTrimModel.CutOff; k > 0; k--)
                        {
                            if (sizeWidth > pageMin)
                            {
                                sizeWidth = reCalculateTrimModel.CutSheetWid * k + reCalculateTrimModel.TrimOfFlute;
                                if (sizeWidth <= pageMax)
                                {
                                    break;
                                }
                            }
                            else break;
                        }

                        switch (reCalculateTrimModel.GroupPaperWidth)
                        {
                            case 1:
                                foreach (var rollWidth in RollWidth)
                                {
                                    if (rollWidth.Group1 >= sizeWidth)
                                    {
                                        routing.PaperWidth = rollWidth.Group1;                                        //Paper Width
                                        routing.CutNo = k;                                                            //จำนวนตัด
                                        routing.Trim = (rollWidth.Group1 - reCalculateTrimModel.CutSheetWid.Value * k);               //เศษตัดริม
                                        routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim) / Convert.ToDouble(rollWidth.Group1) * 100), 2);           //% Waste
                                        break;
                                    }
                                }
                                break;
                            case 2:
                                foreach (var rollWidth in RollWidth)
                                {
                                    if (rollWidth.Group2 >= sizeWidth)
                                    {
                                        routing.PaperWidth = rollWidth.Group2;                                        //Paper Width
                                        routing.CutNo = k;                                                            //จำนวนตัด
                                        routing.Trim = (rollWidth.Group2 - reCalculateTrimModel.CutSheetWid.Value * k);     //เศษตัดริม
                                        routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim) / Convert.ToDouble(rollWidth.Group2) * 100), 2);     //% Waste
                                        break;
                                    }
                                }
                                break;
                        }

                    }

                    dataTable.Rows.Add(
                        reCalculateTrimModel.FactoryCode,
                        reCalculateTrimModel.MaterialNo,
                        reCalculateTrimModel.CutSheetWid,
                        reCalculateTrimModel.Flute,
                        reCalculateTrimModel.PaperWidth,
                        reCalculateTrimModel.CutNo,
                        reCalculateTrimModel.Trim,
                        reCalculateTrimModel.PercenTrim,
                        reCalculateTrimModel.TrimOfFlute,
                        routing.PaperWidth,
                        routing.CutNo,
                        routing.Trim,
                        routing.PercenTrim
                        );

                    //routing update collection(waiting for update)
                    routings.Add(routing);
                }

                if (action == "ReCalculate")
                {
                    using var dbContextTransaction = PMTsDbContext.Database.BeginTransaction();
                    try
                    {
                        routings.ForEach(r => r.UpdatedDate = DateTime.Now);
                        routings.ForEach(r => r.UpdatedBy = _username);

                        reCalculateTrimModels.ForEach(r => r.UpdatedDate = DateTime.Now);
                        reCalculateTrimModels.ForEach(r => r.UpdatedBy = _username);

                        PMTsDbContext.Routing.UpdateRange(routings);
                        PMTsDbContext.SaveChanges();
                        dbContextTransaction.Commit();

                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }

                return reCalculateTrimModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public CalculateOffsetModel CalculateMoTargetQuantityOffset(string factoryCode, double? orderQuant, string materialNo, string orderItem, string userName)
        {
            MasterData masterData = PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus != "X");
            var boms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(materialNo)).ToList();

            var result = new CalculateOffsetModel
            {
                MoData = new MoData(),
                moBomRawmats = []
            };

            if (masterData.MaterialType == "82")
            {
                result.MoData = new MoData { TargetQuant = (int?)orderQuant };
                return result;
            }

            var mainBomLine = boms.Where(x => x.Width > 0).FirstOrDefault() ?? throw new Exception("Width cannot be 0.");
            var otherBomLine = boms.Where(x => x.MaterialNumber != mainBomLine?.MaterialNumber).ToList();
            var boxPacking = masterData.BoxPacking;
            if ((boxPacking ?? 0) == 0)
            {
                throw new Exception("BoxPacking cannot be 0.");
            }
            var routings = PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode);

            var ppcWorkType = PMTsDbContext.PpcWorkType.ToList();

            int workType = 0;
            if (ppcWorkType.Count != 0 && !string.IsNullOrEmpty(masterData.WorkType))
            {
                workType = ppcWorkType.Where(x => x.WorkType == masterData.WorkType).FirstOrDefault()?.Id ?? 0;
            }

            //int custAllowance = 0;

            //if (Tolerance_Over > 0)
            //{
            //    custAllowance = Convert.ToInt32(Math.Round(((double)orderQuant * Tolerance_Over) / 100));
            //}

            #region =============================== คำนวนหาค่าความยาก ===========================

            int difficult = 0;
            var allowanceHard = PMTsDbContext.AllowanceHard.FirstOrDefault(x => x.FactoryCode.Equals(factoryCode) && x.Hardship == masterData.Hardship);

            if (allowanceHard != null)
            {
                decimal percent = allowanceHard.Percen;
                int sheetMin = allowanceHard.SheetMin,
                    sheetMax = allowanceHard.SheetMax;

                difficult = Convert.ToInt32(Convert.ToDecimal(orderQuant) * percent / 100);

                if (difficult < 0)
                {
                    if (Math.Abs(difficult) < sheetMin)
                    {
                        difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMin);
                    }
                    else if (Math.Abs(difficult) > sheetMax)
                    {
                        difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMax);
                    }
                }
                else
                {
                    if (difficult < sheetMin)
                    {
                        difficult = Convert.ToInt32(sheetMin);
                    }
                    else if (difficult > sheetMax)
                    {
                        difficult = Convert.ToInt32(sheetMax);
                    }
                }
            }

            #endregion ========================================================================

            if (routings.Any())
            {
                var lay = mainBomLine.Lay is null ? 0 : mainBomLine.Lay;
                if (lay <= 0)
                {
                    throw new Exception("Lay cannot be 0.");
                }
                var cutSize = Math.Max((decimal)(mainBomLine.CutSize ?? 1), 1);

                //var allowanceProcess = new PpcProductionProcess();
                var allowanceProcess = new List<PpcProductionProcess>();
                var allowancePrintingProcess = new List<PpcProductionPrintingProcess>();
                double? orderPerRound = 0;

                foreach (var r in routings)
                {
                    var machine = PMTsDbContext.Machine.Where(m => m.PlanCode == r.PlanCode && m.FactoryCode == r.FactoryCode).FirstOrDefault();
                    orderPerRound = machine.Glue == true ? orderQuant : orderQuant / lay;
                    var a = PMTsDbContext.PpcProductionProcess.Where(p => p.Plant == factoryCode && p.PlanCode == r.PlanCode && p.QuantityStart <= orderPerRound && p.QuantityTo >= orderPerRound && p.WorkType == workType).FirstOrDefault();
                    var ap = PMTsDbContext.PpcProductionPrintingProcess.Where(p => p.Plant == factoryCode && p.PlanCode == r.PlanCode && p.QuantityStart <= orderPerRound && p.QuantityTo >= orderPerRound).FirstOrDefault();


                    if (a != null)
                        allowanceProcess.Add(a);
                    //allowanceProcess = a;

                    if (ap != null)
                        allowancePrintingProcess.Add(ap);
                }
                var paperWidth = mainBomLine.Width;
                var paperLength = mainBomLine.Length;
                var bomQty = mainBomLine.BomAmount ?? 0;

                var mainQty = Math.Ceiling((decimal)orderQuant * bomQty);
                var PrintingRound = Math.Ceiling((decimal)orderQuant / (int)lay);
                //var paperWasteBefore = (allowanceProcess.Color ?? 0) + ((allowanceProcess.PaperWaste ?? 0) > 0 ?
                //    allowanceProcess.PaperWaste.Value :
                //    Math.Ceiling((decimal)orderQuant * (allowanceProcess.PercentWaste ?? 0M)));
                var paperWasteBefore = 0;
                var paperWasteAfter = 0;

                if (allowanceProcess.Count != 0)
                {
                    foreach (var a in allowanceProcess)
                    {
                        paperWasteBefore += (a.Color ?? 0) + ((a.PaperWaste ?? 0) > 0 ? a.PaperWaste.Value : (int)Math.Ceiling((decimal)orderQuant * (a.PercentWaste ?? 0M)));
                    }
                }

                if (allowancePrintingProcess.Count != 0)
                {
                    foreach (var a in allowancePrintingProcess)
                    {
                        paperWasteAfter += (a.Color ?? 0) + ((a.PaperWaste ?? 0) > 0 ? a.PaperWaste.Value : (int)Math.Ceiling((decimal)orderQuant * (a.PercentWaste ?? 0M)));
                    }
                }
                paperWasteBefore += difficult;
                var targetQty = PrintingRound + paperWasteBefore + paperWasteAfter;
                var GiConsumption = Math.Ceiling(targetQty / cutSize);

                result.moBomRawmats.Add(new MoBomRawMat
                {
                    FactoryCode = factoryCode,
                    OrderItem = orderItem,
                    FgMaterial = materialNo,
                    MaterialNumber = mainBomLine.MaterialNumber,
                    Uom = mainBomLine.Uom,
                    Lay = (int?)lay,
                    CutSize = (int?)cutSize,
                    SizeUom = mainBomLine.SizeUom,
                    Width = paperWidth,
                    Length = paperLength,
                    MaterialDescription = mainBomLine.MaterialDescription,
                    NetWeight = mainQty,
                    BomAmount = bomQty,
                    Plant = mainBomLine.Plant,
                    CreateDate = DateTime.Now,
                    CreateBy = userName
                });

                result.MoData.TargetQuant = (int?)targetQty;
                result.MoData.PrintRoundNo = (int?)PrintingRound;
                result.MoData.AllowancePrintNo = (int?)paperWasteBefore;
                result.MoData.AfterPrintNo = paperWasteAfter;
                result.MoData.DrawAmountNo = (int?)GiConsumption;

                if (otherBomLine.Count != 0)
                {
                    foreach (var o in otherBomLine)
                    {
                        result.moBomRawmats.Add(new MoBomRawMat
                        {
                            FactoryCode = factoryCode,
                            OrderItem = orderItem,
                            FgMaterial = materialNo,
                            MaterialNumber = o.MaterialNumber,
                            Uom = o.Uom,
                            SizeUom = o.SizeUom,
                            MaterialDescription = o.MaterialDescription,
                            NetWeight = Math.Ceiling((decimal)orderQuant / (decimal)(boxPacking ?? 0)),
                            BomAmount = o.BomAmount,
                            Plant = mainBomLine.Plant,
                            CreateDate = DateTime.Now,
                            CreateBy = userName
                        });
                    }
                }
            }

            return result;
        }
        public int CalculateMoTargetQtyPPC(string factoryCode, double? orderQuant, double Tolerance_Over, string materialNo)
        {
            int targetQty = 0;
            MasterData masterData = PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus != "X");
            var boms = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(materialNo)).ToList();

            var result = new CalculateOffsetModel
            {
                MoData = new MoData(),
                moBomRawmats = []
            };

            if (masterData.MaterialType == "82")
            {
                result.MoData = new MoData { TargetQuant = (int?)orderQuant };
                return (int)orderQuant;
            }

            var mainBomLine = boms.Where(x => x.Width > 0).FirstOrDefault() ?? throw new Exception("Width cannot be 0.");
            var otherBomLine = boms.Where(x => x.MaterialNumber != mainBomLine?.MaterialNumber).ToList();
            var boxPacking = masterData.BoxPacking;
            if ((boxPacking ?? 0) == 0)
            {
                throw new Exception("BoxPacking cannot be 0.");
            }
            var routings = PMTsDbContext.Routing.Where(w => w.MaterialNo.Equals(materialNo) && w.FactoryCode == factoryCode);

            var ppcWorkType = PMTsDbContext.PpcWorkType.ToList();

            int workType = 0;
            if (ppcWorkType.Count != 0 && !string.IsNullOrEmpty(masterData.WorkType))
            {
                workType = ppcWorkType.Where(x => x.WorkType == masterData.WorkType).FirstOrDefault()?.Id ?? 0;
            }

            int custAllowance = 0;

            if (Tolerance_Over > 0)
            {
                custAllowance = Convert.ToInt32(Math.Round(((double)orderQuant * Tolerance_Over) / 100));
            }
            #region =============================== คำนวนหาค่าความยาก ===========================

            int difficult = 0;
            var allowanceHard = PMTsDbContext.AllowanceHard.FirstOrDefault(x => x.FactoryCode.Equals(factoryCode) && x.Hardship == masterData.Hardship);

            if (allowanceHard != null)
            {
                decimal percent = allowanceHard.Percen;
                int sheetMin = allowanceHard.SheetMin,
                    sheetMax = allowanceHard.SheetMax;

                difficult = Convert.ToInt32(Convert.ToDecimal(orderQuant) * percent / 100);

                if (difficult < 0)
                {
                    if (Math.Abs(difficult) < sheetMin)
                    {
                        difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMin);
                    }
                    else if (Math.Abs(difficult) > sheetMax)
                    {
                        difficult = Convert.ToInt32(Convert.ToDouble(0) - sheetMax);
                    }
                }
                else
                {
                    if (difficult < sheetMin)
                    {
                        difficult = Convert.ToInt32(sheetMin);
                    }
                    else if (difficult > sheetMax)
                    {
                        difficult = Convert.ToInt32(sheetMax);
                    }
                }
            }

            #endregion ========================================================================

            if (routings.Any())
            {
                var lay = mainBomLine.Lay is null ? 0 : mainBomLine.Lay;
                if (lay <= 0)
                {
                    throw new Exception("Lay cannot be 0.");
                }
                var cutSize = Math.Max((decimal)(mainBomLine.CutSize ?? 1), 1);

                //var allowanceProcess = new PpcProductionProcess();
                var allowanceProcess = new List<PpcProductionProcess>();
                var allowancePrintingProcess = new List<PpcProductionPrintingProcess>();
                double? orderPerRound = 0;

                foreach (var r in routings)
                {
                    var machine = PMTsDbContext.Machine.Where(m => m.PlanCode == r.PlanCode && m.FactoryCode == r.FactoryCode).FirstOrDefault();
                    orderPerRound = machine.Glue == true ? orderQuant : orderQuant / lay;
                    var a = PMTsDbContext.PpcProductionProcess.Where(p => p.Plant == factoryCode && p.PlanCode == r.PlanCode && p.QuantityStart <= orderPerRound && p.QuantityTo >= orderPerRound && p.WorkType == workType).FirstOrDefault();
                    var ap = PMTsDbContext.PpcProductionPrintingProcess.Where(p => p.Plant == factoryCode && p.PlanCode == r.PlanCode && p.QuantityStart <= orderPerRound && p.QuantityTo >= orderPerRound).FirstOrDefault();


                    if (a != null)
                        allowanceProcess.Add(a);
                    //allowanceProcess = a;

                    if (ap != null)
                        allowancePrintingProcess.Add(ap);
                }

                var paperWidth = mainBomLine.Width;
                var paperLength = mainBomLine.Length;
                var bomQty = mainBomLine.BomAmount ?? 0;

                var mainQty = Math.Ceiling((decimal)orderQuant * bomQty);
                var PrintingRound = Math.Ceiling((decimal)(orderQuant + custAllowance) / (int)lay);
                //var paperWasteBefore = (allowanceProcess.Color ?? 0) + ((allowanceProcess.PaperWaste ?? 0) > 0 ?
                //    allowanceProcess.PaperWaste.Value :
                //    Math.Ceiling((decimal)orderQuant * (allowanceProcess.PercentWaste ?? 0M)));
                var paperWasteBefore = 0;
                var paperWasteAfter = 0;

                if (allowanceProcess.Count != 0)
                {
                    foreach (var a in allowanceProcess)
                    {
                        paperWasteBefore += (a.Color ?? 0) + ((a.PaperWaste ?? 0) > 0 ? a.PaperWaste.Value : (int)Math.Ceiling((decimal)orderQuant * (a.PercentWaste ?? 0M)));
                    }
                }

                if (allowancePrintingProcess.Count != 0)
                {
                    foreach (var a in allowancePrintingProcess)
                    {
                        paperWasteAfter += (a.Color ?? 0) + ((a.PaperWaste ?? 0) > 0 ? a.PaperWaste.Value : (int)Math.Ceiling((decimal)orderQuant * (a.PercentWaste ?? 0M)));
                    }
                }

                paperWasteBefore += difficult;
                targetQty = (int)(PrintingRound + paperWasteBefore + paperWasteAfter);
                var GiConsumption = Math.Ceiling(targetQty / cutSize);

                result.MoData.TargetQuant = (int?)targetQty;
                result.MoData.PrintRoundNo = (int?)PrintingRound;
                result.MoData.AllowancePrintNo = (int?)paperWasteBefore;
                result.MoData.AfterPrintNo = paperWasteAfter;
                result.MoData.DrawAmountNo = (int?)GiConsumption;
            }

            return targetQty;
        }
        public string CalSizeDimensions(string factoryCode, string materialNo)
        {
            var result = string.Empty;
            try
            {
                MasterData masterData = PMTsDbContext.MasterData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.MaterialNo == materialNo && m.PdisStatus != "X");

                if (masterData != null)
                {
                    var flute = PMTsDbContext.Flute.FirstOrDefault(p => p.FactoryCode == factoryCode && p.Flute1 == masterData.Flute);
                    double truckStack = 0;
                    if (flute != null) { truckStack = flute.TruckStack ?? 0; }
                    double.TryParse(PMTsDbContext.PmtsConfig.FirstOrDefault(p => p.FactoryCode == factoryCode && p.FucName == "Size_Dimession_Const")?.FucValue, out double sizeDimensionConst);
                    sizeDimensionConst = sizeDimensionConst == 0 ? 1905 : sizeDimensionConst;
                    if (!string.IsNullOrEmpty(masterData.BoxType))
                    {
                        if (!masterData.BoxType.Contains("DIECUT", StringComparison.CurrentCultureIgnoreCase))
                        {
                            double cutSheetWid = 0, cutSheetLeng = 0;
                            cutSheetWid = Convert.ToDouble(masterData.CutSheetWid ?? 0);
                            cutSheetLeng = Convert.ToDouble(masterData.CutSheetLeng ?? 0);

                            double res = 0;
                            if (sizeDimensionConst > 0 && truckStack > 0)
                            {
                                res = (cutSheetWid / sizeDimensionConst) * (cutSheetLeng / truckStack);
                                result = res.ToString();
                                if (result.Length > 32)
                                {
                                    result = result[..32];
                                }
                            }
                        }
                        else if (masterData.BoxType.Contains("DIECUT", StringComparison.CurrentCultureIgnoreCase) || masterData.Hierarchy.StartsWith("03PA"))
                        {
                            var routing = PMTsDbContext.Routing
                                .FirstOrDefault(p =>
                                    p.FactoryCode == factoryCode &&
                                    p.MaterialNo == materialNo &&
                                    ("คลัง".Contains(p.Machine) || "WH".Contains(p.MatCode)));

                            double sheetInWid = 0, sheetInLeg = 0;
                            if (routing != null)
                            {
                                sheetInWid = Convert.ToDouble(routing.SheetInWid ?? 0);
                                sheetInLeg = Convert.ToDouble(routing.SheetInLeg ?? 0);
                                double res = 0;
                                if (sizeDimensionConst > 0 && truckStack > 0)
                                {
                                    res = (sheetInWid / sizeDimensionConst) * (sheetInLeg / truckStack);
                                    result = res.ToString();
                                    if (result.Length > 32)
                                    {
                                        result = result[..32];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return result;
        }
    }
}
