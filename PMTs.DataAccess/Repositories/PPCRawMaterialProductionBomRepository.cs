using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PPCRawMaterialProductionBomRepository : Repository<PpcRawMaterialProductionBom>, IPPCRawMaterialProductionBomRepository
    {
        public PPCRawMaterialProductionBomRepository(PMTsDbContext context) : base(context)
        {

        }
        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<PpcRawMaterialProductionBom> GetPPCRawMaterialProductionBOMsByFgMaterial(string factoryCode, string fgMaterial)
        {
            var result = new List<PpcRawMaterialProductionBom>();

            if (!string.IsNullOrEmpty(fgMaterial))
            {
                result = PMTsDbContext.PpcRawMaterialProductionBom.Where(p => p.FgMaterial.Equals(fgMaterial)).ToList();
            }

            return result;
        }

        public PpcRawMaterialProductionBom GetPPCRawMaterialProductionBOMByFgMaterialAndMaterialNo(string factoryCode, string fgMaterial, string materialNo)
        {
            var result = new PpcRawMaterialProductionBom();

            if (!string.IsNullOrEmpty(fgMaterial))
            {
                result = PMTsDbContext.PpcRawMaterialProductionBom.AsNoTracking().FirstOrDefault(p => p.FgMaterial.Equals(fgMaterial) && p.MaterialNumber.Equals(materialNo));
            }

            return result;
        }
    }
}
