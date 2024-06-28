using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IKindOfProductRepository : IRepository<KindOfProduct>
    {
        IEnumerable<KindOfProduct> GetKindOfProductsByIds(List<string> idKindOfProducts);
    }
}
