using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;

namespace PMTs.DataAccess.ComplexModels
{
    public class ReCalculateTrimModel : Routing
    {
        public string Flute { get; set; }
        public int? TrimOfFlute { get; set; }
        public string Board { get; set; }
        public int GroupPaperWidth { get; set; }
        public int? CutSheetWid { get; set; }
        public int CutOff { get; set; }
        public bool MinTrim { get; set; }
        public int PageMin { get; set; }//MinOut
        public int PageMinTrim { get; set; }
        public int PageMax { get; set; }
        public bool UpdateStatus { get; set; }
        public string ErrorMessase { get; set; }
    }

    public class ChangeReCalculateTrimModel
    {
        public string Flute { get; set; }
        public List<Routing> Routings { get; set; }
        public List<ReCalculateTrimModel> ReCalculateTrimModels { get; set; }
        public DataTable DataTable { get; set; }
    }
}
