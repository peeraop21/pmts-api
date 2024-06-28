using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IRoutingRepository : IRepository<Routing>
    {
        IEnumerable<Routing> GetRoutingByMaterialNo(string factoryCode, string materialNo);
        IEnumerable<Routing> GetRoutingByMaterialNoFactorycodeAndPlant(string factoryCode, string plant, string materialNo);

        int GetNumberOfRoutingByShipBlk(string factoryCode, string materialNo, bool semiBlk);

        //IEnumerable<Routing> GetRoutingByhandshake(string factoryCode, string materialNo);

        IEnumerable<Routing> GetRoutingByhandshake(IConfiguration config, string factoryCode, string materialNo);

        void UpdatePdisStatus(string FactoryCode, string MaterialNo, string Status);

        List<Routing> GetDapperRoutingByMat(IConfiguration config, string fac, string condition);

        List<Routing> GetRoutingsByMaterialNos(string factoryCode, List<string> materialNOs);

        void UpdateRoutings(List<Routing> routings);
        void UpdatePdisStatusEmployment(string FactoryCode, string MaterialNo, string SaleOrg, string username, string Status);
        List<ReCalculateTrimModel> UpdateReCalculateTrim(IConfiguration configuration, string factoryCode, List<ReCalculateTrimModel> reCalculateTrimModels);
        IEnumerable<Routing> GetRoutingsByMaterialNoContain(string factoryCode, string materialNo);
        IEnumerable<Routing> GetRoutingListByDateTime(string factoryCode, string DateFrom, string DateTo);
    }
}
