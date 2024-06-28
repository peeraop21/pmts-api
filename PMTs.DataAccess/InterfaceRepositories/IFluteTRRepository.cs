using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IFluteTRRepository : IRepository<FluteTr>
    {
        IEnumerable<FluteTr> GetFluteTrByFlute(string factoryCode, string flute);
        IEnumerable<FluteTr> GetFluteTrsByFlutes(string factoryCode, List<string> flutes);
    }
}
