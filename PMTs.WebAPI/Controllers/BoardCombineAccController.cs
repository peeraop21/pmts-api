using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class BoardCombineAccController(PMTsDbContext pmtsContext, IConfiguration configuration, IMapper mapper) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.GetAll");
                data = JsonConvert.SerializeObject(_unitOfWork.BoardCombineAccs.GetAll());
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.GetAll Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetCost")]
        public IActionResult GetCost(string FactoryCode, string Code, string CostField)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                //   SqlConnection conn = PMTsConnect();
                SqlConnection conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.GetCost, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode + " , " + Code + " , " + CostField);
                data = JsonConvert.SerializeObject(_unitOfWork.BoardCombineAccs.GetCost(conn, FactoryCode, Code, CostField));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.GetCost Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                    Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
                }
                else if (ex.Message.Contains("does not belong to table"))
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.GetCost Success");
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
                else
                {
                    exceptionMessage = ex.Message;
                    Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
                }
            }
        }

        [HttpPost]
        [Route("AddBoardCombineAccColumn")]
        public IActionResult AddBoardCombineAccColumn(string FactoryCode, string CostField)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                SqlConnection conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.AddColumn, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + CostField);
                _unitOfWork.BoardCombineAccs.AddColumn(conn, CostField);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.AddColumn Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("ImportBoardCombineAcc")]
        public IActionResult ImportBoardCombineAcc(string FactoryCode, string UserName, [FromBody] dynamic JsonDataTable)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string database = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                SqlConnection conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.ImportBoardCombineAcc, Params : " + config.GetConnectionString("PMTsConnect").ToString());
                Match databaseMatch = Regex.Match(conn.ConnectionString, @"Database=([A-Za-z0-9_]+)", RegexOptions.IgnoreCase);
                if (databaseMatch.Success)
                {
                    database = databaseMatch.Groups[1].Value;
                }
                bool isInsert;
                var errorImport = 0;
                var errorImportCode = "";
                var code = "";
                //var connetionString = @"Data Source=localhost;Initial Catalog=testPMTs;User ID=sa;Password=1234qwerT;MultipleActiveResultSets = True";
                conn.Open();

                //string queryCommand = $"exec [dbo].[proc_GetCost] @FactoryCode = '252B', @Code = '2316', @costField = 'RSC'";
                //var boardAccRow = JsonConvert.DeserializeObject<dynamic>(JsonDataTable);
                var boardAccRow = JsonDataTable;
                var tableNo = boardAccRow.Count;
                for (var i = 0; i < tableNo; i++)
                {
                    try
                    {
                        string str = "";
                        code = Convert.ToString(boardAccRow[i].Code);
                        isInsert = _unitOfWork.BoardCombineAccs.CheckExistCode(code, FactoryCode);
                        var dateTimeNow = DateTime.Now;

                        if (isInsert)
                        {
                            var columnNames = new List<string>();
                            var columnValue = $") VALUES ( '{FactoryCode}','{UserName}','{dateTimeNow}',";
                            //insert
                            str = $"INSERT INTO [{database}].[dbo].[Board_Combine_Acc]( FactoryCode, CreatedBy,CreatedDate,";

                            foreach (var boardAccData in boardAccRow[i])
                            {
                                var name = boardAccData.Name;
                                var value = Convert.ToString(boardAccData.Value);
                                str = str + name + ",";
                                columnValue = string.IsNullOrWhiteSpace(value) ? columnValue + "NULL ," : columnValue + $"'{value}',";
                            }

                            str = str.Substring(0, str.Length - 1);
                            str = string.Concat(str, columnValue.AsSpan(0, columnValue.Length - 1), ")");
                        }
                        else
                        {
                            str = $"Update [{database}].[dbo].[Board_Combine_Acc] SET UpdatedBy = '{UserName}', UpdatedDate = '{dateTimeNow}',";

                            foreach (var boardAccData in boardAccRow[i])
                            {
                                var name = boardAccData.Name;
                                var value = Convert.ToString(boardAccData.Value);
                                str = string.IsNullOrWhiteSpace(name) ? str + $" {name} = NULL ," : str + $" {name} = '{value}' ,";
                            }

                            str = str.Substring(0, str.Length - 1);
                            str = str + $" Where FactoryCode = '{FactoryCode}' and Code = '{code}'";
                        }

                        // send query string to repos
                        SqlCommand command = new SqlCommand(str, conn);
                        SqlDataReader reader = command.ExecuteReader();
                    }
                    catch
                    {
                        errorImportCode = i == tableNo ? errorImportCode + code : errorImportCode + code + ", ";
                        errorImport++;
                        continue;
                    }
                }

                errorImportCode = string.IsNullOrWhiteSpace(errorImportCode) ? "" : errorImportCode.Substring(0 - errorImportCode.Length - 2);
                exceptionMessage = string.IsNullOrWhiteSpace(errorImportCode) ? $"{tableNo - errorImport} row successfully \n {errorImport} row failed" : $"{tableNo - errorImport} row successfully \n {errorImport} row failed (Code : {errorImportCode})";
                conn.Close();

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.ImportBoardCombineAcc Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = exceptionMessage });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        #region [Edit CostFiels]
        [HttpGet]
        [Route("GetBoardcombindAcc")]
        public IActionResult GetEditBoardcombindAccFrist(string FactoryCode, string Code)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.BindBoardcombindAccData, Params :  " + FactoryCode.ToString() + " , " + Code.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.BoardCombineAccs.BindBoardcombindAccData(config, FactoryCode, Code));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.BindBoardcombindAccData Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("UpdateBoardcombindAcc")]
        public IActionResult UpdateBoardcombindAcc(string FactoryCode, [FromBody] List<BoardCombindAccUpdate> model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.UpdateBoardcombindAcc, Params : " + config.GetConnectionString("PMTsConnect").ToString());
                _unitOfWork.BoardCombineAccs.UpdateBoardcombindAcc(config, _mapper, model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombineAccs.UpdateBoardcombindAcc Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }
        #endregion
    }
}
