using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Api.SwaggerHelp;

namespace NetCore.Api.Controllers.V1
{
    //Tip：[CustomRoute] 中重新了Route和ApiExplorerSettings
    //[Route("api/[controller]")]
    //[ApiExplorerSettings(GroupName ="v1")]

    [CustomRoute(SwaggerHelp.CustomApiVersion.ApiVersions.v1)]
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
            return Ok("张三");
        }
    }
}