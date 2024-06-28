using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMainmenusRepository : IRepository<MainMenus>
    {
        IEnumerable<MainMenus> GetMainmenuAll();
        IEnumerable<MainMenus> GetMainMenuByRoleId(int roleId);

        //Tassanai Update 03/04/2020
        IEnumerable<MainMenuAllByRoles> GetMainMenuAllByRoleId(int roleId);

    }
}
