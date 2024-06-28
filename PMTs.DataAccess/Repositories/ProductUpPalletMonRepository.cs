using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class ProductUpPalletMonRepository : Repository<ProductUpPalletMon>, IProductUpPalletMonRepository
    {
        public ProductUpPalletMonRepository(PMTsDbContext context)
            : base(context)
        {
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        //public List<PalletCalculateParamMat> GetmasterdataAndPallet()
        //{


        //    return PMTsDbContext.ProductUpPalletMon.Join(PMTsDbContext.MasterData,
        //        p => p.MaterialNo,
        //        m => m.MaterialNo,
        //        (p, m) => new { p, m }).Where(x => x.p.StatusUpdate == null && x.m.FactoryCode == "253B" && x.p.Flute != null).Select(x => new PalletCalculateParamMat
        //        {
        //            //FactoryCode = x.m.FactoryCode,
        //            MaterialNo = x.p.MaterialNo,
        //            FormGroup = "STDRSC",
        //            RSCStyle = "",
        //            Flute = x.p.Flute,
        //            Hig = Convert.ToInt32(x.p.Hig),
        //            palletSizeFilter = "1.00x1.20",
        //            Overhang = 0,
        //            CutSheetWid = Convert.ToInt32(x.p.CutSheetWid),
        //            CutSheetLeng = Convert.ToInt32(x.m.ScoreL6),
        //            JoinTypeFilter = "GIL",
        //            ScoreL6 = x.m.ScoreL6
        //        }).Take(100).ToList();
        //}
    }
}
