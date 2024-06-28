﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class SetCategoriesOldMatController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration config;

        public SetCategoriesOldMatController(PMTsDbContext pmtsContext, IConfiguration configuration)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
            config = configuration;
        }

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMats.GetAll");
                data = JsonConvert.SerializeObject(_unitOfWork.SetCategoriesOldMat.GetAll());
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMats.GetAll Success");
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
        [Route("GetSetCategoriesOldMatByLV2")]
        public IActionResult GetSetCategoriesOldMatByLV2(string FactoryCode, string LV2)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMat.GetSetCategoriesOldMatByLV2, Params : " + LV2.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SetCategoriesOldMat.GetSetCategoriesOldMatByLV2(LV2));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMat.GetSetCategoriesOldMatByLV2 Success");
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
        [Route("GetCategoriesMatrixByLV2")]
        public IActionResult GetCategoriesMatrixByLV2(string FactoryCode, string LV2)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMat.GetCategoriesMatrixByLV2, Params : " + LV2.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SetCategoriesOldMat.GetCategoriesMatrixByLV2(config, LV2));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SetCategoriesOldMat.GetCategoriesMatrixByLV2 Success");
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