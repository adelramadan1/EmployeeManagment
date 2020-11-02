using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Fluent;

namespace EmployeeManagment.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var exceptionDetails = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry,the resources you requested not found";
                    logger.LogWarning($"404 Error occured Path ={ exceptionDetails.OriginalPath}"+ $"and QueryString ={exceptionDetails.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //ViewBag.ExceptionPath = exceptionDetails.Path;
            //ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            //ViewBag.StackTrace = exceptionDetails.Error.StackTrace;
            logger.LogError($"the Path {exceptionDetails.Path}"+" throw an Exception "+$"{ exceptionDetails.Error}");
            return View("Error");
        }
    }
}