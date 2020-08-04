using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MS.Component.Aop;
using MS.Component.Jwt;
using MS.DbContexts;
using MS.Models.Automapper;
using MS.Services;
using MS.UnitOfWork;
using MS.WebApi.Filters;
using MS.WebApi.SwaggerHelp;
using MS.WebCore;
using MS.WebCore.MultiLanguages;

namespace MS.WebApi
{
    public class Startup
    {

        public ILifetimeScope AutofacContainer { get; set; }

        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public Startup(IWebHostEnvironment env)
        {
            // In ASP.NET Core 3.0 `env` will be an IWebHostingEnvironment, not IHostingEnvironment.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        //���autofac��DI��������
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //ע��IBaseService��IRoleService�ӿڼ���Ӧ��ʵ����
            //builder.RegisterType<BaseService>().As<IBaseService>().InstancePerLifetimeScope();
            //builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();


            //ע��aop������ 
            //��ҵ���������ƴ��˽�ȥ����ҵ���ӿں�ʵ������ע�ᣬҲ��ҵ�������������˴���
            builder.AddAopService(ServiceExtensions.GetAssemblyName());//ע��Auto�İ汾�����һ�£���������Aopע��ʧ��
        }

        public IConfiguration Configuration { get; }


        private string ApiName = "API��汾�ӿ�";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //��Ӷ����Ա��ػ�֧��
            services.AddMultiLanguages();

            services.AddControllers(config =>
            {
                //ע�����������������ȫ��Ӧ�ã����е�controller����Ӧ����������������
                //���ֻ�벿��Ӧ�ã����Կ���ʹ��Attribute���͵Ĺ�����
                config.Filters.Add<ApiExceptionFilter>();
                config.Filters.Add<ApiResultFilter>();
            })
                //���������ã��Ǹ�DataAnnotations���ػ���������ע��SharedResource�����ļ�������һ����ViewModel�е�ע��Ҳ����֧�ֶ������ˣ�
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));//��ע����ӱ��ػ���Դ�ṩ��Localizerprovider
                });

            //���swagger
            services.AddSwaggerGen(setupAction => {
                //�����汾��Ϣ
                typeof(SwaggerHelp.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    setupAction.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"{ApiName} �ӿ��ĵ�",
                        Description = $"{ApiName} HTTP API {version}",
                        Version = version,
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "�˼�������-����Ա", Url = new Uri("https://www.cnblogs.com/songl/"), Email = "2769376955@qq.com", },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense() { Name = "�˼�������-����Ա���֤", Url = new Uri("https://www.cnblogs.com/songl/") }
                    });
                });

                #region Swagger����JWT�����Խ�apikey�ӵ�����ͷ�У�
                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement() {
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
                setupAction.IncludeXmlComments(xmlPath);

            });

            //ע��Jwt����
            services.AddJwtServic(Configuration);


            //ע��������
            services.AddCorsPolicy(Configuration);

            //ע��webcore������վ��Ҫ���ã�
            services.AddWebCoreService(Configuration);

            //using MS.DbContexts;
            //using MS.UnitOfWork;
            //using Microsoft.EntityFrameworkCore;
            //������ӵ�using����
            services.AddUnitOfWorkService<MSDbContext>(options => { options.UseMySql(Configuration.GetSection("ConectionStrings:MSDbContext").Value); });

            //ע��automapper����
            services.AddAutoMapperService();


            #region Autofac ��Ҫע�͵�����ConfigureContainer �� ��autofac ע��
            //ע��IBaseService��IRoleService�ӿڼ���Ӧ��ʵ���� 
            //services.AddScoped<IBaseService, BaseService>();
            //services.AddScoped<IRoleService, RoleService>(); 
            #endregion

            
            //services.AddAopService(ServiceExtensions.GetAssemblyName());//ConfigureContainer�� Autofac ע��aop���������������Autofac�����������

            //ע������� Error�������ǲ��еģ���Ҫ�� AddControllers ��ע�������
            //services.AddSingleton<ApiResultFilter>();
            //services.AddSingleton<ApiExceptionFilter>();
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
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    option.SwaggerEndpoint($"swagger/{version}/swagger.json", $"{ApiName} {version}");
                });
                option.RoutePrefix = string.Empty;//swagger ���ʵ�ַǰ׺�����δ������Ϊswagger/index.html,��ҪlaunchSettings.json-->launchUrl=""
            });

            app.UseMultiLanguage(Configuration);//���ö�����

            app.UseRouting();

            app.UseCors(WebCoreExtensions.MyAllowSpecificOrigins);

            app.UseAuthentication();//�����֤�м��(Tip:��֤�м������Ȩ�м��ǰ��)

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        #region NetCore �Դ�����
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        //// This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();

        //    //ע��������
        //    services.AddCorsPolicy(Configuration);

        //    //ע��webcore������վ��Ҫ���ã�
        //    services.AddWebCoreService(Configuration);

        //    //using MS.DbContexts;
        //    //using MS.UnitOfWork;
        //    //using Microsoft.EntityFrameworkCore;
        //    //������ӵ�using����
        //    services.AddUnitOfWorkService<MSDbContext>(options => { options.UseMySql(Configuration.GetSection("ConectionStrings:MSDbContext").Value); });

        //    //ע��automapper����
        //    services.AddAutoMapperService();

        //    //ע��IBaseService��IRoleService�ӿڼ���Ӧ��ʵ����
        //    services.AddScoped<IBaseService, BaseService>();
        //    services.AddScoped<IRoleService, RoleService>();
        //}

        //// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseRouting();

        //    app.UseCors(WebCoreExtensions.MyAllowSpecificOrigins);

        //    app.UseAuthorization();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });
        //} 
        #endregion
    }
}
