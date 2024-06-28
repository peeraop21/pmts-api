using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.Models;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs,TipsSystem")]
    [ApiController]
    public class MoDataController(PMTsDbContext pmtsContext, IMapper mapper, IConfiguration configuration, IStringLocalizerFactory localizer) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration config = configuration;
        private readonly IStringLocalizer _localizer = localizer.Create("Views.MoData.MasterCardMO", Assembly.GetExecutingAssembly().GetName().Name);
        private PMTsDbContext pmtsContext = pmtsContext;

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetAllByFactory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetAllByFactory(m => m.FactoryCode == FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetAllByFactory Success");
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

        [HttpGet]
        [Route("GetMoDataListBySaleOrder")]
        public IActionResult GetMoDataListBySaleOrder(string FactoryCode, string StratSO, string EndSO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + StratSO.ToString() + " , " + EndSO.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMasterDataListBySO(FactoryCode, StratSO, EndSO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMoDataListBySBOExNo")]
        public IActionResult GetMoDataListBySBOExNo(string FactoryCode, string StratSO, string EndSO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + StratSO.ToString() + " , " + EndSO.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataListBySBOExNo(FactoryCode, StratSO, EndSO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMODataListBySONonX")]
        public IActionResult GetMODataListBySONonX(string FactoryCode, string StratSO, string EndSO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                if (EndSO == null)
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + StratSO.ToString());
                else
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + StratSO.ToString() + " , " + EndSO.ToString());

                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMODataListBySONonX(config, FactoryCode, StratSO, EndSO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMoDataListBySearchTypeNonX")]
        public IActionResult GetMoDataListBySearchTypeNonX(string FactoryCode, string SearchType, string SearchText)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                var param = string.Join(',', new { FactoryCode, SearchType, SearchText });
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + param);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMODataListBySearchTypeNonX(config, FactoryCode, SearchType, SearchText));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMoDataBySaleOrderNonX")]
        public IActionResult GetMoDataBySaleOrderNonX(string FactoryCode, string SaleOrder)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + SaleOrder.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetByPredicate(m => m.OrderItem.Equals(SaleOrder) && m.FactoryCode.Equals(FactoryCode) && m.MoStatus.ToLower() != "x"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMODataListBySONonXAndH")]
        public IActionResult GetMODataListBySONonXAndH(string FactoryCode, string StratSO, string EndSO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataListBySONonXAndH, Params : " + FactoryCode.ToString() + " , " + StratSO.ToString() + " , " + EndSO.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMODataListBySONonXAndH(config, FactoryCode, StratSO, EndSO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataListBySONonXAndH Success");
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

        [HttpGet]
        [Route("SearchMODataListBySONonXAndH")]
        public IActionResult SearchMODataListBySONonXAndH(string FactoryCode, string StratSO, string EndSO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataListBySONonXAndH, Params : " + FactoryCode + " , " + StratSO + " , " + EndSO);
                if (string.IsNullOrEmpty(EndSO))
                {
                    EndSO = StratSO;
                }
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.SearchMODataListBySONonXAndH(config, FactoryCode, StratSO, EndSO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataListBySONonXAndH Success");
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

        [HttpGet]
        [Route("GetMoDatasBySaleOrderNonX")]
        public IActionResult GetMoDatasBySaleOrderNonX(string FactoryCode, string SaleOrder)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + " , " + SaleOrder.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetAllByFactoryTake100(m => m.OrderItem.Contains(SaleOrder) && m.FactoryCode.Equals(FactoryCode) && m.MoStatus.ToLower() != "x"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        public static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
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
        [Route("GetMoDataListBySaleOrders")]
        public IActionResult GetMoDataListBySaleOrders(string FactoryCode, string SaleOrders, [FromBody] List<string> saleOrderList)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMasterDataListBySaleOrders(config, FactoryCode, saleOrderList));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders Success");
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

        [HttpPost]
        [Route("GetMoDataListBySaleOrdersByDapper")]
        public IActionResult GetMoDataListBySaleOrdersByDapper(string FactoryCode, string SaleOrders, [FromBody] List<string> saleOrderList)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataListBySaleOrdersByDapper(config, FactoryCode, saleOrderList));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders Success");
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

        [HttpGet]
        [Route("GetMoDataBySuffixSO")]
        public IActionResult GetMoDataBySuffixSO(string FactoryCode, string SO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySuffixSO, Params : " + FactoryCode.ToString() + " , " + SO.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataBySuffixSO(FactoryCode, SO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySuffixSO Success");
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

        [HttpGet]
        [Route("GetMoDataListBySuffixSO")]
        public IActionResult GetMoDataListBySuffixSO(string FactoryCode, string SO)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySuffixSO, Params : " + FactoryCode.ToString() + " , " + SO.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataListBySuffixSO(FactoryCode, SO));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySuffixSO Success");
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

        [HttpPost]
        public IActionResult Post(string FactoryCode, [FromBody] ParentModel model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add");
                _unitOfWork.MoDatas.Add(model.MoData);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add Success");
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

        [HttpPost]
        [Route("CreateLogPrintMO")]
        public IActionResult CreateLogPrintMO(string FactoryCode, [FromBody] LogPrintMo logPrintMo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "CreateLogPrintMO.Add");
                _unitOfWork.LogPrintMo.Add(logPrintMo);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "CreateLogPrintMO.Add Success");
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

        [HttpPost]
        [Route("CreateMODataFromExcelFile")]
        public IActionResult CreateMODataFromExcelFile(string FactoryCode, string Username, [FromBody] MOManualModel mOManualModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add");
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.AddManualMOData(mOManualModel.MoData, mOManualModel.MoSpec, mOManualModel.MoRoutings, new List<RunningNo>(), new AttachFileMo()));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add Success");
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

        [HttpPut]
        public IActionResult Put(string FactoryCode, [FromBody] MoData model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Update");
                _unitOfWork.MoDatas.Update(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Update Success");
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

        [HttpPost]
        [Route("SaveMoDataFromInterfaceMO")]
        public IActionResult SaveMoDataFromInterfaceMO(string FactoryCode, string OrderItem, [FromBody] MoData model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add");
                List<MoData> itemToDelete = _unitOfWork.MoDatas.GetMoDataListByOrderItem(FactoryCode, OrderItem);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetAllByPredicate Success");
                if (itemToDelete.Count() > 0)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.RemoveRange");
                    _unitOfWork.MoDatas.RemoveRange(itemToDelete);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.RemoveRange Success");
                }
                _unitOfWork.MoDatas.Add(model);
                //data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.AddManualMOData(mOManualModel.MoData, mOManualModel.MoSpec, mOManualModel.MoRoutings, new List<RunningNo>(), new AttachFileMO()));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.Add Success");
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

        [HttpPost]
        [Route("UpdateMoDataSentKIWI")]
        public IActionResult UpdateMoDataSentKIWI(string FactoryCode, string SaleOrder, string UserBy)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdateMoDataSentKIWI");
                _unitOfWork.MoDatas.UpdateMoDataSentKIWI(FactoryCode, SaleOrder, UserBy);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdateMoDataSentKIWI Success");
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

        [HttpGet]
        [Route("GetMoDataManualListToSendKIWI")]
        public IActionResult GetMoDataManualListToSendKIWI(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataManualListToSendKIWI(FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpPost]
        [Route("UpdateSentKIWI")]
        public IActionResult UpdateSentKIWI(string FactoryCode, string SaleOrder, string SendKiwi, string UpdateBy)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdateSentKIWI");
                _unitOfWork.MoDatas.UpdateMoDataSentKIWI(FactoryCode, SaleOrder, bool.Parse(SendKiwi), UpdateBy);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdateSentKIWI Success");
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

        [HttpGet]
        [Route("GetMoDataListByDateTime")]
        public IActionResult GetMoDataListByDateTime(string FactoryCode, string DateFrom, string DateTo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + "," + DateFrom + "," + DateTo);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataListByDateTime(FactoryCode, DateFrom, DateTo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO Success");
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

        [HttpGet]
        [Route("GetMoDatasByDueDateRange")]
        public IActionResult GetMoDatasByDueDateRange(string FactoryCode, string StartDueDate, string EndDueDate)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDatasByDueDateRange, Params : " + FactoryCode.ToString() + "," + StartDueDate + "," + EndDueDate);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDatasByDueDateRange(config, FactoryCode, Convert.ToDateTime(StartDueDate), Convert.ToDateTime(EndDueDate)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDatasByDueDateRange Success");
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

        [HttpGet]
        [Route("GetMoDatasByDueDateRangeAndStatus")]
        public IActionResult GetMoDatasByDueDateRangeAndStatus(string FactoryCode, string Status, string StartDueDate, string EndDueDate)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDatasByDueDateRangeAndStatus, Params : " + FactoryCode.ToString() + "," + StartDueDate + "," + EndDueDate);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDatasByDueDateRangeAndStatus(config, FactoryCode, Status, Convert.ToDateTime(StartDueDate), Convert.ToDateTime(EndDueDate)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDatasByDueDateRangeAndStatus Success");
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

        [HttpPost]
        [Route("CreateMOManual")]
        public IActionResult CreateMOManual([FromBody] List<MoData> moDatas)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            var result = new List<MoData>();
            var moDataResult = new MoData();
            var runningNos = new List<RunningNo>();
            var attachFileMo = new AttachFileMo();
            var FactoryCode = moDatas.Count > 0 ? moDatas.FirstOrDefault().FactoryCode : string.Empty;

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "CreateMOManual Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "VMIService.CreateMOManual");
                foreach (var moData in moDatas)
                {
                    try
                    {
                        moDataResult = new MoData();
                        runningNos = new List<RunningNo>();
                        var masterdata = _unitOfWork.MasterDatas.GetByPredicate(m => m.MaterialNo == moData.MaterialNo && m.FactoryCode == FactoryCode && m.PdisStatus.ToLower().Trim() != "x");

                        if (masterdata == null)
                        {
                            moData.OrderItem = "";
                            moDataResult = moData;
                        }
                        else
                        {
                            var runningNO = new RunningNo();
                            var moSpec = new MoSpec();
                            var moRoutings = new List<MoRouting>();

                            var userLogon = moDatas != null && moDatas.Count > 0 ? "ZZ_Import" : moData.UpdatedBy;
                            var flute = _unitOfWork.Flutes.GetFluteByFlute(masterdata.FactoryCode, masterdata.Flute);

                            #region Set MO Data

                            var custRecvTime = string.Empty;
                            var soldTo = moData.SoldTo ?? "";

                            var custShips = _unitOfWork.CustShipTos.GetCustShipToListByShipTo(soldTo).ToList(); //Singleton.GetCustShipTos().Where(x => x.ShipTo == soldTo).ToList();
                            if (custShips.Count > 0)
                            {
                                custRecvTime = custShips.OrderBy(o => o.Seq)?.FirstOrDefault()?.CustRecvTime;
                            }
                            else
                            {
                                while (soldTo.StartsWith("0"))
                                {
                                    soldTo = soldTo.Substring(1, soldTo.Length - 1);
                                }
                                custShips = _unitOfWork.CustShipTos.GetCustShipToListByShipTo(soldTo).ToList();
                                if (custShips.Count > 0)
                                {
                                    custRecvTime = custShips.OrderBy(o => o.Seq)?.FirstOrDefault()?.CustRecvTime;
                                }
                                else
                                {
                                    custRecvTime = string.Empty;
                                }
                            }
                            moData.Id = 0;
                            moData.Printed = 0;
                            var culture = new CultureInfo("en-US");
                            moData.DateTimeStamp = DateTime.Now.ToString("yMMddHHmmss", culture);
                            moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
                            moData.OriginalDueDate = moData.DueDate;
                            moData.MoStatus = !string.IsNullOrEmpty(moData.ItemNote) && moData.ItemNote.Trim().ToUpper().StartsWith("SSS") ? "S" : "C";//"C";
                            moData.PlanStatus = null;
                            moData.StockQty = null;
                            moData.SentKiwi = false;
                            moData.IsCreateManual = true;
                            moData.CreatedDate = DateTime.Now;
                            moData.CreatedBy = userLogon;
                            moData.UpdatedDate = DateTime.Now;
                            moData.UpdatedBy = userLogon;
                            moData.Name = !string.IsNullOrEmpty(masterdata.CustName) ? masterdata.CustName.Trim().Replace("'", "") : masterdata.CustName;
                            moData.ToleranceOver = moData.ToleranceOver != null && moData.ToleranceOver.HasValue ? moData.ToleranceOver : 0;
                            moData.CustRecvTime = custRecvTime;

                            #endregion Set MO Data

                            #region Set MO Spec

                            moSpec = _mapper.Map<MasterData, MoSpec>(masterdata);
                            moSpec.Id = 0;
                            moSpec.User = userLogon;
                            moSpec.OrderItem = moData.OrderItem;
                            moSpec.LastUpdate = DateTime.Now;
                            moSpec.CreateDate = DateTime.Now;
                            moSpec.FluteDesc = flute != null ? flute.Description : moSpec.FluteDesc;

                            #endregion Set MO Spec

                            #region Set MO Routing

                            var routings = new List<Routing>();
                            routings = _unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, masterdata.MaterialNo).ToList();

                            foreach (var routing in routings)
                            {
                                var moRouting = new MoRouting();
                                moRouting = _mapper.Map<MoRouting>(routing);
                                moRouting.Id = 0;
                                moRouting.Plant = moData.FactoryCode;
                                moRouting.FactoryCode = moData.FactoryCode;
                                moRouting.OrderItem = moData.OrderItem;
                                moRoutings.Add(moRouting);
                            }

                            if (moData.OrderQuant >= 0)
                            {
                                var routingCorr = routings.FirstOrDefault(r => r.FactoryCode.Equals(FactoryCode) && r.MaterialNo.Equals(moData.MaterialNo) && r.MatCode.Contains("COR") && r.PdisStatus != "X");
                                var cutNO = routingCorr != null && routingCorr.CutNo.HasValue && routingCorr.CutNo != null ? routingCorr.CutNo.Value : 0;
                                moData.TargetQuant = moData.TargetQuant == 0 ? _unitOfWork.Formulas.CalculateMoTargetQuantity(FactoryCode, moData.OrderQuant, moData.ToleranceOver.Value, masterdata.Flute, moData.MaterialNo, cutNO) : moData.TargetQuant;
                                moData.TargetQuant = moData.TargetQuant == -1 ? 0 : moData.TargetQuant;
                            }
                            else
                            {
                                throw new Exception("Can't calculate order quantity.");
                            }

                            #endregion Set MO Routing

                            #region Set AttachFile From MasterData

                            if (!string.IsNullOrEmpty(masterdata.AttachFileMoPath))
                            {
                                attachFileMo = new AttachFileMo
                                {
                                    Id = 0,
                                    FactoryCode = moData.FactoryCode,
                                    OrderItem = moData.OrderItem,
                                    PathInit = Path.GetFileName(masterdata.AttachFileMoPath),
                                    PathNew = masterdata.AttachFileMoPath,
                                    SeqNo = 1,
                                    Status = true,
                                    UpdatedBy = moData.UpdatedBy,
                                    UpdatedDate = DateTime.Now,
                                    CreatedBy = moData.UpdatedBy,
                                    CreatedDate = DateTime.Now,
                                };
                            }

                            #endregion Set AttachFile From MasterData

                            #region Genarate orderItem

                            runningNO = _unitOfWork.RunningNos.GetRunningNoByGroupId(FactoryCode, "SO");

                            if (runningNO != null && !string.IsNullOrEmpty(moData.OrderItem))
                            {
                                if (Convert.ToInt32(moData.OrderItem.Substring(6, 4)) <= Convert.ToInt32(runningNO.Running))
                                {
                                    moData.OrderItem = GenSaleOrderNumber(runningNO);
                                    moSpec.OrderItem = moData.OrderItem;
                                    moRoutings.ForEach(r => r.OrderItem = moData.OrderItem);
                                }
                            }
                            else if (string.IsNullOrEmpty(moData.OrderItem))
                            {
                                moData.OrderItem = GenSaleOrderNumber(runningNO);
                                moSpec.OrderItem = moData.OrderItem;
                                moRoutings.ForEach(r => r.OrderItem = moData.OrderItem);
                            }

                            moData.SoKiwi = moData.OrderItem.Length > 10 ? moData.OrderItem.Remove(0, moData.OrderItem.Length - 10) : moData.OrderItem;

                            #endregion Genarate orderItem

                            #region Update running number

                            var Running = 0;
                            var thisCompanyProfile = _unitOfWork.CompanyProfiles.GetByPredicate(c => c.Plant == FactoryCode);
                            var companyProfiles = _unitOfWork.CompanyProfiles.GetAllByFactory(c => c.SaleOrg == thisCompanyProfile.SaleOrg);

                            //runningNO = _unitOfWork.RunningNos.GetRunningNoByGroupId(FactoryCode, "SO");
                            if (runningNO != null)
                            {
                                Running = runningNO.Running + 1;
                                if (runningNO.UpdatedDate.HasValue && (runningNO.UpdatedDate.Value.Month != DateTime.Now.Month))
                                {
                                    Running = 1;
                                }
                            }

                            foreach (var companyProfile in companyProfiles)
                            {
                                var runningNoObject = _unitOfWork.RunningNos.GetRunningNoByGroupId(companyProfile.Plant, "SO");

                                // Running Number
                                runningNoObject.Running = Running;
                                runningNoObject.UpdatedBy = userLogon;
                                runningNoObject.UpdatedDate = DateTime.Now;

                                if (runningNoObject.FactoryCode != FactoryCode && runningNoObject.UpdatedDate.HasValue && (runningNoObject.UpdatedDate.Value.Month != DateTime.Now.Month))
                                {
                                    runningNoObject.Running = 0;
                                }

                                //_unitOfWork.RunningNos.Update(runningNoObject);
                                runningNos.Add(runningNoObject);
                            }

                            #endregion Update running number

                            var errorMessage = _unitOfWork.MoDatas.AddManualMOData(moData, moSpec, moRoutings, runningNos, attachFileMo);

                            if (string.IsNullOrEmpty(moData.OrderItem))
                            {
                                moData.OrderItem = string.Empty;
                                throw new Exception(errorMessage);
                            }

                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                moData.OrderItem = string.Empty;
                                throw new Exception(errorMessage);
                            }

                            result.Add(moData);
                        }
                    }
                    catch
                    {
                        moData.OrderItem = "";
                        result.Add(moData);
                        continue;
                    }
                }

                data = JsonConvert.SerializeObject(result);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "VMIService.CreateMOManual Success");
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

        [HttpPost]
        [Route("ReportCheckRepeatOrder")]
        public IActionResult ReportCheckRepeatOrder(string FactoryCode, string dateFrom, string dateTo, string repeatCount)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportCheckRepeatOrder, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.CheckRepeatOrder(config, FactoryCode, dateFrom, dateTo, Convert.ToInt32(repeatCount)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportCheckRepeatOrder Success");
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

        [HttpPost]
        [Route("ReportCheckOrderQtyTooMuch")]
        public IActionResult ReportCheckOrderQtyTooMuch(string FactoryCode, string dateFrom, string dateTo, string repeatCount)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportCheckOrderQtyTooMuch, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.CheckOrderQtyTooMuch(config, FactoryCode, dateFrom, dateTo, Convert.ToInt32(repeatCount)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportCheckOrderQtyTooMuch Success");
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

        [HttpPost]
        [Route("CheckDiffDueDate")]
        public IActionResult CheckDiffDueDate(string FactoryCode, int datediff, string dateFrom, string dateTo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckDiffDueDate, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.CheckDiffDueDate(config, FactoryCode, datediff, Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo)).OrderBy(x => x.DueDate));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckDiffDueDate Success");
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

        [HttpPost]
        [Route("CheckDueDateToolong")]
        public IActionResult CheckDueDateToolong(string FactoryCode, int dayCount)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckDueDateToolong, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.CheckDueDateToolong(config, FactoryCode, dayCount));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckDueDateToolong Success");
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

        [HttpPost]
        [Route("GetReportCheckData")]
        public IActionResult GetReportCheckData(string Username, string FactoryCode, string StartDate, string EndDate)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetReportCheckData, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetReportCheck(config, Username, FactoryCode, StartDate, EndDate));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetReportCheckData Success");
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

        [HttpPost]
        [Route("ReportMOManual")]
        public IActionResult ReportMOManual(string FactoryCode, string MaterialNo, string CustName, string PC, string StartDueDate, string EndDueDate, string StartCreateDate, string EndCreateDate, string StartUpdateDate, string EndUpdateDate, string PO, string SO, string Note, string SoStatus)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.ReportMOManual, Params : " + FactoryCode.ToString() + "," + MaterialNo + "," + CustName + "," + PC);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetDataReportMoManual(config, FactoryCode, MaterialNo, CustName, PC, StartDueDate, EndDueDate, StartCreateDate, EndCreateDate, StartUpdateDate, EndUpdateDate, PO, SO, Note, SoStatus));
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

        [HttpGet]
        [Route("GetMoDataBySOKiwi")]
        public IActionResult GetMoDataBySOKiwi(string FactoryCode, string SO_Kiwi)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySOKiwi, Params : " + FactoryCode + "," + SO_Kiwi);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataBySOKiwi(FactoryCode, SO_Kiwi));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataBySOKiwi Success");
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

        [HttpPost]
        [Route("GetMasterCardMOsBySaleOrders")]
        public IActionResult GetMasterCardMOsBySaleOrders(string FactoryCode, [FromBody] List<string> saleOrderList)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMasterCardMOsBySaleOrders(config, FactoryCode, saleOrderList));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataListBySaleOrders Success");
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

        [HttpPost]
        [Route("GetBaseOfMasterCardMOsBySaleOrders")]
        public IActionResult GetBaseOfMasterCardMOsBySaleOrders(string FactoryCode, bool isUserTIPs, [FromBody] List<string> saleOrderList)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBaseOfMasterCardMOsBySaleOrders, Params : " + FactoryCode.ToString() + $",isUserTIPs: {isUserTIPs}");
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetBaseOfMasterCardMOsBySaleOrders(config, _mapper, FactoryCode, isUserTIPs, saleOrderList));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBaseOfMasterCardMOsBySaleOrders Success");
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

        #region TIP System Call Api

        [HttpGet]
        [Authorize(Roles = "TipsSystem")]
        [Route("GetMoDataByFlagInterfaceTips")]
        public IActionResult GetMoDataByFlagInterfaceTips(string Interface_TIPs)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataByFlagInterfaceTips, Params : " + Interface_TIPs);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataByInterface_TIPs(config, Convert.ToBoolean(Interface_TIPs)));
                Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataByFlagInterfaceTips Success");
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
                Logger.Error(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Authorize(Roles = "TipsSystem")]
        [Route("GetMoDataByFlagInterfaceTipsAndFactoryCode")]
        public IActionResult GetMoDataByFlagInterfaceTipsAndFactoryCode(string FactoryCode, string Interface_TIPs)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataByFlagInterfaceTips, Params : " + Interface_TIPs);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMoDataByInterface_TIPs(config, FactoryCode, Convert.ToBoolean(Interface_TIPs)));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMoDataByFlagInterfaceTips Success");
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
                Logger.Error(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Authorize(Roles = "TipsSystem")]
        [Route("UpdateMoDataFlagInterfaceTips")]
        public IActionResult UpdateMoDataFlagInterfaceTips(string FactoryCode, string OrderItem, string Interface_TIPs)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateMoDataFlagInterfaceTips, Params : " + $"FactoryCode = {FactoryCode} ,OrderItem = {OrderItem}, Interface_TIPs = {Interface_TIPs}");
                if (string.IsNullOrEmpty(Interface_TIPs))
                    throw new Exception("Interface_TIPs must not be null.");
                _unitOfWork.MoDatas.UpdateMoDataFlagInterfaceTips(FactoryCode, OrderItem, Interface_TIPs);
                Logger.Info(AppCaller, "TIPs", this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateMoDataFlagInterfaceTips Success");
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

        [HttpPost]
        [Authorize(Roles = "PMTs,TipsSystem")]
        [Route("UpdateMODatasInterfaceTIPsByOrderItems")]
        public IActionResult UpdateMODatasInterfaceTIPsByOrderItems(string FactoryCode, bool Interface_tips, [FromBody] List<string> orderItems)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateMODatasInterfaceTIPsByOrderItems , Params : " + $"FactoryCode = {FactoryCode} ,Interface_tips = {Interface_tips}, orderItems = {orderItems}");
                _unitOfWork.MoDatas.UpdateMODatasInterfaceTIPsByOrderItems(FactoryCode, Interface_tips, orderItems);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateMODatasInterfaceTIPsByOrderItems Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = "Success" });
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

        #endregion TIP System Call Api

        #region Function

        private string GenSaleOrderNumber(RunningNo Running)
        {
            try
            {
                if (Running == null)
                {
                    var ex = new ArgumentNullException($"Running No does not exist.");
                    throw new Exception(ex.Message);
                }

                if (Running.Running >= Running.EndNo)
                {
                    var ex = new ArgumentOutOfRangeException(nameof(Running), $"Limited Running No. ,Please contact admin to correct.");
                    throw new Exception(ex.Message);
                }

                int so_no;
                string so_str, saleOrderNO;
                so_no = Running.Running + 1;
                so_str = Convert.ToString(so_no);
                so_str = so_str.PadLeft(Running.Length, '0');
                var fixStr = string.Empty;
                if (Running.UpdatedDate.HasValue && (Running.UpdatedDate.Value.Month != DateTime.Now.Month))
                {
                    so_str = "1".PadLeft(Running.Length, '0');
                }

                fixStr = Running.Fix + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0');
                saleOrderNO = fixStr + so_str;
                return saleOrderNO;
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion Function

        #region [EditBlockPlaten]

        [HttpGet]
        [Authorize(Roles = "PMTs")]
        [Route("GetBlockPlatenMaster")]
        public IActionResult GetBlockPlatenMaster(string factorycode, string material, string pc)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBlockPlatenMaster, Params : " + material + " - " + pc);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetBlockPlatenMaster(factorycode, material, pc));
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBlockPlatenMaster Success");
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
                Logger.Error(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Authorize(Roles = "PMTs")]
        [Route("GetBlockPlatenRouting")]
        public IActionResult GetBlockPlatenRouting(string factorycode, string material)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBlockPlatenRouting, Params : " + material);
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetBlockPlatenRouting(factorycode, material));
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetBlockPlatenRouting Success");
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
                Logger.Error(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Authorize(Roles = "PMTs")]
        [Route("UpdateBlockPlatenRouting")]
        public IActionResult UpdateBlockPlatenRouting(string factorycode, string username, [FromBody] List<EditBlockPlatenRouting> model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateBlockPlatenRouting start");
                _unitOfWork.MoDatas.UpdateBlockPlatenRouting(factorycode, username, model);
                Logger.Info(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.UpdateBlockPlatenRouting Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = "Success" });
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
                Logger.Error(AppCaller, factorycode, this.ToString(), MethodBase.GetCurrentMethod().Name, exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        #endregion [EditBlockPlaten]

        [HttpGet]
        [Route("CheckMaterialNo")]
        public IActionResult CheckMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckMaterialNo start");
                data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.CheckMaterialNo(MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.CheckMaterialNo Success");
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

        [HttpGet]
        [AllowAnonymous]
        [Route("GetMODataWithBomRawMatsByOrderItem")]
        public IActionResult GetMODataWithBomRawMatsByOrderItem(string OrderItem)
        {
            {
                string AppCaller = User.GetClaimValue("AppName");
                string exceptionMessage;
                var data = string.Empty;
                var FactoryCode = string.Empty;
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

                try
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataWithBomRawMatsForAngel, Params : " + FactoryCode + "," + OrderItem);
                    data = JsonConvert.SerializeObject(_unitOfWork.MoDatas.GetMODataWithBomRawMatsByOrderItem(OrderItem));
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GetMODataWithBomRawMatsForAngel Success");
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

        [HttpPost]
        [Route("UpdatePrintedMODataByOrderItems")]
        public IActionResult UpdatePrintedMODataByOrderItems(string FactoryCode, string Username, [FromBody] List<string> orderItems)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdatePrintedMODataByOrderItems, Params : " + FactoryCode + "," + Username + "," + orderItems);
                _unitOfWork.MoDatas.UpdatePrintedMODataByOrderItems(FactoryCode, Username, orderItems);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDataRepository.UpdatePrintedMODataByOrderItems Success");
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

        #region Genarate Mastercard MO PDF

        [AllowAnonymous]
        [HttpPost]
        [Route("GenarateMastercardMOPdfByte")]
        public IActionResult GenarateMastercardMOPdfByte([FromBody] PrintMastercardMO PrintMastercardMO, string FactoryCode, string Language)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            var printMasterCardMOModel = new PrintMasterCardMOModel();
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GenarateMastercardMOPdf, Params : " + FactoryCode.ToString());
                var pdfController = new PDFController();
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Language);
                var pmtsConfig = _unitOfWork.PMTsConfigs.GetPMTsConfigByFactoryName(FactoryCode, "ManageMO_Path");
                byte[] bytes;
                //var Vn = "https://pmtsstorageprod";
                //var In = "https://pmtsstorageidprod";
                //var moSpecs = PrintMastercardMO.MoSpecs;
                //var tempFolder = "D:\\Temp\\MO";
                //var isOverSea = moSpecs.FirstOrDefault(
                //    p =>
                //        Vn.Contains(p.DiecutPictPath ?? "") ||
                //        In.Contains(p.DiecutPictPath ?? "") ||
                //        Vn.Contains(p.PalletizationPath ?? "") ||
                //        In.Contains(p.PalletizationPath ?? "")
                //        ) != null;
                //if (isOverSea)
                //{
                //    printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCardOverSea(PrintMastercardMO, FactoryCode, _mapper, _localizer);
                //}
                //else
                //{
                //    printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCard(PrintMastercardMO, FactoryCode, _mapper,_localizer);

                //}
                printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCard(PrintMastercardMO, FactoryCode, _mapper, _localizer);
                bytes = pdfController.SavePDFWithOutAttachFile(FactoryCode, printMasterCardMOModel, pmtsConfig.FucValue, this);
                data = Convert.ToBase64String(bytes);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GenarateMastercardMOPdf Success");
                //return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                return new FileContentResult(bytes, "application/pdf") { FileDownloadName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).pdf" };
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

        [AllowAnonymous]
        [HttpPost]
        [Route("GenarateMastercardMOPdfBase64")]
        public IActionResult GenarateMastercardMOPdfBase64([FromBody] PrintMastercardMO PrintMastercardMO, string FactoryCode, string Language)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            var printMasterCardMOModel = new PrintMasterCardMOModel();
            Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GenarateMastercardMOPdf, Params : " + FactoryCode.ToString());
                var pdfController = new PDFController();
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Language);
                var pmtsConfig = _unitOfWork.PMTsConfigs.GetPMTsConfigByFactoryName(FactoryCode, "ManageMO_Path");
                byte[] bytes;
                //var Vn = "https://pmtsstorageprod";
                //var In = "https://pmtsstorageidprod";
                //var moSpecs = PrintMastercardMO.MoSpecs;
                //var tempFolder = "D:\\Temp\\MO";
                //var isOverSea = moSpecs.FirstOrDefault(
                //    p =>
                //        Vn.Contains(p.DiecutPictPath ?? "") ||
                //        In.Contains(p.DiecutPictPath ?? "") ||
                //        Vn.Contains(p.PalletizationPath ?? "") ||
                //        In.Contains(p.PalletizationPath ?? "")
                //        ) != null;
                //if (isOverSea)
                //{
                //    printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCardOverSea(PrintMastercardMO, FactoryCode, _mapper, _localizer);
                //}
                //else
                //{
                //    printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCard(PrintMastercardMO, FactoryCode, _mapper,_localizer);

                //}
                printMasterCardMOModel = _unitOfWork.MoDatas.GetDataForMasterCard(PrintMastercardMO, FactoryCode, _mapper, _localizer);
                bytes = pdfController.SavePDFWithOutAttachFile(FactoryCode, printMasterCardMOModel, pmtsConfig.FucValue, this);
                data = Convert.ToBase64String(bytes);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), MethodBase.GetCurrentMethod().Name, "MoDatas.GenarateMastercardMOPdf Success");
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                //return new FileContentResult(bytes, "application/pdf") { FileDownloadName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).pdf" };
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

        #endregion Genarate Mastercard MO PDF
    }
}