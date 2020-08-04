using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MS.Component.Jwt.UserClaim;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MS.Component.Jwt
{
    public class JwtService
    {
        private readonly JwtSetting _jwtSetting;
        private readonly TimeSpan _tokenLifeTime;

        public JwtService(IOptions<JwtSetting> options)
        {
            _jwtSetting = options.Value;
            _tokenLifeTime = TimeSpan.FromMinutes(options.Value.LifeTime);
        }

        /// <summary>
        /// 生成身份信息
        /// </summary>
        /// <param name="userData">要声明的用户信息</param>
        /// <returns></returns>
        public Claim[] BuildClaims(UserData userData)
        {
            // 配置用户标识
            var userClaims = new Claim[]
            {
                //new Claim(ClaimTypes.Name,userData.Name.ToString()),//  为什么不用 ClaimTypes.Name 
                new Claim(UserClaimType.Id,userData.Id.ToString()),//id
                new Claim(UserClaimType.Account,userData.Account),//account
                new Claim(UserClaimType.Name,userData.Name),//name
                new Claim(UserClaimType.RoleName,userData.RoleName),//rolename
                new Claim(UserClaimType.RoleDisplayName,userData.RoleDisplayName),//roledisplayname
                new Claim(JwtRegisteredClaimNames.Jti,userData.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                //new Claim(JwtRegisteredClaimNames.Iss,_jwtSetting.Issuer),
                //new Claim(JwtRegisteredClaimNames.Aud,_jwtSetting.Audience),
                //new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //这个就是过期时间，可自定义，注意JWT有自己的缓冲过期时间
                //new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.Add(_tokenLifeTime)).ToUnixTimeSeconds()}"),
            };
            return userClaims;
        }

        /// <summary>
        /// 生成jwt令牌
        /// </summary>
        /// <param name="claims">自定义的claim</param>
        /// <returns></returns>
        public string BuildToken(Claim[] claims)
        {
            var nowTime = DateTime.Now;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken tokenkey = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                notBefore: nowTime,
                expires: nowTime.Add(_tokenLifeTime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenkey);
        }
    }

}
