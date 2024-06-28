using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PPCRawMaterialMasterRepository : Repository<PpcRawMaterialMaster>, IPPCRawMaterialMasterRepository
    {
        public PPCRawMaterialMasterRepository(PMTsDbContext context) : base(context)
        {

        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(string factoryCode, string materialNo, string materialDesc)
        {
            return PMTsDbContext.PpcRawMaterialMaster
                .Where(rmm =>
                        (rmm.MaterialNumber.Contains(materialNo) || rmm.MaterialDescription.Contains(materialDesc))
                        && rmm.Plant == factoryCode)
                .OrderByDescending(p => p.UpdateDate)
                .ThenByDescending(o => o.DateOfCreation)
                .Take(100)
                .ToList();

        }

        public IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMastersByFactoryCode(string factoryCode)
        {
            return PMTsDbContext.PpcRawMaterialMaster
                .Where(rmm => rmm.Plant.Equals(factoryCode))
                .OrderByDescending(p => p.UpdateDate)
                .ThenByDescending(o => o.DateOfCreation)
                .Take(100)
                .ToList();
        }

        public IEnumerable<PpcRawMaterialMaster> SearchPPCRawMaterialMasterByMaterialNo(string MaterialNo, string MaterialDesc)
        {
            return PMTsDbContext.PpcRawMaterialMaster.Where(x => x.MaterialNumber.Contains(MaterialNo) || x.MaterialDescription.Contains(MaterialDesc)).ToList();
        }
        public IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMasterByMaterialNo(string factoryCode, string MaterialNo)
        {
            return PMTsDbContext.PpcRawMaterialMaster.Where(x => x.MaterialNumber.Equals(MaterialNo) && x.Plant.Equals(factoryCode)).ToList();
        }
    }

}
