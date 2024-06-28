using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMachineGroupRepository : IRepository<MachineGroup>
    {
        IEnumerable<GroupMachineModels> GetMachineGroup(string FactoryCode);
    }
}
