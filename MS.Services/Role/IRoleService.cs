using MS.Entities;
using MS.Models.ViewModel;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services //MS.Services.Role
{
    public interface IRoleService:IBaseService
    {
        Task<ExecuteResult<Role>> Create(RoleViewModel viewModel);
        Task<ExecuteResult> Update(RoleViewModel viewModel);
        Task<ExecuteResult> Delete(RoleViewModel viewModel);
    }
}
