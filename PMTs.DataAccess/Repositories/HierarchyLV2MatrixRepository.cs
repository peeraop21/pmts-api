using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class HierarchyLv2MatrixRepository : Repository<HierarchyLv2Matrix>, IHierarchyLV2MatrixRepository
    {
        public HierarchyLv2MatrixRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<HierarchyLv2Matrix> GetHierarchyLv2MatrixAll()
        {
            return PMTsDbContext.HierarchyLv2Matrix.ToList();
        }

        public List<ProductTypeOptionModel> GetHierarchyLv2MatrixGroupByKindOfProduct()
        {
            var result = PMTsDbContext.HierarchyLv2Matrix.OrderBy(o => o.SortIndex)
                .Join(PMTsDbContext.ProductType, (h => h.IdProductType), (p => p.Id), (h, p) => new { HierarchyLv2Matrix = h, h.IdProductType, ProductTypeName = p.Name })
                .Join(PMTsDbContext.KindOfProduct, (h => h.HierarchyLv2Matrix.IdKindOfProduct), (k => k.Id), (h, k) => new { HierarchyLv2Matrix = h.HierarchyLv2Matrix, h.IdProductType, h.ProductTypeName, h.HierarchyLv2Matrix.IdKindOfProduct, KindOfProductName = k.Name })
                .GroupBy(x => x.IdKindOfProduct)
                .Select(x => new ProductTypeOptionModel
                {
                    IdKindOfProduct = x.Key.GetValueOrDefault(),
                    KindOfProductName = x.Where(w => w.IdKindOfProduct == x.Key).Select(s => s.KindOfProductName).FirstOrDefault(),
                    ProductTypeOptions = x.GroupBy(g => g.IdProductType).Select(s => new ProductTypeOption
                    {
                        Id = s.Key.GetValueOrDefault(),
                        Name = s.Select(gs => gs.ProductTypeName).FirstOrDefault(),
                        Value = x.Where(w => w.IdKindOfProduct == x.Key && w.IdProductType == s.Key).Select(xs => xs.HierarchyLv2Matrix.HierarchyLv2Code).FirstOrDefault()
                    }).ToList(),
                }).ToList();
            return result;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
