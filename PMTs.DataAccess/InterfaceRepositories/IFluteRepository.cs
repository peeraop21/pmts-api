using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IFluteRepository : IRepository<Flute>
    {
        Flute GetFluteByFlute(string factoryCode, string flute);
        FluteMaintainModel GetMaintainFlute(string factoryCode);
        bool AddMaintainFlute(FluteMaintainModel model);
        bool UpdateMaintainFlute(FluteMaintainModel model);
        IEnumerable<FluteAndMachineModel> GetFlutesAndMachinesByFactoryCode(string factoryCode, IConfiguration config);
        List<string> GetFlutesList(string factoryCode);
    }
}
