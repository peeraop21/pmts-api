using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class KindOfProductRepository : Repository<KindOfProduct>, IKindOfProductRepository
    {
        public KindOfProductRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<KindOfProduct> GetKindOfProductsByIds(List<string> idKindOfProducts)
        {
            var kindOfProducts = new List<KindOfProduct>();

            foreach (var idKindOfProduct in idKindOfProducts)
            {
                kindOfProducts.AddRange(PMTsDbContext.KindOfProduct.Where(k => k.Id == Convert.ToInt32(idKindOfProduct)).AsNoTracking().ToList());
            }

            return kindOfProducts;
        }
    }
}
