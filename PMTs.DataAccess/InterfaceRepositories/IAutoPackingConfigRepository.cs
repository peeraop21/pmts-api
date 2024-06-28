using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAutoPackingConfigRepository : IRepository<AutoPackingConfig>
    {
        AutoPackingConfig GetAutoPackingConfigByID(string factoryCode, string iD);
    }
}
