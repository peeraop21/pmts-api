﻿using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class JoinCharacterRepository : Repository<JoinCharacter>, IJoinCharacterRepository
    {
        public JoinCharacterRepository(PMTsDbContext context)
          : base(context)
        {
        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}