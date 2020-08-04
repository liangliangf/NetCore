using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MS.Component.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.WebApi.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            string methodInfo = $"{context.RouteData.Values["controller"] as string}Controller.{context.RouteData.Values["action"] as string}:{context.HttpContext.Request.Method}";

            //如果不是AopHandledException异常，则可能没有记录过日志，进行日志记录
            if (!(context.Exception is AopHandledException))
            {
                _logger.LogError(methodInfo);
            }

            context.Result = new JsonResult(new { 
                status=501,
                ErrorMessage= "服务器错误",
            });
        }
    }
}
