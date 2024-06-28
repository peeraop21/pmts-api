using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IOrderTrackingServiceRepository : IRepository<MasterData>
    {
        OrderTrackingServiceModel OrderTrackingService(IConfiguration config, string FactoryCode, string UpdateDateFrom, string UpdateDateTo);
        List<MoData> OrderTrackingServiceMoData(IConfiguration config, string FactoryCode, string UpdateDateFrom, string UpdateDateTo);
        List<MoSpec> OrderTrackingServiceMoSpect(IConfiguration config, string FactoryCode, List<string> orderTrackingServiceModel);
        List<MoRouting> OrderTrackingServiceMORouting(IConfiguration config, string FactoryCode, List<string> orderTrackingServiceModel);

        List<AllOrderTracking> GetAllOrderByDate(IConfiguration config, string UpdateDateFrom, string UpdateDateTo);
        OrderTrackingServiceModel GetMoByListOrderItems(IConfiguration config, string ListOrder);
    }
}
