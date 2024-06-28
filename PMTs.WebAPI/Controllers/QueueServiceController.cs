using Microsoft.AspNetCore.Authorization;
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

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs,QueueSystem")]
    [ApiController]
    public class QueueServiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration config;

        public QueueServiceController(PMTsDbContext dbContext, IConfiguration config)
        {
            _unitOfWork = new UnitOfWork(dbContext);
            this.config = config;
        }

        [HttpGet]
        [Route("GetAllFlutes")]
        public IActionResult GetAllFlutes()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Flute.GetAllFlutes, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.Flutes.GetAll());
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Flute.GetAllFlutes Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetAllPaperWidths")]
        public IActionResult GetAllPaperWidths()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PaperWidth.GetAllPaperWidths, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.PaperWidths.GetAll());
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PaperWidth.GetAllPaperWidths Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetAllPaperGrades")]
        public IActionResult GetAllPaperGrades()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PaperGrade.GetAllPaperGrades, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.PaperGrades.GetAllPaperGrades(config));
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PaperGrade.GetAllPaperGrades Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetAllMachines")]
        public IActionResult GetAllMachines()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machine.GetAllMachines, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetAll());
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machine.GetAllMachines Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetAllCorConfigs")]
        public IActionResult GetAllCorConfigs()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CorConfig.GetAllCorConfigs, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.CorConfigs.GetAll());
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CorConfig.GetAllCorConfigs Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetAllScoreGaps")]
        public IActionResult GetAllScoreGaps()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ScoreGap.GetAllScoreGaps, Params : Queue_Service");
                data = JsonConvert.SerializeObject(_unitOfWork.ScoreGaps.GetAll());
                Logger.Info(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ScoreGap.GetAllScoreGaps Success");
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
                Logger.Error(AppCaller, "Queue_Service", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

    }
}