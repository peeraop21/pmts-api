using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "PMTs")]
    [ApiController]
    public class BoardCostController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public BoardCostController(PMTsDbContext pmtsContext)
        {
            _unitOfWork = new UnitOfWork(pmtsContext);
        }

        /*
        [HttpGet]
        public JsonResult Get(string AppName, string FactoryCode)
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
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.GetAllByFactory, Params : " + FactoryCode.ToString());
                    data = JsonConvert.SerializeObject(_unitOfWork.BoardCosts.GetAllByFactory(b=>b.FactoryCode == FactoryCode));
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.GetAllByFactory Success");
                    isSuccess = true;
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

        [HttpGet]
        [Route("GetBoardCostByCode")]
        public JsonResult GetBoardCostByCode(string AppName, string FactoryCode, string BoardId, string costField)
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
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.GetBoardCostByCode, Params : " + FactoryCode.ToString() +" , "+BoardId.ToString()  +" , "+ costField.ToString());
                    data = JsonConvert.SerializeObject(_unitOfWork.BoardCosts.GetBoardCostByCode(FactoryCode, BoardId, costField));
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.GetBoardCostByCode Success");
                    isSuccess = true;
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

        [HttpPost]
        public JsonResult Post(string AppName, string FactoryCode,[FromBody] BoardCost model)
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
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.Add");
                    _unitOfWork.BoardCosts.Add(model);
                    Logger.Info(AppCaller, FactoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "BoardCosts.Add Success");
                    isSuccess = true;
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

        }*/
    }
}
