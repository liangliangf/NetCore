using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MS.Common.Security;
using MS.Component.Jwt.UserClaim;
using MS.DbContexts;
using MS.Entities;
using MS.Entities.Core;
using MS.UnitOfWork.UnitOfWork;
using MS.WebCore;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Models.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "LoginViewModel_DisplayName_Account")]
        [Required(ErrorMessage = "DataAnnotation_ErrorMessage_Required")]
        [StringLength(16, ErrorMessage = "DataAnnotation_ErrorMessage_StringLength")]
        [RegularExpression(@"^[a-zA-Z0-9_]{4,16}$", ErrorMessage = "DataAnnotation_ErrorMessage_AccountRegular")]
        public string Account { get; set; }

        [Display(Name = "LoginViewModel_DisplayName_Password")]
        [Required(ErrorMessage = "DataAnnotation_ErrorMessage_Required")]
        public string Password { get; set; }

        public async Task<ExecuteResult<UserData>> LoginValidate(IUnitOfWork<MSDbContext> unitOfWork, IMapper mapper, SiteSetting siteSetting, IStringLocalizer localizer)
        {
            ExecuteResult<UserData> result = new ExecuteResult<UserData>();

            //将登录用户查出来
            var loginUserInDB = await unitOfWork.GetRepository<UserLogin>().FindAsync(Account);

            //用户不存在
            if (loginUserInDB is null)
            {
                //localizer.GetString("DataAnnotations_ErrorMessage_NotExist", localizer.GetString("LoginViewModel_DisplayName_Account"))
                return result.SetFailMessage(localizer.GetString("DataAnnotations_ErrorMessage_NotExist", new[] { localizer.GetString("LoginViewModel_DisplayName_Account") }));
                //return result.SetFailMessage("用户不存在");
            }

            //用户被锁定
            if (loginUserInDB.IsLocked &&
                loginUserInDB.LockedTime.HasValue &&
                (DateTime.Now - loginUserInDB.LockedTime.Value).Minutes < siteSetting.LoginLockTimeout)
            {
                return result.SetFailMessage(localizer.GetString("LoginViewModel_ServiceError_UserLocked", siteSetting.LoginLockTimeout));
                //return result.SetFailMessage(string.Format("用户已被锁定，请{0}分钟后再试！", siteSetting.LoginLockTimeout.ToString()));
            }

            //密码正确
            if (Crypto.VerifyHashedPassword(loginUserInDB.HashedPassword, Password))
            {
                //密码正确后才加载用户信息、角色信息
                var userInDB = await unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                    predicate: a => a.Id == loginUserInDB.UserId,
                    include: source => source.Include(u => u.Role));

                //如果用户已失效
                if (userInDB.StatusCode != StatusCode.Enable)
                {
                    return result.SetFailMessage(localizer.GetString("LoginViewModel_ServiceError_UserDisabled"));
                    //return result.SetFailMessage("用户已失效，请联系管理员！");
                }

                //用户正常、密码正确，更新相应字段
                loginUserInDB.IsLocked = false;
                loginUserInDB.AccessFailedCount = 0;
                loginUserInDB.LastLoginTime = DateTime.Now;
                //提交到数据库
                await unitOfWork.SaveChangesAsync();

                //得到userdata
                UserData userData = mapper.Map<UserData>(userInDB);
                return result.SetData(userData);
            }
            else
            {
                loginUserInDB.AccessFailedCount++;//失败次数累加
                result.SetFailMessage(localizer.GetString("LoginViewModel_ServiceError_LoginError"));
                //超出失败次数限制
                if (loginUserInDB.AccessFailedCount >= siteSetting.LoginFailedCountLimits)
                {
                    loginUserInDB.IsLocked = true;
                    loginUserInDB.LockedTime = DateTime.Now;
                    result.SetFailMessage(localizer.GetString("LoginViewModel_ServiceError_UserLocked", siteSetting.LoginLockTimeout));
                }
                //提交到数据库
                await unitOfWork.SaveChangesAsync();
                return result;
            }

        }
    }
}
