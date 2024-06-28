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
    public class PresaleChangeProductRepository : Repository<PresaleChangeProduct>, IPresaleChangeProductRepository
    {
        public PresaleChangeProductRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public PresaleChangeProduct GetPresaleChangeProductByPsmId(string psmId)
        {
            return PMTsDbContext.PresaleChangeProduct.FirstOrDefault(p => p.PsmId == psmId);
        }

        public IEnumerable<PresaleChangeProduct> GetPresaleChangeProductsByKeySearch(IConfiguration config, string factoryCode, string typeSearch, string keySearch)
        {
            var presaleChangeProductStr = string.Empty;
            switch (typeSearch)
            {
                case "Material No":
                    {
                        presaleChangeProductStr = $"Where Material_No like '%{keySearch}%'";
                        break;
                    }
                case "PSMID":
                    {
                        presaleChangeProductStr = $"Where PSM_ID like '%{keySearch}%'";
                        break;
                    }
                default:
                    presaleChangeProductStr = $"Where Material_No like '%{keySearch}%'";
                    break;
            }
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            string sql = @"
                SELECT top 100 [Id] as Id         
                      ,[FactoryCode] as FactoryCode
                      ,[PSM_ID] as PsmId      
                      ,[Material_No] as MaterialNo 
                      ,[Description] as Description
                      ,[Sale_Text1] as SaleText1  
                      ,[Sale_Text2] as SaleText2  
                      ,[Sale_Text3] as SaleText3  
                      ,[Sale_Text4] as SaleText4  
                      ,[Piece_Set] as PieceSet   
                      ,[Print_Method] as PrintMethod
                      ,[High_Group] as HighGroup  
                      ,[High_Value] as HighValue  
                      ,[Bun] as Bun        
                      ,[BunLayer] as BunLayer   
                      ,[LayerPalet] as LayerPalet 
                      ,[IsApprove] as IsApprove
                      ,[FileChange]
                      ,[Status]
                  FROM [dbo].[PresaleChangeProduct]
                  " + presaleChangeProductStr + $" And FactoryCode ='{factoryCode}' AND IsApprove = '0' And ((Status != '4' And Status != '2' )or Status is null)";
            string message = string.Format(sql);

            return db.Query<PresaleChangeProduct>(message).ToList();
        }
        public IEnumerable<PresaleChangeProduct> GetPresaleChangeProductsByActiveStatus(IConfiguration config, string factoryCode)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            string sql = @"
                SELECT top 100 [Id] as Id         
                ,[FactoryCode] as FactoryCode
                ,[PSM_ID] as PsmId      
                ,[Material_No] as MaterialNo 
                ,[Description] as Description
                ,[Sale_Text1] as SaleText1  
                ,[Sale_Text2] as SaleText2  
                ,[Sale_Text3] as SaleText3  
                ,[Sale_Text4] as SaleText4  
                ,[Piece_Set] as PieceSet   
                ,[Print_Method] as PrintMethod
                ,[High_Group] as HighGroup  
                ,[High_Value] as HighValue  
                ,[Bun] as Bun        
                ,[BunLayer] as BunLayer   
                ,[LayerPalet] as LayerPalet 
                ,[IsApprove] as IsApprove
                ,[FileChange]
                ,[Status]
                FROM [dbo].[PresaleChangeProduct]
                " + $" Where FactoryCode ='{factoryCode}' AND IsApprove = '0' And ((Status != '4' And Status != '2' )or Status is null)";
            string message = string.Format(sql);

            return db.Query<PresaleChangeProduct>(message).ToList();
        }
    }
}


