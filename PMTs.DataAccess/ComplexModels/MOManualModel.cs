using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public class MOManualModel
    {
        public MoData MoData { get; set; }
        public MoSpec MoSpec { get; set; }
        public List<MoRouting> MoRoutings { get; set; }
    }
}
