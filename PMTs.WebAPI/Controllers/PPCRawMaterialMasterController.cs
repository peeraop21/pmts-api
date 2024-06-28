using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class PPCRawMaterialMasterController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private char[] delimiterChars = { '*' };
        public PPCRawMaterialMasterController(PMTsDbContext pmtsContext)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
        }

        [HttpGet]
        [Route("SearchPPCRawMaterialMasterByMaterialNo")]
        public IActionResult SearchPPCRawMaterialMasterByMaterialNo(string FactoryCode, string MaterialNo, string MaterialDesc)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;


            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.SearchPPCRawMaterialMasterByMaterialNo, Params : " + FactoryCode.ToString());
                var splitMatDesc = !string.IsNullOrEmpty(MaterialDesc) ? MaterialDesc.Split(this.delimiterChars).ToList() : new List<string>();
                var rawData = new List<PpcRawMaterialMaster>();
                var counter = 0;
                if (splitMatDesc.Count > 1)
                {
                    splitMatDesc = splitMatDesc.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var matDesc in splitMatDesc)
                    {
                        if (counter == 0)
                        {
                            rawData.AddRange(_unitOfWork.PPCRawMaterialMaster.SearchPPCRawMaterialMasterByMaterialNo(MaterialNo, matDesc));
                        }
                        else
                        {
                            if (rawData.Count > 0)
                            {
                                rawData = rawData.Where(x => x.MaterialDescription.Contains(matDesc)).ToList();
                            }
                            else
                            {
                                rawData.AddRange(_unitOfWork.PPCRawMaterialMaster.SearchPPCRawMaterialMasterByMaterialNo(MaterialNo, matDesc));
                            }
                        }
                        counter++;
                    }
                    if (!string.IsNullOrEmpty(MaterialNo))
                    {
                        rawData = rawData.Where(p => p.MaterialNumber.Contains(MaterialNo)).ToList();
                    }
                    data = JsonConvert.SerializeObject(rawData);
                }
                else
                {
                    data = JsonConvert.SerializeObject(_unitOfWork.PPCRawMaterialMaster.SearchPPCRawMaterialMasterByMaterialNo(MaterialNo, MaterialDesc));
                }

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "AutoPackingSpec.GetAutoPackingSpecByMaterialNo Success");
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
        [Route("GetPPCRawMaterialMasterByFactoryAndMaterialNo")]
        public IActionResult GetPPCRawMaterialMasterByFactoryAndMaterialNo(string FactoryCode, string MaterialNo)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMasterByMaterialNo, Params : " + FactoryCode.ToString() + MaterialNo);
                data = JsonConvert.SerializeObject(_unitOfWork.PPCRawMaterialMaster.GetPPCRawMaterialMasterByMaterialNo(FactoryCode, MaterialNo));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMasterByMaterialNo Success");
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
        [Route("GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription")]
        public IActionResult GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(string FactoryCode, string MaterialNo, string MaterialDesc)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription, Params : " + FactoryCode.ToString() + MaterialNo + MaterialDesc);
                var splitMatDesc = !string.IsNullOrEmpty(MaterialDesc) ? MaterialDesc.Split(this.delimiterChars).ToList() : new List<string>();
                var rawData = new List<PpcRawMaterialMaster>();
                var counter = 0;
                if (splitMatDesc.Count > 1)
                {
                    splitMatDesc = splitMatDesc.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    foreach (var matDesc in splitMatDesc)
                    {
                        if (counter == 0)
                        {
                            rawData.AddRange(_unitOfWork.PPCRawMaterialMaster.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(FactoryCode, MaterialNo, matDesc));
                        }
                        else
                        {
                            if (rawData.Count > 0)
                            {
                                rawData = rawData.Where(x => x.MaterialDescription.Contains(matDesc)).ToList();
                            }
                            else
                            {
                                rawData.AddRange(_unitOfWork.PPCRawMaterialMaster.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(FactoryCode, MaterialNo, matDesc));
                            }
                        }
                        counter++;
                    }
                    if (!string.IsNullOrEmpty(MaterialNo))
                    {
                        rawData = rawData.Where(p => p.MaterialNumber.Contains(MaterialNo)).ToList();
                    }
                    data = JsonConvert.SerializeObject(rawData);
                }
                else
                {
                    data = JsonConvert.SerializeObject(_unitOfWork.PPCRawMaterialMaster.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(FactoryCode, MaterialNo, MaterialDesc));
                }
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription Success");
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
        [Route("GetPPCRawMaterialMastersByFactoryCode")]
        public IActionResult GetPPCRawMaterialMastersByFactoryCode(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMastersByFactoryCode, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.PPCRawMaterialMaster.GetPPCRawMaterialMastersByFactoryCode(FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMasterRepository.GetPPCRawMaterialMastersByFactoryCode Success");
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
        [Route("GetPPCRawMaterialMasterById")]
        public IActionResult GetPPCRawMaterialMasterById(string FactoryCode, int Id)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetPPCRawMaterialMasterById, Params : " + Id.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.PPCRawMaterialMaster.GetById(Id));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "GetPPCRawMaterialMasterById Success");
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
        public IActionResult Post(string FactoryCode, [FromBody] PpcRawMaterialMaster model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PpcRawMaterialMaster.Add");
                _unitOfWork.PPCRawMaterialMaster.Add(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PpcRawMaterialMaster.Add Success");
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
        public IActionResult Put(string FactoryCode, [FromBody] PpcRawMaterialMaster model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PpcRawMaterialMaster.Update");
                _unitOfWork.PPCRawMaterialMaster.Update(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PpcRawMaterialMaster.Update Success");
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
        [Route("Delete")]
        public IActionResult DeletePPCRawMaterialMaster(string FactoryCode, [FromBody] PpcRawMaterialMaster model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;

            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMaster.Remove");
                _unitOfWork.PPCRawMaterialMaster.Remove(model);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "PPCRawMaterialMaster.Remove Success");
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
