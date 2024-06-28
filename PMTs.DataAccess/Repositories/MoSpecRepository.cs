using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MoSpecRepository : Repository<MoSpec>, IMoSpecRepository
    {
        public MoSpecRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public MoSpec GetMoSpecBySaleOrder(string factoryCode, string orderItem)
        {
            var moSpecs = new MoSpec();
            moSpecs = PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).AsNoTracking().FirstOrDefault();

            return moSpecs;
        }

        public MoSpec GetMoSpecByOrderItem(string factoryCode, string orderItem)
        {
            var moSpecs = new MoSpec();
            moSpecs = PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).AsNoTracking().FirstOrDefault();
            moSpecs = moSpecs == null ? PMTsDbContext.MoSpec.Where(m => m.OrderItem == orderItem).AsNoTracking().FirstOrDefault() : moSpecs;
            return moSpecs;
        }

        public MoSpec GetMoSpecBySuffixSO(string factoryCode, string SO)
        {
            return PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && m.OrderItem.Trim().EndsWith(SO)).AsNoTracking().FirstOrDefault();
        }

        public string GetMoSpecDiecutPath(string factoryCode, string orderItem)
        {
            return PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).AsNoTracking().FirstOrDefault().DiecutPictPath;
        }
        public string GetMoSpecPalletPath(string factoryCode, string orderItem)
        {
            return PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).AsNoTracking().FirstOrDefault().PalletizationPath;
        }

        public IEnumerable<MoSpec> GetMOSpecsBySaleOrders(string factoryCode, List<string> saleOrders)
        {
            var moSpecs = new List<MoSpec>();
            moSpecs.AddRange(PMTsDbContext.MoSpec.Where(m => m.FactoryCode == factoryCode && saleOrders.Contains(m.OrderItem)).AsNoTracking().ToList());

            return moSpecs;
        }

        //Tassanai update 
        public void UpdateMoSpecChange(string FactoryCode, string OrderItem, string ChangeInfo)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var some = PMTsDbContext.MoSpec.Where(s => s.FactoryCode == FactoryCode && s.OrderItem == OrderItem).ToList();
                    some.ForEach(a => a.ChangeInfo = ChangeInfo);
                    var response = PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }
        public IEnumerable<MasterData> GetMatNoOrder(IConfiguration config, string factoryCode, string custCode, string yearCreate, string yearUpdate)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            string condition = string.Empty;
            condition = !string.IsNullOrEmpty(custCode) ? condition + $" and Cust_Code = '{custCode}'" : condition;
            condition = !string.IsNullOrEmpty(yearCreate) ? condition + $" and YEAR(CreateDate) = '{yearCreate}'" : condition;
            condition = !string.IsNullOrEmpty(yearUpdate) ? condition + $" and YEAR(LastUpdate) = '{yearUpdate}'" : condition;

            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                    select m.FactoryCode, m.Material_No MaterialNo, PC, Cust_Code CustCode, Description, Box_Type BoxType, CreateDate, CreatedBy, LastUpdate, PDIS_Status PdisStatus
                    from
	                    (select *
	                    from MasterData
	                    where FactoryCode = '{0}' and PDIS_Status <> 'X') m 
	                    left outer join (
                        select FactoryCode, Material_No 
	                    from (  select FactoryCode, Material_No from MO_Spec group by FactoryCode, Material_No
		                        union 
		                        select FactoryCode, Material_No from MO_Spec_Archive group by FactoryCode, Material_No) a
	                    where FactoryCode = '{1}'
	                    group by FactoryCode, Material_No ) mo on mo.Material_No = m.Material_No
                        where mo.Material_No is null " + condition + @"
                ";

            string message = string.Format(sql, factoryCode, factoryCode);

            return db.Query<MasterData>(message).ToList();
        }
    }
}
