using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPresaleChangeProductRepository : IRepository<PresaleChangeProduct>
    {
        PresaleChangeProduct GetPresaleChangeProductByPsmId(string psmId);
        IEnumerable<PresaleChangeProduct> GetPresaleChangeProductsByKeySearch(IConfiguration config, string factoryCode, string typeSearch, string keySearch);
        IEnumerable<PresaleChangeProduct> GetPresaleChangeProductsByActiveStatus(IConfiguration config, string factoryCode);
    }
}
