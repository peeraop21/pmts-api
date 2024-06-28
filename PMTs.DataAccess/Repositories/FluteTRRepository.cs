using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class FluteTRRepository : Repository<FluteTr>, IFluteTRRepository
    {
        public FluteTRRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<FluteTr> GetFluteTrByFlute(string factoryCode, string flute)
        {
            return PMTsDbContext.FluteTr.Where(f => f.FluteCode == flute && f.FactoryCode == factoryCode).OrderBy(f => f.Item).ToList();
        }

        public IEnumerable<FluteTr> GetFluteTrsByFlutes(string factoryCode, List<string> flutes)
        {
            var fluteTrs = new List<FluteTr>();
            fluteTrs.AddRange(PMTsDbContext.FluteTr.Where(f => flutes.Contains(f.FluteCode) && f.FactoryCode == factoryCode).AsNoTracking().ToList());

            return fluteTrs.OrderBy(f => f.Item).ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
