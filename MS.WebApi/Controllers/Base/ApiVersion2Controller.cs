using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MS.WebApi.Controllers.Base
{
    [SwaggerHelp.CustomRoute(SwaggerHelp.ApiVersions.v2)]
    [ApiController]
    public class ApiVersion2Controller : AuthorizeController
    {
    }
}