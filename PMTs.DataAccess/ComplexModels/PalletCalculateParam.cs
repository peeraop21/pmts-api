namespace PMTs.DataAccess.ComplexModels
{
    public partial class PalletCalculateParam
    {
        public string FormGroup { get; set; }
        public string RSCStyle { get; set; }
        public string Flute { get; set; }
        public int WidDC { get; set; }
        public int LegDC { get; set; }
        public int? Hig { get; set; }
        public string palletSizeFilter { get; set; }
        public int Overhang { get; set; }
        public int CutSheetWid { get; set; }
        public int CutSheetLeng { get; set; }
        public string JoinTypeFilter { get; set; }
        public int? ScoreL6 { get; set; }
    }

    //tasanai update 25052021
    public partial class PalletCalculateParamMat
    {

        public string FactoryCode { get; set; }
        public string MaterialNo { get; set; }
        public string FormGroup { get; set; }
        public string RSCStyle { get; set; }
        public string Flute { get; set; }

        public int? Hig { get; set; }
        public string palletSizeFilter { get; set; }
        public int Overhang { get; set; }
        public int CutSheetWid { get; set; }
        public int CutSheetLeng { get; set; }
        public string JoinTypeFilter { get; set; }
        public int? ScoreL6 { get; set; }


    }

    public class DataPallet
    {
        public string PicPallet { get; set; }
        public string BunLayer { get; set; }
    }

}
