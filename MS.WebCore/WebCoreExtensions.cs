using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

namespace MS.WebCore
{
    public static class WebCoreExtensions
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// 添加跨域策略，从appsetting中读取配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, configurePolicy =>
                {
                    configurePolicy.WithOrigins(configuration["Startup:Cors:AllowOrigins"].Split(",")).AllowAnyHeader().AllowAnyMethod();
                });
            });
            return services;
        }


        public static IServiceCollection AddWebCoreService(this IServiceCollection services, IConfiguration configuration)
        {
            //绑定appsetting中的SiteSetting
            services.Configure<SiteSetting>(configuration.GetSection("SiteSetting")); //Microsoft.Extensions.Options.ConfigurationExtensions

            #region 单例化雪花算法 （实例化雪花算法IdWorker并注册为单例（保证全局只有一个单例））
            string workerIdStr = configuration["SiteSetting:WorkerId"];
            string dataCenterIdStr = configuration["SiteSetting:DatacenterId"];
            long workerId, dataCenterId;
            try
            {
                workerId = long.Parse(workerIdStr);
                dataCenterId = long.Parse(dataCenterIdStr);
            }
            catch (Exception)
            {

                throw;
            }
            Common.IDCode.IdWorker idWorker = new Common.IDCode.IdWorker(workerId, dataCenterId);
            services.AddSingleton(idWorker); 
            #endregion

            return services; 
        }
    }
}
