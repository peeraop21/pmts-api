using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPresaleChangeRoutingRepository : IRepository<PresaleChangeRouting>
    {
        IEnumerable<PresaleChangeRouting> GetPresaleChangeRoutingByPsmId(string psmId);

        void UpdatePresaleRoutings(List<PresaleChangeRouting> presaleChangeRoutings);
    }
}
