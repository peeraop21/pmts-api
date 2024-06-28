using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public ProductType GetProductTypeByBoxType(string productTypeName)
        {
            return PMTsDbContext.ProductType.FirstOrDefault(w => w.Name.Equals(productTypeName));
        }

        public IEnumerable<ProductType> GetProductTypesByHierarchyLv2(string lv2s, string formgroup)
        {

            return PMTsDbContext.ProductType.Where(m => m.HierarchyLv2 == lv2s && m.FormGroup == formgroup).ToList();

        }
        public List<ProductTypeOptionModel> GetProductTypesGroupByGroupProduct()
        {       
            var result = PMTsDbContext.ProductType.GroupBy(g => g.GroupProduct.Replace("\r\n","").Trim())
                .Select(s => new ProductTypeOptionModel
                {
                    IdKindOfProduct = s.Where(w => w.GroupProduct == s.Key).Select(s => s.Id).FirstOrDefault(),
                    KindOfProductName = s.Key,
                    GroupPriority = s.Where(w => w.GroupProduct == s.Key).Select(s => s.GroupPriority).FirstOrDefault(),
                    ProductTypeOptions = s.Select(x => new ProductTypeOption
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.HierarchyLv2
                    }).OrderBy(xo => xo.Id).ToList()
                }).OrderBy(o => o.IdKindOfProduct).ToList();
            return result;
        }

        public IEnumerable<ProductType> GetProductTypesByHierarchyLv2s(List<string> lv2s)
        {
            var productTypes = new List<ProductType>();
            productTypes.AddRange(PMTsDbContext.ProductType.Where(p => lv2s.Contains(p.HierarchyLv2)).AsNoTracking().ToList());
            return productTypes;
        }
    }
}
