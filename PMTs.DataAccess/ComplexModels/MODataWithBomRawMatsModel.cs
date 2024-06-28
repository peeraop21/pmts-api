using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.ComplexModels
{
    public class MODataWithBomRawMatsModel
    {
        public MoData MoData { get; set; }
        public IEnumerable<MoBomRawMat> MoBomRawMats { get; set; }
    }
}