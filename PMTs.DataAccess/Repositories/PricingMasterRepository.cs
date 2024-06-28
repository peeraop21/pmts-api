using Dapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PricingMasterRepository : Repository<PricingMaster>, IPricingMasterRepository
    {
        public PricingMasterRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<PricingMaster> GetPricingMaster(IConfiguration config, string saleOrg, string materialNo)
        {
            using (System.Data.IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect")))
            {
                if (db.State == System.Data.ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                string sql = @"
                        SELECT [Id] as Id
                      ,[condition type] as  ConditionType
                      ,[sales org] as SalesOrg
                      ,[distribution channel] as  DistributionChannel
                      ,[material no] as MaterialNo
                      ,[validity start from] as  ValidityStartFrom
                      ,[validity end date] as  ValidityEndDate
                      ,[condition number] as ConditionNumber
                      ,[rate] as Rate
                      ,[currency key] as CurrencyKey
                      ,[condition pricing unit] as ConditionPricingUnit
                      ,[UOM]  as Uom
                        from 
                         (Select * from PricingMaster where  GETDATE() BETWEEN [validity start from] and [validity end date]) p 
                         where p.[material no] = '{0}' and p.[sales org] = '{1}'";

                string message = string.Format(sql, materialNo, saleOrg);

                return db.Query<PricingMaster>(message).ToList();
            }
        }
    }
}
