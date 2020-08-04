using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MS.Models.Automapper
{
    //class AutoMapperServiceExtensions
    //{
    //}

    public static class ModelExtensions
    {
        /// <summary>
        /// 注册automapper服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapperService(this IServiceCollection services)
        {
            //将AutoMapper映射配置所在的程序集名称注册
            string executingAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            services.AddAutoMapper(Assembly.Load(executingAssembly));
            return services;
        }
    }
}
