using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace PMTs.DataAccess.Repositories
{
    public class StandardPatternNameRepository : Repository<StandardPatternName>, IStandardPatternNameRepository
    {
        public StandardPatternNameRepository(PMTsDbContext context)
           : base(context)
        {
        }



        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public StandardPatternName GetStandardPatternName(string palletname)
        {
            return PMTsDbContext.StandardPatternName.FirstOrDefault(x => x.PictureNamePallet.Equals(palletname));
        }

        public PalletCalulate GetCalculatePallet(string FactoryCode, PalletCalculateParam model)
        {
            PalletCalulate palletCalulate = new PalletCalulate();

            // หา thickness Flute 
            //double dataflute = string.Empty;

            var flu = PMTsDbContext.Flute.Where(f => f.Flute1 == model.Flute && f.FactoryCode == FactoryCode).FirstOrDefault();
            //Flute flute = new
            var formgroup = model.FormGroup;
            var Wid = 0;
            var Leg = 0;
            var Hig = 0;
            var ScoreL6 = model.ScoreL6;

            if (formgroup.Contains("RSC"))
            {
                Wid = Convert.ToInt32(model.CutSheetWid);
                Leg = Convert.ToInt32(model.ScoreL6);
                Hig = Convert.ToInt32(model.Hig);
            }
            else if ((formgroup.Contains("DC") || formgroup.Contains("HB") || formgroup.Contains("AC") || formgroup.Contains("SB") || formgroup.Contains("SS")) && (model.JoinTypeFilter != "OOO"))
            {
                Wid = model.WidDC;
                Leg = model.LegDC / 2;
                Hig = 1;
            }
            else if ((formgroup.Contains("DC") || formgroup.Contains("HB") || formgroup.Contains("AC") || formgroup.Contains("SB") || formgroup.Contains("SS")) && (model.JoinTypeFilter == "OOO"))
            {
                Wid = model.WidDC;
                Leg = model.LegDC;
                Hig = 1;
            }


            var PalletResult = func_pallet(model.FormGroup, flu.Flute1, flu.A, flu.B, flu.C, flu.D1, flu.Thickness,
     Wid, Leg, Hig, model.palletSizeFilter, model.Overhang, model.CutSheetWid, model.CutSheetLeng, model.JoinTypeFilter);

            palletCalulate.BunLayer = PalletResult.Item2;
            palletCalulate.PicPallet = PalletResult.Item1;

            return palletCalulate;
        }

        public (string, int) func_pallet(string FormGroup, string Flute1, int? A, int? B, int? C, int? D1, double Thickness,
            int? wid, int? leg, int? hig, string palletsize, int palletOverhang, int? CutSheetWid, int? CutSheetLeng, string JoinTypeFilter)
        {

            double FlatBoxtype_H = 0.0;
            int FlatBoxtype_L = 0;
            int FlatBoxtype_W = 0;
            int ForWL;

            // make ค่า Die Cut	เผื่อกว้างยาว กับ ??
            int SpareWL = 0;
            ForWL = SpareWL;

            if (FormGroup == "STDRSC")
            {
                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness * 2;
            }

            else if (FormGroup == "STDDC")
            {
                //ForBoxtype_l = dataflute.FluteDC;
                //ForBoxtype_W = dataflute.FluteDC;
                //ForBoxtype_D = dataflute.FluteDC;
                //ForBoxtype_T = dataflute.Thickness;

                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness;

            }
            else if ((FormGroup.Contains("DC") || FormGroup.Contains("HB") || FormGroup.Contains("AC") || FormGroup.Contains("SB") || FormGroup.Contains("SS")))
            //  Pallet Size
            {
                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness;
            }
            var PalletSize = palletsize.Split('x');
            // decimal PalletSize0 = Convert.ToDecimal(PalletSize[0]);
            var PalletNormal_W = Convert.ToInt32(Convert.ToDecimal(PalletSize[0]) * 1000);
            var PalletNormal_L = Convert.ToInt32(Convert.ToDecimal(PalletSize[1]) * 1000);
            var PalletOVERHANG_L = PalletNormal_L + palletOverhang;
            var PalletOVERHANG_W = PalletNormal_W + palletOverhang;



            //           ยอมให้วางเลย pallet เมื่อ มีด้านที่ยาวกว่า 1200
            int? Pallet_L1 = 0, Pallet_W1 = 0;
            int? Pallet_L2 = 0, Pallet_W2 = 0;
            Pallet_L1 = (FlatBoxtype_L <= 300) ? PalletNormal_L : PalletOVERHANG_L;
            Pallet_W1 = (FlatBoxtype_W <= 300) ? PalletNormal_W : PalletOVERHANG_W;
            Pallet_L2 = (FlatBoxtype_W <= 300) ? PalletNormal_L : PalletOVERHANG_L;
            Pallet_W2 = (FlatBoxtype_W <= 300) ? PalletNormal_W : PalletOVERHANG_W;





            List<PalletresultModel> palletresultModel = new List<PalletresultModel>();

            List<ColumnPalletModel> columnPalletModel = new List<ColumnPalletModel>();

            // =====Update getpallet from database
            //var standardPatternName2 = JsonConvert.DeserializeObject<List<StandardPatternName>>(_standardPatternNameAPIRepository.GetAllByFactory(_factoryCode, _token));
            var standardPatternName = PMTsDbContext.StandardPatternName.ToList();
            foreach (var values in standardPatternName.OrderBy(x => x.Id).ThenBy(x => x.Type))
            {
                var calcL1 = Convert.ToInt32(values.Col) * FlatBoxtype_L;
                var calcW1 = Convert.ToInt32(values.Row) * FlatBoxtype_W;
                var calcL2 = Convert.ToInt32(values.Col) * FlatBoxtype_W;
                var calcW2 = Convert.ToInt32(values.Row) * FlatBoxtype_L;


                var L1_L1 = calcL1 <= Pallet_L1 ? 1 : 0;
                //  var L1_L1 = calcL1, Pallet_L1);
                var W1_W1 = calcW1 <= Pallet_W1 ? 1 : 0;
                var L1_W1 = calcL1 <= Pallet_W1 ? 1 : 0;
                var W1_L1 = calcW1 <= Pallet_L1 ? 1 : 0;
                var L2_L2 = calcL2 <= Pallet_L2 ? 1 : 0;
                var W2_W2 = calcW2 <= Pallet_W2 ? 1 : 0;
                var L2_W2 = calcW2 <= Pallet_L2 ? 1 : 0;
                var W2_L2 = calcL2 <= Pallet_W2 ? 1 : 0;
                var CalcLLWW1 = L1_L1 * W1_W1; //A
                var CalcLWWL1 = L1_W1 * W1_L1; //B
                var CalcLLWW2 = L2_L2 * W2_W2; //C
                var CalcLWWL2 = L2_W2 * W2_L2; //D
                                               // var ChkL1 = chkValue2(CalcLL1WW1, Convert.ToInt32(values.BundlePerLayyer));
                var ChkLLWW1 = CalcLLWW1 == 1 ? Convert.ToInt32(values.Amount) : 0; //E
                var ChkLLWWL1 = CalcLWWL1 == 1 ? Convert.ToInt32(values.Amount) : 0; //F
                var ChkcLLWW2 = CalcLLWW2 == 1 ? Convert.ToInt32(values.Amount) : 0; //G
                var ChkcLWWL2 = CalcLWWL2 == 1 ? Convert.ToInt32(values.Amount) : 0;//H

                var maxChk = new[] { ChkLLWW1, ChkLLWWL1, ChkcLLWW2, ChkcLWWL2 };
                var qtyMax = maxChk.Max(); //ใบ/ชั้น

                columnPalletModel.Add(new ColumnPalletModel
                {
                    Type = values.Type,
                    LxW = values.Col + "x" + values.Row,
                    BundlePerLayyer = Convert.ToInt32(values.Amount),
                    L = FlatBoxtype_L,
                    W = FlatBoxtype_W,
                    L1 = calcL1,
                    W1 = calcW1,
                    L2 = calcL2,
                    W2 = calcW2,
                    L1_L1 = L1_L1,
                    W1_W1 = W1_W1,
                    L1_W1 = L1_W1,
                    W1_L1 = W1_L1,
                    L2_L2 = L2_L2,
                    W2_W2 = W2_W2,
                    L2_W2 = L2_W2,
                    W2_L2 = W2_L2,
                    CartonPerLayer = qtyMax


                });
                if (values.Type == "Column")
                {
                    palletresultModel.Add(new PalletresultModel
                    {
                        // formatPalletName = "3.ColumnType",
                        formatPalletName = values.Type,
                        typePalletName = values.PictureNamePallet,
                        qtycartonPerLayer = qtyMax
                    });
                }
                // Interlock  ====

                if (values.Type != "Column")
                {
                    palletresultModel.Add(new PalletresultModel
                    {
                        formatPalletName = values.Type,
                        typePalletName = values.PictureNamePallet,
                        qtycartonPerLayer = Qtycartontable(values.Type, FlatBoxtype_L, FlatBoxtype_W, PalletNormal_L, PalletOVERHANG_L,
                        PalletNormal_W, PalletOVERHANG_W, values.Col.Value, values.Row.Value, values.Amount.Value, values.PictureNamePallet)
                    });
                }




            }

            string lblMaxCarton = "";
            var pathPic = "";
            string result = "";
            int bunlayer = 0;

            var dataresult2 = palletresultModel.Where(x => x.qtycartonPerLayer >= 0).ToList();


            var dataresult = palletresultModel.Where(x => x.qtycartonPerLayer == palletresultModel.Where(xx => xx.qtycartonPerLayer >= 0).Max(y => y.qtycartonPerLayer)).OrderBy(x => x.formatPalletName).FirstOrDefault();



            lblMaxCarton = dataresult.qtycartonPerLayer.ToString();


            // find หา new name

            var PatternName = PMTsDbContext.StandardPatternName.FirstOrDefault(x => x.PictureNamePallet.Equals(dataresult.typePalletName));

            if (lblMaxCarton == "0")
            {
                pathPic = "C0111.png";
                bunlayer = 0;
                result = pathPic;
            }
            else
            {
                pathPic = PatternName.PatternName + ".png";
                bunlayer = Convert.ToInt32(lblMaxCarton);
                result = pathPic;
            }

            return (result, bunlayer);
        }



        public int Qtycartontable(string typename, int FlatBoxtype_L, int FlatBoxtype_W, int PalletNormal_L, int PalletOVERHANG_L, int PalletNormal_W,
    int PalletOVERHANG_W, int Col, int Row, int Amount, string PictureNamePallet)
        {
            var result = 0;

            try
            {


                // string typename = "";
                var L_W_ = 0; //L'+W'
                var L_ = 0; //L'
                var W2 = 0; //2W'
                var W2_2 = 0; //2W' check


                var PalletL_L1 = 0; //L
                var PalletL_W1 = 0; // W
                var PalletL_L2 = 0;// 2W'|L
                var PalletL_W2 = 0;// 2W'|W
                int L_L_W = 0;//L/(L'+W')
                int W_2W_L = 0;//W/ 2W' OR L'
                int L_2W_L = 0;//L/2W' OR L'
                var W_L_W = 0; //W/(L'+W') 
                var LPallet_L = 0;   //L_L_W     //หันด้านยาวไว้ด้าน L --L (Pallet)
                var WPallet_L = 0; // W_2W_L       //หันด้านยาวไว้ด้าน L --W (Pallet)
                var LPallet_W = 0;// L_2W_L       //หันด้านยาวไว้ด้าน L --L (Pallet)
                var WPallet_W = 0;// W_L_W       //หันด้านยาวไว้ด้าน L --W (Pallet)
                var chkLPallet_L = 0;
                var chkLPallet_W = 0;
                var cartonperlayer1 = 0;
                var cartonperlayer2 = 0;
                var maxcartonperLayer = 0; // ===================== Max

                var qtyCartonPLay = 0;

                //สลับด้าน กว้าง ยาว ของกล่อง สำหรับ Interlock กับ Spiral
                #region Interlock
                //
                // typename = "Type 4";   
                //var f_l = FlatBoxtype_W;
                //var f_w = FlatBoxtype_L;

                var f_l = FlatBoxtype_L > FlatBoxtype_W ? FlatBoxtype_L : FlatBoxtype_W;
                var f_w = FlatBoxtype_L < FlatBoxtype_W ? FlatBoxtype_L : FlatBoxtype_W;




                //  FlatBoxtype_L = f_l;
                //  f_l = f_w;
                if (typename == "Interlock")
                {

                    if (PictureNamePallet != "Type 15")
                    {
                        L_ = f_l * Row; //L'
                        W2 = f_w * Col; //2W'
                        W2_2 = W2 > L_ ? W2 : L_; //2W' check
                        L_W_ = f_l + f_w; //L'+W'
                        qtyCartonPLay = Amount;

                    }

                    else if (PictureNamePallet == "Type 15")
                    {

                        L_ = f_l * Row; //L'
                        W2 = f_w * Col; //2W'
                        var W_2W_L15 = (f_l * Row) + f_w;
                        // W2_2 = W2 > L_ ? W2 : L_ ; //2W' check
                        W2_2 = Math.Max(W2, Math.Max(L_, W_2W_L15));
                        L_W_ = f_w + f_l; //L'+W'

                        qtyCartonPLay = Amount;
                    }

                }
                #endregion
                #region Spiral
                if (typename == "Spiral")
                {
                    if (PictureNamePallet != "Type 24")
                    {
                        L_W_ = f_l + (f_w * (Col / 2)); //L'+W'

                        // // L_W_ = (f_l * 2) ; //L'+W'
                        //L_ = L_W_; //L'+W'
                        //W2 = L_W_; //2W'
                        //W2 = L_W_; //2W'
                        //W2_2 = L_W_; //2W' check
                        //qtyCartonPLay = Amount;
                    }

                    else if (PictureNamePallet == "Type 24")
                    {
                        L_W_ = (f_l * 2) + (f_w * 4); //L'+W'

                    }

                    L_ = L_W_; //L'+W'
                    W2 = L_W_; //2W'
                    W2 = L_W_; //2W'
                    W2_2 = L_W_; //2W' check
                    qtyCartonPLay = Amount;

                }

                #endregion


                PalletL_L1 = L_W_ <= 300 ? PalletNormal_L : PalletOVERHANG_L; //L
                PalletL_W1 = PalletNormal_W; // W
                PalletL_L2 = W2_2 <= 300 ? PalletNormal_L : PalletOVERHANG_L;// 2W'|L
                PalletL_W2 = PalletOVERHANG_W;// 2W'|W
                //L_L_W = (int)Math.Floor(Convert.ToDouble(PalletL_L1 / L_W_));//L/(L'+W')
                L_L_W = (int)Math.Floor(Convert.ToDouble(PalletL_L1 / L_W_));//L/(L'+W')
                W_2W_L = (int)Math.Floor(Convert.ToDouble(PalletL_W1 / W2_2));//W/ 2W' OR L'
                L_2W_L = (int)Math.Floor(Convert.ToDouble(PalletOVERHANG_L / W2_2));//L/2W' OR L'
                W_L_W = (int)Math.Floor(Convert.ToDouble(PalletOVERHANG_W / L_W_));//W/(L'+W') 
                LPallet_L = L_L_W;    //หันด้านยาวไว้ด้าน L --L (Pallet)
                WPallet_L = W_2W_L;      //หันด้านยาวไว้ด้าน L --W (Pallet)
                LPallet_W = L_2W_L;     //หันด้านยาวไว้ด้าน L --L (Pallet)
                WPallet_W = W_L_W;    //หันด้านยาวไว้ด้าน L --W (Pallet)
                chkLPallet_L = (LPallet_L & WPallet_L) == 1 ? 1 : 0;
                chkLPallet_W = (LPallet_W & WPallet_W) == 1 ? 1 : 0;
                cartonperlayer1 = chkLPallet_L * qtyCartonPLay;
                cartonperlayer2 = chkLPallet_W * qtyCartonPLay;
                maxcartonperLayer = cartonperlayer1 >= cartonperlayer2 ? cartonperlayer1 : cartonperlayer2; // ===================== Max


                result = maxcartonperLayer;
            }
            catch
            {
                result = 0;
            }


            return result;
        }


    }
}



