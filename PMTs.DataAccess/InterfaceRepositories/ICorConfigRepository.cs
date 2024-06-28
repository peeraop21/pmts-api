using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICorConfigRepository : IRepository<CorConfig>
    {
        CorConfig GetPMTsConfigByFactoryName(string factoryCode);
        CorConfig GetPMTsConfigByMachine(string factoryCode, string Machine);
    }
}
