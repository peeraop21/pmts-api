using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using PMTs.Logs.Logger;
using PMTs.WebAPI.JwtHelpers;
using PMTs.WebAPI.ResponseFormats;
using PMTs.WebAPI.Utility;
using PMTs.WebAPI.Utils;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoginController(PMTsDbContext pmtsContext, IConfiguration configuration) : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(pmtsContext);
        public IConfiguration _configuration { get; } = configuration;

        [HttpPost("login")]
        public JsonResult Post(string AppName, string FactoryCode, [FromBody] MasterUser model)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var data = string.Empty;
            string AppCaller = "";
            bool isAuthen = false;
            Extensions.Decrypt(AppName, ref AppCaller, ref isAuthen);
            Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            if (isAuthen)
            {
                try
                {
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername");
                    data = JsonConvert.SerializeObject(_unitOfWork.MasterUsers.GetUsername(model.UserName, model.Password, model.UserDomain));

                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "MasterUsers.GetUsername Success");

                    if (data == "null")
                    {
                        isSuccess = false;
                        exceptionMessage = "Data Is Null";
                    }
                    else
                    {
                        isSuccess = true;
                        var token = new JwtTokenBuilder()
                         .AddSecurityKey(JwtSecurityKey.Create(_configuration.GetValue<string>("JwtSecretKey")))
                         .AddIssuer(_configuration.GetValue<string>("JwtIssuer"))
                         .AddAudience(_configuration.GetValue<string>("JwtAudience"))
                         .AddExpiry(1)
                         .AddClaim("AppName", "Admin")
                         .AddClaim("Factory", "259B")
                         .AddClaim("UserName", "Boo")
                         .AddRole("PMTs")
                         .Build();
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
                return Json(new { isSuccess = isSuccess, exceptionMessage = exceptionMessage, data = data });
            }
            else
            {
                return Json(new { isSuccess = false, exceptionMessage = Extensions.AUTHEN_FIAL, data = data });
            }
        }

        #region [Login Api]
        [HttpPost]
        [Route("LoginApi")]
        public IActionResult Login(MasterUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var user = _unitOfWork.MasterUsers.GetAll().Where(x => x.Email == model.Email && x.Password == model.Password).FirstOrDefault();

                if (user != null)
                {
                    // var userDTO = Mapper.Map<MasterUser, UserDTO>(user);

                    var userDTO = new UserDTO();
                    userDTO.Id = user.Id;

                    userDTO.GenerateToken(_configuration);

                    return Ok(new CustomResponse<UserDTO> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = userDTO });
                }
                else
                {
                    return Ok(new CustomResponse<Error>
                    {
                        Message = Global.ResponseMessages.Forbidden,
                        StatusCode = StatusCodes.Status403Forbidden,
                        Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("username or password") }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Error.LogError(ex));
            }
        }

        [Route("LoginApiAsAdmin")]
        [HttpGet]
        public IActionResult LoginAsAdmin(string username, string password)
        {
            if (username == "Admin" && password == "Pass")
            {
                var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create(_configuration.GetValue<string>("JwtSecretKey")))
                                .AddIssuer(_configuration.GetValue<string>("JwtIssuer"))
                                .AddAudience(_configuration.GetValue<string>("JwtAudience"))
                                .AddExpiry(1)
                                .AddClaim("AppName", "Admin")
                                .AddClaim("Factory", "259B")
                                .AddClaim("UserName", "Boo")
                                .AddRole("PMTs")
                                .Build();

                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = token.Value });
            }
            else
            {
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("username or password") } });
            }
        }

        [Route("GetUser")]
        [Authorize(Roles = "PMTs, Other")]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = "You are an authorized user" });
        }

        [Route("GetAdmin")]
        [Authorize(Roles = "PMTs")]
        [HttpGet]
        public IActionResult GetAdmin()
        {
            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = "You are an authorized user" });
        }

        [Route("GetAdmin")]
        [Authorize(Roles = "PMTs")]
        [HttpPost]
        public IActionResult UpdateAdmin(string AppName, string FactoryCode, [FromBody] MasterUser model)
        {
            try
            {
                var name = User.GetClaimValue("Name");
                model.FirstNameEn = name;
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = StatusCodes.Status200OK, Result = JsonConvert.SerializeObject(model) });
            }
            catch (Exception ex)
            {
                string exceptionMessage = "";
                if (ex.Message.Contains(Messages.INNER_EXCEPTION))
                {
                    exceptionMessage = ex.InnerException.Message;
                }
                else
                {
                    exceptionMessage = ex.Message;
                }
                Logger.Error("", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Forbidden, StatusCode = StatusCodes.Status403Forbidden, Result = new Error { ErrorMessage = exceptionMessage } });
            }
        }
        #endregion
    }
}
