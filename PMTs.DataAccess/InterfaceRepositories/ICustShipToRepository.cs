using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICustShipToRepository : IRepository<CustShipTo>
    {
        IEnumerable<CustShipTo> GetCustShipToListByCustCode(string custCode);
        IEnumerable<CustShipTo> GetCustShipToListByShipTo(string shipTo);
    }
}