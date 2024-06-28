using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMachineRepository : IRepository<Machine>
    {
        Machine GetMachineGroupByMachine(string factoryCode, string machine);
        IEnumerable<Machine> GetMachinesByPlanCodes(string factoryCode, List<string> planCodes);
        //IEnumerable<Machine> GetMachineHierarchy(string factoryCode, string hieLv2,MasterData masterData);

        List<Machine> GetMachineHierarchy(IConfiguration config, string factory, string hieLv2, MasterData masterData, string floxotype, string JoinType);
        List<Machine> GetMachineByMachineGroup(IConfiguration config, string factory, string machineGroup);
        void UpdateMachine(Machine machine);
        void CreateMachine(Machine machine);
        List<string> GetMachinesList(string factoryCode);
    }
}
