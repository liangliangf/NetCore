using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MS.WebApi.Controllers
{
    [SwaggerHelp.CustomRoute(SwaggerHelp.ApiVersions.v1)]
    public class ApiVersion1Controller : AuthorizeController
    {
    }
}