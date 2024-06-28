using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IFSCCodeRepository : IRepository<PpcFscCode>
    {
        PpcFscCode GetFSCCodeByFSCCode(string fSCCode);
    }
}
