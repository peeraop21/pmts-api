using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ITransactionsDetailRepository : IRepository<TransactionsDetail>
    {
        TransactionsDetail GetTransactionsDetailByMaterialNo(string factoryCode, string materialNo);
        TransactionsDetail GetSelectedFirstOutsourceByMaterialNo(IConfiguration configuration, string factoryCode, string materialNo);
        IEnumerable<TransactionsDetail> GetTransactionsDetailsByMaterialNOs(string factoryCode, List<string> materialNOs);
        string GetAllMatOutsourceByMaterialNo(IConfiguration configuration, string factoryCode, string materialNo);
        TransactionsDetail GetMatOutsourceByMatSaleOrg(IConfiguration configuration, string factoryCode, string materialNo);
    }
}
