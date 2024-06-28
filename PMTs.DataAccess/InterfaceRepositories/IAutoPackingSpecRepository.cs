using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAutoPackingSpecRepository : IRepository<AutoPackingSpec>
    {
        IEnumerable<AutoPackingSpec> GetAutoPackingSpecByMaterialNo(string factoryCode, string materialNo);
        IEnumerable<AutoPackingSpec> CreateAutoPackingSpecs(List<AutoPackingSpec> autoPackingSpecs);
        void SaveAndUpdateAutoPackingSpecFromCusId(string factoryCode, string cusId, string username, string materialNo);
    }
}
