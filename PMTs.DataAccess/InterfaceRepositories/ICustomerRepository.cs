using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetCustomerByCusID(string factoryCode, string cusID);
        Customer GetCustomerByCustIdAndCustName(string FactoryCode, string custId, string custName);
        IEnumerable<CustomerShipToViewModel> GetCustomerShipToByCustCode(string factoryCode, string custCode);
        List<Customer> GetCustomerById(string factoryCode, int Id);
        IEnumerable<CustomerShipToViewModel> GetCustomerShipTo(string factoryCode);
        List<Customer> GetCustomersByCusId(string factoryCode, string cusId);
        IEnumerable<Customer> selectAllcus();
        IEnumerable<Customer> selectAllcusNoParalel();

        IEnumerable<CustomerShipToViewModel> GetCustomerShipToByParalel(string factoryCode);

        IEnumerable<CustomerShipToViewModel> GetCustomerByCustname(string FactoryCode, string CustName);
        IEnumerable<CustomerShipToViewModel> GetCustomerByCustCode(string FactoryCode, string custCode);
        IEnumerable<CustomerShipToViewModel> GetCustomerByCustId(string FactoryCode, string custId);

        void DeleteCustomerByID(int ID);
        IEnumerable<Customer> GetCustomersByCustomerGroup(string factoryCode);
    }
}