using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPricingMasterRepository
    {
        List<PricingMaster> GetPricingMaster(IConfiguration config, string saleOrg, string materialNo);
    }
}
