using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CustomerSKICRepository : Repository<CustomerSkic>, ICustomerSKICRepository
    {
        public CustomerSKICRepository(PMTsDbContext context) : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
        public IEnumerable<CustomerSkic> GetCustomerSKICAll()
        {
            return PMTsDbContext.CustomerSkic.ToList();
        }

        public CustomerSkic GetCustomerSKICByCustId(string CustId)
        {
            var cust = PMTsDbContext.CustomerSkic.FirstOrDefault(w => w.CustomerNo.Equals(CustId));
            if (cust == null)
            {
                cust = new CustomerSkic();
            }
            return cust;
        }

    }
}