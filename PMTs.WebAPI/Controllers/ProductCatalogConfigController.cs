using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
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
    public class ProductCatalogConfigController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;

        [HttpGet]
        public IActionResult Get(string FactoryCode, string Username)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetProductCatalogConfigs, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetProductCatalogConfigs(FactoryCode, Username));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetProductCatalogConfigs Success");
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
        public IActionResult Post(string FactoryCode, string Username, [FromBody] List<ProductCatalogConfig> Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.AddList");
                var Temp = _unitOfWork.ProductCatalogConfigRepository.GetProductCatalogConfigs(FactoryCode, Username);
                if (Temp != null || Temp.Count > 0)
                {
                    _unitOfWork.ProductCatalogConfigRepository.RemoveRange(Temp);
                    _unitOfWork.ProductCatalogConfigRepository.AddList(Model);
                }
                else
                {
                    _unitOfWork.ProductCatalogConfigRepository.AddList(Model);
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.AddList Success");
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
        [Route("SaveProductCatalogRemark")]
        public IActionResult SaveProductCatalogRemark(string FactoryCode, [FromBody] ProductCatalogRemark Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateRemark");
                _unitOfWork.ProductCatalogConfigRepository.UpdateRemark(config, Model, FactoryCode);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateRemark Success");
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
        [Route("GetProductCatalogRemark")]
        public IActionResult GetProductCatalogRemark(string FactoryCode, string PC)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetRemark, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetRemark(config, FactoryCode, PC));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetRemark Success");
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
        [Route("GetHoldMaterialByMaterial")]
        public IActionResult GetHoldMaterialByMaterial(string FactoryCode, string Material)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetHoldMaterialByMaterial, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(Material));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetHoldMaterialByMaterial Success");
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
        [Route("SaveHoldMaterial")]
        public IActionResult SaveHoldMaterial(string FactoryCode, [FromBody] HoldMaterial Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial");
                _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialByMaterial(Model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial Success");
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
        [Route("UpdateHoldMaterial")]
        [Authorize(Roles = "PMTs, PresaleSystem")]
        public IActionResult UpdateHoldMaterial(string FactoryCode, [FromBody] HoldMaterial Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial");
                _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial(Model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial Success");
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
        [Route("GetHoldMaterialHistoryByMaterial")]
        public IActionResult GetHoldMaterialHistoryByMaterial(string FactoryCode, string Material)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetHoldMaterialHistory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialHistory(Material));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetHoldMaterialHistory Success");
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
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("SaveHoldMaterialHistory")]
        public IActionResult SaveHoldMaterialHistory(string FactoryCode, [FromBody] HoldMaterialHistory Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialHistory");
                _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(Model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialHistory Success");
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
        [Route("GetOrderItemByMoData")]
        public IActionResult GetOrderItemByMoData(string FactoryCode, string Material)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetOrderItemInMoData, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetOrderItemInMoData(FactoryCode, Material));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetOrderItemInMoData Success");
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
        [Route("GetScalePriceMatProduct")]
        public IActionResult GetScalePriceMatProduct(string FactoryCode, string CustId, string CustName, string CustCode, string Pc1, string Pc2, string Pc3, string MaterialType, string salePlants, string plantPdts, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SearchScalePriceMatProduct, Params : FactoryCode=" + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetScalePriceMatProduct(config, FactoryCode, CustId, CustName, CustCode, Pc1, Pc2, Pc3, MaterialType, salePlants, plantPdts, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SearchScalePriceMatProduct Success");
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
        [Route("GetBOMMaterialProduct")]
        public IActionResult GetBOMMaterialProduct(string FactoryCode, string CustId, string CustName, string CustCode, string Pc1, string Pc2, string Pc3)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SearchBOMMaterialProduct, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetBOMMaterialProduct(config, FactoryCode, CustId, CustName, CustCode, Pc1, Pc2, Pc3));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SearchBOMMaterialProduct Success");
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
        [Authorize(Roles = "PMTs, PresaleSystem")]
        [Route("UpdateHoldMaterialPresale")]
        public IActionResult UpdateHoldMaterialPresale(string FactoryCode, [FromBody] HoldMaterial Model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage = string.Empty;
            string data = string.Empty;
            FactoryCode = FactoryCode;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialPresale");
                _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialPresale(Model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialPresale Success");
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
