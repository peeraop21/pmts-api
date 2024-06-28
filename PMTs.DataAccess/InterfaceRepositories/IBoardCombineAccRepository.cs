using AutoMapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBoardCombineAccRepository : IRepository<BoardCombineAcc>
    {
        Cost GetCost(SqlConnection conn, string FactoryCode, string code, string costField);

        void AddColumn(SqlConnection conn, string costField);

        //bool ImportBoardCombineAcc(SqlConnection conn, string queryString);

        bool CheckExistCode(string code, string saleOrg);

        EditCostFieldsModel BindBoardcombindAccData(IConfiguration config, string FactoryCode, string Code);
        void UpdateBoardcombindAcc(IConfiguration config, IMapper mapper, List<BoardCombindAccUpdate> model);

    }
}
