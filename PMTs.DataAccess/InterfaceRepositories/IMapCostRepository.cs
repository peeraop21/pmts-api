using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMapCostRepository : IRepository<MapCost>
    {
        IEnumerable<MapCost> GetMapCostAll();
        MapCost GetCostField(string lv2, string lv3, string lv4);
    }
}
