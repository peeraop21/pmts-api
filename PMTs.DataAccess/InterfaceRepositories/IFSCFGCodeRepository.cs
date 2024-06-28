using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IFSCFGCodeRepository : IRepository<PpcFscFgCode>
    {
        PpcFscFgCode GetFSCFGCodeByFSCFGCode(string fSCFGCode);
    }
}
