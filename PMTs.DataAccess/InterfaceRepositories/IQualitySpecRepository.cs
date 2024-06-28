using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IQualitySpecRepository : IRepository<QualitySpec>
    {
        IEnumerable<QualitySpec> GetQualitySpecsByMaterialNos(string factoryCode, List<string> materialNos);
    }
}
