using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using AutoMapper.Configuration;


using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.JwtHelpers;
using PMTs.WebAPI.Models;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    // [Authorize(Roles = "PMTs")]
    [ApiController]
    public class MasterUserController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        public IConfiguration _configuration { get; } = configuration;

        [HttpGet]
        [Authorize(Roles = "PMTs")]
        public IActionResult Get(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByFactory, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetAllByFactory(m => m.FactoryCode == FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByFactory Success");
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
        [Authorize(Roles = "PMTs")]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                Logger.Info(AppCaller, "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAll");
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetAll());
                Logger.Info(AppCaller, "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAll Success");
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
                Logger.Error(AppCaller, "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }

        [HttpGet]
        [Authorize(Roles = "PMTs")]
        [Route("GetRole")]
        public IActionResult GetRole(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterRoles.GetAll");
                data = JsonConvert.SerializeObject(_unitOfWork.MasterRoles.GetAll());
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterRoles.GetAll Success");
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
        [Authorize(Roles = "PMTs")]
        [Route("GetUserWithRole")]
        public IActionResult GetUserWithRole(string FactoryCode)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetMasterUserRoleAll, Params : " + FactoryCode.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetMasterUserRoleAll(FactoryCode));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetMasterUserRoleAll Success");
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
        [Authorize(Roles = "PMTs")]
        [Route("GetAccountById")]
        public IActionResult Get(string FactoryCode, int Id)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetById, Params : " + Id.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetById(Id));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetById Success");
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
        [Authorize(Roles = "PMTs")]
        public IActionResult Post(string FactoryCode, [FromBody] ParentModel parentModel)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Add");
                parentModel.MasterUser.AppName = "PMTs";
                _unitOfWork.MasterUsers.Add(parentModel.MasterUser);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Add Success");
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
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update");
                parentModel.MasterUser.AppName = "PMTs";
                _unitOfWork.MasterUsers.Update(parentModel.MasterUser);
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update Success");
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
        [Route("Login")]
        public IActionResult PostLogin(string FactoryCode, [FromBody] MasterUser model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            var exceptionMessage = string.Empty;
            var stepperError = string.Empty;
            var data = string.Empty;
            string errorGenerate = "";
            bool isSuccess = false;
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                isSuccess = true;
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByPredicate, Params : " + model.UserName.ToString());
                var user = _unitOfWork.MasterUsers.GetAllByPredicate(m => m.UserName == model.UserName).FirstOrDefault();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByPredicate Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername, Params : " + model.UserName.ToString() + " , " + model.Password.ToString());
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetUsername(model.UserName, model.Password, user.UserDomain));
                if (data == "null")
                {
                    stepperError = "Y";
                }
                else
                {
                    var dataIsToken = JsonConvert.DeserializeObject<UserDTO>(data);
                    if (string.IsNullOrEmpty(user.AppName))
                    {
                        errorGenerate = "AppName is null";
                    }
                    else if (string.IsNullOrEmpty(user.FactoryCode))
                    {
                        errorGenerate = "FactoryCode is null";
                    }
                    else if (string.IsNullOrEmpty(user.UserName))
                    {
                        errorGenerate = "Username is null";
                    }
                    //Generate Token
                    var token = new JwtTokenBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create(_configuration.GetValue<string>("JwtSecretKey")))
                    .AddIssuer(_configuration.GetValue<string>("JwtIssuer"))
                    .AddAudience(_configuration.GetValue<string>("JwtAudience"))
                    .AddExpiry(1)
                    .AddClaim("AppName", user.AppName)
                    .AddClaim("Factory", user.FactoryCode)
                    .AddClaim("UserName", user.UserName)
                    .AddRole(user.AppName)
                    .Build();
                    dataIsToken.Token = token.Value;
                    data = JsonConvert.SerializeObject(dataIsToken);
                }

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername Success");

                if (string.IsNullOrEmpty(user.UserDomain))
                {
                    if (data == "null")
                    {
                        isSuccess = false;
                        exceptionMessage = "Data Is Null";

                        if (user != null)
                        {
                            user.NumberOfLogins++;
                            if (user.NumberOfLogins > 5)
                            {
                                exceptionMessage = "Locked User";
                                user.IsLockedOut = true;
                            }

                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update");
                            _unitOfWork.MasterUsers.Update(user);
                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update Success");
                        }
                    }
                    else
                    {
                        isSuccess = true;
                        var dateLastLogin = user.LastLoginDate != null ? (DateTime.Now - user.LastLoginDate.Value).TotalDays : 0;
                        var dateLastPasswordChange = user.LastPasswordChangeDate != null ? (DateTime.Now - user.LastPasswordChangeDate.Value).TotalDays : 0;
                        var isLockedOut = user.IsLockedOut != null ? user.IsLockedOut.Value : false;

                        if (dateLastLogin >= 90 || dateLastPasswordChange >= 90)
                        {
                            isSuccess = false;
                            exceptionMessage = "Change Password";
                        }
                        else if (isLockedOut)
                        {
                            isSuccess = false;
                            exceptionMessage = "Locked User";
                        }
                        else
                        {
                            user.NumberOfLogins = 0;
                            user.LastLoginDate = DateTime.Now;
                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update");
                            _unitOfWork.MasterUsers.Update(user);
                            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.Update Success");
                        }
                    }
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
                isSuccess = false;
            }

            if (isSuccess)
            {
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = data });
            }
            else
            {
                if (!string.IsNullOrEmpty(stepperError))
                {
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = "User Or Password not found!" } });
                }
                else
                {
                    if (errorGenerate == "")
                    {
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = errorGenerate } });
                    }
                }
            }
        }

        [HttpPost]
        [Route("CheckLogin")]
        public IActionResult CheckLogin(string FactoryCode, [FromBody] MasterUser model)
        {
            string AppCaller = User.GetClaimValue("AppName");
            string exceptionMessage;
            var data = string.Empty;
            string userDomain = string.IsNullOrEmpty(model.UserDomain) ? null : model.UserDomain.ToString();
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername, Params : " + model.UserName.ToString() + " , " + model.Password.ToString() + " , " + userDomain);
                data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetUsername(model.UserName, model.Password, model.UserDomain));
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername Success");

                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByPredicate, Params : " + model.UserName.ToString());
                var user = _unitOfWork.MasterUsers.GetAllByPredicate(m => m.UserName == model.UserName).FirstOrDefault();
                Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetAllByPredicate Success");
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
