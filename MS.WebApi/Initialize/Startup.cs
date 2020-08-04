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

        //添加autofac的DI配置容器
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //注册IBaseService和IRoleService接口及对应的实现类
            //builder.RegisterType<BaseService>().As<IBaseService>().InstancePerLifetimeScope();
            //builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();


            //注册aop拦截器 
            //将业务层程序集名称传了进去，给业务层接口和实现做了注册，也给业务层各方法开启了代理
            builder.AddAopService(ServiceExtensions.GetAssemblyName());//注意Auto的版本，最好一致，否再容易Aop注入失败
        }

        public IConfiguration Configuration { get; }


        private string ApiName = "API多版本接口";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加多语言本地化支持
            services.AddMultiLanguages();

            services.AddControllers(config =>
            {
                //注册过滤器，这样做是全局应用，所有的controller都会应用上面两个过滤器
                //如果只想部分应用，可以考虑使用Attribute类型的过滤器
                config.Filters.Add<ApiExceptionFilter>();
                config.Filters.Add<ApiResultFilter>();
            })
                //多语言配置（是给DataAnnotations本地化，依旧是注册SharedResource语言文件，这样一来，ViewModel中的注解也都能支持多语言了）
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));//给注解添加本地化资源提供器Localizerprovider
                });

            //添加swagger
            services.AddSwaggerGen(setupAction => {
                //遍历版本信息
                typeof(SwaggerHelp.ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    setupAction.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API {version}",
                        Version = version,
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "人间有妖气-管理员", Url = new Uri("https://www.cnblogs.com/songl/"), Email = "2769376955@qq.com", },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense() { Name = "人间有妖气-管理员许可证", Url = new Uri("https://www.cnblogs.com/songl/") }
                    });
                });

                #region Swagger启用JWT（可以将apikey加到请求头中）
                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
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

            //注册Jwt服务
            services.AddJwtServic(Configuration);


            //注册跨域策略
            services.AddCorsPolicy(Configuration);

            //注册webcore服务（网站主要配置）
            services.AddWebCoreService(Configuration);

            //using MS.DbContexts;
            //using MS.UnitOfWork;
            //using Microsoft.EntityFrameworkCore;
            //以上添加到using引用
            services.AddUnitOfWorkService<MSDbContext>(options => { options.UseMySql(Configuration.GetSection("ConectionStrings:MSDbContext").Value); });

            //注册automapper服务
            services.AddAutoMapperService();


            #region Autofac 需要注释掉，在ConfigureContainer 中 用autofac 注入
            //注册IBaseService和IRoleService接口及对应的实现类 
            //services.AddScoped<IBaseService, BaseService>();
            //services.AddScoped<IRoleService, RoleService>(); 
            #endregion

            
            //services.AddAopService(ServiceExtensions.GetAssemblyName());//ConfigureContainer中 Autofac 注册aop拦截器，如果不用Autofac，可以用这个

            //注册过滤器 Error：这样是不行的，需要在 AddControllers 中注册过滤器
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
                option.RoutePrefix = string.Empty;//swagger 访问地址前缀，如果未设置则为swagger/index.html,需要launchSettings.json-->launchUrl=""
            });

            app.UseMultiLanguage(Configuration);//启用多语言

            app.UseRouting();

            app.UseCors(WebCoreExtensions.MyAllowSpecificOrigins);

            app.UseAuthentication();//添加认证中间件(Tip:认证中间件在授权中间件前面)

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        #region NetCore 自带容器
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        //// This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();

        //    //注册跨域策略
        //    services.AddCorsPolicy(Configuration);

        //    //注册webcore服务（网站主要配置）
        //    services.AddWebCoreService(Configuration);

        //    //using MS.DbContexts;
        //    //using MS.UnitOfWork;
        //    //using Microsoft.EntityFrameworkCore;
        //    //以上添加到using引用
        //    services.AddUnitOfWorkService<MSDbContext>(options => { options.UseMySql(Configuration.GetSection("ConectionStrings:MSDbContext").Value); });

        //    //注册automapper服务
        //    services.AddAutoMapperService();

        //    //注册IBaseService和IRoleService接口及对应的实现类
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
