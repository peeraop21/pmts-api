using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IVMIServiceRepository : IRepository<MasterData>
    {
        List<VMIAllMasterDataModel> GetAllMasterDataByCustInvType(IConfiguration config, string CustInvType);
        List<MasterData> GetMasterDataByCustInvType(IConfiguration config, string CustInvType);
        List<Routing> GetRoutingByListMaterialNo(IConfiguration config, string MaterialList);
        List<MoData> GetMoDataByListMaterialNo(IConfiguration config, string MaterialList);

        //Tassanai Update
        List<BomStruct> GetBomStructs(IConfiguration config, string MaterialList);

    }
}
