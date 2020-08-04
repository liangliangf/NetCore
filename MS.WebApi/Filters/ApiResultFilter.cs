using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MS.WebApi.Filters
{
    /// <summary>
    /// 对返回值又包了一层，status是状态码，data是数据
    /// </summary>
    public class ApiResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result != null)
            {
                if (context.Result is ObjectResult objectResult)
                {
                    if (objectResult.DeclaredType is null)//返回的是IActionResult类型
                    {
                        context.Result = new JsonResult(new
                        {
                            code = objectResult.StatusCode,
                            data = objectResult.Value
                        });
                    }
                    else //返回的是string、List这种其他类型，此时没有statusCode，应尽量使用IActionResult类型
                    {
                        context.Result = new JsonResult(new
                        {
                            code =200,
                            data = objectResult.Value
                        });
                    }
                }
                else if (context.Result is EmptyResult)
                {
                    context.Result = new JsonResult(new { 
                        code=200,
                        data=""
                    });
                }
                else
                {
                    throw new Exception($"未经处理的Result类型：{ context.Result.GetType().Name}");
                }
            }



            
        }
    }
}
