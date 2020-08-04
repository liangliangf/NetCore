using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore.Api.SwaggerHelp;

namespace NetCore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private string ApiName = "API多版本接口";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                typeof(SwaggerHelp.CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API {version}",
                        Version = version,
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "人间有妖气-管理员", Url = new Uri("https://www.cnblogs.com/songl/"), Email = "2769376955@qq.com", },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense() { Name = "人间有妖气-管理员许可证", Url = new Uri("https://www.cnblogs.com/songl/") }
                    });
                });


                #region Swagger启用JWT（可以将apikey加到请求头中）
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme(){
                            Reference=new OpenApiReference{
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },new string[]{ }
                    }
                });
                #endregion


                var xmlPath = typeof(Program).Assembly.Location.Replace("dll", "xml");
                c.IncludeXmlComments(xmlPath);
            });

            #region JwtBearer身份认证
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证发行人
                        ValidateAudience = true,//是否验证受众人
                        ValidateIssuerSigningKey = true,//是否验证密钥
                        ValidateLifetime = true,//验证生命周期
                        RequireExpirationTime = true,//过期时间 --
                        ValidIssuer = Configuration["Jwt:Issuer"],//发行人
                        ValidAudience = Configuration["Jwt:Audience"],//受众人
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),//密钥
                    };
                });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                typeof(CustomApiVersion.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    option.SwaggerEndpoint($"swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                option.RoutePrefix = string.Empty;//swagger 访问地址前缀，如果未设置则为swagger/index.html,需要launchSettings.json-->launchUrl=""
            });

            app.UseRouting();


            #region 配置身份认证和配置授权 Tips：注意先后顺序
            app.UseAuthentication();//身份认证
            app.UseAuthorization();//配置授权 
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
