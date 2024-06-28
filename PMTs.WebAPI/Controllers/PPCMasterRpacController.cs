using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class PPCMasterRpacController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public PPCMasterRpacController(PMTsDbContext pmtsContext)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
        }
        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetAllByFactory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.PPCMasterRpac.GetAll());
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetAllByFactory Success");
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
        [Route("GetPPCMasterRpacsByFactoryCode")]
        public IActionResult GetPPCMasterRpacsByFactoryCode(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetPPCMasterRpacsByFactoryCode, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.PPCMasterRpac.GetAllByFactory(p => p.FactoryCode == FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetPPCMasterRpacsByFactoryCode Success");
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
        [Route("GetPPCMasterRpacsByDimensionCode")]
        public IActionResult GetPPCMasterRpacsByDimensionCode(string FactoryCode, string DimensionCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");


            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetPPCMasterRpacsByDimensionCode, Params : " + FactoryCode.ToString() + " , " + DimensionCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.PPCMasterRpac.GetAllByFactory(p => p.FactoryCode == FactoryCode && p.DimensionCode.Equals(DimensionCode)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.GetPPCMasterRpacsByDimensionCode Success");
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
        public IActionResult Post(string FactoryCode, [FromBody] PpcMasterRpac model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.Add");
                _unitOfWork.PPCMasterRpac.Add(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.Add success");
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

        [HttpPut]
        public IActionResult Put(string FactoryCode, [FromBody] PpcMasterRpac model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.Update");
                _unitOfWork.PPCMasterRpac.Update(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCMasterRpac.Update Success");
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
