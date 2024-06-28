using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MoRoutingRepository : Repository<MoRouting>, IMoRoutingRepository
    {
        public MoRoutingRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public int CheckExistBlockNo(string factoryCode, string SO, string blockNo)
        {
            return PMTsDbContext.MoRouting.Where(m => m.FactoryCode == factoryCode && m.OrderItem != SO && m.BlockNo == blockNo).Count();
        }

        public IEnumerable<MoRouting> GetMORoutingsBySaleOrders(string factoryCode, List<string> saleOrders)
        {
            var moRoutings = new List<MoRouting>();
            moRoutings.AddRange(PMTsDbContext.MoRouting.Where(m => m.FactoryCode == factoryCode && saleOrders.Contains(m.OrderItem)).AsNoTracking().ToList());

            return moRoutings;
        }
        public IEnumerable<MoRouting> GetMORoutingsBySBOExNo(string factoryCode, string orderItem)
        {
            var moRoutings = new List<MoRouting>();
            if (!string.IsNullOrEmpty(orderItem))
            {
                moRoutings.AddRange(PMTsDbContext.MoRouting.Where(m => m.FactoryCode == factoryCode && m.SboExternalNumber.Equals(orderItem)).AsNoTracking().ToList());
            }
            return moRoutings;
        }
    }
}
