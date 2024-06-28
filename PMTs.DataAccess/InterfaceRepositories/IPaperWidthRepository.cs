using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPaperWidthRepository : IRepository<PaperWidth>
    {
        List<PaperWidth> GetPaperWidth(string factoryCode);
    }
}
