using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class KindOfProductGroupRepository(PMTsDbContext context) : Repository<KindOfProductGroup>(context), IKindOfProductGroupRepository
    {
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<KindOfProductGroup> GetKindOfProductGroupsByIds(List<string> idKindOfProductGroups)
        {
            var kindOfProductGroups = new List<KindOfProductGroup>();

            foreach (var idKindOfProductGroup in idKindOfProductGroups)
            {
                kindOfProductGroups.AddRange(PMTsDbContext.KindOfProductGroup.Where(k => k.Id == Convert.ToInt32(idKindOfProductGroup)).AsNoTracking().ToList());
            }

            return kindOfProductGroups;
        }
    }
}
