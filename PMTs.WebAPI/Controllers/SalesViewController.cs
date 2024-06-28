using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
using System.Data.SqlClient;

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class SalesViewController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;

        [HttpGet]
        [Route("GetSaleViewByMaterialNo")]
        public IActionResult GetSaleViewByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewByMaterialNo(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByMaterialNo Success");
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
        [Route("GetSaleViewByMaterialNoAndChannelAndDevPlant")]
        public IActionResult GetSaleViewByMaterialNoAndChannelAndDevPlant(string FactoryCode, string MaterialNo, int Channel, string DevPlant)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetByPredicate(s => s.MaterialNo.Equals(MaterialNo) && s.FactoryCode.Equals(FactoryCode) && s.Channel.Equals(Channel) && s.DevPlant.Equals(DevPlant) && s.PdisStatus != "X"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByMaterialNo Success");
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
        [Route("GetSaleViewsByMaterialNo")]
        public IActionResult GetSaleViewsByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewsByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewsByMaterialNo(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewsByMaterialNo Success");
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
        [Route("GetSaleViewsByMaterialNoAndFactoryCode")]
        public IActionResult GetSaleViewsByMaterialNoAndFactoryCode(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewsByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewsByMaterialNoAndFactoryCode(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewsByMaterialNo Success");
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
        [Route("GetSaleViewBySaleOrg")]
        public IActionResult GetSaleViewBySaleOrg(string FactoryCode, string MaterialNo, string SaleOrg)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewBySaleOrg, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + SaleOrg.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewBySaleOrg(FactoryCode, MaterialNo, SaleOrg));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewBySaleOrg Success");
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
        [Route("GetSaleViewBySaleOrgChannel")]
        public IActionResult GetSaleViewBySaleOrgChannel(string FactoryCode, string MaterialNo, string SaleOrg, byte Channel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewBySaleOrg, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + SaleOrg.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewBySaleOrgChannel(FactoryCode, MaterialNo, SaleOrg, Channel));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewBySaleOrg Success");
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
        [Route("GetSaleViewByhandshake")]
        public IActionResult GetSaleViewByhandshake(string FactoryCode, [FromBody] UpdateMatModel updateMatModel, string SaleOrg)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
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
                SqlConnection conn = new SqlConnection(config.GetConnectionString("PMTsConnect"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByhandshake, Params : " + config.GetConnectionString("PMTsConnect").ToString() + " , " + FactoryCode.ToString() + " , " + materialno.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetSaleViewByhandshake(conn, FactoryCode, materialno));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.GetSaleViewByhandshake Success");
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
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Add");
                _unitOfWork.SalesViews.Add(parentModel.SalesView);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Add Success");
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
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Update");
                _unitOfWork.SalesViews.Update(parentModel.SalesView);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Update Success");
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

        [HttpDelete]
        [Route("DeleteSaleviews")]
        public IActionResult DeleteSaleviews(string FactoryCode, [FromBody] List<SalesView> salesViews)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Update");
                _unitOfWork.SalesViews.RemoveRange(salesViews);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SalesViews.Update Success");
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
        [Route("GetReuseSaleViewsByMaterialNos")]
        public IActionResult GetReuseSaleViewsByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> MaterialNos)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string data;
            bool isAuthen = false;

            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SaleViews.GetReuseSaleViewsByMaterialNos, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.SalesViews.GetReuseSaleViewsByMaterialNos(FactoryCode, MaterialNos));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "SaleViews.GetReuseSaleViewsByMaterialNos Success");
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
