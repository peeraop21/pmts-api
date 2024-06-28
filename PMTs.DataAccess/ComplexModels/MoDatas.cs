using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.ComplexModels
{
    public partial class MoDatas : MoData
    {
        public string PC { get; set; }
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }
    }
}
