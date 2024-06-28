using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMoSpecRepository : IRepository<MoSpec>
    {
        MoSpec GetMoSpecBySaleOrder(string factoryCode, string orderItem);
        MoSpec GetMoSpecByOrderItem(string factoryCode, string orderItem);
        MoSpec GetMoSpecBySuffixSO(string factoryCode, string SO);

        string GetMoSpecDiecutPath(string factoryCode, string orderItem);
        string GetMoSpecPalletPath(string factoryCode, string orderItem);
        IEnumerable<MoSpec> GetMOSpecsBySaleOrders(string factoryCode, List<string> saleOrders);

        void UpdateMoSpecChange(string FactoryCode, string OrderItem, string ChangeInfo);
        IEnumerable<MasterData> GetMatNoOrder(IConfiguration config, string factoryCode, string custCode, string yearCreate, string yearUpdate);
    }
}
