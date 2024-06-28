using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.Models;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class MachineController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetAllByFactory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetAllByFactory(m => m.FactoryCode == FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetAllByFactory Success");
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
        [Route("GetMachineById")]
        public IActionResult GetMachineById(string FactoryCode, int Id)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetById, Params : " + Id.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetById(Id));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetById Success");
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
        [Route("GetMachineGroupByMachine")]
        public IActionResult GetMachineGroupByMachine(string FactoryCode, string Machine)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Machine = ConvertHexToString(Machine, System.Text.Encoding.Unicode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachineGroupByMachine, Params : " + FactoryCode.ToString() + " , " + Machine.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetMachineGroupByMachine(FactoryCode, Machine));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachineGroupByMachine Success");
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
        [Route("GetMachineGroupByPlanCode")]
        public IActionResult GetMachineGroupByPlanCode(string FactoryCode, string PlanCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachineGroupByMachine, Params : " + FactoryCode.ToString() + " , " + PlanCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetByFactory(m => m.PlanCode.Equals(PlanCode) && m.FactoryCode.Equals(FactoryCode)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachineGroupByMachine Success");
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
        [Route("GetMachinesByPlanCodes")]
        public IActionResult GetMachinesByPlanCodes(string AppName, string FactoryCode, [FromBody] List<string> planCodes)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachinesByPlanCodes, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetMachinesByPlanCodes(FactoryCode, planCodes));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetMachinesByPlanCodes Success");
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

        private static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        [HttpPost]
        public IActionResult Post(string FactoryCode, [FromBody] ParentModel model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.Add");
                _unitOfWork.Machines.CreateMachine(model.Machine);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.Add Success");
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
        public IActionResult Put(string FactoryCode, [FromBody] ParentModel parentModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.Update");
                _unitOfWork.Machines.UpdateMachine(parentModel.Machine);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.Update Success");
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
        [Route("GetMachineHierarchy")]
        public IActionResult GetMachineHierarchy(string factoryCode, string hieLv2, string materialNo, string floxotype, string JoinType, string token)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string datamaster = string.Empty;
            Logger.Info(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                MasterData masterData = _unitOfWork.MasterDatas.GetMasterDataByMaterialNumber(factoryCode, materialNo);
                Logger.Info(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetHierarchy, Params : " + hieLv2.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetMachineHierarchy(config, factoryCode, hieLv2, masterData, floxotype, JoinType));
                Logger.Info(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetById Success");
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
                Logger.Error(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetMachineByMachineGroup")]
        public IActionResult GetMachineByMachineGroup(string factoryCode, string machineGroup, string token)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                // find masterdata 
                string data = JsonConvert.SerializeObject(_unitOfWork.Machines.GetMachineByMachineGroup(config, factoryCode, machineGroup));
                Logger.Info(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Machines.GetmachineGroup Success");
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
                Logger.Error(AppCaller, factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }
    }
}
