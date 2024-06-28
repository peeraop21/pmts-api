using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPPCRawMaterialProductionBomRepository : IRepository<PpcRawMaterialProductionBom>
    {
        List<PpcRawMaterialProductionBom> GetPPCRawMaterialProductionBOMsByFgMaterial(string factoryCode, string fgMaterial);
        PpcRawMaterialProductionBom GetPPCRawMaterialProductionBOMByFgMaterialAndMaterialNo(string factoryCode, string fgMaterial, string materialNo);
    }
}
