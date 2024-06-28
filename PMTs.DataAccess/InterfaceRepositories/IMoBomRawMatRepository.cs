using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMoBomRawMatRepository : IRepository<MoBomRawMat>
    {
        List<MoBomRawMat> GetMoBomRawMatByOrderItem(string factoryCode, string orderItem);
        IEnumerable<MoBomRawMat> GetMoBomRawMatsByFgMaterial(string factoryCode, string fgMaterial, string orderItem);
        IEnumerable<MoBomRawMat> GetMoBomRawMatsByFactoryCode(string factoryCode);
        IEnumerable<MoBomRawMat> GetMoBomRawMatsByOrderItems(IConfiguration config, List<string> orderItems);
    }
}
