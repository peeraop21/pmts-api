using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    class MainmenusRepository : Repository<MainMenus>, IMainmenusRepository
    {
        public MainmenusRepository(PMTsDbContext context) : base(context)
        {

        }

        public IEnumerable<MainMenus> GetMainmenuAll()
        {
            return PMTsDbContext.MainMenus.ToList();
        }

        public IEnumerable<MainMenus> GetMainMenuByRoleId(int roleId)
        {

            return (from m in PMTsDbContext.MainMenus
                    join me in PMTsDbContext.MenuRole on m.Id equals me.IdMenu
                    where me.IdRole == roleId
                    orderby m.SortMenu ascending
                    select new MainMenus
                    {
                        Id = m.Id,
                        MenuNameTh = m.MenuNameTh,
                        MenuNameEn = m.MenuNameEn,
                        Controller = m.Controller,
                        Action = m.Action,
                        IconName = m.IconName
                    }).ToList();
        }

        public IEnumerable<MainMenuAllByRoles> GetMainMenuAllByRoleId(int roleId)
        {

            var query = from m in PMTsDbContext.MainMenus
                        join r in PMTsDbContext.MenuRole.Where(x => x.IdRole == roleId)
                       on m.Id equals r.IdMenu into g
                        select new MainMenuAllByRoles
                        {
                            Id = m.Id,
                            MenuNameTh = m.MenuNameTh,
                            MenuNameEn = m.MenuNameEn,
                            Controller = m.Controller,
                            Action = m.Action,
                            IconName = m.IconName,
                            RoleId = (from x in g select x.IdRole).FirstOrDefault(),
                            IdmenuRole = (from x in g select x.Id).FirstOrDefault()
                        };
            return query.ToList();

            //return (from m in PMTsDbContext.MainMenus 
            //        join r in PMTsDbContext.MenuRole on m.Id equals r.IdMenu into mr
            //        from r in mr.DefaultIfEmpty()
            //        where r.IdRole == roleId
            //        orderby m.SortMenu ascending
            //        select new MainMenuAllByRoles
            //        {
            //            Id = m.Id,
            //            MenuNameTh = m.MenuNameTh,
            //            MenuNameEn = m.MenuNameEn,
            //            Controller = m.Controller,
            //            Action = m.Action,
            //            IconName = m.IconName,
            //            RoleId = r.IdRole
            //        }).ToList();

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
