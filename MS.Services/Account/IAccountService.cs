using MS.Component.Jwt.UserClaim;
using MS.Models.ViewModel;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services.Account
{
    public interface IAccountService:IBaseService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task<ExecuteResult<UserData>> Login(LoginViewModel viewModel);
    }
}
