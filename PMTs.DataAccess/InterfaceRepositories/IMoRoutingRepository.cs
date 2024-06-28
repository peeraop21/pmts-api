using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMoRoutingRepository : IRepository<MoRouting>
    {
        int CheckExistBlockNo(string factoryCode, string SO, string blockNo);
        IEnumerable<MoRouting> GetMORoutingsBySaleOrders(string factoryCode, List<string> saleOrders);
        IEnumerable<MoRouting> GetMORoutingsBySBOExNo(string factoryCode, string orderItem);
    }
}
