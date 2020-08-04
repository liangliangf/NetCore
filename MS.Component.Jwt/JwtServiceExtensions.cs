using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MS.Component.Jwt.UserClaim;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Component.Jwt
{
    public static class JwtServiceExtensions
    {
        public static IServiceCollection AddJwtServic(this IServiceCollection services, IConfiguration configuration)
        {
            //绑定appsetting中的jwtsetting
            services.Configure<JwtSetting>(configuration.GetSection(nameof(JwtSetting)));

            //注册jwtservice
            services.AddSingleton<JwtService>();

            //注册 HttpContextAccessor
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            //注册 ClaimsAccessor
            services.AddScoped<IClaimsAccessor, ClaimsAccessor>();

            var jwtConfig = configuration.GetSection(nameof(JwtSetting));

            //添加身份认证
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,//是否验证密钥
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["SecurityKey"])),

                    ValidateIssuer = true,//是否验证发行人
                    ValidIssuer = jwtConfig["Issuer"],

                    ValidateAudience = true,//是否验证受众人
                    ValidAudience = jwtConfig["Audience"],

                    //总的Token有效时间 = JwtRegisteredClaimNames.Exp + ClockSkew ；
                    RequireExpirationTime = true,
                    ValidateLifetime = true,// 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比.同时启用ClockSkew 
                    ClockSkew = TimeSpan.Zero //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
            };

        });


            return services;
        }
}
}
