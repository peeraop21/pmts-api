using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BoardCombineAccRepository : Repository<BoardCombineAcc>, IBoardCombineAccRepository
    {
        public BoardCombineAccRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public void AddColumn(SqlConnection conn, string costField)
        {
            //AddBoardCombineAccColumn
            conn.Open();
            string queryCommand = $"ALTER TABLE Board_Combine_Acc ADD {costField} FLOAT;";

            SqlCommand command = new SqlCommand(queryCommand, conn);
            SqlDataReader reader = command.ExecuteReader();
            conn.Close();
        }

        public bool CheckExistCode(string code, string facoryCode)
        {
            var result = PMTsDbContext.BoardCombineAcc.AsNoTracking().FirstOrDefault(b => b.Code == code && b.FactoryCode == facoryCode) != null ? false : true;

            return result;
        }

        public IEnumerable<BoardCombineAcc> GetBoardCombineAccAll()
        {
            return PMTsDbContext.BoardCombineAcc.ToList();
        }

        public Cost GetCost(SqlConnection conn, string FactoryCode, string code, string costField)
        {
            DataTable dt = new DataTable();
            string sqlQuery = "select * from Board_Combine_Acc where FactoryCode = '" + FactoryCode + "' and Code = '" + code + "'";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            var BoardAcc = (from DataRow row in dt.Rows
                            select new Cost
                            {
                                FactoryCode = row["FactoryCode"].ToString(),
                                BoardCode = row["Code"].ToString(),
                                CostField = costField,
                                CostPerTon = row[costField].ToString() == "" ? 0.0 : Convert.ToDouble(row[costField])
                            }).FirstOrDefault();

            return BoardAcc;
        }

        //public void ImportBoardCombineAcc(SqlConnection conn, ref string exceptionMessage, string saleOrg, DataTable dataTable)
        //{
        //    bool isInsert;
        //    var errorImport = 0;
        //    var errorImportCode = "";

        //    conn.Open();

        //    //string queryCommand = $"exec [dbo].[proc_GetCost] @FactoryCode = '252B', @Code = '2316', @costField = 'RSC'";

        //    var rows = dataTable.Rows;
        //    var columns = dataTable.Columns;
        //    var code = "";
        //    for (var r = 0; r < rows.Count; r++)
        //    {
        //        try
        //        {
        //            string str = "";
        //            code = rows[r][0].ToString();
        //            isInsert = PMTsDbContext.BoardCombineAcc.AsNoTracking().FirstOrDefault(b => b.Code == code && b.FactoryCode == saleOrg) != null ? false : true;
        //            if (isInsert)
        //            {
        //                //insert
        //                str = $"INSERT INTO [PMTsDev].[dbo].[Board_Combine_Acc]( FactoryCode,";
        //                foreach (var column in columns)
        //                {
        //                    str = str + column + ",";
        //                }
        //                str = str.Substring(0, str.Length - 1);
        //                str = str + $") VALUES ( '{saleOrg}',";

        //                for (var c = 0; c < columns.Count; c++)
        //                {
        //                    if (c == 0)
        //                    {
        //                        str = string.IsNullOrWhiteSpace(rows[r][c].ToString()) ? str + "NULL ," : str + $"'{rows[r][c]}',";
        //                    }
        //                    else
        //                    {
        //                        str = string.IsNullOrWhiteSpace(rows[r][c].ToString()) ? str + "NULL ," : str + rows[r][c] + ",";
        //                    }
        //                }

        //                str = str.Substring(0, str.Length - 1);
        //                str = str + ")";
        //            }
        //            else
        //            {
        //                //update
        //                str = "Update [PMTsDev].[dbo].[Board_Combine_Acc] SET ";

        //                for (var c = 1; c < columns.Count; c++)
        //                {
        //                    str = string.IsNullOrWhiteSpace(rows[r][c].ToString()) ? str + $" {columns[c].ToString()} = NULL ," : str + $" {columns[c].ToString()} = {rows[r][c]} ,";
        //                }

        //                str = str.Substring(0, str.Length - 1);
        //                str = str + $" Where FactoryCode = '{saleOrg}' and Code = '{code}'";
        //            }

        //            SqlCommand command = new SqlCommand(str, conn);
        //            SqlDataReader reader = command.ExecuteReader();
        //        }
        //        catch (Exception ex)
        //        {
        //            errorImportCode = r == rows.Count ? errorImportCode + code : errorImportCode + code + ", ";
        //            errorImport++;
        //            continue;
        //        }

        //    }
        //    errorImportCode = string.IsNullOrWhiteSpace(errorImportCode) ? "" : errorImportCode.Substring(0 - errorImportCode.Length - 2);
        //    exceptionMessage = string.IsNullOrWhiteSpace(errorImportCode) ? $"{rows.Count - errorImport} row successfully \n {errorImport} row failed" : $"{rows.Count - errorImport} row successfully \n {errorImport} row failed (Code : {errorImportCode})";

        //    conn.Close();
        //}


        #region [Board Cost Dynamic Update]
        public EditCostFieldsModel BindBoardcombindAccData(IConfiguration config, string FactoryCode, string Code)
        {
            var editCostFieldsModel = new EditCostFieldsModel();
            using (IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                string SelectplantCostfields = @"
                                   SELECT
                                  [FactoryCode]
                                  ,[CostField]
                              FROM PlantCostField where FactoryCode ='{0}'  
                        ";
                string querysql1 = string.Format(SelectplantCostfields, string.IsNullOrEmpty(FactoryCode) ? "" : FactoryCode);
                editCostFieldsModel.plantCostFields = db.Query<BoardCombindAccPlantCostField>(querysql1).ToList();

                string SelectBoardcombindSchema = @"
                          select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, 
                                   NUMERIC_PRECISION, DATETIME_PRECISION, 
                                   IS_NULLABLE 
                            from INFORMATION_SCHEMA.COLUMNS
                            where TABLE_NAME='Board_Combine_Acc'
                        ";
                editCostFieldsModel.boardCombindAccSchemas = db.Query<BoardCombindAccSchema>(SelectBoardcombindSchema).ToList();


                string fildsSelect = string.Empty;
                foreach (var item in editCostFieldsModel.plantCostFields)
                {
                    string tmpfields = item.CostField + ',';
                    fildsSelect = fildsSelect + tmpfields;
                }
                if (!string.IsNullOrEmpty(fildsSelect))
                {
                    string qSelect = fildsSelect.Substring(0, fildsSelect.Length - 1);
                    string SelectBoardcombindPivot = @"
                                 SELECT col as ColumnName, value as Value from 
                                 (select
                                 SelectFields
                                 from Board_Combine_Acc where FactoryCode = '{0}' and Code = '{1}') as tbl
                                 UNPIVOT
                                 (
                                  value
                                  FOR col IN (DataFields)
                                 ) un1 
                        ";
                    SelectBoardcombindPivot = SelectBoardcombindPivot.Replace("SelectFields", qSelect);
                    SelectBoardcombindPivot = SelectBoardcombindPivot.Replace("DataFields", qSelect);
                    string querysql2 = string.Format(SelectBoardcombindPivot, FactoryCode, Code);
                    editCostFieldsModel.boardCombindAccPivots = db.Query<BoardCombindAccPivot>(querysql2).ToList();
                }
            }

            return editCostFieldsModel;
        }

        public void UpdateBoardcombindAcc(IConfiguration config, IMapper mapper, List<BoardCombindAccUpdate> model)
        {
            if (model.Count > 0)
            {
                using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                string factorycode = string.Empty;
                string code = string.Empty;
                string fields = string.Empty;
                string updateby = string.Empty;
                foreach (var item in model)
                {
                    factorycode = item.FactoryCode;
                    code = item.Code;
                    updateby = item.UpdateBy;

                    string valuefields = item.Value == null ? "NULL" : item.Value.ToString();
                    string tmpfields = item.ColumnName + "=" + valuefields + ",";
                    fields = fields + tmpfields;
                }

                string sqlcount = @"select count(*) as cc from Board_Combine_Acc where factorycode  = '{0}' and code = '{1}'";
                string querysqlcount = string.Format(sqlcount, factorycode, code);
                string countdata = db.Query<string>(querysqlcount).FirstOrDefault();

                //  var pmtsboardcombindacc = PMTsDbContext.BoardCombineAcc.Where(x => x.Code == code && x.FactoryCode == factorycode).ToList();
                if (countdata == "0")
                {
                    string colfields = string.Empty;
                    string valfields = string.Empty;
                    foreach (var item in model)
                    {
                        string tmpcol = item.ColumnName + ",";
                        colfields = colfields + tmpcol;
                        string valuefields = item.Value == null ? "NULL" : item.Value.ToString();
                        string tmpval = valuefields + ",";
                        valfields = valfields + tmpval;
                    }
                    colfields = colfields + "CreatedDate,CreatedBy,Code,FactoryCode";
                    valfields = valfields + "GETDATE(),'" + updateby + "','" + code + "','" + factorycode + "'";
                    string SelectplantCostfields = @"
                           INSERT INTO Board_Combine_Acc
                                               (
		                                             colfileds
		                                       )
                                         VALUES
                                               (
		                                              valfileds
		                                       )
                        ";
                    SelectplantCostfields = SelectplantCostfields.Replace("colfileds", colfields);
                    SelectplantCostfields = SelectplantCostfields.Replace("valfileds", valfields);
                    db.Execute(SelectplantCostfields);
                }
                else
                {
                    if (!string.IsNullOrEmpty(fields))
                    {
                        fields = fields + "UpdatedDate = GETDATE()" + ",UpdatedBy = '" + updateby + "' ";
                        if (db.State == ConnectionState.Closed)
                            db.Open();
                        string SelectplantCostfields = @"
                            UPDATE Board_Combine_Acc
                               SET 
                               sqlfieldsupdate
                             where FactoryCode = '{0}' and Code = '{1}' 
                        ";
                        SelectplantCostfields = SelectplantCostfields.Replace("sqlfieldsupdate", fields);
                        string querysql1 = string.Format(SelectplantCostfields, factorycode, code);
                        db.Execute(querysql1);
                    }

                }
                if (model.Where(w => w.ColumnName == "SB").Any())
                {
                    //var orderItemList = PMTsDbContext.MoSpec.Join(PMTsDbContext.BoardCombineAcc, ms => ms.Code, b => b.Code, (ms, b) => new { OrderItem = ms.OrderItem, FactoryCode = ms.FactoryCode, Code = ms.Code })
                    //    .Where(w => w.FactoryCode == factorycode && w.Code == code)
                    //    .Select(s => s.OrderItem).Distinct().ToList();
                    var boardCom = PMTsDbContext.BoardCombine.FirstOrDefault(b => b.Code == code);
                    if (boardCom != null)
                    {
                        var orderItemList = PMTsDbContext.MoSpec
                            .Where(
                                    p =>
                                        p.FactoryCode == factorycode &&
                                        (
                                            !string.IsNullOrEmpty(p.Board) && p.Board.StartsWith("FSC/") ?
                                                p.Board.Replace("FSC/", "") :
                                                p.Board
                                        )
                                            ==
                                        (
                                            !string.IsNullOrEmpty(boardCom.Board) && boardCom.Board.StartsWith("FSC/") ?
                                                boardCom.Board.Replace("FSC/", "") :
                                                boardCom.Board
                                         ) &&
                                        p.Flute == boardCom.Flute
                                    )
                            .Select(s => s.OrderItem)
                            .ToList();
                        if (orderItemList != null && orderItemList.Count > 0)
                        {
                            var modataList = PMTsDbContext.MoData
                                .Where(w => w.FactoryCode == factorycode && orderItemList.Contains(w.OrderItem) && w.MoStatus == "H")
                                .ToList();
                            if (modataList != null && modataList.Count > 0)
                            {
                                var modatalogList = mapper.Map<List<MoDatalog>>(modataList);
                                modatalogList.ForEach(m => m.Id = 0);
                                PMTsDbContext.MoDatalog.AddRange(modatalogList);
                                modataList.ForEach(m =>
                                {
                                    m.MoStatus = "C";
                                    m.UpdatedDate = DateTime.Now;
                                    m.UpdatedBy = updateby;
                                });
                                PMTsDbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
        }


        #endregion
    }
}
