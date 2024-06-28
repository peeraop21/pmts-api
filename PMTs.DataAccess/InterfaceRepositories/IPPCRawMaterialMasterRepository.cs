using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPPCRawMaterialMasterRepository : IRepository<PpcRawMaterialMaster>
    {
        IEnumerable<PpcRawMaterialMaster> SearchPPCRawMaterialMasterByMaterialNo(string MaterialNo, string MaterialDesc);
        IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(string factoryCode, string materialNo, string materialDesc);
        IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMastersByFactoryCode(string factoryCode);
        IEnumerable<PpcRawMaterialMaster> GetPPCRawMaterialMasterByMaterialNo(string factoryCode, string MaterialNo);
    }
}
