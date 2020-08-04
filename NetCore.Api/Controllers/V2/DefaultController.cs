using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Api.SwaggerHelp;

namespace NetCore.Api.Controllers.V2
{
    [CustomRoute(SwaggerHelp.CustomApiVersion.ApiVersions.v2)]
    [ApiController]
    [Authorize]
    public class DefaultController : ControllerBase
    {
        /// <summary>
        /// 获取名字
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("李四");
        }
    }
}