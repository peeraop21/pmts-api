using System;

namespace PMTs.DataAccess.ComplexModels
{
    public class ReCalculateTrimResultModel
    {
        public int Id { get; set; }
        public string MaterialNo { get; set; }
        public string Flute { get; set; }
        public string PlanCode { get; set; }
        public string Machine { get; set; }
        public int? Trim { get; set; }
        public double? PercenTrim { get; set; }
        public int? PaperWidth { get; set; }
        public int? CutNo { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
