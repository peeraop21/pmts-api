using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPMTsConfigRepository : IRepository<PmtsConfig>
    {
        PmtsConfig GetSlit(string FactoryCode);
        PmtsConfig GetPMTsConfigByFactoryName(string FactoryCode, string funcName);

        PmtsConfig GetPMTsConfigByFucName(string FactoryCode, string funcName);
    }
}
