using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ITruckOptimizeRepository : IRepository<TruckOptimize>
    {
        IEnumerable<TruckOptimize> GetTruckOptimizeByMaterialNo(string factoryCode, string materialNo);

        IEnumerable<TruckOptimize> CreateTruckOptimizes(List<TruckOptimize> truckOptimizes);

    }
}
