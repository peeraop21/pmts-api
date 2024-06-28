using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ISubMenusRepository : IRepository<SubMenus>
    {
        IEnumerable<SubMenus> GetSubMenusAll();
        IEnumerable<SubMenus> GetSubMenusListBYRole(int roleId);
        IEnumerable<SubMenusAllByRoles> GetSubMenusAllListBYRole(int roleId);






    }

}
