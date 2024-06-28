using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class TransactionsDetailRepository : Repository<TransactionsDetail>, ITransactionsDetailRepository
    {
        public TransactionsDetailRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public TransactionsDetail GetTransactionsDetailByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.TransactionsDetail.FirstOrDefault(t => t.MaterialNo == materialNo && t.FactoryCode == factoryCode && t.PdisStatus != "X");
        }

        public IEnumerable<TransactionsDetail> GetTransactionsDetailListByMaterialNo(string materialNo)
        {
            return PMTsDbContext.TransactionsDetail.Where(t => t.MaterialNo == materialNo).ToList();
        }

        public IEnumerable<TransactionsDetail> GetTransactionsDetailsByMaterialNOs(string factoryCode, List<string> materialNOs)
        {
            var transactionsDetails = new List<TransactionsDetail>();

            if (materialNOs != null && materialNOs.Count > 0)
            {
                transactionsDetails.AddRange(PMTsDbContext.TransactionsDetail.Where(m => m.FactoryCode == factoryCode && materialNOs.Contains(m.MaterialNo)).AsNoTracking().ToList());
            }

            return transactionsDetails;
        }

        public TransactionsDetail GetSelectedFirstOutsourceByMaterialNo(IConfiguration configuration, string factoryCode, string matSaleOrg)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    SELECT 
                    TOP 1
                        t.FactoryCode,
                        t.HireOrderType,
                        t.Id
                    FROM
                        MasterData m
                    INNER JOIN Transactions_Detail t
                        ON m.Material_No = t.MaterialNo and m.FactoryCode = t.FactoryCode
	                    where t.MatSaleOrg ='{0}' and m.FactoryCode != '{1}'
	                    order by m.CreateDate";
            string message = string.Format(sql, matSaleOrg, factoryCode);
            return db.Query<TransactionsDetail>(message).FirstOrDefault();
        }
        public string GetAllMatOutsourceByMaterialNo(IConfiguration configuration, string factoryCode, string materialNo)
        {
            var matOutsources = PMTsDbContext.TransactionsDetail.Where(t => t.MatSaleOrg == materialNo && t.FactoryCode != factoryCode).Select(m => m.MaterialNo).ToList();
            var result = matOutsources.Count() > 0 ? string.Join(",", matOutsources) : string.Empty;
            return result;
        }

        public TransactionsDetail GetMatOutsourceByMatSaleOrg(IConfiguration configuration, string factoryCode, string materialNo)
        {
            return PMTsDbContext.TransactionsDetail.Where(t => t.MatSaleOrg == materialNo && t.FactoryCode != factoryCode).FirstOrDefault();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
