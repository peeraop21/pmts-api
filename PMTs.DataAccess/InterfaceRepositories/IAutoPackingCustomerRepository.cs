using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAutoPackingCustomerRepository : IRepository<AutoPackingCustomer>
    {
        AutoPackingCustomer GetAutoPackingCustomerByCusID(string factoryCode, string cusID);
        List<AutoPackingCustomerData> GetAllAutoPackingCustomerAndCustomer(string FactoryCode);
        List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCustName(string FactoryCode, string KeySearch);
        List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCustCode(string FactoryCode, string KeySearch);
        List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCusId(string FactoryCode, string KeySearch);
        void UpdateAutoPackingCustomer(AutoPackingCustomer model);
    }
}
