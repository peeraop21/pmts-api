using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.Redis.Interfaces;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class BoardCombindMaintainController(PMTsDbContext pmtsContext, IConfiguration configuration, ICacheService cacheService) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;
        private readonly ICacheService _cacheService = cacheService;

        [HttpGet]
        [Route("GetAllDataMainTain")]
        public IActionResult GetAllDataMainTain(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                BoardCombindMaintainModel model = new BoardCombindMaintainModel();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllBoardcombind, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString());
                var cacheData = _cacheService.GetData<List<BoardCombind>>("BoardCombinds");
                if (cacheData != null)
                {
                    model.BoardCombind = cacheData;
                }
                else
                {
                    model.BoardCombind = _unitOfWork.BoardCombindMaintains.GetAllBoardcombind(config);
                    _cacheService.SetData<List<BoardCombind>>("BoardCombinds", model.BoardCombind, DateTimeOffset.Now.AddDays(1));
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllBoardcombind Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllFluteByFactoryCode, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString());
                model.FluteTR = _unitOfWork.BoardCombindMaintains.GetAllFluteByFactoryCode(config, FactoryCode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllFluteByFactoryCode Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetDistinctFluteByFactoryCode, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString());
                model.Option = _unitOfWork.BoardCombindMaintains.GetDistinctFluteByFactoryCode(config, FactoryCode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetDistinctFluteByFactoryCode Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllMaxcode, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString());
                var tmp = _unitOfWork.BoardCombindMaintains.GetAllMaxcode(config);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllMaxcode Success");
                model.MaxID = tmp == null ? "" : tmp.MaxID;

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllPaperGrade, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString());
                model.PaperGrade = _unitOfWork.BoardCombindMaintains.GetAllPaperGrade(config, FactoryCode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllPaperGrade Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetHierarchyLv2MatrixGroupByKindOfProduct, Params : ");
                model.ProductTypeOptions = _unitOfWork.ProductType.GetProductTypesGroupByGroupProduct();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetHierarchyLv2MatrixGroupByKindOfProduct Success");
                data = JsonConvert.SerializeObject(model);
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
        [Route("GetMaxCode")]
        public IActionResult GetMaxCode(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                BoardCombindMaintainModel model = new BoardCombindMaintainModel();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllMaxcode, Params : " + config.GetConnectionString("PMTsConnect").ToString());
                var tmp = _unitOfWork.BoardCombindMaintains.GetAllMaxcode(config);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllMaxcode Success");
                model.MaxID = tmp == null ? "" : tmp.MaxID;
                data = JsonConvert.SerializeObject(model);
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
        [Route("GetBoardSpect")]
        public IActionResult GetBoardSpect(string FactoryCode, string Code)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                var codeStr = string.IsNullOrEmpty(Code) ? string.Empty : Code.ToString();
                BoardCombindMaintainModel model = new BoardCombindMaintainModel();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllBoardSpectByCode, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + codeStr);
                model.BoardSpect = _unitOfWork.BoardCombindMaintains.GetAllBoardSpectByCode(config, Code);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.GetAllBoardSpectByCode Success");
                data = JsonConvert.SerializeObject(model);
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
        [Route("AddBoardCombind")]
        public IActionResult AddBoardCombind(string FactoryCode, [FromBody] BoardCombindMaintainModel model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.AddBoard, Params : " + config.GetConnectionString("PMTsConnect").ToString());
                model._BoardCombind.CreatedBy = User.GetClaimValue("UserName");
                var result = _unitOfWork.BoardCombindMaintains.AddBoard(config, model);

                // caching
                _cacheService.RemoveData("BoardCombinds");
                var boardCombinds = _unitOfWork.BoardCombindMaintains.GetAllBoardcombind(config);
                _cacheService.SetData<List<BoardCombind>>("BoardCombinds", boardCombinds, DateTimeOffset.Now.AddDays(1));

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.AddBoard Success");
                return Ok(new CustomResponse<bool> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = result });
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
        [Route("UpdateBoardCombind")]
        public IActionResult UpdateBoardCombind(string FactoryCode, [FromBody] BoardCombindMaintainModel model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.UpdateBoard, Params : " + config.GetConnectionString("PMTsConnect").ToString());
                model._BoardCombind.UpdatedBy = User.GetClaimValue("UserName");
                _unitOfWork.BoardCombindMaintains.UpdateBoard(config, model);

                // caching
                _cacheService.RemoveData("BoardCombinds");
                var boardCombinds = _unitOfWork.BoardCombindMaintains.GetAllBoardcombind(config);
                _cacheService.SetData<List<BoardCombind>>("BoardCombinds", boardCombinds, DateTimeOffset.Now.AddDays(1));

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombindMaintains.UpdateBoard Success");
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
        [AllowAnonymous]
        [HttpGet]
        [Route("GenerateCode")]
        public IActionResult GenerateCode(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombines.GenerateCode");
                data = JsonConvert.SerializeObject(_unitOfWork.BoardCombines.GenerateCode());
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombines.GenerateCode Success");
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

        [AllowAnonymous]
        [HttpPost]
        [Route("GenerateDataForSAP")]
        public IActionResult GenerateDataForSAP(string FactoryCode, [FromBody] ExportDataForSAPRequest req)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombines.GenerateFileForSAP");
                data = JsonConvert.SerializeObject(_unitOfWork.BoardCombines.GenerateDataForSAP(req));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCombines.GenerateFileForSAP Success");
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
    }
}
