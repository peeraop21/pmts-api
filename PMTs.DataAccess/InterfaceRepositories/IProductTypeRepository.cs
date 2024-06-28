using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IProductTypeRepository : IRepository<ProductType>
    {
        ProductType GetProductTypeByBoxType(string productTypeName);
        IEnumerable<ProductType> GetProductTypesByHierarchyLv2s(List<string> lv2s);
        IEnumerable<ProductType> GetProductTypesByHierarchyLv2(string lv2s, string formgroup);
        List<ProductTypeOptionModel> GetProductTypesGroupByGroupProduct();
    }
}
