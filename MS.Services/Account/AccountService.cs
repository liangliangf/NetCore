using AutoMapper;
using Microsoft.Extensions.Options;
using MS.Common.IDCode;
using MS.Component.Jwt;
using MS.Component.Jwt.UserClaim;
using MS.DbContexts;
using MS.Models.ViewModel;
using MS.UnitOfWork.UnitOfWork;
using MS.WebCore;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;


namespace MS.Services.Account
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly SiteSetting _siteSetting;
        private readonly JwtService _jwtService;

        public AccountService(IOptions<SiteSetting> siteSetting, JwtService jwtService, IUnitOfWork<MSDbContext> unitOfWork, IMapper mapper, IdWorker idWorker, IClaimsAccessor claimsAccessor,IStringLocalizer localizer) : base(unitOfWork, mapper, idWorker, claimsAccessor, localizer)
        {
            _siteSetting = siteSetting.Value;
            _jwtService = jwtService;
        }

        public async Task<ExecuteResult<UserData>> Login(LoginViewModel viewModel)
        {
            var result = await viewModel.LoginValidate(_unitOfWork, _mapper, _siteSetting, _localizer);
            if (result.IsSucceed)
            {
                //登录成功创建Token
                string token = _jwtService.BuildToken(_jwtService.BuildClaims(result.Result));
                result.Result.Token = token;
                return new ExecuteResult<UserData>(result.Result);
            }

            return new ExecuteResult<UserData>(result.Message);
        }
    }
}
