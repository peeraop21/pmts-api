using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMasterUserRepository : IRepository<MasterUser>
    {
        MasterUser GetUsername(string UserName, string Password, string DomainName);
        IEnumerable<MasterUserRole> GetMasterUserRoleAll(string factoryCode);
        // MasterUser GetUsernameFinddomain(string UserName);


    }
}
