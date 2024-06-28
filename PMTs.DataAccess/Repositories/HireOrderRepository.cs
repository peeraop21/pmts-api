using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class HireOrderRepository : Repository<HireOrder>, IHireOrderRepository
    {
        public HireOrderRepository(PMTsDbContext context) : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<HireOrder> GetHireOrder(int group)
        {
            return PMTsDbContext.HireMapping
            .Where(m => m.GroupBu == group)
            .Join(PMTsDbContext.HireOrder, m => m.OrderId, o => o.Id, (m, o) => new { m, o })
            .Select(x => new HireOrder
            {
                Id = x.o.Id,
                OrderType = x.o.OrderType,
                EmployeeChannel = x.o.EmployeeChannel,
                SyncMat = x.o.SyncMat
            }).ToList();
        }
    }
}
