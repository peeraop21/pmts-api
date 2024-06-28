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
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class RoutingController(PMTsDbContext pmtsContext, IConfiguration configuration, IMapper mapper) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        private readonly IConfiguration config = configuration;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetAllByFactory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetAllByFactory(r => r.FactoryCode == FactoryCode && r.PdisStatus != "X"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetAllByFactory Success");
                //Logger.Info("RoutingController-Get");
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
                //   Logger.Error(exceptionMessage);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpPost]
        [Route("GetDapperRoutingByMat")]
        public IActionResult GetDapperRoutingByMat(string FactoryCode, [FromBody] List<string> Condition)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetDapperRoutingByMat, Params : " + FactoryCode);
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetDapperRoutingByMat(config, FactoryCode, Condition != null ? Condition[0] : ""));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetDapperRoutingByMat Success");
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
        [Route("GetRoutingsByMaterialNos")]
        public IActionResult GetRoutingsByMaterialNos(string AppName, string FactoryCode, [FromBody] List<string> materialNOs)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNos, Params : " + FactoryCode);
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetRoutingsByMaterialNos(FactoryCode, materialNOs));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNos Success");
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
        [Route("GetRoutingsByMaterialNo")]
        public IActionResult GetRoutingsByMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNo Success");
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
        [Route("GetRoutingsByMaterialNoContain")]
        public IActionResult GetRoutingsByMaterialNoContain(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetRoutingsByMaterialNoContain(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingsByMaterialNo Success");
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
        [Route("GetRoutingByMaterialNoAndMachine")]
        public IActionResult GetRoutingByMaterialNoAndMachine(string FactoryCode, string MaterialNo, string Machine)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetByPredicate(r => r.FactoryCode.Equals(FactoryCode) && r.MaterialNo.Equals(MaterialNo) && r.Machine.Equals(Machine) && r.PdisStatus != "X"));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");
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
        [Route("GetNumberOfRoutingByShipBlk")]
        public IActionResult GetNumberOfRoutingByShipBlk(string FactoryCode, string MaterialNo, bool semiBlk)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetNumberOfRoutingByShipBlk, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + semiBlk.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetNumberOfRoutingByShipBlk(FactoryCode, MaterialNo, semiBlk));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetNumberOfRoutingByShipBlk Success");
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
        [Route("GetRoutingByhandshake")]
        public IActionResult GetRoutingByhandshake(string FactoryCode, UpdateMatModel updateMatModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
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
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByhandshake, Params : " + FactoryCode.ToString() + " , " + materialno.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetRoutingByhandshake(config, FactoryCode, materialno));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByhandshake Success");
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
        public IActionResult Post(string FactoryCode, string Materialno, [FromBody] List<Routing> model)//, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.InfoRouting(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(model));
                if (model.Count > 0)
                {
                    var masterdata = new MasterData();
                    masterdata = _unitOfWork.MasterDatas.GetMasterDataByMaterialNumber(FactoryCode, model.FirstOrDefault(p => !string.IsNullOrEmpty(p.MaterialNo)).MaterialNo);
                    //สำหรับ auto คลัง โดยที่ คลังเป็น seq_no = 1
                    foreach (var modelupdate in model)
                    {
                        if (modelupdate.Machine.Contains("คลัง") && modelupdate.SeqNo == 1 && modelupdate.WeightIn == 0)
                        {
                            var weightbox = masterdata.WeightBox;
                            model.ToList().ForEach(s => s.WeightIn = weightbox);
                            model.ToList().ForEach(s => s.WeightOut = weightbox);
                        }
                    }

                    string Username = User.GetClaimValue("UserName");
                    model.ToList().ForEach(s => s.UpdatedBy = Username);
                    model.ToList().ForEach(s => s.UpdatedDate = DateTime.Now);

                    var MaterialNo = Materialno;//  model.FirstOrDefault();
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                    var checkUpdateMaster = model.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                    List<Routing> itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNoFactorycodeAndPlant(checkUpdateMaster.FactoryCode, checkUpdateMaster.Plant, MaterialNo).ToList();//(w => w.MaterialNo == modelToSave.MaterialNo);
                    if (itemToDelete.Count == 0)
                    {
                        itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNoFactorycodeAndPlant(checkUpdateMaster.FactoryCode, checkUpdateMaster.FactoryCode, MaterialNo).ToList();
                        if (itemToDelete.Count > 0)
                        {
                            model.ForEach(a => a.Plant = a.FactoryCode);
                        }
                    }

                    List<Routing2pc> routing2Pc = new List<Routing2pc>();
                    foreach (var item2pc in model)
                    {
                        var temp2pc = _mapper.Map<Routing2pc>(item2pc);
                        routing2Pc.Add(temp2pc);
                    }
                    _unitOfWork.Routings2pc.AddList(routing2Pc);

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");
                    if (checkUpdateMaster != null && checkUpdateMaster.Machine.Contains("คลัง"))
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateWeigthBox, Params : " + checkUpdateMaster.MaterialNo.ToString() + " , " + checkUpdateMaster.WeightOut.ToString());
                        _unitOfWork.MasterDatas.UpdateWeigthBox(checkUpdateMaster.MaterialNo, checkUpdateMaster.WeightOut, Username, FactoryCode);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateWeigthBox Success");

                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime, Params : " + checkUpdateMaster.MaterialNo.ToString());
                        _unitOfWork.MasterDatas.UpdateLeadTime(FactoryCode,checkUpdateMaster.MaterialNo, model, Username);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime Success");

                    }
                    if (!string.IsNullOrEmpty(masterdata?.BoxType))
                    {
                        var sizeDimension = _unitOfWork.Formulas.CalSizeDimensions(FactoryCode, MaterialNo);
                        if (!string.IsNullOrEmpty(sizeDimension))
                        {
                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateSizeDimension, Params : " + MaterialNo);
                            _unitOfWork.MasterDatas.UpdateSizeDimension(FactoryCode, MaterialNo, Username, sizeDimension);
                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateSizeDimension Success");
                        }
                    }
                    if (itemToDelete.Count > 0)
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveandAddList");
                        _unitOfWork.Routings.RemoveandAddList(itemToDelete, model);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveandAddList Success");
                    }
                    else
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.AddList");
                        _unitOfWork.Routings.AddList(model);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.AddList Success");
                    }
                    
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
        [Route("ExternalPost")]
        public IActionResult ExternalPost(string FactoryCode, string Materialno, [FromBody] List<Routing> model)//, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                if (model.Count > 0)
                {
                    string Username = User.GetClaimValue("UserName");
                    model.ToList().ForEach(s => s.UpdatedBy = Username);
                    model.ToList().ForEach(s => s.UpdatedDate = DateTime.Now);

                    var MaterialNo = Materialno;//  model.FirstOrDefault();
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                    var checkUpdateMaster = model.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                    List<Routing> itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNo(checkUpdateMaster.Plant, MaterialNo).ToList();//(w => w.MaterialNo == modelToSave.MaterialNo);
                    List<Routing2pc> routing2Pc = new List<Routing2pc>();
                    foreach (var item2pc in model)
                    {
                        var temp2pc = _mapper.Map<Routing2pc>(item2pc);
                        routing2Pc.Add(temp2pc);
                    }
                    _unitOfWork.Routings2pc.AddList(routing2Pc);

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");
                    if (checkUpdateMaster != null && checkUpdateMaster.Machine.Contains("คลัง"))
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateWeigthBox, Params : " + checkUpdateMaster.MaterialNo.ToString() + " , " + checkUpdateMaster.WeightOut.ToString());
                        _unitOfWork.MasterDatas.UpdateWeigthBox(checkUpdateMaster.MaterialNo, checkUpdateMaster.WeightOut, Username, FactoryCode);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateWeigthBox Success");

                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime, Params : " + checkUpdateMaster.MaterialNo.ToString());
                        _unitOfWork.MasterDatas.UpdateLeadTime(FactoryCode, checkUpdateMaster.MaterialNo, model, Username);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime Success");

                    }
                    if (itemToDelete.Count > 0)
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange");
                        _unitOfWork.Routings.RemoveandAddList(itemToDelete, model);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange Success");
                    }
                    else
                    {
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateList");
                        _unitOfWork.Routings.AddList(model);
                        Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateList Success");
                    }
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

        [HttpPut]
        public IActionResult Put(string FactoryCode, [FromBody] List<Routing> model)// , string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                string Username = User.GetClaimValue("UserName");
                model.ToList().ForEach(s => s.UpdatedBy = Username);
                model.ToList().ForEach(s => s.UpdatedDate = DateTime.Now);

                var routing = model.FirstOrDefault();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + routing.MaterialNo.ToString());
                List<Routing> itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, routing.MaterialNo).ToList();//(w => w.MaterialNo == modelToSave.MaterialNo);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");

                List<Routing2pc> routing2Pc = new List<Routing2pc>();

                foreach (var item2pc in model)
                {
                    var temp2pc = _mapper.Map<Routing2pc>(item2pc);
                    routing2Pc.Add(temp2pc);
                }
                _unitOfWork.Routings2pc.AddList(routing2Pc);

                
                var checkUpdateMaster = model.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                if (checkUpdateMaster != null && checkUpdateMaster.Machine.Contains("คลัง"))
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateWeigthBox, Params : " + checkUpdateMaster.MaterialNo.ToString() + " , " + checkUpdateMaster.WeightOut.ToString());
                    _unitOfWork.MasterDatas.UpdateWeigthBox(checkUpdateMaster.MaterialNo, checkUpdateMaster.WeightOut, Username, FactoryCode);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateWeigthBox Success");

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime, Params : " + checkUpdateMaster.MaterialNo.ToString());
                    _unitOfWork.MasterDatas.UpdateLeadTime(FactoryCode, checkUpdateMaster.MaterialNo, model, Username);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterDatas.UpdateLeadTime Success");

                }
                if (itemToDelete.Count > 0)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange");
                    _unitOfWork.Routings.RemoveandAddList(itemToDelete, model);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange Success");
                }
                else
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateList");
                    _unitOfWork.Routings.AddList(model);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateList Success");
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

        [HttpPut]
        [Route("Update1RowOfRouting")]
        public IActionResult Update1RowOfRouting(string FactoryCode, [FromBody] Routing model)// , string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.Update1RowOfRouting, Params : " + FactoryCode.ToString());
                _unitOfWork.Routings.Update(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.Update1RowOfRouting Success");
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
        [Route("UpdateRoutingPDISStatus")]
        public IActionResult UpdateRoutingPDISStatus(string FactoryCode, string MaterialNo, string Status)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutingPDISStatus, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + Status.ToString());
                _unitOfWork.Routings.UpdatePdisStatus(FactoryCode, MaterialNo, Status);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutingPDISStatus Success");
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
        [Route("UpdateRoutings")]
        public IActionResult UpdateRoutings(string FactoryCode, [FromBody] List<Routing> routings)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutings, Params : " + FactoryCode.ToString());
                string Username = User.GetClaimValue("UserName");
                routings.ToList().ForEach(s => s.UpdatedBy = Username);
                routings.ToList().ForEach(s => s.UpdatedDate = DateTime.Now);
                _unitOfWork.Routings.UpdateRoutings(routings);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutings Success");
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
        [Route("DeleteRoutingByMatAndFac")]
        public IActionResult DeleteRoutingByMatAndFac(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                List<Routing> itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, MaterialNo).ToList();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");
                if (itemToDelete.Count > 0)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange");
                    _unitOfWork.Routings.RemoveRange(itemToDelete);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange Success");
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
        [Route("DeleteRoutingByMatAndFacAndSeq")]
        public IActionResult DeleteRoutingByMatAndFacAndSeq(string FactoryCode, string MaterialNo, string Seq)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString());
                List<Routing> itemToDelete = _unitOfWork.Routings.GetRoutingByMaterialNo(FactoryCode, MaterialNo).ToList();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.GetRoutingByMaterialNo Success");
                if (itemToDelete.Count > 0)
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange");
                    _unitOfWork.Routings.RemoveRange(itemToDelete.Where(s => s.SeqNo == Convert.ToInt16(Seq)).ToList());
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.RemoveRange Success");
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
        [Route("UpdateRoutingPDISStatusEmployment")]
        public IActionResult UpdateRoutingPDISStatusEmployment(string FactoryCode, string MaterialNo, string SaleOrg, string Status)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutingPDISStatusEmployment, Params : " + FactoryCode.ToString() + " , " + MaterialNo.ToString() + " , " + SaleOrg.ToString());
                string Username = User.GetClaimValue("UserName");
                _unitOfWork.Routings.UpdatePdisStatusEmployment(FactoryCode, MaterialNo, SaleOrg, Username, Status);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateRoutingPDISStatusEmployment Success");
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
        [Route("UpdateReCalculateTrimFromFile")]
        public IActionResult UpdateReCalculateTrimFromFile(string FactoryCode, [FromBody] List<ReCalculateTrimModel> reCalculateTrimModels)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateReCalculateTrimFromFile, Params : " + FactoryCode.ToString());
                string Username = User.GetClaimValue("UserName");
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.UpdateReCalculateTrim(config, FactoryCode, reCalculateTrimModels));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Routings.UpdateReCalculateTrimFromFile Success");
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
        [Route("GetRoutingListByDateTime")]
        public IActionResult GetRoutingListByDateTime(string FactoryCode, string DateFrom, string DateTo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MoDatas.GetMasterDataListBySO, Params : " + FactoryCode.ToString() + "," + DateFrom + "," + DateTo);
                data = JsonConvert.SerializeObject(_unitOfWork.Routings.GetRoutingListByDateTime(FactoryCode, DateFrom, DateTo));
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
    }
}
