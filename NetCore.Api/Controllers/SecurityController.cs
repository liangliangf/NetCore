using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NetCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private IConfiguration _config;

        public SecurityController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="Name">用户名</param>
        /// <param name="Pwd">密码</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string Name, string Pwd)
        {
            IActionResult response = Unauthorized();
            UserModel user = new UserModel
            {
                Name = Name,
                Password = Pwd,
                Birthdate = Convert.ToDateTime("2020-5-5")
            };

            #region 验证逻辑
            if (user.Name == "admin" && user.Password == "123456")
            {
                var tokenString = BuildToken(user);
                response = Ok(new { 
                    User = user,
                    access_token = tokenString,
                    token_type = "Bearer"
                });
            } 
            #endregion

            return response;
        }

        private string BuildToken(UserModel user)
        {
            //添加Claims信息(不要包含敏感信息)
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt_token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,//添加claims
                notBefore:DateTime.Now,//生效时间
                expires: DateTime.Now.AddMinutes(30), //过期时间
                signingCredentials: creds);

            //一个典型的JWT 字符串由三部分组成:

            //header: 头部,meta信息和算法说明
            //payload: 负荷(Claims), 可在其中放入自定义内容, 比如, 用户身份等
            //signature: 签名, 数字签名, 用来保证前两者的有效性

            //三者之间由.分隔, 由Base64编码.根据Bearer 认证规则, 添加在每一次http请求头的Authorization字段中, 这也是为什么每次这个字段都必须以Bearer jwy - token这样的格式的原因.
            return new JwtSecurityTokenHandler().WriteToken(jwt_token);
        }

        private class UserModel
        {
            //public Users user { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public DateTime Birthdate { get; set; }
        }
    }
    
}