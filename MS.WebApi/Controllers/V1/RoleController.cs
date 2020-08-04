using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.DbContexts;
using MS.Entities;
using MS.Models.ViewModel;
using MS.Services;
using MS.UnitOfWork.UnitOfWork;
using MS.WebCore.Core;

namespace MS.WebApi.Controllers.V1
{
    [ApiController]
    public class RoleController : ApiVersion1Controller  //AuthorizeController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<ExecuteResult> Post(RoleViewModel viewModel)
        {
            return await _roleService.Create(viewModel);
        }

        [HttpPut]
        public async Task<ExecuteResult> Put(RoleViewModel viewModel)
        {
            return await _roleService.Update(viewModel);
        }

        [HttpDelete]
        public async Task<ExecuteResult> Delete(RoleViewModel viewModel)
        {
            return await _roleService.Delete(viewModel);
        }

    }
}