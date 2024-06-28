using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.WebAPI.Models
{
    public class PresaleModel
    {
        public PresaleChangeProduct presaleChange { get; set; }
        public List<PresaleChangeRouting> presaleRouting { get; set; }

    }

    //public class MasterDatas
    //{
    //    public string Description { get; set; }
    //    public string SaleText1 { get; set; }
    //    public string SaleText2 { get; set; }
    //    public string SaleText3 { get; set; }
    //    public string SaleText4 { get; set; }
    //    public int? PieceSet { get; set; }
    //    public string PrintMethod { get; set; }
    //    public string HighGroup { get; set; }
    //    public string HighValue { get; set; }
    //    public int? Bun { get; set; }
    //    public int? BunLayer { get; set; }
    //    public int? LayerPalet { get; set; }
    //}

    //public class Routings
    //{
    //    public string Machine { get; set; }
    //    public string Color1 { get; set; }
    //    public string Shade1 { get; set; }
    //    public string Color2 { get; set; }
    //    public string Shade2 { get; set; }
    //    public string Color3 { get; set; }
    //    public string Shade3 { get; set; }
    //    public string Color4 { get; set; }
    //    public string Shade4 { get; set; }
    //    public string Color5 { get; set; }
    //    public string Shade5 { get; set; }
    //    public string Color6 { get; set; }
    //    public string Shade6 { get; set; }
    //    public string Color7 { get; set; }
    //    public string Shade7 { get; set; }
    //}


}
