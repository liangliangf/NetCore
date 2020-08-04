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

        private string ApiName = "API��汾�ӿ�";

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
                        Title = $"{ApiName} �ӿ��ĵ�",
                        Description = $"{ApiName} HTTP API {version}",
                        Version = version,
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "�˼�������-����Ա", Url = new Uri("https://www.cnblogs.com/songl/"), Email = "2769376955@qq.com", },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense() { Name = "�˼�������-����Ա���֤", Url = new Uri("https://www.cnblogs.com/songl/") }
                    });
                });


                #region Swagger����JWT�����Խ�apikey�ӵ�����ͷ�У�
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

            #region JwtBearer�����֤
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤������
                        ValidateAudience = true,//�Ƿ���֤������
                        ValidateIssuerSigningKey = true,//�Ƿ���֤��Կ
                        ValidateLifetime = true,//��֤��������
                        RequireExpirationTime = true,//����ʱ�� --
                        ValidIssuer = Configuration["Jwt:Issuer"],//������
                        ValidAudience = Configuration["Jwt:Audience"],//������
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),//��Կ
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
                option.RoutePrefix = string.Empty;//swagger ���ʵ�ַǰ׺�����δ������Ϊswagger/index.html,��ҪlaunchSettings.json-->launchUrl=""
            });

            app.UseRouting();


            #region ���������֤��������Ȩ Tips��ע���Ⱥ�˳��
            app.UseAuthentication();//�����֤
            app.UseAuthorization();//������Ȩ 
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
