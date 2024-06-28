using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.Models;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    //Tassanai Update 30062020 -start
    //[Authorize(Roles = "PMTs")]
    [Authorize(Roles = "PMTs,NerpSystem")]
    //Tassanai Update 30062020 -end
    [ApiController]
    public class MasterDataController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new(pmtsContext);
        private readonly IConfiguration config = configuration;

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetAllByFactory, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetAllByFactory(m => m.FactoryCode == FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetAllByFactory Success");
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
        [Route("GetMasterDataByMaterialNo")]
        public IActionResult GetMasterDataByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialNumber(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");
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
        [Route("GetOutsourceFromMaterialNoAndSaleOrg")]
        public IActionResult GetOutsourceFromMaterialNoAndSaleOrg(string FactoryCode, string MaterialNo, string SaleOrg, string FactoryCodeOutsource)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetOutsourceFromMaterialNoAndSaleOrg, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + SaleOrg.ToString() + " , " + FactoryCodeOutsource.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetOutsourceFromMaterialNoAndSaleOrg(FactoryCodeOutsource, MaterialNo, SaleOrg));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetOutsourceFromMaterialNoAndSaleOrg Success");
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
        [Route("GetMasterDatasByMaterialNos")]
        public IActionResult GetMasterDatasByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> MaterialNos)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMaterialNos, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDatasByMaterialNos(FactoryCode, MaterialNos));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMaterialNos Success");
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
        [Route("GetReuseMasterDatasByMaterialNos")]
        public IActionResult GetReuseMasterDatasByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> MaterialNos)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetReuseMasterDatasByMaterialNos, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetReuseMasterDatasByMaterialNos(config, FactoryCode, MaterialNos));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetReuseMasterDatasByMaterialNos Success");
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
        [Route("GetMasterDataRoutingsByMaterialNos")]
        public IActionResult GetMasterDataRoutingsByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> MaterialNos)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataRoutingsByMaterialNos, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataRoutingsByMaterialNos(config, FactoryCode, MaterialNos));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataRoutingsByMaterialNos Success");
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
        [Route("GetReuseMasterDataRoutingsByMaterialNos")]
        public IActionResult GetReuseMasterDataRoutingsByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> MaterialNos)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetReuseMasterDataRoutingsByMaterialNos, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetReuseMasterDataRoutingsByMaterialNos(config, FactoryCode, MaterialNos));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetReuseMasterDataRoutingsByMaterialNos Success");
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
        [Route("GetMasterDataByMaterialNoAndFactory")]
        public IActionResult GetMasterDataByMaterialNoAndFactory(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumberAndFactory, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialNoAndFactory(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumberAndFactory Success");
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
        [Route("GetMasterDatasByMatSaleOrgNonX")]
        public IActionResult GetMasterDatasByMatSaleOrgNonX(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMatSaleOrgNonX, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDatasByMatSaleOrgNonX(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMatSaleOrgNonX Success");
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
        [Route("GetMasterDataByMaterialNumberNonX")]
        public IActionResult GetMasterDataByMaterialNumberNonX(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string newMaterialNo = !string.IsNullOrEmpty(MaterialNo) ? MaterialNo.ToString() : "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString() + " , " + newMaterialNo);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialNumberNonX(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");
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
        [Route("GetMasterDataByMaterialNumberNonNotX")]
        public IActionResult GetMasterDataByMaterialNumberNonNotX(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string newMaterialNo = !string.IsNullOrEmpty(MaterialNo) ? MaterialNo.ToString() : "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumberNonNotX, Params : " + FactoryCode.ToString() + " , " + newMaterialNo);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialNumberNonNotX(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumberNonNotX Success");
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
        [Route("GetMasterDataByMaterialNumberX")]
        public IActionResult GetMasterDataByMaterialNumberX(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialNumberX(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");
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
        [Route("GetMasterDataByMaterialNoOnly")]
        public IActionResult GetMasterDataByMaterialNoOnly(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetByPredicate(m => m.MaterialNo.Equals(MaterialNo) && m.PdisStatus != "X"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");
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
        [Route("GetMasterDatasByMaterialNo")]
        public IActionResult GetMasterDatasByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string factoryCode = User.GetClaimValue("UserName") + "-" + (!string.IsNullOrEmpty(FactoryCode) ? FactoryCode.ToString() : "");
            string matNo = !string.IsNullOrEmpty(MaterialNo) ? MaterialNo.ToString() : null;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMaterialNumber, Params : " + factoryCode + " , " + matNo);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDatasByMaterialNumber(MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDatasByMaterialNumber Success");
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
        [Route("GetMasterDataList")]
        public IActionResult GetMasterDataRouting(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataList, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataList(config, FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataList Success");
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
        [Route("GetMasterDataTop100Update")]
        public IActionResult GetMasterDataTop100Update(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataTop100Update, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataTop100Update(FactoryCode));
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataTop100Update Success");
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
        [Route("GetMasterDataByBomChild")]
        public IActionResult GetMasterDataByBomChild(string FactoryCode, string MaterialNo, string Custcode, string ProductCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            MaterialNo ??= "";
            Custcode ??= "";
            ProductCode ??= "";

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByBomChild, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + Custcode.ToString() + " , " + ProductCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByBomChild(FactoryCode, MaterialNo, Custcode, ProductCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByBomChild Success");
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
        [Route("GetMasterDataByDescription")]
        public IActionResult GetMasterDataByDescription(string FactoryCode, string Description)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string descriptionValue = string.IsNullOrEmpty(Description) ? "" : Description;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByDescription, Params : " + FactoryCode.ToString() + " , " + descriptionValue);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByDescription(FactoryCode, Description));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByDescription Success");
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
        [Route("GetMasterByProdCode")]
        public IActionResult GetMasterByProdCode(string FactoryCode, string prodCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByProdCode, Params : " + FactoryCode.ToString() + " , " + prodCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByProdCode(FactoryCode, prodCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByProdCode Success");
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
        public IActionResult Post(string FactoryCode, [FromBody] ParentModel parentModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Add");
                _unitOfWork.MasterDatas.Add(parentModel.MasterData);
                string Username = User.GetClaimValue("UserName");
                string data = string.Empty;
                _unitOfWork.MasterDatas.UpdateIsTranfer(string.IsNullOrEmpty(FactoryCode) ? User.GetClaimValue("Factory") : FactoryCode, parentModel.MasterData.MaterialNo, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Add Success");
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
        [Route("UpdateProductCodeAndDescriptionFromPresaleNewMat")]
        public IActionResult UpdateProductCodeAndDescriptionFromPresaleNewMat(string FactoryCode, string ProductCode, string Description, string MaterialOriginal)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateProductCodeAndDescriptionFromPresaleNewMat");
                string Username = User.GetClaimValue("UserName");
                string data = string.Empty;
                _unitOfWork.MasterDatas.UpdateProductCodeAndDescriptionFromPresaleNewMat(config, FactoryCode, ProductCode, Description, MaterialOriginal, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateProductCodeAndDescriptionFromPresaleNewMat Success");
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
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update");

                string Username = User.GetClaimValue("UserName");
                var masterData = new MasterData();
                masterData = parentModel.MasterData;
                masterData.UpdatedBy = Username;
                masterData.LastUpdate = DateTime.Now;

                _unitOfWork.MasterDatas.Update(masterData);
                _unitOfWork.MasterDatas.UpdateIsTranfer(string.IsNullOrEmpty(FactoryCode) ? User.GetClaimValue("Factory") : FactoryCode, parentModel.MasterData.MaterialNo, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update Success");
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
        [Route("UpdateMasterDatas")]
        public IActionResult UpdateMasterDatas(string FactoryCode, [FromBody] List<MasterData> masterDatas)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDatas");

                string Username = User.GetClaimValue("UserName");

                _unitOfWork.MasterDatas.UpdateList(masterDatas);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDatas Success");
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
        [Route("SearchBomStructsByMaterialNo")]
        public IActionResult SearchBomStructsByMaterialNo(string FactoryCode, string materialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchBomStructsByMaterialNo, Params : " + FactoryCode.ToString() + " , " + materialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.SearchBomStructsByMaterialNoAll(FactoryCode, materialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchBomStructsByMaterialNo Success");
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
        [Route("SearchBomStructsBytxtSearch")]
        public IActionResult SearchBomStructsBytxtSearch(string FactoryCode, string txtSearch)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchBomStructsBytxtSearch, Params : " + FactoryCode.ToString() + " , " + txtSearch.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.SearchBomStructsBytxtSearchAll(FactoryCode, txtSearch));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchBomStructsBytxtSearch Success");
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
        [Route("GetMasterDataByKeySearch")]
        public IActionResult GetMasterDataByKeySearch(string FactoryCode, string typeSearch, string keySearch, string flag)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            typeSearch ??= "";
            keySearch ??= "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByKeySearch, Params : " + FactoryCode.ToString() + " , " + typeSearch.ToString() + " , " + keySearch.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByKeySearch(config, FactoryCode, typeSearch, keySearch, flag));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByKeySearch Success");
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
        [Route("GetMasterDataAllByKeySearch")]
        public IActionResult GetMasterDataAllByKeySearch(string FactoryCode, string keySearch)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataAllByKeySearch, Params : " + keySearch.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataAllByKeySearch(keySearch));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataAllByKeySearch Success");
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
        [Route("GetMasterDataByUser")]
        public IActionResult GetMasterDataByUser(string FactoryCode, string UserString)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByUser, Params : " + User);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByUser(FactoryCode, UserString));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByUser Success");
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
        [Route("GetMasterDataByProdCode")]
        public IActionResult GetMasterDataByProdCode(string FactoryCode, string prodCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            prodCode = !string.IsNullOrEmpty(prodCode) ? prodCode.ToString() : "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByProdCode, Params : " + FactoryCode.ToString() + " , " + prodCode);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByProdCode(FactoryCode, prodCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByProdCode Success");
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
        [Route("UpdatePDISStatus")]
        public IActionResult UpdatePDISStatus(string FactoryCode, string MaterialNo, string Status)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdatePDISStatus, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + Status.ToString());
                string Username = User.GetClaimValue("UserName");
                _unitOfWork.MasterDatas.UpdatePDISStatus(FactoryCode, MaterialNo, Status, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdatePDISStatus Success");
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

        //InterfaceHandShake
        [HttpGet]
        [Route("GetMasterDataHandshake")]
        public IActionResult GetMasterDataHandshake(string FactoryCode, string saleOrg)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataHandshake, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataHandshake(FactoryCode, saleOrg));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataHandshake Success");
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

        //InterfaceHandShake
        [HttpGet]
        [Route("GetMasterDataHandshakeOCG")]
        public IActionResult GetMasterDataHandshakeOCG(string FactoryCode, string saleOrg)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataHandshakeOCG, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataHandshakeOCG(FactoryCode, saleOrg));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataHandshakeOCG Success");
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
        [Route("UpdateMasterDataTransStatus")]
        public IActionResult UpdateMasterDataTransStatus(string FactoryCode, [FromBody] UpdateMatModel updateMatModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool Status = true;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                var material = updateMatModel.MatMasters;
                string MatHandshake = "";
                foreach (var mat in material)
                {
                    MatHandshake = MatHandshake + ",'" + mat.MaterialNo.ToString() + "'";
                }

                var materialno = MatHandshake.Remove(0, 1);

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateTranStatus, Params : " + FactoryCode.ToString() + " , " + materialno.ToString() + " , " + Status.ToString());
                string Username = User.GetClaimValue("UserName");
                _unitOfWork.MasterDatas.UpdateTranStatusFromHandshake(config, FactoryCode, materialno, Status, Username);

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateTranStatus Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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
        [Route("UpdateMasterDataTransStatusByBomStruct")]
        public IActionResult UpdateMasterDataTransStatusByBomStruct(string FactoryCode, string MaterialNo, string StatusCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            bool Status = true;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataTransStatusByBomStruct, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + Status.ToString());
                string Username = User.GetClaimValue("UserName");
                _unitOfWork.MasterDatas.UpdateTranStatusByBomStruct(FactoryCode, MaterialNo, StatusCode, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataTransStatusByBomStruct Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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
        [Route("UpdateCapImgTransactionDetail")]
        public IActionResult UpdateCapImgTransactionDetail(string FactoryCode, string MaterialNo, string StatusCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            bool Status = true;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateCapImgTransactionDetail, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + Status.ToString());
                _unitOfWork.MasterDatas.UpdateCapImgTransactionDetails(FactoryCode, MaterialNo, StatusCode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateCapImgTransactionDetail Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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

        //Product catalog
        [HttpPost]
        [Route("GetProductCatalog")]
        public IActionResult GetProductCatalog(string FactoryCode, [FromBody] ProductCatalogsSearch ProductCatalogModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalog, Params : " + FactoryCode);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetProductCatalog(config, FactoryCode, ProductCatalogModel));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalog Success");
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
        [Route("GetProductCatalogEOR")]
        public IActionResult GetProductCatalogEOR(string FactoryCode, [FromBody] ProductCatalogsSearch ProductCatalogModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalog, Params : " + FactoryCode);
                var data = (_unitOfWork.MasterDatas.GetProductCatalog(config, FactoryCode, ProductCatalogModel)).ToList();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalog Success");
                return Json(new { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
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
                return Json(new { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("GetProductCatalogNotop")]
        public IActionResult GetProductCatalogNotop(string FactoryCode, [FromBody] ProductCatalogsSearch ProductCatalogModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalogNoTop, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetProductCatalogNotop(config, FactoryCode, ProductCatalogModel));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetProductCatalogNoTop Success");
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
        [Route("GetCountProductCatalogNotop")]
        public IActionResult GetCountProductCatalogNotop(string FactoryCode, [FromBody] ProductCatalogsSearch ProductCatalogModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetCountProductCatalogNotop, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetCountProductCatalogNotop(config, FactoryCode, ProductCatalogModel));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetCountProductCatalogNotop Success");
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

        // ----- Get Picture Path -------
        [HttpGet]
        [Route("GetMasterDataDiecutPath")]
        //[Authorize(Roles = "NerpSystem")]
        public IActionResult GetMasterDataDiecutPath(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath, Params : " + FactoryCode.ToString());
                string pathdiecut = _unitOfWork.MasterDatas.GetMasterDataDiecutPath(FactoryCode, MaterialNo);
                if (string.IsNullOrEmpty(pathdiecut))
                {
                    exceptionMessage = "Diecut Path IsNullOrEmpty";
                    string data = "";
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath Success");
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
                else
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(pathdiecut);
                    string data = "data:image/jpg;base64," + Convert.ToBase64String(imageArray);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath Success");
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = "Nodata";
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        // ----- Get Picture Path -------
        [HttpGet]
        [Route("GetMasterDataPalletPath")]
        //[Authorize(Roles = "NerpSystem")]
        public IActionResult GetMasterDataPalletPath(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath, Params : " + FactoryCode.ToString());
                //data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataDiecutPath(FactoryCode,  MaterialNo));
                string pathdiecut = _unitOfWork.MasterDatas.GetMasterDataPalletPath(FactoryCode, MaterialNo);
                if (string.IsNullOrEmpty(pathdiecut))
                {
                    exceptionMessage = "Pallet Path IsNullOrEmpty";
                    string data = "";
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath Success");
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
                else
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(pathdiecut);
                    string data = "data:image/jpg;base64," + Convert.ToBase64String(imageArray);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataDiecutPath Success");
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    // exceptionMessage = ex.Message;
                    exceptionMessage = "Nodata";
                }
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("UpdateLotsMasterData")]
        public IActionResult UpdatePCandDescription(string FactoryCode, string userUpdate, string flagUpdate, [FromBody] UpdateMatModel updateMatModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                var conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update");
                var masterRow = updateMatModel.MatMasters;
                conn.Open();
                string str = "";
                string abc = "";
                string Username = User.GetClaimValue("UserName");
                if (flagUpdate == "upData")
                {
                    foreach (var m in masterRow)
                    {
                        //str = $" Update [dbo].[MasterData] SET ";
                        //str = string.IsNullOrWhiteSpace(m.PC) ? str + $" " : str + $" PC = '{m.PC}' ,";
                        //str = string.IsNullOrWhiteSpace(m.Description) ? str + $" " : str + $" Description = '{m.Description}' ,";
                        //str = str + $" PDIS_Status = 'M', Tran_Status = 0, LastUpdate = GETDATE(), UpdatedBy = '{userUpdate}', [User] = 'ZZ_Import' ";
                        //str = str + $" \n Where FactoryCode = '{FactoryCode}' and Material_No = '{m.MaterialNo}' \n";

                        str = $" Update m SET ";
                        str = string.IsNullOrWhiteSpace(m.PC) ? str + $" " : str + $" m.PC = '{m.PC}' ,";
                        str = string.IsNullOrWhiteSpace(m.Description) ? str + $" " : str + $" m.Description = '{m.Description}' ,";
                        str += $" m.PDIS_Status = 'M', m.Tran_Status = case when m.Sale_Org <> c.Sale_Org then 1 else 0 end, m.LastUpdate = GETDATE(), m.UpdatedBy = '{userUpdate}', m.[User] = 'ZZ_Import' ";
                        str += $" \n from MasterData m left outer join CompanyProfile c on c.Plant = m.FactoryCode Where m.FactoryCode = '{FactoryCode}' and m.Material_No = '{m.MaterialNo}' \n";

                        // send query string to repos
                        var command = new SqlCommand(str, conn);
                        var reader = command.ExecuteReader();
                        _unitOfWork.MasterDatas.UpdateIsTranfer(string.IsNullOrEmpty(FactoryCode) ? User.GetClaimValue("Factory") : FactoryCode, m.MaterialNo, Username);
                    }
                }
                else if (flagUpdate == "upStatus")
                {
                    foreach (var m in masterRow)
                    {
                        abc = abc + "'" + m.MaterialNo + "',";
                        _unitOfWork.MasterDatas.UpdateIsTranfer(string.IsNullOrEmpty(FactoryCode) ? User.GetClaimValue("Factory") : FactoryCode, m.MaterialNo, Username);
                    }
                    abc = abc[..^1];

                    //str = $" Update [dbo].[MasterData] SET ";
                    //str = str + $" PDIS_Status = 'X', Tran_Status = 0, LastUpdate = GETDATE(), UpdatedBy = '{userUpdate}', [User] = 'ZZ_Import'";
                    //str = str + $" \n Where FactoryCode = '{FactoryCode}' and Material_No in ({abc}) \n";

                    str = $" Update m SET ";
                    str += $" m.PDIS_Status = 'X', m.Tran_Status = case when m.Sale_Org <> c.Sale_Org then 1 else 0 end, m.LastUpdate = GETDATE(), m.UpdatedBy = '{userUpdate}', m.[User] = 'ZZ_Import'";
                    str += $" \n from MasterData m left outer join CompanyProfile c on c.Plant = m.FactoryCode Where m.FactoryCode = '{FactoryCode}' and m.Material_No in ({abc}) \n";

                    var command = new SqlCommand(str, conn);
                    var reader = command.ExecuteReader();
                }

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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
        [Route("UpdateReuseMaterialNos")]
        public IActionResult UpdateReuseMaterialNos(string FactoryCode, [FromBody] ParentModel parentModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateReuseMaterialNos");
                string Username = User.GetClaimValue("UserName");
                _unitOfWork.MasterDatas.UpdateReuseMaterialNos(parentModel.MasterDataList, parentModel.RoutingList, parentModel.PlantViewList, parentModel.SalesViewList, parentModel.TransactionsDetailList, Username);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateReuseMaterialNos Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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
        [Route("CreateChangeBoardNewMaterial")]
        public IActionResult CreateChangeBoardNewMaterial(string AppName, string FactoryCode, string Username, string IsCheckImport, [FromBody] List<ChangeBoardNewMaterial> changeBoardNewMaterials)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            var conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.CreateChangeBoardNewMaterial, Params : FactoryCode = " + FactoryCode.ToString() + ", User =" + User + ",JsonData = " + JsonConvert.SerializeObject(changeBoardNewMaterials));
                List<ChangeBoardNewMaterial> results = _unitOfWork.MasterDatas.CreateChangeBoardNewMaterial(conn, FactoryCode, Username, Convert.ToBoolean(IsCheckImport), changeBoardNewMaterials).ToList();
                string data = JsonConvert.SerializeObject(results);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.CreateChangeBoardNewMaterial Success");
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
        [Route("CreateChangeFactoryNewMaterial")]
        public IActionResult CreateChangeFactoryNewMaterial(string AppName, string FactoryCode, string Username, string IsCheckImport, [FromBody] List<ChangeBoardNewMaterial> changeBoardNewMaterials)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            bool isAuthen = false;
            var conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.CreateChangeBoardNewMaterial, Params : FactoryCode = " + FactoryCode.ToString() + ", User =" + User + ",JsonData = " + JsonConvert.SerializeObject(changeBoardNewMaterials));
                List<ChangeBoardNewMaterial> results = _unitOfWork.MasterDatas.CreateChangeFactoryNewMaterial(conn, FactoryCode, Username, Convert.ToBoolean(IsCheckImport), changeBoardNewMaterials).ToList();
                string data = JsonConvert.SerializeObject(results);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.CreateChangeBoardNewMaterial Success");
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
        [Route("UpdateRoutingFromExcelFile")]
        public IActionResult UpdateRoutingsFromExcelFile(string FactoryCode, string Username, [FromBody] List<Routing> routings)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateRoutingFromExcelFile, Params : " + FactoryCode.ToString() + " , " + Username.ToString() + "," + JsonConvert.SerializeObject(routings));
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.UpdateRoutingsFromExcelFile(routings));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateRoutingFromExcelFile Success");
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
        [Route("UpdateMasterDatasFromExcelFile")]
        public IActionResult UpdateMasterDataFromExcelFile(string FactoryCode, string Username, [FromBody] List<MasterData> masterDatas)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataFromExcelFile, Params : " + FactoryCode.ToString() + " , " + Username.ToString() + "," + JsonConvert.SerializeObject(masterDatas));
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.UpdateMasterDatasFromExcelFile(masterDatas));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataFromExcelFile Success");
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
        [Route("GetMasterDataByMaterialAddtag")]
        public IActionResult GetMasterDataByMaterialAddtag(string FactoryCode, string ddlSearch, string inputSerach)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataByMaterialAddtag(FactoryCode, ddlSearch, inputSerach));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");
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
        [Route("UpdateMasterDataByChangePalletSize")]
        public IActionResult UpdateMasterDataByChangePalletSize(string FactoryCode, string UserLogin, [FromBody] MasterData masterData)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataByChangePalletSize, Params : " + FactoryCode + " , " + JsonConvert.SerializeObject(masterData));
                _unitOfWork.MasterDatas.UpdateMasterDataByChangePalletSize(FactoryCode, UserLogin, masterData);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateMasterDataByChangePalletSize Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = string.Empty });
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
        [Route("SearchMasterDataByMaterialNo")]
        public IActionResult SearchMasterDataByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchMasterDataByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.SearchMasterDataByMaterialNo(MaterialNo, FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.SearchMasterDataByMaterialNo Success");
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
        [Route("GetMasterDataListByMaterialNoAndPC")]
        public IActionResult GetMasterDataListByMaterialNoAndPC(string FactoryCode, string MaterialNo, string PC)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataListByMaterialNoAndPC, Params : " + FactoryCode.ToString() + " , " + (string.IsNullOrEmpty(MaterialNo) ? string.Empty : MaterialNo) + " , " + (string.IsNullOrEmpty(PC) ? string.Empty : PC));
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataListByMaterialNoAndPC(config, FactoryCode, MaterialNo, PC));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataListByMaterialNoAndPC Success");
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
        [Route("GetMasterDataListByDateTime")]
        public IActionResult GetMasterDataListByDateTime(string FactoryCode, string DateFrom, string DateTo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + "," + DateFrom + "," + DateTo);
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetMasterDataListByDateTime(FactoryCode, DateFrom, DateTo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetBoardDistinctFromMasterData")]
        public IActionResult GetBoardDistinctFromMasterData(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetBoardDistinctFromMasterData, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetBoardDistinctFromMasterData(FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetBoardDistinctFromMasterData Success");
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
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Route("GetCustomerDistinctFromMasterData")]
        public IActionResult GetCustomerDistinctFromMasterData(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetCustomerDistinctFromMasterData, Params : " + FactoryCode.ToString());
                string data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetCustomerDistinctFromMasterData(FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetCustomerDistinctFromMasterData Success");
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
                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("GetForTemplateChangeBoardNewMaterials")]
        public IActionResult GetForTemplateChangeBoardNewMaterials(string FactoryCode, [FromBody] SearchMaterialTemplateParam searchMaterialTemplateParam)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetForTemplateChangeBoardNewMaterials, Params : FactoryCode = " + FactoryCode.ToString() + ",JsonData = " + JsonConvert.SerializeObject(searchMaterialTemplateParam));
                List<ChangeBoardNewMaterial> results = _unitOfWork.MasterDatas.GetForTemplateChangeBoardNewMaterials(FactoryCode, searchMaterialTemplateParam);
                string data = JsonConvert.SerializeObject(results);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetForTemplateChangeBoardNewMaterials Success");
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
        [Route("ReportCheckStatusColor")]
        public IActionResult ReportCheckStatusColor(string FactoryCode, int ColorId)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportMOManual, Params : " + FactoryCode.ToString() + "," + ColorId);
                data = JsonConvert.SerializeObject(_unitOfWork.MasterDatas.GetReportCheckStatusColor(config, FactoryCode, ColorId));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportMOManual Success");
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
                Logger.Error(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }
    }
}