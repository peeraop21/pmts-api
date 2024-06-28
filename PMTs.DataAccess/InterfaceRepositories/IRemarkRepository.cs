using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IRemarkRepository : IRepository<Remark>
    {
        IEnumerable<Remark> GetRemarksBypcs(string factoryCode, List<string> pc);
    }
}
