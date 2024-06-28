using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MasterUserRepository : Repository<MasterUser>, IMasterUserRepository
    {
        //private string connectionString;
        public MasterUserRepository(PMTsDbContext context) : base(context)
        {
        }
        //  return PMTsDbContext.MasterUser.ToList();

        //public MasterUserRole GetMasterUserRoleAll()
        public IEnumerable<MasterUserRole> GetMasterUserRoleAll(string factoryCode)
        {
            List<MasterUserRole> result = new List<MasterUserRole>();
            result = (from u in PMTsDbContext.MasterUser.Where(w => w.FactoryCode == factoryCode)
                      join r in PMTsDbContext.MasterRole on u.DefaultRoleId equals r.RoleId
                      select new MasterUserRole
                      {
                          Id = u.Id,

                          UserName = u.UserName,
                          Password = u.Password,
                          FirstNameTh = u.FirstNameTh,
                          LastNameTh = u.LastNameTh,
                          IsFlagDelete = u.IsFlagDelete,
                          RoleId = r.RoleId,
                          RoleName = r.RoleName
                      }).ToList();

            return result;
            //  return PMTsDbContext.MasterUser.Where(x => x.UserName == UserName && x.Password == Password && x.UserDomain == DomainName).FirstOrDefault();
            //  throw new NotImplementedException();
        }

        public MasterUser GetUsername(string UserName, string Password, string DomainName)
        {
            if (!string.IsNullOrEmpty(DomainName))
            {
                return PMTsDbContext.MasterUser.Where(x => x.UserName == UserName && x.UserDomain == DomainName).FirstOrDefault();
            }
            return PMTsDbContext.MasterUser.Where(x => x.UserName == UserName && x.Password == Password && x.UserDomain == DomainName).FirstOrDefault();
            //  throw new NotImplementedException();
        }

        //public MasterUser GetUsernameFinddomain(string UserName)
        //{

        //        return PMTsDbContext.MasterUser.Where(x => x.UserName == UserName).FirstOrDefault();

        //}


        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }


    }
}
