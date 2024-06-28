using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VMIServiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper _mapper;

        public VMIServiceController(PMTsDbContext pmtsContext, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
            config = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("GetAllMasterDataByCustInvType")]
        public IActionResult GetAllMasterDataByCustInvType(string CustInvType)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            // var data = string.Empty;
            string FactoryCode = "";
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMasterDataByCustInvType Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType Params : " + $"CustInvType = {CustInvType}");
                var data = _unitOfWork.VMIService.GetAllMasterDataByCustInvType(config, CustInvType);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType Success");
                return Ok(new CustomResponse<List<VMIAllMasterDataModel>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
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
        //[Authorize(Roles = "VMISystem,PMTs")]
        //[Route("GetRoutingByListMaterialNo")]
        //public IActionResult GetRoutingByListMaterialNo(string FactoryCode, [FromBody] List<string> ListMaterialNo)
        //{

        //    string AppCaller = User.GetClaimValue("AppName");
        //    string exceptionMessage;
        //    var data = string.Empty;
        //    try
        //    {

        //        if (ListMaterialNo.Count > 200)
        //        {
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.NotFound, StatusCode = StatusCodes.Status200OK, Result = "List Material Limit 200" });
        //        }
        //        else
        //        {
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetRoutingByListMaterialNo Start");

        //            int xx = 0;
        //            List<string> conditionlist = new List<string>();
        //            string ss = string.Empty;
        //            for (int i = 1; i <= ListMaterialNo.Count; i++)
        //            {
        //                string mat = "'" + ListMaterialNo[i - 1].ToString() + "',";
        //                ss = ss + mat;
        //                if ((i % 1000) == 0)
        //                {
        //                    string tmpss = ss.Substring(0, ss.Length - 1);
        //                    conditionlist.Add(tmpss);
        //                    ss = "";
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(ss))
        //            {
        //                string tmpss2 = ss.Substring(0, ss.Length - 1);
        //                conditionlist.Add(tmpss2);
        //            }

        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType");
        //            List<Routing> RoutingTemp = new List<Routing>();
        //            for (int it = 0; it < conditionlist.Count; it++)
        //            {
        //                List<string> conditionfinal = new List<string>();
        //                var temp = _unitOfWork.VMIService.GetRoutingByListMaterialNo(config, FactoryCode, conditionlist[it]);
        //                RoutingTemp.AddRange(temp);
        //            }
        //            // data = JsonConvert.SerializeObject(RoutingTemp);
        //            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType Success");

        //            return Ok(new CustomResponse<List<Routing>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = RoutingTemp });
        //        }
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

        [HttpPost]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("GetMasterData")]
        public IActionResult GetMasterData([FromBody] List<VMIAllMasterDataModel> ListMaterialNo)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            try
            {
                if (ListMaterialNo.Count > 10000)
                {
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.PayloadTooLarge, StatusCode = StatusCodes.Status414UriTooLong, Result = "List Material Limit 10000" });
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMasterDataByCustInvType Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMasterDataByCustInvType Params : " + JsonConvert.SerializeObject(ListMaterialNo));

                    List<string> conditionlist = new List<string>();
                    string ss = string.Empty;
                    for (int i = 1; i <= ListMaterialNo.Count; i++)
                    {
                        string mat = "(FactoryCode='" + ListMaterialNo[i - 1].FactoryCode.ToString() + "' and  Material_No ='" + ListMaterialNo[i - 1].MaterialNo.ToString() + "') or";
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

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType");
                    List<MasterData> MasterDataTemp = new List<MasterData>();
                    for (int it = 0; it < conditionlist.Count; it++)
                    {
                        List<MasterData> masterdata = new List<MasterData>();
                        masterdata = _unitOfWork.VMIService.GetMasterDataByCustInvType(config, conditionlist[it]);
                        MasterDataTemp.AddRange(masterdata);
                    }

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType Success");
                    return Ok(new CustomResponse<List<MasterData>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = MasterDataTemp });
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

        [HttpPost]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("GetRoutingByListMaterialNo")]
        public IActionResult GetRoutingByListMaterialNo([FromBody] List<VMIAllMasterDataModel> ListMaterialNo)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string FactoryCode = string.Empty;
            var data = string.Empty;
            try
            {

                if (ListMaterialNo.Count > 2500)
                {
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.PayloadTooLarge, StatusCode = StatusCodes.Status414UriTooLong, Result = "List Material Limit 2500" });
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetRoutingByListMaterialNo Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetRoutingByListMaterialNo Params : " + JsonConvert.SerializeObject(ListMaterialNo));

                    List<string> conditionlist = new List<string>();
                    string ss = string.Empty;
                    for (int i = 1; i <= ListMaterialNo.Count; i++)
                    {
                        string mat = "(FactoryCode='" + ListMaterialNo[i - 1].FactoryCode.ToString() + "' and  Material_No ='" + ListMaterialNo[i - 1].MaterialNo.ToString() + "') or";
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

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType");
                    List<Routing> RoutingTemp = new List<Routing>();
                    for (int it = 0; it < conditionlist.Count; it++)
                    {
                        var temp = _unitOfWork.VMIService.GetRoutingByListMaterialNo(config, conditionlist[it]);
                        RoutingTemp.AddRange(temp);
                    }
                    // data = JsonConvert.SerializeObject(RoutingTemp);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMasterDataByCustInvType Success");

                    return Ok(new CustomResponse<List<Routing>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = RoutingTemp });
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

        [HttpPost]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("GetMoDataByListMaterialNo")]
        public IActionResult GetMoDataByListMaterialNo([FromBody] List<VMIAllMasterDataModel> ListMaterialNo)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            string FactoryCode = string.Empty;
            var data = string.Empty;
            try
            {

                if (ListMaterialNo.Count > 5000)
                {
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.PayloadTooLarge, StatusCode = StatusCodes.Status414UriTooLong, Result = "List Material Limit 5000" });
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMoDataByListMaterialNo Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetMoDataByListMaterialNo Params : " + JsonConvert.SerializeObject(ListMaterialNo));

                    List<string> conditionlist = new List<string>();
                    string ss = string.Empty;
                    for (int i = 1; i <= ListMaterialNo.Count; i++)
                    {
                        string mat = "(FactoryCode='" + ListMaterialNo[i - 1].FactoryCode.ToString() + "' and  Material_No ='" + ListMaterialNo[i - 1].MaterialNo.ToString() + "') or";
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

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMoDataByListMaterialNo");
                    List<MoData> ModataTemp = new List<MoData>();
                    for (int it = 0; it < conditionlist.Count; it++)
                    {
                        var temp = _unitOfWork.VMIService.GetMoDataByListMaterialNo(config, conditionlist[it]);
                        ModataTemp.AddRange(temp);
                    }
                    // data = JsonConvert.SerializeObject(ModataTemp);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetMoDataByListMaterialNo Success");

                    return Ok(new CustomResponse<List<MoData>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = ModataTemp });
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

        [HttpPost]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("CreateMOManual")]
        public IActionResult CreateMOManual([FromBody] List<MoData> moDatas)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            var result = new List<MODataManualModel>();
            var moDataResult = new MoData();
            var mOManualModel = new MODataManualModel();
            var runningNos = new List<RunningNo>();
            var attachFileMo = new AttachFileMo();
            var FactoryCode = moDatas.Count > 0 ? moDatas.FirstOrDefault().FactoryCode : string.Empty;

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CreateMOManual Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.CreateMOManual");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.CreateMOManual Params : " + JsonConvert.SerializeObject(moDatas));
                foreach (var moData in moDatas)
                {
                    try
                    {
                        moDataResult = new MoData();
                        mOManualModel = new MODataManualModel();
                        runningNos = new List<RunningNo>();
                        var masterdata = _unitOfWork.MasterDatas.GetByPredicate(m => m.MaterialNo == moData.MaterialNo && m.FactoryCode == FactoryCode && m.PdisStatus.ToLower().Trim() != "x");
                        runningNos = new List<RunningNo>();

                        if (masterdata == null)
                        {
                            moData.OrderItem = "";
                            throw new Exception($"Material No. {moData.MaterialNo} doesn't exist.");
                        }
                        else
                        {
                            var runningNO = new RunningNo();
                            var moSpec = new MoSpec();
                            var moRoutings = new List<MoRouting>();

                            runningNO = _unitOfWork.RunningNos.GetRunningNoByGroupId(FactoryCode, "SO");
                            var flute = _unitOfWork.Flutes.GetFluteByFlute(masterdata.FactoryCode, masterdata.Flute);

                            var userLogon = moData.UpdatedBy;

                            #region Set MO Data
                            moData.Id = 0;
                            moData.Printed = 0;
                            var culture = new CultureInfo("en-US");
                            moData.DateTimeStamp = DateTime.Now.ToString("yMMddHHmmss", culture);
                            moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
                            moData.MoStatus = "C";
                            moData.PlanStatus = null;
                            moData.StockQty = null;
                            moData.SentKiwi = false;
                            moData.IsCreateManual = true;
                            moData.OriginalDueDate = moData.DueDate;
                            moData.CreatedDate = DateTime.Now;
                            moData.CreatedBy = userLogon;
                            //moData.UpdatedDate = DateTime.Now;
                            //moData.UpdatedBy = userLogon;
                            moData.Name = masterdata.CustName;
                            moData.ToleranceOver = moData.ToleranceOver != null && moData.ToleranceOver.HasValue ? moData.ToleranceOver : 0;

                            #endregion

                            #region Set MO Spec
                            moSpec = _mapper.Map<MasterData, MoSpec>(masterdata);
                            moSpec.Id = 0;
                            moSpec.User = userLogon;
                            moSpec.OrderItem = moData.OrderItem;
                            moSpec.LastUpdate = DateTime.Now;
                            moSpec.CreateDate = DateTime.Now;
                            moSpec.FluteDesc = flute != null ? flute.Description : moSpec.FluteDesc;
                            #endregion

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
                                var routingCorr = routings.FirstOrDefault(r => r.FactoryCode.Equals(FactoryCode) && r.MaterialNo.Equals(moData.MaterialNo) && r.Machine.Equals("COR") && r.PdisStatus != "X");
                                var cutNO = routingCorr != null && routingCorr.CutNo.HasValue && routingCorr.CutNo != null ? routingCorr.CutNo.Value : 0;
                                moData.TargetQuant = _unitOfWork.Formulas.CalculateMoTargetQuantity(FactoryCode, moData.OrderQuant, moData.ToleranceOver.Value, masterdata.Flute, moData.MaterialNo, cutNO);
                            }
                            else
                            {
                                throw new Exception("Can't calculate Order Quantity.");
                            }

                            #endregion

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
                            #endregion

                            if (runningNO == null)
                            {
                                moData.OrderItem = string.Empty;
                                throw new Exception("Can't genarate material number with this running no.");
                            }

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
                            #endregion

                            var errorMessage = _unitOfWork.MoDatas.AddManualMOData(moData, moSpec, moRoutings, runningNos, attachFileMo);

                            if (string.IsNullOrEmpty(moData.OrderItem) || !string.IsNullOrEmpty(errorMessage))
                            {
                                moData.OrderItem = string.Empty;
                                throw new Exception(errorMessage);
                            }

                            mOManualModel.MoData = moData;
                            mOManualModel.ErrorMessage = string.Empty;
                            mOManualModel.CreatedStatus = true;


                            result.Add(mOManualModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        moData.OrderItem = string.Empty;
                        mOManualModel.MoData = moData;
                        mOManualModel.ErrorMessage = ex.Message;
                        mOManualModel.CreatedStatus = false;
                        result.Add(mOManualModel);
                        continue;
                    }
                }

                //data = JsonConvert.SerializeObject(_unitOfWork.VMIService.GetMasterDataByCustInvType(config, CustInvType));
                data = JsonConvert.SerializeObject(result);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.CreateMOManual Success");
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
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("EditMOManualByOrderItem")]
        public IActionResult EditMOManualByOrderItem(string UserName, string OrderItem, string OrderQty, string DueDate)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            var moData = new MoData();
            string FactoryCode = "";
            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "EditMOManualByOrderItem Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.EditMOManualByOrderItem");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.EditMOManualByOrderItem Params : " + $"UserName = {UserName}, OrderItem = {OrderItem}, OrderQty = {OrderQty}, DueDate = {DueDate}");

                moData = _unitOfWork.MoDatas.GetByPredicate(m => m.OrderItem == OrderItem);
                var masterdata = _unitOfWork.MasterDatas.GetByPredicate(m => m.MaterialNo == moData.MaterialNo && m.FactoryCode == moData.FactoryCode && m.PdisStatus.ToLower().Trim() != "x");

                if (moData != null && masterdata != null)
                {
                    var culture = new CultureInfo("en-US");
                    moData.UpdatedDate = DateTime.Now;
                    moData.UpdatedBy = UserName;
                    moData.DueDate = DateTime.ParseExact(DueDate, "yyyy-MM-dd HH:mm:ss", culture);
                    moData.OrderQuant = Convert.ToInt32(OrderQty);
                    moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
                    moData.ToleranceOver = moData.ToleranceOver != null && moData.ToleranceOver.HasValue ? moData.ToleranceOver : 0;

                    var routings = new List<Routing>();
                    routings = _unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, masterdata.MaterialNo).ToList();

                    if (moData.OrderQuant >= 0)
                    {
                        var routingCorr = routings.FirstOrDefault(r => r.FactoryCode.Equals(FactoryCode) && r.MaterialNo.Equals(moData.MaterialNo) && r.Machine.Equals("COR") && r.PdisStatus != "X");
                        var cutNO = routingCorr != null && routingCorr.CutNo.HasValue && routingCorr.CutNo != null ? routingCorr.CutNo.Value : 0;
                        moData.TargetQuant = _unitOfWork.Formulas.CalculateMoTargetQuantity(moData.FactoryCode, moData.OrderQuant, moData.ToleranceOver.Value, masterdata.Flute, moData.MaterialNo, cutNO);
                    }

                    _unitOfWork.MoDatas.Update(moData);
                    data = JsonConvert.SerializeObject(moData);
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.EditMOManualByOrderItem Success");
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

        //Tassanai Update API get Bom_Struct
        [HttpPost]
        [Authorize(Roles = "VMISystem,PMTs")]
        [Route("GetBomStruct")]
        public IActionResult GetBomStruct([FromBody] List<VMIAllMasterDataModel> ListMaterialNo)
        {


            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            try
            {
                if (ListMaterialNo.Count > 10000)
                {
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.PayloadTooLarge, StatusCode = StatusCodes.Status414UriTooLong, Result = "List Material Limit 10000" });
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetBomStruct Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetBomStruct Params : " + JsonConvert.SerializeObject(ListMaterialNo));

                    List<string> conditionlist = new List<string>();
                    string ss = string.Empty;
                    for (int i = 1; i <= ListMaterialNo.Count; i++)
                    {
                        string mat = "(FactoryCode='" + ListMaterialNo[i - 1].FactoryCode.ToString() + "' and  Material_No ='" + ListMaterialNo[i - 1].MaterialNo.ToString() + "') or";
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

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetBomStruct");
                    List<BomStruct> bomStructTemp = new List<BomStruct>();
                    for (int it = 0; it < conditionlist.Count; it++)
                    {
                        List<BomStruct> bomStructs = new List<BomStruct>();
                        bomStructs = _unitOfWork.VMIService.GetBomStructs(config, conditionlist[it]);
                        bomStructTemp.AddRange(bomStructs);
                    }

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "VMIService.GetBomStruct Success");
                    return Ok(new CustomResponse<List<BomStruct>> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = bomStructTemp });
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

        #endregion

    }
}
