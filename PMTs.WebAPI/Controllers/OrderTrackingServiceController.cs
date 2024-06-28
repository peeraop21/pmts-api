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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTrackingServiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration config;

        public OrderTrackingServiceController(PMTsDbContext pmtsContext, IConfiguration configuration)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
            config = configuration;
        }


        [HttpPost]
        [Authorize(Roles = "OrderTracking,PMTs")]
        [Route("GetAllOrderByDate")]
        public IActionResult GetAllOrderByDate(string UpdateDateFrom, string UpdateDateTo)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = User.GetClaimValue("Factory");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetAllOrderByDate Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.GetAllOrderByDate");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.GetAllOrderByDate Params : " + $"UpdateDateFrom = {UpdateDateFrom}, UpdateDateTo = {UpdateDateTo}");
                var datax = _unitOfWork.OrderTrackingService.GetAllOrderByDate(config, UpdateDateFrom, UpdateDateTo);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.GetAllOrderByDate Success");
                return Ok(new CustomResponse<List<AllOrderTracking>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });
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
        [Authorize(Roles = "OrderTracking,PMTs")]
        [Route("GetMoByLisOrderItems")]
        public IActionResult GetMoByLisOrderItems([FromBody] List<AllOrderTracking> OrderList)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = User.GetClaimValue("Factory");

            try
            {
                if (OrderList.Count > 1000)
                {
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.PayloadTooLarge, StatusCode = StatusCodes.Status414UriTooLong, Result = "List Material Limit 1000" });
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMoByLisOrderItems Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMoByLisOrderItems Params : " + JsonConvert.SerializeObject(OrderList));
                    List<string> conditionlist = new List<string>();
                    string ss = string.Empty;
                    for (int i = 1; i <= OrderList.Count; i++)
                    {
                        string mat = "(FactoryCode='" + OrderList[i - 1].FactoryCode.ToString() + "' and  OrderItem ='" + OrderList[i - 1].OrderItem.ToString() + "') or";
                        ss = ss + mat;
                        if ((i % 1000) == 0)
                        {
                            string tmpss = ss.Substring(0, ss.Length - 2);
                            conditionlist.Add(tmpss);
                            ss = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(ss))
                    {
                        string tmpss2 = ss.Substring(0, ss.Length - 2);
                        conditionlist.Add(tmpss2);
                    }
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.GetMoByLisOrderItems");
                    var datax = _unitOfWork.OrderTrackingService.GetMoByListOrderItems(config, conditionlist[0]);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.GetMoByLisOrderItems Success");
                    return Ok(new CustomResponse<OrderTrackingServiceModel> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });
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
                    exceptionMessage = ex.Message;
                }


                Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }

        }

        //[HttpPost]
        //[Authorize(Roles = "PMTs")]
        //[Route("OrderTrackingService")]
        //public IActionResult OrderTrackingService(string FactoryCode , string UpdateDateFrom ,string UpdateDateTo)
        //{

        //    string AppCaller = User.GetClaimValue("AppName");
        //    string exceptionMessage;
        //    var data = string.Empty;

        //    try
        //    {
        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService Start");
        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingService");
        //        data = JsonConvert.SerializeObject(_unitOfWork.OrderTrackingService.OrderTrackingService(config, FactoryCode, UpdateDateFrom,UpdateDateTo));
        //        var datax = _unitOfWork.OrderTrackingService.OrderTrackingService(config, FactoryCode, UpdateDateFrom, UpdateDateTo);
        //        List<string> cccc = new List<string>();
        //        foreach (var item in datax.moDatas)
        //        {
        //            cccc.Add(item.MaterialNo);
        //        }
        //        var yyy = JsonConvert.SerializeObject(cccc);


        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingService Success");
        //        return Ok(new CustomResponse<OrderTrackingServiceModel> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });


        //       // return Json(data,);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains(Messages.INNER_EXCEPTION))
        //        {
        //            exceptionMessage = ex.InnerException.Message;
        //        }
        //        else
        //        {
        //            exceptionMessage = ex.Message;
        //        }


        //        Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
        //    }

        //}

        //[HttpPost]
        //[Authorize(Roles = "OrderTracking,PMTs")]
        //[Route("OrderTrackingServiceMOData")]
        //public IActionResult OrderTrackingServiceMOData(string FactoryCode, string UpdateDateFrom, string UpdateDateTo)
        //{

        //    string AppCaller = User.GetClaimValue("AppName");
        //    string exceptionMessage;
        //    var data = string.Empty;

        //    try
        //    {
        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingServiceMOData Start");
        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoData");
        //       // data = JsonConvert.SerializeObject(_unitOfWork.OrderTrackingService.OrderTrackingService(config, FactoryCode, UpdateDateFrom, UpdateDateTo));
        //        var datax = _unitOfWork.OrderTrackingService.OrderTrackingServiceMoData(config, FactoryCode, UpdateDateFrom, UpdateDateTo);
        //        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoData Success");
        //        return Ok(new CustomResponse<List<MoData>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains(Messages.INNER_EXCEPTION))
        //        {
        //            exceptionMessage = ex.InnerException.Message;
        //        }
        //        else
        //        {
        //            exceptionMessage = ex.Message;
        //        }


        //        Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
        //    }

        //}

        //[HttpPost]
        //[Authorize(Roles = "OrderTracking,PMTs")]
        //[Route("OrderTrackingServiceMoSpect")]
        //public IActionResult OrderTrackingServiceMoSpect(string FactoryCode,[FromBody] List<string> ListMaterial)
        //{

        //    string AppCaller = User.GetClaimValue("AppName");
        //    string exceptionMessage;
        //    var data = string.Empty;

        //    try
        //    {
        //        if (ListMaterial.Count > 100)
        //        {
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.NotFound, StatusCode = StatusCodes.Status200OK, Result = "List Material Limit 100" });
        //        }
        //        else
        //        {


        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingServiceMoSpect Start");
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoSpect");
        //            //  data = JsonConvert.SerializeObject(_unitOfWork.OrderTrackingService.OrderTrackingService(config, FactoryCode, UpdateDateFrom, UpdateDateTo));
        //            var datax = _unitOfWork.OrderTrackingService.OrderTrackingServiceMoSpect(config, FactoryCode, ListMaterial);
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoSpect Success");
        //            return Ok(new CustomResponse<List<MoSpec>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });
        //        }


        //        // return Json(data,);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains(Messages.INNER_EXCEPTION))
        //        {
        //            exceptionMessage = ex.InnerException.Message;
        //        }
        //        else
        //        {
        //            exceptionMessage = ex.Message;
        //        }


        //        Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
        //    }

        //}


        //[HttpPost]
        //[Authorize(Roles = "OrderTracking,PMTs")]
        //[Route("OrderTrackingServiceMoRouting")]
        //public IActionResult OrderTrackingServiceMoRouting(string FactoryCode, [FromBody] List<string> ListMaterial)
        //{

        //    string AppCaller = User.GetClaimValue("AppName");
        //    string exceptionMessage;
        //    var data = string.Empty;

        //    try
        //    {
        //        if (ListMaterial.Count > 100)
        //        {
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.NotFound, StatusCode = StatusCodes.Status200OK, Result = "List Material Limit 100" });
        //        }
        //        else
        //        {
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingServiceMoSpect Start");
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoSpect");
        //            //  data = JsonConvert.SerializeObject(_unitOfWork.OrderTrackingService.OrderTrackingService(config, FactoryCode, UpdateDateFrom, UpdateDateTo));
        //            var datax = _unitOfWork.OrderTrackingService.OrderTrackingServiceMORouting(config, FactoryCode, ListMaterial);
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "OrderTrackingService.OrderTrackingServiceMoSpect Success");
        //            return Ok(new CustomResponse<List<MoRouting>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datax });
        //        }

        //        // return Json(data,);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains(Messages.INNER_EXCEPTION))
        //        {
        //            exceptionMessage = ex.InnerException.Message;
        //        }
        //        else
        //        {
        //            exceptionMessage = ex.Message;
        //        }


        //        Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
        //    }

        //}
    }
}
