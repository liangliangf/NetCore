using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MS.DbContexts;
using MS.UnitOfWork.UnitOfWork;
using MS.WebApi.Initialize;

using Autofac.Extensions.DependencyInjection;
using MS.WebCore.Logger;

namespace MS.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                using (IServiceScope scope = host.Services.CreateScope())
                {
                    //using MS.WebCore.Logger
                    //添加以上using引用
                    ///确保NLog.config中连接字符串与appsettings.json中同步
                    NLogExtensions.EnsureNlogConfig("NLog.config", "MySQL", scope.ServiceProvider.GetRequiredService<IConfiguration>().GetSection("ConectionStrings:MSDbContext").Value);

                    //初始化数据库
                    DBSeed.Initialize(scope.ServiceProvider.GetRequiredService<IUnitOfWork<MSDbContext>>());
                }
                host.Run();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .AddNlogService()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //替换为autofac为默认DI容器
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
