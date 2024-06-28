using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MoDataLogRepository : Repository<MoDatalog>, IMoDataLogRepository
    {
        public MoDataLogRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<MoDatalog> GetMoDatalogListBySO(string factoryCode, string stratSO, string endSO)
        {
            return PMTsDbContext.MoDatalog.Where(m => m.FactoryCode == factoryCode && (m.OrderItem.CompareTo(stratSO) >= 0 && m.OrderItem.CompareTo(endSO) <= 0)).AsNoTracking().ToList();
        }
    }
}
