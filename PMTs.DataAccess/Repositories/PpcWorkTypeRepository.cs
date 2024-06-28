﻿using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class PpcWorkTypeRepository : Repository<PpcWorkType>, IPpcWorkTypeRepository
    {
        public PpcWorkTypeRepository(PMTsDbContext context)
             : base(context)
        {
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}