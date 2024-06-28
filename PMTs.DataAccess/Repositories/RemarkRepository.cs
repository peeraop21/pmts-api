using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class RemarkRepository : Repository<Remark>, IRemarkRepository
    {
        public RemarkRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<Remark> GetRemarksBypcs(string factoryCode, List<string> pc)
        {
            var remark = new List<Remark>();

            if (pc != null && pc.Count > 0)
            {
                remark.AddRange(PMTsDbContext.Remark.Where(m => m.FactoryCode == factoryCode && pc.Contains(m.Pc)).AsNoTracking().ToList());
            }

            return remark;
        }
    }
}
