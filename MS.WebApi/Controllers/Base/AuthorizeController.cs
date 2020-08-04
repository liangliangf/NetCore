using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MS.WebApi.Controllers //注意命名空间(和控制器在一个命名空间下)
{
    //[Route("api/[controller]")]
    [Authorize]//需要认证授权后才能访问
    public class AuthorizeController : ControllerBase
    {
    }
}