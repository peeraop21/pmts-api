using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class SubMenusRepository : Repository<SubMenus>, ISubMenusRepository
    {
        public SubMenusRepository(PMTsDbContext context) : base(context)
        {

        }
        public IEnumerable<SubMenus> GetSubMenusAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SubMenus> GetSubMenusListBYRole(int roleId)
        {
            return (from m in PMTsDbContext.SubMenus
                    join me in PMTsDbContext.SubMenurole on m.Id equals me.IdSubMenuRole
                    where me.IdRole == roleId
                    orderby m.MainMenuId, m.SeqNo
                    select new SubMenus
                    {
                        SubMenuName = m.SubMenuName,
                        Controller = m.Controller,
                        Action = m.Action,
                        MainMenuId = m.MainMenuId,
                        SeqNo = m.SeqNo

                    }).ToList();
        }

        //Tassanai Update 10/04/2020

        public IEnumerable<SubMenusAllByRoles> GetSubMenusAllListBYRole(int roleId)
        {


            var query = from m in PMTsDbContext.SubMenus
                        join r in PMTsDbContext.SubMenurole.Where(x => x.IdRole == roleId)
                       on m.Id equals r.IdSubMenuRole into g
                        select new SubMenusAllByRoles
                        {
                            Id = m.Id,
                            SubMenuName = m.SubMenuName,
                            Controller = m.Controller,
                            Action = m.Action,
                            MainMenuId = m.MainMenuId,
                            SubMenuroleID = (from x in g select x.Id).FirstOrDefault(),
                            IdSubMenuRole = (from x in g select x.IdSubMenuRole).FirstOrDefault(),
                            Idrole = (from x in g select x.IdRole).FirstOrDefault(),
                            Idmenu = (from x in g select x.IdMenu).FirstOrDefault()

                        };
            return query.ToList();


            //var query = from m in PMTsDbContext.SubMenus 
            //            join r in PMTsDbContext.SubMenurole.Where(x => x.IdRole == roleId)
            //           on m.Id equals r.IdMenu into g
            //            select new SubMenusAllByRoles
            //            {
            //                Id = m.Id,

            //                Action = m.Action

            //            };
            //return query.ToList();



        }


        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
