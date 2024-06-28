using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IRunningNoRepository : IRepository<RunningNo>
    {
        RunningNo GetRunningNoByGroupId(string factoryCode, string groupId);
    }
}
