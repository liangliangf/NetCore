using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MS.Component.Jwt.UserClaim
{
    /// <summary>
    /// 这个类是声明用户信息的
    /// 里面的值都是从System.Security.Claims.ClaimTypes里挑选出来的值，也可以自行定义
    /// </summary>
    public static class UserClaimType
    {
        public const string Id = "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid";
        public const string Account = "http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber";
        public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public const string Phone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone";
        public const string RoleName = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        public const string RoleDisplayName = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor";
    }
}
