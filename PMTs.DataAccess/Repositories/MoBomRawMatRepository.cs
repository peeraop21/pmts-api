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
    public class MoBomRawMatRepository : Repository<MoBomRawMat>, IMoBomRawMatRepository
    {
        public MoBomRawMatRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<MoBomRawMat> GetMoBomRawMatByOrderItem(string factoryCode, string orderItem)
        {
            return PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem).AsNoTracking().ToList();
        }

        public IEnumerable<MoBomRawMat> GetMoBomRawMatsByFactoryCode(string factoryCode)
        {
            return PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode).AsNoTracking().ToList();

        }

        public IEnumerable<MoBomRawMat> GetMoBomRawMatsByFgMaterial(string factoryCode, string fgMaterial, string orderItem)
        {
            return PMTsDbContext.MoBomRawMat.Where(m => m.FactoryCode == factoryCode && m.FgMaterial.Equals(fgMaterial) && m.OrderItem.Equals(orderItem)).AsNoTracking().ToList();
        }

        public IEnumerable<MoBomRawMat> GetMoBomRawMatsByOrderItems(Microsoft.Extensions.Configuration.IConfiguration config, List<string> orderItems)
        {

            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                        SELECT DISTINCT A.[ID] as Id
                        ,A.[FactoryCode] as FactoryCode
                        ,A.[OrderItem] as OrderItem
                        ,A.[FG_Material] as FgMaterial
                        ,A.[Material_Type] as MaterialType
                        ,A.[Material_Number] as MaterialNumber
                        ,A.[Plant] as Plant
                        ,A.[Material_Description] as MaterialDescription
                        ,A.[NET_Weight] as NetWeight
                        ,A.[Material_Group] as MaterialGroup
                        ,A.[SIZE_UOM] as SizeUom
                        ,A.[WIDTH] as Width
                        ,A.[LENGTH] as Length
                        ,A.[LAY] as Lay
                        ,A.[CUT_SIZE] as CutSize
                        ,A.[BOM_Amount] as BomAmount
                        ,A.[UOM] as Uom
                        ,A.[CREATE_DATE] as CreateDate
                        ,A.[CREATE_BY] as CreateBy
                        ,A.[UPDATE_DATE] as UpdateDate
                        ,A.[UPDATE_BY] as UpdateBy
                        ,A.[OLD_Material_Number] as OldMaterialNumber  
                        FROM [MO_BOM_RawMat] A
                        JOIN (
                          SELECT COUNT(*) as Count, B.OrderItem
                          FROM [MO_BOM_RawMat] B
                          GROUP BY B.OrderItem
                        ) AS B ON A.OrderItem = B.OrderItem
                        WHERE A.OrderItem IN " + $"('{string.Join("','", orderItems.ToArray())}') and A.WIDTH > 0 and A.LENGTH > 0 ORDER by A.OrderItem"; ;

            string message = string.Format(sql);

            return db.Query<MoBomRawMat>(message).ToList();
        }
    }
}
