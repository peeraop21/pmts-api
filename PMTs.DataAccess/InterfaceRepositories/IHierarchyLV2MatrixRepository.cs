using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IHierarchyLV2MatrixRepository : IRepository<HierarchyLv2Matrix>
    {
        List<ProductTypeOptionModel> GetHierarchyLv2MatrixGroupByKindOfProduct();
    }
}
