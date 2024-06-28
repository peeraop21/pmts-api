using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public partial class ParamCalPaperWidth
    {
        public string Machine { get; set; }
        public string FactoryCode { get; set; }
        public string Flute { get; set; }
        public int SheetInWid { get; set; }
        public string MaterialNo { get; set; }
        IEnumerable<string> PaperItem { get; set; }
        public int TrimOfFlute { get; set; }

        public string PaperWidth { get; set; }
        public string CutNo { get; set; }
        public string Trim { get; set; }
        public string PercenTrim { get; set; }
    }

    public partial class ReturnCalPaperWidth
    {
        public string MaterialNo { get; set; }
        public string MachineName { get; set; }
        public int SheetInWid { get; set; }
        public string Flute { get; set; }

        public string PaperWidthOld { get; set; }
        public string CutOld { get; set; }
        public string TrimOld { get; set; }
        public string PercentTrimOld { get; set; }

        public string PaperWidth { get; set; }
        public string Cut { get; set; }
        public string Trim { get; set; }
        public string PercentTrim { get; set; }
    }
}
