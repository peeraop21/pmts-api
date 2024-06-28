using Dapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class SetCategoriesOldMatRepository : Repository<SetCategoriesOldMat>, ISetCategoriesOldMatRepository
    {
        public SetCategoriesOldMatRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<SetCategoriesOldMat> GetSetCategoriesOldMatAll()
        {
            return PMTsDbContext.SetCategoriesOldMat.ToList();
        }

        public IEnumerable<SetCategoriesOldMat> GetSetCategoriesOldMatByLV2(string LV2)
        {
            return PMTsDbContext.SetCategoriesOldMat.Where(s => s.Lv2 == LV2).ToList();
        }

        public IEnumerable<SetCategoriesOldMat> GetCategoriesMatrixByLV2(IConfiguration config, string LV2)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                select h.IdProductType, p.Name pdtName, h.HierarchyLv2Code LV2, h.IdKindOfProductGroup, 
                g.Name kpgName, h.IdProcessCost, c.Name pccName, h.IdKindOfProduct, k.Name kopName
                from Hierarchy_Lv2_Matrix h 
                left outer join ProductType p on p.Id = h.IdProductType
                left outer join KindOfProductGroup g on g.Id = h.IdKindOfProductGroup
                left outer join ProcessCost c on c.Id = h.IdProcessCost
                left outer join KindOfProduct k on k.Id = h.IdKindOfProduct
                where h.HierarchyLv2Code = '{0}'
                                ";

            string message = string.Format(sql, LV2);

            return db.Query<SetCategoriesOldMat>(message).ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
