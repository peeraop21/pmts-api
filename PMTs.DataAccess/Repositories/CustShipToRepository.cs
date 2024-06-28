using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CustShipToRepository : Repository<CustShipTo>, ICustShipToRepository
    {
        public CustShipToRepository(PMTsDbContext context) : base(context)
        {
        }

        public IEnumerable<CustShipTo> GetCustShipToAll()
        {
            return PMTsDbContext.CustShipTo.ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<CustShipTo> GetCustShipToListByCustCode(string custCode)
        {
            return PMTsDbContext.CustShipTo.Where(w => w.CustCode.Equals(custCode)).ToList();
        }

        public IEnumerable<CustShipTo> GetCustShipToListByShipTo(string shipTo)
        {
            return PMTsDbContext.CustShipTo.Where(w => w.ShipTo.Equals(shipTo)).ToList();
        }
    }
}