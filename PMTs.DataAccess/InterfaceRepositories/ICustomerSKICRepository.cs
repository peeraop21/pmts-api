using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICustomerSKICRepository : IRepository<CustomerSkic>
    {
        IEnumerable<CustomerSkic> GetCustomerSKICAll();
        CustomerSkic GetCustomerSKICByCustId(string CustId);
    }
}