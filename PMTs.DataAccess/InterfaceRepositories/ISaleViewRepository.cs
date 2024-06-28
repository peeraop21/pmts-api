using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ISaleViewRepository : IRepository<SalesView>
    {
        SalesView GetSaleViewByMaterialNo(string factoryCode, string materialNo);
        IEnumerable<SalesView> GetSaleViewsByMaterialNo(string factoryCode, string materialNo);
        SalesView GetSaleViewBySaleOrg(string factoryCode, string materialNo, string saleOrg);
        IEnumerable<SalesView> GetSaleViewsByMaterialNoAndFactoryCode(string factoryCode, string materialNo);
        // SalesView GetSaleViewByhandshake(string factoryCode, string materialNo);

        IEnumerable<SalesView> GetSaleViewByhandshake(SqlConnection conn, string FactoryCode, string materialNo);

        SalesView GetSaleViewBySaleOrgChannel(string factoryCode, string materialNo, string saleOrg, byte channel);
        IEnumerable<SalesView> GetReuseSaleViewsByMaterialNos(string factoryCode, List<string> materialNos);
    }
}
