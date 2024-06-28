using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IConfigWordingReportRepository : IRepository<ConfigWordingReport>
    {
        ConfigWordingReport UpdateConfigWordingReportByFactoryCode(string factoryCode, string username, string configWording);
    }
}
