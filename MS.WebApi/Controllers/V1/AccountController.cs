using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Component.Jwt.UserClaim;
using MS.Models.ViewModel;
using MS.Services.Account;
using MS.WebCore.Core;

namespace MS.WebApi.Controllers.V1
{
    [ApiController]
    public class AccountController : ApiVersion1Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService; 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ExecuteResult<UserData>> Login(LoginViewModel viewModel)
        {
            return await _accountService.Login(viewModel);   
        }
    }
}