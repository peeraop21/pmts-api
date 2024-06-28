using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System.Linq;

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresaleServiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public PresaleServiceController(PMTsDbContext pmtsContext)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
        }

        [HttpPut]
        [Authorize(Roles = "PMTs, PresaleSystem")]
        [Route("AddPresaleChangeProduct")]
        public IActionResult Post(string FactoryCode, [FromBody] PresaleModel presaleModel)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            if (string.IsNullOrEmpty(FactoryCode))
            {
                FactoryCode = presaleModel.presaleChange.FactoryCode;
            }
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");


            try
            {
                var presaleData = _unitOfWork.PresaleChangeProduct.GetPresaleChangeProductByPsmId(presaleModel.presaleChange.PsmId);
                var presaleRoutings = _unitOfWork.PresaleChangeRouting.GetPresaleChangeRoutingByPsmId(presaleModel.presaleChange.PsmId).ToList();

                if (presaleData == null)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeProduct.Add");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeProduct.Add Params : " + JsonConvert.SerializeObject(presaleModel));
                    presaleModel.presaleChange.IsApprove = false;
                    _unitOfWork.PresaleChangeProduct.Add(presaleModel.presaleChange);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeProduct.Add Success");
                    //return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
                }
                else
                {
                    if (string.IsNullOrEmpty(presaleData.MaterialNo) || presaleRoutings.Where(r => string.IsNullOrEmpty(r.MaterialNo)).Count() > 0)
                    {
                        throw new Exception("Invalid material number.");
                    }

                    presaleData.IsApprove = false;
                    presaleData.Description = presaleModel.presaleChange.Description;
                    presaleData.SaleText1 = presaleModel.presaleChange.SaleText1;
                    presaleData.SaleText2 = presaleModel.presaleChange.SaleText2;
                    presaleData.SaleText3 = presaleModel.presaleChange.SaleText3;
                    presaleData.SaleText4 = presaleModel.presaleChange.SaleText4;
                    presaleData.PieceSet = presaleModel.presaleChange.PieceSet;
                    presaleData.PrintMethod = presaleModel.presaleChange.PrintMethod;
                    presaleData.HighGroup = presaleModel.presaleChange.HighGroup;
                    presaleData.HighValue = presaleModel.presaleChange.HighValue;
                    presaleData.Bun = presaleModel.presaleChange.Bun;
                    presaleData.BunLayer = presaleModel.presaleChange.BunLayer;
                    presaleData.LayerPalet = presaleModel.presaleChange.LayerPalet;

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeProduct.Update");
                    _unitOfWork.PresaleChangeProduct.Update(presaleData);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeProduct.Update Success");

                }

                if (presaleRoutings.Count > 0)
                {
                    foreach (var routing in presaleRoutings)
                    {
                        var changeRouting = presaleModel.presaleRouting.Where(w => w.PlanCode == routing.PlanCode).FirstOrDefault();
                        if (changeRouting != null)
                        {
                            routing.Color1 = changeRouting.Color1;
                            routing.Color2 = changeRouting.Color2;
                            routing.Color3 = changeRouting.Color3;
                            routing.Color4 = changeRouting.Color4;
                            routing.Color5 = changeRouting.Color5;
                            routing.Color6 = changeRouting.Color6;
                            routing.Color7 = changeRouting.Color7;
                            routing.Shade1 = changeRouting.Shade1;
                            routing.Shade2 = changeRouting.Shade2;
                            routing.Shade3 = changeRouting.Shade3;
                            routing.Shade4 = changeRouting.Shade4;
                            routing.Shade5 = changeRouting.Shade5;
                            routing.Shade6 = changeRouting.Shade6;
                            routing.Shade7 = changeRouting.Shade7;
                        }
                        routing.IsApprove = false;
                    }

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeRouting.update");
                    _unitOfWork.PresaleChangeRouting.UpdateList(presaleRoutings);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeRouting.update Success");
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeRouting.Add");
                    _unitOfWork.PresaleChangeRouting.AddRange(presaleModel.presaleRouting);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PresaleChangeRouting.Add Success");

                }
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

        /*
        [HttpPut]
        public JsonResult Put(string AppName, string FactoryCode, string MaterialNo)
        {

            bool isSuccess;
            string exceptionMessage;
            var data = string.Empty;

            string AppCaller = "";
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            if (isAuthen)
            {
                try
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                    var masterData = _unitOfWork.MasterDatas.GetMasterDataByMaterialNumber(FactoryCode, MaterialNo);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.GetMasterDataByMaterialNumber Success");

                    if (masterData != null)
                    {
                        if (masterData.Description != null)
                        {
                            string trimDesc = masterData.Description.TrimEnd();
                            if (trimDesc.Substring(trimDesc.Length - 2) != "ปป")
                            {
                                masterData.Description += " ปป";
                            }
                        }
                        else
                        {
                            masterData.Description = " ปป";
                        }
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update");
                        _unitOfWork.MasterDatas.Update(masterData);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.Update Success");
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                        exceptionMessage = "Material_No not found!";
                    }


                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                    Logger.Error(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    isSuccess = false;
                }
                return Json(new { isSuccess = isSuccess, exceptionMessage = exceptionMessage, data = data });
            }
            else
            {
                return Json(new { isSuccess = false, exceptionMessage = Extensions.AUTHEN_FIAL, data = data });
            }

        }*/

        [HttpPost]
        [Authorize(Roles = "PMTs, PresaleSystem")]
        [Route("HoldMaterial")]
        public IActionResult HoldMaterial([FromBody] HoldAndUnHoldMaterialRequestModel HoldMaterialRequest)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                HoldMaterial HoldMatModel = new HoldMaterial();
                HoldMatModel.Id = 0;
                HoldMatModel.IsLocked = true;
                HoldMatModel.HoldRemark = HoldMaterialRequest.HoldRemark;
                HoldMatModel.MaterialNo = HoldMaterialRequest.MaterialNo;
                HoldMatModel.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                HoldMatModel.CreatedBy = HoldMaterialRequest.User;
                HoldMatModel.UpdatedBy = HoldMaterialRequest.User;
                HoldMaterialHistory holdHis = new HoldMaterialHistory();
                holdHis.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                holdHis.CreatedBy = HoldMaterialRequest.User;
                holdHis.CreatedDate = DateTime.Now;
                holdHis.HoldRemark = HoldMaterialRequest.HoldRemark;
                holdHis.Id = 0;
                holdHis.IsLocked = true;
                holdHis.MaterialNo = HoldMaterialRequest.MaterialNo;

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "HoldMaterial Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "HoldMaterial Params : " + JsonConvert.SerializeObject(HoldMaterialRequest));

                var checkMat = _unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(HoldMaterialRequest.MaterialNo);
                if (checkMat == null)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial");
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialByMaterial(HoldMatModel);
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial Success");
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial");
                    _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial(HoldMatModel);
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial Success");
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial");
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetResponseHoldMaterial(HoldMaterialRequest.MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial Success");


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
        [Route("UnHoldMaterial")]
        public IActionResult UnHoldMaterial([FromBody] HoldAndUnHoldMaterialRequestModel HoldMaterialRequest)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                HoldMaterial HoldMatModel = new HoldMaterial();
                HoldMatModel.Id = 0;
                HoldMatModel.IsLocked = false;
                HoldMatModel.HoldRemark = HoldMaterialRequest.HoldRemark;
                HoldMatModel.MaterialNo = HoldMaterialRequest.MaterialNo;
                HoldMatModel.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                HoldMatModel.CreatedBy = HoldMaterialRequest.User;
                HoldMatModel.UpdatedBy = HoldMaterialRequest.User;
                HoldMaterialHistory holdHis = new HoldMaterialHistory();
                holdHis.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                holdHis.CreatedBy = HoldMaterialRequest.User;
                holdHis.CreatedDate = DateTime.Now;
                holdHis.HoldRemark = HoldMaterialRequest.HoldRemark;
                holdHis.Id = 0;
                holdHis.IsLocked = false;
                holdHis.MaterialNo = HoldMaterialRequest.MaterialNo;
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "UnHoldMaterial Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "UnHoldMaterial Params : " + JsonConvert.SerializeObject(HoldMaterialRequest));
                var checkMat = _unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(HoldMaterialRequest.MaterialNo);
                if (checkMat == null)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial");
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialByMaterial(HoldMatModel);
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial Success");
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial");
                    _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial(HoldMatModel);
                    _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial Success");
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial");
                data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetResponseHoldMaterial(HoldMaterialRequest.MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial Success");


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
        [Authorize(Roles = "PMTs, PresaleSystem")]
        [Route("CheckMaterialHold")]
        public IActionResult CheckMaterialHold(string FactoryCode, string MaterialNo)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            //string FactoryCode = "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CheckMaterialHold Start");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CheckMaterialHold Params : " + FactoryCode + "-" + MaterialNo);
                var datas = _unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(MaterialNo);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial Success");
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "CheckMaterialHold Success");

                return Ok(new CustomResponse<HoldMaterial> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = datas });
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
        [Route("HoldListMaterial")]
        public IActionResult HoldListMaterial([FromBody] List<HoldAndUnHoldMaterialRequestModel> HoldMaterialRequestList)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                foreach (var HoldMaterialRequest in HoldMaterialRequestList)
                {
                    HoldMaterial HoldMatModel = new HoldMaterial();
                    HoldMatModel.Id = 0;
                    HoldMatModel.IsLocked = true;
                    HoldMatModel.HoldRemark = HoldMaterialRequest.HoldRemark;
                    HoldMatModel.MaterialNo = HoldMaterialRequest.MaterialNo;
                    HoldMatModel.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                    HoldMatModel.CreatedBy = HoldMaterialRequest.User;
                    HoldMatModel.UpdatedBy = HoldMaterialRequest.User;
                    HoldMaterialHistory holdHis = new HoldMaterialHistory();
                    holdHis.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                    holdHis.CreatedBy = HoldMaterialRequest.User;
                    holdHis.CreatedDate = DateTime.Now;
                    holdHis.HoldRemark = HoldMaterialRequest.HoldRemark;
                    holdHis.Id = 0;
                    holdHis.IsLocked = true;
                    holdHis.MaterialNo = HoldMaterialRequest.MaterialNo;

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "HoldMaterial Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "HoldMaterial Params : " + JsonConvert.SerializeObject(HoldMaterialRequest));

                    var checkMat = _unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(HoldMaterialRequest.MaterialNo);
                    if (checkMat == null)
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial");
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialByMaterial(HoldMatModel);
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial Success");
                    }
                    else
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial");
                        _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial(HoldMatModel);
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial Success");
                    }
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial");
                    data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetResponseHoldMaterial(HoldMaterialRequest.MaterialNo));
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial Success");
                }

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
        [Route("UnHoldListMaterial")]
        public IActionResult UnHoldListMaterial([FromBody] List<HoldAndUnHoldMaterialRequestModel> HoldMaterialRequestList)
        {

            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string FactoryCode = "";
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                foreach (var HoldMaterialRequest in HoldMaterialRequestList)
                {
                    HoldMaterial HoldMatModel = new HoldMaterial();
                    HoldMatModel.Id = 0;
                    HoldMatModel.IsLocked = false;
                    HoldMatModel.HoldRemark = HoldMaterialRequest.HoldRemark;
                    HoldMatModel.MaterialNo = HoldMaterialRequest.MaterialNo;
                    HoldMatModel.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                    HoldMatModel.CreatedBy = HoldMaterialRequest.User;
                    HoldMatModel.UpdatedBy = HoldMaterialRequest.User;
                    HoldMaterialHistory holdHis = new HoldMaterialHistory();
                    holdHis.ChangeProductNo = HoldMaterialRequest.ChangeProductNo;
                    holdHis.CreatedBy = HoldMaterialRequest.User;
                    holdHis.CreatedDate = DateTime.Now;
                    holdHis.HoldRemark = HoldMaterialRequest.HoldRemark;
                    holdHis.Id = 0;
                    holdHis.IsLocked = false;
                    holdHis.MaterialNo = HoldMaterialRequest.MaterialNo;
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "UnHoldMaterial Start");
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "UnHoldMaterial Params : " + JsonConvert.SerializeObject(HoldMaterialRequest));
                    var checkMat = _unitOfWork.ProductCatalogConfigRepository.GetHoldMaterialByMaterial(HoldMaterialRequest.MaterialNo);
                    if (checkMat == null)
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial");
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialByMaterial(HoldMatModel);
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.SaveHoldMaterialByMaterial Success");
                    }
                    else
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial");
                        _unitOfWork.ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial(HoldMatModel);
                        _unitOfWork.ProductCatalogConfigRepository.SaveHoldMaterialHistory(holdHis);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.UpdateHoldMaterialByMaterial Success");
                    }
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial");
                    data = JsonConvert.SerializeObject(_unitOfWork.ProductCatalogConfigRepository.GetResponseHoldMaterial(HoldMaterialRequest.MaterialNo));
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "ProductCatalogConfigRepository.GetResponseHoldMaterial Success");
                }

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
