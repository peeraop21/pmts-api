using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ISetCategoriesOldMatRepository : IRepository<SetCategoriesOldMat>
    {
        IEnumerable<SetCategoriesOldMat> GetSetCategoriesOldMatByLV2(string LV2);
        IEnumerable<SetCategoriesOldMat> GetCategoriesMatrixByLV2(IConfiguration config, string LV2);
    }
}
