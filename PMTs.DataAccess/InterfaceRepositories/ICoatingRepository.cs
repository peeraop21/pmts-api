using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICoatingRepository : IRepository<Coating>
    {

        IEnumerable<Coating> GetCoatingByMaterialNumber(string factoryCode, string materialNo);
    }
}
